namespace MB.New.Model
{
    public class WorkTimeInfoModel
    {
        /// <summary>本周有效工时</summary>
        public decimal? weekTotalWorkTime { get; set; }

        /// <summary>本月总有效工时</summary>
        public decimal? monthTotalWorkTime { get; set; }

        /// <summary>昨日有效工时</summary>
        public decimal? yesterdayWorkTime { get; set; }

        /// <summary>本周平均工时</summary>
        public decimal? weekAvgWorkTIme { get; set; }

        /// <summary>头像地址</summary>
        public string imgUrl { get; set; }
    }
}