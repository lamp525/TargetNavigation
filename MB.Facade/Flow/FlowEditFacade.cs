using MB.DAL;
using MB.New.BLL;
using MB.New.Common;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.Facade.Flow
{
    public class FlowEditFacade : IFlowEditFacade
    {
        #region 节点设置

        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public List<PageNodeInfoModel> GetNodeInfo(int templateId)
        {
            IFlowCommonBLL FlowCommon = new FlowCommonBLL();

            var result = new List<PageNodeInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = FlowCommon.GetNodeInfo(db, templateId);
                result = ModelMapping.JsonMapping<List<NodeInfoModel>, List<PageNodeInfoModel>>(info);
            }
            return result;
        }

        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<PageNodeFieldEditModel> GetNodeFieldInfo(int templateId, int? nodeId)
        {
            IFlowEditBLL FlowEdit = new FlowEditBLL();

            var result = new List<PageNodeFieldEditModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = FlowEdit.GetNodeFieldList(db, templateId, nodeId);
                result = ModelMapping.JsonMapping<List<NodeFieldEditModel>, List<PageNodeFieldEditModel>>(info);
            }

            return result;
        }

        /// <summary>
        /// 检查节点的删除标志
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        public bool CheckDeleteFlag(int nodeId)
        {
            bool result = false;

            return result;
        }

        /// <summary>
        /// 保存节点设置
        /// </summary>
        /// <param name="nodeEditInfo"></param>
        /// <returns></returns>
        public bool SaveFlowNode(PageNodeEditModel nodeEditInfo)
        {
            return true;
        }

        #endregion 节点设置

        #region 流程设置

        /// <summary>
        /// 取得流程节点信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public List<PageNodeModel> GetFlowNodeInfo(int templateId)
        {
            IFlowCommonBLL FlowCommon = new FlowCommonBLL();

            var result = new List<PageNodeModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = FlowCommon.GetFlowNodeInfo(db, templateId);
                result = ModelMapping.JsonMapping<List<NodeModel>, List<PageNodeModel>>(info);
            }

            return result;
        }

        /// <summary>
        /// 取得节点流程信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public List<PageNodeInfoModel> GetNodeLinkInfo(int templateId)
        {
            var result = new List<PageNodeInfoModel>();

            return result;
        }

        /// <summary>
        /// 取得流程条件信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="linkId"></param>
        /// <returns></returns>
        public List<PageLinkConditionModel> GetLinkConditionRelatedInfo(int templateId, int? linkId)
        {
            var result = new List<PageLinkConditionModel>();

            return result;
        }

        /// <summary>
        /// 保存流程设置
        /// </summary>
        /// <param name="flowEditInfo"></param>
        public bool SaveNodeLink(PageFlowEditModel flowEditInfo)
        {
            return true;
        }

        #endregion 流程设置
    }
}