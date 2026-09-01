using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundClipSet<T>
    where T : Enum
{
    private readonly Dictionary<T, AudioClip[]> dictionary = new();

    public void BuildDictionary(IEnumerable<SoundEntry<T>> clipCollector)
    {
        dictionary.Clear();
        if (clipCollector == null)
        {
            Debug.LogError($"ClipDictionary received null clip collection.");
            return;
        }
        foreach (var entry in clipCollector)
        {
            #region Check
            if (entry.Clips == null || entry.Clips.Length == 0)
            {
                Debug.LogWarning($"Clip type '{entry.Type}' has no AudioClips assigned. Skipped");
                continue;
            }
            for (int i = 0; i < entry.Clips.Length; i++)
            {
                if (entry.Clips[i] == null)
                {
                    Debug.LogWarning($"Null AudioClip in '{entry.Type}' at index {i}");
                }
            }
            if (dictionary.ContainsKey(entry.Type))
            {
                Debug.LogWarning(
                    $"Duplicate clip type '{entry.Type}' detected. Overwriting previous entry."
                );
            }
            #endregion
            dictionary[entry.Type] = entry.Clips;
        }
    }

    public AudioClip GetClip(T soundType)
    {
        #region Error Check 
        if (!dictionary.TryGetValue(soundType, out var clips))
        {
            Debug.LogError($"{soundType} type audio clip not found!");
            return null;
        }
        #endregion
        return clips[Random.Range(0, clips.Length)];
    }
    public AudioClip GetClip(T soundType, int index)
    {
        #region Error Check 
        if (!dictionary.TryGetValue(soundType, out var clips))
        {
            Debug.LogError($"{soundType} type audio clip not found!");
            return null;
        }
        #endregion
        return clips[index];
    }
}

public abstract class SoundLibrary<T> : ScriptableObject, ISoundProvider
    where T : Enum
{
    [SerializeField] public List<SoundEntry<T>> Entries;

    SoundClipSet<T> clipsDictionary;

    public Type SoundType => typeof(T);
    public virtual void OnEnable()
    {
        Validate(); // Validation warns about a data issues.
        Build(); // Build always succeeds using last-write-wins policy.
    }
    void Validate()
    {
        if (Entries == null || Entries.Count == 0)
        {
            Debug.LogWarning($"[{name}] has no clip entries", this);
            return;
        }

        var keySet = new HashSet<T>();

        foreach (var entry in Entries)
        {
            if (!keySet.Add(entry.Type))
            {
                Debug.LogWarning($"[{name}] Duplicate clip type '{entry.Type}'.", this);
            }
            if (entry.Clips == null || entry.Clips.Length == 0)
            {
                Debug.LogWarning($"[{name}] Clip type '{entry.Type}' has no AudioClips.", this);
                continue;
            }
            for (int i = 0; i < entry.Clips.Length; i++)
            {
                if (entry.Clips[i] == null)
                {
                    Debug.LogWarning(
                        $"[{name}] Null AudioClip at index {i} " +
                        $"for clip type '{entry.Type}'.",
                        this
                    );
                }
            }
        }
    }
    void Build()
    {
        clipsDictionary = new SoundClipSet<T>();
        clipsDictionary.BuildDictionary(Entries);
    }
    public AudioClip GetClip(Enum type)
    {
        #region Error Check 
        if (type is not T soundType)
        {
            Debug.LogError($"[{name}] invalid sound type '{type}'.", this);
            return null;
        }
        if (clipsDictionary == null)
        {
            Debug.LogError(
                $"[{name}] is not initialized.",
                this
            );
            return null;
        }
        #endregion
        return clipsDictionary.GetClip(soundType);
    }
    public AudioClip GetClip(Enum type, int index)
    {
        #region Error Check 
        if (type is not T soundType)
        {
            Debug.LogError($"[{name}] invalid sound type '{type}'.", this);
            return null;
        }
        if (clipsDictionary == null)
        {
            Debug.LogError(
                $"[{name}] is not initialized.",
                this
            );
            return null;
        }
        #endregion
        return clipsDictionary.GetClip(soundType, index);
    }
}




[CreateAssetMenu(fileName = "SoundBank", menuName = "ScriptableObjects/SoundBank")]
public class SoundBank : ScriptableObject
{
    public MusicLibrary music;
    public UISoundLibrary ui;
    public GameplaySoundLibrary gameplay;
    public List<SoundSOEntry<UnitType>> enemy;
    public List<SoundSOEntry<TowerType>> tower;

    Dictionary<UnitType, GameplaySoundLibrary> enemyDict = new Dictionary<UnitType, GameplaySoundLibrary>();
    Dictionary<TowerType, GameplaySoundLibrary> towerDict = new Dictionary<TowerType, GameplaySoundLibrary>();
    Dictionary<Type, ISoundProvider> providerMap = new();

    public void Build()
    {
        enemyDict.Clear();
        towerDict.Clear();

        foreach (var item in enemy)
        {
            enemyDict.Add(item.Type, item.Entry);
        }
        foreach (var item in tower)
        {
            towerDict.Add(item.Type, item.Entry);
        }
        providerMap.Clear();
        Register(music);
        Register(ui);
        Register(gameplay);
    }
    public void Register(ISoundProvider provider)
    {
        providerMap[provider.SoundType] = provider;
    }

    public AudioClip GetClip<TSoundType>(TSoundType soundType)
    where TSoundType : Enum
    {
        var enumType = typeof(TSoundType);

        if (!providerMap.TryGetValue(enumType, out var provider))
        {
            Debug.LogError($"No sound provider registered for {enumType}");
            return null;
        }

        return provider.GetClip(soundType);
    }
    public AudioClip GetClip<TSourceType, TSoundType>(TSourceType sourceType, TSoundType soundType)
        where TSourceType : Enum
        where TSoundType : Enum
    {
        if (soundType is GamePlaySFX_Type gamePlaySFX_Type)
        {
            if (sourceType is UnitType unitType && enemyDict.TryGetValue(unitType, out var enemy))
            {
                return enemy.GetClip(gamePlaySFX_Type);
            }
            if (sourceType is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
            {
                return tower.GetClip(gamePlaySFX_Type);
            }
        }
        Debug.LogWarning($"I got the source '{sourceType}' and the sound '{soundType}'");
        return null;
    }
    public AudioClip GetClip<TSourceType, TSoundType>(TSourceType sourceType, TSoundType soundType, int index)
        where TSourceType : Enum
        where TSoundType : Enum
    {
        if (soundType is GamePlaySFX_Type gamePlaySFX_Type)
        {
            if (sourceType is UnitType unitType && enemyDict.TryGetValue(unitType, out var enemy))
            {
                return enemy.GetClip(gamePlaySFX_Type, index);
            }
            if (sourceType is TowerType towerType && towerDict.TryGetValue(towerType, out var tower))
            {
                return tower.GetClip(gamePlaySFX_Type, index);
            }
        }
        Debug.LogWarning($"I got the source '{sourceType}' and the sound '{soundType}'");
        return null;
    }
}
