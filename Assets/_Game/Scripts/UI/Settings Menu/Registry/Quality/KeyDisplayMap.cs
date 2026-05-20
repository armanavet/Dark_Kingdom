using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class KeyDisplayMap
{
    private static readonly Dictionary<Key, string> map = new()
    {
        {Key.W, "W" },
        {Key.A, "A" },
        {Key.S, "S" },
        {Key.D, "D" },
    };

    public static string Get(Key key)
    {
        if(map.TryGetValue(key, out var value))
            return value;
        
        return key.ToString();
    }
}
