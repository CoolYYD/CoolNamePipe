using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoolNamePipe.Demo.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            PipeServer server0 = new PipeServer("Client0");
            PipeServer server1 = new PipeServer("Client1");

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    string tmpIn = "【Client0】" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString();
                    string tmpRes = server0.SendMessage(tmpIn);
                    if (tmpRes != null)
                    {
                        Console.WriteLine($"Server Get Message:{tmpRes}");
                    }
                    else
                    {
                        Console.Error.WriteLine($"Client0 is disconnected");
                    }

                    Thread.Sleep(2000);
                }
            });


            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    string tmpIn = "【Client1】" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString();
                    string tmpRes = server1.SendMessage(tmpIn);
                    if (tmpRes != null)
                    {
                        Console.WriteLine($"Server Get Message:{tmpRes}");
                    }
                    else
                    {
                        Console.Error.WriteLine($"Client1 is disconnected");
                    }

                    Thread.Sleep(2000);
                }
            });

            while (true)
            {
                Thread.Sleep(10000000);
            }
        }
    }
}
