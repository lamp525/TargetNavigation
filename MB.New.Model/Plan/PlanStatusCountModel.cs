namespace MB.New.Model
{
    public class PlanStatusCountModel
    {
        /// <summary>
        /// 一般计划待提交数量
        /// </summary>
        public int planSubmitingCount { get; set; }

        /// <summary>
        /// 一般计划待审核数量
        /// </summary>
        public int planCheckingCount { get; set; }

        /// <summary>
        /// 一般计划进行中数量
        /// </summary>
        public int planCheckedCount { get; set; }

        /// <summary>
        /// 一般计划待确认数量
        /// </summary>
        public int planConfirmingCount { get; set; }

        /// <summary>
        /// 一般计划中止数量
        /// </summary>
        public int planStopCount { get; set; }

        /// <summary>
        /// 一般计划完成数量
        /// </summary>
        public int planCompeteCount { get; set; }

        /// <summary>
        /// 循环计划待提交数量
        /// </summary>
        public int loopSubmitingCount { get; set; }

        /// <summary>
        /// 循环计划待审核数量
        /// </summary>
        public int loopCheckingCount { get; set; }

        /// <summary>
        /// 循环计划进行中数量
        /// </summary>
        public int loopCheckedCount { get; set; }

        /// <summary>
        /// 循环计划待确认数量
        /// </summary>
        public int loopConfirmingCount { get; set; }

        /// <summary>
        /// 循环计划中止数量
        /// </summary>
        public int loopStopCount { get; set; }

        /// <summary>
        /// 循环计划已完成数量
        /// </summary>
        public int loopCompletedCount { get; set; }
    }
}