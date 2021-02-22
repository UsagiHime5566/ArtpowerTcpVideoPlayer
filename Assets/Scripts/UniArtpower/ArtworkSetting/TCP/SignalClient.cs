using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using UnityEngine.Events;
using System.Threading.Tasks;

public class SignalClient : HimeLib.SingletonMono<SignalClient> {
    public string serverIP = "127.0.0.1";
    public int serverPort = 25568;
    public int recvBufferSize = 1024;
    public string EndToken = "[/TCP]";

    [HimeLib.HelpBox] public string tip = "所有的訊息接編碼為UTF-8";
    public SocketSignalEvent OnSignalReceived;


    [Header("Auto Work")]
    public bool runInStart = false;



	TcpClient tcp_socket;
	NetworkStream net_stream;
	StreamWriter socket_writer;
	StreamReader socket_reader;
    string [] token;
	Thread connectThread;
    Action ActionQueue;

	async void Start()
    {
		token = new string[]{ EndToken };

        await Task.Delay(1000);

        if(this == null) return;

        if(!runInStart) return;

        InitSocket();
	}

    void Update(){
        if(ActionQueue != null){
            ActionQueue?.Invoke();
            ActionQueue = null;
        }
    }

	public async void InitSocket()
	{
        bool connect = false;
        await Task.Run(delegate {
            try
            {
                tcp_socket = new TcpClient(serverIP, serverPort);

                net_stream = tcp_socket.GetStream();
                socket_writer = new StreamWriter(net_stream);
                socket_reader = new StreamReader(net_stream);

                connect = true;
            }
            catch (Exception e)
            {
                Debug.LogError("Socket error: " + e);
                connect = false;
            }
        });
        
        if(!connect){
            RestartSocket();
            return;
        }

        //開啟一個線程連接，必須的，否則主線程卡死  
        connectThread = new Thread (ClientWork);
        connectThread.Start ();

        Debug.Log ($"Connect to Server ({serverIP}) at port :" + serverPort);
	}


	//Glass Client just need to Receive Touch data from Server
	void ClientWork()
	{
		while (true) {
			Thread.Sleep (20);

            byte[] data = new byte[recvBufferSize];
            int bytes = 0;
            
            try
            {
                // *** networkStream.Read will let programe get Stuck ***
                bytes = net_stream.Read(data, 0, data.Length);
            }
            catch (System.IO.IOException)
            {
                Debug.LogError("Disconnect Exception. Try to Reconned...");
                ActionQueue += delegate {
                    RestartSocket();
                };
                break;
            }

            if(bytes == 0)
            {
                Debug.Log("Disconnect Action. Try to Reconned...");
                ActionQueue += delegate {
                    RestartSocket();
                };
                break;
            }

            string responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            //Recieve Data Will Be   245,135,90[/TCP]   , str 不會包含[/TCP]
            string[] substrings = responseData.Split (token, StringSplitOptions.None);  // => 245,135,90

            // rs,x,y
            if (substrings.Length > 1) {
                Debug.Log($"TCP >> Recieved : {substrings[0]}");

                ActionQueue += delegate {
                    OnSignalReceived.Invoke(substrings[0]);
                };
            }
		}
	}

	public void SocketSend(string sendStr)
	{
        try {
            sendStr = sendStr + EndToken;
            socket_writer.Write(sendStr);
            socket_writer.Flush();

            Debug.Log ($"TCP >> Send: {sendStr}");
        }
        catch(System.Exception e){
            Debug.LogError(e.Message.ToString());
        }
	}

	public void CloseSocket()
	{
        if(connectThread != null)
        {
            connectThread.Interrupt();
			connectThread.Abort ();
        }

		socket_writer?.Close();
		socket_reader?.Close();
        net_stream?.Close();
		tcp_socket?.Close();

        print("diconnect.");
	}

    public async void RestartSocket(){
        CloseSocket();
        await Task.Delay(5000);
        if(this == null) return;
        InitSocket();
    }

    void OnApplicationQuit()
	{
		CloseSocket();
	}

    [Header("Signal Emulor")]
    public string signalForRecieved = "";
    public string signalForSend = "";
    [EasyButtons.Button] void EmuSignalRecieve(){
        OnSignalReceived?.Invoke(signalForRecieved);
    }
    [EasyButtons.Button] void EmuSignalSend(){
        SocketSend(signalForSend);
    }

    [Serializable]
    public class SocketSignalEvent : UnityEvent<string>
    {}
}
