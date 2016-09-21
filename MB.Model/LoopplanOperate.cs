using System;

namespace MB.Model
{
    public class LoopplanOperate
    {
        public int operateId { get; set; }
        public int loopId { get; set; }
        public string message { get; set; }
        public Nullable<int> result { get; set; }
        public Nullable<int> reviewUser { get; set; }
        public Nullable<System.DateTime> reviewTime { get; set; }
    }
}