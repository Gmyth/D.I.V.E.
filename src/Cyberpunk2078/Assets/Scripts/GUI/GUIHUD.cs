using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUIHUD : GUIWindow
{
    [Header("References")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image spBar;
    [SerializeField] private Image feverBar;

    private PlayerCharacter player;


    public override void OnOpen(params object[] args)
    {
        player = (PlayerCharacter)args[0];


        UpdateMaxHp(Mathf.FloorToInt(player[StatisticType.MaxHp]));
        UpdateMaxSp(Mathf.FloorToInt(player[StatisticType.MaxSp] + player[StatisticType.MaxOsp]));

        UpdateHp(Mathf.FloorToInt(player[StatisticType.Hp]));
        UpdateSp(Mathf.FloorToInt(player[StatisticType.Sp] + player[StatisticType.Osp]));
        UpdateFever(Mathf.FloorToInt(player[StatisticType.Fever]));


        player.OnStatisticChange.AddListener(HandleStatisticChange);
    }

    public override void OnClose()
    {
        player.OnStatisticChange.RemoveListener(HandleStatisticChange);
    }

    
    private void UpdateHp(int value)
    {
        hpBar.fillAmount = Mathf.Lerp(0.5f, 1, value / player[StatisticType.MaxHp]);
    }

    private void UpdateMaxHp(int value)
    {
        hpBar.material.SetInt("_NumStride", Mathf.FloorToInt(value));
    }

    private void UpdateSp(int value)
    {
        spBar.fillAmount = Mathf.Lerp(0.5f, 1, value / (player[StatisticType.MaxSp] + player[StatisticType.MaxOsp]));
    }

    private void UpdateMaxSp(int value)
    {
        spBar.material.SetInt("_NumStride", Mathf.FloorToInt(value));
    }

    private void UpdateFever(int value)
    {
        feverBar.fillAmount = Mathf.Lerp(0.5f, 1, value / player[StatisticType.MaxFever]);
    }


    private void HandleStatisticChange(StatisticType statistic, float previousValue, float currentValue)
    {
        switch (statistic)
        {
            case StatisticType.Hp:
                UpdateHp(Mathf.RoundToInt(currentValue));
                break;


            case StatisticType.Sp:
                UpdateSp(Mathf.RoundToInt(currentValue + player[StatisticType.Osp]));
                break;


            case StatisticType.Osp:
                UpdateSp(Mathf.RoundToInt(currentValue + player[StatisticType.Sp]));
                break;


            case StatisticType.MaxSp:
                UpdateMaxSp(Mathf.FloorToInt(currentValue + player[StatisticType.MaxOsp]));
                break;


            case StatisticType.MaxOsp:
                UpdateMaxSp(Mathf.FloorToInt(currentValue + player[StatisticType.MaxSp]));
                break;


            case StatisticType.Fever:
                UpdateFever(Mathf.RoundToInt(currentValue));
                break;
        }
    }
}
