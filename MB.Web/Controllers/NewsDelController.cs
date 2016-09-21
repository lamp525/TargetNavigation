using MB.Web.Models;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class NewsDelController : BaseController
    {
        #region 变量

        private INewsBLL NewsBLL { get; set; }

  
        #endregion 变量

        //
        // GET: /NewsDel/

        public ActionResult NewsDel(string id)
        {
            NewsBLL.UpdateNewsNum(int.Parse(id));
            NewsInfo newsinfo = NewsBLL.getNewsbyId(int.Parse(id));
            Session["Flag"] = newsinfo.notice;
            ViewBag.NewsInfo = newsinfo;
            NewsInfo newsLast = NewsBLL.GetLastNews(newsinfo.createTime, bool.Parse(newsinfo.notice.ToString()));
            ViewBag.newslast = newsLast;
            NewsInfo newsNext = NewsBLL.GetNextNews(newsinfo.createTime, bool.Parse(newsinfo.notice.ToString()));
            ViewBag.newsNext = newsNext;
            NewsDirectoryInfo DInfo = NewsBLL.GetTitle(int.Parse(id), bool.Parse(newsinfo.notice.ToString()));
            ViewBag.DInfp = DInfo;
            return View();
        }

        //public ActionResult GetNewsInfobyid()
        //{
        //}
    }
}