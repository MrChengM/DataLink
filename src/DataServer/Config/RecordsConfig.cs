using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
    public class RecordsConfig
    {
        private Dictionary<string, RecordItemConfig> recordGroup = new Dictionary<string, RecordItemConfig>();
        [DataMember]
        public Dictionary<string,RecordItemConfig> RecordGroup
        {
            get { return recordGroup; }
            set { recordGroup = value; }
        }

    }
}
