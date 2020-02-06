using UnityEngine;


public class Explosive : MonoBehaviour
{
    [SerializeField] private int hitDataID;
    [SerializeField] private int explosionObjectID = 0;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }


    protected virtual void Explode()
    {
        HitBox explosionHitBox = ObjectRecycler.Singleton.GetObject<HitBox>(explosionObjectID);
        explosionHitBox.LoadHitData(DataTableManager.singleton.GetHitData(hitDataID));
        explosionHitBox.transform.position = transform.position;
    }
}
