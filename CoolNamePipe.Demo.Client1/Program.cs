using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoolNamePipe.Demo.Client1
{
    class Program
    {
        private static PipeClient pipeClient = new PipeClient("Client1");

        static void Main(string[] args)
        {
            pipeClient.MessageReceived += PipeClient_MessageReceived;

            while (true)
            {
                Thread.Sleep(100000000);
            }
        }

        private static void PipeClient_MessageReceived(string message)
        {
            Console.WriteLine($"Client1 Get Message:{message}");
            pipeClient.SendMessage("Client1:" + message);
        }
    }
}
