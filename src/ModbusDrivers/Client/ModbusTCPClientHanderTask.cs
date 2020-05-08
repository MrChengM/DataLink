using DataServer;
using ds = DataServer;
using DataServer.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Utillity;
using System.Xml;
using System.Threading;
using Timer=System.Timers;

namespace ModbusDrivers.Client
{
    public class ModbusTCPClientHanderTask
    {
        ModbusTCPClient client;
        ModbusTCPClientConfig config;
        PointCollcet points;
        XMLWorkbook workbook;
        EthernetSetUp setup;
        TimeOut timeout;
        ILog log;

        public ILog Log
        {
            get { return log; }
            set { log = value; }
        }
        
        public ModbusTCPClientHanderTask(ILog log)
        {
            this.log = log;
        }
        public bool OnInit()
        {
          

                config = ReaderXMLUtil.ReadXMLConfig<ModbusTCPClientConfig>("../../../../conf/Configuration.xml", ModbusTCPClientConfig.ReadConfig, "setup")[0];
                setup = new EthernetSetUp(config.IpAddress, config.Port);
                timeout = new TimeOut("ModbusTCPClientHanderTask", config.TimeOut, log);
                client = new ModbusTCPClient(setup, timeout, log);
                workbook = XmlSerialiaztion.XmlDeserial<XMLWorkbook>(config.SignalListFilePath, log);
                points = PointCollcet.CreateDevicePoints(workbook);
                PointsRegister.Register(points, log);
                return true;
        }

        public bool OnStart()
        {
            client.Connect();
            Timer.Timer timeRead = new Timer.Timer(config.PollingTime);
            timeRead.Elapsed += TimeRead_Elapsed;
            timeRead.AutoReset = true;
            timeRead.Start();
            return true;
        }

        private void TimeRead_Elapsed(object sender, Timer.ElapsedEventArgs e)
        {
            var t = sender as Timer.Timer;
            t.Stop();
            if (client.IsConnect)
            {
                foreach (var point in points.BoolPoints)
                {
                    var devicePoint = point as DevicePoint<bool>;
                    devicePoint.Value= client.ReadBools(devicePoint.Address, (ushort)devicePoint.Length);
                }
                //foreach (var point in points.BytePoints)
                //{
                //    var devicePoint = point as DevicePoint<byte>;
                //    client.ReadBytes(devicePoint.Address, (ushort)devicePoint.Length);
                //}
                foreach (var point in points.UshortPoints)
                {
                    var devicePoint = point as DevicePoint<ushort>;
                    devicePoint.Value=client.ReadUShorts(devicePoint.Address, (ushort)devicePoint.Length);
                }
                foreach (var point in points.ShortPoints)
                {
                    var devicePoint = point as DevicePoint<short>;
                    devicePoint.Value=client.ReadShorts(devicePoint.Address, (ushort)devicePoint.Length);
                }
                foreach (var point in points.IntPoints)
                {
                    var devicePoint = point as DevicePoint<int>;
                    devicePoint.Value=client.ReadInts(devicePoint.Address, (ushort)devicePoint.Length);

                }
                foreach (var point in points.UintPoints)
                {
                    var devicePoint = point as DevicePoint<uint>;
                    devicePoint.Value=client.ReadUInts(devicePoint.Address, (ushort)devicePoint.Length);
                }
                foreach (var point in points.FloatPoints)
                {
                    var devicePoint = point as DevicePoint<float>;
                    devicePoint.Value=client.Readfloats(devicePoint.Address, (ushort)devicePoint.Length);
                }
                foreach (var point in points.StringPoints)
                {
                    var devicePoint = point as DevicePoint<string>;
                    devicePoint.Value= client.ReadStrings(devicePoint.Address, (ushort)devicePoint.Length);
                }
            }
            else
            {
                client.Connect();
            }
            t.Start();
        }

        public bool OnStop()
        {
            client.DisConnect();
            return default(bool);
        }
  
    }

   
}
