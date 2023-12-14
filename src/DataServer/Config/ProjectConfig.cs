using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    [DataContract]
    public class ProjectConfig
    {
        private ClientConfig client;
        [DataMember]
        public ClientConfig Client
        {
            get { return client; }
            set { client = value; }
        }

        private ServersConfig server;
        [DataMember]
        public ServersConfig Server
        {
            get { return server; }
            set { server = value; }
        }

        private AlarmsConfig alarms;
        [DataMember]
        public AlarmsConfig Alarms
        {
            get { return alarms; }
            set { alarms = value; }
        }

        private RecordsConfig records;
        [DataMember]
        public RecordsConfig Records
        {
            get { return records; }
            set { records = value; }
        }

        public ProjectConfig()
        {
            client = new ClientConfig();
            server = new ServersConfig();
            alarms = new AlarmsConfig();
            records = new RecordsConfig();
        }
    }
}
