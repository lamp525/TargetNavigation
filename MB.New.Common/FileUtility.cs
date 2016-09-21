using System;
using System.IO;
using System.Web;

namespace MB.New.Common
{
    public class FileUtility
    {
        #region 读取web.config中的上传地址

        /// <summary>
        /// 读取web.config中的上传地址
        /// </summary>
        /// <param name="folderType"></param>
        /// <returns></returns>
        public static string GetDirectoryPath(EnumDefine.FileFolderType folderType)
        {
            var directoryPath = string.Empty;
            switch (folderType)
            {
                case EnumDefine.FileFolderType.NewsImage:
                    directoryPath = System.Web.HttpContext.Current.Server.MapPath("~") + ConstVar.NewsUpLoadPath;
                    break;

                case EnumDefine.FileFolderType.CompanyDocument:
                    directoryPath = ConstVar.DocumentUpLoadPath;
                    break;

                case EnumDefine.FileFolderType.PlanAttachment:
                    directoryPath = ConstVar.PlanUpLoadPath;
                    break;

                case EnumDefine.FileFolderType.UserDocument:
                    directoryPath = ConstVar.MineUpLoadPath;
                    break;

                case EnumDefine.FileFolderType.HeadImage:
                    directoryPath = System.Web.HttpContext.Current.Server.MapPath("~") + ConstVar.HeadImageUpLoadPath;
                    break;

                case EnumDefine.FileFolderType.IMFile:
                    directoryPath = System.Web.HttpContext.Current.Server.MapPath("~") + ConstVar.IMUploadPath;
                    break;

                case EnumDefine.FileFolderType.ObjectiveFile:
                    directoryPath = ConstVar.ObjectiveUploadPath;
                    break;

                case EnumDefine.FileFolderType.FlowFile:
                    directoryPath = ConstVar.FlowIndexUploadPath;
                    break;

                case EnumDefine.FileFolderType.MeetingFile:
                    directoryPath = ConstVar.MeetingUploadPath;
                    break;

                case EnumDefine.FileFolderType.PreviewFile:
                    directoryPath = System.Web.HttpContext.Current.Server.MapPath("~") + ConstVar.PreviewPath;
                    break;

                default: break;
            }
            return directoryPath;
        }

        #endregion 读取web.config中的上传地址

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="saveName"></param>
        /// <returns>True：删除成功 False：删除失败</returns>
        public static bool Delete(EnumDefine.FileFolderType folderType, string saveName)
        {
            if (string.IsNullOrEmpty(saveName)) return false;

            string filePath = GetFilePath(folderType, saveName);

            if (!File.Exists(filePath)) return false;

            File.Delete(filePath);

            return true;
        }

        #endregion 删除文件

        #region 上传文件

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>文件保存名</returns>
        public static string Upload(EnumDefine.FileFolderType folderType, HttpPostedFileBase file, int? userId = null)
        {
            string extension = GetFileExtension(file.FileName);
            string saveName = string.Empty;
            if (folderType == EnumDefine.FileFolderType.HeadImage)
            {
                saveName = ImageHelper.GetUserImageName(userId.Value, extension, true);
            }
            else
            {
                saveName = GetFileSaveName();
            }

            string pathForSaving = GetDirectoryPath(folderType);

            if (CreateFolderIfNeeded(pathForSaving))
            {
                Delete(folderType, saveName);

                file.SaveAs(Path.Combine(pathForSaving, saveName));
            }

            return saveName;
        }

        #endregion 上传文件

        #region 取得文件的保存名

        /// <summary>
        /// 取得文件的保存名
        /// </summary>
        /// <returns></returns>
        public static string GetFileSaveName()
        {
            string saveName = string.Empty;

            Random rd = new Random();
            int numName = rd.Next(1000, 9999);
            saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();

            return saveName;
        }

        #endregion 取得文件的保存名

        #region 取得文件名中的后缀名

        /// <summary>
        /// 取得文件名中的后缀名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileExtension(string fileName)
        {
            string extension = string.Empty;

            if (fileName.LastIndexOf(".") > 0) extension = fileName.Substring(fileName.LastIndexOf('.'));

            return extension;
        }

        #endregion 取得文件名中的后缀名

        #region 取得文件地址

        /// <summary>
        /// 取得文件地址
        /// </summary>
        /// <param name="folderType"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static string GetFilePath(EnumDefine.FileFolderType folderType, string saveName)
        {
            string filePath = string.Empty;

            if (!string.IsNullOrEmpty(saveName))
            {
                var directoryPath = GetDirectoryPath(folderType);
                filePath = Path.Combine(directoryPath, saveName);
            }

            return filePath;
        }

        #endregion 取得文件地址

        #region 取得文件大小

        /// <summary>
        /// 取得文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>当前文件大小（字节数）</returns>
        public static long GetFileSize(string filePath)
        {
            if (!File.Exists(filePath)) return 0;
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        #endregion 取得文件大小

        #region 检查是否要创建文件夹，如果没有就创建

        /// <summary>
        /// 检查是否要创建文件夹，如果没有就创建
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool CreateFolderIfNeeded(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return true;
        }

        #endregion 检查是否要创建文件夹，如果没有就创建
    }
}