using System.Collections.Generic;

namespace MB.Model
{
    public class AddstationManualModel
    {
        public int[] deleteStation { get; set; }

        public List<LoopPlanInfo> loopPlanList { get; set; }

        public int stationId { get; set; }
    }
}