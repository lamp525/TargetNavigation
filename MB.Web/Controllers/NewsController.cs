using MB.Web.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class NewsController : BaseController
    {
        private INewsBLL NewsBLL { get; set; }

        

        //
        // GET: /News/

        public ActionResult NewsList(string Flag, int DriID)
        {
            Session["Flag"] = Flag;
            List<NewsInfo> impnewsList = NewsBLL.GetTopNewsByNotice(bool.Parse(Flag.ToString()));
            ViewBag.ImpNewsList = impnewsList;
            ViewBag.Falg = Flag;
            return View();
        }

        public string GetNew(string DriID)
        {
            bool Flag = bool.Parse(Session["Flag"].ToString());
            List<NewsInfo> newsList = new List<NewsInfo>();

            newsList = NewsBLL.GetNewsByNotice(Flag, int.Parse(DriID));

            foreach (var item in newsList)
            {
                item.title = MB.Common.StringUtils.CutString(item.title, 40);
                item.FCreatTime = item.createTime.ToString("yyyy-MM-dd");
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, newsList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public ActionResult AjaxImageNewsData()
        {
            string context = GetImageNewsJsonData();
            return Content(context);
        }

        public string GetImageNewsJsonData()
        {
            bool Flag = bool.Parse(Session["Flag"].ToString());
            string jsonIndex;
            var imgList = new List<NewsInfo>();
            List<NewsInfo> impnewsList = NewsBLL.GetTopNewsByNotice(bool.Parse(Flag.ToString()));
            foreach (var item in impnewsList)
            {
                if (item.titleImage != null)
                {
                    item.titleImage = "../" + ConfigurationManager.AppSettings["NewsUpLoadPath"].ToString() + "/" + item.titleImage;
                    imgList.Add(item);
                }
                //else
                //{
                //    impnewsList.Remove(item);
                //}
            }
            if (impnewsList.Count > 0)
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, imgList, "正常");
                jsonIndex = JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.error, null, "没有一个新闻哦");
                jsonIndex = JsonConvert.SerializeObject(jsonResult);
            }
            return jsonIndex;
        }

        public string getDirectory(string DriID)
        {
            bool Flag = bool.Parse(Session["Flag"].ToString());
            var newDirectory = new List<NewsDirectoryInfo>();
            string data = null;
            if (DriID == "0")
            {
                newDirectory = NewsBLL.GetDirectoryList(0, Flag);
            }
            else
            {
                newDirectory = NewsBLL.GetDirectoryList(int.Parse(DriID), Flag);
            }
            // ViewBag.Directory = newDirectory;
            if (newDirectory.Count == 0)
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, data, "error");
                data = JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, newDirectory, "error");
                data = JsonConvert.SerializeObject(jsonResult);
            }
            //  NewsList(Session["Flag"].ToString(),int.Parse( DriID));
            return data;
        }
    }
}