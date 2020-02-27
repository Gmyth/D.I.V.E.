﻿using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;


public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; } = null;

    /// <summary>
    /// Enemies needed to be reset and restore position
    /// </summary>
    public List<GameObject> savedEnemies = new List<GameObject>();

    /// <summary>
    /// Dummy reference
    /// </summary>
    public List<GameObject> Enemies = new List<GameObject>();

    /// <summary>
    /// //Objects needed to be restored
    /// </summary>
    private List<GameObject> Objects = new List<GameObject>();

    /// <summary>
    /// Blackscreen Reference
    /// </summary>
    /// 

    [SerializeField] private GameObject blackScreen;

    [SerializeField] private float SpeedFactor = 1;

    private Transform playerLastCheckPoint;

    private CheckPointTrigger cpt;

    void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        GameObject dummyHolder = GameProcessManager.Singleton.GetCurrentDummies();      
        var dummies = dummyHolder.GetComponentsInChildren<Dummy>();
        Enemies.Clear();
        for (int i = 0; i < dummies.Length; i++)
        {
            Enemies.Add(dummies[i].gameObject);
        }

        //GameObject ObjHolder = GameProcessManager.Singleton.GetCurrentObjects();
        //Objects.Clear();
        //for(int i=0; i < ObjHolder.transform.childCount; i++)
        //{
        //    var obj = ObjHolder.transform.GetChild(i).gameObject;
        //    Objects.Add(obj);
        //}
    }

    public void Save(Transform _playertransform, CheckPointTrigger checkPointTrigger)
    {
        playerLastCheckPoint = _playertransform;

        //Clear when reach new checkpoint
        savedEnemies.Clear();
        Objects.Clear();

        //save enemies position
        for (int i = 0; i < Enemies.Count; i++)
        {
            //record position
            Enemies[i].GetComponent<Enemy>().lastCheckPointTransform = Enemies[i].gameObject.transform.position;
        }

        for(int i = 0; i < Objects.Count; i++)
        {
            Objects[i].GetComponent<Restorable>().Save();
        }
    }

    public void Restore()
    {
        //AudioManager.Singleton.StopBus("Enemy");
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<Animator>().Play("MainCharacter_Idle",0,0);
        PlayerCharacter.Singleton.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        StartCoroutine(Restoring());
    
        //Restore Camera State
        CameraManager.Instance.Idle();
        //TimeManager.Instance.endSlowMotion();

        //Restore objects
        for(int i = 0; i < Objects.Count; ++i)
        {
            Objects[i].GetComponent<Restorable>().Restore();
        }

        //cpt.enabled = true;
    }

    private void RestoreEnemy()
    {       
        //Enable kill enemies
        for (int i = 0; i < savedEnemies.Count; i++)
        {
            savedEnemies[i].SetActive(true);
        }

        //Restore all enemies position
        for (int i = 0; i < Enemies.Count; i++)
        {
            var lastPos = Enemies[i].GetComponent<Enemy>().lastCheckPointTransform;
            Enemies[i].transform.position = lastPos;
            Enemies[i].GetComponent<Enemy>().Reset();
        }
    }

    IEnumerator Restoring()
    {
        Image image = blackScreen.GetComponent<Image>();
        //Black Screen Fade in
        float a = 0;
        while (a < 1)
        {
            a += (Time.deltaTime* SpeedFactor);
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }

        //Restore player position
        var player = PlayerCharacter.Singleton.gameObject;

        Vector3 position = playerLastCheckPoint.position;
        position.z = 0;
        player.transform.position = position;
        

        RestoreEnemy();

        

        //Black Screen Fade out
        while (a > 0)
        {
            a -= (Time.deltaTime* SpeedFactor);
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }

       
        PlayerCharacter.Singleton.GetFSM().Reboot();
        

        yield return null;
    }

    public void Dead(GameObject obj)
    {
        if(!savedEnemies.Contains(obj))
            savedEnemies.Add(obj);
    }

    public void RestoreObject(GameObject obj)
    {
        if (!Objects.Contains(obj))
        {
            Objects.Add(obj);
        }           
    }
}
