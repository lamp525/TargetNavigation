namespace MB.Model
{
    public class CheckPlanModel
    {
        /// <summary>计划或循环计划Id </summary>
        public int planId { get; set; }

        /// <summary>事项输出结果</summary>
        public string eventOutPut { get; set; }

        /// <summary>责任人名称</summary>
        public string responseName { get; set; }

        /// <summary>是否循环计划</summary>
        public bool isLoopPlan { get; set; }
    }
}