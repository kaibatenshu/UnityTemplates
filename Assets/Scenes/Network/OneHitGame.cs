using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

 
public class OneHitGame : OnehitCore {
    private List<ServerInfo> listIpConnect;
    public ServerInfo currentIPDetail;

    public OneHitGame(List<ServerInfo> _listIp, MessageSending _messageSending, bool _isOnlySend, int _addWaitTimemili) {
        isOnlySend = _isOnlySend;
        addWaitTimemili = _addWaitTimemili;
        messageSending = _messageSending;
        listIpConnect=new List<ServerInfo>();
        for(int i = 0; i < _listIp.Count; i++)
            listIpConnect.Add(_listIp[i]);
    }
    public void RunNetwork(){
        for (int i = 0; i < listIpConnect.Count; i++) {
            currentIPDetail = listIpConnect[i];
            string logError = ConnectIp(currentIPDetail.ip,currentIPDetail.port,1268);
            if(logError == null) { 
                #if TEST
                Debug.LogWarning("successfully connected to " + currentIPDetail.ip+":"+currentIPDetail.port);
                #endif                
                ProcessTCP();
                return;
            }else{
                #if TEST
                Debug.LogWarning("--->Onehit error connect : "+currentIPDetail.ip+"("+currentIPDetail.port+")");
                #endif
            }
        }

        if(onError!=null)
            onError("Network error : "+messageSending.getCMD());
    }
}
