using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using ModbusDrivers.Client;
using DL645Driver;
using SiemensDriver;
using TaskHandler.Config;
using System.IO.Ports;

namespace TaskHandler.Factory
{
    public abstract class ClientFactory
    {

        protected TimeOut _timeOut;
        protected ILog _log;
        public ClientFactory()
        {
        }
        public abstract IPLCDriver CreateClient(ClientName name);

    }

    public  class TCPClientFactory:ClientFactory
    {
        private TCPClientConfig _config;
        private EthernetSetUp _setUp;
        public TCPClientFactory(TCPClientConfig config, TimeOut timeOut, ILog log) : base()
        {
            _config = config;
            _setUp = new EthernetSetUp(config.IpAddress, config.Port);
            _timeOut = timeOut;
            _log = log;
        }

        public override IPLCDriver CreateClient(ClientName name)
        {
            switch (name)
            {
                case ClientName.ModbusTCPClient:
                    return new ModbusTCPClient(_setUp, _timeOut, _log);
                case ClientName.S7CommClient:
                    var config = _config as S7CommClientConfig;
                    int slotNo = config.SlotNo;
                    return new S7CommClient(_setUp, _timeOut, _log, slotNo);
                default:
                    return null;
            }
        }
    }
    public class ComClientFactory : ClientFactory
    {
        private ComClientConfig _config;
        private SerialportSetUp _setUp;
        public ComClientFactory(ComClientConfig config, TimeOut timeOut,ILog log)
        {
            _config = config;
            _setUp = new SerialportSetUp(_config.ComPort, _config.BuadRate, (StopBits)_config.StopBit);
            _timeOut = timeOut;
            _log = log;
        }
        public override IPLCDriver CreateClient(ClientName name)
        {
            switch (name)
            {
                case ClientName.ModbusRTUClient:
                    return new ModbusRTUMaster(_setUp, _timeOut, _log);
                case ClientName.DL645_1997Client:
                    return new DL645_1997Driver(_setUp, _timeOut, _log);
                case ClientName.DL645_2007Client:
                    return new DL645_2007Driver(_setUp, _timeOut, _log);
                default:
                    return null;
            }
        }
    }
}
