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
        if (numHitsRemaining <= 0)
            Die();


        if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                CameraManager.Instance.Shaking(0.20f,0.05f);

                other.GetComponent<Dummy>().ApplyDamage(rawDamage);
                --numHitsRemaining;


                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.position = other.transform.position - (other.transform.position - transform.position) * 0.2f;
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);

                Die();
            }
        }
        else if (other.tag == "Hunch" )
        {
            PlayerCharacter playerCharacter = other.GetComponentInParent<PlayerCharacter>();

            if (playerCharacter.State.Name == "Dash")
            {
                TimeManager.Instance.startSlowMotion(0.2f);
                CameraManager.Instance.flashIn(7f,0.05f,0.15f,0.01f);
            }
            else if (playerCharacter.IsInFeverMode)
            {
                playerCharacter.ConsumeFever(50);
                TimeManager.Instance.startSlowMotion(0.4f);
                CameraManager.Instance.flashIn(7f,0.05f,0.15f,0.01f);
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Name != "Dash")
            {
                other.GetComponent<PlayerCharacter>().ApplyDamage(rawDamage);
                --numHitsRemaining;


                CameraManager.Instance.Shaking(0.20f, 0.05f);
                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);
                

                Die();   
            }
        }
        else if (other.tag == "PlayerAttack")
        {
            if (other.name != "DashAtkBox")
            {
                GetComponent<LinearMovement>().initialPosition = transform.position;
                GetComponent<LinearMovement>().orientation = (Quaternion.Euler(0, 0,  Random.Range(-30, 30)) * (GetComponent<LinearMovement>().orientation * -1)).normalized;
                GetComponent<LinearMovement>().spawnTime = Time.time;

                transform.right = GetComponent<LinearMovement>().orientation;
                
                isFriendly = true;
                

                CameraManager.Instance.Shaking(0.10f,0.05f);


                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);
            }
            else
                TimeManager.Instance.startSlowMotion(0.3f); 
        }
        else if (other.tag == "Ground")
        {
            SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
            Hit.transform.right = transform.right;
            Hit.transform.position = transform.position;
            Hit.transform.localScale = Vector3.one;

            Hit.gameObject.SetActive(true);


            Die();
        }
    }
}
