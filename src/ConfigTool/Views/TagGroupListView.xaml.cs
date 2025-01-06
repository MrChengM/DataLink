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
using ConfigTool.ViewModels;
using ConfigTool.Models;

namespace ConfigTool.Views
{
    /// <summary>
    /// TagGroupListView.xaml 的交互逻辑
    /// </summary>
    public partial class TagGroupListView : UserControl
    {
        public TagGroupListView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as TagGroupListViewModel;
            var contorl = sender as ListViewItem;
            var infor = contorl.DataContext as TagGroupListItem;

            vm.MouseDoubleClickCommand?.Execute(infor);
        }
    }
}
