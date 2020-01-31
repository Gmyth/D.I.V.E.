using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : MonoBehaviour
{
    public float RecoverTime = 2.0f;
    public Drone[] drones;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Drone drone in drones)
        {
            drone.dead.AddListener(DroneDead);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Recover(Drone drone)
    {
        yield return new WaitForSeconds(RecoverTime);
        drone.gameObject.SetActive(true);
    }

    public void DroneDead(Drone drone)
    {
        StartCoroutine(Recover(drone));
    }
}
