using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
using ConfigTool.Models;
using System.Collections.ObjectModel;

namespace ConfigTool.Helper
{
    /// <summary>
    /// Log4net Appender；
    /// Log数据输出到List表
    /// </summary>
    public class Log4ListAppender : AppenderSkeleton
    {
        private ObservableCollection<LogListItem> logList=new ObservableCollection<LogListItem>();
        private const int MAXNUMBER = 1000;

        public LogFilter Filter { get; } = new LogFilter() { Source=LogSource.DataLink|LogSource.ConfigTool,Level=LogLevel.Error|LogLevel.Info};
        public ObservableCollection<LogListItem> LogList
        {
            get {
                return logList;
            }
            set {
                logList = value;
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (logList.Count > MAXNUMBER)
            {
                logList.RemoveAt(0);
            }
            if (!((Filter.Source&LogSource.ConfigTool)>0))
            {
                if (loggingEvent.Domain.Contains(LogSource.ConfigTool.ToString()))
                {
                    return;
                }
            }
            else if (!((Filter.Source & LogSource.DataLink) > 0))
            {
                if (loggingEvent.Domain.Contains(LogSource.DataLink.ToString()))
                {
                    return;
                }
            }
            if ((Filter.Level&LogLevel.Info)==0&&loggingEvent.Level==Level.Info)
            {
                return;
            }
            else if ((Filter.Level & LogLevel.Error) == 0 && loggingEvent.Level >Level.Info)
            {
                return;
            }
            LogListItem item = new LogListItem()
            {
                Date = loggingEvent.TimeStamp.ToShortDateString(),
                Time = loggingEvent.TimeStamp.ToShortTimeString(),
                Level = loggingEvent.Level.Name,
                Source = $"{ loggingEvent.Domain}",
                Message= loggingEvent.RenderedMessage
            };
           
            LogList.Add(item);
        }
    }
}
