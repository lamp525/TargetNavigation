using MB.New.BLL.Tag;
using MB.New.Common;
using MB.New.Model;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class SearchController : BaseController
    {
        //
        // GET: /TagSearch/

        /// <summary> 标签检索对象</summary>
        private TagSearchBLL searchBLL = new TagSearchBLL();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        public ActionResult Search(string tagNames)
        {
            //读取配置文件中的量质时最大值
            ViewBag.CompleteQuantity = ConstVar.CompleteQuantity;
            ViewBag.CompleteQuality = ConstVar.CompleteQuantity;
            ViewBag.CompleteTime = ConstVar.CompleteTime;

            ViewBag.TagNames = tagNames;
            ViewBag.UserId = LoginUserInfo().userId;
            return View();
        }

        #region 取得检索结果信息

        /// <summary>
        /// 取得检索结果信息
        /// </summary>
        /// <returns></returns>
        public string GetSearchResult()
        {
            // 登陆用户ID
            var userId = LoginUserInfo().userId;

            var searchJson = Request.Form["data"];

            if (searchJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var searchInfo = JsonConvert.DeserializeObject<SearchInfoModel>(searchJson);

            object result = null;
            if (searchInfo.keyword != null && searchInfo.keyword.Length != 0)
            {
                //保存用户检索标签（异步处理）
                searchBLL.SaveRecentSearchTag(userId, searchInfo.keyword);

                //取得检索结果信息
                result = searchBLL.GetSearchResult(userId, searchInfo);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, result);

            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 取得检索结果信息

        #region 取得用户最近使用的检索标签信息

        /// <summary>
        /// 取得用户最近使用的检索标签信息
        /// </summary>
        /// <returns></returns>
        public string GetUserRecentSearchTag()
        {
            // 登陆用户ID
            var userId = LoginUserInfo().userId;

            var result = searchBLL.GetRecentSearchTag(userId);

            var jsonResult = new JsonResultModel(JsonResultType.success, result);

            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 取得用户最近使用的检索标签信息
    }
}