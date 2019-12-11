using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{

    public bool isPlayInstantly = false;
    public bool isTheLastTimeline = false;

    [SerializeField] private PlayableDirector[] timelines;
    [SerializeField] private int currentIndex = 0;

    public bool isPlaying = false;
    public bool isTheLastDialogue = false;

    [SerializeField] private Color debugColor;

    // Start is called before the first frame update
    void Start()
    {
        timelines = GetComponentsInChildren<PlayableDirector>();
    }

    public void OnTimelineStart() 
    {
        if (!isPlaying)
        {
            //GetComponent<Collider2D>().enabled = false;

            PlayTimelineInIndex(0);
            isPlaying = true;
        }
    }

    public void OnTimelineEnd(int playerExitState)
    {
        //When timeline end
        //gameObject.SetActive(false);

        //Recover gameplay
        player.EndTimelineWithPlayerExitState(playerExitState);
    }

    public void PlayCurrentTimeline() 
    {
        if (currentIndex <= timelines.Length - 1)
        {
            timelines[currentIndex].Play();
        }
        else
        {
            Debug.LogError("Current Timeline is null!!!  current index: " + currentIndex);
        }



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

    private DialoguePlayer player;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "Player")
        {
            other.gameObject.GetComponent<DialoguePlayer>().CurrentTimelineManager = this;
        }
    }

    public void PlayDialogue(int index)
    {
        DialogueManager.Instance.SetTimelineManager(this);
        StartCoroutine(DialogueManager.Instance.PlayDialogue(index));
    }

    public bool CheckEndState() {
        return isTheLastTimeline && isTheLastDialogue;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        //Gizmos.DrawCube(transform.position, GetComponent<BoxCollider2D>().size);
    }

}
