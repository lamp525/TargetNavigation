using System;

namespace MB.Model
{
    public class ConversationModel
    {
        /// <summary>会话类型</summary>
        public int type { get; set; }

        /// <summary> 用户/群组ID </summary>
        public int id { get; set; }

        /// <summary> 会话名称 </summary>
        public string name { get; set; }

        /// <summary> 头像 </summary>
        public string headImage { get; set; }

        /// <summary> 发送时间 </summary>
        public DateTime sendTime { get; set; }

        /// <summary> 在线 </summary>
        public bool onLine { get; set; }

        /// <summary> 岗位 </summary>
        public string station { get; set; }
    }
}