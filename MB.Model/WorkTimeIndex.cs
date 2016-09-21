using System;

namespace MB.Model
{
    public class WorkTimeIndex
    {
        public Nullable<decimal> totalwork { get; set; }
        public Nullable<decimal> totaleffective { get; set; }
        public Nullable<int> orgid { get; set; }
        public string orgname { get; set; }
        public Nullable<int> workdate { get; set; }
    }
}