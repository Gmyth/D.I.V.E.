using System.Collections;
using MyBox;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_CounterAttack", menuName = "Enemy State/Shield Boss/Counter Attack")]
public class L2ShieldBossState_CounterAttack : ESAttack<L2ShieldBoss>
{
    [SerializeField] private float motionTime;
    [SerializeField] private string animation;
    [SerializeField] private string state_afterAttack;

    private Animator animator;

    private float t_finish = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_finish = 0;


        enemy.AdjustFacingImmediately();


        animator.Play(animation);


        //TimeManager.Instance.startSlowMotion(1f, 0.05f, 0.5f);
        TimeManager.Instance.endSlowMotion();
        enemy.StartCoroutine(SlowMotion());
        CameraManager.Instance.FocusTo(
            (enemy.transform.position - FindObjectOfType<PlayerCharacter>().transform.position)/2
            + FindObjectOfType<PlayerCharacter>().transform.position,  0.3f);
    }

    public override string Update()
    {
        //animator.speed = TimeManager.Instance.TimeFactor;


        t_finish += TimeManager.Instance.ScaledDeltaTime;

        if (t_finish >= motionTime)
            return state_afterAttack;


        return Name;
    }


    private IEnumerator SlowMotion(float duration = 0.3f)
    {
        float originalTimeScale = Time.timeScale;


        

        Time.timeScale = 0.2f;
        
        TimeManager.Instance.ApplyBlackScreen();
        
        animator.speed = 0;
        
        enemy.spark.gameObject.SetActive(true);
        enemy.spark.GetComponent<Animator>().Play("Spark",0,-1);
        enemy.spark.GetComponent<Animator>().speed = 8f;


        yield return new WaitForSecondsRealtime(duration);
        
        
        enemy.spark.gameObject.SetActive(false);
        TimeManager.Instance.EndFeverMotion();
        
        animator.speed = 1f;
        Time.timeScale = originalTimeScale;
    }
}
