using System.Collections.Generic;

namespace MB.Model
{
    public class ConditionModel
    {
        public int[] status { get; set; }
        public int[] stop { get; set; }
        public string[] time { get; set; }
        public int[] person { get; set; }
        public int[] department { get; set; }
        public int[] project { get; set; }
        public int whoPlan { get; set; }
        public int[] soontype { get; set; }
        public List<Sort> sorts { get; set; }
    }

    public class Sort
    {
        //排序的字段
        public int type { get; set; }

        //降序还是升序：1、升序 0、降序
        public int direct { get; set; }
    }
}