using System;

namespace MB.Model
{
    public class NewsinfoModel
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

        //发布状态 True:发布  false:取消发布
        public bool? publish { get; set; }

        //置顶
        public bool? isTop { get; set; }

        //标签
        public string[] keyword { get; set; }

        //修改时间
        public DateTime updateTime { get; set; }

        //修改用户
        public int updateUser { get; set; }

        //新闻/通知分页总条数
        public int pageCount { get; set; }
    }
}