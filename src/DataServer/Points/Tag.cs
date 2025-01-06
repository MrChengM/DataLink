using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Points
{
    public interface ITag
    {
        string Name { get; set; }
        DataType Type { get; set; }
        string Value { get; set; }
        QUALITIES Quality { get; set; }

        DateTime TimeStamp { get; set; }
    }


    public class Tag : ITag
    {
        public string Name { get; set; }
        public DataType Type { get; set; }
        public string Value { get; set; }
        public QUALITIES Quality { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
