using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    private float originalYPosition;
    private float originalXPosition;
    private bool movedUp = true;
    private bool movedDown = true;
    private bool movedLeft = true;
    private bool movedRight = true;
    public float offset = 1f;
    public float Smoothness;

    // Use this for initialization
    void Awake()
    {
        originalYPosition = transform.position.y;
        originalXPosition = transform.position.x;
        Smoothness = Random.Range(0.5f, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (movedUp)
        {
            Vector2 position = Vector2.Lerp(transform.position, new Vector2(originalXPosition, originalYPosition + offset), Smoothness * Time.deltaTime);
            transform.position = new Vector2(transform.position.x, position.y);
        }
        else if (movedDown)
        {
            Vector2 position = Vector2.Lerp((Vector2)(transform.position), new Vector2(originalXPosition, originalYPosition), Smoothness * Time.deltaTime);
            transform.position = new Vector2(transform.position.x, position.y);
        }

        if(transform.position.y -  originalYPosition  > 0.9 * offset)
        {
            movedUp = false;
        }
        else if (transform.position.y - originalYPosition<= 0.1)
        {
            movedUp = true;
        }

    }
}
