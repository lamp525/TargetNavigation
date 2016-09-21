using System.Collections.Generic;

namespace MB.Model
{
    public class ValueDelayRuleModel
    {
        public ValueIncentiveModel value { get; set; }
        public List<ValueIncentiveCustomModel> custom { get; set; }
        public int[] DeleteId { get; set; }
    }
}