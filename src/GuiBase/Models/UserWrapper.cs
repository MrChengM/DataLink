using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using DataServer.Permission;
using GuiBase.Services;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GuiBase.Models
{
    public class UserWrapper : BindableBase
    {
        public string Id { get; set; }
        private string account;

        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value, "Account"); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        private string ssex;

        public string SSex
        {
            get { return ssex; }
            set { SetProperty(ref ssex, value, "SSex"); }
        }

        private string createTime;

        public string CreateTime
        {
            get { return createTime; }
            set { SetProperty(ref createTime, value, "CreateTime"); }
        }

        private string createId;

        public string CreateId
        {
            get { return createId; }
            set { SetProperty(ref createId, value, "CreateId"); }
        }

        private bool status;

        public bool Status
        {
            get { return status; }
            set { SetProperty(ref status, value, "Status"); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value, "Password"); }
        }
        private string roleNames;

        public string RoleNames
        {
            get { return roleNames; }
            set { SetProperty(ref roleNames, value, "RoleNames"); }
        }
        private ObservableCollection<RoleNameEx> roleNameExs;
        public ObservableCollection <RoleNameEx> RoleNameExs 
        {
            get
            { return roleNameExs; }

            set
            {
                roleNameExs = value;
                roleNameExs.CollectionChanged += (s, e) =>
                {
                    getRoleNames();
                    if (e.OldItems != null)
                    {
                        foreach (RoleNameEx roleNameEx in e.OldItems)
                        {
                            roleNameEx.PropertyChanged -= ItemPropertyChanged;
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (RoleNameEx roleNameEx in e.NewItems)
                        {
                            roleNameEx.PropertyChanged += ItemPropertyChanged;
                        }
                    }
                };
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                RoleNameEx roleNameEx = sender as RoleNameEx;

                if (roleNameEx != null)
                {
                    getRoleNames();
                }
            }
        }
        private void getRoleNames()
        {
            IEnumerable<RoleNameEx> roleNameExs = RoleNameExs.Where(b => b.IsChecked == true);

            StringBuilder builder = new StringBuilder();

            foreach (var item in roleNameExs)
            {
                builder.Append(item.Name + " ");
            }

            RoleNames = builder == null ? string.Empty : builder.ToString();
        }

        public static User Convert(UserWrapper wrapper, List<Role> allRoles)
        {
            User result = new User();
            result.Id = wrapper.Id;
            result.Account = wrapper.Account;
            result.Name = wrapper.Name;
            result.SSex = (int)(SSex)Enum.Parse(typeof(SSex), wrapper.SSex);
            if (wrapper.Status)
            {
                result.Status = 0;
            }
            else
            {
                result.Status = 1;

            }
            result.CreateId = wrapper.CreateId;
            result.Password = wrapper.Password;
            result.CreateTime = System.Convert.ToDateTime(wrapper.CreateTime);
            result.Roles = new List<Role>();
            foreach (var roleNameEx in wrapper.RoleNameExs)
            {
                if (roleNameEx.IsChecked)
                {
                    result.Roles.Add(allRoles.First(s => s.Name == roleNameEx.Name));
                }
            }
            return result;
        }
        public static UserWrapper Convert(User user, List<Role> allRoles)
        {
            UserWrapper result = new UserWrapper();
            result.Id = user.Id;
            result.Account = user.Account;
            result.Name = user.Name;
            result.SSex = ((SSex)user.SSex).ToString();
            if (user.Status==0)
            {
                result.Status = true;
            }
            else
            {
                result.Status = false;

            }
            result.CreateId = user.CreateId;
            result.CreateTime =user.CreateTime.ToString("G");
            result.Password = user.Password;
            result.RoleNameExs = new ObservableCollection<RoleNameEx>();
            foreach (var role in allRoles)
            {
                var roleNameEx = new RoleNameEx();
                roleNameEx.Name = role.Name;
                if (user.Roles != null && user.Roles.Count > 0)
                {
                    var enableRole = user.Roles.Find(s => s.Id == role.Id);
                    if (enableRole != null)
                    {
                        roleNameEx.IsChecked = true;
                    }
                    else
                    {
                        roleNameEx.IsChecked = false;
                    }
                }
                else
                {
                    roleNameEx.IsChecked = false;

                }
                result.RoleNameExs.Add(roleNameEx);
            }
            return result;
        }
        public UserWrapper CopyTo()
        {
            return new UserWrapper()
            {
                Id = Id,
                Account = Account,
                Name = Name,
                SSex = SSex,
                CreateTime = CreateTime,
                CreateId = CreateId,
                Status = Status,
                Password = Password,
                RoleNames = RoleNames,
                RoleNameExs = RoleNameExs,
            };
        }
    }
}
