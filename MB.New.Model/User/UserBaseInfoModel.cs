using MB.New.Common;
using System;

namespace MB.New.Model
{
    /// <summary>
    /// 用户详细信息
    /// </summary>
    public class UserBaseInfoModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        public string userNumber { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 密码加密用字符
        /// </summary>
        public string randomKey { get; set; }

        /// <summary>
        /// 用户默认岗位ID
        /// </summary>
        public int? defaultStationId { get; set; }

        /// <summary>
        /// 头像截取位置
        /// </summary>
        public string imagePosition { get; set; }

        /// <summary>
        /// 头像原图位置
        /// </summary>
        public string originalImage { get; set; }

        /// <summary>
        /// 截取后头像
        /// </summary>
        public string headImage { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool? sex { get; set; }

        /// <summary>
        /// 在职状态
        /// </summary>
        public EnumDefine.WorkStatus workStatus { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime? quitTime { get; set; }

        /// <summary>
        /// 密码错误次数
        /// </summary>
        public int? errorPassword { get; set; }

        /// <summary>
        /// 系统管理员标志
        /// </summary>
        public bool? isAdmin { get; set; }

        /// <summary>
        /// 执行力管理员标志
        /// </summary>
        public bool? isExecution { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool deleteFlag { get; set; }
    }
}