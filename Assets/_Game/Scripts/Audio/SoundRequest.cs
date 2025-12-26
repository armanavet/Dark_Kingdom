using System;

//public readonly struct SoundRequest
//{
//    public readonly Enum SoundType;
//    public readonly Enum SourceType;
//    public readonly bool HasSource;

//    private SoundRequest(Enum soundType, Enum sourceType, bool hasSource)
//    {
//        SoundType = soundType;
//        SourceType = sourceType;
//        HasSource = hasSource;
//    }
//    //Source-free sounds like UI, Music, Ambient and Gameplay sfx-es several types.
//    public static SoundRequest Sound(Enum soundType)
//    {
//        return new SoundRequest(soundType, null, false);
//    }
//    //Source-based sounds like gameplay enemies and towers sfx-es. 
//    public static SoundRequest SoundWithSource(Enum soundType, Enum sourceType)
//    {
//        if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
//        return new SoundRequest(soundType, sourceType, true);
//    }

//}
