using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int objectID;
    [SerializeField] private Zone spawnZone;
    [SerializeField] private bool spawnOnEnable = true;
    [SerializeField] private bool allowDuplicates = false;
    [SerializeField][Min(0)] private float spawnTime = 15;

    private bool hasBeenInitialized = false;
    private float t_spawn = 0;
    private Recyclable enemy = null;


    public virtual void Spawn()
    {
        if (allowDuplicates || !enemy)
        {
            enemy = ObjectRecycler.Singleton.GetObject(objectID, spawnZone.Type == ZoneType.Universe ? transform.position : spawnZone.GetRandomPosition());
            enemy.gameObject.SetActive(true);
        }
    }

    public virtual void Clear()
    {
        if (!allowDuplicates && enemy)
            enemy.Die();
    }


    private void OnEnable()
    {
        t_spawn = 0;

        if (hasBeenInitialized && spawnOnEnable)
            Spawn();
    }

    private void Start()
    {
        hasBeenInitialized = true;


        if (spawnOnEnable)
            Spawn();
    }

    private void Update()
    {
        if ((t_spawn += TimeManager.Instance.ScaledDeltaTime) >= spawnTime)
            Spawn();
    }
}
