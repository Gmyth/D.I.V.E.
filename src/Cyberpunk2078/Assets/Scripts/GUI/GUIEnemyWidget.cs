using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;


public class GUIEnemyWidget : GUIWidget
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider fatigueBar;

    private Enemy enemy;
    
    
    public void Show(Enemy enemy)
    {
        Show();
        
        this.enemy = enemy;
        

        UpdateHp(enemy[StatisticType.Hp]);
        UpdateFatigue(enemy[StatisticType.Fatigue]);
        
        
        enemy.OnStatisticChange.AddListener(UpdateStatistic);
    }

    public override void Hide()
    {
        base.Hide();
        
        
        enemy?.OnStatisticChange.RemoveListener(UpdateStatistic);
    }


    private void UpdateStatistic(StatisticType type, float previousValue, float currentValue)
    {
        switch (type)
        {
            case StatisticType.Hp:
                UpdateHp(currentValue);
                break;
            
            
            case StatisticType.Fatigue:
                UpdateFatigue(currentValue);
                break;
        }
    }


    private void UpdateHp(float value)
    {
        if (value == 0)
            Hide();
        else
            hpBar.value = value / enemy[StatisticType.MaxHp];
    }
    
    private void UpdateFatigue(float value)
    {
        float maxValue = enemy[StatisticType.MaxFatigue];
        
        if (maxValue == 0)
            fatigueBar.gameObject.SetActive(false);
        else
        {
            fatigueBar.value = value / enemy[StatisticType.MaxFatigue];
            
            fatigueBar.gameObject.SetActive(true);
        }
    }
}
