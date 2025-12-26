using UnityEngine;
using System;

[Serializable]
public struct SoundEntry<TSound>
{
    public TSound Type;
    public AudioClip[] Clips;
}
[Serializable]
public struct SoEntry<TType> where TType : Enum
{
    public TType Type;
    public GameplaySoundEffects Entry;
}
//[Serializable]
//public struct SourceSoundEntry<TSource, TSound>
//{
//    public TSource OwnerType;
//    public SoundEntry<TSound>[] OwnerClips;
//}
//[Serializable]
//public struct SoundProviderEntry
//{
//    public SoundType Type;
//    //[SerializeReference] public ISoundProvider ClipsLibrary;
//    public ScriptableObject ClipsLibrary;
//}

////--------------------------
//[Serializable]
//public struct SoundClipEntry<TSoundType>
//{
//    public TSoundType Type;
//    public AudioClip[] Clips;
//}
//[Serializable]
//public struct SourcetenantClipEntry<TTenant, TSoundType>
//{
//    public TTenant TenantType;
//    public SoundEntry<TSoundType>[] TenantClips;
//}
////--------------------------
//[Serializable]
//public struct SoundDataEntry<TSoundCategory, TSoundType>
//{
//    public TSoundCategory Category;
//    public SoundClipEntry<TSoundType>[] Data;
//}
//[Serializable]
//public struct SoundSourceDataEntry<TSoundCategory, TTenant, TSoundType>
//{
//    public TSoundCategory Category;
//    public SourcetenantClipEntry<TTenant, TSoundType>[] Data;
//}
