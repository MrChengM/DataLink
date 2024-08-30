using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public class User
    {
        public string Id { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }
        public string Name { get; set; }

        public int SSex { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateId { get; set; }

        public List<Role> Roles { get; set; }

    }
}
