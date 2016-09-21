using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class FlowChartBLL : IFlowChartBLL
    {
        #region 变量区域

        /// <summary>流程共通处理 </summary>
        private FlowCommonBLL flowCommon = new FlowCommonBLL();

        ///<summary>有效节点的操作信息Model</summary>
        private class ValidNodeOperateInfoModel
        {
            //节点ID
            public int? nodeId { get; set; }

            //节点操作信息
            public List<NodeOperateModel> operateInfoList { get; set; }
        }

        #endregion 变量区域

        #region 常量区域

        //操作类型
        private const string CREATE = "创建";

        private const string APPROVAL = "审批";
        private const string COUNTER_SIGN = "会签";
        private const string SUBMIT = "提交";
        private const string ARCHIVE = "归档";
        private const string PASS = "通过";
        private const string RETURN = "退回";
        private const string REVOKE = "撤销";
        private const string READ = "查看";

        #endregion 常量区域

        #region 模版流程图设置

        /// <summary>
        /// 模版流程图设置
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns>l流程图信息</returns>
        public TemplateFlowChartModel SetTemplateFlowChart(int templateId)
        {
            //模版节点及操作信息
            var nodePlusOperateInfoList = new List<NodePlusOperateModel>();

            //取得模版节点信息
            var nodeInfoList = flowCommon.GetNodeList(templateId);

            foreach (var node in nodeInfoList)
            {
                //节点操作人信息
                var operateList = flowCommon.GetTemplateNodeOperateList(node.nodeId.Value);
                //组合节点操作人信息
                var operateInfo = flowCommon.CombineTemplateOperateInfo(node.nodeType, operateList);

                var nodePlusOperate = new NodePlusOperateModel
                {
                    nodeId = node.nodeId,
                    nodeName = node.nodeName,
                    nodeType = node.nodeType,
                    operate = operateInfo
                };

                nodePlusOperateInfoList.Add(nodePlusOperate);
            }

            //节点出口及条件信息
            var linkPlusConditionInfoList = new List<LinkPlusConditionModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                //取得节点出口信息列表
                var nodeLinkList = flowCommon.GetNodeLinkList(db, templateId);
                foreach (var item in nodeLinkList)
                {
                    //取得节点出口的流程条件信息
                    var conditionList = flowCommon.GetLinkConditionInfo(db, item.linkId.Value, templateId);

                    //取得节点出口的流程公式信息
                    var formulaList = flowCommon.GetLinkFormulaInfo(db, item.linkId.Value);

                    //组合节点出口的流程条件信息
                    var conditionInfo = flowCommon.CombineConditionInfo(conditionList, formulaList);

                    var linkInfo = new LinkPlusConditionModel
                    {
                        linkId = item.linkId,
                        templateId = item.templateId,
                        nodeEntryId = item.nodeEntryId,
                        nodeEntryName = item.nodeEntryName,
                        nodeEntryType = item.nodeEntryType,
                        nodeExitId = item.nodeExitId,
                        nodeExitName = item.nodeExitName,
                        nodeExitType = item.nodeExitType,
                        status = item.status,
                        condition = conditionInfo
                    };

                    linkPlusConditionInfoList.Add(linkInfo);
                }
            }

            //模版流程图信息
            var flowChartInfo = new TemplateFlowChartModel();
            flowChartInfo.nodeInfo = nodePlusOperateInfoList;
            flowChartInfo.linkInfo = linkPlusConditionInfoList;
            return flowChartInfo;
        }

        #endregion 模版流程图设置

        #region 表单流程图设置

        /// <summary>
        /// 表单流程图设置
        /// </summary>
        /// <param name="formId">表单ID</param>
        /// <returns>l流程图信息</returns>
        public FormFlowChartModel SetFormFlowChart(int formId)
        {
            //流程图信息
            var flowChartInfo = new FormFlowChartModel();

            using (var db = new TargetNavigationDBEntities())
            {
                //取得表单信息
                var formInfo = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (formInfo != null)
                {
                    //表单已操作的所有流程操作信息
                    var formFlowInfo = new List<tblFormFlow>();
                    //表单当前节点尚未操作的操作人信息
                    var unOperateInfo = new List<tblFormDuplicate>();
                    //有效节点的操作人信息
                    var validNodeOperateList = new List<ValidNodeOperateInfoModel>();

                    //取得表单ID对应的模版ID
                    var templateId = db.tblUserForm.Where(p => p.formId == formId).FirstOrDefault().templateId;
                    ////表单创建者信息
                    var userInfo = flowCommon.GetCreateUserInfo(db, formInfo.createUser);
                    //userInfo.stationId = formInfo.stationId;
                    //userInfo.orgnationId = formInfo.organizationId;

                    //取得表单已操作的所有流程操作信息
                    formFlowInfo = db.tblFormFlow.Where(p => p.formId == formId && p.result != (int)ConstVar.FormOperateType.Download).OrderBy(p => p.createTime).ToList();

                    //表单未完结
                    if (formInfo.status != (int)ConstVar.userFormStatus.hasCompleted)
                    {
                        //表单当前节点尚未操作的操作人信息
                        unOperateInfo = db.tblFormDuplicate.Where(p => p.formId == formInfo.formId && p.nodeId == formInfo.currentNode).ToList();

                        //关联委托情况取得操作人信息
                        if (unOperateInfo.Count > 0)
                        {
                            foreach (var item in unOperateInfo)
                            {
                                //提交、审批和会签的场合
                                if (item.alreadyRead == (int)ConstVar.FormDuplicateStatus.Approval || item.alreadyRead == (int)ConstVar.FormDuplicateStatus.countersign || item.alreadyRead == (int)ConstVar.FormDuplicateStatus.Submit)
                                    item.userId = flowCommon.GetMandataryUserInfo(db, templateId, item.userId);
                            }
                        }

                        #region 表单当前节点之后的操作信息

                        //取得节点出口状态为通过的节点流程信息
                        var nodeLinkList = flowCommon.GetNodeLinkList(db, templateId).Where(p => p.status == (int)ConstVar.LinkStatus.Pass).ToList();
                        var nextNode = formInfo.currentNode.Value;
                        while (nextNode != 0)
                        {
                            nextNode = flowCommon.GetNextOperateNodeId(db, nextNode, formId, ConstVar.LinkStatus.Pass);

                            if (nextNode != 0)
                            {
                                var validNodeOperate = new ValidNodeOperateInfoModel();
                                validNodeOperate.nodeId = nextNode;
                                //取得节点的操作人信息
                                validNodeOperate.operateInfoList = flowCommon.GetNodeOperateList(db, nextNode, formInfo.createUser, formId);

                                validNodeOperateList.Add(validNodeOperate);
                            }
                        }

                        #endregion 表单当前节点之后的操作信息
                    }

                    //表单申请人小头像
                    flowChartInfo.smallImage = string.IsNullOrEmpty(userInfo.smallImage) ? FilePath.DefaultHeadPortrait : (FilePath.HeadImageUpLoadPath + userInfo.smallImage);
                    //表单节点操作人信息
                    flowChartInfo.operateInfo = CombineFormOperateInfo(db, formInfo, formFlowInfo, unOperateInfo, validNodeOperateList);
                }
            }

            return flowChartInfo;
        }

        #endregion 表单流程图设置

        #region 私有方法

        #region 组合表单节点操作人信息

        /// <summary>
        /// 组合表单节点操作人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="formInfo">当前表单信息</param>
        /// <param name="formFlowInfo"></param>
        /// <param name="unOperateInfo"></param>
        /// <param name="validNodeOperateList"></param>
        /// <returns>表单节点操作人信息</returns>
        private List<OperateInfoModel> CombineFormOperateInfo(TargetNavigationDBEntities db, tblUserForm formInfo, List<tblFormFlow> formFlowInfo,
                                                                                                     List<tblFormDuplicate> unOperateInfo, List<ValidNodeOperateInfoModel> validNodeOperateList)
        {
            var operateInfoList = new List<OperateInfoModel>();

            //表单已操作的操作人信息
            if (formFlowInfo.Count > 0)
            {
                var hasArchiveInfo = false;

                foreach (var item in formFlowInfo)
                {
                    var info = new OperateInfoModel();

                    //操作时间
                    info.operateTime = string.Format("{0:M-dd HH:mm}", item.createTime);
                    //操作类型
                    info.result = item.result.Value;
                    //节点意见
                    info.contents = item.contents;

                    //操作人
                    var operateUser = flowCommon.GetUserNameById(db, item.createUser);
                    //表单操作类型名
                    var formOperateName = this.GetOperateNameById(item.result.Value);
                    //操作信息
                    if (info.result != (int)ConstVar.FormOperateType.Archive)
                    {
                        if (info.result == (int)ConstVar.FormOperateType.Submit)
                        {
                            info.operateInfo = string.Format("{0} {1}了《{2}》", operateUser, formOperateName, formInfo.title);
                        }
                        else
                        {
                            info.operateInfo = string.Format("{0}已{1}", operateUser, formOperateName);
                        }

                        operateInfoList.Add(info);
                    }
                    //归档操作信息
                    else
                    {
                        if (!hasArchiveInfo)
                        {
                            info.operateInfo = string.Format("已{0}", ARCHIVE);
                            operateInfoList.Add(info);
                            hasArchiveInfo = true;
                        }
                    }
                }
            }

            //表单当前节点未操作的操作人信息
            if (unOperateInfo.Count > 0)
            {
                //节点操作信息
                var info = new OperateInfoModel();

                //审批信息
                var approvalInfo = unOperateInfo.Where(p => p.alreadyRead.Value == (int)ConstVar.FormDuplicateStatus.Approval).ToList();
                var approvalOperateInfo = string.Empty;
                if (approvalInfo.Count > 0)
                {
                    var approvalUser = new List<string>();
                    foreach (var item in approvalInfo)
                    {
                        approvalUser.Add(flowCommon.GetUserNameById(db, item.userId));
                    }
                    approvalOperateInfo = string.Format("等待{0}{1}<br /> ", string.Join(",", approvalUser), APPROVAL);
                }
                //会签信息
                var countersignInfo = unOperateInfo.Where(p => p.alreadyRead == (int)ConstVar.FormDuplicateStatus.countersign).ToList();
                var countersignOperateInfo = string.Empty;
                if (countersignInfo.Count > 0)
                {
                    var countersignUser = new List<string>();
                    foreach (var item in countersignInfo)
                    {
                        countersignUser.Add(flowCommon.GetUserNameById(db, item.userId));
                    }
                    countersignOperateInfo = string.Format("等待{0}{1}<br />", string.Join(",", countersignUser), COUNTER_SIGN);
                }
                //查阅信息
                var readOperateInfo = string.Empty;
                var readInfo = unOperateInfo.Where(p => p.alreadyRead.Value == (int)ConstVar.FormDuplicateStatus.Ready).ToList();
                if (readInfo.Count > 0)
                {
                    var readUser = new List<string>();
                    foreach (var item in readInfo)
                    {
                        readUser.Add(flowCommon.GetUserNameById(db, item.userId));
                    }
                    readOperateInfo = string.Format("等待{0}{1}<br /> ", string.Join(",", readUser), READ);
                }

                //提交信息
                var submitInfo = unOperateInfo.Where(p => p.alreadyRead.Value == (int)ConstVar.FormDuplicateStatus.Submit).ToList();
                var submitOperateInfo = string.Empty;
                if (submitInfo.Count > 0)
                {
                    var submitUser = new List<string>();
                    foreach (var item in submitInfo)
                    {
                        submitUser.Add(flowCommon.GetUserNameById(db, item.userId));
                    }
                    submitOperateInfo = string.Format("等待{0}{1}<br /> ", string.Join(",", submitUser), SUBMIT);
                }

                if (approvalInfo.Count == 0 && countersignInfo.Count == 0 && submitInfo.Count == 0 && readInfo.Count == 0)
                {
                    info.operateInfo = "未找到符合条件的操作人";
                }
                else
                {
                    info.operateInfo = submitOperateInfo + approvalOperateInfo + countersignOperateInfo + readOperateInfo;
                }
                operateInfoList.Add(info);
            }

            //表单剩余节点的操作人信息
            if (validNodeOperateList.Count > 0)
            {
                foreach (var node in validNodeOperateList)
                {
                    //节点操作信息
                    var info = new OperateInfoModel();

                    //归档信息
                    var archiveInfo = node.operateInfoList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Archive).FirstOrDefault();
                    if (archiveInfo != null)
                    {
                        info.operateInfo = string.Format("等待{0}", ARCHIVE);
                    }
                    else
                    {
                        //审批信息
                        var approvalOperateInfo = string.Empty;
                        var approvalInfo = node.operateInfoList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Approval).FirstOrDefault();
                        if (approvalInfo != null)
                        {
                            approvalOperateInfo = string.Format("等待{0}{1}<br />", string.Join(",", approvalInfo.targetName), APPROVAL);
                        }

                        //会签信息
                        var countersignOperateInfo = string.Empty;
                        var countersignInfo = node.operateInfoList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Countersign).FirstOrDefault();
                        if (countersignInfo != null)
                        {
                            countersignOperateInfo = string.Format("等待{0}{1}<br />", string.Join(",", countersignInfo.targetName), COUNTER_SIGN);
                        }

                        //查阅信息
                        var readOperateInfo = string.Empty;
                        var readInfo = node.operateInfoList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Duplicate).FirstOrDefault();
                        if (readInfo != null)
                        {
                            readOperateInfo = string.Format("等待{0}{1}<br />", string.Join(",", readInfo.targetName), READ);
                        }

                        //提交信息
                        var submitOperateInfo = string.Empty;
                        var submitInfo = node.operateInfoList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Submit).FirstOrDefault();
                        if (submitInfo != null)
                        {
                            submitOperateInfo = string.Format("等待{0}{1}<br />", string.Join(",", submitInfo.targetName), SUBMIT);
                        }

                        if (approvalInfo == null && countersignInfo == null && submitInfo == null && readInfo == null)
                        {
                            info.operateInfo = "未找到符合条件的操作人";
                        }
                        else
                        {
                            info.operateInfo = approvalOperateInfo + countersignOperateInfo + submitOperateInfo + readOperateInfo;
                        }
                    }

                    operateInfoList.Add(info);
                }
            }

            if (operateInfoList.Count == 0)
            {
                var errorInfo = new OperateInfoModel();
                errorInfo.operateInfo = "未找到符合流程条件的操作信息";
                operateInfoList.Add(errorInfo);
            }

            return operateInfoList;
        }

        #endregion 组合表单节点操作人信息

        #region 根据表单操作类型取得操作名称

        /// <summary>
        /// 根据表单操作类型取得操作名称
        /// </summary>
        /// <param name="operateType">表单操作类型</param>
        /// <returns></returns>
        private string GetOperateNameById(int operateType)
        {
            var operateName = string.Empty;

            switch (operateType)
            {
                case (int)ConstVar.FormOperateType.Submit:
                    operateName = SUBMIT;
                    break;

                case (int)ConstVar.FormOperateType.Pass:
                    operateName = PASS;
                    break;

                case (int)ConstVar.FormOperateType.Return:
                    operateName = RETURN;
                    break;

                case (int)ConstVar.FormOperateType.Revoke:
                    operateName = REVOKE;
                    break;

                case (int)ConstVar.FormOperateType.Read:
                    operateName = READ;
                    break;
            }

            return operateName;
        }

        #endregion 根据表单操作类型取得操作名称

        #endregion 私有方法
    }
}