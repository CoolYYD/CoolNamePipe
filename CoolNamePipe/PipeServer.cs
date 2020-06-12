using log4net;
using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoolNamePipe
{
    public class PipeServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PipeServer));
        private NamedPipeServerStream pipeServer;
        private string pipeName;
        public ConcurrentQueue<Message> sendMsgQueue = new ConcurrentQueue<Message>();
        private const int PipeBufferSize = 65535;

        private bool isConnected;
        private Message sendMsg;

        public PipeServer(string pipeName)
        {
            this.pipeName = pipeName;
            Task.Factory.StartNew(CreateServer);
        }

        private void CreateServer()
        {
            try
            {
                using (pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                {
                    Logger.Info("Wait Client connect PipeName:" + pipeName);
                    pipeServer.WaitForConnection();
                    Logger.Info("Client is connected PipeName:" + pipeName);
                    this.isConnected = true;

                    while (true)
                    {
                        Thread.Sleep(1);
                        bool isSuccess = sendMsgQueue.TryDequeue(out sendMsg);
                        if (isSuccess)
                        {
                            byte[] arrByte = Encoding.UTF8.GetBytes(sendMsg.sendContent);
                            pipeServer.Write(arrByte, 0, arrByte.Length);
                            byte[] data = new byte[PipeBufferSize];
                            int read = pipeServer.Read(data, 0, data.Length);
                            sendMsg.resContent = Encoding.UTF8.GetString(data, 0, read);
                            pipeServer.FlushAsync();
                            sendMsg.areReceive.Set();
                            if (string.IsNullOrEmpty(sendMsg.resContent) && !pipeServer.IsConnected)
                            {
                                this.isConnected = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CreateServer err:" + ex);
                this.isConnected = false;
                sendMsg.areReceive.Set();
            }
            Thread.Sleep(1);
            CreateServer();
        }


        public string SendMessage(string msg)
        {
            if (this.isConnected)
            {
                Message sendMsg = new Message(msg);
                sendMsgQueue.Enqueue(sendMsg);
                sendMsg.areReceive.WaitOne();
                return sendMsg.resContent;
            }
            else
            {
                Logger.Warn("PipeServer is disconnected:PipeName:" + pipeName);
                return null;
            }
        }
    }

    public class Message
    {
        public Message(string msg)
        {
            areReceive = new AutoResetEvent(false);
            sendContent = msg;
        }

        public AutoResetEvent areReceive { get; set; }
        public string sendContent { get; set; }
        public string resContent { get; set; }
    }
}
