using System.Collections;
using UnityEngine;

public class Recyclable : MonoBehaviour
{
    public int id = -1;
    public float lifeSpan = 0;
    public bool enableTimeScale = true;


    protected void Die()
    {
        StopAllCoroutines();

        ObjectRecycler.Singleton.Recycle(this);
    }


    protected virtual void OnEnable()
    {
        if (lifeSpan > 0)
            StartCoroutine(RecycleAfter(lifeSpan));
    }


    private IEnumerator RecycleAfter(float t)
    {
        t = lifeSpan;


        yield return null;


        if (enableTimeScale)
            while (t > 0)
            {
                t -= Time.deltaTime * TimeManager.Instance.TimeFactor;

                yield return null;
            }
        else
            yield return new WaitForSeconds(t);
        

        Die();
    }
}
