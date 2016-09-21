using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IFlowEditBLL
    {
        /// 保存流程信息设置
        void SaveNodeLink(List<NodeLinkInfoModel> linkInfo, int[] deleteLinkId, int[] deleteConditionId);
    }
}