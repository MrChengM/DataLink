using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.Helper
{
    public class LogFilter
    {
        private LogSource source;

        public LogSource Source
        {
            get { return source; }
            set { source = value; }
        }

        private LogLevel level;

        public LogLevel Level
        {
            get { return level; }
            set { level = value; }
        }

    }
    [Flags]
    public enum LogSource
    {
        ConfigTool=0x01,
        DataLink=0x02
    }
    [Flags]
    public enum LogLevel
    {
        Info=0x01,
        Error=0x02

    }
}
