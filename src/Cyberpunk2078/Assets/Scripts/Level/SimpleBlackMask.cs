using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBlackMask : MonoBehaviour
{

    [SerializeField] private float targetFXAlpha = 0;
    private float velocity;

    private SpriteRenderer sprite;

    private bool triggered;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered &&  sprite.color.a != targetFXAlpha)
        {
            float current = sprite.color.a;
            sprite.color =new Color(sprite.color.r,sprite.color.g,sprite.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
        }
    }


    public void Trigger()
    {
        triggered = true;
    }
}
