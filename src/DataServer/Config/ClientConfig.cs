using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
   public class ClientConfig
    {
        private Dictionary<string,ChannelConfig> channels;
        [DataMember]
        public Dictionary<string,ChannelConfig> Channels
        {
            get { return channels; }
            set { channels = value; }
        }

        public ClientConfig()
        {
            channels = new Dictionary<string, ChannelConfig>();
        }
    }
}
