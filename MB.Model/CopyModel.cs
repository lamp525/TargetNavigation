namespace MB.Model
{
    public class CopyModel
    {
        //源文档类型
        public int documentType { get; set; }

        //所要复制的文档
        public int[] documentId { get; set; }

        //复制到的公司文件夹
        public int?[] companyFolder { get; set; }

        //复制到的个人文件夹
        public int?[] userFolder { get; set; }

        //权限
        public bool withAuth { get; set; }
    }
}