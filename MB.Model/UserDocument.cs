using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class UserDocument
    {
        /// <summary>
        /// 文档ID
        /// </summary>
        public int documentId { get; set; }

        /// <summary>
        /// 上级文件夹
        /// </summary>
        public Nullable<int> folder { get; set; }

        /// <summary>
        /// 文件目录
        /// </summary>
        public string folderName { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 存储名
        /// </summary>
        public string saveName { get; set; }

        /// <summary>
        /// 后缀名
        /// </summary>
        public string extension { get; set; }

        /// <summary>
        /// 归档标志
        /// </summary>
        public Nullable<bool> archive { get; set; }

        /// <summary>
        /// archiveTime
        /// </summary>
        public Nullable<System.DateTime> archiveTime { get; set; }

        /// <summary>
        /// 共享标志
        /// </summary>
        public Nullable<bool> withShared { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Nullable<bool> isFolder { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int updateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool deleteFlag { get; set; }

        /// <summary>
        /// 个人文档共享表
        /// </summary>
        public List<DocumentShared> documentSharedList { get; set; }
    }
}