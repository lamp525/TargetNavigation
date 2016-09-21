namespace MB.New.Model
{
    public class LoopPlanOperateModel
    {
        /// <summary>
        /// 提交ID
        /// </summary>
        public int? submitId { get; set; }

        public int unitTime { get; set; }

        /// <summary>
        /// 循环计划Id
        /// </summary>
        public int? loopId { get; set; }

        /// <summary>
        /// 循环计划提交数量
        /// </summary>
        public int? number { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool isApprove { get; set; }

        /// <summary>
        /// 有无附件
        /// </summary>
        public bool? isAttach { get; set; }

        /// <summary>
        /// 重要度
        /// </summary>
        public int? importance { get; set; }

        /// <summary>
        /// 紧急度
        /// </summary>
        public int? urgency { get; set; }

        /// <summary>
        /// 困难度
        /// </summary>
        public int? difficulty { get; set; }

        /// <summary>
        ///完成数量
        /// </summary>
        public int? completeQuantity { get; set; }

        /// <summary>
        /// 完成质量
        /// </summary>
        public int? completeQuality { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public int? completeTime { get; set; }

        /// <summary>
        /// 提交结果内容
        /// </summary>
        public string result { get; set; }
    }
}