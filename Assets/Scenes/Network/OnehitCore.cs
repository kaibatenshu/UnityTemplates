using System;
using System.Threading;
using System.Net.Sockets;

public class OnehitCore{
    public const short ERRORCODE_NO_MESSAGE_SENDING = 1;
    public const short ERRORCODE_CREATE_SOCKET = 2;
    public const short ERRORCODE_LENGTH_SERVER_ERROR = 3;

    public const short SOCKET_TIME_OUT_1 = 100;
    public const short SOCKET_TIME_OUT_2 = 101;
    public const short SOCKET_TIME_OUT_3 = 102;
    private bool isRunning;
    protected MessageSending messageSending;
	public int addWaitTimemili;
    
    public string ConnectIp(String _ip,int _port,long TIME_OUT){
        string logError = null;
        if (_ip.Contains(":") || _ip.Contains("v6"))
            tcpSocket = new TcpClient(AddressFamily.InterNetworkV6);
        else
            tcpSocket = new TcpClient(AddressFamily.InterNetwork);
        try{
            tcpSocket.BeginConnect(_ip, _port, null, null);
        }catch(SocketException scE){
            logError = scE.Message.ToString();
        }
        
        for(int i=0;i<TIME_OUT/5;i++)
            if (tcpSocket.Connected)
                return null;
            else
                Thread.Sleep(5);
        tcpSocket.Close();
        return logError;
    }
    

    private TcpClient tcpSocket;
    private NetworkStream networkStream;
    protected void ProcessTCP() {
        isRunning=true;
        networkStream = tcpSocket.GetStream();
        messageReceiving = null;
        try {
            ProcessNetwork();
        }finally {
            Thread.Sleep(3000);
            networkStream.Close();
            tcpSocket.Close();
        }
    }


    public bool isOnlySend;
    public MessageReceiving messageReceiving;
    public Action onSuccess;
    public Action<String> onError;
    private void ProcessNetwork() {
        long timeBeginProcess = DateTimeUtil.currentTimeMillis;
        byte[] handshake = new byte[8];
        if (Wait(8))
            networkStream.Read(handshake);
        else{
            onError("SOCKET_TIME_OUT(ValidateCode)");
            return;
        }
        byte validateCode = handshake[3];
        /**************************************************************/
        byte[] dataMessage = messageSending.getBytesArray();
        int length = dataMessage.Length;
        byte[] datatransfer = new byte[length+11];

        datatransfer[0] = (byte)(handshake[0] ^ validateCode);
        datatransfer[1] = (byte)(handshake[1] ^ validateCode);
        datatransfer[2] = (byte)(handshake[2] ^ validateCode);
        datatransfer[3] = (byte)(handshake[4] ^ validateCode);
        datatransfer[4] = (byte)(handshake[5] ^ validateCode);
        datatransfer[5] = (byte)(handshake[6] ^ validateCode);
        datatransfer[6] = (byte)(handshake[7] ^ validateCode);

        datatransfer[7] = (byte)((int)((uint)length >> 24) & 0xFF);
        datatransfer[8] = (byte)((int)((uint)length >> 16) & 0xFF);
        datatransfer[9] = (byte)((int)((uint)length >> 8) & 0xFF);
        datatransfer[10]= (byte)((int)((uint)length >> 0) & 0xFF);

        for (short i = 0; i < length; i++)
            datatransfer[i + 11] = (byte)(dataMessage[i] ^ validateCode);
        Console.WriteLine("Chưa send gói tin lớn");
        networkStream.Write(datatransfer, 0, dataMessage.Length + 11);
        /**************************************************************/

        if(isOnlySend){/*Only Send*/
            onSuccess();
            return;
        }
        
        /*cho server 3 giây để xử lý*/
        int countWaitPing = (3000 + addWaitTimemili)/5;
        for(int i=0;i<countWaitPing;i++)
            if(isRunning==false){
                onError("SOCKET_CLOSE_BY_USER");
                return;
            }else if (tcpSocket.Available < 4)
                Thread.Sleep(5);
            else
                break;

        if(tcpSocket.Available < 4){
            onError("ERROR(Sai giao thuc)");
            return;
        }

        networkStream.Read(datatransfer, 0, 4);

        int ch1 = datatransfer[0] & 0xFF;
        int ch2 = datatransfer[1] & 0xFF;
        int ch3 = datatransfer[2] & 0xFF;
        int ch4 = datatransfer[3] & 0xFF;
        length = ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
        if (length == 0){
            onSuccess();
            return;
        }else if(length<0 || length>32768){
            onError("LengthError");
            return;
        }

        datatransfer = new byte[length + 2];
        if (length > -1) {
            if (Wait(length)){
                Console.WriteLine("Chưa receive gói tin lớn");
                networkStream.Read(datatransfer, 2, length);
                /**************************************************************/
                for (short i = 0; i < length; i++)
                    datatransfer[i + 2] = (byte)(datatransfer[i + 2] ^ validateCode);

                messageReceiving=new MessageReceiving(datatransfer);
                messageReceiving.cmd = messageSending.getCMD();
                messageReceiving.timeProcess = DateTimeUtil.currentTimeMillis-timeBeginProcess;
                // #if TEST
                // Debug.LogWarning("Onehit " + CMD_ONEHIT.getCMDName(messageSending) + " : " + messageSending.getBytesArray().Length+" byte ➜ "+length+" byte");
                // #endif
                onSuccess();
            }else
                onError("SOCKET_RECEIVE_TIMEOUT");
        }else
            onError("SOCKET_LENGTH_ERROR("+length+")");
    }
    public void CloseConnect(){
        isRunning=false;
    }
    private bool Wait(int _length) {
        for (int i = 0; i < 800; i++)
            if (isRunning == false)
                return false;
            else if (tcpSocket.Available < _length) {
                Thread.Sleep(5);
            } else
                return true;
        return false;
    }

}
