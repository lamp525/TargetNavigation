using System;

namespace MB.Model
{
    public class DocumentModel
    {
        //文档Id
        public int documentId { get; set; }

        //上级文件夹
        public int? folder { get; set; }

        //权限
        public int? power { get; set; }

        //表示名
        public string displayName { get; set; }

        //存储名
        public string saveName { get; set; }

        //后缀名
        public string extension { get; set; }

        //归档
        public bool? archive { get; set; }

        //归档时间
        public DateTime? archiveTime { get; set; }

        //描述
        public string description { get; set; }

        //类型  0、文件  1、文件夹
        public bool? isFolder { get; set; }

        //创建用户
        public int createUser { get; set; }

        public string createUserName { get; set; }

        //创建时间
        public DateTime createTime { get; set; }

        //共享标志 true:共享给他人 false：未共享
        public bool? withShared { get; set; }

        //删除标志
        public bool? deleteFlag { get; set; }

        //上级权限

        public int UpPower { get; set; }

        public string path { get; set; }

        //所属类别 0：个人  1：公司
        public bool? isCompany { get; set; }

        //更新时间
        public DateTime? updateTime { get; set; }
    }
}