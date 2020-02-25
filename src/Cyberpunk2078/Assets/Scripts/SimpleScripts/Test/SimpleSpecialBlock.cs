using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpecialBlock : MonoBehaviour
{
    
    [SerializeField]
    private float timeLimit =5.0f;

    private bool isOccupied = false;
    private float occupiedTime = 0;

    private float spriteAlpha;

    private Collider2D c2d;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        c2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOccupied) 
        {
            occupiedTime += Time.deltaTime;

            spriteAlpha = Mathf.Lerp(0.2f, 1, occupiedTime / timeLimit);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteAlpha);

            if (occupiedTime >= timeLimit)
            {
                OnTriggered();
            }
        }
    }

    private void OnTriggered()
    {
        StartCoroutine(ChangeColliderAfterDelay(0.1f));
        isOccupied = false;

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player")
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            OnTriggered();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Dummy>().Dead();
        }
    }

    private IEnumerator ChangeColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        c2d.isTrigger = false;
    }

}
