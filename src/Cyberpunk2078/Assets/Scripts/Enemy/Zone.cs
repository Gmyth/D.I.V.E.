using System;
using UnityEngine;


public enum ZoneType
{
    Rectangle,
    Circle,
}


[Serializable] public class Zone
{
    [SerializeField] private ZoneType type;
    public Vector2 center;
    [SerializeField][Min(0f)] private float w;
    [SerializeField][Min(0f)] private float h;


    public ZoneType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;

            if (type == ZoneType.Circle)
            {
                float v = Mathf.Max(w, h);

                w = v;
                h = v;
            }
        }
    }

    public float Width
    {
        get
        {
            return w;
        }

        set
        {
            w = value;

            if (type == ZoneType.Circle)
                h = value;
        }
    }

    public float Height
    {
        get
        {
            return h;
        }

        set
        {
            h = value;

            if (type == ZoneType.Circle)
                w = value;
        }
    }

    public float Radius
    {
        get
        {
            return w / 2f;
        }

        set
        {
            w = 2f * value;
            h = 2f * value;
        }
    }


    public bool Contains(Vector3 position)
    {
        if (w <= 0 || h <= 0)
            return true;


        switch (type)
        {
            case ZoneType.Rectangle:
                return Mathf.Abs(position.x - center.x) <= w / 2f && Mathf.Abs(position.y - center.y) <= h / 2f;


            case ZoneType.Circle:
                return Vector2.Distance(center, position) <= Radius;
        }

        return false;
    }

    public bool Contains(Dummy dummy)
    {
        return Contains(dummy.transform.position);
    }
}
