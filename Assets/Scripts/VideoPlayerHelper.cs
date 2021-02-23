using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerHelper : MonoBehaviour
{
    [HimeLib.HelpBox] public string tip = "將 Video 內容播放出來";
    public SignalPlayer targetSignalPlayer;
    public RenderTexture render1920;
    public RenderTexture render3840;
    public RawImage [] targetCanvas;
    public Camera [] targetDisplay;

    public float judgeSpeed = 50;

    
    // private work
    VideoPlayer videoPlayer;
    RenderTexture targetRender;
    float videoWidth;
    float videoHeight;
    int curSetMonitor = 0;

    void Awake(){
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        targetSignalPlayer.OnVideoPrepared += SetupVideoSize;

        targetCanvas[0].rectTransform.anchoredPosition = SystemConfig.Instance.GetData<Vector2>("canvas0_pos");
        targetCanvas[1].rectTransform.anchoredPosition = SystemConfig.Instance.GetData<Vector2>("canvas1_pos");
        targetCanvas[2].rectTransform.anchoredPosition = SystemConfig.Instance.GetData<Vector2>("canvas2_pos");

        targetDisplay[0].targetDisplay = SystemConfig.Instance.GetData<int>("monitor0_dp");
        targetDisplay[1].targetDisplay = SystemConfig.Instance.GetData<int>("monitor1_dp");
        targetDisplay[2].targetDisplay = SystemConfig.Instance.GetData<int>("monitor2_dp");
    }

    void Update() {
        if(Input.GetKey(KeyCode.Q)){
            targetCanvas[0].rectTransform.TranslateAnchor(-judgeSpeed * Time.deltaTime, 0);
            SystemConfig.Instance.SaveData("canvas0_pos", targetCanvas[0].rectTransform.anchoredPosition);
        }
        if(Input.GetKey(KeyCode.W)){
            targetCanvas[0].rectTransform.TranslateAnchor(judgeSpeed * Time.deltaTime, 0);
            SystemConfig.Instance.SaveData("canvas0_pos", targetCanvas[0].rectTransform.anchoredPosition);
        }
        if(Input.GetKey(KeyCode.A)){
            targetCanvas[1].rectTransform.TranslateAnchor(-judgeSpeed * Time.deltaTime, 0);
            SystemConfig.Instance.SaveData("canvas1_pos", targetCanvas[1].rectTransform.anchoredPosition);
        }
        if(Input.GetKey(KeyCode.S)){
            targetCanvas[1].rectTransform.TranslateAnchor(judgeSpeed * Time.deltaTime, 0);
            SystemConfig.Instance.SaveData("canvas1_pos", targetCanvas[1].rectTransform.anchoredPosition);
        }
        if(Input.GetKey(KeyCode.Z)){
            targetCanvas[2].rectTransform.TranslateAnchor(-judgeSpeed * Time.deltaTime, 0);
            SystemConfig.Instance.SaveData("canvas2_pos", targetCanvas[2].rectTransform.anchoredPosition);
        }
        if(Input.GetKey(KeyCode.X)){
            targetCanvas[2].rectTransform.TranslateAnchor(judgeSpeed * Time.deltaTime, 0);
            SystemConfig.Instance.SaveData("canvas2_pos", targetCanvas[2].rectTransform.anchoredPosition);
        }
        if(Input.GetKeyDown(KeyCode.E)){
            curSetMonitor = 0;
        }
        if(Input.GetKeyDown(KeyCode.D)){
            curSetMonitor = 1;
        }
        if(Input.GetKeyDown(KeyCode.C)){
            curSetMonitor = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)){
            targetDisplay[curSetMonitor].targetDisplay = 0;
            SystemConfig.Instance.SaveData($"monitor{curSetMonitor}_dp", 0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)){
            targetDisplay[curSetMonitor].targetDisplay = 1;
            SystemConfig.Instance.SaveData($"monitor{curSetMonitor}_dp", 1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)){
            targetDisplay[curSetMonitor].targetDisplay = 2;
            SystemConfig.Instance.SaveData($"monitor{curSetMonitor}_dp", 2);
        }
    }

    void SetupVideoSize(float width, float height){
        videoWidth = width;
        videoHeight = height;

        if(width > 1920){
            targetRender = render3840;
            ActiveMultiScreen();
        }
        else
            targetRender = render1920;


        // Target RenderTexture Settings
        foreach (var item in targetCanvas)
        {
            item.texture = targetRender;
            item.rectTransform.sizeDelta = new Vector2(targetRender.width, 1080);
        }
        videoPlayer.targetTexture = targetRender;
    }

    void ActiveMultiScreen(){
        Debug.Log("displays connected: " + Display.displays.Length);
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}
