using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUILoading : GUIWindow
{
    public GameObject tmp_loading;
    public GameObject tmp_levelinfo;

    public LevelTitle levelEntry;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Animation());
    }
    
    IEnumerator Animation()
    {

        tmp_loading.GetComponent<TextMeshProUGUI>().text = "Loading";

        yield return new WaitForSeconds(0.3f);
        tmp_loading.GetComponent<TextMeshProUGUI>().text = "Loading.";

        yield return new WaitForSeconds(0.3f);
        tmp_loading.GetComponent<TextMeshProUGUI>().text = "Loading..";

        yield return new WaitForSeconds(0.3f);
        tmp_loading.GetComponent<TextMeshProUGUI>().text = "Loading...";

        yield return null;

        tmp_loading.SetActive(false);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void LevelInfoAnimation(int index)
    {
        //LevelInfo info = GameProcessManager.Singleton.GetLevelInfo();

        for(int i=0; i < levelEntry.SerializedEntries.Count; i++)
        {
            if (levelEntry.SerializedEntries[i].id == index)
            {
                tmp_levelinfo.GetComponent<TextMeshProUGUI>().text = levelEntry.SerializedEntries[i].name;
                break;
            }
            else
                tmp_levelinfo.GetComponent<TextMeshProUGUI>().text = "ERROR";


        }
        // tmp_levelinfo.GetComponent<TextMeshProUGUI>().color = 

        //int chapter = (index % 3 == 0) ? (index / 3) : (index / 3) + 1;
        //int level = index - 3 * (chapter-1);
        //if (index == 5)
        //{
        //    tmp_levelinfo.GetComponent<TextMeshProUGUI>().text = "LEVEL 1-4";
        //    return;
        //}

        //tmp_levelinfo.GetComponent<TextMeshProUGUI>().text = "Level " + chapter + "-" + level;

        tmp_levelinfo.SetActive(true);
    }
}
