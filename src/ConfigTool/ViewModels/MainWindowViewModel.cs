using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace ConfigTool.ViewModels
{
    public class MainWindowViewModel:BindableBase
    {

        private string title ="ConfigTool";

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }

    }
}
