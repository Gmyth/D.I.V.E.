using System.Collections.Generic;
using UnityEngine;


public class Attack : MonoBehaviour
{
    [SerializeField] private float damage = 1;
    [SerializeField] private int[] effectList;
    public bool isFriendly = false;


    HashSet<int> objectsHit = new HashSet<int>();



    private void Start()
    {



    }

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
                CameraManager.Instance.Shaking(0.20f, 0.10f);


                var VFX = ObjectRecycler.Singleton.GetObject<SingleEffect>(getRandomEffect());
                VFX.transform.position = other.transform.position - (other.transform.position - transform.position) * 0.2f;
                VFX.transform.right = transform.right;
                VFX.setTarget(other.transform);
                VFX.transform.localScale = new Vector3(4,1,1);
                VFX.gameObject.SetActive(true);

                var trail = ObjectRecycler.Singleton.GetObject<SingleEffect>(8);
                trail.transform.position = VFX.transform.position;
                trail.setTarget(other.transform);
                trail.transform.right = transform.right;
                trail.transform.localScale = new Vector3(7, 1, 1);
                trail.gameObject.SetActive(true);

                var trail1 = ObjectRecycler.Singleton.GetObject<SingleEffect>(8);
                trail1.transform.position = VFX.transform.position;
                trail1.setTarget(other.transform);
                trail1.transform.right = -transform.right;
                trail1.transform.localScale = new Vector3(7, 1, 1);
                trail1.gameObject.SetActive(true);


                other.GetComponent<Dummy>().ApplyDamage(damage);

                objectsHit.Add(other.gameObject.GetInstanceID());
            }
            else if (other.tag == "Platform" && other.GetComponent<SimpleBreakable>())
            {
                other.GetComponent<SimpleBreakable>().DestroyBreakable(transform.position);
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Name != "Dashing")
            {
                ObjectRecycler.Singleton.GetObject<SingleEffect>(getRandomEffect());


                other.GetComponent<PlayerCharacter>().KnockbackHorizontal(transform.position, 10f);
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
