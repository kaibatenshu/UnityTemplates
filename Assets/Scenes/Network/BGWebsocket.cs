using System;
using System.Collections;
using System.Collections.Generic;


public class BGWebsocket{
    private List<ServerInfo> listServer;
    private ServerInfo currentserver;

    private int currentReceiveBuffer, indexReceiveBuffer;
    private MessageReceiving[] bufferReceiving;
    public BGWebsocket(List<ServerInfo> _listIp){
        listServer = _listIp;

        currentReceiveBuffer = 0;
        indexReceiveBuffer = 0;
        bufferReceiving = new MessageReceiving[short.MaxValue];
    }





}
