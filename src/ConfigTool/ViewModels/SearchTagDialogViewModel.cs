using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigTool.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using System.Windows;

namespace ConfigTool.ViewModels
{
    public class SearchTagDialogViewModel : BindableBase, IDialogAware
    {

        private List<TagListItem> allTags;
        #region Property
        private string _title = "SearchTag";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }
        private ObservableCollection<TagListItem> filterTags;

        public ObservableCollection<TagListItem> FilterTags
        {
            get { return filterTags; }
            set { SetProperty(ref filterTags, value, "FilterTags"); }
        }

        //private TagListItem selectTag;

        //public TagListItem SelectTag
        //{
        //    get { return selectTag; }
        //    set { SetProperty(ref selectTag, value, "SelectTag"); }
        //}

        //private ObservableCollection<TagListItem> selectTags;

        //public ObservableCollection<TagListItem> SelectTags
        //{
        //    get { return selectTags; }
        //    set { SetProperty(ref selectTags, value, "SelectTags"); }
        //}



        private string filterData;

        public string FilterData
        {
            get { return filterData; }
            set { SetProperty(ref filterData, value, "FilterData"); }
        }

        #endregion
        #region Command

        private ICommand confirmCommand;
        public ICommand ConfirmCommand
        {
            get { return confirmCommand; }
            set { confirmCommand = value; }
        }

        private ICommand cancelCommand;

        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            set { SetProperty(ref cancelCommand, value, "CancelCommand"); }
        }

        private ICommand searchCommand;

        public ICommand SearchCommand
        {
            get { return searchCommand; }
            set { searchCommand = value; }
        }

        #endregion

        public SearchTagDialogViewModel()
        {
            confirmCommand = new DelegateCommand<IList>(confrim);
            cancelCommand = new DelegateCommand(cancel);
            searchCommand = new DelegateCommand(search);

        }

        private void cancel()
        {
            closeDialog("Cancel", null);
        }

        private void confrim(IList param)
        {
            var tags = new List<TagListItem>();
            foreach (var p in param)
            {
                tags.Add((TagListItem)p);
            }
            closeDialog("OK", tags);
        }

        private void search()
        {
            if (filterData == null||filterData=="")
            {
                FilterTags = new ObservableCollection<TagListItem>(allTags);
            }
            else
            {
                var temp = allTags.FindAll( (s) =>
                {
                    if (s.Name.ToLower().Contains(filterData.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                );
                FilterTags = new ObservableCollection<TagListItem>(temp);
            };
        }

        private void closeDialog(string parameter, List<TagListItem> tags)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                if (tags == null||tags.Count==0)
                {
                    MessageBox.Show("Tag未选择！");
                    return;
                    //result = ButtonResult.Cancel;
                }
                else
                {
                    result = ButtonResult.OK;
                    param.Add("SelectTags", tags);
                }
            }
            else if (parameter?.ToLower() == "cancel")
                result = ButtonResult.Cancel;
            RequestClose(new DialogResult(result, param));
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            allTags = parameters.GetValue<List<TagListItem>>("AllTags");
            FilterData = parameters.GetValue<string>("SourceTag");
            search();
        }
    }
}
