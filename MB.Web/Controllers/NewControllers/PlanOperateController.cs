using MB.Facade.PlanOperate;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class PlanOperateController : BaseController
    {
        //
        // GET: /PlanOperate/

        private IPlanOperateFacade facade { get; set; }

        //预览文件转换事件委托
        private delegate void ConvertHandler(int planId, string previewPath);

        public ActionResult Index()
        {
            return View();
        }

        #region 一般计划

        /// <summary>
        /// 批量提交待提交的计划信息
        /// </summary>
        /// <returns></returns>
        public string SubmitMultiPlan()
        {
            var planInfo = JsonConvert.DeserializeObject<List<PagePlanOperateModel>>(Request.Form["data"]);
            facade.SubmitMultiPlan(LoginUserInfo().userId, planInfo);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 提交待提交的计划信息
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="initial">
        /// 0：临时计划
        /// 1：一般计划
        /// </param>
        /// <returns></returns>
        public string SubmitPlan(int planId, bool isInitial)
        {
            facade.SubmitPlan(LoginUserInfo().userId, planId, isInitial);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量删除计划信息
        /// </summary>
        /// <returns></returns>
        public string DeleteMultiPlan()
        {
            var planIds = JsonConvert.DeserializeObject<int[]>(Request.Form["data"]);
            facade.DeleteMultiPlan(LoginUserInfo().userId, planIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除计划信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string DeletePlan(int planId)
        {
            facade.DeletePlan(LoginUserInfo().userId, planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 转办计划
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="responsibleUser">责任人ID</param>
        /// <param name="confirmUser">确认人ID</param>
        /// <returns></returns>
        public string TurnPlan(int planId, int responsibleUser, int confirmUser)
        {
            facade.TurnPlan(LoginUserInfo().userId, planId, responsibleUser, confirmUser);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 申请修改计划
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string AlterPlan(int planId)
        {
            facade.AlterPlan(LoginUserInfo().userId, planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 申请中止计划
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string StopPlan(int planId)
        {
            facade.StopPlan(LoginUserInfo().userId, planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 重新开始计划
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string RestartPlan(int planId)
        {
            facade.RestartPlan(LoginUserInfo().userId, planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 提交确认
        /// </summary>
        /// <returns></returns>
        public string SubmitConfirmPlan()
        {
            var operateInfo = JsonConvert.DeserializeObject<PagePlanOperateModel>(Request.Form["data"]);

            //提交确认处理
            facade.SubmitConfirmPlan(LoginUserInfo().userId, operateInfo);

            //附件预览转化处理
            ConvertHandler handler = new ConvertHandler(facade.PlanAttachConvertAsync);
            //预览文件夹物理地址
            string previewPath = WebHostPhysicalPath + ConstVar.PreviewPath;
            handler.BeginInvoke(operateInfo.planId, previewPath, null, null);

            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 更新计划进度
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="progress">计划进度</param>
        /// <returns></returns>
        public string UpdateProgress(int planId, int progress)
        {
            facade.UpdateProgress(LoginUserInfo().userId, planId, progress);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量审批计划
        /// </summary>
        /// <returns></returns>
        public string ApproveMultiPlan()
        {
            var planOperateList = JsonConvert.DeserializeObject<List<PagePlanOperateModel>>(Request.Form["data"]);
            facade.ApproveMultiPlan(LoginUserInfo().userId, planOperateList);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 审批计划
        /// </summary>
        /// <returns></returns>
        public string ApprovePlan()
        {
            var planOperate = JsonConvert.DeserializeObject<PagePlanOperateModel>(Request.Form["data"]);
            facade.ApprovePlan(LoginUserInfo().userId, planOperate);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 确认计划
        /// </summary>
        /// <returns></returns>
        public string ConfirmPlan()
        {
            var planOperate = JsonConvert.DeserializeObject<PagePlanOperateModel>(Request.Form["data"]);
            facade.ConfirmPlan(LoginUserInfo().userId, planOperate);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 撤销计划上次操作
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public string RevokePlan(int planId)
        {
            facade.RevokePlan(LoginUserInfo().userId, planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 一般计划

        #region 循环计划

        /// <summary>
        /// 批量提交待提交的循环计划信息
        /// </summary>
        /// <returns></returns>
        public string SubmitMultiLoopPlan()
        {
            var loopIds = JsonConvert.DeserializeObject<int[]>(Request.Form["data"]);
            facade.SubmitMultiLoopPlan(LoginUserInfo().userId, loopIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 提交待提交的循环计划信息
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="initial">
        /// 0：临时计划
        /// 1：一般计划
        /// </param>
        /// <returns></returns>
        public string SubmitLoopPlan(int loopId)
        {
            facade.SubmitLoopPlan(LoginUserInfo().userId, loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量删除循环计划信息
        /// </summary>
        /// <returns></returns>
        public string DeleteMultiLoopPlan()
        {
            var loopIds = JsonConvert.DeserializeObject<int[]>(Request.Form["data"]);
            facade.DeleteMultiLoopPlan(LoginUserInfo().userId, loopIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除循环计划信息
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string DeleteLoopPlan(int loopId)
        {
            facade.DeleteLoopPlan(LoginUserInfo().userId, loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 转办循环计划
        /// </summary>
        /// <param name="loopId"></param>
        /// <param name="responsibleUser">责任人ID</param>
        /// <param name="confirmUser">确认人ID</param>
        /// <returns></returns>
        public string TurnLoopPlan(int loopId, int responsibleUser, int confirmUser)
        {
            facade.TurnLoopPlan(LoginUserInfo().userId, loopId, responsibleUser, confirmUser);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 申请修改循环计划
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string AlterLoopPlan(int loopId)
        {
            facade.AlterLoopPlan(LoginUserInfo().userId, loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 申请中止循环计划
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string StopLoopPlan(int loopId)
        {
            facade.StopLoopPlan(LoginUserInfo().userId, loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 重新开始循环计划
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string RestartLoopPlan(int loopId)
        {
            facade.RestartPlan(LoginUserInfo().userId, loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 循环计划提交确认
        /// </summary>
        /// <returns></returns>
        public string SubmitConfirmLoopPlan()
        {
            var planOpreate = JsonConvert.DeserializeObject<PageLoopPlanOperateModel>(Request.Form["data"]);
            facade.SubmitConfirmLoopPlan(LoginUserInfo().userId, planOpreate);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量审批循环计划
        /// </summary>
        /// <returns></returns>
        public string ApproveMultiLoopPlan()
        {
            var planOperateList = JsonConvert.DeserializeObject<List<PageLoopPlanOperateModel>>(Request.Form["data"]);
            facade.ApproveMultiLoopPlan(LoginUserInfo().userId, planOperateList);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 审批循环计划
        /// </summary>
        /// <returns></returns>
        public string ApproveLoopPlan()
        {
            var planOperate = JsonConvert.DeserializeObject<PageLoopPlanOperateModel>(Request.Form["data"]);
            facade.ApproveLoopPlan(LoginUserInfo().userId, planOperate);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 确认循环计划
        /// </summary>
        /// <returns></returns>
        public string ConfirmLoopPlan()
        {
            var planOperate = JsonConvert.DeserializeObject<PageLoopPlanOperateModel>(Request.Form["data"]);
            facade.ConfirmLoopPlan(LoginUserInfo().userId, planOperate);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 撤销循环计划上次操作
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public string RevokeLoopPlan(int loopId)
        {
            facade.RevokeLoopPlan(LoginUserInfo().userId, loopId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 撤销循环计划完成情况的上次操作
        /// </summary>
        /// <param name="submitId"></param>
        /// <returns></returns>
        public string RevokeLoopPlanSubmit(int submitId)
        {
            facade.RevokeLoopPlanSubmit(LoginUserInfo().userId, submitId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

      

        #endregion 循环计划
    }
}