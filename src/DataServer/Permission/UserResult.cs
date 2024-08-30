using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public class UserResult
    {
        public bool IsSucess { get; set; }

        public User User { get; set; }
        public string Token { get; set; }
    }
}
