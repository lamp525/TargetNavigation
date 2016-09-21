using MB.New.Common;
using System.Collections.Generic;

namespace MB.Facade.Plan
{
    public interface IPlanFacade
    {
        #region 新建画面绑定

        /// <summary>
        /// 取得默认计划信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PagePlanDefaultInfoModel GetDefaultPlanInfo(int userId);

        /// <summary>
        /// 取得默认循环计划信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PageLoopPlanDefaultInfoModel GetDefaultLoopPlanInfo(int userId);

        /// <summary>
        /// 取得执行方法列表信息
        /// </summary>
        /// <returns></returns>
        List<PageExecutionInfoModel> GetExecutionInfo();

        /// <summary>
        /// 取得用户常用标签
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string[] GetMostUsedTag(int userId);

        #endregion 新建画面绑定

        #region 计划（新建、更新和详情）

        /// <summary>
        /// 快捷新建计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quickAddPlan"></param>
        /// <returns></returns>
        bool QuickAddPlan(int userId, PageQuickAddModel quickAddPlan);

        /// <summary>
        /// 新建/更新计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planInfo"></param>
        /// <returns></returns>
        void AddOrUpdatePlan(int userId, PagePlanInfoModel planInfo);

        /// <summary>
        /// 新建/更新循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopPlanInfo"></param>
        /// <returns></returns>
        void AddOrUpdateLoopPlan(int userId, PageLoopPlanInfoModel loopPlanInfo);

        /// <summary>
        /// 取得计划详情
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        PagePlanInfoModel GetPlanInfo(int planId);

        /// <summary>
        /// 取得循环计划详情
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        PageLoopPlanInfoModel GetLoopPlanInfo(int loopId);

        /// <summary>
        /// 取得循环计划提交信息
        /// </summary>
        /// <param name="loopId"></param>
        /// <param name="mode"></param>
        /// <param name="pageNum"></param>       
        /// <returns></returns>
        List<PageLoopPlanSubmitInfoModel> GetLoopPlanSubmitInfo(int loopId, EnumDefine.rollMode mode, int pageNum);

        #endregion 计划（新建、更新和详情）

        #region 操作日志

        /// <summary>
        /// 取得一般计划操作日志
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        List<PagePlanLogInfoModel> GetPlanLogInfo(int planId);

        /// <summary>
        /// 取得循环计划操作日志
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        List<PagePlanLogInfoModel> GetLoopPlanLogInfo(int loopId);

        #endregion 操作日志

        #region 计划评论

        /// <summary>
        /// 取得计划评论信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        List<PagePlanCommentModel> GetPlanCommentInfo(int planId);

        /// <summary>
        /// 添加计划评论
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commentInfo"></param>
        void AddPlanComment(int userId, PagePlanCommentModel commentInfo);

        #endregion 计划评论

        #region 我的计划

        /// <summary>
        /// 取得我的计划状态数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetMyPlanStatusInfo(int userId);

        /// <summary>
        /// 取得我的计划列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PagePlanInfoModel> GetMyPlanList(int userId, PagePlanSearchModel searchInfo);

        /// <summary>
        /// 取得我的循环计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PageLoopPlanInfoModel> GetMyLoopPlanList(int userId, PagePlanSearchModel searchInfo);

        /// <summary>
        /// 取得我的一般计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PagePlanGroupInfoModel> GetMyPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得我的循环计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PagePlanGroupInfoModel> GetMyLoopPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得我的月有效工时信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PageMonthWorkTimeModel GetMyMonthWorkTimeInfo(int userId, string yearMonth);

        /// <summary>
        /// 取得我的月计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetMyPlanPieChartInfo(int userId, string yearMonth);

        /// <summary>
        /// 取得我的月循环计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetMyLoopPlanPieChartInfo(int userId, string yearMonth);

        #endregion 我的计划

        #region 下属计划

        /// <summary>
        /// 取得下属计划状态数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PagePlanStatusCountModel GetSubordinatePlanStatusInfo(int userId);

        /// <summary>
        /// 取得下属计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PagePlanInfoModel> GetSubordinatePlanList(int userId, PagePlanSearchModel searchInfo);

        /// <summary>
        /// 取得下属循环计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PagePlanInfoModel> GetSubordinateLoopPlanList(int userId, PagePlanSearchModel searchInfo);

        /// <summary>
        /// 取得下属一般计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PagePlanGroupInfoModel> GetSubordinatePlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得下属循环计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PagePlanGroupInfoModel> GetSubordinateLoopPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得下属月有效工时信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PageMonthWorkTimeModel GetSubordinateMonthWorkTimeInfo(int userId, string yearMonth);

        /// <summary>
        /// 取得下属月计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetSubordinatePlanPieChartInfo(int userId, string yearMonth);

        /// <summary>
        /// 取得下属月循环计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetSubordinateLoopPlanPieChartInfo(int userId, string yearMonth);

        #endregion 下属计划

        #region 协作计划

        /// <summary>
        /// 取得协作计划状态数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PagePlanStatusCountModel GetCooperationPlanStatusInfo(int userId);

        /// <summary>
        /// 取得协作计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PagePlanInfoModel> GetCooperationPlanList(int userId, PagePlanSearchModel searchInfo);

        /// <summary>
        /// 取得协作循环计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        List<PagePlanInfoModel> GetCooperationLoopPlanList(int userId, PagePlanSearchModel searchInfo);

        /// <summary>
        /// 取得协作一般计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PagePlanGroupInfoModel> GetCooperationPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得协作循环计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        List<PagePlanGroupInfoModel> GetCooperationLoopPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType);

        /// <summary>
        /// 取得月协作计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetCooperationPlanPieChartInfo(int userId, string yearMonth);

        /// <summary>
        /// 取得月协作循环计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        PagePlanStatusCountModel GetCooperationLoopPlanPieChartInfo(int userId, string yearMonth);

        #endregion 协作计划
    }
}