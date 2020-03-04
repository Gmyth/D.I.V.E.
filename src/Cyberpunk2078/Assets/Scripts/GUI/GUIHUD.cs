using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GUIHUD : GUIWindow
{
    [Header("References")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image spBar;
    [SerializeField] private Image feverBar;
    [SerializeField] private Text textArea;
    [SerializeField] private GameObject resourceInspector;
    [SerializeField] private GUIDialogueWidget dialogueWidget;
    [SerializeField] private GUIItemWidget itemWidget;
    [SerializeField] private GUIEnemyWidget enemyWidget;

    [Header("Configuration")]
    [SerializeField] private Color feverBlinkColor = Color.yellow;
    [SerializeField] private float feverBlinkSpeed = 5;
    [SerializeField] private float textDuration = 5;
    [SerializeField] private float textSpeed = 0.02f;

    private PlayerCharacter playerCharacter;

    private Coroutine textCoroutine = null;
    private Coroutine feverCoroutine = null;

    [HideInInspector]
    public bool isInDialogue = false;

    public override void OnOpen(params object[] args)
    {
        dialogueWidget.Hide();
        enemyWidget.Hide();


        playerCharacter = (PlayerCharacter)args[0];
        Player player = Player.CurrentPlayer;


        UpdateMaxHp(Mathf.FloorToInt(playerCharacter[StatisticType.MaxHp]));
        UpdateMaxSp(Mathf.FloorToInt(playerCharacter[StatisticType.MaxSp] + playerCharacter[StatisticType.MaxOsp]));

        UpdateHp(Mathf.FloorToInt(playerCharacter[StatisticType.Hp]));
        UpdateSp(Mathf.FloorToInt(playerCharacter[StatisticType.Sp] + playerCharacter[StatisticType.Osp]));
        UpdateFever(Mathf.FloorToInt(playerCharacter[StatisticType.UltimateEnergy]));


        itemWidget.Refresh(player.inventory[0]);


        playerCharacter.OnStatisticChange.AddListener(HandleStatisticChange);

        player.inventory.OnItemChange.AddListener(UpdateItem);
    }

    public override void OnClose()
    {
        playerCharacter.OnStatisticChange.RemoveListener(HandleStatisticChange);

        Player.CurrentPlayer.inventory.OnItemChange.RemoveListener(UpdateItem);
    }


    public void ShowText(string text)
    {
        if (textCoroutine != null)
            StopCoroutine(textCoroutine);

        textCoroutine = StartCoroutine(ShowText(text, textDuration));
    }

    public void ShowDialogue(DialogueData dialogue)
    {
        resourceInspector.SetActive(false);
        dialogueWidget.Show(dialogue, HideDialogue);

    }

    public void ShowDialogue(DialogueData dialogue, params Action[] callbacks)
    {
        isInDialogue = true;
        Action[] newCallbacks = new Action[callbacks.Length + 1];
        callbacks.CopyTo(newCallbacks, 0);
        newCallbacks[callbacks.Length] = HideDialogue;


        resourceInspector.SetActive(false);
        dialogueWidget.Show(dialogue, newCallbacks);
    }

    public void HideDialogue()
    {
        resourceInspector.SetActive(true);
        dialogueWidget.Hide();
        isInDialogue = false;
    }


    public void ShowEnemyWidget(Enemy enemy)
    {
        enemyWidget.Show(enemy);
    }
    
    public void HideEnemyWidget()
    {
        enemyWidget.Hide();
    }


    public void HighlightFeverBar()
    {
        if (feverCoroutine != null)
            StopCoroutine(feverCoroutine);
        
        feverCoroutine = StartCoroutine(BlinkFeverBar());
    }

    public void DehighlightFeverBar()
    {
        if (feverCoroutine != null)
        {
            StopCoroutine(feverCoroutine);

            feverBar.color = Color.white;
        }
    }


    private void UpdateHp(int value)
    {
        hpBar.fillAmount = Mathf.Lerp(0.5f, 1, value / playerCharacter[StatisticType.MaxHp]);
    }

    private void UpdateMaxHp(int value)
    {
        hpBar.material.SetInt("_NumStride", Mathf.FloorToInt(value));
    }

    private void UpdateSp(int value)
    {
        spBar.fillAmount = Mathf.Lerp(0.5f, 1, value / (playerCharacter[StatisticType.MaxSp] + playerCharacter[StatisticType.MaxOsp]));
    }

    private void UpdateMaxSp(int value)
    {
        spBar.material.SetInt("_NumStride", Mathf.FloorToInt(value));
    }

    private void UpdateFever(int value)
    {
        feverBar.fillAmount = Mathf.Lerp(0.5f, 1, value / playerCharacter[StatisticType.MaxUltimateEnergy]);
    }


    private void UpdateItem(int id, uint currentValue, uint previousValue)
    {
        if (id == 0)
            itemWidget.Refresh(currentValue);
    }


    private void HandleStatisticChange(StatisticType statistic, float previousValue, float currentValue)
    {
        switch (statistic)
        {
            case StatisticType.Hp:
                UpdateHp(Mathf.RoundToInt(currentValue));
                break;


            case StatisticType.Sp:
                UpdateSp(Mathf.RoundToInt(currentValue + playerCharacter[StatisticType.Osp]));
                break;


            case StatisticType.Osp:
                UpdateSp(Mathf.RoundToInt(currentValue + playerCharacter[StatisticType.Sp]));
                break;


            case StatisticType.MaxSp:
                UpdateMaxSp(Mathf.FloorToInt(currentValue + playerCharacter[StatisticType.MaxOsp]));
                break;


            case StatisticType.MaxOsp:
                UpdateMaxSp(Mathf.FloorToInt(currentValue + playerCharacter[StatisticType.MaxSp]));
                break;


            case StatisticType.UltimateEnergy:
                UpdateFever(Mathf.RoundToInt(currentValue));
                break;
        }
    }


    private IEnumerator BlinkFeverBar()
    {
        AudioManager.Singleton.PlayOnce("EnergyFull");

        float t = Time.time;


        while (true)
        {
            feverBar.color = Color.Lerp(Color.white, feverBlinkColor, (Mathf.Sin(feverBlinkSpeed * (Time.time - t)) + 1) / 2);

            yield return null;
        }
    }

    private IEnumerator ShowText(string text, float waitTime)
    {
        float t = Time.time;

        int i = 0;

        textArea.text += "\n\n";

        while (i < text.Length)
        {
            while (i < (Time.time - t) / textSpeed && i < text.Length)
            {
                textArea.text += text[i];

                ++i;
            }

            yield return null;
        }


        yield return new WaitForSeconds(waitTime);


        textArea.text = "";


        yield break;
    }
}
