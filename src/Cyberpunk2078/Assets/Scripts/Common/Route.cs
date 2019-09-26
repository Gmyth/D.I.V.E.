using System;
using System.Collections;
using UnityEngine;


public class PathAttribute : PropertyAttribute
{
    public readonly bool isLooping;


    public PathAttribute(bool isLooping = false)
    {
        this.isLooping = isLooping;
    }
}


[Serializable] public class Route : IEnumerable
{
    [SerializeField] private Vector3[] wayPoints;


    public Vector3 this[int index]
    {
        get
        {
            return wayPoints[index];
        }

        set
        {
            wayPoints[index] = value;
        }
    }


    public int NumWayPoints
    {
        get
        {
            return wayPoints.Length;
        }
    }


    public IEnumerator GetEnumerator()
    {
        return wayPoints.GetEnumerator();
    }
}
