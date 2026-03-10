using System;
using UnityEngine;

public interface ISoundProvider
{
    //AudioClip GetClip(SoundRequest request);
    Type SoundType { get; }
    AudioClip GetClip(Enum type);
}