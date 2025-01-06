using ConfigTool.Models;
using ConfigTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConfigTool.Views
{
    /// <summary>
    /// TagBindingListView.xaml 的交互逻辑
    /// </summary>
    public partial class TagBindingListView : UserControl
    {
        public TagBindingListView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as TagBindingListViewModel;
            var contorl = sender as ListViewItem;
            var infor = contorl.DataContext as TagBindingListItem;
            vm.MouseDoubleClickCommand?.Execute(infor);
        }

        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var vm = DataContext as TagBindingListViewModel;
                var contorl = sender as ListViewItem;
                var infor = contorl.DataContext as TagBindingListItem;
                vm.DeleteItemCommand?.Execute(infor);
            }
        }
    }
}
