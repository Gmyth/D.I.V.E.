using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformSync : MonoBehaviour
{

    public Transform player;
    public void Awake()
    {      
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;
        player.localPosition = Vector3.zero;
    }

    public void PlayAnimation(string name) {
        player.GetComponentInChildren<Animator>().Play("MainCharacter_" + name);
    }

}
