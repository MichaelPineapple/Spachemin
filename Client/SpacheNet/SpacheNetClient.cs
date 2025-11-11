using System.Net.Sockets;

namespace SpacheNet;

public class SpacheNetClient
{
    private const string IP_LOCALHOST = "127.0.0.1";
    private const int PORT_DEFAULT = 9001;
    private const byte LOGIN_KEY = 69;
    public const int MAX_DATA = 10;
    
    private Stream? Stream;
    private TcpClient? Client;
    
    private int Frame;
    public int PlayerCount { get; private set; }
    public int PlayerId { get; private set; }
    
    public void Connect(string? ip, int port = PORT_DEFAULT)
    {
        if (ip != null)
        {
            if (ip.Length == 0) ip = IP_LOCALHOST;
            Client = new TcpClient();
            Client.Connect(ip, port);
            Stream = Client.GetStream();
        }
        Login();
    }
    
    private void Login()
    {
        if (Client != null)
        {
            byte[] loginBuffer = new byte[4];
            _ = Stream?.Read(loginBuffer, 0, loginBuffer.Length);
            PlayerCount = loginBuffer[0];
            PlayerId = loginBuffer[1];
            Frame = loginBuffer[2];
            Client.NoDelay = (loginBuffer[3] != 0);
            Stream?.Write(new[] { LOGIN_KEY }, 0, 1);
        }
        else PlayerCount = 1;
    }
    
    public byte[][] Step(byte[] data)
    {
        if (data.Length > MAX_DATA) throw new Exception("Step data exceeds MAX_DATA limit (" + MAX_DATA + ")");
        
        byte[] frameIndexData = BytesFromInt(Frame);
        data = PrependData(data, new byte[MAX_DATA]);
        byte[] augmented = PrependData(frameIndexData, data);
        Stream?.Write(augmented, 0, MAX_DATA + frameIndexData.Length);
        
        int bufferLen = PlayerCount * MAX_DATA;
        byte[] buffer = new byte[bufferLen];
        _ = Stream?.Read(buffer, 0, bufferLen);
        if (Stream == null) buffer = data;
        
        byte[][] output = new byte[PlayerCount][];
        int j = -1;
        for (int i = 0; i < bufferLen; i++)
        {
            int k = i % MAX_DATA;
            if (k == 0)
            {
                j++;
                output[j] = new byte[MAX_DATA];
            }
            output[j][k] = buffer[i];
        }
        
        Frame++;
        return output;
    }
    
    public void Disconnect()
    {
        Client?.Close();
        Client = null;
        Stream = null;
    }
    
    private static byte[] BytesFromInt(int val)
    {
        byte[] output = new byte[4];
        output[0] = (byte)(val >> 24);
        output[1] = (byte)(val >> 16);
        output[2] = (byte)(val >> 8);
        output[3] = (byte)(val);
        return output;
    }
    
    private static byte[] PrependData(byte[] prefix, byte[] array)
    {
        byte[] output = new byte[prefix.Length + array.Length];
        for (int i = 0; i < prefix.Length; i++) output[i] = prefix[i];
        int j = prefix.Length;
        for (int i = 0; i < array.Length; i++)
        {
            output[j] = array[i];
            j++;
        }
        return output;
    }
}