namespace MB.Model
{
    public class PerWorkTimeModel
    {
        /// <summary>用户ID</summary>
        public int id { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 有效工时和
        /// </summary>
        public decimal effectiveTimeSum { get; set; }

        /// <summary>
        /// 实际工时和
        /// </summary>
        public decimal workTimeSum { get; set; }
    }
}