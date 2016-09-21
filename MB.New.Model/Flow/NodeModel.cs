namespace MB.New.Model
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
}