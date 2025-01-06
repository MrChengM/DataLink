using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace ConfigTool.Models
{
    public class PropertyData:BindableBase
    {

        private string name;

        public string Name
        {
            get { return name; }
            set {SetProperty(ref name,value,"Name" ); }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value, "Value"); }
        }

        private bool isWrite;

        public bool IsWrite
        {
            get { return isWrite; }
            set { SetProperty(ref isWrite, value, "IsWrite"); }
        }

    }
}
