using Prism.Mvvm;
using DataServer.Permission;
using GuiBase.Services;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System;

namespace GuiBase.Models
{
    public class RoleWrapper: BindableBase
    {
        public string Id { get; set; }
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        
        private string createTime;

        public string CreateTime
        {
            get { return createTime; }
            set { SetProperty(ref createTime, value, "CreateTime"); }
        }

        private string createId;

        public string CreateId
        {
            get { return createId; }
            set { SetProperty(ref createId, value, "CreateId"); }
        }

        private bool status;

        public bool Status
        {
            get { return status; }
            set { SetProperty(ref status, value, "Status"); }
        }
        private List<string> resourceIds;

        public List<string> ResourceIds
        {
            get { return resourceIds; }
            set { SetProperty(ref resourceIds, value, "ResourceIds"); }
        }

        public static Role Convert(RoleWrapper wrapper, List<Resource> allResource)
        {
            var result = new Role();
            result.Id = wrapper.Id;
            result.Name = wrapper.Name;

            if (wrapper.Status)
            {
                result.Status = 0;
            }
            else
            {
                result.Status = 1;

            }
            result.CreateId = wrapper.CreateId;
            result.CreateTime = System.Convert.ToDateTime(wrapper.CreateTime);
            result.Resources = new List<Resource>();
            foreach (var resourceId in wrapper.ResourceIds)
            {
                var resource = allResource.First(s => s.Id == resourceId);
                if (resource!=null)
                {
                    result.Resources.Add(resource);

                }
            }
            return result;
        }
        public static RoleWrapper Convert(Role role)
        {
            var result = new RoleWrapper();
            result.Id = role.Id;
            result.Name = role.Name;
            if (role.Status == 0)
            {
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }
            result.CreateId = role.CreateId;
            result.CreateTime = role.CreateTime.ToString("G");
            result.ResourceIds = new List<string>();
            foreach (var resource in role.Resources)
            {
                result.ResourceIds.Add(resource.Id);
            }
            return result;
        }
        public RoleWrapper CopyTo()
        {
            return new RoleWrapper()
            {
                Id = Id,
                Name = Name,
                CreateTime = CreateTime,
                CreateId = CreateId,
                Status = Status,
                ResourceIds = ResourceIds,
            };
        }
    }
}
