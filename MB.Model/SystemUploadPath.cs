namespace MB.Model
{
    public class SystemUploadPath
    {
        //新闻通知分页
        public string pageNum { get; set; }

        //用户列表
        public string userPage { get; set; }

        //设置量质时的最大值
        public string completeQuantity { get; set; }

        public string completeQuality { get; set; }
        public string completeTime { get; set; }

        //设置错误信息文件
        public string MessagePath { get; set; }

        //密码错误次数验证码
        public string InputErrorValidate { get; set; }

        //上传文件路径
        public string NewsUpLoadPath { get; set; }

        public string DocumentUpLoadPath { get; set; }
        public string PlanUpLoadPath { get; set; }
        public string MineUpLoadPath { get; set; }
        public string HeadImageUpLoadPath { get; set; }
        public string IMUploadPath { get; set; }
        public string ObjectiveUploadPath { get; set; }
        public string FlowIndexUploadPath { get; set; }
        public string MeetingUpLoadPath { get; set; }
        public string ConvertFilePath { get; set; }

        //IM通讯服务
        public string IMHost { get; set; }

        //模板路径
        public string PlanTemplate { get; set; }

        //量质时系数
        public string maxQuantity { get; set; }

        public string maxQuality { get; set; }
        public string maxTime { get; set; }
    }
}