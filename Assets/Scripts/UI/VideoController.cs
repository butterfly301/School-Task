using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoController : MonoBehaviour 
{
    public VideoPlayer videoPlayer;
    public UnityEvent onVideoPlaying;
    //public CanvasGroup mainMenuCanvasGroup;
    
    void Start() 
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onVideoPlaying.Invoke();
        }
    }

    void OnVideoFinished(VideoPlayer vp) 
    {
        videoPlayer.gameObject.SetActive(false);
        onVideoPlaying.Invoke();
        //initialLoad.GetInPersistent();
        //mainMenuCanvasGroup.gameObject.SetActive(true);
    }
    
}