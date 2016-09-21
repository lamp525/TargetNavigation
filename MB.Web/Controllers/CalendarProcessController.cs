using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class CalendarProcessController : BaseController
    {
        private IObjectiveIndexBLL ObjectiveIndexBLL { get; set; }
        private IPlanBLL PlanBLL { get; set; }
        private IFlowIndexBLL FlowIndexBLL { get; set; }

     

        //
        // GET: /CalendarProcess/

        public ActionResult CalendarProcess()
        {
            ViewBag.CompleteQuantity = ConfigurationManager.AppSettings["maxQuantity"];
            ViewBag.CompleteQuality = ConfigurationManager.AppSettings["maxQuality"];
            ViewBag.CompleteTime = ConfigurationManager.AppSettings["maxTime"];
            return View();
        }

        //获取计划列表
        public string GetPlanList()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) ReturnJson(JsonResultType.error, null, "获取计划列表失败");
            var conditionModel = JsonConvert.DeserializeObject<CalendarPlanModel>(dataJson);
            var conditionString = string.Empty;
            //var loopCondition = string.Empty;   //获取循环列表的筛选条件
            var userId = Convert.ToInt32(Session["userId"]);
            var conditionStart = conditionModel.date.Date;
            var conditionEnd = conditionStart.AddDays(1);
            //var flag = 0;     //判断是否是取下属计划的待确认状态列表，取循环计划用

            #region 拼凑SQL

            if (conditionModel.calendarType == 1)  //我的日程
            {
                conditionString = " responsibleUser==" + userId + " And (status==0 Or status==20 Or status==15 Or status==40) And stop==0 ";
                //loopCondition = " responsibleUser==" + userId + " And (status==0 Or status==20 Or status==15 ) ";
            }
            else          //下属日程
            {
                conditionString = " confirmUser==" + userId + "  ";
                //loopCondition =" confirmUser==" + userId + "  ";
                if (conditionModel.status == 1)    //待审核计划
                {
                    conditionString += " And ( stop==10 Or (stop==0 And (status=10 Or status==25))) ";
                    //loopCondition += " And ( status=10 Or status==25) ";
                    //flag = 1;
                }
                else                  //待确认计划
                {
                    conditionString += " And stop==0 And status==30 ";
                    //loopCondition += " And status==20 ";
                    //flag = 2;
                }
            }

            #endregion 拼凑SQL

            var planList = PlanBLL.GetCalendarPlanList(userId, conditionString, conditionModel.calendarType == 1 ? string.Empty : "1", conditionStart, conditionEnd);
            //var loopList = planBll.GetLoopPlanList(userId, loopCondition, conditionStart, conditionEnd, flag);
            //planList.AddRange(loopList);
            planList = planList.OrderByDescending(p => p.updateTime).ToList();
            return ReturnJson(JsonResultType.success, planList, "获取计划列表失败");
        }

        //获取计划详情
        public string GetPlanDetail(int planId, bool isloop, int? submitId)
        {
            var planInfoNew = PlanBLL.GetPlanInfoByPlanId(planId, isloop, false, false, submitId);
            var jsonResult = new JsonResultModel(JsonResultType.success, planInfoNew, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取批量操作的计划集合
        public string GetPlanSimpleList()
        {
            var planJson = Request.Form["plans"];
            var planIds = JsonConvert.DeserializeObject<int?[]>(planJson);
            var loopJson = Request.Form["loops"];
            var loopIds = JsonConvert.DeserializeObject<int?[]>(loopJson);
            var planList = PlanBLL.GetPlanSimpleList(planIds, loopIds);
            return ReturnJson(JsonResultType.success, planList, "获取计划集合出错!");
        }

        //获取流程列表
        public string GetFlowList(int calendarType, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            var flowList = new List<UserFormModel>();
            var userId = Convert.ToInt32(Session["userId"]);
            if (calendarType == 1)    //我的日程
            {
                flowList = FlowIndexBLL.GetCalerdarUnSubmitFlowList(userId, string.Empty, start, end);
            }
            else               //下属日程
            {
                flowList = FlowIndexBLL.GetCalendarUnCheckFlowList(userId, string.Empty, start, end, null);
            }
            return ReturnJson(JsonResultType.success, flowList, "获取流程列表失败");
        }

        //获取目标列表
        public string GetObjectiveList()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) ReturnJson(JsonResultType.error, null, "获取计划列表失败");
            var conditionModel = JsonConvert.DeserializeObject<CalendarPlanModel>(dataJson);
            var date = conditionModel.date.Date;
            var conditionString = string.Empty;
            var userId = Convert.ToInt32(Session["userId"]);
            if (conditionModel.calendarType == 1)   //我的日程
            {
                conditionString = " (responsibleUser==" + userId + " Or authorizedUser==" + userId + ") And (status==1 Or status==3) ";
            }
            else         //下属日程
            {
                conditionString = " confirmUser==" + userId + " ";
                if (conditionModel.status == null)
                {
                    conditionString += " And (status==2 Or status==4) ";
                }
                else if (conditionModel.status == 1)  //待审核目标
                {
                    conditionString += " And status==2 ";
                }
                else if (conditionModel.status == 2) //待确认目标
                {
                    conditionString += " And status==4 ";
                }
            }
            var objectiveInfo = ObjectiveIndexBLL.GetCalendarObjectiveList(conditionString, date, userId);
            //var objectiveModel = new ObjectiveInfo();
            //if (objectiveInfo.ObjectiveList!=null)
            //{
            //    objectiveModel.userId=userId;
            //    objectiveModel.ObjectiveList = objectiveInfo.ObjectiveList.OrderByDescending(p => p.updateTime).ToList();
            //};
            return ReturnJson(JsonResultType.success, objectiveInfo, "获取目标列表失败");
        }

        //计划批量审核
        public string PlanBatchAudit()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "审核失败");
            var checkModel = JsonConvert.DeserializeObject<PlanCheckModel>(dataJson);
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = true;
            //普通计划的审核
            if (checkModel.planId != null || checkModel.planId.Length > 0)
            {
                foreach (var item in checkModel.planId)
                {
                    this.PlanBLL.ExaminePlan(item.Value, checkModel.importance, checkModel.urgency, checkModel.difficulty, checkModel.type, userId, checkModel.message);
                }
            }
            //循环计划的审核
            if (checkModel.loopInfo != null && checkModel.loopInfo.Count > 0)
            {
                foreach (var item in checkModel.loopInfo)
                {
                    flag = this.PlanBLL.ExamineLoopPlan(item.loopId, checkModel.importance, checkModel.urgency, checkModel.difficulty, checkModel.message, checkModel.type, userId);
                }
            }
            return ReturnJson(JsonResultType.success, flag, "审核失败");
        }

        //计划批量确认
        public string PlanBatchComfirm()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "确认失败");
            var checkModel = JsonConvert.DeserializeObject<PlanCheckModel>(dataJson);
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = true;
            //普通计划的确认
            if (checkModel.planId != null || checkModel.planId.Length > 0)
            {
                foreach (var item in checkModel.planId)
                {
                    this.PlanBLL.ConfirmPlan(item.Value, userId, checkModel.importance, checkModel.urgency, checkModel.difficulty, checkModel.type, checkModel.message);
                }
            }
            //循环计划的确认
            if (checkModel.loopInfo != null || checkModel.loopInfo.Count > 0)
            {
                foreach (var item in checkModel.loopInfo)
                {
                    flag = this.PlanBLL.ConfirmLoopPlan(item.submitId.Value, item.loopId, checkModel.completeQuantity, checkModel.completeQuality, checkModel.completeTime, checkModel.message, checkModel.type, userId);
                }
            }
            return ReturnJson(JsonResultType.success, flag, "确认失败");
        }

        //流程批量审批
        public string FlowBatchAudit()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "操作失败");
            var checkModel = JsonConvert.DeserializeObject<FlowBatchCheckModel>(dataJson);
            if (checkModel != null && checkModel.flowList.Count <= 0) return ReturnJson(JsonResultType.error, null, "操作失败");
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = true;
            if (checkModel.type)   //同意
            {
                checkModel.flowList.ForEach(p =>
                {
                    flag = FlowIndexBLL.AgreeFormFlow(p.templateId, p.formId, p.nodeId, checkModel.content, userId, p.isEtruct);
                });
            }
            else         //退回
            {
                checkModel.flowList.ForEach(p =>
                {
                    flag = FlowIndexBLL.TurnBack(p.nodeId, p.templateId, p.formId, checkModel.content, userId, p.isEtruct);
                });
            }
            return ReturnJson(JsonResultType.success, flag, "操作失败");
        }

        //目标的批量确认 type:11：确认通过 12：确认不通过
        public string ObjectiveBatchComfirm(string message, int type)
        {
            var dataJson = Request.Form["objectiveId"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "操作失败");
            var objectiveId = JsonConvert.DeserializeObject<int[]>(dataJson);
            if (objectiveId.Length <= 0) return ReturnJson(JsonResultType.error, null, "未选择目标进行确认");
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = true;
            foreach (var item in objectiveId)
            {
                flag = ObjectiveIndexBLL.ConfirmObjective(item, message, type, userId);
            }
            return ReturnJson(JsonResultType.error, flag, "操作失败");
        }

        //返回JSON串
        public string ReturnJson(JsonResultType type, object content, string errorMsg)
        {
            var jsonResult = new JsonResultModel(type, content, errorMsg);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}