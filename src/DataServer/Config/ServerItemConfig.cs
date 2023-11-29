using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using DataServer;

namespace DataServer.Config
{
    [DataContract]
    public class ServerItemConfig
    {
        private string name;
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ServerOption type;
        [DataMember]
        public ServerOption Option
        {
            get { return type; }
            set { type = value; }
        }


        private ComPhyLayerSetting phyLayerSetting=new ComPhyLayerSetting();
        [DataMember]
        public ComPhyLayerSetting ComunicationSetUp
        {
            get { return phyLayerSetting; }
            set { phyLayerSetting = value; }
        }
        private string addressId;

        [DataMember]
        public string Address
        {
            get { return addressId; }
            set { addressId = value; }
        }
        private Dictionary<string,TagBindingConfig> tagBindingList=new Dictionary<string, TagBindingConfig>();
        [DataMember]
        public Dictionary<string, TagBindingConfig> TagBindingList
        {
            get { return tagBindingList; }
            set { tagBindingList = value; }
        }
    }
}
