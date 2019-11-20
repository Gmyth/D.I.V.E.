using System.Collections;
using UnityEngine;

public class Recyclable : MonoBehaviour
{
    public int id = -1;
    public float lifeSpan = 0;


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
        yield return new WaitForSeconds(t);
        Die();
    }
}
