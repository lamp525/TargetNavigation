using System.Collections.Generic;

namespace MB.Model
{
    public class JsonModelIndex
    {
        public List<WorkTimeIndex> top10 { get; set; }
        public List<Worktime>[] top3 { get; set; }
    }
}