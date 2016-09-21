namespace MB.Model
{
    public class IncentiveDataModel
    {
        public string strname { get; set; }

        /// <summary>
        /// 月份或日期
        /// </summary>
        public int name { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int? count { get; set; }

        /// <summary>
        /// 完成数
        /// </summary>
        public int? completeCount { get; set; }

        /// <summary>
        /// 超时数
        /// </summary>
        public int? timeoutCount { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>
        public decimal? completeRate { get; set; }

        /// <summary>
        /// 超时率
        /// </summary>
        public decimal? timeoutRate { get; set; }
    }
}