using MB.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class NewsSeachListController : BaseController
    {
        private INewsBLL NewsBLL { get; set; }

   

        //
        // GET: /NewsSeachList/

        public ActionResult _NewsSeachList(string Flag, int DriID)
        {
            List<NewsInfo> newsList = NewsBLL.GetNewsByNotice(bool.Parse(Flag.ToString()), DriID);
            ViewBag.NewsList = newsList;
            return View(" /Views/News/_NewsSeachList.cshtml");
        }
    }
}