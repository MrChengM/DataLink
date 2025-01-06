using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace GuiBase.Models
{
    public class UserFilterCondition : BindableBase
    {
        private string account;

        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value, "Account"); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

    }
}
