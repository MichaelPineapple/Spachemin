using SpacheNet;

namespace Spachemin;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("*** Spachemin ***");
        string? ip = null;
        if (!(args.Length > 0 && args[0] == "local")) ip = Console.ReadLine();
        if (ip?.Trim().ToLower() == "l") ip = null;
        SpacheNetClient net = new SpacheNetClient();
        net.Connect(ip);
        Console.WriteLine("Connected");
        Spachemin spachemin = new Spachemin(net);
        spachemin.Run();
        net.Disconnect();  
    }
}