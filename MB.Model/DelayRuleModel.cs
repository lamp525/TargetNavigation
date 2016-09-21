namespace MB.Model
{
    public class DelayRuleModel
    {
        public int? ruleId { get; set; }
        public int? ruletype { get; set; }
        public decimal? delayStartTime { get; set; }
        public decimal? delayEndTime { get; set; }
        public int? deductionMode { get; set; }
        public decimal? deductionNum { get; set; }
    }
}