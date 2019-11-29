using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleChargePoint : MonoBehaviour
{
    public float RecoverTime = 2.0f;
    public bool isReady = true;

    private Color chargedColor;
    

    // Start is called before the first frame update
    void Start()
    {
        chargedColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnergyCharge() 
    {
        PlayerCharacter.Singleton.AddOverLoadEnergy(1);
        OnDrain();
    }

    public void OnDrain()
    {
        isReady = false;
        GetComponent<SpriteRenderer>().color = Color.black;

        StartCoroutine(Recover());
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("Charge point collide with " + other.gameObject.name);

        if (isReady)
        {
            if (other.tag == "Player")
            {
                PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();

                if (playerCharacter[StatisticType.Osp] <= 0)
                    OnEnergyCharge();
            }
            else if (other.tag == "PlayerHitBox")
            {
                PlayerCharacter playerCharacter = PlayerCharacter.Singleton;

                if (playerCharacter[StatisticType.Osp] <= 0)
                    OnEnergyCharge();
            }
        }
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(RecoverTime);
        isReady = true;
        GetComponent<SpriteRenderer>().color = chargedColor;
    }
}
