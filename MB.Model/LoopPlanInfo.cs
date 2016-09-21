using System;

namespace MB.Model
{
    public class LoopPlanInfo
    {
        //循环计划信息
        public int? loopId { get; set; }

        public Nullable<int> projectId { get; set; }
        public Nullable<int> stationId { get; set; }
        public Nullable<int> organizationId { get; set; }
        public Nullable<int> executionModeId { get; set; }
        public Nullable<int> eventOutputId { get; set; }
        public string eventOutput { get; set; }
        public Nullable<int> responsibleOrganization { get; set; }
        public Nullable<int> responsibleUser { get; set; }
        public Nullable<int> confirmOrganization { get; set; }
        public Nullable<int> confirmUser { get; set; }
        public Nullable<DateTime> startTime { get; set; }
        public Nullable<DateTime> endTime { get; set; }
        public string comment { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> loopType { get; set; }
        public bool loopStatus { get; set; }
        public Nullable<int> importance { get; set; }
        public Nullable<int> urgency { get; set; }
        public Nullable<int> difficulty { get; set; }
        public Nullable<int> unitTime { get; set; }
        public Nullable<decimal> completeQuantity { get; set; }
        public Nullable<decimal> completeQuality { get; set; }
        public Nullable<decimal> completeTime { get; set; }
        public Nullable<DateTime> lastDate { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
        public bool deleteFlag { get; set; }
        public string loopYear { get; set; }
        public string loopMonth { get; set; }
        public string loopWeek { get; set; }
        public string loopDay { get; set; }
        public string loopTime { get; set; }
        public string executionName { get; set; }

        ///<summary>标签</summary>
        public string[] keyword { get; set; }

        //循环操作表
        //public int operateId { get; set; }
        //public int loopId { get; set; }
        //public string message { get; set; }
        //public int result { get; set; }
        //public int reviewUser { get; set; }
        //public DateTime reviewTime { get; set; }
    }
}