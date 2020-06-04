using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHandler.Builder;
using TaskHandler.Config;

namespace TaskHandler.Factory
{
    public class TaskFactory
    {
        public TaskFactory()
        {

        }
        public AbstractTask CreateTask(TaskConfig config,ILog log)
        {
            ServerName servername = 0;
            ClientName clientname;
            AbstractTask task = null;
            switch (config.TsType)
            {

                case TaskType.Server:
                    if (Enum.TryParse(config.Id, out servername))
                    {
                        switch (config.DrType)
                        {
                            case DriverType.Ethernet:
                                var builder = new TCPServerTaskBuilder(servername,log);
                                task = builder.GetResult();
                                break;
                            case DriverType.Serialport:
                                var builder1 = new ComServerTaskBuilder(servername,log);
                                task = builder1.GetResult();
                                break;
                        }
                    }
                    break;
                case TaskType.Client:
                    if (Enum.TryParse(config.Id, out clientname))
                    {
                        switch (config.DrType)
                        {
                            case DriverType.Ethernet:
                                var builder2 = new TCPClientTaskBuilder(clientname,log);
                                task = builder2.GetResult();
                                break;
                            case DriverType.Serialport:
                                var builder3 = new ComClientTaskBuilder(clientname,log);
                                task = builder3.GetResult();
                                break;
                        }
                    }
                    break;
            }
            return task;
        }
    }
}
