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
                other.GetComponent<Dummy>().ApplyDamage(GetInstanceID(),rawDamage);
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
        else if (other.tag == "Hunch" && other.GetComponentInParent<PlayerCharacter>().State.Name == "Dash")
        {
            TimeManager.Instance.startSlowMotion(0.3f);
            CameraManager.Instance.flashIn(7f,0.05f,0.1f,0.01f);
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
                Die();   
            }

        }
        
        else if (other.tag == "PlayerAttack")
        {
            if (other.name != "DashAtkBox")
            {
                
                gameObject.GetComponent<LinearMovement>().orientation *= -1;
                isFriendly = true;
                
                CameraManager.Instance.Shaking(0.10f,0.05f);
                var Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.gameObject.SetActive(true);
                Hit.transform.right = transform.right;
                Hit.transform.position =
                    other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;
            }
            else
            {
                TimeManager.Instance.startSlowMotion(0.3f); 
            }

        }else if (other.tag == "Ground")
        {
            Die();
        }

    }
}
