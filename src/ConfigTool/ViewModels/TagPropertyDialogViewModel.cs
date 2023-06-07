using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigTool.ViewModels
{
   public class TagPropertyDialogViewModel:PropertyDialogViewModel, IDialogAware
    {
        
        public TagPropertyDialogViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager, eventAggregator)
        {

        }
        #region IDialogAware
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
        }
        #endregion
    }
}
