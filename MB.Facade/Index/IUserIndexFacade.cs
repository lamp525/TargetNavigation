using MB.New.Common;
using System.Collections.Generic;

namespace MB.Facade.Index
{
    public interface IUserIndexFacade
    {
        /// <summary>
        /// 取得用户工作信息
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <returns></returns>
        PageWorkTimeInfoModel GetUserWorkTimeInfo(int userId);

        /// <summary>
        /// 取得激励信息
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        PageIncentiveInfoModel GetUserIncentiveInfo(int userId, EnumDefine.StatisticsType statisticsType);

        /// <summary>
        /// 用户工时统计信息（折线图）
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        List<PageWorkTimeStatisticsModel> GetUserWorkTimeStatistics(int userId, EnumDefine.StatisticsType statisticsType);

        /// <summary>
        /// 取得绩效排行信息
        /// </summary>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        List<PagePerformanceModel> GetPerformanceRankInfo(EnumDefine.StatisticsType statisticsType);

        /// <summary>
        /// 用户计划完成情况
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        PagePlanCompleteCountModel GetUserPlanCompleteInfo(int userId, EnumDefine.StatisticsType statisticsType);

        /// <summary>
        ///用户计划状态统计
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        PagePlanStatusCountModel GetUserPlanStatusInfo(int userId, EnumDefine.StatisticsType statisticsType);

        /// <summary>
        /// 取得计划列表信息
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <param name="planListType"></param>
        /// <returns></returns>
        List<PagePlanInfoModel> GetNeedToDoPlanList(int userId, EnumDefine.PlanListType planListType);
    }
}