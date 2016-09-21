namespace MB.Model
{
    public class NewsDirectoryModel
    {
        ///// <summary>目录ID</summary>
        //public int? directoryId { get; set; }

        ///// <summary>目录名</summary>
        //public string directoryName { get; set; }

        ///// <summary>父目录ID</summary>
        //public int? parentDirectory { get; set; }

        ///// <summary>排序</summary>
        //public int orderNum { get; set; }

        /// <summary>目录ID</summary>
        public int id { get; set; }

        // <summary>目录名</summary>
        public string name { get; set; }

        /// <summary>是否为父目录</summary>
        public bool isParent { get; set; }
    }
}