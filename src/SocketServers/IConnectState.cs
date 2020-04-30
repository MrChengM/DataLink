using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServers
{
    public interface IConnectState : IDisposable
    {
        BufferMangment BufferPool { get; }
        ILog Log
        {
            get;
            set;
        }
        TimeOut TimeOut
        {
            get;
            set;
        }
        int ID
        {
            get;
        }
        void Init();

        void Disconnect();

        void ReceiveAsync(int count);

        void SendAsync(byte[] buff);

    }
}
