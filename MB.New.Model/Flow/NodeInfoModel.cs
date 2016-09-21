using System.Collections.Generic;

namespace MB.New.Model
{
    public class NodeInfoModel
    {
        //流程节点
        public NodeModel node { get; set; }

        //节点字段
        public List<NodeFieldModel> nodeField { get; set; }

        //节点操作人
        public List<NodeOperateModel> nodeOperate { get; set; }
    }
}