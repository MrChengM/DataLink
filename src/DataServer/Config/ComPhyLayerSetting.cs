using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataServer.Config
{
   public class ComPhyLayerSetting
    {
        public SerialportSetUp SerialportSet { get; set; } = new SerialportSetUp();

        public EthernetSetUp EthernetSet { get; set; } = new EthernetSetUp();

        public MemorySetUp MemorySetUp { get; set; } = new MemorySetUp();
    }
}
