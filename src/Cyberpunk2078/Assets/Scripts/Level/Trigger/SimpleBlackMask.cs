using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBlackMask : TriggerObject
{

    [SerializeField] private float targetFXAlpha = 0;
    private float velocity;

    private SpriteRenderer sprite;

    private bool triggered;

    private float defaultAlpha;
    
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        triggered = false;
        defaultAlpha = sprite.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered &&  sprite.color.a != targetFXAlpha)
        {
            float current = sprite.color.a;
            sprite.color =new Color(sprite.color.r,sprite.color.g,sprite.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
        }
        else if(!triggered && sprite.color.a != defaultAlpha)
        {
             float current = sprite.color.a;
             sprite.color =new Color(sprite.color.r,sprite.color.g,sprite.color.b, Mathf.SmoothDamp(current, defaultAlpha, ref velocity, 0.1f));
        }
    }


    public override void Enable()
    {
        triggered = true;
    }


    public override void Disable()
    {
        triggered = false; 
    }
    
}
