using ConfigTool.Models;
using ConfigTool.Helper;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Service
{
    public class SingleLoggerServer : ISingleLoggerServer
    {
        ObservableCollection<LogListItem> logLists;
        public ObservableCollection<LogListItem> LogLists
        {
            get => logLists;
        }

        private ILog log;

        private Log4ListAppender lappender;
        public ILog Log
        {
            get {
                return log;
            }
        }

        public LogFilter LogFilter
        {
            get { return lappender.Filter; }
        }

        public SingleLoggerServer()
        {
            init();
        }

        private void init()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            log = LogManager.GetLogger("ConfigToolLogger");
            var logger = hierarchy.GetLogger("ConfigToolLogger", hierarchy.LoggerFactory);
            lappender = new Log4ListAppender() { Name = "ListAppender"};
            logLists = lappender.LogList;
            logger.AddAppender(lappender);
        }
    }
}
