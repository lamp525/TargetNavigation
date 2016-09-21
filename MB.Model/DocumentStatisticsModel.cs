namespace MB.Model
{
    public class DocumentStatisticsModel
    {
        //类型 1、公司文档 2、用户文档 3、我的共享 4、他人共享
        public int id { get; set; }

        //统计的数量
        public int count { get; set; }

        //描述
        public string text { get; set; }
    }
}