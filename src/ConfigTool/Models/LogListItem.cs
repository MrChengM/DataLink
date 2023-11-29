using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
namespace ConfigTool.Models
{
    public class LogListItem : BindableBase

    {
        private string  date;

        public string  Date
        {
            get {
                return date;
            }
            set {
                SetProperty(ref date, value, "Date");
            }
        }

        private string time;

        public string Time
        {
            get {
                return time;
            }
            set {
                SetProperty(ref time, value, "Time");
            }
        }

        private string level;

        public string Level
        {
            get {
                return level;
            }
            set {
                SetProperty(ref level, value, "Level");
            }
        }

        private string source;

        public string Source
        {
            get {
                return source;
            }
            set {
                SetProperty(ref source, value, "Source");
            }
        }

        private string message;

        public string Message
        {
            get {
                return message;
            }
            set {
                SetProperty(ref message, value, "Message");
            }
        }


    }
}
