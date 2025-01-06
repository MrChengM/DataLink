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
        string _configFath = "../../../../conf/Configuration.xml";

        public TaskFactory(string configFath)
        {
            _configFath = configFath;
        }
        public List<AbstractTask> CreateTasks(TaskConfig config,ILog log)
        {
            ServerName servername;
            ClientName clientname;
            List<AbstractTask> tasks = new List<AbstractTask>();
            //创建配置工厂
            var configFactory = new ConfigFactory(config.TaskName, _configFath);

            switch (config.TsType)
            {
                case TaskType.Server:
                    if (Enum.TryParse(config.TaskName.Replace("HandlerTask", ""), out servername))
                    {
                        //创建服务类型配置列表
                        List<ServerConfig> serverConfigs = configFactory.CreatServerConfigs(servername);
                        switch (config.DrType)
                        {
                            case CommunicationType.Ethernet:
                                foreach(var serverconfig in serverConfigs)
                                {
                                    var builder = new TCPServerTaskBuilder(servername, log, serverconfig);
                                    var task = builder.GetResult();
                                    tasks.Add(task);
                                }
                                break;
                            case CommunicationType.Serialport:
                                foreach (var serverconfig in serverConfigs)
                                {
                                    var builder1 = new ComServerTaskBuilder(servername, log, serverconfig);
                                    var task = builder1.GetResult();
                                    tasks.Add(task);
                                }
                                break;
                        }
                    }
                    break;
                case TaskType.Client:
                    if (Enum.TryParse(config.TaskName.Replace("HandlerTask", ""), out clientname))
                    {
                        List<ClientConfig> clientConfigs = configFactory.CreatClientConfigs(clientname);
                        switch (config.DrType)
                        {
                            case CommunicationType.Ethernet:
                                foreach(var clientConfig in clientConfigs)
                                {
                                    var builder2 = new TCPClientTaskBuilder(clientname, log,clientConfig);
                                    var task = builder2.GetResult();
                                    tasks.Add(task);
                                }
                                break;
                            case CommunicationType.Serialport:
                                foreach(var clientConfig in clientConfigs)
                                {
                                    var builder3 = new ComClientTaskBuilder(clientname, log,clientConfig);
                                    var task = builder3.GetResult();
                                    tasks.Add(task);
                                }
                                break;
                        }
                    }
                    break;
            }
            return tasks;
        }
    }
}
