using System;
using System.Collections.Generic;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface IPlanStatisticsBLL
    {
        /// 添加统计信息
        StatisticsInfo AddStatisticsInfo(StatisticsInfo stainfo);

        /// 按部门号获取该部门成员的计划完成情况
        List<PlanStatisticsModel> GetPlanStatistics(int organizationId, DateTime startTime, DateTime endTime, int sortby, int sort);

        /// 按统计号获取各部门计划完成情况
        List<PlanStatusByOrgModel> GetStatusByOrg(int statisticsId, DateTime startTime, DateTime endTime, int sortby, int sortDirect);

        /// 初始化一个状态统计list
        List<Plan> AddPlanModel(List<Plan> tempList);

        /// 获取统计名信息
        List<StatisticsInfo> GetStatisticsList();

        /// 递归获取所有的相关部门Id
        List<int> GetAllorganizationIds(int organizationId, TargetNavigationDBEntities db);
    }
}