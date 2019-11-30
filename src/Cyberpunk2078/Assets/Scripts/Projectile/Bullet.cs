﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Bullet : Recyclable
{
    public bool isFriendly = false;
    public int numHits = 1;
    public int rawDamage = 1;
    
    [SerializeField] private float hitEstimationTimeInterval = 0.02f;
    private float lastHitEstimation;
    private bool hunchTriggered;
    
    private int numHitsRemaining;


    protected override void OnEnable()
    {
        base.OnEnable();
        hunchTriggered = false;
        numHitsRemaining = numHits;
    }

    private void Update()
    {
        if (lastHitEstimation + hitEstimationTimeInterval < Time.unscaledTime && !hunchTriggered && !isFriendly)
        {
            // time to check
            var direction = GetComponent<LinearMovement>().orientation;
            RaycastHit2D hit = Physics2D.Raycast(transform.position,direction, 3.5f);
            if (hit.collider != null && hit.transform.CompareTag("Player"))
            {
                //hit! Hunch Trigger
                PlayerCharacter playerCharacter = hit.collider.GetComponent<PlayerCharacter>();
                if (playerCharacter.IsInFeverMode || !playerCharacter.IsInFeverMode )
                {
                    hunchTriggered = true;
                    playerCharacter.ConsumeFever(50);
                    TimeManager.Instance.startSlowMotion(0.8f);
                    CameraManager.Instance.FocusAt(playerCharacter.transform,0.8f);
                    CameraManager.Instance.FlashIn(7f,0.05f,0.15f,0.01f);
                }
            }

            lastHitEstimation = Time.unscaledTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (numHitsRemaining <= 0)
            Die();


        if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                CameraManager.Instance.Shaking(0.20f,0.05f);

                other.GetComponent<Dummy>().ApplyDamage(rawDamage);
                --numHitsRemaining;


                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.position = other.transform.position - (other.transform.position - transform.position) * 0.2f;
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);

                Die();
            }else if (other.tag == "Ground")
            {
                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = transform.position;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);


                Die();
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Index != 6)
            {
                //Not in dash, deal damage
                other.GetComponent<PlayerCharacter>().ApplyDamage(rawDamage);
                other.GetComponent<PlayerCharacter>().Knockback(transform.position, 300f, 0.3f);
                --numHitsRemaining;
                
                TimeManager.Instance.endSlowMotion();
                CameraManager.Instance.Reset();
                
                CameraManager.Instance.Shaking(0.20f, 0.05f);
                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);

                Die();   
            }
        }
        else if (other.tag == "PlayerAttack")
        {
            if (other.name != "DashAtkBox")
            {
                GetComponent<LinearMovement>().initialPosition = transform.position;
                GetComponent<LinearMovement>().orientation = (Quaternion.Euler(0, 0,  Random.Range(-30, 30)) * (GetComponent<LinearMovement>().orientation * -1)).normalized;
                GetComponent<LinearMovement>().spawnTime = Time.time;

                transform.right = GetComponent<LinearMovement>().orientation;
                
                isFriendly = true;
                

                CameraManager.Instance.Shaking(0.10f,0.05f);
                TimeManager.Instance.startSlowMotion(0.05f);

                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);
            }

        }
        else if (other.tag == "Ground")
        {
            SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
            Hit.transform.right = transform.right;
            Hit.transform.position = transform.position;
            Hit.transform.localScale = Vector3.one;

            Hit.gameObject.SetActive(true);


            Die();
        }
    }
}
