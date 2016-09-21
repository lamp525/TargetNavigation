using System;

namespace MB.Model
{
    public class WorkTimeModel
    {
        /// <summary>循环计划提交ID</summary>
        public int? submitId { get; set; }

        /// <summary>计划ID</summary>
        public int? planId { get; set; }

        /// <summary>用户ID</summary>
        public int userId { get; set; }

        /// <summary>组织架构ID</summary>
        public int orgId { get; set; }

        /// <summary>提交时间</summary>
        public DateTime? submitTime { get; set; }

        /// <summary>确认时间</summary>
        public DateTime? confirmTime { get; set; }

        /// <summary>有效工时</summary>
        public decimal? effectiveTime { get; set; }

        /// <summary>实际工时</summary>
        public decimal? workTime { get; set; }

        /// <summary>统计标识</summary>
        public int haveStatistics { get; set; }
    }
}