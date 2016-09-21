using MB.Web.Models;
using System;
using System.Configuration;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class SearchController : BaseController
    {
        //
        // GET: /TagSearch/

        /// <summary> 标签检索对象</summary>
        private ITagSearchBLL TagSearchBLL { get; set; }

  

        public ActionResult Search(string tagNames)
        {
            //读取配置文件中的量质时最大值
            ViewBag.CompleteQuantity = ConfigurationManager.AppSettings["maxQuantity"];
            ViewBag.CompleteQuality = ConfigurationManager.AppSettings["maxQuality"];
            ViewBag.CompleteTime = ConfigurationManager.AppSettings["maxTime"];
            ViewBag.TagNames = tagNames;
            ViewBag.UserId = Session["userId"];
            return View();
        }

        /// <summary>
        /// 取得检索结果信息
        /// </summary>
        /// <returns></returns>
        public string GetSearchResult()
        {
            // 登陆用户ID
            var userId = Session["userId"];

            if (userId == null)
            {
                return AjaxCallBack.FAIL;
            }

            var searchJson = Request.Form["data"];

            if (searchJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var searchInfo = JsonConvert.DeserializeObject<SearchInfoModel>(searchJson);

            object result = null;
            if (searchInfo.keyword != null && searchInfo.keyword.Length != 0)
            {
                //保存用户检索标签
                TagSearchBLL.SaveRecentSearchTag(Convert.ToInt32(userId), searchInfo.keyword);

                //取得检索结果信息
                result = TagSearchBLL.GetSearchResult(Convert.ToInt32(userId), searchInfo);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, result, "正常");

            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户最近使用的检索标签信息
        /// </summary>
        /// <returns></returns>
        public string GetUserRecentSearchTag()
        {
            // 登陆用户ID
            var userId = Session["userId"];

            if (userId == null)
            {
                return AjaxCallBack.FAIL;
            }

            var result = TagSearchBLL.GetRecentSearchTag(Convert.ToInt32(userId));

            var jsonResult = new JsonResultModel(JsonResultType.success, result, "正常");

            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}