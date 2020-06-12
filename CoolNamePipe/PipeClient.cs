using log4net;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoolNamePipe
{
    public class PipeClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PipeClient));

        private bool isConnected;
        NamedPipeClientStream pipeClient;
        private int timeOut = 3000;
        private string pipeName;

        public delegate void MessageReceivedHandler(string message);
        public event MessageReceivedHandler MessageReceived;

        private const int PipeBufferSize = 65535;

        public PipeClient(string pipeName)
        {
            this.pipeName = pipeName;
            Task.Factory.StartNew(CreateClient);
        }

        private void CreateClient()
        {
            using (pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
            {
                try
                {
                    Logger.Info("Client connecting PipeName:" + pipeName);
                    pipeClient.Connect(timeOut);
                    Logger.Info("Client is connected PipeName:" + pipeName);
                    this.isConnected = true;
                    while (true)
                    {
                        Thread.Sleep(1);
                        byte[] data = new byte[PipeBufferSize];
                        int read = pipeClient.Read(data, 0, data.Length);
                        string resMsg = Encoding.UTF8.GetString(data, 0, read);
                        pipeClient.FlushAsync();
                        Logger.Info("Get Message:" + resMsg);
                        if (string.IsNullOrEmpty(resMsg))
                        {
                            break;
                        }
                        this.MessageReceived?.Invoke(resMsg);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("CreateClient err:" + ex.Message);
                    this.isConnected = false;
                }
            }
            Logger.Info("Client is disconnected:" + pipeName);
            Thread.Sleep(1);
            CreateClient();
        }

        public void SendMessage(string msg)
        {
            if (isConnected)
            {
                byte[] sendMsg = Encoding.UTF8.GetBytes(msg);
                pipeClient.Write(sendMsg, 0, sendMsg.Length);
            }
            else
            {
                Logger.Error("SendMessage err, Server is disconnected:" + pipeName);
            }
        }
    }
}


