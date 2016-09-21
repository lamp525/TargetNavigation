namespace MB.Model
{
    public class DocConditionModel
    {
        //文档类型  1、公司文档 2、个人文档 3、我的共享 4、他人共享
        public int type { get; set; }

        //文档名称
        public string docName { get; set; }

        //上级文件夹Id
        public int? folder { get; set; }

        //创建时间
        public string[] time { get; set; }

        //创建人
        public int?[] person { get; set; }

        //排序
        public Sort sorts { get; set; }

        //
    }
}