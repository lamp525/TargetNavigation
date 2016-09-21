using System;

namespace MB.Model
{
    public class NewsInfo
    {
        //新闻Id
        public int newId { get; set; }

        //新闻标题
        public string title { get; set; }

        //新闻内容
        public string contents { get; set; }

        //图片链接地址
        public string titleImage { get; set; }

        //新闻创建时间
        public DateTime createTime { get; set; }

        //通知/新闻 True：通知 False：新闻
        public bool? notice { get; set; }

        //摘要
        public string summary { get; set; }

        //查看次数
        public int? viewNum { get; set; }

        //作者ID
        public int createUser { get; set; }

        //作者
        public string UserName { get; set; }

        //转换时间
        public string FCreatTime { get; set; }

        //类型
        public int? directoryId { get; set; }

        //分类名称
        public string directoryName { get; set; }
    }
}