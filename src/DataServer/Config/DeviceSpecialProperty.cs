using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Config
{
    public class DeviceSpecialProperty
    {
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

        //private Type type;

        //public Type Type
        //{
        //    get { return type; }
        //    set { type = value; }
        //}


        private string value;

		public string Value
		{
			get { return value; }
			set { this.value = value; }
		}

	}
}
