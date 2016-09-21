using System;

namespace MB.Model
{
    //文档日志
    public class DocumentLogModel
    {
        //日志编号
        public int logId { get; set; }

        //文档编号
        public int documentId { get; set; }

        //操作类型  1：下载 2：复制 3：移动 4、新建文件夹 5、删除
        public int type { get; set; }

        //操作描述
        public string comment { get; set; }

        //创建人
        public int createUser { get; set; }

        public string createUserName { get; set; }

        //创建时间
        public DateTime createTime { get; set; }

        public string createDate { get; set; }
        public string createHMS { get; set; }
    }
}