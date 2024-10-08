using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Helper
{
    public static class DialogServiceWithRegionExtension
    {
        public static void Show(this IDialogService dialogService, string name)
        {
            dialogService.Show(name, new DialogParameters(), null);
        }
        public static void Show(this IDialogService dialogService, string name, Action<IDialogResult> callback)
        {
            dialogService.Show(name, new DialogParameters(), callback);
        }
        public static void ShowDialog(this IDialogService dialogService, string name) =>
           dialogService.ShowDialog(name, null, null);
        public static void ShowDialog(this IDialogService dialogService, string name, Action<IDialogResult> callback) =>
          dialogService.ShowDialog(name, null, callback);
        public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters) =>
          dialogService.ShowDialog(name, parameters, null);
    }
}
