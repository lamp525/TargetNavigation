using MB.Web.Models;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class CompanyController : BaseController
    {
        private ICompanyBLL CompanyBLL { get; set; }

       

        //CompanyBLL companyBill = new CompanyBLL();

        //新闻绑定
        public ActionResult CompanyIndex()
        {
            //bool notice = false;
            List<NewsInfo> list = CompanyBLL.GetNewsInfo();
            ViewBag.NewsList = list;
            return View();
        }

        public ActionResult newsIndex()
        {
            // bool notice = false;
            List<NewsInfo> list = CompanyBLL.GetNewsInfo();
            ViewBag.NewsList = list;
            return View("_newstoplist");
        }

        public ActionResult notesIndex()
        {
            // bool notice = false;
            List<NewsInfo> list = CompanyBLL.GetNewsInfo();
            ViewBag.NewsList = list;
            return View("_noticeList");
        }

        #region 首页Ajax获取新闻图片信息前5条     --谢小鹏

        /// <summary>
        /// 首页Ajax获取新闻图片信息
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxImageNewsData()
        {
            string context = GetImageNewsJsonData();
            return Content(context);
        }

        /// <summary>
        /// 首页图片新闻数据前5条转换成JSON格式数据
        /// </summary>
        /// <returns></returns>
        public string GetImageNewsJsonData()
        {
            string jsonIndex;
            List<IndexImageInfo> newsImageNewsIndex = CompanyBLL.GetCompabyImageTop5();
            foreach (var item in newsImageNewsIndex)
            {
                item.imgUrl = "../" + ConfigurationManager.AppSettings["NewsUpLoadPath"].ToString() + "/" + item.imageName;
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, newsImageNewsIndex, "正常");

            jsonIndex = JsonConvert.SerializeObject(jsonResult);

            return jsonIndex;
        }

        #endregion 首页Ajax获取新闻图片信息前5条     --谢小鹏

        #region 首页Ajax获取文档数据前9条        --谢小鹏

        /// <summary>
        /// 首页Ajax获取文档数据
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxDocumentData()
        {
            string context = GetDocumentJsonData();
            return Content(context);
        }

        /// <summary>
        /// 首页文档数据前9条转换成JSON格式数据
        /// </summary>
        /// <returns></returns>
        public string GetDocumentJsonData()
        {
            // string jsonIndex;
            List<DocumentInfo> DocumentListsIndex = CompanyBLL.GetDocumentTop9();
            ViewBag.DownLoad = DocumentListsIndex;
            if (DocumentListsIndex.Count > 0)
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, DocumentListsIndex, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "请检查传递的参数【后期返回友好错误信息】");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        #endregion 首页Ajax获取文档数据前9条        --谢小鹏

        #region 首页下载文档的方法；              --谢小鹏

        /// <summary>
        /// 首页下载文档的方法
        /// </summary>
        /// <returns></returns>
        public ActionResult DownDocument()
        {
            string fileX = null;
            DocumentInfo docinfo = new DocumentInfo();
            if (!(string.IsNullOrEmpty(Request.QueryString["DownDocumentid"])))
            {
                docinfo = CompanyBLL.GetDocumentById(int.Parse(Request.QueryString["DownDocumentid"]));
                fileX = docinfo.saveName;
            }
            string filePath = Path.Combine(ConfigurationManager.AppSettings["DocumentUpLoadPath"].ToString(), fileX);
            if (System.IO.File.Exists(filePath))
            {
                return File(new FileStream(filePath, FileMode.Open), "application/octet-stream", HttpUtility.UrlEncode(docinfo.displayName, System.Text.Encoding.UTF8));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new JsonResultModel(JsonResultType.error, null, "您下载的文件不存在")));
            }
        }

        #endregion 首页下载文档的方法；              --谢小鹏

        public ActionResult GetWorktime()
        {
            return View("~/Views/Statement/_worktimeIndex.cshtml");
        }
    }
}