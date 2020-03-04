using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlertDoor : Restorable
{

    public delegate bool EnemyDeathDelegate(Enemy enemy);

    public SimpleAlertEnemy[] enemies;
    private int enemyCount;

    private bool unlocked;

    private SpriteRenderer spriteRenderer;
    ///////////////////////////////////////////
    private bool s_collider;
    private bool s_Renderer;
    private Sprite s_sprite;
    void Start()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].BindAlertDoor(this);
        }

        enemyCount = enemies.Length;
        GetComponent<Animator>().Play("KillDoor_Idle",0,-1);
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        Register(gameObject);
    }

    public override void Save()
    {
        s_collider = GetComponent<BoxCollider2D>().enabled;
        s_Renderer = spriteRenderer.enabled;
        s_sprite = spriteRenderer.sprite;
}

    public override void Restore()
    {
        GetComponent<BoxCollider2D>().enabled = s_collider;
        spriteRenderer.enabled = s_Renderer;
        enemyCount = enemies.Length;
        spriteRenderer.sprite = s_sprite;
    }

    public void Register(GameObject obj)
    {
        CheckPointManager.Instance.RegisterObj(obj);
    }
}
