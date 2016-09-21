using System;

namespace MB.New.Model
{
    /// <summary>
    /// 计划评论
    /// </summary>
    public class PlanCommentModel
    {
        /// <summary>
        /// 意见ID
        /// </summary>
        public int? suggestionId { get; set; }

        /// <summary>
        /// 计划ID
        /// </summary>
        public int? planId { get; set; }

        /// <summary>
        /// 意见
        /// </summary>
        public string suggestion { get; set; }

        /// <summary>
        /// 回复人
        /// </summary>
        public int? replyUser { get; set; }

        /// <summary>
        /// 回复人名
        /// </summary>
        public string replyUserName { get; set; }

        /// <summary>
        /// 回复人头像
        /// </summary>
        public string replyUserImage { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int? createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int? updateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? updateTime { get; set; }
    }
}