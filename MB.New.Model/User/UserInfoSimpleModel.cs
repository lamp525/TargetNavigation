namespace MB.New.Model
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoSimpleModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 截取后头像
        /// </summary>
        public string headImage { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int? orgId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string orgName { get; set; }

        /// <summary>
        /// 部门信息（含上级部门名称）
        /// </summary>
        public string orgInfo { get; set;}

        /// <summary>
        /// 岗位ID
        /// </summary>
        public int? stationId { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string stationName { get; set; }
     
    }
}