using System;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;

public class RealtimeCore{
    private const long DELAY_TIME_OUT = 30000;

    public const int ERRORCODE_CREATE_SOCKET        = 1;
    public const int ERRORCODE_SERVER_FULL          = 2;
    public const int ERRORCODE_LENGTH_SERVER_ERROR  = 3;
    public const int ERROR_CMD                      = 4;

    public const int ERRORCODE_SOCKET_TIMEOUT_1     = 101;
    public const int ERRORCODE_SOCKET_TIMEOUT_2     = 102;
    public const int ERRORCODE_SOCKET_TIMEOUT_3     = 103;
    
    public Func<TcpClient> CreateSocket;
    public Action<MessageReceiving> onReceive;
    public Action onServerFull,onConnectSuccess,onReconnect,onClose;
    public Action<int> onNetworkError;
    public long ping;
    public short sessionId;
    private System.Object syncLock;
    private byte countIdSend;
    private byte countIdReceive;
    private MessageSending[] bufferSending;
    protected RealtimeTcp myTCP;
    public TcpClient GetConnect(String _ip,int _port,long TIME_OUT) {
        TcpClient _tcpClient;
        if (_ip.Contains(":"))
            _tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
        else
            _tcpClient = new TcpClient();
        _tcpClient.NoDelay=true;
        _tcpClient.BeginConnect(_ip, _port, null, null);
        for(int i=0;i<TIME_OUT/5;i++)
            if (_tcpClient.Connected)
                return _tcpClient;
            else
                Thread.Sleep(5);
        #if TEST
        Debug.LogError(">>> ErrorConnect : " + _ip + " (" +_port+")");
        #endif
        _tcpClient.Close();
        return null;
    }

    
    public void Send(MessageSending messageSending) {
        lock (syncLock) {
            bufferSending[countIdSend++] = messageSending;
            myTCP.Send(messageSending);
        }
    }

    public void ProcessTCP() {
        TcpClient tcpClient=CreateSocket();
        if(tcpClient==null){
            if(onNetworkError!=null)
                onNetworkError(ERRORCODE_CREATE_SOCKET);
            if (onClose != null)
                onClose();
            return;
        }

        syncLock = new System.Object();
        countIdSend = 0;
        countIdReceive = 0;
        bufferSending = new MessageSending[256];

        myTCP = new RealtimeTcp(tcpClient);
        runRealtime();
        sessionId=-1;
        myTCP.Clean();
        #if TEST
        Debug.LogWarning(">>Realtime release socket");
        #endif
        if (onClose != null)
            onClose();
    }
    
    private int runRealtime() {
        byte[] datatransfer=new byte[8192];
        if (myTCP.Wait3Second(8))
            myTCP.Read(datatransfer, 8);
        else
            return ERRORCODE_SOCKET_TIMEOUT_1;

        #if TEST
        Debug.LogWarning(">>Realtime : receive handShake");
        #endif

        byte validateCode = datatransfer[5];
        myTCP.validateCode = validateCode;
        myTCP.validateData[0] = (byte)(datatransfer[0] ^ validateCode);
        myTCP.validateData[1] = (byte)(datatransfer[1] ^ validateCode);
        myTCP.validateData[2] = (byte)(datatransfer[2] ^ validateCode);
        myTCP.validateData[3] = (byte)(datatransfer[3] ^ validateCode);
        myTCP.validateData[4] = (byte)(datatransfer[4] ^ validateCode);
        myTCP.validateData[5] = (byte)(datatransfer[6] ^ validateCode);
        myTCP.validateData[6] = (byte)(datatransfer[7] ^ validateCode);

        datatransfer[0] = myTCP.validateData[0];
        datatransfer[1] = myTCP.validateData[1];
        datatransfer[2] = myTCP.validateData[2];
        datatransfer[3] = myTCP.validateData[3];
        datatransfer[4] = myTCP.validateData[4];
        datatransfer[5] = myTCP.validateData[5];
        datatransfer[6] = myTCP.validateData[6];
        
        datatransfer[7] = 255;
        datatransfer[8] = 255;
        myTCP.Write(datatransfer, 9);

        if (myTCP.Wait3Second(2))
            myTCP.Read(datatransfer, 2);
        else
            return ERRORCODE_SOCKET_TIMEOUT_2;

        int ch1 = datatransfer[0] & 0xFF;
        int ch2 = datatransfer[1] & 0xFF;
        short length = (short)((ch1 << 8) + (ch2 << 0));
        if (length == -7){
            if(onServerFull!=null)
                onServerFull();
            return ERRORCODE_SERVER_FULL;
        }else if (length < 0)
            return length;
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #if TEST
        Debug.LogWarning(">>Connect success");
        #endif
        sessionId = length;
        if (onConnectSuccess != null)
            onConnectSuccess();
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        long timeOut = DateTimeUtil.currentTimeMillis + DELAY_TIME_OUT;
        long lastTimePing = DateTimeUtil.currentTimeMillis;
        byte[] dataSend;
        long nextTimeReconnect=0;
        while(myTCP.isRunning && DateTimeUtil.currentTimeMillis<timeOut) {
            if (DateTimeUtil.currentTimeMillis-lastTimePing>1000) {
                myTCP.Ping();    
                lastTimePing = DateTimeUtil.currentTimeMillis; 
            }

            if (myTCP.Avaiable() > 1) {
                myTCP.Read(datatransfer, 2);
                ch1 = datatransfer[0] & 0xFF;
                ch2 = datatransfer[1] & 0xFF;
                length = (short)((ch1 << 8) + (ch2 << 0));

                if (length < 1)
                    return ERRORCODE_LENGTH_SERVER_ERROR;
                else if (length == 1) {
                    ping = DateTimeUtil.currentTimeMillis - lastTimePing;
                    timeOut = DateTimeUtil.currentTimeMillis + DELAY_TIME_OUT;
                    //Console.WriteLine("  PING : "+ping+"===>"+sessionId);
                } else {
                    if (myTCP.Wait(length))
                        myTCP.Read(datatransfer, length);
                    else
                        return ERRORCODE_SOCKET_TIMEOUT_3;

                    dataSend = new byte[length];
                    for (short i = 0; i < length; i++)
                        dataSend[i] = (byte)(datatransfer[i]^myTCP.validateCode);

                    onReceive(new MessageReceiving(dataSend));

                    lastTimePing = DateTimeUtil.currentTimeMillis;
                    timeOut = DateTimeUtil.currentTimeMillis + DELAY_TIME_OUT;
                    countIdReceive++;
                }
                continue;
            }
            if (DateTimeUtil.currentTimeMillis+DELAY_TIME_OUT-3000 > timeOut && DateTimeUtil.currentTimeMillis> nextTimeReconnect) {
                nextTimeReconnect = DateTimeUtil.currentTimeMillis + 1000;
                ReconnectSetup();
            }
            Thread.Sleep(1);
        }
        return 0;
    }

    private void ReconnectSetup() {
        TcpClient tcpReconnect = CreateSocket();
        if (tcpReconnect == null)
            return;
        RealtimeTcp newReconnect = new RealtimeTcp(tcpReconnect);
        newReconnect.validateCode = myTCP.validateCode;
        newReconnect.validateData = myTCP.validateData;
        if (ReconnectProcess(newReconnect)){
            if(onReconnect!=null)
                onReconnect();
        } else
            newReconnect.Clean();
    }

    private bool ReconnectProcess(RealtimeTcp tcpRe) {
        byte[] datatransfer = new byte[17];
        if (tcpRe.Wait3Second(8))
            tcpRe.Read(datatransfer, 8);
        else
            return false;

        byte vData = datatransfer[5];
        datatransfer[0] = (byte)(datatransfer[0] ^ vData);
        datatransfer[1] = (byte)(datatransfer[1] ^ vData);
        datatransfer[2] = (byte)(datatransfer[2] ^ vData);
        datatransfer[3] = (byte)(datatransfer[3] ^ vData);
        datatransfer[4] = (byte)(datatransfer[4] ^ vData);
        datatransfer[5] = (byte)(datatransfer[6] ^ vData);
        datatransfer[6] = (byte)(datatransfer[7] ^ vData);
        short lengthSend = sessionId;
        datatransfer[7] = (byte)((int)((uint)lengthSend >> 8) & 0xFF);
        datatransfer[8] = (byte)((int)((uint)lengthSend >> 0) & 0xFF);

        datatransfer[9] = tcpRe.validateData[0];
        datatransfer[10] = tcpRe.validateData[1];
        datatransfer[11] = tcpRe.validateData[2];
        datatransfer[12] = tcpRe.validateData[3];
        datatransfer[13] = tcpRe.validateData[4];
        datatransfer[14] = tcpRe.validateData[5];
        datatransfer[15] = tcpRe.validateData[6];
        datatransfer[16] = countIdReceive;
        tcpRe.Write(datatransfer, 17);

        if (tcpRe.Wait3Second(1)) {
            if(tcpRe.ReadByte()!=2){
                myTCP.isRunning=false;
                return false;
            }
            if (tcpRe.Wait3Second(1)) {
                lock (syncLock) {
                    myTCP.Clean();
                    myTCP = tcpRe;
                    int countBuffer = 0;
                    byte begin = (byte)tcpRe.ReadByte();
                    if (begin < countIdSend) {
                        for (short i = begin; i < countIdSend; i++) {
                            countBuffer += tcpRe.Send(bufferSending[i]);
                            if (countBuffer > 4096) {
                                countBuffer = 0;
                                Thread.Sleep(500);
                            }
                        }
                    } else if (begin > countIdSend) {
                        for (short i = begin; i < bufferSending.Length; i++) {
                            countBuffer += tcpRe.Send(bufferSending[i]);
                            if (countBuffer > 4096) {
                                countBuffer = 0;
                                Thread.Sleep(500);
                            }
                        }
                        for (short i = 0; i < countIdSend; i++) {
                            countBuffer += tcpRe.Send(bufferSending[i]);
                            if (countBuffer > 4096) {
                                countBuffer = 0;
                                Thread.Sleep(500);
                            }
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }

    // public void ClearSocketForNewSocket(){
    //     #if TEST
    //     Debug.LogError("Gọi hàm đóng kết nối RealtimeCore");
    //     #endif
    //     myTCP.Clean();
    // }
    // private DateTime dateTime1970;
    //public long currentTimeMillis { get { return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; } }
    //public long currentTimeMillis { get { return DateTimeOffset.Now.ToUnixTimeMilliseconds(); } }
    // public long currentTimeMillis { get { return System.DateTime.Now.ToUniversalTime(); } }
}
