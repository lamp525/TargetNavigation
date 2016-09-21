using System;

namespace MB.Model
{
    public class MeetingModel
    {
        /// <summary>会议Id</summary>
        public int meetingId { get; set; }

        /// <summary>会议室Id</summary>
        public int roomId { get; set; }

        /// <summary>会议室名称</summary>
        public string roomName { get; set; }

        /// <summary>开始时间</summary>
        public DateTime startTime { get; set; }

        /// <summary>结束时间 </summary>
        public DateTime endTime { get; set; }

        /// <summary>内容</summary>
        public string content { get; set; }

        /// <summary>预约人</summary>
        public string createUser { get; set; }

        /// <summary>人数</summary>
        public int? member { get; set; }

        /// <summary>是否已完成  true:已完成 false:未完成</summary>
        public bool isComplete { get; set; }

        /// <summary>true:有附件  false：没有附件 </summary>
        public bool? isHasAttachment { get; set; }
    }
}