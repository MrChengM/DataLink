using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
    public class TagConfig
    {
        private string name;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        private string address;
        [DataMember]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private DataType dataType;
        [DataMember]
        public DataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        private int length;
        [DataMember]
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        private Scaling scaling = new Scaling();
        [DataMember]
        public Scaling Scaling
        {
            get { return scaling; }
            set { scaling = value; }
        }

        private OperateWay operate;
        [DataMember]
        public OperateWay Operate
        {
            get { return operate; }
            set { operate = value; }
        }

    }
}
