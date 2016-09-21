namespace MB.Model
{
    public class UnderReward
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 实际数
        /// </summary>
        public int actualNum { get; set; }

        /// <summary>
        /// 应得数
        /// </summary>
        public int deservedNum { get; set; }

        public bool isParent { get; set; }
    }
}