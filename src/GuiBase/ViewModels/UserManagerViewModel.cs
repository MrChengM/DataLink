using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using Prism.Services.Dialogs;
using Prism.Events;
using GuiBase.Services;
using System.Collections.ObjectModel;
using GuiBase.Models;
using DataServer.Permission;
using System.Windows;

namespace GuiBase.ViewModels
{
    public class UserManagerViewModel : BindableBase, INavigationAware
    {

        private ISecurityService _securityService;
        private IDialogService _dialogService;
        private IEventAggregator _ea;
        private ILocalizationService _localizationService;

        private ObservableCollection<UserWrapper> userWrappers;

        public ObservableCollection<UserWrapper> UserWrappers
        {
            get { return userWrappers; }
            set { SetProperty(ref userWrappers, value, "UserWrappers"); }
        }

        private UserCaptions captions;

        public UserCaptions Captions
        {
            get { return captions; }
            set { SetProperty(ref captions, value, "Captions"); }
        }



        private List<User> users;
        public UserFilterCondition UserFilter { get; set; }

        public DelegateCommand<UserWrapper> EditUserCommand { get; set; }
        public DelegateCommand<UserWrapper> DeleteUserCommand { get; set; }
        public DelegateCommand SearchUsersCommand { get; set; }
        public DelegateCommand AddUserCommand { get; set; }
        public DelegateCommand ExportUsersCommand { get; set; }
        public DelegateCommand ImportUsersCommand { get; set; }
        public DelegateCommand<UserWrapper> EnableOpertCommand { get; set; }

        public UserManagerViewModel(ISecurityService securityService, IDialogService dialogService,ILocalizationService localizationService,IEventAggregator ea)
        {
            _securityService = securityService;
            _dialogService = dialogService;
            _ea = ea;
            _ea.GetEvent<PubSubEvent<DialogClosedResult>>().Subscribe(onDialogClosed);

            _localizationService = localizationService;
            _localizationService.LanguageChanged += onLanguageChanged;
            Captions = new UserCaptions(_localizationService);
            translate();

            UserFilter = new UserFilterCondition();
            EditUserCommand = new DelegateCommand<UserWrapper>(editUser);
            DeleteUserCommand = new DelegateCommand<UserWrapper>(deleteUser);
            EnableOpertCommand= new DelegateCommand<UserWrapper>(enableOpert);
            SearchUsersCommand = new DelegateCommand(searchUsers);
            AddUserCommand = new DelegateCommand(addUser);
            ExportUsersCommand = new DelegateCommand(exportUsers);
            ImportUsersCommand = new DelegateCommand(importUsers);
            UserWrappers = new ObservableCollection<UserWrapper>();
            searchUsers();
        }
        private void onDialogClosed(DialogClosedResult result)
        {
            if (result.ViewName == "AccManagerView")
            {
                Clear();
            }
        }
        private void onLanguageChanged(LanguageChangedEvent e)
        {
            translate();
        }
        private void translate()
        {
            Captions.GetContent();
            
        }
        private void enableOpert(UserWrapper wrapper)
        {
            var caption = _localizationService.Translate(TranslateCommonId.WaringId);
            var message = _localizationService.Translate(TranslateCommonId.DisableUserWarningId);
            var message1 = _localizationService.Translate(TranslateCommonId.DisableUserFailId);
            var message2 = _localizationService.Translate(TranslateCommonId.EnableUserFailId);
            if (!wrapper.Status)
            {
                var btnResult = MessageBox.Show(message, caption, MessageBoxButton.OKCancel);
                if (btnResult == MessageBoxResult.OK)
                {
                    var roles = _securityService.GetAllRoles();
                    if (_securityService.UpdateUser(UserWrapper.Convert(wrapper, roles)))
                    {
                        wrapper.Status = false;
                    }
                    else
                    {
                        MessageBox.Show(message1, caption);
                        wrapper.Status = true;

                    }
                }
                else
                {
                    wrapper.Status = true;
                }
            }
            else
            {
                var roles = _securityService.GetAllRoles();
                if (_securityService.UpdateUser(UserWrapper.Convert(wrapper, roles)))
                {
                    wrapper.Status = true;
                }
                else
                {
                    MessageBox.Show(message2, caption);
                    wrapper.Status = false;

                }
            }
        }

        private void importUsers()
        {
        }

        private void exportUsers()
        {
        }

        private void addUser()
        {
            var dParam = new DialogParameters();
            dParam.Add("isBuild", true);
            _dialogService.ShowDialog("UserEditView", dParam, s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    searchUsers();
                }

            });
        }

        private void searchUsers()
        {
            users = _securityService.GetAllUsers();
            var roles = _securityService.GetAllRoles();

            IEnumerable<User> users1, users2;

            if (UserFilter.Account != null && UserFilter.Account != "")
            {
                users1 = from a in users
                         where a.Account == UserFilter.Account
                         select a;
                if (userWrappers.Count() > 0)
                {
                    if (UserFilter.Name != null && UserFilter.Name != "")
                    {
                        users2 = from a in users1
                                 where a.Account.Contains(UserFilter.Account)
                                 select a;
                    }
                    else
                    {
                        users2 = users1;
                    }
                }
                else
                {
                    users2 = users1;
                }
            }
            else
            {
                if (UserFilter.Name != null && UserFilter.Name != "")
                {
                    users2 = from a in users
                             where a.Account.Contains(UserFilter.Account)
                             select a;
                }
                else
                {
                    users2 = users;
                }
            }
            UserWrappers.Clear();
            foreach (var user in users2)
            {
                UserWrappers.Add(UserWrapper.Convert(user, roles));
            }
        }

        private void deleteUser(UserWrapper wrapper)
        {
            var caption = _localizationService.Translate(TranslateCommonId.WaringId);
            var meassage = _localizationService.Translate(TranslateCommonId.DeleteUserWarningId);
            var meassage1 = _localizationService.Translate(TranslateCommonId.DeleteUserFailId);
            var btnResult = MessageBox.Show(meassage, caption, MessageBoxButton.OKCancel);
            if (btnResult == MessageBoxResult.OK)
            {
                var roles = _securityService.GetAllRoles();
                if (_securityService.DeleteUser(UserWrapper.Convert(wrapper, roles)))
                {
                    UserWrappers.Remove(wrapper);
                }
                else
                {
                    MessageBox.Show(meassage1, caption);
                }
            }

        }

        private void editUser(UserWrapper user)
        {
            var dParam = new DialogParameters();
            dParam.Add("isBuild", false);
            dParam.Add("userInfo", user);

            _dialogService.ShowDialog("UserEditView", dParam, s =>
            {
                if (s.Result == ButtonResult.OK)
                {
                    searchUsers();
                }

            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
        public void Clear()
        {
            _localizationService.LanguageChanged -= onLanguageChanged;
            _ea.GetEvent<PubSubEvent<DialogClosedResult>>().Unsubscribe(onDialogClosed);

        }
    }
}
