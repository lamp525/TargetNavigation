namespace MB.Model
{
    public class MoveModel
    {
        //源文档类型
        public int documentType { get; set; }

        //所要移动的文档
        public int[] documentId { get; set; }

        //移动的目标文件夹
        public int?[] folder { get; set; }

        // 权限
        public bool withAuth { get; set; }
    }
}