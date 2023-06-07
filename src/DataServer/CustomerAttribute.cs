using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
   [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
  public sealed class DriverDescriptionAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly CommunicationType  _driverType;

        readonly string _description;

        // This is a positional argument
        public DriverDescriptionAttribute(string description, CommunicationType driverType)
        {
            _description = description;
            _driverType = driverType;

       }

        public CommunicationType CommType
        {
            get { return _driverType; }
        }
        public string Description
        {
            get { return _description; }
        }
        // This is a named argument
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DeviceMarkAttribute : Attribute
    {
        public DeviceMarkAttribute()
        {

        }
    }
}
