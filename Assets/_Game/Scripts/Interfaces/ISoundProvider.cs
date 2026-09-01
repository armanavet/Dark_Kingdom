using System;
using UnityEngine;

public interface ISoundProvider
{
    Type SoundType { get; }
    AudioClip GetClip(Enum type);
}