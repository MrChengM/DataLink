﻿using ConfigTool.ViewModels;
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
    /// RecordTagsListView.xaml 的交互逻辑
    /// </summary>
    public partial class RecordTagsListView : UserControl
    {
        public RecordTagsListView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as RecordTagsListViewModel;
            var contorl = sender as ListViewItem;
            var infor = contorl.DataContext as string;
            vm.MouseDoubleClickCommand?.Execute(infor);
        }
    }
}
