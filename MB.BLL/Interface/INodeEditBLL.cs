using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface INodeEditBLL
    {
        /// 取得节点字段信息
        List<NodeFieldEditModel> GetNodeFieldList(int? nodeId, int templateId);

        /// 保存节点信息设置
        void SaveFlowNode(List<NodeInfoModel> nodeInfo, int[] deleteNode, int[] deleteOperateId);

        /// 检查节点删除标志
        bool CheckDeleteFlag(int nodeId);
    }
}