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
        User GetCurrentUser();
        List<User> GetAllUsers();
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        List<Role> GetAllRoles();
        bool CreateRole(Role role);

        bool UpdateRole(Role role);
        bool DeleteRole(Role role);
        List<Resource> GetAllResources();

        bool IsPermission(string name,ResourceType type);
        bool ResgisterResourceName(string name, ResourceType type);
        List<string> GetResourceNames();

        bool CreateResource(Resource resource);

        bool UpdateResource(Resource resource);
        bool DeleteResource(Resource resource);

        event Action<User> UserChangeEvent;
    }
}
