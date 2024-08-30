using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public class Role
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateId { get; set; }

        public List<Resource> Resources { get; set; }
    }
}
