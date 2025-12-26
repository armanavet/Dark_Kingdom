using AudioSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundClipSet<TSound>
    where TSound : Enum
{
    private readonly Dictionary<TSound, AudioClip[]> dictionary = new();

    public void BuildDictionary(IEnumerable<SoundEntry<TSound>> clipCollector)
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

    public AudioClip GetClip(TSound soundType)
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
}

public abstract class SoundLibrary<TSound> : ScriptableObject, ISoundProvider
    where TSound : Enum
{
    [SerializeField] public List<SoundEntry<TSound>> Entries;

    SoundClipSet<TSound> clipsDictionary;

    public virtual void OnEnable()
    {
        Validate(); // VAlidation warns about a data issues.
        Build(); // Build always succeeds using last-write-wins policy.
    }
    void Validate()
    {
        if (Entries == null || Entries.Count == 0)
        {
            Debug.LogWarning($"[{name}] has no clip entries", this);
            return;
        }

        var keySet = new HashSet<TSound>();

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
        clipsDictionary = new SoundClipSet<TSound>();
        clipsDictionary.BuildDictionary(Entries);
    }
    public AudioClip GetClip(Enum type)
    {
        #region Error Check 
        if (type is not TSound soundType)
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
}




[CreateAssetMenu(fileName = "SoundBank", menuName = "ScriptableObjects/SoundBank")]
public class SoundBank : ScriptableObject
{
    public Music musicSO;
    public UISoundEffects uiSoundEffectsSO;
    public GameplaySoundEffects gameplaySoundEffectsSO;
    public List<SoEntry<UnitType>> enemySoundEffectsSO;
    public List<SoEntry<TowerType>> towerSoundEffectsSO;

    Dictionary<UnitType, GameplaySoundEffects> enemySoundDict = new Dictionary<UnitType, GameplaySoundEffects>();
    Dictionary<TowerType, GameplaySoundEffects> towerSoundDict = new Dictionary<TowerType, GameplaySoundEffects>();

    void OnEnable()
    {
        Build();
    }
    void Build()
    {
        foreach (var item in enemySoundEffectsSO)
        {
            enemySoundDict.Add(item.Type, item.Entry);
        }
        foreach (var item in towerSoundEffectsSO)
        {
            towerSoundDict.Add(item.Type, item.Entry);
        }
    }
    public AudioClip GetClip<TSoundType>(TSoundType soundType) 
        where TSoundType : Enum
    {
        if (soundType is UISFX_Type uiSFX_Type)
        {
            return uiSoundEffectsSO.GetClip(uiSFX_Type);
        }
        if (soundType is MusicType musicType)
        {
            return musicSO.GetClip(musicType);
        }
        if (soundType is GamePlaySFX_Type gamePlaySFX_Type)
        {
            return gameplaySoundEffectsSO.GetClip(gamePlaySFX_Type);
        }
        Debug.LogWarning($"I got the sound type '{soundType}'");
        return null;
    }
    public AudioClip GetClip<TSourceType, TSoundType>(TSourceType sourceType, TSoundType soundType)
        where TSourceType : Enum
        where TSoundType : Enum
    {
        if (soundType is GamePlaySFX_Type gamePlaySFX_Type)
        {
            if (sourceType is UnitType unitType && enemySoundDict.TryGetValue(unitType, out var enemy))
            {
                return enemy.GetClip(gamePlaySFX_Type);
            }
            if (sourceType is TowerType towerType && towerSoundDict.TryGetValue(towerType, out var tower))
            {
                return tower.GetClip(gamePlaySFX_Type);
            }
        }
        Debug.LogWarning($"I got the source '{sourceType}' and the sound '{soundType}'");
        return null;
    }
}





//public abstract class OwnerSoundLibrary<TSource, TSound> : ScriptableObject, ISoundProvider
//    where TSource : Enum
//    where TSound : Enum
//{
//    [SerializeField] protected List<SourceSoundEntry<TSource, TSound>> Libraries;

//    Dictionary<TSource, SoundClipSet<TSound>> library;

//    public virtual void OnEnable()
//    {
//        Validate(); // VAlidation warns about a data issues.
//        Build(); // Build always succeeds using last-write-wins policy.
//    }

//    void Validate()
//    {
//        if (Libraries == null || Libraries.Count == 0)
//        {
//            Debug.LogWarning($"[{name}] has no libraris assigned.", this);
//            return;
//        }
//        var ownerSet = new HashSet<TSource>();

//        foreach (var ownerEntry in Libraries)
//        {
//            if (!ownerSet.Add(ownerEntry.OwnerType))
//            {
//                Debug.LogWarning($"[{name}] Dublicate owner type, '{ownerEntry.OwnerType}'", this);
//                continue;
//            }
//            if (ownerEntry.OwnerClips == null || ownerEntry.OwnerClips.Length == 0)
//            {
//                Debug.LogWarning($"[{name}] Owner '{ownerEntry.OwnerType}' has no clip entries.", this);
//                continue;
//            }
//            ValidateOwnerClips(ownerEntry);
//        }
//    }
//    void ValidateOwnerClips(SourceSoundEntry<TSource, TSound> ownerEntry)
//    {
//        var clipTypeSet = new HashSet<TSound>();

//        foreach (var clipEntry in ownerEntry.OwnerClips)
//        {
//            if (!clipTypeSet.Add(clipEntry.Type))
//            {
//                Debug.LogWarning(
//                    $"[{name}] Duplicate clip type '{clipEntry.Type}' " +
//                    $"for owner '{ownerEntry.OwnerType}'.",
//                    this
//                );
//                continue;
//            }

//            if (clipEntry.Clips == null || clipEntry.Clips.Length == 0)
//            {
//                Debug.LogWarning(
//                    $"[{name}] Clip type '{clipEntry.Type}' for owner " +
//                    $"'{ownerEntry.OwnerType}' has no AudioClips assigned.",
//                    this
//                );
//                continue;
//            }

//            for (int i = 0; i < clipEntry.Clips.Length; i++)
//            {
//                if (clipEntry.Clips[i] == null)
//                {
//                    Debug.LogWarning(
//                        $"[{name}] Null AudioClip at index {i} in clip type " +
//                        $"'{clipEntry.Type}' for owner '{ownerEntry.OwnerType}'.",
//                        this
//                    );
//                }
//            }
//        }
//    }
//    void Build()
//    {
//        library = new Dictionary<TSource, SoundClipSet<TSound>>();

//        foreach (var entry in Libraries)
//        {
//            var clipsDictionary = new SoundClipSet<TSound>();
//            clipsDictionary.BuildDictionary(entry.OwnerClips);
//            library.Add(entry.OwnerType, clipsDictionary);
//        }
//    }
//    public AudioClip GetClip(SoundRequest request)
//    {
//        #region Error Check 
//        if (!request.HasSource)
//        {
//            Debug.LogError($"[{name}] requires only source - based request.", this);
//            return null;
//        }
//        if (request.SourceType is not TSource sourceType)
//        {
//            Debug.LogError($"[{name}] invalid source type '{request.SourceType}'.", this);
//            return null;
//        }
//        if (request.SoundType is not TSound soundType)
//        {
//            Debug.LogError($"[{name}] invalid clip type '{request.SoundType}'.", this);
//            return null;
//        }
//        if (!library.TryGetValue(sourceType, out var clipsDictionary))
//        {
//            Debug.LogError($"[{name}] No clips registered for source '{sourceType}'.", this);
//            return null;
//        }
//        #endregion
//        return clipsDictionary.GetClip(soundType);
//    }
//}

//---------------------
//{
//[SerializeField] 
//public List<SoundProviderEntry> Providers;
//Dictionary<SoundType, ISoundProvider> providerMap = new Dictionary<SoundType, ISoundProvider>();
//void OnEnable()
//{
//    Build();    
//}
//void Build()
//{
//    providerMap.Clear();
//    foreach (var provider in Providers)
//    {
//        if(provider.ClipsLibrary == null)
//        {
//            throw new ArgumentNullException(nameof(provider.ClipsLibrary));
//        }

//        var soundProvider = provider.ClipsLibrary as ISoundProvider;
//        if (soundProvider == null)
//        {
//            Debug.LogError(
//                $"[{name}] '{provider.ClipsLibrary.name}' does not implement ISoundProvider.",
//                this
//            );
//            continue;
//        }
//        if (providerMap.ContainsKey(provider.Type))
//        {
//            Debug.LogWarning($"[{name}] Duplicate type '{provider.Type}'.",this);
//            continue;
//        }
//        if (!providerMap.TryAdd(provider.Type, soundProvider))
//        {
//            Debug.LogWarning($"[{name}] Duplicate SoundType '{provider.Type}'.", this);
//        }
//    }
//}

//public AudioClip GetClip(Enum type, SoundRequest request)
//{
//    if (type is not SoundType soundType) 
//    {
//        Debug.LogError($"[{name}] is received wrong type, '{type}'");
//        return null;
//    }
//    if (Convert.ToInt32(type) == 0)
//    {
//        Debug.LogWarning($"[{name}] SoundType has null type, '{type}'", this);
//        return null;
//    }

//    if (!providerMap.TryGetValue(soundType, out var provider))
//    {
//        Debug.LogError($"[{name}] No ISoundProvider found for SoundType '{request.SoundType}'.", this);
//        return null;
//    }

//    return provider.GetClip(request);
//}
//}


