using SocketServers.SAEA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServers
{
   public interface ISockteServer:IDisposable
    {
        bool Init();
        bool Start();
        bool Stop();

        event Action<IConnectState> ReadComplete;
        event Action<IConnectState> SendComplete;
    }
}
