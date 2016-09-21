using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class UserController : BaseController
    {
        private IPlanBLL PlanBLL { get; set; }
        private IUserBLL UserBLL { get; set; }
        private IFlowIndexBLL FlowIndexBLL { get; set; }

 

        //
        // GET: /User/

        public ActionResult Head()
        {
            return View("~/Views/User/UserHeadEdit.cshtml");
        }

        public ActionResult UserIndex()
        {
            return View();
        }

        public ActionResult GetUserList()
        {
            List<UserIndexModel> IndexList = new List<UserIndexModel>();
            var userId = Convert.ToInt32(Session["userId"]);
            List<PlanInfo> PlanList = PlanBLL.GetPlanListDESC(int.Parse(Session["userId"].ToString()));
            foreach (PlanInfo planinfo in PlanList)
            {
                UserIndexModel userIndexModel = new UserIndexModel();
                userIndexModel.planId = planinfo.planId;
                userIndexModel.endTime = planinfo.endTime;
                DateTime now = DateTime.Now;
                if (!string.IsNullOrEmpty(planinfo.endTime.ToString()))
                {
                    TimeSpan span = now.Subtract(DateTime.Parse(planinfo.endTime.ToString()));
                    userIndexModel.TimeNum = (int)Math.Ceiling(span.TotalDays);
                }
                userIndexModel.eventOutput = planinfo.eventOutput;
                userIndexModel.importance = planinfo.importance;
                if (planinfo.progress == null)
                {
                    userIndexModel.progress = planinfo.progress.ToString() + "%";
                }
                userIndexModel.status = planinfo.status;
                userIndexModel.isloop = planinfo.isLoopPlan;
                userIndexModel.stop = planinfo.stop;
                userIndexModel.withfront = planinfo.isFronPlan;
                userIndexModel.collPlan = planinfo.IsCollPlan;
                userIndexModel.initial = planinfo.initial;
                userIndexModel.urgency = planinfo.urgency;
                IndexList.Add(userIndexModel);
                //获取登录用户信息

                //统计用户今日未完成和超时计划的数量
                ViewBag.Type = false;
            }
            ViewBag.List = IndexList;
            return View("_MyPlanList");
        }

        public ActionResult GetPlanInfo()
        {
            return View("/Plan/PlanInfo");
        }

        public string GetPlanInfoByPlanId(int planId, int isloop, int withfront, int collPlan)
        {
            var planInfoNew = PlanBLL.GetPlanInfoByPlanId(planId, (isloop == 1 ? true : false), (withfront == 1 ? true : false), (collPlan == 0 ? true : false), null);
            return JsonConvert.SerializeObject(planInfoNew);
        }

        public void GetUserInfoById()
        {
            UserInfo userinfo = new UserInfo();

            if (Session["userId"].ToString() != null)
            {
                userinfo = UserBLL.GetUserById(int.Parse(Session["userId"].ToString()));
            }
        }

        public void GetOutTimePlanNUmById()
        {
            UserPlanCountInfo userPlanCount = new UserPlanCountInfo();

            if (Session["userId"].ToString() != null)
            {
                userPlanCount = PlanBLL.StatisticsUserPlan(int.Parse(Session["userId"].ToString()));
            }
            ViewBag.OutTimePlan = userPlanCount;
        }

        public void AddCalendarById()
        {
            Calendar calendarInfo = JsonConvert.DeserializeObject<Calendar>(Request.Form["agendaData"]);
            calendarInfo.UserId = int.Parse(Session["userId"].ToString());
            if (calendarInfo.calendarId.HasValue)
            {
                UserBLL.UpdateCal(calendarInfo);
            }
            else
            {
                UserBLL.AddCalendarId(calendarInfo);
            }
        }

        public string GetXXUser()
        {
            var seachtext = Request.Form["text"];
            var UserList = new List<UserInfo>();
            if (!string.IsNullOrEmpty(seachtext))
            {
                UserList = PlanBLL.SelectUserList(seachtext.ToString(), true);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, UserList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string GetLoginUser()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            //获取登录用户信息
            var userInfo = PlanBLL.GetUserInfoById(userId);
            //统计用户今日未完成和超时计划的数量
            var userPlanCount = PlanBLL.StatisticsUserPlan(userId);
            //统计用户的未完成流程数量
            var userFlowCount = FlowIndexBLL.GetUserUnCompleteFlowCount(userId);

            loginUserModel logUser = new loginUserModel();
            logUser.userId = userId;
            logUser.userName = userInfo.userName;
            logUser.stationName = userInfo.stationName;
            logUser.bigImage = userInfo.bigImage;
            logUser.overTimePlan = userPlanCount.overTimePlan;
            logUser.todayPlanTotal = userPlanCount.todayPlanTotal;
            logUser.todayUnfinished = userPlanCount.todayUnfinished;
            logUser.unCompleteFlowCount = userFlowCount;

            //logUser.phone = userInfo.mobile1;
            var jsonResult = new JsonResultModel(JsonResultType.success, logUser, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string getCalendarJsonByCid(string calendarId)
        {
            List<Calendar> list = UserBLL.getCalendarByCId(int.Parse(calendarId));
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #region 获取日程安排Json数据

        /// <summary>
        /// 获取日程安排Json数据
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns></returns>
        public string getcalendarJson(string year, string month, string day)
        {
            int id = Convert.ToInt32(Session["userId"]);
            string strtime = null;

            List<Calendar> list = new List<Calendar>();
            if (day != null)
            {
                strtime = year + "/" + month + "/" + day;
                list = UserBLL.getCanlendarByUserTime(id, strtime);
            }
            else
            {
                DateTime d1 = new DateTime(int.Parse(year), int.Parse(month), 1);
                DateTime d2 = new DateTime(int.Parse(year), int.Parse(month) + 1, 1);
                list = UserBLL.getCanlendarByUserendTime(id, d1, d2);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 获取日程安排Json数据

        #region 更新个人设定

        /// <summary>
        /// 更新个人设定
        /// </summary>
        /// <param name="pagesize">每页显示数量</param>
        /// <param name="refreshTime">站内信刷新时间</param>
        /// <returns></returns>
        public string UpPersonalSetting(int pagesize, int refreshTime)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            if (UserBLL.UpPersonalSetting(userId, pagesize, refreshTime))
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "失败");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        #endregion 更新个人设定

        #region 更新用户密码

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="oldpwd">当前用户密码</param>
        /// <param name="pwd">用户密码</param>
        /// <returns></returns>
        public string UpPwd(string oldpwd, string pwd)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            int num = UserBLL.UpUserPwd(userId, oldpwd, pwd);
            if (num.Equals(0))
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, new string[] { "修改密码成功" }, "修改密码成功");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else if (num.Equals(1))
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, new string[] { "原始密码错误" }, "原始密码错误");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, new string[] { "查无此用户" }, "查无此用户");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        #endregion 更新用户密码

        #region 获取用户信息

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetPersonalSetting()
        {
            int userId = Convert.ToInt32(Session["userId"]);
            PersonalSetting personalSetting = UserBLL.GetPersonalSetting(userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, personalSetting, "成功");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 获取用户信息
    }
}