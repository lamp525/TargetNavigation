using System;

namespace MB.Model
{
    public class MeetingInfo
    {
        /// <summary>会议Id</summary>
        public int meetingId { get; set; }

        /// <summary>开始时间 </summary>
        public DateTime startTime { get; set; }

        /// <summary>结束时间</summary>
        public DateTime endTime { get; set; }

        /// <summary>会议事项</summary>
        public string content { get; set; }

        /// <summary>主讲人</summary>
        public string[] speakerName { get; set; }

        /// <summary>参与会议人员</summary>
        public string[] participantUser { get; set; }
    }
}