using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
   public class ClientConfig
    {
        private Dictionary<string,ChannelConfig> channels;

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
