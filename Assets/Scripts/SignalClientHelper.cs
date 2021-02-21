using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

[RequireComponent(typeof(SignalClient))]
public class SignalClientHelper : MonoBehaviour
{
    public string fileName = "Loader.txt";
    public System.Action<string> OnVideoPathReady;
    
    
    //Root dictionary of application , without '/'
    string exePath, filePath;
    SignalClient signalClient;

    void Awake(){
        signalClient = GetComponent<SignalClient>();
        exePath = Path.GetDirectoryName(Application.dataPath);
        filePath = exePath + "/" + fileName;
    }

    async void Start()
    {
        await Task.Yield();
        ReadFileForClient();
    }

    void ReadFileForClient(){
        if(!File.Exists(filePath))
            return;

        StreamReader reader = new StreamReader(filePath); 
        string _serverIP = reader.ReadLine();
        string _serverPort = reader.ReadLine();
        string _videoName = "file://" + reader.ReadLine();
        string _buffSize = reader.ReadLine();
        string _EndToken = reader.ReadLine();
        reader.Close();

        if(!string.IsNullOrEmpty(_serverIP))
            signalClient.serverIP = _serverIP;
        
        if(!string.IsNullOrEmpty(_EndToken))
            signalClient.EndToken = _EndToken;

        int serverPort = 25566;
        if(int.TryParse(_serverPort, out serverPort))
            signalClient.serverPort = serverPort;
        int buffSize = 1024;
        if(int.TryParse(_buffSize, out buffSize))
            signalClient.recvBufferSize = buffSize;

        OnVideoPathReady?.Invoke(_videoName);
    }
}
