using System;

namespace MB.Model
{
    public class MyAppointmentCondition
    {
        /// <summary>开始时间</summary>
        public DateTime? start { get; set; }

        /// <summary>结束时间</summary>
        public DateTime? end { get; set; }

        /// <summary>0：全部 1：未进行 2：已完成</summary>
        public int status { get; set; }
    }
}