using MB.New.Common;
using System;

namespace MB.Facade.Index
{
    /// <summary>
    /// 计划信息
    /// </summary>
    public class PagePlanInfoModel
    {
        public int planId { get; set; }
        public int loopId { get; set; }
        public bool isLoop { get; set; }
        public EnumDefine.PlanStatus status { get; set; }
        public string executionMode { get; set; }
        public string eventOutput { get; set; }
        public int? responsibleUser { get; set; }
        public string responsibleUserName { get; set; }
        public string responsibleUserImage { get; set; }
        public int? confirmUser { get; set; }
        public string confirmUserName { get; set; }
        public string confirmUserImage { get; set; }
        public int? importance { get; set; }
        public int? urgency { get; set; }
        public int? difficulty { get; set; }
        public int? progress { get; set; }
        public EnumDefine.PlanStopStatus stop { get; set; }
        public DateTime? endTime { get; set; }
        public int? initial { get; set; }
        public bool? isAttach { get; set; }
    }

    /// <summary>
    /// 计划状态统计
    /// </summary>
    public class PagePlanStatusCountModel
    {
        /// <summary>
        /// 一般和循环计划待提交数量
        /// </summary>
        public int submitingCount { get; set; }

        /// <summary>
        /// 一般和循环计划待审核数量
        /// </summary>
        public int checkingCount { get; set; }

        /// <summary>
        /// 一般和循环计划进行中数量
        /// </summary>
        public int checkedCount { get; set; }

        /// <summary>
        /// 一般和循环计划待确认数量
        /// </summary>
        public int confirmingCount { get; set; }
    }

    /// <summary>
    /// 工时信息
    /// </summary>
    public class PageWorkTimeInfoModel
    {
        /// <summary>本周有效工时</summary>
        public decimal? weekTotalWorkTime { get; set; }

        /// <summary>本月总有效工时</summary>
        public decimal? monthTotalWorkTime { get; set; }

        /// <summary>昨日有效工时</summary>
        public decimal? yesterdayWorkTime { get; set; }

        /// <summary>本周平均工时</summary>
        public decimal? weekAvgWorkTIme { get; set; }

        /// <summary>头像地址</summary>
        public string imgUrl { get; set; }
    }

    /// <summary>
    /// 工时统计
    /// </summary>
    public class PageWorkTimeStatisticsModel
    {
        public decimal actualWorkTime { get; set; }
        public decimal effectiveWorkTime { get; set; }
        public string workdate { get; set; }
    }

    /// <summary>
    /// 计划完成情况
    /// </summary>
    public class PagePlanCompleteCountModel
    {
        /// <summary>
        /// 计划总数
        /// </summary>
        public int completeCount { get; set; }

        /// <summary>
        /// 未完成数
        /// </summary>
        public int notCompleteCount { get; set; }
    }

    /// <summary>
    /// 绩效排行对象
    /// </summary>
    public class PagePerformanceModel
    {
        public string headImage { get; set; }
        public string userName { get; set; }
        public decimal? workTime { get; set; }
    }

    public class PageIncentiveInfoModel
    {
        /// <summary> 奖励金额</summary>
        public decimal rewardAmount { get; set; }

        /// <summary>扣罚金额</summary>
        public decimal punishmentAmount { get; set; }
    }
}