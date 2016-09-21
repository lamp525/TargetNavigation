namespace MB.Model
{
    public class CompleteStatisticsModel
    {
        public int userId { get; set; }
        public int organizationId { get; set; }
        public string statisticalTime { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int allCount { get; set; }

        /// <summary>
        //已完成数量
        /// </summary>
        public int completeCount { get; set; }

        /// <summary>
        /// 待确认数量
        /// </summary>
        public int confirmedCount { get; set; }

        /// <summary>
        /// 未完成数量
        /// </summary>
        public int notCompleteCount { get; set; }

        /// <summary>
        /// 审核中数量
        /// </summary>
        public int examineCount { get; set; }

        /// <summary>
        /// 待提交数量
        /// </summary>
        public int submitCount { get; set; }

        /// <summary>
        /// 已终止数量
        /// </summary>
        public int stopCount { get; set; }

        /// <summary>
        /// 超时数量
        /// </summary>
        public int timeOut { get; set; }
    }
}