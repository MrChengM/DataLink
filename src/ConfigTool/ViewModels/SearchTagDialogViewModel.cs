using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
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

        private TagListItem selectTag;

        public TagListItem SelectTag
        {
            get { return selectTag; }
            set { SetProperty(ref selectTag, value, "SelectTag"); }
        }

        private string filterData;

        public string FilterData
        {
            get { return filterData; }
            set { SetProperty(ref filterData, value, "FilterData"); }
        }

        #endregion
        #region Command

        private ICommand closeDialogCommand;
        public ICommand CloseDialogCommand
        {
            get { return closeDialogCommand; }
            set { closeDialogCommand = value; }
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
            closeDialogCommand = new DelegateCommand<string>(closeDialog);
            searchCommand = new DelegateCommand(search);

        }

        private void search()
        {
            if (filterData == null||filterData=="")
            {
                FilterTags=new ObservableCollection<TagListItem>(allTags);
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

        private void closeDialog(string parameter)
        {
            var result = new ButtonResult();
            var param = new DialogParameters();
            if (parameter?.ToLower() == "ok")
            {
                if (selectTag==null)
                {
                    MessageBox.Show("Tag未选择！");
                    return;
                    //result = ButtonResult.Cancel;

                }
                else
                {
                    result = ButtonResult.OK;
                    param.Add("SelectTag", selectTag);
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
