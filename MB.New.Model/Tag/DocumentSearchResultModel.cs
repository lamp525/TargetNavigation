using System;

namespace MB.New.Model
{
    public class DocumentSearchResultModel
    {
        /// <summary>文档Id</summary>
        public int documentId { get; set; }

        /// <summary>表示名</summary>
        public string displayName { get; set; }

        /// <summary>存储名 </summary>
        public string saveName { get; set; }

        /// <summary>后缀名</summary>
        public string extension { get; set; }

        /// <summary>所属类别 0：个人  1：公司</summary>
        public bool? isCompany { get; set; }

        /// <summary>创建用户</summary>
        public int createUser { get; set; }

        /// <summary>创建用户名</summary>
        public string createUserName { get; set; }

        /// <summary>创建时间 </summary>
        public DateTime createTime { get; set; }

        /// <summary>更新时间</summary>
        public DateTime? updateTime { get; set; }

        /// <summary>共享标志 true:共享给他人 false：未共享</summary>
        public bool? withShared { get; set; }

        /// <summary>权限</summary>
        public int? power { get; set; }
    }
}