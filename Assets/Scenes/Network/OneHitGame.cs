using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

 
public class OneHitGame : OnehitCore {
    private List<ServerInfo> listIpConnect;
    public ServerInfo currentIPDetail;

    public OneHitGame(List<ServerInfo> _listIp, MessageSending _messageSending, int _addWaitTimemili) {
        addWaitTimemili = _addWaitTimemili;
        messageSending = _messageSending;
        listIpConnect=new List<ServerInfo>();
        for(int i = 0; i < _listIp.Count; i++)
            listIpConnect.Add(_listIp[i]);
    }
    public void RunNetwork(){
        TcpClient _tcpClient = null;
        for (int i = 0; i < listIpConnect.Count; i++) {
            currentIPDetail = listIpConnect[i];
            _tcpClient = ConnectIp(currentIPDetail.ip,currentIPDetail.port,1268);
            if(_tcpClient==null){
                //currentIPDetail.beingError = true;
                //currentIPDetail.countConnectionError ++;
                // #if TEST
                // Debug.LogWarning("--->Onehit error connect : "+currentIPDetail.ip+"("+currentIPDetail.port_onehit+")");
                // #endif
            }else{
                // #if TEST
                // Debug.LogWarning("Tạo kết nối thành công đến : "+currentIPDetail.ip+"("+currentIPDetail.port_onehit+")");
                // #endif                
                break;
            }
        }
        if (_tcpClient == null){
            #if TEST
            for (int i = 0; i < listIpConnect.Count; i++)
                Debug.LogError("TCP Socket Error ➞ "+listIpConnect[i].ip+":"+listIpConnect[i].port_onehit);
            #endif
            if(onError!=null)
                onError("Network error : "+messageSending.getCMD());
        }else
            ProcessTCP(_tcpClient);
    }
}
