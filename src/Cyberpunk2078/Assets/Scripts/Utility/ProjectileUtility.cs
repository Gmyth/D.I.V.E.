using UnityEngine;


public struct ProjectileUtility
{
    public static Vector3 GetDeviatedBulletDirection(Vector3 initialDirection, float minAngle, float maxAngle, float deviation = 1)
    {
        if (deviation == 0)
            return initialDirection;


        float angle = (Random.Range(0, 1) * 2 - 1) * Random.Range(deviation * minAngle, deviation * maxAngle);


        Debug.LogWarning(deviation);


        return (Quaternion.Euler(0, 0, angle) * initialDirection).normalized;
    }
}
