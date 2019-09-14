using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{

    public bool isTheLastTimeline = false;

    [SerializeField] private PlayableDirector[] timelines;
    [SerializeField] private int currentIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        timelines = GetComponentsInChildren<PlayableDirector>();
    }

    public void OnTimelineStart() 
    {
        //When timeline start

        GetComponent<Collider2D>().enabled = false;
        //Stop gameplay

        PlayTimelineInIndex(0);
    }

    public void OnTimelineEnd()
    {
        //When timeline end
        gameObject.SetActive(false);

        //Recover gameplay
    }

    public void PlayCurrentTimeline() 
    {
        timelines[currentIndex].Play();
        if (currentIndex == timelines.Length - 1) {
            isTheLastTimeline = true;
        }
    }

    public void PlayNextTimeline()
    {
        currentIndex++;
        if (currentIndex < timelines.Length) {
            PlayCurrentTimeline();
        }
        else {
            currentIndex = 0;
            Debug.LogError("Next Timeline is null");
        }
    }

    public void PlayTimelineInIndex(int index) 
    {
        if (index < timelines.Length) {
            currentIndex = index;
            PlayCurrentTimeline();
        }
        else {
            currentIndex = 0;
            Debug.LogError("No Timeline in index " + index);
        }
    }

    public void PauseTimeline() 
    {
        timelines[currentIndex].Pause();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "Player")
        {
            OnTimelineStart();
        }
    }

    public void PlayDialogue(int index)
    {
        DialogueManager.Instance.SetTimelineManager(this);
        StartCoroutine(DialogueManager.Instance.PlayDialogue(index, transform));
    }

}
