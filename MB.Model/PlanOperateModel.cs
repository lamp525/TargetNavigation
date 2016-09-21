using System;

namespace MB.Model
{
    //计划日志模型
    public class PlanOperateModel
    {
        public int operateId { get; set; }
        public int planId { get; set; }
        public string message { get; set; }
        public Nullable<int> type { get; set; }
        public Nullable<int> userId { get; set; }
        public string user { get; set; }
        public Nullable<DateTime> time { get; set; }
        public string timeNew { get; set; }
    }
}