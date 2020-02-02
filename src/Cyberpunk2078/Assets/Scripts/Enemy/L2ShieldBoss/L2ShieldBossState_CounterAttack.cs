using System.Collections;
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
        CameraManager.Instance.FlashIn(7f, 0.2f, 0.7f, 0.3f);
    }

    public override string Update()
    {
        //animator.speed = TimeManager.Instance.TimeFactor;


        t_finish += TimeManager.Instance.ScaledDeltaTime;

        if (t_finish >= motionTime)
            return state_afterAttack;


        return Name;
    }


    private IEnumerator SlowMotion(float duration = 1f)
    {
        float originalTimeScale = Time.timeScale;


        TimeManager.Instance.ApplyBlackScreen();

        Time.timeScale = 0.2f;


        var spark = enemy.GetComponentInChildren(typeof(CustomAnimator), true) as CustomAnimator;
        spark.gameObject.SetActive(true);
        spark.Play(false);


        yield return new WaitForSecondsRealtime(duration);
        
        
        spark.gameObject.SetActive(false);


        Time.timeScale = originalTimeScale;
    }
}
