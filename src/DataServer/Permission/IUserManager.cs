using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public interface IUserManager
    {
        bool ValidateUser(string account, string pwd);

        User GetUser(string account, string pwd);
        List<User> GetAllUsers();

        bool CreateUser(User user);

        bool UpdateUser(User user);

        bool DeleteUser(User user);

    }
}
