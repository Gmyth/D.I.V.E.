using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : MonoBehaviour
{
    public float RecoverTime = 2.0f;
    public Drone[] drone;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(RecoverTime);
        gameObject.SetActive(true);
    }
}
