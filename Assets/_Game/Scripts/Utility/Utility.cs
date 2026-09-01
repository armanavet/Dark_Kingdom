using System;
using UnityEngine;

public static class Utility
{
    public static int GetLayerFromMask(this LayerMask mask)
    {
        int value = mask.value;

        if (value == 0 || (value & (value - 1)) != 0)
        {
            return 0;
        }
        return Mathf.RoundToInt(MathF.Log(value, 2));
    }
}
