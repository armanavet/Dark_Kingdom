using System;
using UnityEngine;

public interface ISoundProvider
{
    //AudioClip GetClip(SoundRequest request);
    AudioClip GetClip(Enum type);
} 