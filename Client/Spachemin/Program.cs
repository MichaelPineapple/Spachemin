using SpacheNet;

namespace Spachemin;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("*** Spachemin ***");
        if (args.Length > 0 && args[0] == "local")
        {
            Spachemin spachemin = new Spachemin(null);
            spachemin.Run();
        }
        else
        {
            SpacheNetClient net = new SpacheNetClient();
            net.Connect(Console.ReadLine());
            Console.WriteLine("Connected");
            Spachemin spachemin = new Spachemin(net);
            spachemin.Run();
            net.Disconnect();  
        }
    }
}