﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Utillity
{
   public static class FileOperate
    {
        /// <summary>
        /// 输入文件
        /// </summary>
        /// <param name="sfileName"></param>
        /// <param name="sFilter">"|*.xx"</param>
        /// <returns></returns>
        public static bool InputFile(ref string sfileName, string sFilter)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = sfileName;
            dialog.Filter = sFilter;

            if (dialog.ShowDialog() == true)
            {
                // sPath = dialog.FileName;
                sfileName = dialog.FileName;
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="sfileName"></param>
        /// <param name="sFilter"></param>
        /// <returns></returns>
        public static bool OutputFile(ref string sfileName, string sFilter)
        {

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = sfileName;
            dialog.Filter = sFilter;

            // Show save file dialog box
            Nullable<bool> result = dialog.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                sfileName = dialog.FileName;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}