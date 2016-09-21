namespace MB.Model
{
    public class UserPlanCountInfo
    {
        //今日未完成计划的数量
        public int todayUnfinished { get; set; }

        //今日未完成计划总数
        public int todayPlanTotal { get; set; }

        //超市计划的数量
        public int overTimePlan { get; set; }
    }
}