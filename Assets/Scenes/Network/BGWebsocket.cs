using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading;

public class BGWebsocket {
    public Action onConnectSuccess;
    public Action onConnectFailure;
    public Action onClose;
    public bool isRunning;

    private ClientWebSocket client;
    private List<string> listUriConnect;
    private Object lockSend;

    private byte countIdSend, countIdReceive;
    private MessageSending[] bufferSending;
    public BGWebsocket(List<string> _listUriConnect){
        listUriConnect = _listUriConnect;
        
        lockSend = new Object();
        countIdSend = 0;
        countIdReceive = 0;
        bufferSending = new MessageSending[256];
    }

    private bool DoConnect(){
        foreach(string strUri in listUriConnect) {
            client = new ClientWebSocket();
            client.ConnectAsync(new Uri(strUri), CancellationToken.None);
            for (int i = 0; i < 300; i++)
                if (client.State == WebSocketState.Open)
                    return true;
                else
                    Thread.Sleep(5);
            client.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
            client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            client.Dispose();
        }
        return false;
    }

    public void send(MessageSending messageSending) {
        lock (lockSend){
            bufferSending[countIdSend++] = messageSending;
            client.SendAsync(messageSending.getBytesArray(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }

    private void ProcessRealtime(){
        while (isRunning && client.State==WebSocketState.Open){
            




        }
    }

    public void Start() {
        isRunning = true;
        if (!DoConnect()) {
            isRunning = false;
            if (onConnectFailure != null)
                onConnectFailure();
            if (onClose != null)
                onClose();
            return;
        }

        if (onConnectSuccess != null)
            onConnectSuccess();
        try { 
            ProcessRealtime();
        }finally { 
            isRunning = false;
            if (onClose != null)
                onClose();
            client.Dispose();
        }
    }



}