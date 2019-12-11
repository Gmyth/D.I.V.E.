using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class SimpleRandomMovement : MonoBehaviour
{
    [SerializeField] private float timer = 0f;
    [SerializeField] private float factor = 1f;
    [SerializeField] private float maxDistance = 0.03f;
    [SerializeField] private float speed = 0.03f;

    private Vector2 intialPos;
    private float seed;
    private Light2D light;

    // Start is called before the first frame update
    void Start()
    {
        intialPos = transform.position;
        seed = Random.Range(0f, 10000f);
        light = gameObject.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float y = Mathf.PerlinNoise(0f, (timer + seed)) * factor;
        float x = Mathf.PerlinNoise((timer + seed), 0f) * factor;

        Vector2 targetPos = new Vector2(intialPos.x + x, intialPos.y + y);
        
        transform.position = Vector2.MoveTowards(transform.position, targetPos, maxDistance);

        light.intensity = Mathf.Lerp(light.intensity, x/3 , timer * speed);
    }


}
