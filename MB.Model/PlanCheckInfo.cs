using System;

namespace MB.Model
{
    //计划审核需要的信息
    public class PlanCheckInfo
    {
        public int planId { get; set; }

        public Nullable<int> status { get; set; }

        public Nullable<int> stop { get; set; }

        public Nullable<int> importance { get; set; }

        public Nullable<int> urgency { get; set; }

        public Nullable<int> difficulty { get; set; }

        public Nullable<int> time { get; set; }

        public Nullable<int> quantity { get; set; }
    }
}