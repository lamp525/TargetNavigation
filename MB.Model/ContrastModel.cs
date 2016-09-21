namespace MB.Model
{
    public class ContrastModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public int organizationId { get; set; }
        public string orgName { get; set; }
        public int? planCount { get; set; }
        public int? completeCount { get; set; }
        public int? timeoutCount { get; set; }
        public string name { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>
        public decimal comcompleteRate { get; set; }

        /// <summary>
        /// 超时率
        /// </summary>
        public decimal timeoutRate { get; set; }
    }
}