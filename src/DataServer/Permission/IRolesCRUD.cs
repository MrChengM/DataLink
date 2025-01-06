using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public interface IRolesCRUD
    {
        Role GetRoleById(string id);

        Role GetRoleByName(string roleName);
        List<Role> GetAllRoles();

        bool CreateRole(Role role);

        bool UpdateRole(Role role);

        bool DeleteRole(Role role);
    }
}
