using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private float lastSpriteSec;
    [SerializeField] private float SpriteReplaceInterval = 2.0f;
    public bool loop;
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
        //while (Time.unscaledTime - lastSpriteSec >= SpriteReplaceInterval)
        //{
        //    Debug.LogWarning(Time.unscaledTime);
        //    render.sprite = sprites[index];
        //    lastSpriteSec += SpriteReplaceInterval;
        //    Debug.LogWarning(index);
        //    if (index >= sprites.Length - 1 && loop) { index = 0;}

        //    else if (index >= sprites.Length - 1 && !loop) { render.color = Color.clear; }

        //    else { index += 1; }
        //}

    }

    public void Play(bool _loop = true) {
        loop = _loop;
        index = 0;
        lastSpriteSec = Time.unscaledTime;
        render.color = Color.white;
        StartCoroutine(playAnimation());
    }

    private IEnumerator playAnimation() {

        while (Time.unscaledTime - lastSpriteSec >= SpriteReplaceInterval)
        {
            Debug.LogWarning(Time.unscaledTime);
            render.sprite = sprites[index];
            lastSpriteSec += SpriteReplaceInterval;
            Debug.LogWarning(index);
            if (index >= sprites.Length - 1 && loop) { index = 0; }

            else if (index >= sprites.Length - 1 && !loop) { render.color = Color.clear; }

            else { index += 1; }


            yield return null;
        }
        yield return null;

    }

}
