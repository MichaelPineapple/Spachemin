using SpacheNet;

namespace Spachemin;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, Spachemin!");
        SpacheNetClient net = new SpacheNetClient();
        net.Connect(Console.ReadLine());
        Console.WriteLine("Connected");
        Spachemin spachemin = new Spachemin(net);
        spachemin.Run();
        net.Disconnect();        
    }
}