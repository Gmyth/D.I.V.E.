using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUILoading : GUIWindow
{
    public GameObject tmp_loading;
    public GameObject tmp_levelinfo;
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
        int chapter = (index / 3) + 1;
        int level = index % 3;

        tmp_levelinfo.GetComponent<TextMeshProUGUI>().text = "Level " + chapter + "-" + level;

        tmp_levelinfo.SetActive(true);
      
    }
}
