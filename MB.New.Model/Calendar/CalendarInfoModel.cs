using System;

namespace MB.New.Model
{
    public class CalendarInfoModel
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
}