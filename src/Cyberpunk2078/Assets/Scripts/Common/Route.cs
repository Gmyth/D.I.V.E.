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
    [SerializeField] private float[] stayTimes;


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


    public float GetStayTime(int index)
    {
        if (index < 0 || index >= stayTimes.Length)
            return 0;


        return stayTimes[index];
    }


    public IEnumerator GetEnumerator()
    {
        return wayPoints.GetEnumerator();
    }
}
