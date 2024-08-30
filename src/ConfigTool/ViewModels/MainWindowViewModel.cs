using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.Service;
using Prism.Mvvm;

namespace ConfigTool.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {

        private string title ="ConfigTool";
        private IConfigDataServer _configDataServer;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }
        public MainWindowViewModel( IConfigDataServer configDataServer)
        {

            _configDataServer = configDataServer;
            updata(null, _configDataServer.IsConnect);
            _configDataServer.OnlineChangeEvent += updata;

        }
        void updata(object s,bool e)
        {
            if (e)
            {
                Title = "ConfigTool" + "[Online]";
            }
            else
            {
                Title = "ConfigTool" + "[Offline]";

            }
        }
    }
}
