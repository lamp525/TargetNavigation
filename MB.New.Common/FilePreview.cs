using System.IO;
using System.Linq;

namespace MB.New.Common
{
    public class FilePreview
    {
        #region 变量

        /// <summary>可预览的文件格式</summary>
        private static string[] previewableFileFormat = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx", ".bmp", ".jpg", ".tiff", ".gif", ".pcx", ".tga", ".raw", ".png" };

        /// <summary>可转换为预览文件的文件格式</summary>
        private static string[] convertibleFileFormat = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx" };

        /// <summary>常用图片文件格式</summary>
        private static string[] commonImageFileFormat = new string[] { ".bmp", ".jpg", ".tiff", ".gif", ".pcx", ".tga", ".raw", ".png" };

        #endregion 变量

        #region 取得预览文件URL

        /// <summary>
        /// 取得预览文件URL
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetPreviewFileURL(string saveName, string extension)
        {
            string resultURL = string.Empty;

            //取得预览文件夹路径
            string previewDirectoryPath = FileUtility.GetDirectoryPath(EnumDefine.FileFolderType.PreviewFile);

            //文件格式处理
            if (extension.IndexOf(".") != 0)
                extension = "." + extension;

            //判断文件是否为常用图片格式
            if (IsCommonImageFile(extension))
            {
                if (File.Exists(previewDirectoryPath + "/" + saveName + extension))
                    resultURL = "http://" + ConstVar.WebHostURL + "/" + ConstVar.PreviewPath + "/" + saveName + extension;
            }

            //判断文件类型是否可以转换的文件格式
            if (IsConvertibleFile(extension))
            {
                if (File.Exists(previewDirectoryPath + "/" + saveName + ".swf"))
                    resultURL = "http://" + ConstVar.WebHostURL + "/" + ConstVar.PreviewPath + "/" + saveName + ".swf";
            }

            return resultURL;
        }

        #endregion 取得预览文件URL

        #region 将文件转换成.swf文件以供预览

        /// <summary>
        /// 将文件转换成.swf文件以供预览
        /// </summary>
        /// <param name="type"></param>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns>True：处理成功 False：处理失败</returns>
        public static bool ConvertFile(string previewPath, EnumDefine.FileFolderType type, string saveName, string extension)
        {
            //string previewDirectoryPath = FileUtility.GetDirectoryPath(EnumDefine.FileType.PreviewFile);
            string previewDirectoryPath = previewPath;
            string sourceFilePath = FileUtility.GetFilePath(type, saveName);

            //判断源文件是否存在
            if (!File.Exists(sourceFilePath))
            {
                return false;
            }

            //文件格式处理
            if (extension.IndexOf(".") != 0)
                extension = "." + extension;

            //判断是否需要创建预览文件夹
            FileUtility.CreateFolderIfNeeded(previewDirectoryPath);

            //判断文件是否为常用图片格式
            if (IsCommonImageFile(extension))
            {
                string preivewFileFullPath = Path.Combine(previewDirectoryPath, saveName) + extension;
                if (!File.Exists(preivewFileFullPath))
                {
                    File.Copy(sourceFilePath, preivewFileFullPath);
                }
                return true;
            }

            //判断文件类型是否可以转换的文件格式
            if (IsConvertibleFile(extension))
            {
                string inputFullPath = Path.Combine(previewDirectoryPath, saveName) + extension;
                string preivewFileFullPath = Path.Combine(previewDirectoryPath, saveName) + ".swf";

                //判断预览文件是否已经存在
                if (!File.Exists(preivewFileFullPath))
                {
                    //判断用于转换的文件是否存在
                    if (!File.Exists(inputFullPath))
                    {
                        File.Copy(sourceFilePath, inputFullPath);
                    }

                    //将文件转化成Flash文件
                    PrintToFlash(inputFullPath, preivewFileFullPath);
                }

                return true;
            }

            return false;
        }

        #endregion 将文件转换成.swf文件以供预览

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

        #endregion 检查文件是否可以预览

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

        #endregion 检查文件类型是否需要转换以供预览

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

        #endregion 根据后缀名判断文件是否为常用图片格式

        #region 私用方法

        #region Print2Flash3 文件转化处理

        /// <summary>
        ///  Print2Flash3 文件转化处理
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <returns></returns>
        private static bool PrintToFlash(string inputFilePath, string outputFilePath)
        {
            //await Task.Factory.StartNew(() =>
            //    {
            Print2Flash3.Server2 p2fServer = new Print2Flash3.Server2();
            p2fServer.DefaultProfile.InterfaceOptions = 0;
            p2fServer.DefaultProfile.ProtectionOptions = (int)Print2Flash3.PROTECTION_OPTION.PROTENAPI;
            p2fServer.ConvertFile(inputFilePath, outputFilePath);
            //删除临时文件
            File.Delete(inputFilePath);
            //});
            return true;
        }

        #endregion Print2Flash3 文件转化处理

        #endregion 私用方法
    }
}