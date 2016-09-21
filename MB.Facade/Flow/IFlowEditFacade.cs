using System.Collections.Generic;

namespace MB.Facade.Flow
{
    public interface IFlowEditFacade
    {
        #region 节点设置

        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        List<PageNodeInfoModel> GetNodeInfo(int templateId);

        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        List<PageNodeFieldEditModel> GetNodeFieldInfo(int templateId, int? nodeId);

        /// <summary>
        /// 检查节点的删除标志
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        bool CheckDeleteFlag(int nodeId);

        /// <summary>
        /// 保存节点设置
        /// </summary>
        /// <param name="nodeEditInfo"></param>
        /// <returns></returns>
        bool SaveFlowNode(PageNodeEditModel nodeEditInfo);

        #endregion 节点设置

        #region 流程设置

        /// <summary>
        /// 取得流程节点信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        List<PageNodeModel> GetFlowNodeInfo(int templateId);

        /// <summary>
        /// 取得节点流程信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        List<PageNodeInfoModel> GetNodeLinkInfo(int templateId);

        /// <summary>
        /// 取得流程条件信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="linkId"></param>
        /// <returns></returns>
        List<PageLinkConditionModel> GetLinkConditionRelatedInfo(int templateId, int? linkId);

        /// <summary>
        /// 保存流程设置
        /// </summary>
        /// <param name="flowEditInfo"></param>
        bool SaveNodeLink(PageFlowEditModel flowEditInfo);

        #endregion 流程设置
    }
}