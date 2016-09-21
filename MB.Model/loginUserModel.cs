namespace MB.Model
{
    public class loginUserModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string stationName { get; set; }
        public string bigImage { get; set; }
        public int todayUnfinished { get; set; }
        public int todayPlanTotal { get; set; }
        public int overTimePlan { get; set; }
        public string phone { get; set; }

        /// <summary>未完成流程数量</summary>
        public int unCompleteFlowCount { get; set; }
    }
}