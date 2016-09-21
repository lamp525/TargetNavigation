namespace MB.Model
{
    public class FlowProcessModel
    {
        //状态Id:1、待提交 2、已提交 3、待处理 4、已处理 5、已办结
        public int id { get; set; }

        //状态名
        public string text { get; set; }

        //状态下的流程数量
        public int count { get; set; }
    }
}