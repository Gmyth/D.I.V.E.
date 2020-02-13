using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlertDoor : MonoBehaviour
{

    public delegate bool EnemyDeathDelegate(Enemy enemy);

    public SimpleAlertEnemy[] enemies;
    private int enemyCount;

    private bool unlocked;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].BindAlertDoor(this);
        }

        enemyCount = enemies.Length;
        GetComponent<Animator>().Play("KillDoor_Idle",0,-1);
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
        //OpenedDoor.SetActive(true);
        //CameraManager.Instance.Shaking(0.6f, 1000000f);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Animator>().Play("KillDoor_Unlock",0,-1);
    }

}
