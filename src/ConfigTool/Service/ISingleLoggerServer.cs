using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.Helper;
using ConfigTool.Models;
using System.Collections.ObjectModel;
using log4net.Core;
using log4net;

namespace ConfigTool.Service
{
    public interface ISingleLoggerServer
    {
        /// <summary>
        /// log记录数组
        /// </summary>
        ObservableCollection<LogListItem> LogLists { get;}
        ILog Log
        {
            get;
        }

        LogFilter LogFilter { get; }
    }
}
