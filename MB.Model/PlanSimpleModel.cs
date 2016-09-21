using System;

namespace MB.Model
{
    public class PlanSimpleModel
    {
        /// <summary>计划ID</summary>
        public int planId { get; set; }

        /// <summary>执行方式</summary>
        public string executionMode { get; set; }

        /// <summary>输出对象</summary>
        public string eventOutput { get; set; }

        /// <summary>责任人</summary>
        public string responsibleUser { get; set; }

        /// <summary>头像</summary>
        public string smallImage { get; set; }

        /// <summary>确认人 </summary>
        public string confirmUser { get; set; }

        /// <summary>结束时间</summary>
        public DateTime? endTime { get; set; }

        /// <summary>创建时间</summary>
        public DateTime createTime { get; set; }
    }
}