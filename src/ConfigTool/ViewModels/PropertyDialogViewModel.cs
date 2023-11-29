using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataServer.Config;
using ConfigTool.Models;
using Prism.Regions;
using System.Windows.Input;
using Prism.Commands;
using Prism.Ioc;
using Prism.Events;
using DataServer;
using ConfigTool.Service;

namespace ConfigTool.ViewModels
{


    public class PropertyDialogViewModel : BindableBase, IDialogAware
    {

        private readonly IEventAggregator _ea;
        private IPropertyViewDespatcher _propertyViewDespatcher;
        #region Property
        private string _title = "Property";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }

        private ObservableCollection<PropertyOptionItem> optionItems;
        /// <summary>
        /// Property选项卡名称集合
        /// </summary>
        public ObservableCollection<PropertyOptionItem> OptionItems
        {
            get { return optionItems; }
            set { SetProperty(ref optionItems, value, "OptionItems"); }
        }
        //}
        //private ICommand optionSelectCommand;

        //public ICommand OptionSelectCommand
        //{
        //    get { return optionSelectCommand; }
        //    set { optionSelectCommand = value; }
        //}

        private ICommand closeDialogCommand;

        public event Action<IDialogResult> RequestClose;

        public ICommand CloseDialogCommand
        {
            get { return closeDialogCommand; }
            set { closeDialogCommand = value; }
        }

        #endregion

        public PropertyDialogViewModel(IEventAggregator eventAggregator)
        {
            _ea = eventAggregator;
            closeDialogCommand = new DelegateCommand<string>(closeDialog);
        }
        #region ICommand
        void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                _ea.GetEvent<ButtonConfrimEvent> ().Publish(result);
            }
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            RequestClose(new DialogResult(result, param));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _ea.GetEvent<ButtonConfrimEvent>().Clear();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _propertyViewDespatcher = parameters.GetValue<IPropertyViewDespatcher>("ViewDespatcher");
            OptionItems = _propertyViewDespatcher.OptionItems;

            ///初始化属性界面
            if (OptionItems.Count>0)
            {
                OptionItems[0].OptionSelectCommand.Execute(OptionItems[0].Url);
            }
            Title = _propertyViewDespatcher.Title;
        }
        #endregion
    }
}
