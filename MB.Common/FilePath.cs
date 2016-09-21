using System.Configuration;

namespace MB.Common
{
    /// <summary>
    /// 服务器路径【计划模块暂未使用】，2015-5-14
    /// </summary>
    public class FilePath
    {
        /// <summary>
        /// 用户默认头像
        /// </summary>
        public static string DefaultHeadPortrait = "../../Images/common/portrait.png";

        /// <summary>
        /// 新闻路径
        /// </summary>
        public static string NewsUpLoadPath = ConfigurationManager.AppSettings["NewsUpLoadPath"].ToString();

        /// <summary>
        /// 文档路径
        /// </summary>
        public static string DocumentUpLoadPath = ConfigurationManager.AppSettings["DocumentUpLoadPath"].ToString();

        /// <summary>
        /// 计划路径
        /// </summary>
        public static string PlanUpLoadPath = ConfigurationManager.AppSettings["PlanUpLoadPath"].ToString();

        /// <summary>
        /// 个人路径
        /// </summary>
        public static string MineUpLoadPath = ConfigurationManager.AppSettings["MineUpLoadPath"].ToString();

        /// <summary>
        /// 头像路径
        /// </summary>
        public static string HeadImageUpLoadPath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

        /// <summary>
        /// IM文件路径
        /// </summary>
        public static string IMUploadPath = ConfigurationManager.AppSettings["IMUploadPath"].ToString();

        /// <summary>
        /// 流程首页文件路径
        /// </summary>
        public static string FlowIndexUploadPath = ConfigurationManager.AppSettings["FlowIndexUploadPath"].ToString();

        /// <summary>
        /// 会议室附件下载路径
        /// </summary>
        public static string MeetingUploadPath = ConfigurationManager.AppSettings["MeetingUpLoadPath"].ToString();

       /// <summary>
        /// 预览用文件路径<
       /// </summary>
        public static string PreviewPath = ConfigurationManager.AppSettings["ConvertFilePath"].ToString();
    }
}