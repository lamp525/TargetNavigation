using System;

namespace MB.Model
{
    public class PlanSearchResultModel
    {
        /// <summary>计划ID</summary>
        public int planId { get; set; }

        /// <summary>执行方式</summary>
        public string executionMode { get; set; }

        /// <summary>输出对象</summary>
        public string eventOutput { get; set; }

        /// <summary>重要度</summary>
        public int? importance { get; set; }

        /// <summary>紧急度</summary>
        public int? urgency { get; set; }

        /// <summary>难易度</summary>
        public int? difficulty { get; set; }

        ///<summary>状态</summary>
        public int status { get; set; }

        ///<summary>进度</summary>
        public int? progress { get; set; }

        /// <summary>结束时间</summary>
        public DateTime? endTime { get; set; }

        /// <summary>责任人</summary>
        public string responsibleUserName { get; set; }

        public int responsibleUserId { get; set; }

        /// <summary>确认人 </summary>
        public string confirmUserName { get; set; }

        public int confirmUserId { get; set; }

        ///<summary>创建人</summary>
        public string createUserName { get; set; }

        public int createUserId { get; set; }

        /// <summary>创建时间</summary>
        public DateTime createTime { get; set; }

        /// <summary>生成时间</summary>
        public DateTime? generateTime { get; set; }

        //是否是协作计划：1、是 0、否
        public int IsCollPlan { get; set; }

        //是否是循环计划:1、是 0、否
        public int isLoopPlan { get; set; }

        //是否是前提计划
        public int isFronPlan { get; set; }

        //是否是下属计划
        public bool isSubordinatePlan { get; set; }

        //中止状态
        public int stop { get; set; }

        //初始计划 0：临时计划 1：目标计划
        public int? initial { get; set; }
    }
}