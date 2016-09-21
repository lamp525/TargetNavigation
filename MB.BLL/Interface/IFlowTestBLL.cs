using System.Collections.Generic;
using MB.Common;
using MB.Model;

namespace MB.BLL
{
    public interface IFlowTestBLL
    {
        /// 取得创建用户的岗位信息
        List<StationModel> GetCreateUserStationList(int userId, int orgId);

        /// 取得流程的有效节点信息
        List<NodeModel> GetValidNodeInfoList(int templateId);

        /// 取得模版流程图信息
        TemplateFlowChartModel GetTemplateFlowChartInfo(int templateId);

        /// 判断表单创建者是否有效
        bool CheckFormCreateUser(int createUserId, int createNodeId);

        /// 表单操作处理
        bool FormOperate(AddFormContentModel formModel, ConstVar.LinkStatus status);

        /// 取得流程测试标志
        bool GetFlowTestFlag(int templateId);
    }
}