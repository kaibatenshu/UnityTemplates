using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

public class RealtimeTcp{
    public byte validateCode;
    public byte[] validateData;
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private byte[] PING;
    public bool isRunning;
    public RealtimeTcp(TcpClient tcp) {
        tcpClient = tcp;
        networkStream = tcp.GetStream();
        validateData = new byte[7];
        PING = new byte[] { 0, 1 };
        isRunning=true;
    }

    public int Send(MessageSending messageSending) {
        short lengSend = (short)messageSending.avaiable();
        byte[] dataSend = new byte[lengSend + 2];
        dataSend[0] = (byte)((int)((uint)lengSend >> 8) & 0xFF);
        dataSend[1] = (byte)((int)((uint)lengSend >> 0) & 0xFF);
        lengSend = (short)messageSending.avaiable();
        byte[] data = messageSending.getBytesArray();
        for (short i = 0; i < lengSend; i++)
            dataSend[i + 2] = (byte)(data[i] ^ validateCode);
        try {
            networkStream.Write(dataSend,0,dataSend.Length);
            return lengSend + 2;
        } catch {
            //Console.WriteLine(sc.Message.ToString());
            return 0;
        }
    }

    public void Ping() {
        try {
            networkStream.Write(PING,0,2);
        } catch {
            //Console.WriteLine(sc.Message.ToString());
        }
    }

    public int ReadByte() {
        try {
            return networkStream.ReadByte();
        } catch {
            //Console.WriteLine(sc.Message.ToString());
            return 0;
        }
    }public void Read(byte[] data,int length) {
        try {
            networkStream.Read(data, 0, length);
        } catch {
            //Console.WriteLine(sc.Message.ToString());
        }
    }
    public void Write(byte[] data, int length) {
        try {
            networkStream.Write(data, 0, length);
        } catch {
            //Console.WriteLine(sc.Message.ToString());
        }
    }

    public int Avaiable() {
        try {
            return tcpClient.Available;
        } catch {
            //Console.WriteLine(sc.Message.ToString());
            return 0;
        }
    }
    
    public bool Wait(int _length) { 
        for (int i = 0; i < 589; i++) 
            if(isRunning==false)
                return false;
            else if (tcpClient.Available < _length) 
                Thread.Sleep(1); 
            else 
                return true; 
        return false; 
    }
    public bool Wait3Second(int _length) { 
        for (int i = 0; i < 3689; i++) 
            if(isRunning==false)
                return false;
            else if (tcpClient.Available < _length) 
                Thread.Sleep(1); 
            else 
                return true; 
        return false; 
    }

    public void Clean() {
        networkStream.Close();
        tcpClient.Close();
    }
}
