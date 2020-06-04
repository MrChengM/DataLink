using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHandler
{
    [Flags]
    public enum TaskType
    {
        Client,
        Server
    }
}
