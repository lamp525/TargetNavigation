using System.Collections.Generic;

namespace MB.Model
{
    public class FlowBatchCheckModel
    {
        /// <summary>流程列表</summary>
        public List<FlowCheckInfo> flowList { get; set; }

        /// <summary>审批意见</summary>
        public string content { get; set; }

        /// <summary>true:同意 false:退回</summary>
        public bool type { get; set; }
    }

    public class FlowCheckInfo
    {
        /// <summary>表单Id</summary>
        public int formId { get; set; }

        /// <summary>模板Id</summary>
        public int templateId { get; set; }

        /// <summary>当前节点Id</summary>
        public int nodeId { get; set; }

        /// <summary>是否委托：1、是委托 0、不是委托</summary>
        public int isEtruct { get; set; }
    }
}