using MB.Facade.Plan;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class PlanController : BaseController
    {
        //
        // GET: /Plan/

        private IPlanFacade facade { get; set; }

        public ActionResult PlanIndex()
        {
            return View();
        }

        #region 新建画面绑定

        /// <summary>
        /// 取得计划默认信息
        /// </summary>
        /// <returns></returns>
        public string GetDefaultPlanInfo()
        {
            var result = facade.GetDefaultPlanInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得循环计划默认信息
        /// </summary>
        /// <returns></returns>
        public string GetDefaultLoopPlanInfo()
        {
            var result = facade.GetDefaultLoopPlanInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得执行方法列表信息
        /// </summary>
        /// <returns></returns>
        public string GetExecutionInfo()
        {
            var result = facade.GetExecutionInfo();
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户常用标签
        /// </summary>
        /// <returns></returns>
        public string GetMostUsedTag()
        {
            //登陆用户ID
            var userId = LoginUserInfo().userId;

            //常用标签数组
            var tag = facade.GetMostUsedTag(userId);

            var result = new JsonResultModel(JsonResultType.success, tag);

            return JsonConvert.SerializeObject(result);
        }

        #endregion 新建画面绑定

        #region 计划（新建、更新和详情）

        /// <summary>
        /// 快捷新建计划
        /// </summary>
        /// <returns></returns>
        public string QuickAddPlan()
        {
            var QuickAddPlan = JsonConvert.DeserializeObject<PageQuickAddModel>(Request.Form["data"]);
            var result = facade.QuickAddPlan(LoginUserInfo().userId, QuickAddPlan);

            JsonResultModel jsonResult = null;

            //成功的场合
            if (result) jsonResult = new JsonResultModel(JsonResultType.success, null);

            //失败的场合
            else jsonResult = new JsonResultModel(JsonResultType.error, null);

            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新建/更新计划
        /// </summary>
        /// <returns></returns>
        public string AddOrUpdatePlan()
        {
            var planInfo = JsonConvert.DeserializeObject<PagePlanInfoModel>(Request.Form["data"]);
            var now = DateTime.Now;
            var userId = LoginUserInfo().userId;

            facade.AddOrUpdatePlan(userId, planInfo);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新建/更新循环计划
        /// </summary>
        /// <returns></returns>
        public string AddOrUpdateLoopPlan()
        {
            var loopPlanInfo = JsonConvert.DeserializeObject<PageLoopPlanInfoModel>(Request.Form["data"]);
            var now = DateTime.Now;
            var userId = LoginUserInfo().userId;

            facade.AddOrUpdateLoopPlan(userId, loopPlanInfo);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得计划详情
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string GetPlanInfo(int planId)
        {
            var planInfo = facade.GetPlanInfo(planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, planInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得循环计划详情
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string GetLoopPlanInfo(int loopId)
        {
            var loopPlanInfo = facade.GetLoopPlanInfo(loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, loopPlanInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得循环计划提交信息
        /// </summary>
        /// <param name="loopId"></param>
        /// <param name="rollMode"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public string GetLoopPlanSubmitInfo(int loopId, EnumDefine.rollMode mode, int pageNum)
        {
            var submitInfo = facade.GetLoopPlanSubmitInfo(loopId,mode,pageNum);
            var jsonResult = new JsonResultModel(JsonResultType.success, submitInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 计划（新建、更新和详情）

        #region 操作日志

        /// <summary>
        /// 取得一般计划操作日志
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string GetPlanLogInfo(int planId)
        {
            var result = facade.GetPlanLogInfo(planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得循环计划操作日志
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string GetLoopPlanLogInfo(int loopId)
        {
            var result = facade.GetLoopPlanLogInfo(loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 操作日志

        #region 评论

        /// <summary>
        /// 取得计划评论信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string GetPlanCommentInfo(int planId)
        {
            var result = facade.GetPlanCommentInfo(planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 添加计划评论信息
        /// </summary>
        /// <returns></returns>
        public string AddPlanComment()
        {
            var commentInfo = JsonConvert.DeserializeObject<PagePlanCommentModel>(Request.Form["data"]);

            facade.AddPlanComment(LoginUserInfo().userId, commentInfo);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "评论成功！");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 评论

        #region 我的计划

        /// <summary>
        /// 取得我的计划状态数量
        /// </summary>
        /// <returns></returns>
        public string GetMyPlanStatusInfo()
        {
            var planStatus = facade.GetMyPlanStatusInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, planStatus);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的计划列表
        /// </summary>
        /// <returns></returns>
        public string GetMyPlanList()
        {
            var planSearch = JsonConvert.DeserializeObject<PagePlanSearchModel>(Request.Form["data"]);
            var planList = facade.GetMyPlanList(LoginUserInfo().userId, planSearch);
            var jsonResult = new JsonResultModel(JsonResultType.success, planList);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的循环计划列表
        /// </summary>
        /// <returns></returns>
        public string GetMyLoopPlanList()
        {
            var planSearch = JsonConvert.DeserializeObject<PagePlanSearchModel>(Request.Form["data"]);
            var loopPlanList = facade.GetMyLoopPlanList(LoginUserInfo().userId, planSearch);
            var jsonResult = new JsonResultModel(JsonResultType.success, loopPlanList);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的一般计划分组信息
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public string GetMyPlanGourpInfo(EnumDefine.PlanGroupType groupType)
        {
            var planGroupInfo = facade.GetMyPlanGourpInfo(LoginUserInfo().userId, groupType);
            var jsonResult = new JsonResultModel(JsonResultType.success, planGroupInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的循环计划分组信息
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public string GetMyLoopPlanGourpInfo(EnumDefine.PlanGroupType groupType)
        {
            var planGroupInfo = facade.GetMyLoopPlanGourpInfo(LoginUserInfo().userId, groupType);
            var jsonResult = new JsonResultModel(JsonResultType.success, planGroupInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的月有效工时信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetMyMonthWorkTimeInfo(string yearMonth)
        {
            var MonthWorkTimeInfo = facade.GetMyMonthWorkTimeInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, MonthWorkTimeInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的月计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetMyPlanPieChartInfo(string yearMonth)
        {
            var PlanPieChartInfo = facade.GetMyPlanPieChartInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, PlanPieChartInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得我的月循环计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetMyLoopPlanPieChartInfo(string yearMonth)
        {
            var LoopPlanPieChartInfo = facade.GetMyLoopPlanPieChartInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, LoopPlanPieChartInfo);
            return JsonConvert.SerializeObject(jsonResult);
        } 
        #endregion 我的计划

        #region 下属计划

        /// <summary>
        /// 取得下属计划状态数量
        /// </summary>
        /// <returns></returns>
        public string GetSubordinatePlanStatusInfo()
        {
            var planStatus = facade.GetSubordinatePlanStatusInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, planStatus);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属计划列表
        /// </summary>
        /// <returns></returns>
        public string GetSubordinatePlanList()
        {
            var planSearch = JsonConvert.DeserializeObject<PagePlanSearchModel>(Request.Form["data"]);
            var planList = facade.GetSubordinatePlanList(LoginUserInfo().userId, planSearch);
            var jsonResult = new JsonResultModel(JsonResultType.success, planList);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属循环计划列表
        /// </summary>
        /// <returns></returns>
        public string GetSubordinateLoopPlanList()
        {
            var planSearch = JsonConvert.DeserializeObject<PagePlanSearchModel>(Request.Form["data"]);
            var loopPlanList = facade.GetSubordinateLoopPlanList(LoginUserInfo().userId, planSearch);
            var jsonResult = new JsonResultModel(JsonResultType.success, loopPlanList);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属一般计划分组信息
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public string GetSubordinatePlanGourpInfo(EnumDefine.PlanGroupType groupType)
        {
            var planGroupInfo = facade.GetSubordinatePlanGourpInfo(LoginUserInfo().userId, groupType);
            var jsonResult = new JsonResultModel(JsonResultType.success, planGroupInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属循环计划分组信息
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public string GetSubordinateLoopPlanGourpInfo(EnumDefine.PlanGroupType groupType)
        {
            var planGroupInfo = facade.GetSubordinateLoopPlanGourpInfo(LoginUserInfo().userId, groupType);
            var jsonResult = new JsonResultModel(JsonResultType.success, planGroupInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属月有效工时信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetSubordinateMonthWorkTimeInfo(string yearMonth)
        {
            var MonthWorkTimeInfo = facade.GetSubordinateMonthWorkTimeInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, MonthWorkTimeInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属月计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetSubordinatePlanPieChartInfo(string yearMonth)
        {
            var PlanPieChartInfo = facade.GetSubordinatePlanPieChartInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, PlanPieChartInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得下属月循环计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetSubordinateLoopPlanPieChartInfo(string yearMonth)
        {
            var PlanPieChartInfo = facade.GetSubordinateLoopPlanPieChartInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, PlanPieChartInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 下属计划

        #region 协作计划

        /// <summary>
        /// 取得协作计划状态数量
        /// </summary>
        /// <returns></returns>
        public string GetCooperationPlanStatusInfo()
        {
            var planStatus = facade.GetCooperationPlanStatusInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, planStatus);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得协作计划列表
        /// </summary>
        /// <returns></returns>
        public string GetCooperationPlanList()
        {
            var planSearch = JsonConvert.DeserializeObject<PagePlanSearchModel>(Request.Form["data"]);
            var CooperationPlanList = facade.GetCooperationPlanList(LoginUserInfo().userId, planSearch);
            var jsonResult = new JsonResultModel(JsonResultType.success, CooperationPlanList);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得协作循环计划列表
        /// </summary>
        /// <returns></returns>
        public string GetCooperationLoopPlanList()
        {
            var planSearch = JsonConvert.DeserializeObject<PagePlanSearchModel>(Request.Form["data"]);
            var CooperationPlanList = facade.GetCooperationLoopPlanList(LoginUserInfo().userId, planSearch);
            var jsonResult = new JsonResultModel(JsonResultType.success, CooperationPlanList);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得协作一般计划分组信息
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public string GetCooperationPlanGourpInfo(EnumDefine.PlanGroupType groupType)
        {
            var planGroupInfo = facade.GetCooperationPlanGourpInfo(LoginUserInfo().userId, groupType);
            var jsonResult = new JsonResultModel(JsonResultType.success, planGroupInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得协作循环计划分组信息
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public string GetCooperationLoopPlanGourpInfo(EnumDefine.PlanGroupType groupType)
        {
            var planGroupInfo = facade.GetCooperationLoopPlanGourpInfo(LoginUserInfo().userId, groupType);
            var jsonResult = new JsonResultModel(JsonResultType.success, planGroupInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得月协作计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public string GetCooperationPlanPieChartInfo(string yearMonth)
        {
            var planPieChartInfo = facade.GetCooperationPlanPieChartInfo(LoginUserInfo().userId, yearMonth);
            var jsonResult = new JsonResultModel(JsonResultType.success, planPieChartInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 协作计划
    }
}