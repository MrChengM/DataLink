using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using Prism.Services.Dialogs;
using GuiBase.Services;
using System.Collections.ObjectModel;
using GuiBase.Models;
using DataServer.Permission;
using System.Windows;

namespace GuiBase.ViewModels
{
    public class ResourceNameListViewModel : BindableBase,IDialogAware
    {
        public ISecurityService _securityService;

        public ObservableCollection<string> ResourceNames { get; set; }

        public DelegateCommand<string> ConfrimBtnCommand { get; set; }

        private string selectName;

        public string SelectName
        {
            get { return selectName; }
            set { SetProperty(ref selectName, value, "SelectName"); }
        }

        public string Title => "Resource Name List";

        public event Action<IDialogResult> RequestClose;

        public ResourceNameListViewModel(ISecurityService securityService)
        {
            _securityService = securityService;
            ConfrimBtnCommand = new DelegateCommand<string>(confrimBtn);
            initList();
        }

        void initList()
        {
            ResourceNames = new ObservableCollection<string>();
            var allResourceNames = _securityService.GetResourceNames();
            var resources = _securityService.GetAllResources();
            if (resources == null)
            {
                ResourceNames.AddRange(allResourceNames);
                return;
            }
            foreach (var name in allResourceNames)
            {
                if (resources.Find(s => s.Name == name) == null)
                {
                    ResourceNames.Add(name);
                }
            }
        }


        private void confrimBtn(string param)
        {
            var dialogParam = new DialogParameters();
            var btnResult = new ButtonResult();
            if (param == "OK")
            {
                if (SelectName != null)
                {
                    dialogParam.Add("ResourceName", SelectName);
                    btnResult = ButtonResult.OK;
                }
                else
                {
                    MessageBox.Show("请选择资源名称！");
                }
            }
            else if (param == "Cancel")
            {
                btnResult = ButtonResult.Cancel;
            }
            RequestClose?.Invoke(new DialogResult(btnResult, dialogParam));
        }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
