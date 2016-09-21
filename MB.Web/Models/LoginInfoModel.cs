namespace MB.Web.Models
{
    /// <summary>
    /// 登陆用户信息
    /// </summary>
    public class LoginInfoModel
    {
        /// <summary>用户ID</summary>
        public int userId { get; set; }

        ///<summary>用户名称</summary>
        public string userName { get; set; }

        /// <summary>岗位ID</summary>
        public int stationId { get; set; }

        /// <summary>岗位名称</summary>
        public int stationName { get; set; }

        /// <summary>组织ID</summary>
        public int orgId { get; set; }

        /// <summary>组织名称</summary>
        public string  orgName { get; set; }
    }
}