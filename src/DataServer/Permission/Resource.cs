using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public class Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentName { get; set; }

        public string ParentId { get; set; }

        public bool Disable { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserId { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string UpdateUserId { get; set; }
    }
}
