using MB.New.BLL.Tag;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class SharedController : BaseController
    {
        //
        // GET: /Shared/

        private TagManagementBLL tagManagement = new TagManagementBLL();

        public ActionResult Index()
        {
            return View();
        }

        #region 文件预览

        /// <summary>
        /// 取得预览文件地址
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string GetPreviewFile(string saveName, string extension)
        {
            var fileURL = FilePreview.GetPreviewFileURL(saveName, extension);

            JsonResultModel result = null;

            //判断预览文件是否存在
            if (string.IsNullOrEmpty(fileURL))
                result = new JsonResultModel(JsonResultType.error, null, "预览文件不存在！");
            else
                result = new JsonResultModel(JsonResultType.success, fileURL);

            return JsonConvert.SerializeObject(result);
        }

        #endregion 文件预览

        #region 用户常用标签

        /// <summary>
        /// 取得登陆用户常用标签
        /// </summary>
        /// <returns></returns>
        public string GetMostUsedTag()
        {
            //登陆用户ID
            var userId = this.LoginUserInfo().userId;

            //常用标签数组
            var tag = tagManagement.GetMostUsedTag(userId);

            var result = new JsonResultModel(JsonResultType.success, tag);

            return JsonConvert.SerializeObject(result);
        }

        #endregion 用户常用标签
    }
}