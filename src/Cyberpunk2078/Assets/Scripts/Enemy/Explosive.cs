using UnityEngine;


public class Explosive : Recyclable
{
    [SerializeField] public Dummy source;

    [Header("Explosion")]
    [SerializeField] private int hitDataID;
    [SerializeField] private int explosionObjectID = 0;


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogWarning(other.tag);
        Explode();
    }


    protected virtual void Explode()
    {
        HitBox explosionHitBox = ObjectRecycler.Singleton.GetObject<HitBox>(explosionObjectID);
        explosionHitBox.LoadHitData(DataTableManager.singleton.GetHitData(hitDataID));
        explosionHitBox.hit.source = source;
        explosionHitBox.transform.position = transform.position;

        explosionHitBox.gameObject.SetActive(true);


        Die();
    }
}
