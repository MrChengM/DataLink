using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using ConfigTool.Models;
using ConfigTool.Service;

namespace ConfigTool.ViewModels
{
    public class LogDisplayViewModel:BindableBase
    {
        private ISingleLoggerServer _singleLoggerServer;

        private ObservableCollection<LogListItem> logLists;

        public ObservableCollection<LogListItem> LogLists
        {
            get {
                return logLists;
            }
            set {
                SetProperty(ref logLists, value, "LogLists");
            }
        }

        public LogDisplayViewModel(ISingleLoggerServer singleLoggerServer)
        {
            _singleLoggerServer = singleLoggerServer;
            logLists = _singleLoggerServer.LogLists;
        }
    }
}
