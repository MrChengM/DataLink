using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;

namespace ConfigTool.Models
{
    public class PropertyOptionItem:BindableBase
    {
        private string content;

        public string Content
        {
            get { return content; }
            set { SetProperty(ref content, value, "Content"); }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set { SetProperty(ref url, value, "Url"); }
        }

        private ICommand optionSelectCommand;

        public ICommand OptionSelectCommand
        {
            get { return optionSelectCommand; }
            set { SetProperty(ref optionSelectCommand, value, "OptionSelectCommand"); }
        }

        public object Config { get; set; }
    }
}
