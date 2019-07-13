using UnityEngine;

public class Turret : Enemy
{
    [SerializeField] private GameObject bullet;

    private int fireRate;

    private float lastFireSecond;

    private Character target;

    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindObjectOfType<Character>();
        fireRate = Random.Range(2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        int time = (int)Time.fixedUnscaledTime;
        if (time % (fireRate-1) == 0)
        {
            direction = target.transform.position;
        }
        if ((transform.position - target.transform.position).magnitude < 10)
        {
            if (time % fireRate == 0 && lastFireSecond != time)
            {
                lastFireSecond = time;
                var temp = Instantiate(bullet, transform.position, Quaternion.identity);
                float angle = Random.Range(-10, 10);
                temp.GetComponent<Bullet>().setDirection(direction,angle);
            }
        }
    }
}
