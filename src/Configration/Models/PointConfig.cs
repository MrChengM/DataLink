using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Conifguration
{
  public class PointConfig
    {
        private string tagName;

        public string TagName
        {
            get { return tagName; }
            set { tagName = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string clientAccess;

        public string ClientAccess
        {
            get { return clientAccess; }
            set { clientAccess = value; }
        }

    }
}
