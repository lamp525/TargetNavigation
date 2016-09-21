using MB.Web.Models;
using System;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class PlanStatisticsController : BaseController
    {
        //
        // GET: /PlanStatistics/
        private IPlanStatisticsBLL PlanStatisticsBLL { get; set; }

        private IPlanBLL PlanBLL { get; set; }

 
        public ActionResult Index()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var execution = PlanBLL.GetExecution(userId);
            ViewBag.Execution = execution;
            return View();
        }

        //获取统计名信息
        public string GetStatisticsList()
        {
            var statisticsList = PlanStatisticsBLL.GetStatisticsList();
            var jsonResult = new JsonResultModel(JsonResultType.success, statisticsList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取部门信息列表
        public string GetOrganizationInfo()
        {
            var departList = PlanBLL.GetDepartmentList();
            var jsonResult = new JsonResultModel(JsonResultType.success, departList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //新建统计信息
        public string AddStatisticsInfo()
        {
            var statics = Request.Form["param"];
            var staticsInfo = JsonConvert.DeserializeObject<StatisticsInfo>(statics);
            staticsInfo.createUser = int.Parse(Session["userId"].ToString());
            staticsInfo.createTime = DateTime.Now;
            staticsInfo.updateUser = int.Parse(Session["userId"].ToString());
            staticsInfo.updateTime = DateTime.Now;
            var jsonResult = new JsonResultModel(JsonResultType.success, PlanStatisticsBLL.AddStatisticsInfo(staticsInfo), "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //按统计号获取各部门计划完成情况
        public string GetStatusByOrg(int statisticsId, DateTime startTime, DateTime endTime, int sortby, int sortDirect)
        {
            var list = PlanStatisticsBLL.GetStatusByOrg(statisticsId, startTime, endTime.AddDays(1), sortby, sortDirect);
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //按部门号获取该部门成员的计划完成情况
        public string GetPlanStatistics(int organizationId, DateTime startTime, DateTime endTime, int sortby, int sort)
        {
            var list = PlanStatisticsBLL.GetPlanStatistics(organizationId, startTime, endTime.AddDays(1), sortby, sort);
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}