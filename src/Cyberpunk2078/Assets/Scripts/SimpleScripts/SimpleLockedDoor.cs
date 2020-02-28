using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLockedDoor : Restorable
{

    public List<SimpleLocker> lockers;
    public int lockerCount = 0;

    [SerializeField] public List<GameObject> dots;

    [SerializeField] private Sprite unlocked;
    
    [SerializeField] private Transform focus;

    private bool isPlayed = false;

    ////////////////////////////////////////////////////
    private Sprite s_DotSprite;
    private int s_lockerCount;
    private List<bool> s_collider = new List<bool>();
    private bool s_isPlayed;
    // Start is called before the first frame update
    void Awake()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddLocker(SimpleLocker targetLocker)
    {
        lockers.Add(targetLocker);
        lockerCount++;
    }

    public void DeleteLocker()
    {
        CameraManager.Instance.Shaking(0.05f,0.1f);

        --lockerCount;
        
        if (lockerCount <= 0)
        {
            Unlock();
        }
        else
        {
             dots[lockerCount].GetComponent<SpriteRenderer>().sprite = unlocked;
        }
    }

    public void Unlock()
    {
        CameraManager.Instance.Reset();
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "NoInput";
        PlayerCharacter.Singleton.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        PlayerCharacter.Singleton.GetComponentInChildren<Animator>().Play("MainCharacter_Idle", -1, 0f);
        StartCoroutine(cameraResetDelay(1.5f));
        foreach (var collider in GetComponentsInChildren<BoxCollider2D>())
        {
            collider.enabled = false;
        }
        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowText("A door opened somewhere...");
        //CameraManager.Instance.Shaking(0.6f, 1000000f);
    }

    private IEnumerator cameraResetDelay(float duration)
    {
        
        yield return new WaitForSeconds(1.5f);
        CameraManager.Instance.FocusTo(focus.position);
        yield return new WaitForSeconds(1f);
        dots[lockerCount].GetComponent<SpriteRenderer>().sprite = unlocked;
        GetComponent<Animator>().speed = 1;
        GetComponent<Animator>().Play("DoorOpen",-1,0);
        isPlayed = true;
        yield return new WaitForSeconds(0.5f);
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "Idle";
       
        yield return new WaitForSeconds(duration - 0.5f);
        
        CameraManager.Instance.Reset();
        
    }

    public override void Save()
    {
        for(int i=0; i < dots.Count; i++)
        {
            s_DotSprite = dots[i].GetComponent<SpriteRenderer>().sprite;
        }
        
        s_lockerCount = lockerCount;
        var colliders = GetComponentsInChildren<BoxCollider2D>();
        s_collider.Clear();
        for (int i=0; i < colliders.Length; i++)
        {
            s_collider.Add(colliders[i]);
        }
        s_isPlayed = isPlayed;
    }

    public override void Restore()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].GetComponent<SpriteRenderer>().sprite = s_DotSprite;
        }
        lockerCount = s_lockerCount;
        var colliders = GetComponentsInChildren<BoxCollider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = s_collider[i];
        }
        var animator = GetComponent<Animator>();
        if (s_isPlayed)
        {
            animator.Play("DoorOpen", -1, 1);
        }
        else
        {
            animator.speed = 0;
            animator.Play("DoorOpen", -1, 0);
        }
    }

    public void RegisterObj()
    {
        CheckPointManager.Instance.RegisterObj(gameObject);
    }
}
