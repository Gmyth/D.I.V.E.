using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class SimpleBreakable : Restorable
{
    private Animator anim;

    private bool triggered;

    private List<GameObject> frags;

    [SerializeField] private float min = 1f;

    [SerializeField] private float max = 1.5f;
    
    [SerializeField] private bool triggerSlowMotion = false;
    // Start is called before the first frame update
    [SerializeField] private TriggerObject[] triggers;

    private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_collider;
    private Animator m_animator;
    /////////////////////////////////////////////////
    private bool s_renderer;
    private bool s_collider;
    private bool s_triggered;
    private bool s_animator;
    private SpriteRenderer s_spriteRenderer;
    void Start()
    {
        triggered = false;
        frags = new List<GameObject>();
        collectFrag();
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_collider = gameObject.GetComponent<BoxCollider2D>();
        m_animator = gameObject?.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
//        if (Input.GetButtonDown("Dashing") && !triggered)
//        {
//            DestroyBreakable(transform.position - new Vector3(0,-2,0));
//            triggered = true;
//        }
    }

    public void DestroyBreakable(Vector3 pos)
    {
        if (!triggered)
        {
            CheckPointManager.Instance.RegisterObj(gameObject);
            Debug.LogWarning("Break");
            AudioManager.Singleton.PlayOnce("BreakWindow");
            if (m_animator)
            {
                m_animator.speed = 1;
                m_animator.Play("Glass", -1, 0);
            }
            triggered = true;
            //Play break animation
            CameraManager.Instance.Shaking(0.1f,0.1f,true);
            GetComponent<BoxCollider2D>().enabled = false;
            explode(pos);
        }

    }
    
    //private void Disable()
    //{
    //    gameObject.SetActive(false);
    //    triggered = false;
    //    GetComponent<BoxCollider2D>().enabled = true;
    //}


    private void collectFrag()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SimpleItem>())
            {
                frags.Add(transform.GetChild(i).gameObject);
            }
        }

    }

    private void explode(Vector3 pos)
    {
        GetComponent<Explodable>().explode();
        foreach (var frag in frags)
        {
            float force = Random.Range(500, 1000);
            frag.GetComponent<Rigidbody2D>().AddForce(force * (frag.transform.position - pos).normalized);
            Destroy(frag,Random.Range(min,max));
        }

        if (triggerSlowMotion)
        {
            TimeManager.Instance.startSlowMotion(1f,0.5f);
        }
        
        if (triggers.Length != 0)
        {
            foreach (var trigger in triggers)
            {
                trigger.Enable();
            }
                
        }
    }

    public override void Save()
    {
        s_renderer = m_spriteRenderer.enabled;
        s_spriteRenderer = m_spriteRenderer;
        s_collider = m_collider.enabled;
        s_triggered = triggered;
        if(m_animator != null)
            s_animator = m_animator.enabled;
        
    }
    public override void Restore()
    {
        m_spriteRenderer.enabled = s_renderer;
        m_spriteRenderer = s_spriteRenderer;
        m_collider.enabled = s_collider;

        if (s_triggered == false && triggered == true)
        {
            //restore fragments
            GetComponent<Explodable>().fragmentInEditor();
        }
        frags = new List<GameObject>();
        collectFrag();
        triggered = s_triggered;

        if (m_animator != null)
        {
            m_animator.enabled = s_animator;
            m_animator.speed = 0;
            m_animator.Play("Glass", -1, 0);
        }
            
    }





    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "PlayerHitBox")
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "PlayerHitBox")
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

}
