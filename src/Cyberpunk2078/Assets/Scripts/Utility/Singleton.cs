using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
        {
            Instance = FindObjectOfType<T>();
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
