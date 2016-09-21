using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class UserFormModel
    {
        //表单ID
        public int formId { get; set; }

        //模版ID
        public int templateId { get; set; }

        public int categoryId { get; set; }

        //组织ID
        public int organizationId { get; set; }

        //组织名称
        public string organizationName { get; set; }

        //岗位ID
        public int stationId { get; set; }

        //标题
        public string title { get; set; }

        //默认标题 0：自定义标题 1：系统默认标题
        public bool defaultTitle { get; set; }

        //紧急度 0~5
        public int? urgency { get; set; }

        //状态 1：待提交 2：流程中 50：已办结
        public int? status { get; set; }

        //显示操作的标识:1、待提交 2、已提交 3、待处理 4、已处理 5、已办结 6、待审核 7、待查阅 8、已审核 9、已查阅
        public int operateStatus { get; set; }

        //当前流程节点
        public int? currentNode { get; set; }

        //当前节点名
        public string nodeName { get; set; }

        //操作类型:0：未查阅 1：未提交 2：未审批 3：已处理
        public int alreadyRead { get; set; }

        //归档标志
        public bool? archive { get; set; }

        //归档时间
        public DateTime? archiveTime { get; set; }

        //工作流
        public string templateName { get; set; }

        //当前未操作人
        public List<User> operate { get; set; }

        //创建用户
        public int? createUser { get; set; }

        public string createUserName { get; set; }

        /// <summary>是否管理员:true、管理员 false、不是管理员</summary>
        public bool? admin { get; set; }

        //用户头像
        public string img { get; set; }

        //创建时间
        public DateTime? createTime { get; set; }

        //修改用户
        public int? updateUser { get; set; }

        //修改时间
        public DateTime? updateTime { get; set; }

        //删除标志
        public bool? deleteFlag { get; set; }

        //是否委托流程:1、是委托流程
        public int isEntruct { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }

        public int? mandataryUser { get; set; }
        public string mandataryUserName { get; set; }
    }
}