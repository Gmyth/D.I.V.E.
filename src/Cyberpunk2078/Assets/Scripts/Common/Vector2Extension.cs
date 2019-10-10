using UnityEngine;


public static class Vector2Extension
{
    public static Vector2 Left(this Vector2 v)
    {
        Vector2 result;

        result.y = v.x;
        result.x = -v.y;

        return result;
    }

    public static Vector2 Right(this Vector2 v)
    {
        Vector2 result;

        result.y = -v.x;
        result.x = v.y;

        return result;
    }
}
