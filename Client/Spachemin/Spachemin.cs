using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace Spachemin;

public class Spachemin
{
    public static void Main()
    {
        Console.WriteLine("Hello, Spachemin!");
        string? ip = Console.ReadLine(); // 44.245.211.131
        if (ip == null) ip = "";
        if (ip.Length == 0) ip = "127.0.0.1";
        TcpClient client = new TcpClient();
        Console.WriteLine("Connecting to `" + ip + "`...");
        client.Connect(ip, 9001);
        Stream stream = client.GetStream();
        OnConnect(stream);
        client.Close();
    }
    
    static void OnConnect(Stream _stream)
    {
        Console.WriteLine("Connected");
        while (true)
        {
            string? input = Console.ReadLine();
            if (input == null) input = "";
            if (input.Length == 0) break;
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] transData = encoder.GetBytes(input);
            Console.WriteLine("Sending...");
            Stopwatch watch = Stopwatch.StartNew();
            _stream.Write(transData, 0, transData.Length);
            byte[] ackData = new byte[255];
            int ackLen = _stream.Read(ackData, 0, 255);
            watch.Stop();
            long latency = watch.ElapsedMilliseconds;
            string ackStr = "";
            for (int i = 0; i < ackLen; i++) ackStr += Convert.ToChar(ackData[i]);
            ackStr = ackStr.Replace("\0", "").Trim();
            Console.WriteLine("ACK: " + ackStr + " (" + latency + ")");
        }
    }
}