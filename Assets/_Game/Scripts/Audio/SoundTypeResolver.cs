using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundTypeResolver
{
    private readonly Dictionary<Enum, SoundType> map = new Dictionary<Enum, SoundType>();

    public SoundTypeResolver()
    {
        map.Clear();

        Register<MusicType>(SoundType.Music);
        Register<UISFX_Type>(SoundType.UISFX);
        Register<GamePlaySFX_Type>(SoundType.GameplaySFX);
        Register<AmbientType>(SoundType.Ambient);
    }

    private void Register<TEnum>(SoundType type)
        where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        var values = Enum.GetValues(enumType);

        if (values.Length == 0)
            throw new InvalidOperationException($"Enum {enumType.Name} has no values.");

        foreach (Enum value in values)
        {
            if (Convert.ToInt32(value) == 0) continue;

            if (map.TryGetValue(value, out var existing))
                throw new InvalidOperationException($"Enum {value} value from {enumType.Name} is already registered as {existing}");

            map.Add(value, type);
        }
    }
    public SoundType Resolve(Enum type)
    {
        if (type == null || !map.TryGetValue(type, out var soundType)) return SoundType.Null;

        return soundType;
    }


}
