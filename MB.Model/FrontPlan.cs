using System;

namespace MB.Model
{
    public class FrontPlan
    {
        //前提计划Id
        public int planId { get; set; }

        //前提计划的事项输出
        public string eventOutput { get; set; }

        //前提计划完成进度
        public Nullable<int> process { get; set; }
    }
}