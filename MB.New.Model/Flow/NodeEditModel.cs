using System.Collections.Generic;

namespace MB.New.Model
{
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