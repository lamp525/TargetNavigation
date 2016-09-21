using System;

namespace MB.Model
{
    public class FormDuplicateTimeModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public DateTime createStartTime { get; set; }
        public DateTime creatTrueTime { get; set; }
    }
}