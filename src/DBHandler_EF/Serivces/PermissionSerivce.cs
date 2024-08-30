using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Permission;
using dbm=DBHandler_EF.Modules;

namespace DBHandler_EF.Serivces
{
    public class PermissionSerivce : IPermissionManager
    {
        public bool CreateResource(Resource resource)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                perContent.Resource.Add(Convert(resource));
                result = perContent.SaveChanges();
            }
            return result > 0 ;
        }
        public bool CreateUser(User user)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                perContent.User.Add(Convert(user));
                result = perContent.SaveChanges();
                if (result > 0)
                {
                    perContent.User_Role.AddRange(GetUser_Roles(user));
                    result = perContent.SaveChanges();
                }
            }
            return result > 0 ;

        }
        public bool CreateRole(Role role)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                perContent.Role.Add(Convert(role));
                result = perContent.SaveChanges();
                if (result > 0)
                {
                    perContent.Role_Resource.AddRange(GetRole_Resources(role));
                    result = perContent.SaveChanges();
                }
            }
            return result > 0;

        }

        public bool UpdateRole(Role role)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                var res = perContent.Role.AsNoTracking().FirstOrDefault(s => s.Id == role.Id);
                var newRole = Convert(role);
                perContent.Role.Attach(newRole);
                perContent.Entry(newRole).State = System.Data.Entity.EntityState.Modified;
                result = perContent.SaveChanges();

                if (result>0)
                {
                    var newRole_ResL = GetRole_Resources(role);
                    var oldRole_ResL = from a in perContent.Role_Resource
                                       where a.RoleId == role.Id
                                       select a;
                    foreach (var role_res in newRole_ResL)
                    {
                        var oldOne = oldRole_ResL.First(s => s.ResourceId == role_res.ResourceId);
                        if (oldOne == null)
                        {
                            perContent.Role_Resource.Add(role_res);
                        }
                    }
                    foreach (var role_res in oldRole_ResL)
                    {
                        var newOne = newRole_ResL.First(s => s.ResourceId == role_res.ResourceId);
                        if (newOne == null)
                        {
                            perContent.Role_Resource.Remove(role_res);
                        }
                    }
                    result = perContent.SaveChanges();
                }
               
            }
            return result > 0;
        }

        public bool DeleteRole(Role role)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                var newRole = Convert(role);
                perContent.Role.Remove(newRole);
                result = perContent.SaveChanges();
                if (result > 0)
                {
                    var oldRole_Ress = from a in perContent.Role_Resource
                                      where a.RoleId == role.Id
                                      select a;
                    var oldUser_Roles = from a in perContent.User_Role
                                        where a.RoleId == role.Id
                                        select a;
                    perContent.Role_Resource.RemoveRange(oldRole_Ress);
                    perContent.User_Role.RemoveRange(oldUser_Roles);
                    result = perContent.SaveChanges();
                }

            }
            return result > 0;
        }

        public bool UpdateResource(Resource resource)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                var res = perContent.Resource.AsNoTracking().FirstOrDefault(s => s.Id == resource.Id);
                var newRes = Convert(resource);
                perContent.Resource.Attach(newRes);
                perContent.Entry(newRes).State= System.Data.Entity.EntityState.Modified;
                result = perContent.SaveChanges();
            }
            return result > 0 ;
        }

        public bool DeleteResource(Resource resource)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                var res = perContent.Resource.FirstOrDefault(s => s.Id == resource.Id);
                perContent.Resource.Remove(res);
                result = perContent.SaveChanges();
                if (result>0)
                {
                    var role_Res = from a in perContent.Role_Resource
                                   where a.ResourceId == resource.Id
                                   select a;
                    perContent.Role_Resource.RemoveRange(role_Res);
                    result = perContent.SaveChanges();
                }
            }
            return result > 0 ;
        }
        public bool UpdateUser(User user)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                var res = perContent.User.AsNoTracking().FirstOrDefault(s => s.Id == user.Id);
                var newUser = Convert(user);
                perContent.User.Attach(newUser);
                perContent.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                result = perContent.SaveChanges();
                if (result>0)
                {
                    var newUser_RoleL = GetUser_Roles(user);
                    var oldUser_RoleL = from a in perContent.User_Role
                                        where a.UserId == user.Id
                                        select a;
                    foreach (var user_Role in newUser_RoleL)
                    {
                        var oldOne = oldUser_RoleL.First(s => s.RoleId == user_Role.RoleId);
                        if (oldOne == null)
                        {
                            perContent.User_Role.Add(user_Role);
                        }
                    }
                    foreach (var user_Role in oldUser_RoleL)
                    {
                        var newOne = newUser_RoleL.First(s => s.RoleId == user_Role.RoleId);
                        if (newOne == null)
                        {
                            perContent.User_Role.Remove(user_Role);
                        }
                    }
                    result = perContent.SaveChanges();
                }
            }
            return result > 0;
        }
        public bool DeleteUser(User user)
        {
            int result;
            using (var perContent = new dbm.PermissionContent())
            {
                var newUser = Convert(user);
                perContent.User.Remove(newUser);
                result = perContent.SaveChanges();
                if (result > 0)
                {
                    var oldUser_Roles = from a in perContent.User_Role
                                        where a.UserId == user.Id
                                        select a;
                    perContent.User_Role.RemoveRange(oldUser_Roles);
                    result = perContent.SaveChanges();
                }
            }
            return result > 0;
        }
        public Resource GetResourceById(string id)
        {
            Resource result;
            using (var perContent = new dbm.PermissionContent())
            {
                result = Convert(perContent.Resource.First(s => s.Id == id));
            }
            return result;
        }

        public Resource GetResourceByName(string name)
        {

            Resource result;
            using (var perContent = new dbm.PermissionContent())
            {
                result = Convert(perContent.Resource.First(s => s.Name == name));
            }
            return result;
        }
        public List<Resource> GetAllResources()
        {
            List<Resource> result=new List<Resource>();
            using (var perContent = new dbm.PermissionContent())
            {
                foreach (var resource in perContent.Resource)
                {
                    result.Add(Convert(resource));
                } 
            }
            return result;
        }
        public List<Role> GetAllRoles()
        {
            List<Role> result = new List<Role>();
            using (var perContent = new dbm.PermissionContent())
            {
                foreach (var role in perContent.Role)
                {
                    var newRole = Convert(role);
                    newRole.Resources = new List<Resource>();
                    var role_Ress = from a in perContent.Role_Resource
                                    where a.RoleId == newRole.Id
                                    select a;
                    foreach (var role_Res in role_Ress)
                    {
                        newRole.Resources.Add(GetResourceById(role_Res.ResourceId));
                    }
                    result.Add(newRole);
                }
            }
            return result;
        }
        public Role GetRoleById(string id)
        {
            Role result;
            using (var perContent = new dbm.PermissionContent())
            {

                var newRole = Convert(perContent.Role.First(s => s.Id == id));
                if (newRole!=null)
                {
                    newRole.Resources = new List<Resource>();
                    var role_Ress = from a in perContent.Role_Resource
                                    where a.RoleId == newRole.Id
                                    select a;
                    foreach (var role_Res in role_Ress)
                    {
                        newRole.Resources.Add(GetResourceById(role_Res.ResourceId));
                    }
                    result = newRole;
                }
                else
                {
                    result = null;
                }
               
            }
            return result;
        }

        public Role GetRoleByName(string roleName)
        {

            Role result;
            using (var perContent = new dbm.PermissionContent())
            {

                var newRole = Convert(perContent.Role.First(s => s.Name == roleName));
                if (newRole!=null)
                {
                    newRole.Resources = new List<Resource>();
                    var role_Ress = from a in perContent.Role_Resource
                                    where a.RoleId == newRole.Id
                                    select a;
                    foreach (var role_Res in role_Ress)
                    {
                        newRole.Resources.Add(GetResourceById(role_Res.ResourceId));
                    }
                    result = newRole;
                }
                else
                {
                    result = null;
                }
              
            }
            return result;
        }
        public List<User> GetAllUsers()
        {
            List<User> result = new List<User>();
            using (var perContent = new dbm.PermissionContent())
            {
                foreach (var user in perContent.User)
                {
                    var newUser = Convert(user);
                    newUser.Roles = new List<Role>();
                    var user_Roles = from a in perContent.User_Role
                                    where a.UserId == newUser.Id
                                    select a;
                    foreach (var user_Role in user_Roles)
                    {
                        newUser.Roles.Add(GetRoleById(user_Role.RoleId));
                    }
                    result.Add(newUser);
                }
            }
            return result;
        }
        public bool ValidateUser(string account, string pwd)
        {
            
            using (var perContent = new dbm.PermissionContent())
            {
                var newUser = perContent.User.First(s => s.Account == account);
                if (newUser != null)
                {
                    if (newUser.Password == pwd)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
        }
        public User GetUser(string account, string pwd)
        {

            User result;
            using (var perContent = new dbm.PermissionContent())
            {
                var newUser = Convert(perContent.User.First(s => s.Account == account));
                if (newUser != null)
                {
                    if (newUser.Password == pwd)
                    {
                        result = newUser;
                    }
                    else
                    {
                      return  result = null;
                    }
                    newUser.Roles = new List<Role>();
                    var user_Roles = from a in perContent.User_Role
                                     where a.UserId == newUser.Id
                                     select a;
                    foreach (var user_Role in user_Roles)
                    {
                        newUser.Roles.Add(GetRoleById(user_Role.RoleId));
                    }
                    
                }
                else
                {
                    result = null;
                }
              
            }
            return result;
        }
        dbm.Resource Convert( Resource resource)
        {
            if (resource!=null)
            {
                return new dbm.Resource()
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Description = resource.Description,
                    ParentName = resource.ParentName,
                    ParentId = resource.ParentId,
                    Disable = resource.Disable,
                    CreateUserId = resource.CreateUserId,
                    CreateTime = resource.CreateTime,
                    UpdateTime = resource.UpdateTime,
                    UpdateUserId = resource.UpdateUserId
                };
            }
            else
            {
                return null;
            }
           
        }
        Resource Convert(dbm.Resource resource)
        {
            if (resource != null)
            {
                return new Resource()
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Description = resource.Description,
                    ParentName = resource.ParentName,
                    ParentId = resource.ParentId,
                    Disable = resource.Disable,
                    CreateUserId = resource.CreateUserId,
                    CreateTime = resource.CreateTime,
                    UpdateTime = resource.UpdateTime,
                    UpdateUserId = resource.UpdateUserId
                };
            }
            else
            {
                return null;
            }

        }
        dbm.Role Convert(Role role)
        {
            if (role!=null)
            {
                return new dbm.Role()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Status = role.Status,
                    CreateTime = role.CreateTime,
                    CreateId = role.CreateId
                };
            }
            else
            {
                return null;
            }
         
        }
        Role Convert(dbm.Role role)
        {
            if (role!=null)
            {
                return new Role()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Status = role.Status,
                    CreateTime = role.CreateTime,
                    CreateId = role.CreateId
                };
            }
            else
            {
                return null;
            }
           
        }

        List<dbm.Role_Resource> GetRole_Resources(Role role)
        {
            List<dbm.Role_Resource> result = new List<dbm.Role_Resource>();
            foreach (var res in role.Resources)
            {
                result.Add(new dbm.Role_Resource() { RoleId = role.Id, ResourceId = res.Id });
            }
            return result;
        }
        dbm.User Convert(User user)
        {
            return new dbm.User()
            {
                Id=user.Id,
                Account=user.Account,
                Password=user.Password,
                Name=user.Name,
                SSex=user.SSex,
                Status=user.Status,
                CreateId=user.CreateId,
                CreateTime=user.CreateTime
            };
        }
        User Convert(dbm.User user)
        {
            if (user!=null)
            {
                return new User()
                {
                    Id = user.Id,
                    Account = user.Account,
                    Password = user.Password,
                    Name = user.Name,
                    SSex = user.SSex,
                    Status = user.Status,
                    CreateId = user.CreateId,
                    CreateTime = user.CreateTime
                };
            }
            else
            {
                return null;
            }
           
        }
        List<dbm.User_Role> GetUser_Roles(User user)
        {
            List<dbm.User_Role> result = new List<dbm.User_Role>();
            foreach (var role in user.Roles)
            {
                result.Add(new dbm.User_Role() { RoleId = role.Id, UserId = user.Id });
            }
            return result;
        }

       
    }
}
