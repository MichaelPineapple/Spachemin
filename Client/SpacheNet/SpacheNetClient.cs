using System.Net.Sockets;

namespace SpacheNet;

public class SpacheNetClient
{
    private const string IP_LOCALHOST = "127.0.0.1";
    private const int PORT_DEFAULT = 9001;
    private const byte LOGIN_KEY = 69;
    public const int MAX_DATA = 3;
    
    private Stream? stream;
    private TcpClient? client;
    
    private int frame;
    public int PlayerCount { get; private set; }
    public int PlayerId { get; private set; }
    
    public void Connect(string? ip, int port = PORT_DEFAULT)
    {
        if (stream != null) throw new Exception("Client already connected!");
        if (ip == null) ip = "";
        if (ip.Length == 0) ip = IP_LOCALHOST;
        client = new TcpClient();
        client.Connect(ip, port);
        stream = client.GetStream();
        Login();
    }
    
    private void Login()
    {
        byte[] loginBuffer = new byte[4];
        _ = stream?.Read(loginBuffer, 0, loginBuffer.Length);
        PlayerCount = loginBuffer[0];
        PlayerId = loginBuffer[1];
        frame = loginBuffer[2];
        client.NoDelay = (loginBuffer[3] != 0);
        stream?.Write(new []{ LOGIN_KEY }, 0, 1);
    }
    
    public byte[][] Step(byte[] data)
    {
        if (data.Length > MAX_DATA) throw new Exception("Step data exceeds MAX_DATA limit (" + MAX_DATA + ")");
        byte[] frameIndexData = BytesFromInt(frame);
        data = PrependData(data, new byte[MAX_DATA]);
        data = PrependData(frameIndexData, data);
        stream?.Write(data, 0, MAX_DATA + frameIndexData.Length);
        byte[][] output = new byte[PlayerCount][];
        for (int i = 0; i < PlayerCount; i++)
        {
            output[i] = new byte[MAX_DATA];
            _ = stream?.Read(output[i], 0, MAX_DATA);
        }
        frame++;
        return output;
    }
    
    public void Disconnect()
    {
        client?.Close();
        stream = null;
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