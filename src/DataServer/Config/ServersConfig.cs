using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
    public class ServersConfig
    {
        private Dictionary<string,ServerItemConfig> items;
        [DataMember]
        public Dictionary<string,ServerItemConfig> Items
        {
            get { return items; }
            set { items = value; }
        }

        public ServersConfig()
        {
            items = new Dictionary<string,ServerItemConfig>();
        }

    }
}
