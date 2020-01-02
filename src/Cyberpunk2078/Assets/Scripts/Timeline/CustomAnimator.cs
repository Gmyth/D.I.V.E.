using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private float lastSpriteSec;
    [SerializeField] private float SpriteReplaceInterval = 2.0f;
    private SpriteRenderer render;
    private int index; 
    // Start is called before the first frame update
    void OnEnable()
    {
        index = 0;
        render = GetComponent<SpriteRenderer>();
        lastSpriteSec = Time.unscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastSpriteSec + SpriteReplaceInterval < Time.unscaledTime) {
            render.sprite = sprites[index];
            lastSpriteSec = Time.unscaledTime;
            if (index >= sprites.Length - 1) index = 0;
            else index += 1;
        }
    }

}
