using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMgr.Task;
using DataServer.Points;
using DataServer.Config;
using DataServer;
   

namespace TaskMgr.Factory
{
    public class ChannelTaskFactory
    {
        private IPointMapping _pointMapping;
        private ChannelConfig _channelConfig;
        private ILog _log;

        public  ChannelTaskFactory(IPointMapping pointMapping,ChannelConfig channelConfig,ILog log)
        {
            _pointMapping = pointMapping;
            _channelConfig = channelConfig;
            _log = log;
        }
        public AbstractTask CreatChannelTask()
        {
            CommunicationType type = _channelConfig.DriverInformation.CommType;
            switch (type)
            {
                case CommunicationType.Serialport:
                    return new ComChannelTask(_channelConfig, _pointMapping) { Log = _log };
                case CommunicationType.Ethernet:
                    return new EthernetChannelTask(_channelConfig, _pointMapping) { Log = _log };
                case CommunicationType.File:
                    return null;
                case CommunicationType.Memory:
                    return null;
                default:
                    return null;
            }
        }
    }
}
