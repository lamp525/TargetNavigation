using MB.DAL;
using MB.New.BLL.Plan;
using MB.New.Common;
using MB.New.Model;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace MB.Facade.File
{
    public class FileOperateFacade : IFileOperateFacade
    {
        #region 取得文件信息

        /// <summary>
        /// 取得文件信息
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">文件所属对象ID</param>
        /// <returns></returns>
        public List<PageFileInfoModel> GetFileInfo(EnumDefine.FileType type, int targetId)
        {
            IPlanBLL Plan = new PlanBLL();
            var result = new List<PageFileInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = new List<FileInfoModel>();
                switch (type)
                {
                    //一般计划附件
                    case EnumDefine.FileType.Plan:
                        info = Plan.GetPlanAttachInfoByPlanId(db, targetId);

                        break;

                    //循环计划附件
                    case EnumDefine.FileType.LoopPlan:
                        info = Plan.GetLoopPlanAttachInfoBySubmitId(db, targetId);
                        break;

                        //TODO: 其他文件类型处理
                }
                result = ModelMapping.JsonMapping<List<FileInfoModel>, List<PageFileInfoModel>>(info);
            }

            return result;
        }

        #endregion 取得文件信息

        #region 上传

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">计划、循环计划、目标、流程或会议室ID</param>
        /// <param name="fileInfo">文件信息</param>
        /// <returns>上传的文件信息</returns>
        public List<PageFileInfoModel> UploadFile(int userId, EnumDefine.FileType type, int targetId, HttpFileCollectionBase fileInfo)
        {
            IPlanBLL Plan = new PlanBLL();
            List<PageFileInfoModel> result = new List<PageFileInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                foreach (string item in fileInfo)
                {
                    HttpPostedFileBase file = fileInfo[item] as HttpPostedFileBase;
                    if (file == null || file.ContentLength == 0)
                    {
                        continue;
                    }
                    //文件后缀名
                    string extension = FileUtility.GetFileExtension(file.FileName);
                    var fileModel = new FileInfoModel()
                    {
                        uploadUserId = userId,
                        targetId = targetId,
                        displayName = file.FileName,
                        extension = extension,
                        isPreviewable = FilePreview.IsPreviewable(extension)
                    };

                    switch (type)
                    {
                        //一般计划附件
                        case EnumDefine.FileType.Plan:
                            fileModel.saveName = FileUtility.Upload(EnumDefine.FileFolderType.PlanAttachment, file);
                            fileModel.fileId = Plan.InsPlanAttachInfo(db, fileModel);
                            break;

                        //循环计划附件
                        case EnumDefine.FileType.LoopPlan:
                            fileModel.saveName = FileUtility.Upload(EnumDefine.FileFolderType.PlanAttachment, file);
                            fileModel.fileId = Plan.InsLoopPlanAttachInfo(db, fileModel);
                            break;

                        //用户头像
                        case EnumDefine.FileType.HeadImage:
                            fileModel.saveName = FileUtility.Upload(EnumDefine.FileFolderType.HeadImage, file, userId);
                            fileModel.filePath = "http://" + ConstVar.WebHostURL + "/" + ConstVar.HeadImageUpLoadPath + "/" + fileModel.saveName;
                            break;

                            //TODO: 其他文件类型处理
                    }

                    var fileResultModel = ModelMapping.JsonMapping<FileInfoModel, PageFileInfoModel>(fileModel);

                    result.Add(fileResultModel);
                }

                db.SaveChanges();
            }

            return result;
        }

        #endregion 上传

        #region 删除

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="fileId">附件ID或文档ID</param>
        /// <returns>True：删除成功 False：删除失败</returns>
        public bool DeleteFile(EnumDefine.FileType type, int fileId)
        {
            IPlanBLL Plan = new PlanBLL();

            bool result = false;

            var fileInfo = new FileInfoModel();

            using (var db = new TargetNavigationDBEntities())
            {
                switch (type)
                {
                    //一般计划附件
                    case EnumDefine.FileType.Plan:

                        //取得附件信息
                        fileInfo = Plan.GetPlanAttachInfoByFileId(db, fileId);

                        //删除服务器上的附件
                        result = FileUtility.Delete(EnumDefine.FileFolderType.PlanAttachment, fileInfo.saveName);

                        //删除计划附件表信息
                        Plan.DelPlanAttachInfoByAttachId(db, fileId);

                        break;

                    //循环计划附件
                    case EnumDefine.FileType.LoopPlan:

                        //取得附件信息
                        fileInfo = Plan.GetLoopPlanAttachInfoByFileId(db, fileId);

                        //删除服务器上的附件
                        result = FileUtility.Delete(EnumDefine.FileFolderType.PlanAttachment, fileInfo.saveName);

                        //删除循环计划附件表信息
                        Plan.DelLoopPlanAttachInfoByAttachId(db, fileId);

                        break;

                        //TODO: 其他文件类型处理
                }
                db.SaveChanges();
            }

            return result;
        }

        #endregion 删除

        #region 下载

        /// <summary>
        /// 下载单个文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="fileId">附件ID或文档ID</param>
        /// <returns>文件路径</returns>
        public PageFileInfoModel SingleDownload(EnumDefine.FileType type, int fileId)
        {
            IPlanBLL Plan = new PlanBLL();
            var result = new PageFileInfoModel();

            var fileInfo = new FileInfoModel();
            using (var db = new TargetNavigationDBEntities())
            {
                switch (type)
                {
                    //一般计划附件
                    case EnumDefine.FileType.Plan:
                        fileInfo = Plan.GetPlanAttachInfoByFileId(db, fileId);
                        break;

                    //循环计划附件
                    case EnumDefine.FileType.LoopPlan:
                        fileInfo = Plan.GetLoopPlanAttachInfoByFileId(db, fileId);
                        break;

                        //TODO: 其他文件类型处理
                }
            }

            result = ModelMapping.JsonMapping<FileInfoModel, PageFileInfoModel>(fileInfo);

            return result;
        }

        /// <summary>
        /// 下载多个文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">计划、循环计划、目标、流程或会议室ID</param>
        /// <returns>文件路径</returns>
        public MemoryStream MultiDownload(EnumDefine.FileType type, int targetId)
        {
            IPlanBLL Plan = new PlanBLL();
            var fileInfoList = new List<FileInfoModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                switch (type)
                {
                    //一般计划附件
                    case EnumDefine.FileType.Plan:
                        fileInfoList = Plan.GetPlanAttachInfoByPlanId(db, targetId);
                        break;

                    //循环计划附件
                    case EnumDefine.FileType.LoopPlan:
                        fileInfoList = Plan.GetLoopPlanAttachInfoBySubmitId(db, targetId);
                        break;

                        //TODO: 其他文件类型处理
                }
            }

            //文件打包处理
            var compressFileList = new List<CompressInfo>();
            if (fileInfoList.Count > 0)
            {
                foreach (var item in fileInfoList)
                {
                    if (System.IO.File.Exists(item.filePath))
                    {
                        var compressFile = new CompressInfo
                        {
                            path = item.filePath,
                            display = item.displayName
                        };
                        compressFileList.Add(compressFile);
                    }
                }
            }

            MemoryStream result = new MemoryStream();
            if (compressFileList.Count > 0)
            {
                SharpZipLibrary sharpZip = new SharpZipLibrary();
                result = sharpZip.Compress(compressFileList);
            }

            return result;
        }

        #endregion 下载

        #region 预览

        /// <summary>
        /// 文件预览
        /// </summary>
        /// <param name="saveName">文件保存名</param>
        /// <param name="extension">后缀名</param>
        /// <returns>预览文件URL</returns>
        public string Preview(string saveName, string extension)
        {
            return FilePreview.GetPreviewFileURL(saveName, extension);
        }

        #endregion 预览
    }
}