using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleScaler : MonoBehaviour
{
    [SerializeField] private Transform t;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x / t.localScale.x, transform.localScale.y / t.localScale.y, transform.localScale.z / t.localScale.z);
    }

}
