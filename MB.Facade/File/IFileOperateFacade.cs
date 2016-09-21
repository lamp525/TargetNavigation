using MB.New.Common;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace MB.Facade.File
{
    public interface IFileOperateFacade
    {
        /// <summary>
        /// 取得文件信息
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">文件所属对象ID</param>
        /// <returns></returns>
        List<PageFileInfoModel> GetFileInfo(EnumDefine.FileType type, int targetId);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">计划、循环计划、目标、流程或会议室ID</param>
        /// <param name="fileInfo">文件信息</param>
        /// <returns>上传的文件信息</returns>
        List<PageFileInfoModel> UploadFile(int userId, EnumDefine.FileType type, int targetId, HttpFileCollectionBase fileInfo);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="fileId">附件ID或文档ID</param>
        /// <returns>True：删除成功 False：删除失败</returns>
        bool DeleteFile(EnumDefine.FileType type, int fileId);

        /// <summary>
        /// 下载单个文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="fileId">附件ID或文档ID</param>
        /// <returns>文件信息</returns>
        PageFileInfoModel SingleDownload(EnumDefine.FileType type, int fileId);

        /// <summary>
        /// 下载多个文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">计划、循环计划、目标、流程或会议室ID</param>
        /// <returns>文件信息</returns>
        MemoryStream MultiDownload(EnumDefine.FileType type, int targetId);

        /// <summary>
        /// 文件预览
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        string Preview(string saveName, string extension);
    }
}