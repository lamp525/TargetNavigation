using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class RosterModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string userNumber { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 随机8位秘钥
        /// </summary>
        public string randomKey { get; set; }

        public string imagePosition { get; set; }
        public string originalImage { get; set; }
        public string bigImage { get; set; }
        public string midImage { get; set; }
        public string smallImage { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Nullable<bool> sex { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string nation { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string political { get; set; }

        /// <summary>
        /// 婚否
        /// </summary>
        public Nullable<bool> marriage { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string identityCard { get; set; }

        /// <summary>
        /// 手机1
        /// </summary>
        public string mobile1 { get; set; }

        /// <summary>
        /// 手机2
        /// </summary>
        public string mobile2 { get; set; }

        /// <summary>
        /// 实际住址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 工作地点
        /// </summary>
        public string workPlace { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public Nullable<System.DateTime> entryTime { get; set; }

        /// <summary>
        /// 试用期
        /// </summary>
        public Nullable<int> probationaryPeriod { get; set; }

        /// <summary>
        /// 转正时间
        /// </summary>
        public Nullable<System.DateTime> positiveDate { get; set; }

        /// <summary>
        /// 合同期
        /// </summary>
        public Nullable<int> term { get; set; }

        /// <summary>
        /// 合同到期日
        /// </summary>
        public Nullable<System.DateTime> expiredDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public Nullable<System.DateTime> birthday { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public Nullable<int> age { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public string nature { get; set; }

        /// <summary>
        /// 省份Id
        /// </summary>
        public int? provinceId { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        public int? cityId { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public int? districtId { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string district { get; set; }

        /// <summary>
        /// 乡镇
        /// </summary>
        public string township { get; set; }

        /// <summary>
        /// 毕业学院
        /// </summary>
        public string school { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        public string professional { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public Nullable<int> education { get; set; }

        /// <summary>
        /// 首次参加工作时间
        /// </summary>
        public Nullable<System.DateTime> firstWork { get; set; }

        /// <summary>
        /// 资格证书
        /// </summary>
        public string qualification { get; set; }

        /// <summary>
        /// 短号
        /// </summary>
        public string cornet { get; set; }

        /// <summary>
        /// 紧急联系号码
        /// </summary>
        public string emergencyNumber { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCard { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string bankName { get; set; }

        /// <summary>
        /// 在职状态  1：转正 2：离职 3：退休
        /// </summary>
        public Nullable<int> workStatus { get; set; }

        /// <summary>
        /// 用户类型  1：实习 2：试用
        /// </summary>
        public Nullable<int> userType { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        public Nullable<System.DateTime> quitTime { get; set; }

        /// <summary>
        /// 工资
        /// </summary>
        public Nullable<decimal> salary { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string nativePlace { get; set; }

        /// <summary>
        /// qq
        /// </summary>
        public string qq { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }

        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
        public bool deleteFlag { get; set; }
        public List<Organization> orgList { get; set; }

        //删除岗位
        public List<int> deleteStation { get; set; }

        //添加岗位
        public List<int> newStation { get; set; }
    }

    public class Organization
    {
        /// <summary>
        /// 组织架构Id
        /// </summary>
        public int organizationId { get; set; }

        /// <summary>
        /// 组织名
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// 职位Id
        /// </summary>
        public int stationId { get; set; }

        /// <summary>
        /// 职位名
        /// </summary>
        public string stationName { get; set; }
    }

    /// <summary>
    /// 用户列表所需
    /// </summary>
    public class RosterInfo
    {
        public int userId { get; set; }
        public string userNumber { get; set; }
        public string userName { get; set; }
        public string headImage { get; set; }
        public string mobile1 { get; set; }
        public string mobile2 { get; set; }
        public List<Organization> station { get; set; }
        public int? workStatus { get; set; }
        public int? userType { get; set; }
        public DateTime? validDate { get; set; }
        public DateTime? positiveDate { get; set; }
        public DateTime? quitTime { get; set; }
        public DateTime? internDate { get; set; }
        public DateTime? probationDate { get; set; }
        public int pageCount { get; set; }
    }

    //银行、省份、城市的Id,name
    public class data
    {
        public int strId { get; set; }
        public string strName { get; set; }
    }
}