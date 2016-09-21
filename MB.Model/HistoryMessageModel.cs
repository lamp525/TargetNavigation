using System;

namespace MB.Model
{
    public class MessageFilterModel
    {
        public int type { get; set; }
        public int id { get; set; }
        public int? page { get; set; }
        public string time { get; set; }
        public string message { get; set; }
    }

    public class HistoryMessageModel
    {
        public int? type { get; set; }

        /// <summary>发送者ID</summary>
        public int userId { get; set; }

        /// <summary>发送者名</summary>
        public string userName { get; set; }

        /// <summary>头像</summary>
        public string headImage { get; set; }

        /// <summary>消息</summary>
        public string message { get; set; }

        /// <summary>文件名</summary>
        public string fileName { get; set; }

        /// <summary>发送时间</summary>
        public DateTime sendTime { get; set; }
    }
}