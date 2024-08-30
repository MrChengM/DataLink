using DataServer;
using DataServer.Permission;
using System;
using System.Collections.Generic;
using Utillity.Communication;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly string ServerUrl = "http://localhost:3051/api/Permission";
        string _taskName;
        User currentUser;
        ILog _log;
        public SecurityService(ILog log)
        {
            _taskName = nameof(SecurityService);
            _log = log;
        }
        public void CancelLogin()
        {
            currentUser = null;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool result = false;
            var url = $"{ServerUrl}/User";
            var aut= RestAPIClient.UserNameToBase64Str(userName, oldPassword);
            currentUser.Password = newPassword;
            try
            {
                result = RestAPIClient.PutFuncJson<User,bool>(url,currentUser,aut);

            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}:{ ServerUrl} change password  error':{e.Message}'!");
            }
            return result;
        }

        public bool DeleteResource(Resource resource)
        {

            bool result = false;
            var url = $"{ServerUrl}/User";
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.DeleteFuncJson<Resource, bool>(url, resource, aut);
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public bool DeleteRole(Role role)
        {
            bool result = false;
            var url = $"{ServerUrl}/User";
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.DeleteFuncJson<Role, bool>(url, role, aut);
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public bool DeleteUser(User user)
        {
            bool result = false;
            var url = $"{ServerUrl}/User";
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.DeleteFuncJson<User, bool>(url, user, aut);
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public List<Resource> GetAllResources()
        {
            List<Resource> result = new List<Resource>();
            var url = $"{ServerUrl}/AllUsers";
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.GetFuncJson<List<Resource>>(url, aut);
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public List<Role> GetAllRoles()
        {
            List<Role> result = new List<Role>();
            var url = $"{ServerUrl}/AllUsers";
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.GetFuncJson<List<Role>>(url, aut);
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public List<User> GetAllUsers()
        {
            List<User> result = new List<User>();
            var url = $"{ServerUrl}/AllUsers";
            var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
            try
            {
                result = RestAPIClient.GetFuncJson<List<User>>(url, aut);
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public User GetCurrentUser()
        {
            return currentUser;
        }

        public bool IsPermission(ResourceType resourceName)
        {
            throw new NotImplementedException();
        }

        public bool IsValidLogin(string userName, string password)
        {
            bool result = false;
            var url = $"{ServerUrl}/ValidateUser?account={userName}&pwd={password}";
            try
            {
                currentUser = RestAPIClient.GetFuncJson<User>(url);
                if (currentUser != null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                currentUser = null;
                _log.ErrorLog($"{_taskName}: { ServerUrl} Login error':{e.Message}'!");
            }
            return result;
        }

        public bool UpdateResource(Resource resource)
        {
            bool result = false;
            try
            {
                var url = $"{ServerUrl}/User";
                var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
                result = RestAPIClient.PutFuncJson<Resource, bool>(url, resource, aut);
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: { ServerUrl} Update resource  error':{e.Message}'!");
            }
            return result;
        }

        public bool UpdateRole(Role role)
        {
            bool result = false;
            try
            {
                var url = $"{ServerUrl}/User";
                var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
                result = RestAPIClient.PutFuncJson<Role, bool>(url, role, aut);
            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: { ServerUrl} Update role  error':{e.Message}'!");
            }
            return result;
        }

        public bool UpdateUser(User user)
        {

            bool result = false;
            try
            {
                var url = $"{ServerUrl}/User";
                var aut = RestAPIClient.UserNameToBase64Str(currentUser.Account, currentUser.Password);
                result = RestAPIClient.PutFuncJson<User, bool>(url, user, aut);

            }
            catch (Exception e)
            {
                _log.ErrorLog($"{_taskName}: { ServerUrl} Update User  error':{e.Message}'!");
            }
            return result;
        }
    }
}
