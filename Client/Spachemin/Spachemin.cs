using System.Net.Sockets;
using System.Text;

namespace Spachemin;

public class Spachemin
{
    public static void Main()
    {
        Console.WriteLine("Hello, Spachemin!");
        string? ip = Console.ReadLine(); // 44.245.211.131
        if (ip == null) ip = "";
        if (ip.Length == 0) ip = "127.0.0.1";
        Console.WriteLine("Connecting to `" + ip + "`...");
        
        TcpClient client = new TcpClient();
        client.Connect(ip, 9001);
        Stream stream = client.GetStream();
        
        byte[] loginData = new byte[2];
        _ = stream.Read(loginData, 0, 2);
        int playerCount = loginData[0];
        int id = loginData[1];
        
        OnConnect(stream, playerCount, id);
        
        client.Close();
    }
    
    private static void OnConnect(Stream stream, int playerCount, int id)
    {
        Console.WriteLine("Connected");
        while (true)
        {
            string? input = Console.ReadLine();
            if (input == null) input = "";
            if (input.Length == 0) break;
            Console.WriteLine("Sending...");
            SendTransmission(stream, input);
            for (int i = 0; i < playerCount; i++)
            {
                string str = ReceiveTransmission(stream);
                Console.WriteLine("Player " + i + "> " + str);
            }
        }
    }

    private static void SendTransmission(Stream stream, string data)
    {
        ASCIIEncoding encoder = new ASCIIEncoding();
        byte[] transData = encoder.GetBytes(data);
        stream.Write(transData, 0, transData.Length);
    }

    private static string ReceiveTransmission(Stream stream)
    {
        const int BUFFER_SIZE = 255;
        byte[] buffer = new byte[BUFFER_SIZE];
        int len = stream.Read(buffer, 0, BUFFER_SIZE);
        string str = "";
        for (int i = 0; i < len; i++) str += Convert.ToChar(buffer[i]);
        return str.Replace("\0", "").Trim();
    }
}