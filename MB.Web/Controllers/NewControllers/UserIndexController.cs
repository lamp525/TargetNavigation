using MB.Facade.Index;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class UserIndexController : BaseController
    {
        //
        // GET: /UserIndex/

        private IUserIndexFacade facade { set; get; }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得用户工作信息
        /// </summary>
        /// <returns></returns>
        public string GetUserWorkTimeInfo()
        {
            var result = facade.GetUserWorkTimeInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户激励信息
        /// </summary>
        /// <param name="statisticsType">1：周统计 2：月统计</param>
        /// <returns></returns>
        public string GetUserIncentiveInfo(EnumDefine.StatisticsType statisticsType)
        {
            var result = facade.GetUserIncentiveInfo(LoginUserInfo().userId, statisticsType);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户工时统计信息
        /// </summary>
        /// <param name="statisticsType">1：周统计 2：月统计</param>
        /// <returns></returns>
        public string GetUserWorkTimeStatistics(EnumDefine.StatisticsType statisticsType)
        {
            var result = facade.GetUserWorkTimeStatistics(LoginUserInfo().userId, statisticsType);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得绩效排行信息
        /// </summary>
        /// <param name="statisticsType">1：周统计 2：月统计</param>
        /// <returns></returns>
        public string GetPerformanceRankInfo(EnumDefine.StatisticsType statisticsType)
        {
            var result = facade.GetPerformanceRankInfo(statisticsType);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户计划完成情况
        /// </summary>
        /// <param name="statisticsType">1：周统计 2：月统计</param>
        /// <returns></returns>
        public string GetUserPlanCompleteInfo(EnumDefine.StatisticsType statisticsType)
        {
            var result = facade.GetUserPlanCompleteInfo(LoginUserInfo().userId, statisticsType);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户计划状态统计
        /// </summary>
        /// <param name="statisticsType">1：周统计 2：月统计</param>
        /// <returns></returns>
        public string GetUserPlanStatusInfo(EnumDefine.StatisticsType statisticsType)
        {
            var result = facade.GetUserPlanStatusInfo(LoginUserInfo().userId, statisticsType);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得计划列表信息
        /// </summary>
        /// <param name="type">0：我的计划 1：下属的计划</param>
        /// <returns></returns>
        public string GetNeedToDoPlanList(EnumDefine.PlanListType type)
        {
            var result = facade.GetNeedToDoPlanList(LoginUserInfo().userId, type);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}