using Prism.Regions;
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
using System.Windows.Shapes;

namespace GuiBase.Views
{
    /// <summary>
    /// StartUpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartUpWindow : Window
    {
        //IRegionManager _regionManager;
        public StartUpWindow(IRegionManager regionManager, string viewName)
        {
            InitializeComponent();

            //if (viewName=="LoadView")
            //{
                regionManager.RegisterViewWithRegion("LoadRegion", viewName);

            //}
            //else
            //{
            //    regionManager.RequestNavigate("LoadRegion", viewName);

            //}

        }
    }
}
