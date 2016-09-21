using System;

namespace MB.Model
{
    public class UserIndexModel
    {
        //重要度
        public int? importance { get; set; }

        //紧急度
        public int? urgency { get; set; }

        //完成情况
        public string progress { get; set; }

        //输出结果
        public string eventOutput { get; set; }

        //计划完成时间
        public DateTime? endTime { get; set; }

        //计划Id
        public int planId { get; set; }

        //时间差
        public int TimeNum { get; set; }

        //
        public int? status { get; set; }

        //
        public int? stop { get; set; }

        //
        public int isloop { get; set; }

        public int withfront { get; set; }

        public int collPlan { get; set; }

        public int? initial { get; set; }
    }
}