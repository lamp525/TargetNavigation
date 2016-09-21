using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class ImMessageModel
    {
        /// <summary>消息ID</summary>
        public string messageId { get; set; }

        /// <summary>消息类型</summary>
        public int type { get; set; }

        /// <summary>会话ID</summary>
        public int? groupId { get; set; }

        /// <summary>发送用户</summary>
        public UserModel sendUser { get; set; }

        /// <summary>接收用户/summary>
        public List<UserModel> receiveUser { get; set; }

        /// <summary>消息</summary>
        public string message { get; set; }

        /// <summary>文件名</summary>
        public string fileName { get; set; }

        /// <summary>发送时间</summary>
        public DateTime sendTime { get; set; }
    }

    public class UserModel
    {
        /// <summary>用户ID</summary>
        public int userId { get; set; }

        /// <summary>用户名称</summary>
        public string userName { get; set; }

        /// <summary>头像</summary>
        public string headImage { get; set; }
    }

    public class TypeCountModel
    {
        /// <summary>常用联系人数</summary>
        public int contactCount { get; set; }

        /// <summary>群组数</summary>
        public int groupCount { get; set; }

        /// <summary>用户数</summary>
        public int userCount { get; set; }

        /// <summary>最近会话数</summary>
        public int conversationCount { get; set; }
    }
}