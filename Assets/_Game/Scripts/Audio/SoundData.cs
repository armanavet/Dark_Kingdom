using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    [Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;

        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffetcs;
        public bool bypassReverbZones;

        public int priority = 128;
        public float volume = 1f;
        public float pitch = 1f;
        public float panStereo;
        public float spatialBlend;
        public float reverbZoneMix = 1f;
        public float dopplerLevel = 1f;
        public float spread;

        public float minDistance = 1f;
        public float maxDistance = 500f;

        public bool ignoreListenerVolume;
        public bool ignoreListenerPause;

        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    }
    public enum SoundDataType
    {
        Enemy,
        EnemyProjectile,
        Tower,
        UI,
        Music
    }
    [Serializable]
    public class SoundDataStorage
    {
        public SoundDataType type;
        public SoundData data;
    }
    [Serializable]
    public class SoundDataRepository
    {
        public SoundDataStorage[] soundDataStorage;
        Dictionary<SoundDataType, SoundDataStorage> soundDataDict = new Dictionary<SoundDataType, SoundDataStorage>();

        public void Organize()
        {
            FillSelectedDictionary.Fill(soundDataDict, soundDataStorage, type => type.type);
        }
        public SoundData GetSoundDataByType(SoundDataType soundDataType)
        {
            if (!soundDataDict.TryGetValue(soundDataType, out var item))
            {
                Debug.LogError($"SoundData for {soundDataType} not found!");
                return null;
            }
            return item.data;
        }
    }
}
//public class SoundData_Enemy
//{
//    public bool mute = false;
//    public bool bypassEffects = false;
//    public bool bypassListenerEffetcs = false;
//    public bool bypassReverbZones = false;
//    public bool loop = false;
//    public bool playOnAwake = false;

//    public bool frequentSound = false;

//    public int priority = 128;
//    public float volume = 0.7f;
//    public float pitch = 1f;
//    public float panStereo = 0;
//    public float spatialBlend = 1f;
//    public float reverbZoneMix = 1f;

//    public float dopplerLevel = 0f;
//    public float spread = 30f;
//    public float minDistance = 10f;
//    public float maxDistance = 30f;
//    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

//    public bool ignoreListenerVolume;
//    public bool ignoreListenerPause;

//}
//public class SoundData_Tower
//{
//    public bool mute = false;
//    public bool bypassEffects = false;
//    public bool bypassListenerEffetcs = false;
//    public bool bypassReverbZones = false;
//    public bool loop = false;
//    public bool playOnAwake = false;

//    public bool frequentSound = false;

//    public int priority = 128;
//    public float volume = 0.8f;
//    public float pitch = 1f;
//    public float panStereo = 0;
//    public float spatialBlend = 1f;
//    public float reverbZoneMix = 1f;

//    public float dopplerLevel = 0f;
//    public float spread = 30f;
//    public float minDistance = 10f;
//    public float maxDistance = 30f;
//    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

//    public bool ignoreListenerVolume;
//    public bool ignoreListenerPause;
//}

