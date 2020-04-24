using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
     public interface IServerDrivers
    {
        void Init();
        bool Start();
        void Stop();
        bool IsRunning { get;}
    }
}
