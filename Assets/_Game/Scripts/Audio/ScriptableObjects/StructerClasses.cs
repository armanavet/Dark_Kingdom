using UnityEngine;
using System;
using AudioSystem;

[Serializable]
public struct SoundEntry<T>
{
    public T Type;
    public AudioClip[] Clips;
}
[Serializable]
public struct SoundSOEntry<T> where T : Enum
{
    public T Type;
    public GameplaySoundLibrary Entry;
}
[Serializable]
public struct SourceEntry<T>
{
    public T Type;
    public SoundData Data;
}
[Serializable]
public struct SourceSOEntry<T> where T : Enum
{
    public T Type;
    public SourceDataSO Entry;
}

