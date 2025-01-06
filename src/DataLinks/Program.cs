using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgr;

namespace DriverLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TaskMagr.Main(args, "DataLinksLogger");
            Console.ReadLine();
        }
    }
}
