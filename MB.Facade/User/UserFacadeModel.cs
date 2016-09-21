namespace MB.Facade.User
{
    /// <summary>
    /// 用户部门信息
    /// </summary>
    public class PageUserOrgInfoModel
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public int orgId { get; set; }

        /// <summary>
        /// 部门信息（含上级部门）
        /// </summary>
        public string orgInfo { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class PageUserInfoSimpleModel
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
        /// 岗位ID
        /// </summary>
        public int? stationId { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string stationName { get; set; }
    }

    /// <summary>
    /// 用户头像信息
    /// </summary>
    public class PageUserHeadImageModel
    {
        /// <summary>
        /// 用户原头像
        /// </summary>
        public string originalImage { get; set; }

        /// <summary>
        /// 头像位置
        /// </summary>
        public string imagePosition { get; set; }

        /// <summary>
        /// 截取处的坐标X值
        /// </summary>
        public int startPointX { get; set; }

        /// <summary>
        /// 截取处的坐标Y值
        /// </summary>
        public int startPointY { get; set; }

        /// <summary>
        /// 截取图片的宽度
        /// </summary>
        public int cutWidth { get; set; }

        /// <summary>
        /// 截取图片的高度
        /// </summary>
        public int cutHeight { get; set; }

        /// <summary>
        /// 后缀名
        /// </summary>
        public string extension { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string imageUrl { get; set; }

        /// <summary>
        /// 是否上传
        /// </summary>
        public bool isUploaded { get; set; }
    }

    /// <summary>
    /// 用户密码信息
    /// </summary>
    public class PageUserPasswordModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? userId { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string newPassword { get; set; }
    }
}