using DataServer.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SignalRSelfHost.WebApi
{
    public class PermissionController : ApiController
    {
        private  IPermissionManager _permissionManager;

        public PermissionController(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }
        [ActionName("ValidateUser")]
        [HttpGet]
        public bool ValidateUser(string account, string pwd)
        {
            return _permissionManager.ValidateUser(account, pwd);
        }
        [ActionName("User")]
        [HttpGet]
        public User GetUser(string account,string pwd)
        {
           return _permissionManager.GetUser(account, pwd);
        }
        [ActionName("AllUsers")]
        [HttpGet]
        [AdminBasicAuthenticationFilter]
        public List<User> GetAllUser()
        {
            return _permissionManager.GetAllUsers();
        }
        [BasicAuthenticationFilter]
        [ActionName("User")]
        [HttpPut]
        public bool UpdateUser([FromBody]User user)
        {
            return _permissionManager.UpdateUser(user);
        }
        [AdminBasicAuthenticationFilter]
        [ActionName("User")]
        [HttpPost]
        public bool CreateUser([FromBody] User user)
        {
            return _permissionManager.CreateUser(user);
        }
        [AdminBasicAuthenticationFilter]
        [ActionName("User")]
        [HttpDelete]
        public bool DeleteUser([FromBody] User user)
        {
            return _permissionManager.DeleteUser(user);
        }
        [ActionName("Role")]
        [HttpGet]
        [AdminBasicAuthenticationFilter]
        public Role GetRole(string id)
        {
            return _permissionManager.GetRoleById(id);
        }
        [ActionName("AllRoles")]
        [HttpGet]
        [AdminBasicAuthenticationFilter]
        public List<Role> GetAllRoles()
        {
            return _permissionManager.GetAllRoles();
        }
        [ActionName("Role")]
        [HttpPut]
        [AdminBasicAuthenticationFilter]
        public bool UpdateRole([FromBody]Role role )
        {
            return _permissionManager.UpdateRole(role);
        }
        [ActionName("Role")]
        [HttpPost]
        [AdminBasicAuthenticationFilter]
        public bool CreateRole([FromBody] Role role)
        {
            return _permissionManager.CreateRole(role);
        }
        [ActionName("Role")]
        [HttpDelete]
        [AdminBasicAuthenticationFilter]
        public bool DeleteRole([FromBody] Role role)
        {
            return _permissionManager.DeleteRole(role);
        }
        [ActionName("Resource")]
        [HttpGet]
        [AdminBasicAuthenticationFilter]
        public Resource GetResource(string id)
        {
            return _permissionManager.GetResourceById(id);
        }
        [ActionName("AllResources")]
        [HttpGet]
        [AdminBasicAuthenticationFilter]
        public List<Resource> GetAllResources ()
        {
            return _permissionManager.GetAllResources();
        }
        [ActionName("Resource")]
        [HttpPut]
        [AdminBasicAuthenticationFilter]
        public bool UpdateResource([FromBody] Resource resource)
        {
            return _permissionManager.UpdateResource(resource);
        }
        [ActionName("Resource")]
        [HttpPost]
        [AdminBasicAuthenticationFilter]
        public bool CreateResource([FromBody] Resource resource)
        {
            return _permissionManager.CreateResource(resource);
        }
        [ActionName("Resource")]
        [HttpDelete]
        [AdminBasicAuthenticationFilter]
        public bool DeleteResource([FromBody] Resource resource)
        {
            return _permissionManager.DeleteResource(resource);
        }
    }
}
