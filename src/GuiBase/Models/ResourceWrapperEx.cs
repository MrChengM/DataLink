using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
namespace GuiBase.Models
{
    public class ResourceWrapperEx:BindableBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value, "IsChecked"); }
        }
        private ResourceWrapper wrapper;

        public ResourceWrapper Wrapper
        {
            get { return wrapper; }
            set { SetProperty(ref wrapper, value, "Wrapper"); }
        }

    }
}
