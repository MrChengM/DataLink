using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace DataServer.Config
{
    [DataContract]
    public class TagBindingConfig
    {
        private string destTagName;

        [DataMember]
        public string DestTagName
        {
            get { return destTagName; }
            set { destTagName = value; }
        }


        private string sourceTagName;
        [DataMember]
        public string SourceTagName
        {
            get { return sourceTagName; }
            set { sourceTagName = value; }
        }

    }
}
