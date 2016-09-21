using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MB.Common
{
    public class FilePreview
    {
        #region 变量
        /// <summary>可预览的文件格式</summary>
        private static string[] previewableFileFormat = new string[] { ".doc", ".docx", "xls", ".xlsx", ".txt", ".bmp", ".jpg", ".tiff", ".gif", ".pcx", ".tga", ".raw" };

        /// <summary>可转换为预览文件的文件格式</summary>
        private static string[] convertibleFileFormat = new string[] { ".doc", ".docx", "xls", ".xlsx", ".txt" };

        /// <summary>常用图片文件格式</summary>
        private static string[] commonImageFileFormat = new string[] { ".bmp", ".jpg", ".tiff", ".gif", ".pcx", ".tga", ".raw" };
        #endregion

        #region 将文件转换成.swf文件以供预览
        /// <summary>
        /// 将文件转换成.swf文件以供预览
        /// </summary>
        /// <param name="type"></param>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string ConvertFile(int type, string saveName, string extension)
        {
            FileUpload fileUpload = new FileUpload();
            string previewFileUrl = string.Empty;
            string　sourceFilePath = Path.Combine(fileUpload.ConfigPath(type),saveName);

            //判断源文件是否存在
            if(!File.Exists(sourceFilePath))
            {
                return string.Empty;
            }

            //判断文件是否为常用图片格式
            if (IsCommonImageFile(extension))
            {
                return sourceFilePath;
            }

            //检查文件类型是否可以转换
            if (IsConvertibleFile(extension))
            {
                string inputFullPath = Path.Combine(fileUpload.ConfigPath(10), saveName) + extension;
                string outputFullPath = Path.Combine(fileUpload.ConfigPath(10), saveName) + ".swf";
                previewFileUrl = "/" + FilePath.PreviewPath + "/" + saveName + ".swf";

                //判断预览文件是否已经存在
                if (File.Exists(outputFullPath))
                {
                    return previewFileUrl;
                }

                //判断预览文件夹是否存在
                if (!Directory.Exists(fileUpload.ConfigPath(10)))
                {
                    Directory.CreateDirectory(fileUpload.ConfigPath(10));
                }

                //判断用于转换的文件是否存在
                if (!File.Exists(inputFullPath))
                {
                    File.Copy(sourceFilePath, inputFullPath);
                }

                Print2Flash3.Server2 p2fServer = new Print2Flash3.Server2();
                p2fServer.DefaultProfile.InterfaceOptions = 0;
                p2fServer.DefaultProfile.ProtectionOptions = (int)Print2Flash3.PROTECTION_OPTION.PROTENAPI;
                p2fServer.ConvertFile(inputFullPath, outputFullPath);

                File.Delete(inputFullPath);
            }

            return previewFileUrl;
        }
        #endregion

        #region 检查文件是否可以预览
        /// <summary>
        /// 检查文件是否可以预览
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsPreviewable(string extension)
        {
            return previewableFileFormat.Contains(extension);
        }
        #endregion

        #region 检查文件类型是否需要转换以供预览
        /// <summary>
        /// 检查文件类型是否需要转换以供预览
        /// </summary>
        /// <param name="extension">文件后缀名</param>
        /// <returns>true：可以转换 false：不可转换</returns>
        public static bool IsConvertibleFile(string extension)
        {
            return convertibleFileFormat.Contains(extension);
        }
        #endregion

        #region 根据后缀名判断文件是否为常用图片格式
        /// <summary>
        /// 根据后缀名判断文件是否为常用图片格式
        /// </summary>
        /// <param name="extension">文件后缀名</param>
        /// <returns></returns>
        public static bool IsCommonImageFile(string extension)
        {
            return commonImageFileFormat.Contains(extension);
        }
        #endregion
    }
}
