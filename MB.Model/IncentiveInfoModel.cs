namespace MB.Model
{
    public class IncentiveInfoModel
    {
        public int type { get; set; }
        public int contrastType { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public int[] userId { get; set; }
        public int[] orgId { get; set; }
    }
}