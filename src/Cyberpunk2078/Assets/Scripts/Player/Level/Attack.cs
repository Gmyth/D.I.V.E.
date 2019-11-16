using System.Collections.Generic;
using UnityEngine;


public class Attack : MonoBehaviour
{
    [SerializeField] private float damage = 1;
    [SerializeField] private int[] effectList;
    public bool isFriendly = false;


    HashSet<int> objectsHit = new HashSet<int>();
    

    public void  setDamage(float amount)
    {
        damage = amount;
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                CameraManager.Instance.Shaking(0.20f,0.10f);
                var VFX = ObjectRecycler.Singleton.GetObject<SingleEffect>(getRandomEffect());
                VFX.transform.position =
                    other.transform.position - (other.transform.position - transform.position) * 0.2f;
                VFX.transform.right = transform.right;
                VFX.setTarget(other.transform);
                VFX.transform.localScale = new Vector3(4,1,1);
                VFX.gameObject.SetActive(true);

                other.GetComponent<Dummy>().ApplyDamage(damage);
                objectsHit.Add(other.gameObject.GetInstanceID());
            }
            else if (other.tag == "Platform" && other.GetComponent<SimpleBreakable>())
            {
                other.GetComponent<SimpleBreakable>().DestoryBreakable();
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Name != "Dash")
            {
                ObjectRecycler.Singleton.GetObject<SingleEffect>(getRandomEffect());

                other.GetComponent<PlayerCharacter>().ApplyDamage(damage);
                objectsHit.Add(other.gameObject.GetInstanceID());
            }
        }
        


    }

    public int getRandomEffect()
    {
        if (effectList.Length > 1)
        {
            return effectList[Random.Range(0, effectList.Length)];
        }

        return effectList[0];
    }


    private void OnEnable()
    {
        objectsHit.Clear();
    }
}
