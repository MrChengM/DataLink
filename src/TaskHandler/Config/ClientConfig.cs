using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TaskHandler.Config
{
    public abstract class ClientConfig
    {
        protected int _pollingTime;
        protected int _timeout;
        protected string _signalListFilePath;
        public ClientConfig()
        {
            _pollingTime = 1000;
            _timeout = 1000;
            _signalListFilePath = "";//点表文件：默认为空 必须配置 
        }
        public int TimeOut { get { return _timeout; } set { _timeout = value; } }
        public int PollingTime { get { return _pollingTime; } set { _pollingTime = value; } }
        public string SignalListFilePath { get { return _signalListFilePath; } set { _signalListFilePath = value; } }

    }

    public class TCPClientConfig : ClientConfig
    { /***************************
       Ip地址、端口和点文件路径必须配置
       其他参数可以设置为默认值
       ****************************/
        string _ipString;
        int _port;
        public TCPClientConfig() : base()
        {
            _ipString = "";//IP地址及端口号，必须指定
            _port = 0;
        }
        public string IpAddress { get { return _ipString; } set { _ipString = value; } }
        public int Port { get { return _port; } set { _port = value; } }
    }
    public class S7CommClientConfig : TCPClientConfig
    {
        int _slotNo;
        public S7CommClientConfig() : base()
        {
            _slotNo = 2;
        }
        public int SlotNo
        {
            get
            {
                return _slotNo;
            }
            set
            {
                _slotNo = value;
            }
        }
    }
    public class ComClientConfig : ClientConfig
    {
        /***************************
     com端口和点文件路径必须配置
     其他参数可以设置为默认值
     ****************************/
        string _comport; //主键 默认为空 必须配置
        int _buadRate;
        byte _dataBit;
        StopBits _stopBit;
        Parity _OddEvenCheck;

        public ComClientConfig() : base()
        {
            _comport = "";
            _buadRate = 9600;
            _dataBit = 8;
            _stopBit = StopBits.One;
            _OddEvenCheck = Parity.None;
        }
        public string ComPort { get { return _comport; } set { _comport = value; } }
        public int BuadRate { get { return _buadRate; } set { _buadRate = value; } }
        public byte DataBit { get { return _dataBit; } set { _dataBit = value; } }
        public StopBits StopBit { get { return _stopBit; } set { _stopBit = value; } }
        public Parity OddEvenCheck { get { return _OddEvenCheck; } set { _OddEvenCheck = value; } }

      
    }
}
