using System;

namespace MB.Model
{
    public class PlanSubmitModel
    {
        public int planId { get; set; }

        public int? organizationId { get; set; }

        public int? status { get; set; }

        public int? userId { get; set; }

        public int? stop { get; set; }

        public DateTime? endTime { get; set; }

        public DateTime? updateTime { get; set; }
    }
}