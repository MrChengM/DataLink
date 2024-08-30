using System;
using System.Collections.Generic;
using System.Text;
using GuiBase.ViewModels;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Commands;
using GuiBase.Models;
using MaterialDesignThemes.Wpf;

namespace GuiBase.ViewModels
{
    public class HeaderViewModel:BindableBase
    {
        private IDialogService _dialogService;
        public List<NavigationItem> DialogList { get; set; }

        private NavigationItem selectedItem;

        public DelegateCommand DialogClickCommand { get; set; }
        public NavigationItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem"); }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value, "SelectedIndex"); }
        }

        public HeaderViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            initNavigationItems();
            DialogClickCommand = new DelegateCommand(DialogClick);

        }

        private void DialogClick()
        {
            if (SelectedItem!=null)
            {
                if (SelectedItem.Title == "Eixt")
                {
                    App.Current.Shutdown();

                }
                else if (SelectedItem.Title == "LogOn")
                {
                    _dialogService.ShowDialog(selectedItem.ViewName, s => SelectedItem = null);

                }
                else
                {
                    _dialogService.Show(selectedItem.ViewName, s => SelectedItem = null);
                }

            }
        }

        void initNavigationItems()
        {
            DialogList = new List<NavigationItem>();
            DialogList.Add(new NavigationItem()
            {
                Title = "LogOn",
                SelectedIcon = PackIconKind.AccountCircle,
                UnselectedIcon = PackIconKind.AccountCircleOutline,
                ViewName = "LogOnView",

            });
            DialogList.Add(new NavigationItem()
            {
                Title = "Alarm",
                SelectedIcon = PackIconKind.AlarmLight,
                UnselectedIcon = PackIconKind.AlarmLightOutline,
                ViewName = "AlarmView",

            });
            DialogList.Add(new NavigationItem()
            {
                Title = "HistoryAlarm",
                SelectedIcon = PackIconKind.AlarmPanel,
                UnselectedIcon = PackIconKind.AlarmPanelOutline,
                ViewName = "HistoryAlarmView",

            });
            DialogList.Add(new NavigationItem()
            {
                Title = "OperRecord",
                SelectedIcon = PackIconKind.BookOpen,
                UnselectedIcon = PackIconKind.BookOpenOutline,
                ViewName = "OperRecordView",

            });
            //NavigationList.Add(new NavigationItem()
            //{
            //    Title = "Legend",
            //    SelectedIcon = PackIconKind.Motion,
            //    UnselectedIcon = PackIconKind.MotionOutline,
            //    NavigtionViewName = "",

            //});
            DialogList.Add(new NavigationItem()
            {
                Title = "Eixt",
                SelectedIcon = PackIconKind.Power,
                UnselectedIcon = PackIconKind.Power,
                ViewName = "",

            });
           
        }
    }
}
