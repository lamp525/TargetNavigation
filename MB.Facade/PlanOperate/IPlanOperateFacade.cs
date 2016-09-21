using System.Collections.Generic;

namespace MB.Facade.PlanOperate
{
    public interface IPlanOperateFacade
    {
        #region 一般计划

        /// <summary>
        /// 计划附件预览转化处理
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="previewPath"></param>
        /// <returns></returns>
        void PlanAttachConvertAsync(int planId, string previewPath);

        /// <summary>
        /// 批量提交计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planInfo"></param>
        void SubmitMultiPlan(int userId, List<PagePlanOperateModel> planInfo);

        /// <summary>
        /// 提交计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void SubmitPlan(int userId, int planId, bool isInitial);

        /// <summary>
        /// 批量删除计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planIds"></param>
        void DeleteMultiPlan(int userId, int[] planIds);

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void DeletePlan(int userId, int planId);

        /// <summary>
        /// 转办计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        ///  <param name="responsibleUser"></param>
        /// <param name="confirmUser"></param>
        void TurnPlan(int userId, int planId, int responsibleUser, int confirmUser);

        /// <summary>
        /// 申请修改计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void AlterPlan(int userId, int planId);

        /// <summary>
        /// 申请中止计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void StopPlan(int userId, int planId);

        /// <summary>
        /// 重新开始计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void RestartPlan(int userId, int planId);

        /// <summary>
        /// 提交确认
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void SubmitConfirmPlan(int userId, PagePlanOperateModel planOperate);

        /// <summary>
        /// 更新计划进度
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="progress">计划进度</param>
        /// <returns></returns>
        void UpdateProgress(int userId, int planId, int progress);

        /// <summary>
        /// 批量审批计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperateList"></param>
        void ApproveMultiPlan(int userId, List<PagePlanOperateModel> planOperateList);

        /// <summary>
        /// 审批计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ApprovePlan(int userId, PagePlanOperateModel planOperate);

        /// <summary>
        /// 确认计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ConfirmPlan(int userId, PagePlanOperateModel planOperate);

        /// <summary>
        /// 撤销计划上次操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void RevokePlan(int userId, int planId);

        #endregion 一般计划

        #region 循环计划

        /// <summary>
        /// 批量提交循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopIds"></param>
        void SubmitMultiLoopPlan(int userId, int[] loopIds);

        /// <summary>
        /// 提交循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void SubmitLoopPlan(int userId, int loopId);

        /// <summary>
        /// 批量删除循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopIds"></param>
        void DeleteMultiLoopPlan(int userId, int[] loopIds);

        /// <summary>
        /// 删除循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void DeleteLoopPlan(int userId, int loopId);

        /// <summary>
        /// 转办循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        ///  <param name="responsibleUser"></param>
        /// <param name="confirmUser"></param>
        void TurnLoopPlan(int userId, int loopId, int responsibleUser, int confirmUser);

        /// <summary>
        /// 申请修改循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void AlterLoopPlan(int userId, int loopId);

        /// <summary>
        /// 申请中止循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void StopLoopPlan(int userId, int loopId);

        /// <summary>
        /// 重新开始循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void RestartLoopPlan(int userId, int loopId);

        /// <summary>
        /// 循环计划提交确认
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopPlanOperate"></param>
        void SubmitConfirmLoopPlan(int userId, PageLoopPlanOperateModel loopPlanOperate);

        /// <summary>
        /// 批量审批循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperateList"></param>
        void ApproveMultiLoopPlan(int userId, List<PageLoopPlanOperateModel> loopPlanOperate);

        /// <summary>
        /// 审批循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ApproveLoopPlan(int userId, PageLoopPlanOperateModel loopPlanOperate);

        /// <summary>
        /// 确认循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ConfirmLoopPlan(int userId, PageLoopPlanOperateModel loopPlanOperate);

        /// <summary>
        /// 撤销循环计划上次操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void RevokeLoopPlan(int userId, int loopId);

        /// <summary>
        /// 撤销循环计划完成情况的上次操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="submitId"></param>
        void RevokeLoopPlanSubmit(int userId, int submitId);

        #endregion 循环计划
    }
}