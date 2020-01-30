using System.Collections.Generic;
using UnityEngine;


[System.Serializable] public struct Hit
{
    public enum Type : int
    {
        Melee = 0,
        Projectile = 1,
        CounterAttack = 2,
    }


    public Type type;
    public Dummy source;
    public Bullet bullet;
    public float damage;
    public float knockback;
}


[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    public Hit hit;
    public bool isFriendly = false;
    [SerializeField] private int[] effects;
    [SerializeField] private ContactFilter2D contactFilter = new ContactFilter2D();

    protected HitBoxGroup group;
    protected HashSet<int> objectsHit;

    protected Coroutine bulletTimeCorotine = null;


    private void Awake()
    {
        HitBoxGroup group = transform.parent.GetComponent<HitBoxGroup>();
        objectsHit = group ? group.objectsHit : new HashSet<int>();
    }

    private void OnEnable()
    {
        group?.OnHitBoxEnable(this);


        List<Collider2D> list = new List<Collider2D>();
        int n = GetComponent<Collider2D>().OverlapCollider(contactFilter, list);

        for (int i = 0; i < n; ++i)
            OnTriggerEnter2D(list[i]);
    }

    private void OnDisable()
    {
        if (group)
            group.OnHitBoxDisable(this);
        else
            objectsHit.Clear();
    }


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                Enemy enemy = other.GetComponent<Enemy>();
                int id = enemy.gameObject.GetInstanceID();

                if (!enemy.isEvading && !objectsHit.Contains(id))
                {
                    objectsHit.Add(id);


                    if (GameUtility.ApplyDamage(enemy, hit, other) > 0)
                    {
                        CreateRandomEffect(enemy.transform);

                        var trail = ObjectRecycler.Singleton.GetObject<SingleEffect>(8);
                        trail.transform.position = enemy.transform.position;
                        trail.transform.right = transform.right;
                        trail.transform.localScale = new Vector3(7, 1, 1);
                        trail.target = other.transform;
                        trail.gameObject.SetActive(true);

                        var trail1 = ObjectRecycler.Singleton.GetObject<SingleEffect>(8);
                        trail1.transform.position = enemy.transform.position;
                        trail1.transform.right = -transform.right;
                        trail1.transform.localScale = new Vector3(7, 1, 1);
                        trail1.target = other.transform;
                        trail1.gameObject.SetActive(true);

                        CameraManager.Instance.Shaking(0.20f, 0.10f, true);
                    }
                }
            }
            else if (other.tag == "Platform" && other.GetComponent<SimpleBreakable>())
            {
                other.GetComponent<SimpleBreakable>().DestoryBreakable();
            }
        }
        else if (other.tag == "Player")
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();
            int id = player.gameObject.GetInstanceID();

            if (player.State.Name != "Dash" && !objectsHit.Contains(id))
            {
                objectsHit.Add(id);


                hit.source.OnAttack.Invoke(hit, other);
                player.OnHit?.Invoke(hit, other);


                if (hit.knockback > 0)
                    player.Knockback(hit.source.transform.position, hit.knockback, 0.5f);


                player.ApplyDamage(hit.damage);
                

                CreateRandomEffect(player.transform);


                var trail = ObjectRecycler.Singleton.GetObject<SingleEffect>(8);
                trail.transform.position = other.transform.position - (other.transform.position - transform.position) * 0.2f;
                trail.transform.right = transform.right;
                trail.transform.localScale = new Vector3(20, 1, 1);
                trail.setTarget(other.transform);
                trail.gameObject.SetActive(true);


                var trail1 = ObjectRecycler.Singleton.GetObject<SingleEffect>(8);
                trail1.transform.position = other.transform.position - (other.transform.position - transform.position) * 0.2f;
                trail1.transform.right = -transform.right;
                trail1.transform.localScale = new Vector3(20, 1, 1);
                trail1.setTarget(other.transform);
                trail1.gameObject.SetActive(true);
            }
        }
    }


    protected SingleEffect CreateRandomEffect(Transform targetTransform)
    {
        SingleEffect effect1 = ObjectRecycler.Singleton.GetObject<SingleEffect>(Random.Range(1,2));
        effect1.transform.position = targetTransform.position - (targetTransform.position - transform.position) * 0.2f;
        effect1.transform.right = transform.right;
        effect1.setTarget(targetTransform);
        effect1.gameObject.SetActive(true);
        
        if (effects.Length == 0)
            return null;
            

        SingleEffect effect = ObjectRecycler.Singleton.GetObject<SingleEffect>(effects[Random.Range(0, effects.Length)]);
        effect.transform.position = targetTransform.position - (targetTransform.position - transform.position) * 0.2f;
        effect.transform.right = transform.right;
        effect.transform.localScale = Vector3.one;
        effect.setTarget(targetTransform);
        effect.gameObject.SetActive(true);


        return effect;
    }


    protected SingleEffect CreateEffect(Transform targetTransform, int index = 0)
    {
        if (index < 0 || index >= effects.Length)
            return null;


        SingleEffect effect = ObjectRecycler.Singleton.GetObject<SingleEffect>(effects[index]);
        effect.transform.position = targetTransform.position - (targetTransform.position - transform.position) * 0.2f;
        effect.transform.right = transform.right;
        effect.transform.localScale = Vector3.one;
        effect.setTarget(targetTransform);
        effect.gameObject.SetActive(true);


        return effect;
    }
}
