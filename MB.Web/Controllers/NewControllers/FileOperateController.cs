using MB.Facade.File;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class FileOperateController : BaseController
    {
        //
        // GET: /File/

        private IFileOperateFacade facade { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">文件所属对象ID</param>
        /// <returns></returns>
        public string GetFileInfo(EnumDefine.FileType type, int targetId)
        {
            var result = facade.GetFileInfo(type, targetId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">
        /// -1：头像文件上传用
        /// 其它：附件所属对象ID
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public string UploadFile(EnumDefine.FileType type, int targetId)
        {
            List<PageFileInfoModel> result = null;

            if (Request.Files.Count > 0)
            {
                result = facade.UploadFile(base.LoginUserInfo().userId, type, targetId, Request.Files);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="fileId">附件ID或文档ID</param>
        /// <returns></returns>
        [HttpPost]
        public string DeleteFile(EnumDefine.FileType type, int fileId)
        {
            var result = facade.DeleteFile(type, fileId);

            JsonResultModel jsonResult = null;
            //删除成功
            if (result)
                jsonResult = new JsonResultModel(JsonResultType.success, null);
            //删除失败
            else
                jsonResult = new JsonResultModel(JsonResultType.error, null);

            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 下载单个文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="fileId">附件ID或文档ID</param>
        public ActionResult SingleDownload(EnumDefine.FileType type, int fileId)
        {
            var fileInfo = facade.SingleDownload(type, fileId);

            if (fileInfo == null || !System.IO.File.Exists(fileInfo.filePath))
                return Content("error");
            else
                return File(fileInfo.filePath, "application/octet-stream", HttpUtility.UrlEncode(fileInfo.displayName, System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// 下载多个文件
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <param name="targetId">计划、循环计划、目标、流程或会议室ID</param>
        /// <returns></returns>
        public ActionResult MultiDownload(EnumDefine.FileType type, int targetId)
        {
            System.IO.MemoryStream ms = facade.MultiDownload(type, targetId);

            if (ms == null || ms.Length == 0)
            {
                return Content("error");
            }

            return File(ms, "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}.zip", DateTime.Now.ToLongDateString()), System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// 文件预览
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string Preview(string saveName, string extension)
        {
            var fileURL = facade.Preview(saveName, extension);

            JsonResultModel result = null;

            //预览文件URL是否存为空
            if (string.IsNullOrEmpty(fileURL))
                result = new JsonResultModel(JsonResultType.error, null, "预览文件不存在！");
            else
                result = new JsonResultModel(JsonResultType.success, fileURL);

            return JsonConvert.SerializeObject(result);
        }
    }
}