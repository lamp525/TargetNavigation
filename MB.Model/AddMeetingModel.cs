using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class AddMeetingModel
    {
        /// <summary>会议Id</summary>
        public int? meetingId { get; set; }

        /// <summary>会议室Id </summary>
        public int roomId { get; set; }

        /// <summary>开始时间 </summary>
        public DateTime startTime { get; set; }

        /// <summary>结束时间</summary>
        public DateTime endTime { get; set; }

        /// <summary>会议事项 </summary>
        public string content { get; set; }

        /// <summary>主讲人</summary>
        public int[] speechUser { get; set; }

        /// <summary>参加人员</summary>
        public int[] joinUser { get; set; }

        /// <summary>附件集合</summary>
        public List<UploadFileModel> file { get; set; }
    }
}