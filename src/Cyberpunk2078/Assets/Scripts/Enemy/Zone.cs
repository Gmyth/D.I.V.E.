using System;
using UnityEngine;


public enum ZoneType
{
    Rectangle,
    Sphere,
}


[Serializable] public struct Zone
{
    public ZoneType type;
    public Vector2 center;
    [SerializeField][Min(0f)] private float w;
    [SerializeField][Min(0f)] private float h;


    public bool Contains(Vector3 position)
    {
        if (w <= 0)
            return true;


        switch (type)
        {
            case ZoneType.Rectangle:
                return Mathf.Abs(position.x - center.x) <= w && Mathf.Abs(position.y - center.y) <= h;


            case ZoneType.Sphere:
                return Vector2.Distance(center, position) <= w;
        }

        return false;
    }

    public bool Contains(Dummy dummy)
    {
        return Contains(dummy.transform.position);
    }
}
