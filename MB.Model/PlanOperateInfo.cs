using System;

namespace MB.Model
{
    public class PlanOperateInfo
    {
        public int operateId { get; set; }
        public int planId { get; set; }
        public string message { get; set; }
        public Nullable<int> result { get; set; }
        public Nullable<int> reviewUser { get; set; }
        public Nullable<DateTime> reviewTime { get; set; }
    }
}