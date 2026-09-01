using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorOperations
{
   public static bool PointsLineUp(Vector2Int first, Vector2Int second)
    {
        return first.x == second.x || first.y == second.y;
    }
}
