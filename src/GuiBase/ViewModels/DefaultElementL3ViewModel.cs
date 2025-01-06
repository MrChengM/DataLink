using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Mvvm;
using Prism.Regions;
using GuiBase.Common;
using GuiBase.Services;

namespace GuiBase.ViewModels
{
    public class DefaultElementL3ViewModel : BindableBase, INavigationAware
    {
        private string elementName;

        public string ElementName
        {
            get { return elementName; }
            set { SetProperty(ref elementName, value, "ElementName"); }
        }

        private List<IGCommand> bitCommands;

        public List<IGCommand> BitCommands
        {
            get { return bitCommands; }
            set { SetProperty(ref bitCommands, value, "BitCommands"); }
        }
        private GSignal signal1;

        public GSignal Signal1
        {
            get { return signal1; }
            set { SetProperty(ref signal1, value, "Signal1"); }
        }
        private GSignal signal2;

        public GSignal Signal2
        {
            get { return signal2; }
            set { SetProperty(ref signal2, value, "Signal2"); }
        }
        private GSignal signal3;

        public GSignal Signal3
        {
            get { return signal3; }
            set { SetProperty(ref signal3, value, "Signal3"); }
        }
        private GSignal signal4;

        public GSignal Signal4
        {
            get { return signal4; }
            set { SetProperty(ref signal4, value, "Signal4"); }
        }
        private GSignal signal5;
        public GSignal Signal5
        {
            get { return signal5; }
            set { SetProperty(ref signal5, value, "Signal5"); }
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
            var param = navigationContext.Parameters;
            BitCommands = param.GetValue<List<IGCommand>>("Commands");
            ElementName = param.GetValue<string>("ElementName");
            var gsset = param.GetValue<GSignalSet>("Signals");
            Signal1 = gsset[0];
            Signal2 = gsset[1];
            Signal3 = gsset[2];
            Signal4 = gsset[3];
            Signal5 = gsset[4];
        }
    }
}
