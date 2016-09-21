using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class FormDetailModel
    {
        /// <summary>模板Id</summary>
        public int templateId { get; set; }

        /// <summary>模板名称</summary>
        public string templateName { get; set; }

        /// <summary>默认标题:0：自定义标题 1：系统默认标题</summary>
        public bool defaultTitle { get; set; }

        /// <summary>组织Id</summary>
        public int organizationId { get; set; }

        /// <summary>组织名称</summary>
        public string organizationName { get; set; }

        /// <summary>岗位Id</summary>
        public int stationId { get; set; }

        /// <summary>岗位名称 </summary>
        public string stationName { get; set; }

        /// <summary>标题</summary>
        public string title { get; set; }

        /// <summary>紧急度</summary>
        public int urgency { get; set; }

        /// <summary>状态</summary>
        public int status { get; set; }

        /// <summary>当前节点</summary>
        public int currentNode { get; set; }

        /// <summary>创建用户</summary>
        public int createUser { get; set; }

        public string createUserName { get; set; }

        /// <summary>创建时间</summary>
        public DateTime createTime { get; set; }

        /// <summary>控件值集合</summary>
        public List<ControlNewModel> controlValue { get; set; }
    }

    public class ControlNewModel
    {
        /// <summary>详细Id</summary>
        public int detailId { get; set; }

        /// <summary>控件Id</summary>
        public string controlId { get; set; }

        /// <summary>父控件ID</summary>
        public string parentControl { get; set; }

        /// <summary>控件类型</summary>
        public int controlType { get; set; }

        /// <summary>必须 0：False 1：True</summary>
        public int? require { get; set; }

        /// <summary>标题</summary>
        public string title { get; set; }

        public string description { get; set; }

        /// <summary>控件权限</summary>
        public int status { get; set; }

        /// <summary>明细行</summary>
        public int? rowNumber { get; set; }

        /// <summary>明细行值</summary>
        public string[] detailValue { get; set; }

        /// <summary>文件存储名</summary>
        public string[] saveName { get; set; }
    }
}