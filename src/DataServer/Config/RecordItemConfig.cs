using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace DataServer.Config
{
    [DataContract]
    public class RecordItemConfig
    {
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private RecordWay option;
        [DataMember]
        public RecordWay Option
        {
            get { return option; }
            set { option = value; }
        }

        private int timeSpan;
        [DataMember]
        public int TimeSpan
        {
            get { return timeSpan; }
            set { timeSpan = value; }
        }

        private List<string> tagNames=new List<string>();
        [DataMember]
        public List<string> TagNames
        {
            get { return tagNames; }
            set { tagNames = value; }
        }

    }
}
