using DataServer.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHandler.Config;

namespace TaskHandler.Factory
{
    public class ConfigFactory
    {
        private string _taskName;
        private string _configPath;

        public ConfigFactory(string taskName,string configPath)
        {
            _taskName = taskName;
            _configPath = configPath;
        }
        /// <summary>
        /// 创建单个客户端配置类 ，只取第一个有效位
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        //public ClientConfig CreatClientConfig(ClientName clientName)
        //{
        //    ClientConfig result=null;
        //    if(clientName== ClientName.ModbusTCPClient)
        //    {
        //        var configs= ReaderXMLUtil.ReadXMLConfig<TCPClientConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
        //        if (configs != null)
        //        {
        //            result = configs[0];
        //        }
        //    }
        //    else if (clientName == ClientName.S7CommClient)
        //    {
        //        var configs = ReaderXMLUtil.ReadXMLConfig<S7CommClientConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
        //        if (configs != null)
        //        {
        //            result = configs[0];
        //        }
        //    }
        //    else if (clientName == ClientName.ModbusRTUClient||clientName==ClientName.DL645_1997Client||clientName==ClientName.DL645_2007Client)
        //    {
        //        var configs = ReaderXMLUtil.ReadXMLConfig<ComClientConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
        //        if (configs != null)
        //        {
        //            result = configs[0];
        //        }
        //    }
            
        //    return result;

        //}
        /// <summary>
        /// 创建客户端配置类列表，根据客户端名称获取所有配置表内项目
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public List<ClientConfig> CreatClientConfigs(ClientName clientName)
        {
            List<ClientConfig> result = null;
            if (clientName == ClientName.ModbusTCPClient)
            {
                var configs = ReaderXMLUtil.ReadXMLConfig<TCPClientConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
                if (configs != null)
                {
                    result = new List<ClientConfig>();
                    foreach(var config in configs)
                    {
                        result.Add(config);
                    }
                }
            }
            else if (clientName == ClientName.S7CommClient)
            {
                var configs = ReaderXMLUtil.ReadXMLConfig<S7CommClientConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
                if (configs != null)
                {
                    result = new List<ClientConfig>();
                    foreach (var config in configs)
                    {
                        result.Add(config);
                    }
                }
            }
            else if (clientName == ClientName.ModbusRTUClient || clientName == ClientName.DL645_1997Client || clientName == ClientName.DL645_2007Client)
            {
                var configs = ReaderXMLUtil.ReadXMLConfig<ComClientConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
                if (configs != null)
                {
                    result = new List<ClientConfig>();
                    foreach (var config in configs)
                    {
                        result.Add(config);
                    }
                }
            }
            return result;
        }
        public List<ServerConfig> CreatServerConfigs(ServerName serverName)
        {
            List<ServerConfig> result = null;
            if (serverName == ServerName.ModbusTCPServer)
            {
                var configs = ReaderXMLUtil.ReadXMLConfig<TCPServerConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
                if (configs != null)
                {
                    result = new List<ServerConfig>();
                    foreach (var config in configs)
                    {
                        result.Add(config);
                    }
                }
            }
            else if (serverName == ServerName.ModbusRTUServer)
            {
                var configs = ReaderXMLUtil.ReadXMLConfig<ComServerConfig>(_configPath, ConfigUtilly.ReadConfig, "setup", _taskName);
                if (configs != null)
                {
                    result = new List<ServerConfig>();
                    foreach (var config in configs)
                    {
                        result.Add(config);
                    }
                }
            }
            return result;
        }
    }
}
