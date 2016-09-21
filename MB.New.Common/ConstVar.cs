using System.Configuration;

namespace MB.New.Common
{
    public static class ConstVar
    {
        #region 用户相关

        /// <summary>
        /// 登录信息Cookie名称
        /// </summary>
        public static string CookieName_UserInfo = "UserInfoCookie";

        /// <summary>
        /// 登录验证码Cookie名称
        /// </summary>
        public static string CookieName_VerifyVCode = "VerifyVCodeCookie";

        /// <summary>
        /// 用户IDCookie键名
        /// </summary>
        public static string CookieKey_UserID = "UserId";

        /// <summary>
        /// 岗位IDCookie键名
        /// </summary>
        public static string CookieKey_StationID = "StationId";

        /// <summary>
        /// 部门IDCookie键名
        /// </summary>
        public static string CookieKey_OrgID = "OrgId";

        /// <summary>
        /// 登录用户密码错误次数
        /// </summary>
        public static int WrongPwdNum
        {
            get
            {
                string num = ConfigurationManager.AppSettings["InputErrorValidate"];

                if (string.IsNullOrEmpty(num)) return 5;

                return int.Parse(num);
            }
        }

        /// <summary>
        /// 用户常用标签数量
        /// </summary>
        public static int MostUsedTagNum
        {
            get
            {
                string num = ConfigurationManager.AppSettings["MostUsedTagNum"];

                if (string.IsNullOrEmpty(num)) return 7;

                return int.Parse(num);
            }
        }

        #endregion 用户相关

        #region 计划相关

        /// <summary>
        /// 计划评定系数：完成数量
        /// </summary>
        public static string CompleteQuantity = ConfigurationManager.AppSettings["maxQuantity"];

        /// <summary>
        /// 计划评定系数：完成质量
        /// </summary>
        public static string CompleteQuality = ConfigurationManager.AppSettings["maxQuality"];

        /// <summary>
        /// 计划评定系数：完成时间
        /// </summary>
        public static string CompleteTime = ConfigurationManager.AppSettings["maxQuantity"];

        /// <summary>
        /// 计划列表分页
        /// </summary>
        public static int PlanListPageNum
        {
            get
            {
                string num = ConfigurationManager.AppSettings["PlanListPageNum"];
                if (string.IsNullOrEmpty(num))
                {
                    return 20;
                }

                return int.Parse(num);
            }
        }

        /// <summary>
        /// 计划列表分组后分页
        /// </summary>
        public static int PlanGroupPageNum
        {
            get
            {
                string num = ConfigurationManager.AppSettings["PlanGroupPageNum"];
                if (string.IsNullOrEmpty(num))
                {
                    return 10;
                }

                return int.Parse(num);
            }
        }

        /// <summary>
        /// 计划部门拼接层数
        /// </summary>
        public static int PlanOrgSpliceNum
        {
            get
            {
                string num = ConfigurationManager.AppSettings["PlanOrgSpliceNum"];
                if (string.IsNullOrEmpty(num)) return 3;

                return int.Parse(num);
            }
        }

        /// <summary>
        /// 循环计划提交信息分页数
        /// </summary>
        public static int LoopPlanSubmitPageNum
        {
            get
            {
                string num = ConfigurationManager.AppSettings["LoopPlanSubmitPageNum"];
                if (string.IsNullOrEmpty(num)) return 7;

                return int.Parse(num);
            }
        }

        #endregion 计划相关

        #region 文件夹/文件路径

        /// <summary>
        /// Web服务主机URL地址
        /// </summary>
        public static string WebHostURL = System.Web.HttpContext.Current.Request.Url.Authority;

        /// <summary>
        /// 用户默认头像
        /// </summary>
        public static string DefaultUserHead = "/Images/common/bigUserImg.png";

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
        /// 目标文件路径
        /// </summary>
        public static string ObjectiveUploadPath = ConfigurationManager.AppSettings["ObjectiveUploadPath"].ToString();

        /// <summary>
        /// 会议室附件下载路径
        /// </summary>
        public static string MeetingUploadPath = ConfigurationManager.AppSettings["MeetingUpLoadPath"].ToString();

        /// <summary>
        /// 预览用文件路径
        /// </summary>
        public static string PreviewPath = ConfigurationManager.AppSettings["ConvertFilePath"].ToString();

        #endregion 文件夹/文件路径
    }
}