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

    public void ChargeEnergy() 
    {
        Player.CurrentPlayer.AddOverLoadEnergy(1);
        Drain();
    }

    public void Drain()
    {
        isReady = false;
        GetComponent<SpriteRenderer>().color = Color.black;
        StartCoroutine(Recover());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "PlayerAttack") 
        {
            if (isReady)
            {
                ChargeEnergy();
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
