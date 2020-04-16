using System.Collections;
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

    public void OnEnergyCharge(bool Overload = false)
    {
        if (Overload) PlayerCharacter.Singleton.AddOverLoadEnergy(1);
        else PlayerCharacter.Singleton.AddNormalEnergy(1);
        GetComponent<SpriteRenderer>().color = Color.gray;
        OnDrain();
    }

    public void OnDrain()
    {
        isReady = false;

        GetComponent<Animator>().Play("ChargePointActive",0,0);
        StartCoroutine(Recover());
        GetComponentInChildren<ParticleSystem>().Play();
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("Charge point collide with " + other.gameObject.name);

        if (isReady)
        {
            if (other.tag == "Player")
            {
                PlayerCharacter playerCharacter = PlayerCharacter.Singleton;
                if (playerCharacter[StatisticType.Sp] <= 0)
                    OnEnergyCharge(false);
                
                else if (playerCharacter[StatisticType.Osp] <= 0)
                    OnEnergyCharge(true);
            }
            else if (other.tag == "PlayerHitBox")
            {
                PlayerCharacter playerCharacter = PlayerCharacter.Singleton;

                if (playerCharacter[StatisticType.Sp] <= 0)
                    OnEnergyCharge(false);
                
                else if (playerCharacter[StatisticType.Osp] <= 0)
                    OnEnergyCharge(true);
            }
        }
    }

    private IEnumerator Recover()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        yield return new WaitForSeconds(RecoverTime);
        isReady = true;
        GetComponent<SpriteRenderer>().color = chargedColor;
        GetComponent<Animator>().Play("ChargePointMuted",0,0);
    }
}
