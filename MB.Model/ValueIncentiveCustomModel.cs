namespace MB.Model
{
    public class ValueIncentiveCustomModel
    {
        public int? customId { get; set; }
        public int? customType { get; set; }
        public decimal? customStartTime { get; set; }
        public decimal? customEndTime { get; set; }
        public int? deductionMode { get; set; }
        public decimal? deductionNum { get; set; }

        public int[] deleteId { get; set; }
    }
}