using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class UserIndexWorkTimeController : BaseController
    {
        //
        // GET: /UserIndexWorkTime/
        private CommonWorkTime comworktime = new CommonWorkTime();

        public ActionResult Index()
        {
            return View();
        }

        #region 个人工效价值

        /// <summary>
        /// 个人工效价值
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="mode">0/1，0是年，1是月</param>
        /// <returns></returns>
        public string getJson(int year, int month, int mode)
        {
            DateTime time;
            string strtime = year + "/" + month;
            string size = mode == 0 ? "year" : "month";//mode  0是年，1是月
            string josnindex;
            if (DateTime.TryParse(strtime, out time))
            {
                List<Worktime> worktimeForUserindex = comworktime.getPersonalWorktime(Convert.ToInt32(Session["userId"]), time, size);
                var jsonResult = new JsonResultModel(JsonResultType.success, worktimeForUserindex, "正常");
                josnindex = JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "请检查传递的参数是否符合");
                josnindex = JsonConvert.SerializeObject(jsonResult);
            }
            return josnindex;
        }

        #endregion 个人工效价值
    }
}