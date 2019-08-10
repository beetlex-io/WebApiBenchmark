using BeetleX.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeetleX.WebApiTester.Codes
{
    public class HttpTcpClient : IDisposable
    {
        private static long mId = 0;

        public static long GetID()
        {
            return System.Threading.Interlocked.Increment(ref mId);
        }

        public void Dispose()
        {
            Client.DisConnect();
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public HttpTcpClient(string host, int port)
        {
            Host = host;
            Port = port;
            Buffer = new byte[1024 * 4];
            Client = BeetleX.SocketFactory.CreateClient<AsyncTcpClient>(host, port);
            Client.Connected = (c) => { c.Socket.NoDelay = true; };
            ID = GetID();
        }

        public long ID { get; set; }

        public AsyncTcpClient Client { get; private set; }

        public byte[] Buffer { get; private set; }
    }
}
