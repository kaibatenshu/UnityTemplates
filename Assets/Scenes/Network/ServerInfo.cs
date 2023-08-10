using System;
using System.Collections;
using System.Collections.Generic;


public class ServerInfo{
    public string ip;
    public int port;

    public ServerInfo() { }
    public ServerInfo(string _ip,int _port)
    {
        ip = _ip;
        port = _port;
    }
}
