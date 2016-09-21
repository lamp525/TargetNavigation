using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class PlanStatisticsModel
    {
        public int? userId { get; set; }

        public string name { get; set; }

        //头像地址
        public string image { get; set; }

        //事项总数
        public int eventTotalCount { get; set; }

        //完成件数
        public int completeCount { get; set; }

        //完成率(百分比都带一位小数）
        public string completeRate { get; set; }

        //待确认件数
        public int unConfirm { get; set; }

        //确认率
        public string confirmRate { get; set; }

        //未完成件数
        public int unCompleteCount { get; set; }

        //未完成率
        public string unCompleteRate { get; set; }

        //审核件数
        public int submitCount { get; set; }

        //审核率
        public string submitRate { get; set; }

        //待提交件数
        public int unCommittedCount { get; set; }

        //提交率
        public string committedRate { get; set; }

        //已中止件数
        public int stopCount { get; set; }

        //中止率
        public string stopRate { get; set; }
    }

    public class PlanStatusByOrgModel
    {
        //部门Id
        public int id { get; set; }

        //部门名
        public string name { get; set; }

        //计划量
        public int planCount { get; set; }

        //完成率
        public double completeRate { get; set; }

        //计划信息
        public List<Plan> plans { get; set; }
    }

    public class Plan
    {
        public int status { get; set; }
        public int count { get; set; }
        public string color { get; set; }
        public string statusName { get; set; }
    }

    public class StatisticsInfo
    {
        //统计Id
        public int statisticsId { get; set; }

        //统计名
        public string statisticsName { get; set; }

        //创建用户
        public int createUser { get; set; }

        //创建时间
        public DateTime createTime { get; set; }

        //修改用户
        public int updateUser { get; set; }

        //修改时间
        public DateTime updateTime { get; set; }

        //部门id集合
        public int[] organizationId { get; set; }
    }
}