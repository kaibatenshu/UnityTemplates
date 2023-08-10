using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkGlobal : MonoBehaviour{
    private System.Object syncLock;
    private Action actionUpdateUI;

    void Start(){
        syncLock = new System.Object();
    }



    public void setUpdateUI(Action _action){
        new Thread(new ThreadStart(()=>{
            lock(syncLock){
                while(actionUpdateUI!=null)
                    Thread.Sleep(1);
                actionUpdateUI=_action;
            }
        })).Start();
    }
    IEnumerator doActionUIGame(Action _action) { _action(); yield break; }


    void Update(){
        Action _action = actionUpdateUI;
        actionUpdateUI = null;
        if (_action != null)
            StartCoroutine(doActionUIGame(_action));




    }

    public void StartOnehit(MessageSending _messageSending, Action<MessageReceiving> _onFinished, int _addSleepWait = 0){
        StartOnehit(_messageSending, new ServerInfo("ipDefault",9999), _onFinished, _addSleepWait);//Server Default
    }
    public void StartOnehit(MessageSending _messageSending, ServerInfo serverInfo, Action<MessageReceiving> _onFinished, int _addSleepWait = 0){List<ServerInfo> listSV = new List<ServerInfo>();listSV.Add(serverInfo);StartOnehit(_messageSending, listSV, _onFinished, _addSleepWait);}
    public void StartOnehit(MessageSending _messageSending, List<ServerInfo> listServerInfo, Action<MessageReceiving> _onFinished, int _addSleepWait = 0){
        OneHitGame clientOnehit = new OneHitGame(listServerInfo, _messageSending, _addSleepWait);
        clientOnehit.onError = (n) => {
            #if TEST
            Debug.LogError("**************************************ERROR Onehit(SubServerDetail) : "+CMD_ONEHIT.getCMDName(_messageSending)+"➜"+n);
            #endif
            if (_onFinished != null)
                setUpdateUI(() => {
                    _onFinished(null);
                });
        };
        clientOnehit.onSuccess = () => {
            #if TEST
            Debug.LogWarning("Onehit : " + CMD_ONEHIT.getCMDName(_messageSending.getCMD()) + " " + _messageSending.avaiable() + " byte " + (clientOnehit.messageReceiving == null ? "" : ("➜ " + clientOnehit.messageReceiving.avaiable() + " byte")) + "   " + clientOnehit.currentIPDetail.ip + ":" + clientOnehit.currentIPDetail.port_onehit+" ("+clientOnehit.messageReceiving.timeProcess+" ms)");
            #endif
            if (_onFinished != null)
                setUpdateUI(() => {
                    _onFinished(clientOnehit.messageReceiving);
                    #if TEST
                    if(clientOnehit.messageReceiving.validate()==false)
                        Debug.LogError("Onehit chưa đọc hết "+clientOnehit.messageReceiving.avaiable()+" byte("+CMD_ONEHIT.getCMDName(_messageSending)+"➜"+clientOnehit.messageReceiving.lengthReceive()+")");
                    #endif
                });
        };
        new Thread(new ThreadStart(clientOnehit.RunNetwork)).Start();

    }


    #region RealTime
    /// <summary>
    /// Phần này thuộc real-time
    /// </summary>
    public short sessionId;
    public Action<MessageReceiving>[] listProcessRealtime;
    public Action onConnectRealtimeSuccess, onServerFull,onReconnect,onDisconnect;
    public Action<int> onNetworkError;
    public bool isPauseRealTime;

	public void SetProcessRealTime (short cmd, Action<MessageReceiving> onReceiveMessage){
        listProcessRealtime[cmd]=onReceiveMessage;
	}
    public void realtimeClearProcess(){
        listProcessRealtime=new Action<MessageReceiving>[short.MaxValue+1];
    }

    public RealTimeGame instanceRealTime;
    public bool isRealtime() { return instanceRealTime != null && instanceRealTime.IsRunning(); }
    public void RunRealTime (List<ServerInfo> listServerInfo, Action<int> _onCreateConnectionError, Action _onCreateConnectionSuccess, Action _onDisconnect, Action _onServerFull){
        #if TEST
        Debug.Log("<color=gray>Start Realtime</color> : "+_subServerDeail.getStringTrace());
        #endif
        if (instanceRealTime != null)
            instanceRealTime.StopNetwork();

        instanceRealTime = new RealTimeGame(listServerInfo);
        onNetworkError = _onCreateConnectionError;
        onConnectRealtimeSuccess = _onCreateConnectionSuccess;
        onDisconnect = _onDisconnect;
        onServerFull = _onServerFull;
        new Thread(new ThreadStart(instanceRealTime.RunNetwork)).Start();
    }
    
	public void SendMessageRealTime(MessageSending messageRealTime){
		if (messageRealTime == null || instanceRealTime==null)
			return;
        #if TEST
        if(messageRealTime.getCMD()==-4){
            Debug.LogWarning("<color=#FF6666>Thực hiện đóng kết nối</color>");
        }else{
            if(DataManager.instance.miniGameData.currentMiniGameDetail != null && DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType != IMiniGameInfo.Type.BoomOnline){
                Debug.LogWarning("<color=#FF6666>Send</color> "+CMD_REALTIME.getCMDName(messageRealTime)+" ➜ "+messageRealTime.avaiable()+" byte");
            }else{
                if(messageRealTime.getCMD() != CMD_REALTIME.C_GAMEPLAY_MOVE){
                    Debug.LogWarning("<color=#FF6666>Send</color> "+CMD_REALTIME.getCMDName(messageRealTime)+" ➜ "+messageRealTime.avaiable()+" byte");
                }
            }
        }
        #endif
        new Thread(new ThreadStart(()=>{instanceRealTime.Send(messageRealTime);})).Start();
    }
    public void StopRealTime (){
        if (instanceRealTime != null) {
            onDisconnect = null;
            SendMessageRealTime(new MessageSending(-4));
            instanceRealTime.StopNetwork();
        }
    }

    public void PauseReceiveMessage(){isPauseRealTime=true;}
    public void ResumeReceiveMessage(){isPauseRealTime=false;}
    #endregion



    private static NetworkGlobal ins = null;
    public static NetworkGlobal instance{
        get{
            if (ins == null){
                GameObject go = new GameObject("NetworkGlobal");
                ins = go.AddComponent<NetworkGlobal>();
                DontDestroyOnLoad(ins);
            }
            return ins;
        }
    }
}
