using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(SignalClient))]
[RequireComponent(typeof(SignalClientHelper))]
public class SignalPlayer : MonoBehaviour
{
    [HimeLib.HelpBox] public string tip = "使用 Client 訊號控制、設定影片撥放";
    public VideoPlayer videoPlayer;
    public string RecvSignalToPlay;
    public string SendSignalOnEnd;
    

    [Header("Auto Work")]
    public bool imidiatePlay = false;

    public System.Action<float, float> OnVideoPrepared;


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

    void Update(){
        if(Input.GetKeyDown(KeyCode.F5)){
            if(videoPlayer.isPrepared){
                videoPlayer.time = 0;
                videoPlayer.Play();
            }
        }
    }

    void SetupVideo(string filePath){
        videoPlayer.url = filePath;
        videoPlayer.Prepare();
        videoPlayer.loopPointReached += delegate {
            signalClient.SocketSend(SendSignalOnEnd);
        };
        videoPlayer.prepareCompleted += delegate {
            Debug.Log($"Get video size: {videoPlayer.texture.width}x{videoPlayer.texture.height}");
            OnVideoPrepared?.Invoke(videoPlayer.texture.width, videoPlayer.texture.height);
        };

        if(!imidiatePlay)
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
