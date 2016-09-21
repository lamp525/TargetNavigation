using System.Collections.Generic;

namespace MB.Model
{
    public class NodeModel
    {
        //流程节点ID
        public int? nodeId { get; set; }

        //模版ID
        public int templateId { get; set; }

        //节点名称
        public string nodeName { get; set; }

        //节点类型 1：创建   2：提交 3:审批  4：归档
        public int? nodeType { get; set; }

        public bool? operateEditFlag { get; set; }
    }

    public class NodeInfoModel
    {
        //流程节点
        public NodeModel node { get; set; }

        //节点字段
        public List<NodeFieldModel> nodeField { get; set; }

        //节点操作人
        public List<NodeOperateModel> nodeOperate { get; set; }
    }

    public class NodeEditModel
    {
        //节点信息
        public List<NodeInfoModel> nodeInfo { get; set; }

        //被删除的节点ID
        public int[] deleteNode { get; set; }

        //被删除的节点操作ID
        public int[] deleteOperateId { get; set; }
    }
}