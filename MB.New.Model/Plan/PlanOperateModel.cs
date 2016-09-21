namespace MB.New.Model
{
    public class PlanOperateModel
    {
        /// <summary>
        /// 计划Id
        /// </summary>
        public int planId { get; set; }

        /// <summary>
        /// 计划状态
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 中止状态
        /// </summary>
        public int? stop { get; set; }

        /// <summary>
        /// 临时计划标志
        /// </summary>
        public bool isInitial { get; set; }

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
        /// 是否通过
        /// </summary>
        public bool isApprove { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        ///完成数量
        /// </summary>
        public decimal? completeQuantity { get; set; }

        /// <summary>
        /// 完成质量
        /// </summary>
        public decimal? completeQuality { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public decimal? completeTime { get; set; }

        /// <summary>
        /// 完成质量(自评)
        /// </summary>
        public int? quantity { get; set; }

        /// <summary>
        /// 完成时间（自评）
        /// </summary>
        public int? time { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        public int? progress { get; set; }

        /// <summary>
        /// 提交结果内容
        /// </summary>
        public string result { get; set; }
    }
}