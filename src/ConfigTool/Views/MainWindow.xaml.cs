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
using Prism.Regions;

namespace ConfigTool.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion<MenuView>("MenuRegion");
            regionManager.RegisterViewWithRegion<TreeView>("TreeViewRegion");
            regionManager.RegisterViewWithRegion<DetailedView>("DetailedListRegion");
            regionManager.RegisterViewWithRegion<LogDispalyView>("LogDispalyRegion");


        }
    }
}
