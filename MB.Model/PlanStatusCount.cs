using System;

namespace MB.Model
{
    public class PlanStatusCount
    {
        public Nullable<int> status { get; set; }
        public Nullable<int> stop { get; set; }
        public string statusName { get; set; }
        public int statusCount { get; set; }
    }
}