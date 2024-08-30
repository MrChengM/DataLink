using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Permission;

namespace GuiBase.Services
{
    public interface ISecurityService
    {

        bool IsValidLogin(string userName, string password);
        void CancelLogin();
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool IsPermission(ResourceType resourceName);
        User GetCurrentUser();
        List<User> GetAllUsers();
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        List<Role> GetAllRoles();
        bool UpdateRole(Role role);
        bool DeleteRole(Role role);
        List<Resource> GetAllResources();
        bool UpdateResource(Resource resource);
        bool DeleteResource(Resource resource);
    }
}
