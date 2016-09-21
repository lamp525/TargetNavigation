using System;

namespace MB.Model
{
    public class CalendarPlanModel
    {
        /// <summary>日程类别： 1：我的日程 2：下属日程</summary>
        public int calendarType { get; set; }

        /// <summary>日期 </summary>
        public DateTime date { get; set; }

        /// <summary>状态</summary>
        public int? status { get; set; }
    }
}