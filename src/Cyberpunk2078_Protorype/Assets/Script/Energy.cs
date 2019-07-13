using UnityEngine;
public enum EnergyLevel
{
    Poor = 0,
    Normal,
    OverCharged
}
public class Energy : MonoBehaviour
{
    [SerializeField] private float maskMinPos;
    [SerializeField] private float maskMaxPos;
    [SerializeField] private GameObject mask;
    
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject trail1;
    [SerializeField] private GameObject trail2;
    
    
    [SerializeField] private Color level1Color;
    [SerializeField] private Color level2Color;
    [SerializeField] private Color level3Color;
    
    [SerializeField] private Color backLevel1Color;
    [SerializeField] private Color backLevel2Color;
    [SerializeField] private Color backLevel3Color;

    private float velocity;
    private Animator anim;
   
    private float energyAmount;
    
    public float EnergyAmount
    {
        get { return energyAmount; }

        set
        {
            if (value == energyAmount)
            {
                Debug.LogWarning("Energy does not change!!!!");
            }
            else
            {
                
                energyAmount = Mathf.Max(Mathf.Max(value,0),Mathf.Min(value, 300));
                if (EnergyAmount > 100 && EnergyAmount <= 200)
                {
                    CurrentEnergyLevel = EnergyLevel.Normal;
                }
                else if(EnergyAmount <= 100)
                {
                    CurrentEnergyLevel = EnergyLevel.Poor;
                }else if (energyAmount > 200)
                {
                    CurrentEnergyLevel = EnergyLevel.OverCharged;
                }
            }
        }
    }
    
    private EnergyLevel currentEnergyLevel;

    public EnergyLevel CurrentEnergyLevel
    {
        get { return currentEnergyLevel; }

        private set
        {
            if (value == currentEnergyLevel)
            {
                Debug.LogWarning("Energy level does not change!!!!");
            }
            else
            {
                currentEnergyLevel = value;
                switch (currentEnergyLevel)
                {
                    case EnergyLevel.Poor:
                        anim.Play("ZhanLevel1",0);
                        GetComponent<SpriteRenderer>().color = level1Color;
                        trail1.GetComponent<SpriteRenderer>().color = level1Color;
                        trail1 .SetActive(false);
                        trail2.GetComponent<SpriteRenderer>().color = level1Color;
                        trail2.SetActive(false);
                        background.GetComponent<SpriteRenderer>().color = backLevel1Color;
                        break;
                    case EnergyLevel.Normal:
                        anim.Play("ZhanLevel2",0);
                        GetComponent<SpriteRenderer>().color = level2Color;
                        trail1.GetComponent<SpriteRenderer>().color = Color.black;;
                        trail2.GetComponent<SpriteRenderer>().color = Color.black;;
                        trail1 .SetActive(true);
                        trail2.SetActive(true);
                        background.GetComponent<SpriteRenderer>().color = backLevel2Color;
                        break;
                    case EnergyLevel.OverCharged:
                        anim.Play("ZhanLevel3",0);
                        GetComponent<SpriteRenderer>().color = level3Color;
                        trail1.GetComponent<SpriteRenderer>().color = Color.black;
                        trail2.GetComponent<SpriteRenderer>().color = Color.black;;
                        trail1 .SetActive(true);
                        trail2.SetActive(true);
                        background.GetComponent<SpriteRenderer>().color = backLevel3Color;
                        break;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        currentEnergyLevel = EnergyLevel.Poor;
        energyAmount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        syncEnergyWithIcon();
    }

    private void syncEnergyWithIcon()
    {
        float percentage = (EnergyAmount - 100 * (int)CurrentEnergyLevel) / 100;
        float distance = maskMaxPos - maskMinPos;
        float targetY = maskMinPos + distance * percentage;
        targetY = Mathf.SmoothDamp(mask.transform.localPosition.y, targetY,ref velocity, 0.2f);
        mask.transform.localPosition = new Vector3(
            mask.transform.localPosition.x,
            targetY,
            mask.transform.localPosition.z
        );
    }

    public void changeEnergyAmount( float amountToChange)
    {
        EnergyAmount += amountToChange;
//        if (EnergyAmount + amountToChange>= 100)
//        {
//            // add
//            if (EnergyAmount + amountToChange >200 && CurrentEnergyLevel == EnergyLevel.OverCharged)
//            {
//                // do nothing
//            }else{
//                EnergyAmount += amountToChange;
//                CurrentEnergyLevel += 1;
//            }
//        }else if (EnergyAmount + amountToChange <= 0 ){
//            // reduce
//            if (CurrentEnergyLevel != EnergyLevel.Poor)
//            {
//                EnergyAmount = 100;
//                CurrentEnergyLevel += 1;
//            }
//        }else{
//                EnergyAmount += amountToChange;
//        }
    }
}
