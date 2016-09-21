using System;

namespace MB.Model
{
    //评论模型
    public class DiscussModel
    {
        //意见Id
        public int suggestionId { get; set; }

        //计划Id
        public int planId { get; set; }

        //意见
        public string suggestion { get; set; }

        //回复人Id
        public int replyUser { get; set; }

        //回复人姓名
        public string replyUserName { get; set; }

        //创建人Id
        public int createUser { get; set; }

        //创建人姓名
        public string createUserName { get; set; }

        //评论时间
        public DateTime createTime { get; set; }

        //修改格式后的评论时间
        public string NewCreateTime { get; set; }

        //创建人的头像
        public string img { get; set; }
    }
}