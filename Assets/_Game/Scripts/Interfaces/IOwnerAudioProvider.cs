using System;
using UnityEngine;

public interface IOwnerTypeProvider
{
    AudioClip GetClip(Enum ownerType, Enum soundType);
}
