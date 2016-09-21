using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;

namespace MB.New.BLL.Plan
{
    public interface IPlanBLL
    {
        /// <summary>
        /// 取得执行方法列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        List<ExecutionInfoModel> GetExecutionInfo(TargetNavigationDBEntities db);

        /// <summary>
        /// 用户计划完成情况
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        PlanCompleteCountModel GetUserPlanCompleteInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null);

        #region 一般计划

        /// <summary>
        /// 取得计划协作人信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        List<UserInfoSimpleModel> GetPlanPartnerInfo(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 添加计划协作人
        /// </summary>
        /// <param name="db"></param>
        /// <param name="partnerId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        void InsPlanPartnerInfo(TargetNavigationDBEntities db, int partnerId, int planId);

        /// <summary>
        /// 删除协作人
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        void DeletePlanPartnerInfo(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 取得用户最新计划信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PlanInfoModel GetLatestPlanInfo(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 取得我的计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        PlanStatusCountModel GetMyPlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null);

        /// <summary>
        /// 取得下属计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        PlanStatusCountModel GetSubordinatePlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null);

        /// <summary>
        /// 取得我的计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PlanInfoModel> GetMyPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo);

        /// <summary>
        /// 取得我的一般计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PlanGroupInfoModel> GetMyPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得下属计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PlanInfoModel> GetSubordinatePlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo);

        /// <summary>
        /// 取得下属一般计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PlanGroupInfoModel> GetSubordinatePlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得协作计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PlanInfoModel> GetCooperationPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo);

        /// <summary>
        /// 取得协作一般计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PlanGroupInfoModel> GetCooperationPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 添加计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        /// <returns>计划ID</returns>
        int InsPlanInfo(TargetNavigationDBEntities db, PlanInfoModel planInfo);

        /// <summary>
        /// 更新计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        void UpdPlanInfo(TargetNavigationDBEntities db, PlanInfoModel planInfo);

        /// <summary>
        /// 根据计划ID取得详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        PlanInfoModel GetPlanInfoById(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 添加计划附件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachInfo"></param>
        /// <returns>附件ID</returns>
        int InsPlanAttachInfo(TargetNavigationDBEntities db, FileInfoModel attachInfo);

        /// <summary>
        /// 删除计划附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachId"></param>
        void DelPlanAttachInfoByAttachId(TargetNavigationDBEntities db, int attachId);

        /// <summary>
        /// 删除计划附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        void DelPlanAttachInfoByPlanId(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 根据计划ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        List<FileInfoModel> GetPlanAttachInfoByPlanId(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 根据文件ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        FileInfoModel GetPlanAttachInfoByFileId(TargetNavigationDBEntities db, int fileId);

        /// <summary>
        /// 提交计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="isInitial"></param>
        void SubmitPlanById(TargetNavigationDBEntities db, int userId, int planId, bool isInitial);

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void DeletePlan(TargetNavigationDBEntities db, int userId, int planId);

        /// <summary>
        /// 转办计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        ///  <param name="responsibleUser"></param>
        /// <param name="confirmUser"></param>
        void TurnPlan(TargetNavigationDBEntities db, int userId, int planId, int responsibleUser, int confirmUser);

        /// <summary>
        /// 申请修改计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void AlterPlan(TargetNavigationDBEntities db, int userId, int planId);

        /// <summary>
        /// 申请中止计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void StopPlan(TargetNavigationDBEntities db, int userId, int planId);

        /// <summary>
        /// 重新开始计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void RestartPlan(TargetNavigationDBEntities db, int userId, int planId);

        /// <summary>
        /// 提交确认
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void SubmitConfirmPlan(TargetNavigationDBEntities db, int userId, PlanOperateModel planOperate);

        /// <summary>
        /// 更新计划进度
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="progress">计划进度</param>
        /// <returns></returns>
        void UpdateProgress(TargetNavigationDBEntities db, int userId, int planId, int progress);

        /// <summary>
        /// 审批计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ApprovePlan(TargetNavigationDBEntities db, int userId, PlanOperateModel planOperate);

        /// <summary>
        /// 确认计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ConfirmPlan(TargetNavigationDBEntities db, int userId, PlanOperateModel planOperate);

        /// <summary>
        /// 撤销计划上次操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="status"></param>
        void RevokePlan(TargetNavigationDBEntities db, int userId, int planId);

        #endregion 一般计划

        #region 循环计划

        /// <summary>
        /// 取得用户最新循环计划信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        LoopPlanInfoModel GetLatestLoopPlanInfo(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 取得我的循环计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        PlanStatusCountModel GetMyLoopPlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null);

        /// <summary>
        /// 取得下属循环计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        PlanStatusCountModel GetSubordinateLoopPlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null);

        /// <summary>
        /// 取得我的循环计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<LoopPlanInfoModel> GetMyLoopPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo);

        /// <summary>
        /// 取得我的循环计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PlanGroupInfoModel> GetMyLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得下属循环计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<LoopPlanInfoModel> GetSubordinateLoopPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo);

        /// <summary>
        /// 取得下属循环计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PlanGroupInfoModel> GetSubordinateLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得协作循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<LoopPlanInfoModel> GetCooperationLoopPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo);

        /// <summary>
        /// 取得协作循环计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PlanGroupInfoModel> GetCooperationLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 添加循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        /// <returns>循环计划ID</returns>
        int InsLoopPlanInfo(TargetNavigationDBEntities db, LoopPlanInfoModel loopPlanInfo);

        /// <summary>
        /// 更新循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        void UpdLoopPlanInfo(TargetNavigationDBEntities db, LoopPlanInfoModel loopPlanInfo);

        /// <summary>
        /// 根据循环计划ID取得详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        LoopPlanInfoModel GetLoopPlanInfoById(TargetNavigationDBEntities db, int loopId);

        /// <summary>
        /// 添加循环计划完成信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="submitInfo"></param>
        /// <returns></returns>
        int InsLoopPlanSubmitInfo(TargetNavigationDBEntities db, LoopPlanSubmitInfoModel submitInfo);

        /// <summary>
        /// 添加循环计划附件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachInfo"></param>
        /// <returns>附件ID</returns>
        int InsLoopPlanAttachInfo(TargetNavigationDBEntities db, FileInfoModel attachInfo);

        /// <summary>
        /// 删除循环计划附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachId"></param>
        void DelLoopPlanAttachInfoByAttachId(TargetNavigationDBEntities db, int attachId);

        /// <summary>
        /// 根据循环计划提交ID删除附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="submitId"></param>
        void DelLoopPlanAttachInfoBySubmitId(TargetNavigationDBEntities db, int submitId);

        /// <summary>
        /// 根据循环计划ID删除附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        void DelLoopPlanAttachInfoByLoopId(TargetNavigationDBEntities db, int loopId);

        /// <summary>
        /// 根据循环计划ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        /// <returns></returns>
        List<FileInfoModel> GetLoopPlanAttachInfoByLoopId(TargetNavigationDBEntities db, int loopId);

        /// <summary>
        /// 根据循环计划提交ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="submitId"></param>
        /// <returns></returns>
        List<FileInfoModel> GetLoopPlanAttachInfoBySubmitId(TargetNavigationDBEntities db, int submitId);

        /// <summary>
        /// 根据文件ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        FileInfoModel GetLoopPlanAttachInfoByFileId(TargetNavigationDBEntities db, int fileId);

        /// <summary>
        /// 提交循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void SubmitLoopPlan(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 删除循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void DeleteLoopPlan(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 转办循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        ///  <param name="responsibleUser"></param>
        /// <param name="confirmUser"></param>
        void TurnLoopPlan(TargetNavigationDBEntities db, int userId, int loopId, int responsibleUser, int confirmUser);

        /// <summary>
        /// 申请修改循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void AlterLoopPlan(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 申请中止循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void StopLoopPlan(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 重新开始循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void RestartLoopPlan(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 循环计划提交确认
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopPlanOperate"></param>
        void SubmitConfirmLoopPlan(TargetNavigationDBEntities db, int userId, LoopPlanOperateModel loopPlanOperate);

        /// <summary>
        /// 审批循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ApproveLoopPlan(TargetNavigationDBEntities db, int userId, LoopPlanOperateModel loopPlanOperate);

        /// <summary>
        /// 确认循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        void ConfirmLoopPlan(TargetNavigationDBEntities db, int userId, LoopPlanOperateModel loopPlanOperate);

        /// <summary>
        /// 撤销循环计划上次操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void RevokeLoopPlan(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 撤销循环计划完成情况的上次操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="submitId"></param>
        void RevokeLoopPlanSubmit(TargetNavigationDBEntities db, int userId, int submitId);

        /// <summary>
        /// 取得循环计划提交信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        /// <returns></returns>
        List<LoopPlanSubmitInfoModel> GetLoopPlanSubmitInfo(TargetNavigationDBEntities db, int loopId, int pageNum);

        /// <summary>
        /// 生成循环计划空白提交信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="generateNumber"></param>
        /// <returns></returns>
        List<LoopPlanSubmitInfoModel> GenerateLoopPlanEmptySubmitInfo(EnumDefine.LoopPlanLoopType type, DateTime startTime, int generateNumber);


        #endregion 循环计划
    }
}