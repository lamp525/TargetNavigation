using System;

namespace MB.Model
{
    public class NewsDirModel
    {
        /// <summary>目录ID</summary>
        public int? directoryId { get; set; }

        /// <summary>目录名</summary>
        public string directoryName { get; set; }

        /// <summary>父目录ID</summary>
        public int? parentDirectory { get; set; }

        /// <summary>排序</summary>
        public int orderNum { get; set; }

        /// <summary> 更新时间 </summary>
        public DateTime createTime { get; set; }

        /// <summary> 修改时间 </summary>
        public DateTime updateTime { get; set; }

        /// <summary>  创建用户 </summary>
        public int createUser { get; set; }

        /// <summary> 修改用户</summary>
        public int updateUser { get; set; }

        /// <summary>
        /// tree使用
        /// </summary>
        public int? id { get; set; }

        public string name { get; set; }
        public bool isParent { get; set; }

        //禁止子节点拖拽出去
        public bool childOuter { get; set; }

        //分类下有无新闻/通知
        public bool isNew { get; set; }
    }
}