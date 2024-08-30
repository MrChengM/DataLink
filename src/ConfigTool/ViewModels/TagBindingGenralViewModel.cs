using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServer.Config;
using Prism.Events;
using ConfigTool.Models;
using Prism.Services.Dialogs;
using System.Windows.Input;
using Prism.Commands;
using ConfigTool.Service;
using System.ComponentModel;
using Utillity.Data;

namespace ConfigTool.ViewModels
{
    public class TagBindingGenralViewModel : BindableBase, INavigationAware,IDataErrorInfo
    {
        private bool isFristIn = true;

        private bool buildMode;
        private TagBindingConfig _config;
        private IEventAggregator _ea;
        private IDialogService _dialogService;
        private IConfigDataServer _configDataServer;
        private ServerItemConfig _serverItemConfig;
        public bool BuildMode
        {
            get { return buildMode; }
            set { SetProperty(ref buildMode, value, "BuildMode"); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }

        private string sourceTag;

        public string SourceTag
        {
            get { return sourceTag; }
            set { SetProperty(ref sourceTag, value, "SourceTag"); }
        }


        private ICommand openTagsDailogCommand;

        public ICommand OpenTagsDailogCommand
        {
            get { return openTagsDailogCommand; }
            set { openTagsDailogCommand = value; }
        }
        #region IDataErrorInfo
        public string Error => null;
        private string[] errorMsgBuffer = new string[7];

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == "Name")
                {

                    if (string.IsNullOrEmpty(Name))
                    {
                        result = "Name can not null or empty !";
                    }
                    else if (!RegexCheck.IsString(Name.ToString()))
                    {
                        result = " Name include special character !";
                    }
                    else if ( BuildMode && _serverItemConfig.TagBindingList.ContainsKey(Name))
                    {
                        result = "Name Name is Exit! ";
                    }
                    errorMsgBuffer[0] = result;
                }
                else if (columnName == "SourceTag")
                {
                    if (string.IsNullOrEmpty(SourceTag))
                    {
                        result = "SourceTag can not null or empty !";
                    }

                    errorMsgBuffer[1] = result;
                }
               
                judgeHasError();
                return result;
            }

        }
        void judgeHasError()
        {
            bool hasError = false;
            foreach (var errorMsg in errorMsgBuffer)
            {
                if (errorMsg != string.Empty && errorMsg != null)
                {
                    hasError = true;
                    break;
                }
            }
            _ea.GetEvent<PubSubEvent<bool>>().Publish(hasError);

        }
        #endregion

        public TagBindingGenralViewModel(IEventAggregator ea,IDialogService dialogService,IConfigDataServer configDataServer)
        {
            _ea = ea;
            _dialogService = dialogService;
            _configDataServer = configDataServer;
            _ea.GetEvent<ButtonConfrimEvent>().Subscribe(setConfig);
            openTagsDailogCommand = new DelegateCommand(openTagsDailog);
        }

        private void openTagsDailog()
        {
            
           var dialogPara = new DialogParameters();
            var allTags = _configDataServer.GetAllTags();
            dialogPara.Add("AllTags", allTags);
            dialogPara.Add("SourceTag", sourceTag);
            _dialogService.ShowDialog("SearchTagDialog", dialogPara, r => 
            {
                if (r.Result == ButtonResult.OK)
                {
                    var tagItem = r.Parameters.GetValue<List<TagListItem>>("SelectTags");
                    if (tagItem!=null)
                    {
                        SourceTag = tagItem[0].Name;
                    }
                }
            }
            );

        }

        private void setConfig(ButtonResult button)
        {
            if (button == ButtonResult.OK)
            {
                if (buildMode)
                {
                    _config.DestTagName = Name;
                }
                _config.SourceTagName = SourceTag;
            }
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
            if (isFristIn)
            {
                _serverItemConfig = navigationContext.Parameters.GetValue<ServerItemConfig>("ServerItemConfig");
                _config = navigationContext.Parameters.GetValue<TagBindingConfig>("TagBindingConfig");
                BuildMode = navigationContext.Parameters.GetValue<bool>("isBuild");
                if (!BuildMode)
                {
                    Name = _config.DestTagName;
                    SourceTag = _config.SourceTagName;
                }
                isFristIn = false;
            }
        }
    }
}
