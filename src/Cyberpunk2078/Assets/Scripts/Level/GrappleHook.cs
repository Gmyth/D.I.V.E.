using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum grappleState
{
    Shot,
    Pull,
    Dead
}

public class GrappleHook : Recyclable
{
    private LineRenderer lineRederer;
    private LineRenderer laser;
    [SerializeField] private Transform target;
    [SerializeField] private float maxRange;
    [SerializeField] private float initialForce;
    [SerializeField] private float lifetime;
    [SerializeField] private float pullDelay;
    private float timeToDie;
    private grappleState currentState;
    private Vector3 direction;
    private List<Vector3> Joints;
    private Vector3 catchPos;

    private GameObject hitTarget;
    // Start is called before the first frame update
    private void OnEnable()
    {
        currentState = grappleState.Dead;
        laser = GetComponent<LineRenderer>(); 
        GetComponent<SpriteRenderer>().color = Color.clear;
        Joints = new List<Vector3>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        var rb2d = GetComponent<Rigidbody2D>();
        
        //Disable Input for grapple hook
//        if (Input.GetButtonDown("Special1") && currentState == grappleState.Dead)
//        {
//            Fire();
//        }
        switch (currentState)
        {
            case grappleState.Shot:
                // get current fly distance;
                transform.right = rb2d.velocity.normalized;
                float distance = (target.position - transform.position).sqrMagnitude;
                
                if (timeToDie < Time.time) Dead();
                
                if (distance <= maxRange)
                {
                    Vector3 fixedStart =
                        transform.position + (target.transform.position - transform.position).normalized * 0.1f ;
                    Vector3 fixedEnd =
                        (transform.position - target.transform.position).normalized * 0.1f + target.transform.position;
                    
                    laser.SetPosition(0,fixedStart);
                    laser.SetPosition(1,fixedEnd);
//                    if ((fixedStart - Joints[0]).sqrMagnitude > 0.5f)
//                    {
//                        //character moved
//                        Joints.Insert(0,fixedStart);
//                    }
//                    
//                    if ((fixedEnd - Joints[Joints.Count - 1]).sqrMagnitude > 1.5f)
//                    {
//                        //character moved
//                        Joints.Add(fixedEnd);
//                    }
//
//                    drawCurve();

                    connectivityCheck(fixedStart, fixedEnd);
                }
                else
                {
                    // out of range
                    Dead();
                }
                break;
            case grappleState.Pull:
                transform.position = catchPos;
                var hookFacing = -(target.position - transform.position).normalized;
                transform.right = hookFacing;
                Vector3 start =
                    transform.position + (target.transform.position - transform.position).normalized * 0.1f ;
                Vector3 end =
                    (transform.position - target.transform.position).normalized * 0.1f + target.transform.position;
                laser.SetPosition(0,start);
                laser.SetPosition(1,end);
                //Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);
                connectivityCheck(start, end);
                break;

        }
        
    }
    public void Dead()
    {
        currentState = grappleState.Dead;
        laser.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        GetComponent<SpriteRenderer>().color = Color.clear;
        if (hitTarget) hitTarget.GetComponent<Dummy>().UnitTimeFactor = 1f;
        Joints.Clear();
    }

    private void Fire()
    {
        timeToDie = Time.time + lifetime;
        transform.position = target.position;
        Joints.Add(target.position);
        laser.enabled = true;
        currentState = grappleState.Shot;
        Vector2 dir =  PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().GetAttackDirection();
        transform.right = dir;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        GetComponent<Rigidbody2D>().AddForce(initialForce * dir *100f);
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<Rigidbody2D>().simulated = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != grappleState.Shot) return;
        if (other.tag == "Ground" ||other.tag ==  "Platform")
        {
            currentState = grappleState.Pull;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            catchPos = transform.position;
            StartCoroutine(PullDelay());
           // PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 14;
        }else if (other.tag == "Enemy")
        {
            currentState = grappleState.Pull;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            catchPos = transform.position;
            other.GetComponent<Dummy>().UnitTimeFactor = 0f;
            hitTarget = other.gameObject;
            StartCoroutine(PullDelay());
           // PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 14;
        }
    }

    private void connectivityCheck(Vector2 fixedStart, Vector2 fixedEnd)
    {
        float distance = (target.position - transform.position).sqrMagnitude;
        RaycastHit2D hit = Physics2D.Raycast(fixedStart, fixedEnd - fixedStart);
        if (hit.collider != null 
            && hit.collider.tag == "Ground" 
            && distance > (hit.collider.transform.position - target.position).sqrMagnitude){
            // the line hit ground
            Dead();
        }
    }

    private IEnumerator PullDelay()
    {
        yield return new WaitForSecondsRealtime(pullDelay);
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "GrapplingHook";
    }

    private void drawCurve()
    {
        laser.positionCount = Joints.Count;
        int index = 0;
        foreach (var joint in Joints)
        {
            laser.SetPosition(index,joint);
            index++;
        }
        
    }


}
