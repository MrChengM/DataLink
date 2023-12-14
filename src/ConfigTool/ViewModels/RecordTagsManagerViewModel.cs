using ConfigTool.Service;
using DataServer.Config;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ConfigTool.Models;

namespace ConfigTool.ViewModels
{
    class RecordTagsManagerViewModel : BindableBase, IDialogAware
    {
        //private IEventAggregator _ea;
        private string _recordItemName;
        private IConfigDataServer _configDataServer;
        private IDialogService _dialogService;


        private ObservableCollection<string> recordTags;

        public ObservableCollection<string> RecordTags
        {
            get { return recordTags; }
            set { SetProperty(ref recordTags, value, "RecordTags"); }
        }

        private ICommand deleteTagCommand;

        public ICommand DeleteTagCommand
        {
            get { return deleteTagCommand; }
            set { SetProperty(ref deleteTagCommand, value, "DeleteTagCommand"); }
        }

        private string selectTag;

        public string SelectTag
        {
            get { return selectTag; }
            set { SetProperty(ref selectTag, value, "SelectTag"); }
        }


        private ICommand closeDialogCommand;

        public ICommand CloseDialogCommand
        {
            get { return closeDialogCommand; }
            set { SetProperty(ref closeDialogCommand, value, "CloseDialogCommand"); }
        }

        private string tagName;

        public string TagName
        {
            get { return tagName; }
            set { SetProperty(ref tagName, value, "FilterData"); }
        }

        private ICommand openTagsDailogCommand;

        public ICommand OpenTagsDailogCommand
        {
            get { return openTagsDailogCommand; }
            set { openTagsDailogCommand = value; }
        }
        public string Title => "Record Tags Manager";

        public event Action<IDialogResult> RequestClose;


        public RecordTagsManagerViewModel(IConfigDataServer configDataServer,IDialogService dialogService)
        {
            _configDataServer = configDataServer;
            _dialogService = dialogService;
            deleteTagCommand = new DelegateCommand<string>(delete);
            closeDialogCommand = new DelegateCommand<string>(closeDialog);
            openTagsDailogCommand = new DelegateCommand(openTagsDailog);
        }


        private void openTagsDailog()
        {

            var dialogPara = new DialogParameters();
            var allTags = _configDataServer.GetAllTags();
            dialogPara.Add("AllTags", allTags);
            _dialogService.ShowDialog("SearchTagDialog", dialogPara, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var tags = r.Parameters.GetValue<List<TagListItem>>("SelectTags");
                    foreach (var tag in tags)
                    {
                        if (!RecordTags.Contains(tag.Name))
                            RecordTags.Add(tag.Name);
                    }
                }
            }
            );
        }

        private void delete(string tagName)
        {
            RecordTags.Remove(tagName);
        }
        private void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                result = ButtonResult.OK;
                _configDataServer.ReplaceRecordTags(_recordItemName, RecordTags.ToList());
            }
            else if (parameter?.ToLower() == "cancel")
            {
                result = ButtonResult.Cancel;
            }

            RequestClose(new DialogResult(result, param));
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
            _recordItemName = parameters.GetValue<string>("RecordItem");
            var tagList  = parameters.GetValue<List<string>>("TagNames");
            if (tagList != null)
            {
                RecordTags = new ObservableCollection<string>(tagList);
            }
            else
            {
                RecordTags = new ObservableCollection<string>();
            }
        }
    }
}
