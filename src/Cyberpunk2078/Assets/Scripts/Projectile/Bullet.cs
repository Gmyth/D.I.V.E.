using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Bullet : Recyclable
{
    public bool isFriendly = false;
    public int numHits = 1;
    public int rawDamage = 1;

    private int numHitsRemaining;


    protected override void OnEnable()
    {
        base.OnEnable();

        numHitsRemaining = numHits;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                CameraManager.Instance.Shaking(0.20f,0.05f);
                other.GetComponent<PlayerCharacter>().ApplyDamage(GetInstanceID(),rawDamage);
                var Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.position =
                    other.transform.position - (other.transform.position - transform.position) * 0.2f;
                Hit.gameObject.SetActive(true);
                Hit.transform.right = transform.right;
                Hit.transform.position =
                    other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;
                Die();
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Name != "Dash")
            {
                other.GetComponent<PlayerCharacter>().ApplyDamage(GetInstanceID(), rawDamage);
                CameraManager.Instance.Shaking(0.20f, 0.05f);
                var Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.gameObject.SetActive(true);
                Hit.transform.right = transform.right;
                Hit.transform.position =
                    other.transform.position + (transform.position - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;
            }
            else {
                TimeManager.Instance.startSlowMotion(1f);
            }
            
           
            Die();
        }
        
        else if (other.tag == "PlayerAttack")
        {
            if (other.name != "DashAtkBox")
            {
                CameraManager.Instance.Shaking(0.10f,0.05f);
                var Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.gameObject.SetActive(true);
                Hit.transform.right = transform.right;
                Hit.transform.position =
                    other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;
            }

        }
        
    }
}
