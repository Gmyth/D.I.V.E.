/**
 * @author SerapH
 */

using System;
using UnityEngine;


public struct MathUtility
{
    public static readonly float sqrt2 = Mathf.Sqrt(2);


    public static int ManhattanDistance(int xA, int yA, int xB, int yB)
    {
        return Math.Abs(xA - xB) + Math.Abs(yA - yB);
    }

    public static float ManhattanDistance(float xA, float yA, float xB, float yB)
    {
        return Mathf.Abs(xA - xB) + Mathf.Abs(yA - yB);
    }

    public static float EuclideanDistance(int xA, int yA, int xB, int yB)
    {
        int dx = xA - xB;
        int dy = yA - yB;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static float EuclideanDistance(float xA, float yA, float xB, float yB)
    {
        float dx = xA - xB;
        float dy = yA - yB;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static int ChebyshevDistance(int xA, int yA, int xB, int yB)
    {
        return Math.Max(Math.Abs(xA - xB), Math.Abs(yA - yB));
    }

    public static int ManhattanDistance(int xA, int yA, int zA, int xB, int yB, int zB)
    {
        return Math.Abs(xA - xB) + Math.Abs(yA - yB) + Math.Abs(zA - zB);
    }

    public static float EuclideanDistance(int xA, int yA, int zA, int xB, int yB, int zB)
    {
        int dx = xA - xB;
        int dy = yA - yB;
        int dz = zA - zB;

        return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static int ChebyshevDistance(int xA, int yA, int zA, int xB, int yB, int zB)
    {
        return Math.Max(Math.Abs(xA - xB), Math.Max(Math.Abs(yA - yB), Math.Abs(zA - zB)));
    }

    public static Vector3 GetInitialVelocityForParabolaMovement(Vector3 initialPosition, Vector3 targetPosition, float time, float g)
    {
        return new Vector3((targetPosition.x - initialPosition.x) / time, (targetPosition.y - initialPosition.y) / time + 0.5f * g * time, (targetPosition.z - initialPosition.z) / time);
    }
}
