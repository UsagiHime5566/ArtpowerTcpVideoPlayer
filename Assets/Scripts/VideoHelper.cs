using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(SignalClient))]
[RequireComponent(typeof(SignalClientHelper))]
public class VideoHelper : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string RecvSignalToPlay;
    public string SendSignalOnEnd;
    

    [Header("Auto Work")]
    public bool runInStart = false;


    SignalClientHelper signalClientHelper;
    SignalClient signalClient;

    void Awake(){
        signalClientHelper = GetComponent<SignalClientHelper>();
        signalClient = GetComponent<SignalClient>();
    }

    void Start()
    {
        signalClientHelper.OnVideoPathReady += SetupVideo;
        signalClient.OnSignalReceived.AddListener(SignalRecieved);
    }

    void SetupVideo(string filePath){
        videoPlayer.url = filePath;
        videoPlayer.loopPointReached += delegate {
            signalClient.SocketSend(SendSignalOnEnd);
        };

        if(!runInStart)
            return;
            
        videoPlayer.Play();
    }

    void SignalRecieved(string val){
        if(val == RecvSignalToPlay){
            videoPlayer.time = 0;
            videoPlayer.Play();
        }
    }
}
