using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using DataServer.Permission;

namespace GuiBase.Models
{
    public class ResourceWrapper : BindableBase
    {
        public string Id { get; set; }
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value, "Description"); }
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

        private bool disable;

        public bool Disable
        {
            get { return disable; }
            set { SetProperty(ref disable, value, "Status"); }
        }

        private string parentName;

        public string ParentName
        {
            get { return parentName; }
            set { SetProperty(ref parentName, value, "ParentName"); }
        }
        private string parentId;

        public string ParentId
        {
            get { return parentId; }
            set { SetProperty(ref parentId, value, "ParentId"); }
        }

        private string updateTime;

        public string UpdateTime
        {
            get { return updateTime; }
            set { SetProperty(ref updateTime, value, "UpdateTime"); }
        }

        private string updateId;

        public string UpdateId
        {
            get { return updateId; }
            set { SetProperty(ref updateId, value, "UpdateId"); }
        }


        public static Resource Convert(ResourceWrapper wrapper)
        {
            return new Resource()
            {
                Id = wrapper.Id,
                Name = wrapper.Name,
                Description = wrapper.Description,
                CreateTime = System.Convert.ToDateTime(wrapper.CreateTime),
                CreateUserId = wrapper.CreateId,
                Disable = wrapper.Disable,
                ParentName=wrapper.ParentName,
                ParentId = wrapper.ParentId,
                UpdateTime = System.Convert.ToDateTime(wrapper.UpdateTime),
                UpdateUserId = wrapper.UpdateId
            };
        }
        public static ResourceWrapper Convert(Resource resource)
        {
            return new ResourceWrapper()
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                CreateTime = resource.CreateTime.ToString("G"),
                CreateId = resource.CreateUserId,
                Disable = resource.Disable,
                ParentName = resource.ParentName,
                ParentId = resource.ParentId,
                UpdateTime = resource.UpdateTime?.ToString("G"),
                UpdateId = resource.UpdateUserId
            };
        }
        public  ResourceWrapper CopyTo()
        {
            return new ResourceWrapper()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                CreateTime = CreateTime,
                CreateId = CreateId,
                Disable = Disable,
                ParentName = ParentName,
                ParentId = ParentId,
                UpdateTime = UpdateTime,
                UpdateId = UpdateId
            };
        }
    }
}
