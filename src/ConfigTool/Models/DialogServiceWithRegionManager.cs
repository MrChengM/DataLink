﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace ConfigTool.Models
{
    /// <summary>
    /// 弹窗服务提供RegionManager与DialogWindow绑定,
    /// 修复弹窗无法通过Region进行导航的问题
    /// </summary>
    public class DialogServiceWithRegionManager : DialogService, IDialogService
    {
        private readonly IContainerExtension _containerExtension;
        public DialogServiceWithRegionManager(IContainerExtension containerExtension)
            : base(containerExtension)
        {
            _containerExtension = containerExtension;
        }

        public new void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, true);
        }
        void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, bool isModal, string windowName = null)
        {
            if (parameters == null)
                parameters = new DialogParameters();

            IDialogWindow dialogWindow = CreateDialogWindow(windowName);

            //RegionManager设置操作
            var regionManager = _containerExtension.Resolve<IRegionManager>();
            if (dialogWindow is Window view)
            {
                RegionManager.SetRegionManager(view, regionManager);
                RegionManager.UpdateRegions();
            }

            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent(name, dialogWindow, parameters);

            ShowDialogWindow(dialogWindow, isModal);

        }
    }
}
