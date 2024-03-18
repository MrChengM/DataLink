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
        private ILog _log;
        private Dictionary<string, Type> _driverTypes;

        public  ChannelTaskFactory(IPointMapping pointMapping, Dictionary<string, Type> driverTypes, ILog log)
        {
            _pointMapping = pointMapping;
            _driverTypes = driverTypes;
            _log = log;
        }
        public AbstractTask CreatChannelTask(ChannelConfig channelConfig)
        {
            _driverTypes.TryGetValue(channelConfig.DriverInformation.FullName, out Type driverType);

            CommunicationType commtionType = channelConfig.DriverInformation.CommType;
            switch (commtionType)
            {
                case CommunicationType.Serialport:
                    return new ComChannelTask(channelConfig, _pointMapping, driverType) { Log = _log };
                case CommunicationType.Ethernet:
                    return new EthernetChannelTask(channelConfig, _pointMapping, driverType) { Log = _log };
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
