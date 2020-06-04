using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer;
using ModbusDrivers.Client;
using DL645Driver;
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
        private EthernetSetUp _setUp;
        public TCPClientFactory(EthernetSetUp setUp, TimeOut timeOut, ILog log) : base()
        {
            _setUp = setUp;
            _timeOut = timeOut;
            _log = log;
        }

        public override IPLCDriver CreateClient(ClientName name)
        {
            switch (name)
            {
                case ClientName.ModbusTCPClient:
                    return new ModbusTCPClient(_setUp, _timeOut, _log);
                default:
                    return null;
            }
        }
    }
    public class ComClientFactory : ClientFactory
    {
        private SerialportSetUp _setUp;
        public ComClientFactory(SerialportSetUp setUp,TimeOut timeOut,ILog log)
        {
            _setUp = setUp;
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
