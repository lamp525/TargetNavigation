using System;

namespace MB.Facade.Calendar
{
    public class PageCalendarInfoModel
    {
        public int? calendarId { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string place { get; set; }
        public string comment { get; set; }
        public int createUserId { get; set; }
        public string createUserName { get; set; }
        public DateTime createTime { get; set; }
        public int updateUserId { get; set; }
        public DateTime updateTime { get; set; }
    }

    public class PageCalendarCountModel
    {
        public DateTime date { get; set; }
        public bool isZero { get; set; }
    }
}