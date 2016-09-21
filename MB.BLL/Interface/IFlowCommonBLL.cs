using System.Collections.Generic;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface IFlowCommonBLL
    {
        /// 验证节点设置是否正确
        bool CheckNode(List<NodeInfoModel> nodeInfoList, out string message);

        ///// 验证流程设置是否正确
        bool CheckFlow(List<NodeLinkModel> nodeLink);

        ///  插入模版新增未设置控件的字段权限
        void InsertDefaultNodeField(int templateId);

        /// 取得节点设置信息
        List<NodeInfoModel> GetNodeInfoList(int templateId);

        /// 取得流程节点信息
        List<NodeModel> GetNodeList(int templateId);

        /// 取得节点流程
        List<NodeLinkModel> GetNodeLinkList(int templateId);

        /// 取得流程条件信息
        List<LinkConditionModel> GetLinkConditionInfo(TargetNavigationDBEntities db, int linkId, int templateId);

        /// 取得流程条件相关信息
        LinkConditionInfoModel GetLinkConditionRelatedInfo(int? linkId, int templateId);

        /// 取得模版节点操作人信息
        List<NodeOperateModel> GetTemplateNodeOperateList(int nodeId);

        ///  组合流程条件信息
        string CombineConditionInfo(List<LinkConditionModel> conditionList, List<LinkFormulaModel> formulaInfoList, bool isTest = false);

        ///  组合模版节点操作人信息
        string CombineTemplateOperateInfo(int? nodeType, List<NodeOperateModel> nodeOperateInfoList, bool isTest = false);

        /// 取得表单节点操作人信息
        List<NodeOperateModel> GetNodeOperateList(TargetNavigationDBEntities db, int nodeId, int applicantUserId, int formId = 0);

        /// 取得下一操作节点ID
        int GetNextOperateNodeId(TargetNavigationDBEntities db, int nodeId, int formId, ConstVar.LinkStatus status, int userId = 0);

        /// 取得流程条件公式信息
        List<LinkFormulaModel> GetLinkFormulaInfo(TargetNavigationDBEntities db, int linkId);

        ///  取得表单创建用户信息
        UserInfoSimpleModel GetCreateUserInfo(TargetNavigationDBEntities db, int userId);

        /// 根据控件类型取得控件类型名称
        string GetControlTypeNameByType(int controlType);

        ///  根据用户ID取得用户名
        string GetUserNameById(TargetNavigationDBEntities db, int userId);

        /// 取得操作人的被委托人信息
        int GetMandataryUserInfo(TargetNavigationDBEntities db, int templateId, int userId);

        ///  根据控件ID取得控件信息
        ControlSimpleModel GetControlInfoById(TargetNavigationDBEntities db, string controlId, int templateId);

        /// 取得流程的有效节点信息
        List<NodeModel> GetValidNodeInfoList(int templateId);

        /// 判断模版测试状态
        bool CheckTemplateTestStatus(int templateId);

        /// 更新模版测试标志
        void UpdateTemplateTestFlag(int templateId, bool testFlag);
    }
}