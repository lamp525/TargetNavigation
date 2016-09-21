namespace MB.Model
{
    //文件目录
    public class FileDirectoryModel
    {
        public int? id { get; set; }

        public string name { get; set; }

        public bool isParent { get; set; }

        public bool deletefalse { get; set; }

        public int power { get; set; }

        public int type { get; set; }

        public int parent { get; set; }
    }
}