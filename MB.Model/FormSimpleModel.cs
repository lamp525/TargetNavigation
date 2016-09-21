using System;

namespace MB.Model
{
    public class FormSimpleModel
    {
        /// <summary>表单Id</summary>
        public int formId { get; set; }

        /// <summary>模板Id</summary>
        public int templateId { get; set; }

        /// <summary>当前节点Id</summary>
        public int? nodeId { get; set; }

        /// <summary>状态</summary>
        public int? status { get; set; }

        /// <summary>标题</summary>
        public string title { get; set; }

        /// <summary>创建时间</summary>
        public DateTime? createTime { get; set; }
    }
}