using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class OrganizationModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<OrganizationModel> children { get; set; }
    }

    public class ProjectModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<ProjectModel> children { get; set; }
    }

    //流程首页中用到的用户信息
    public class FlowIndexUserInfo
    {
        /// <summary>用户Id</summary>
        public int userId { get; set; }

        /// <summary>用户名称</summary>
        public string userName { get; set; }

        /// <summary>创建时间</summary>
        public DateTime createTime { get; set; }

        public List<OrganizationModel> orgList { get; set; }
    }
}