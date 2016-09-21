using System;

namespace MB.New.Model
{
    public class NewsSearchResultModel
    {
        /// <summary>新闻Id</summary>
        public int newId { get; set; }

        /// <summary>新闻标题</summary>
        public string title { get; set; }

        /// <summary>分类名称</summary>
        public string directoryName { get; set; }

        /// <summary>创建用户名 </summary>
        public string createUserName { get; set; }

        /// <summary>创建时间</summary>
        public DateTime createTime { get; set; }

        /// <summary>更新时间 </summary>
        public DateTime updateTime { get; set; }

        /// <summary>通知/新闻标志 True：通知 False：新闻</summary>
        public bool? isNotice { get; set; }
    }
}