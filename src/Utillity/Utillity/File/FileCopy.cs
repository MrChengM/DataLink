using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.File
{
    public static class FileCopy
    {
        /// <summary>
        /// 单文件复制到指定文件夹
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        /// <param name="destDir">目标文件夹</param>
        /// <returns></returns>
        public static bool SingleFileCopy(string filePath,string destDir)
        {
            try
            {
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                if (System.IO.File.Exists(filePath))
                {
                    FileInfo info = new FileInfo(filePath);

                    string newFilePath = destDir + "\\" + info.Name;
                    System.IO.File.Copy(filePath, newFilePath, true);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// 源文件夹所有文件复制到指定文件夹
        /// </summary>
        /// <param name="srcDir">源文件夹</param>
        /// <param name="destDir">目标文件夹</param>
        public static void DirToDirFileCopy(string srcDir,string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            if (!Directory.Exists(srcDir))
            {
                return;
            }
            string[] filesPath = Directory.GetFiles(srcDir);
            foreach (var filePath in filesPath)
            {
                FileInfo info = new FileInfo(filePath);

                string newFilePath = destDir +"\\"+ info.Name;
                System.IO.File.Copy(filePath, newFilePath, true);
            }

            string[] subSrcDirs = Directory.GetDirectories(srcDir);
            foreach (var subSrcDir   in subSrcDirs)
            {
                var dirInfo = new DirectoryInfo(subSrcDir);
                var subDestDir = destDir+dirInfo.Name;
                DirToDirFileCopy(subSrcDir , subDestDir);
            }
        }

    }
}
