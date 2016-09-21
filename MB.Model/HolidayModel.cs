using System;

namespace MB.Model
{
    public class HolidayModel
    {
        public int? holidayId { get; set; }
        public int? type { get; set; }
        public DateTime Holiday { get; set; }
        public int creatUserId { get; set; }
        public int updateUserId { get; set; }
        public string creatTime { get; set; }
        public string updateTime { get; set; }
    }
}