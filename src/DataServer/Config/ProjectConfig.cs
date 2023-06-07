using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    public class ProjectConfig
    {
        private ClientConfig client;

        public ClientConfig Client
        {
            get { return client; }
            set { client = value; }
        }

        private ServerConfig server;

        public ServerConfig Server
        {
            get { return server; }
            set { server = value; }
        }

        public ProjectConfig()
        {
            client = new ClientConfig();
            server = new ServerConfig();
        }
    }
}
