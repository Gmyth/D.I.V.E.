using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Tutorial_Deflect", menuName = "Player State/Tutorial Deflect")]
public class PSTutorial_Deflect : PlayerState
{
    [SerializeField] private GameObject SplashFX;

    private float t = 0;

    private GameObject attackEffect;

    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);


        SplashFX.GetComponentInChildren<HitBox>().hit.source = playerCharacter;
    }

    public override string Update()
    {
        if (t == 0)
        {
            Vector2 idealDirection = (SimpleTutorialManager.Instance.DeflectTutorial_Bullet.transform.position - PlayerCharacter.Singleton.transform.position).normalized;

            if (PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().DirectionNotification(idealDirection, 10f))
            {
                if (Input.GetButtonDown("Attack1") && !playerCharacter.isInTutorial)
                {
                    anim.Play("MainCharacter_Atk", -1, 0f);

                    attackEffect = Instantiate(SplashFX);
                    attackEffect.transform.position = playerCharacter.transform.position;
                    attackEffect.transform.right = PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().GetAttackDirection();
                    attackEffect.transform.parent = playerCharacter.transform;
                    //attackEffect.GetComponentInChildren<HitBox>().hit.source = playerCharacter;

                    t = Time.time + 0.5f;

                    SimpleTutorialManager.Instance.AfterDeflectTutorial();
                }
            }
        }
        else if (Time.time > t)
            return "NoInput";

        
        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        playerCharacter.isInTutorial = true;

        t = 0;
    }

    public override void OnStateQuit(State nextState)
    {
        anim.Play("MainCharacter_Idle", -1, 0f);
        Destroy(attackEffect);
    }
}
