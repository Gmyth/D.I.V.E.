using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformSync : MonoBehaviour
{

    public Transform player;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;
        player.localPosition = Vector3.zero;
    }

    public void PlayAnimation(string name) {
        player.GetComponent<Animator>().Play("MainCharacter_" + name);
    }

}
