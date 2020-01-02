using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlertDoor : MonoBehaviour
{

    public delegate bool EnemyDeathDelegate(Enemy enemy);

    public SimpleAlertEnemy[] enemies;
    private int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].BindAlertDoor(this);
        }

        enemyCount = enemies.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyEnemyCountChange(int num)
    {
        enemyCount += num;

        if (enemyCount <= 0)
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        gameObject.SetActive(false);
        //OpenedDoor.SetActive(true);
        //CameraManager.Instance.Shaking(0.6f, 1000000f);
    }

}
