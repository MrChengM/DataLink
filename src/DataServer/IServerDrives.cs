﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;
using DataServer.Points;
using DataServer.Log;

namespace DataServer
{
    public interface IServerDrivers : IDisposable
    {
        string ServerName { get; set; }
        ILog Log { get; set; }
        TimeOut TimeOut { get; set; }
        int MaxConnect { get; set; }
        IPointMapping PointMapping{ get; set; }
        void RegisterMapping(Dictionary<string,TagBindingConfig> tagBindings);
        bool Init();
        bool Start();
        bool Stop();
        bool IsRunning { get;}

    }
    public enum ServerOption
    {
        ModbusTCP,
        ModbusRTU,
        Freedom

    }
}
