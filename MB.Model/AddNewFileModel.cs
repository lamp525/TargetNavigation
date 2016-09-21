using System.Collections.Generic;

namespace MB.Model
{
    public class AddNewFileModel
    {
        //1:公司文档   2:个人文档
        public int type { get; set; }

        //上级文件夹
        public int? folder { get; set; }

        public string displayName { get; set; }

        public string extension { get; set; }

        public string savename { get; set; }

        public List<FileModel> File { get; set; }

        public int[] SheraUser { get; set; }

        //标签
        public string[] keyword { get; set; }
    }

    public class FileModel
    {
        //文件名
        public string fileName { get; set; }

        //后缀名
        public string extension { get; set; }

        //存储名
        public string saveName { get; set; }

        //
        public string displayname { get; set; }
    }
}