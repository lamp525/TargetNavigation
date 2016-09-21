using MB.Web.Models;
using System;
using System.Web.Mvc;
using MB.BLL;
using MB.Web.Common;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class ModelViewController : BaseController
    {
        private IPlanBLL PlanBLL { get; set; }
        private ISharedBLL SharedBLL { get; set; }

 

        //
        // GET: /ModelView/

        public ActionResult Index()
        {
            return View();
        }

        //返回模板页头部
        public ActionResult Top()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var execution = PlanBLL.GetExecution(userId);
            var admin = SharedBLL.GetAdmin(userId);
            ViewBag.Admin = admin;
            ViewBag.Execution = execution;
            Session["execution"] = execution;
            return View("TopView");
        }
    }
}