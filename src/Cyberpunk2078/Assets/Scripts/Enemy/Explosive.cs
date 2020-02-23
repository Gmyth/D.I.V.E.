using UnityEngine;


public class Explosive : Recyclable
{
    [SerializeField] public Dummy source;

    [Header("Explosion")]
    [SerializeField] private int hitDataID;
    [SerializeField] private int explosionObjectID = 0;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.layer & ~LayerMask.GetMask("Obstacle", "Platform", "Player", "PlayerHitBox")) != 0)
            Explode();
    }


    protected virtual void Explode()
    {
        HitBox explosionHitBox = ObjectRecycler.Singleton.GetObject<HitBox>(explosionObjectID);
        explosionHitBox.LoadHitData(DataTableManager.singleton.GetHitData(hitDataID));
        explosionHitBox.hit.source = source;
        explosionHitBox.transform.position = transform.position;

        explosionHitBox.gameObject.SetActive(true);


        CameraManager.Instance.Shaking(0.2f, 0.2f);


        Die();
    }
}
