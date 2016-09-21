using System;

namespace MB.Model
{
    /// <summary>
    /// 个人文档共享表
    /// </summary>
    public class DocumentShared
    {
        /// <summary>
        /// 文档ID
        /// </summary>
        public int documentId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 分享人名字
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 文档表示名
        /// </summary>
        public string disdisplayName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public Nullable<DateTime> creatTime { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public bool deleteFlg { get; set; }
    }
}