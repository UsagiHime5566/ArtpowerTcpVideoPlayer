using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SignalClientHelper))]
public class VideoPlayerTip : MonoBehaviour
{
    public Text [] TXT_Targets;
    public string TipContent = "請將設定資訊檔({0})放置於專案目錄內";
    

    SignalClientHelper signalClientHelper;

    void Start()
    {
        signalClientHelper = GetComponent<SignalClientHelper>();

        foreach (var item in TXT_Targets)
        {
            item.text = string.Format(TipContent, signalClientHelper.fileName);
        }
    }
}
