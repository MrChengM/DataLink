using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;
using Prism.Mvvm;

namespace ConfigTool.Models
{
  public class DeviceSpecailPropertyMVVM:BindableBase
    {
        private string propertyName;

        public string PropertyName
        {
            get { return propertyName; }
            set { SetProperty(ref propertyName, value, "PropertyName"); }
        }

        private string propertyValue;

        public string PropertyValue
        {
            get { return propertyValue; }
            set { SetProperty(ref propertyValue, value, "PropertyValue"); }
        }

        //private Type propertyType;

        //public Type PropertyType
        //{
        //    get { return propertyType; }
        //    set { propertyType = value; }
        //}

        public DeviceSpecialProperty Convert()
        {
            return new DeviceSpecialProperty { Name = PropertyName, Value = PropertyValue };
        }
        public static DeviceSpecailPropertyMVVM Convert(DeviceSpecialProperty specialProperty)
        {
            return new DeviceSpecailPropertyMVVM { PropertyName = specialProperty.Name, PropertyValue = specialProperty.Value };
        }

    }
}
