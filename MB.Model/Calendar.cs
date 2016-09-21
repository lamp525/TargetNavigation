using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class Calendar
    {
        public int? calendarId { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public string place { get; set; }
        public string comment { get; set; }
        public string tag { get; set; }
        public string FstartTime { get; set; }
        public string FendTime { get; set; }
        public List<string> partnerName { get; set; }
        public List<int> partner { get; set; }
        public int UserId { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
    }
}