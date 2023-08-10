using System;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class RealTimeGame : RealtimeCore {
    private List<ServerInfo> listIpConnect;
    private ServerInfo currentIPDetail;
    
    private int currentReceiveBuffer,indexReceiveBuffer;
    private MessageReceiving[] bufferReceiving;
    public RealTimeGame(List<ServerInfo> _listIp) {
        listIpConnect = _listIp;
        sessionId = -1;

        currentReceiveBuffer=0;
        indexReceiveBuffer=0;
        bufferReceiving=new MessageReceiving[short.MaxValue];
    }

    public MessageReceiving GetMessageReceiving(){
        MessageReceiving _mgReceive = bufferReceiving[indexReceiveBuffer];
        if(_mgReceive==null)
            return null;
        indexReceiveBuffer++;
        if(indexReceiveBuffer==short.MaxValue)
            indexReceiveBuffer=0;
        return _mgReceive;
    }

    public void RunNetwork(){
        CreateSocket = ()=>{
            TcpClient _tcpClient = null;
            int _numberIpConnect = listIpConnect.Count;
            for (int i = 0; i < _numberIpConnect; i++) {
                currentIPDetail = listIpConnect[i];
                #if TEST
                if(string.IsNullOrEmpty(NetworkGlobal.TEST_IP)==false){
                    Debug.LogWarning("<color=#FFFF00>"+currentIPDetail.ip+"</color> → "+NetworkGlobal.TEST_IP);
                    currentIPDetail.ip = NetworkGlobal.TEST_IP;
                    currentIPDetail.port_onehit = NetworkGlobal.TEST_ONEHIT;
                    currentIPDetail.port_realtime = NetworkGlobal.TEST_REALTIME;
                }
                Debug.Log(">>> " + currentIPDetail.ip + ":" + currentIPDetail.port_realtime);
                #endif
                _tcpClient=GetConnect(currentIPDetail.ip,currentIPDetail.port,1268);
                if(_tcpClient==null){
                    //currentIPDetail.beingError = true;
                    //currentIPDetail.countConnectionError ++;
                }else{
                    #if TEST
                    Debug.LogWarning("Realtime create connect success : "+currentIPDetail.ip+"("+currentIPDetail.port_onehit+")");
                    #endif   
                    return _tcpClient;
                }
            }
            #if TEST
            Debug.Log(">>> Realtime connect error");
            #endif
            return null;
        };

        onConnectSuccess =  ()=> {
#if TEST
            Debug.Log("<color=yellow>Realtime connect success</color> sessionId("+sessionId+") " + currentIPDetail.ip + ":" + currentIPDetail.port_realtime);
#endif
            NetworkGlobal.instance.sessionId = sessionId;
            NetworkGlobal.instance.setUpdateUI(NetworkGlobal.instance.onConnectRealtimeSuccess);
            NetworkGlobal.instance.onConnectRealtimeSuccess = null;
        };
        onClose =           ()=>{NetworkGlobal.instance.setUpdateUI(NetworkGlobal.instance.onDisconnect);};
        onNetworkError =    (_errCode)=>{if(NetworkGlobal.instance.onNetworkError!=null)NetworkGlobal.instance.setUpdateUI(()=>{NetworkGlobal.instance.onNetworkError(_errCode);});};
        onServerFull =      ()=>{NetworkGlobal.instance.setUpdateUI(NetworkGlobal.instance.onServerFull);};
        onReconnect =       ()=>{NetworkGlobal.instance.setUpdateUI(NetworkGlobal.instance.onReconnect);};

        onReceive = (_message)=>{
            bufferReceiving[currentReceiveBuffer++]=_message;
            if(currentReceiveBuffer==short.MaxValue)
                currentReceiveBuffer=0;
        };

        ProcessTCP();
    }

    public bool IsRunning(){
        return !(myTCP==null || myTCP.isRunning==false);
    }
    public void StopNetwork(){
        #if TEST
        Debug.LogError("Gọi hàm StopNetwork");
        #endif
        if(myTCP!=null)
            myTCP.isRunning = false;
    }
}
