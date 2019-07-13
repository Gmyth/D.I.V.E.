using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{

    [SerializeField] private GameObject boom;
    [SerializeField] private GameObject hitFx;

    private bool dead;
    // Start is called before the first frame update
    void Start()
    {
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerAtk" && !dead)
        {
            // boom
            Time.timeScale = 0.15f;
            GameObject.FindObjectOfType<Character>().addEnergy(70);
            Destroy(Instantiate(hitFx,transform.position,transform.rotation), 0.2f);
            StartCoroutine(killDelay(0.015f));
            CameraManager._instance.Shake(0.05f,0.05f);
            dead = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
            //Destroy(gameObject,0.5f);
        }else if (other.tag == "Bullet" && !dead)
        {
            // boom
            Time.timeScale = 0.15f;
            GameObject.FindObjectOfType<Character>().addEnergy(40);
            Destroy(Instantiate(hitFx,transform.position,transform.rotation), 0.2f);
            StartCoroutine(killDelay(0.015f));
            dead = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
            //Destroy(gameObject,0.5f);
        }
    }

    private IEnumerator killDelay(float time)
    {
        yield return  new WaitForSeconds(time);
        Time.timeScale = 1f;
        Destroy(Instantiate(boom,transform.position,transform.rotation), 0.3f);
        yield return  new WaitForSeconds(0.7f);
        dead = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
