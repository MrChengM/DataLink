using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
    public class TagGroupConfig
    {
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        private Dictionary<string,TagConfig> tags=new Dictionary<string, TagConfig> ();
        [DataMember]
        public Dictionary<string,TagConfig> Tags
        {
            get { return tags; }
            set { tags = value; }
        }

    }
}
