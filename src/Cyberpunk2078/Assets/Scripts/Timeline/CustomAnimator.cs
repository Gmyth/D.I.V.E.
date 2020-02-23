using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesForJoycon;
    [SerializeField] private Sprite[] spritesForKeyboard;
    private float lastSpriteSec;
    [SerializeField] private float SpriteReplaceInterval = 1.0f;
    public bool loop;
    private SpriteRenderer render;
    private int index;
    private Sprite[] sprites;
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

        //    else if (index >= sprites.Length - 1 && !loop) { render.color = Color.clear;}

        //    else { index += 1; }
        //}

    }

    public void Play(bool _loop = true) {

        sprites = MouseIndicator.Singleton.CurrentInputType == InputType.Joystick ? spritesForJoycon :
            spritesForKeyboard;
        render.sprite = sprites[0];
        loop = _loop;
        index = 0;
        lastSpriteSec = Time.unscaledTime;
        render.color = Color.white;
        //StartCoroutine(playAnimation());
    }

    //private IEnumerator playAnimation() {

    //    while (Time.unscaledTime - lastSpriteSec >= SpriteReplaceInterval)
    //    {
    //        Debug.LogWarning(Time.unscaledTime);
    //        render.sprite = sprites[index];
    //        lastSpriteSec += SpriteReplaceInterval;
    //        Debug.LogWarning(index);
    //        if (index >= sprites.Length - 1 && loop) { index = 0; }

    //        else if (index >= sprites.Length - 1 && !loop) { render.color = Color.clear; }

    //        else { index += 1; }

    //        yield return null;
    //    }
    //    yield return null;

    //}

}
