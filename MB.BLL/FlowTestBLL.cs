using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class FlowTestBLL : IFlowTestBLL
    {
        #region 变量区域

        /// <summary>流程共通对象</summary>
        private FlowCommonBLL flowCommon = new FlowCommonBLL();

        /// <summary>表单信息对象</summary>
        private AddFormContentModel formInfo = new AddFormContentModel();

        /// <summary>验证通过的流程条件ID</summary>
        private List<int> verifiedConditionIdList = new List<int>();

        #endregion 变量区域

        #region 常量区域

        //默认日期
        private const string DEFAULT_DATE = "0001-01-01";

        //默认数值
        private const string DEFAULT_NUMBER = "0";

        //测试率颜色
        private const string COLOR_RATIO = "#4A4AFF";

        //节点操作人测试率
        private const string OPERATE_TEST_RATIO = " ( {0}% ) ";

        #endregion 常量区域

        #region 取得创建用户的岗位信息

        /// <summary>
        /// 取得创建用户的岗位信息
        /// </summary>
        /// <param name="orgId">创建用户部门ID</param>
        /// <param name="userId">创建用户ID</param>
        /// <returns>岗位列表</returns>
        public List<StationModel> GetCreateUserStationList(int userId, int orgId)
        {
            var stationList = new List<StationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                stationList = (from us in db.tblUserStation
                               join s in db.tblStation
                               on us.stationId equals s.stationId
                               where us.userId == userId && s.organizationId == orgId
                               select new StationModel
                               {
                                   id = s.stationId,
                                   name = s.stationName
                               }).Distinct().ToList();
            }
            return stationList;
        }

        #endregion 取得创建用户的岗位信息

        #region 取得流程的有效节点信息

        /// <summary>
        /// 取得流程的有效节点信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public List<NodeModel> GetValidNodeInfoList(int templateId)
        {
            var nodeInfoList = flowCommon.GetValidNodeInfoList(templateId);
            return nodeInfoList;
        }

        #endregion 取得流程的有效节点信息

        #region 取得模版流程图信息

        /// <summary>
        /// 取得模版流程图信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns>l流程图信息</returns>
        public TemplateFlowChartModel GetTemplateFlowChartInfo(int templateId)
        {
            //模版节点及操作信息
            var nodePlusOperateInfoList = new List<NodePlusOperateModel>();

            //取得模版节点信息
            var nodeInfoList = flowCommon.GetNodeList(templateId);

            foreach (var node in nodeInfoList)
            {
                //节点操作人信息
                var operateList = flowCommon.GetTemplateNodeOperateList(node.nodeId.Value);

                //已测试操作记录数
                var testedNum = operateList.Where(p => p.testFlag.Value).Count();
                var testRatio = Math.Round((decimal)testedNum / operateList.Count * 100, 2);

                //组合节点操作人信息
                var operateInfo = flowCommon.CombineTemplateOperateInfo(node.nodeType, operateList, true);

                var nodePlusOperate = new NodePlusOperateModel
                {
                    nodeId = node.nodeId,
                    nodeName = node.nodeName + string.Format(OPERATE_TEST_RATIO, testRatio.ToString()),
                    nodeType = node.nodeType,
                    operate = operateInfo,
                    testRatio = testRatio
                };

                nodePlusOperateInfoList.Add(nodePlusOperate);
            }

            //节点出口及条件信息
            var linkPlusConditionInfoList = new List<LinkPlusConditionModel>();
            decimal linkTestRatio = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                //取得节点出口信息列表
                var nodeLinkList = flowCommon.GetNodeLinkList(db, templateId);
                if (nodeLinkList.Count > 0)
                {
                    var testedNum = nodeLinkList.Where(p => p.testFlag.Value).Count();
                    linkTestRatio = Math.Round((decimal)testedNum / nodeLinkList.Count * 100, 2);
                    foreach (var item in nodeLinkList)
                    {
                        //取得节点出口的流程条件信息
                        var conditionList = flowCommon.GetLinkConditionInfo(db, item.linkId.Value, templateId);

                        //取得节点出口的流程公式信息
                        var formulaList = flowCommon.GetLinkFormulaInfo(db, item.linkId.Value);

                        //组合节点出口的流程条件信息
                        var conditionInfo = flowCommon.CombineConditionInfo(conditionList, formulaList, true);

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
                            condition = conditionInfo,
                            testFlag = item.testFlag
                        };

                        linkPlusConditionInfoList.Add(linkInfo);
                    }
                }
            }

            //模版流程图信息
            var flowChartInfo = new TemplateFlowChartModel
            {
                nodeInfo = nodePlusOperateInfoList,
                linkInfo = linkPlusConditionInfoList,
                linkTestRatio = linkTestRatio
            };

            return flowChartInfo;
        }

        #endregion 取得模版流程图信息

        #region 判断表单创建者是否有效

        /// <summary>
        /// 判断表单创建者是否有效
        /// </summary>
        /// <param name="createUserId">表单创建用户ID</param>
        /// <param name="createNodeId">模版创建节点ID</param>
        /// <returns></returns>
        public bool CheckFormCreateUser(int createUserId, int createNodeId)
        {
            var result = false;

            using (var db = new TargetNavigationDBEntities())
            {
                var operateList = flowCommon.GetNodeOperateList(db, createNodeId, createUserId);
                if (operateList.Count > 0)
                {
                    foreach (var item in operateList)
                    {
                        if ((item.userIds.Contains(createUserId)) || item.type == (int)ConstVar.NodeOperatorType.All)
                        {
                            result = true;
                            //更新节点操作人测试标志
                            int?[] operateId = { item.operateId };
                            UpdateOperateTestFlag(db, operateId);
                            break;
                        }
                    }
                }
                db.SaveChanges();
            }

            return result;
        }

        #endregion 判断表单创建者是否有效

        #region 表单操作处理

        /// <summary>
        /// 表单操作处理
        /// </summary>
        /// <param name="formModel">表单信息</param>
        /// <param name="status">节点出口状态</param>
        /// <returns>True：成功 False：失败</returns>
        public bool FormOperate(AddFormContentModel formModel, ConstVar.LinkStatus status)
        {
            //设置测试用表单信息
            formInfo = formModel;

            using (var db = new TargetNavigationDBEntities())
            {
                //取得下一节点出口信息
                var nextLink = GetNextLinkId(db, status);

                if (nextLink.linkId.HasValue)
                {
                    //更新当前Link为测试通过
                    UpdateLinkTestFlag(db, nextLink.linkId.Value);

                    //更新当前Link条件公式中验证通过的条件为测试通过
                    UpdateVerifiedConditionTestFlag(db);

                    //取得下一节点的操作人
                    var operateInfoList = GetNodeOperateList(db, nextLink.nodeExitId.Value);
                    if (operateInfoList.Count > 0)
                    {
                        var operateId = operateInfoList.Select(p => p.operateId).ToArray();
                        //更新节点操作人为测试通过
                        UpdateOperateTestFlag(db, operateId);
                    }
                    else
                        return false;
                }
                else
                    return false;

                db.SaveChanges();
            }
            return true;
        }

        #endregion 表单操作处理

        #region 取得流程测试标志

        /// <summary>
        /// 取得流程测试标志
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns>流程测试标志</returns>
        public bool GetFlowTestFlag(int templateId)
        {
            //检查模版测试结果
            var testFlag = flowCommon.CheckTemplateTestStatus(templateId);

            //更新模版的测试标志
            flowCommon.UpdateTemplateTestFlag(templateId, testFlag);

            return testFlag;
        }

        #endregion 取得流程测试标志

        #region 私有方法

        #region 取得下一节点出口信息

        /// <summary>
        /// 取得下一节点出口信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <returns>下一节点出口信息</returns>
        private NodeLinkModel GetNextLinkId(TargetNavigationDBEntities db, ConstVar.LinkStatus status)
        {
            var nextLink = new NodeLinkModel();

            //取得入口节点为当前节点的节点流程
            var nextLinkList = flowCommon.GetNodeLinkList(db, formInfo.templateId).Where(p => p.status == (int)status && p.nodeEntryId == formInfo.currentNode).ToList();

            if (nextLinkList.Count > 0)
            {
                var validLinkList = new List<NodeLinkModel>();
                var noFormulaLinkList = new List<NodeLinkModel>();
                foreach (var item in nextLinkList)
                {
                    //取得节点出口对应的流程条件信息
                    var conditionInfo = flowCommon.GetLinkConditionInfo(db, item.linkId.Value, formInfo.templateId);
                    //取得节点出口对应的条件公式信息
                    var formulaInfo = flowCommon.GetLinkFormulaInfo(db, item.linkId.Value);
                    // 判断流程条件公式是否成立
                    if (CheckFormula(db, conditionInfo, formulaInfo))
                    {
                        validLinkList.Add(item);
                    }
                    if (formulaInfo.Count == 0)
                    {
                        noFormulaLinkList.Add(item);
                    }
                }
                if (validLinkList.Count == 1)
                {
                    nextLink = validLinkList[0];
                }
                else
                {
                    if (noFormulaLinkList.Count == 1 && validLinkList.Count == 0)
                    {
                        nextLink = noFormulaLinkList[0];
                    }
                }
            }
            return nextLink;
        }

        #endregion 取得下一节点出口信息

        #region 取得表单节点操作人信息

        /// <summary>
        /// 取得表单节点操作人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点ID</param>
        /// <returns>节点操作人信息</returns>
        private List<NodeOperateModel> GetNodeOperateList(TargetNavigationDBEntities db, int nodeId)
        {
            var operateInfoList = new List<NodeOperateModel>();

            //表单申请人信息
            var userInfo = new UserInfoSimpleModel
            {
                organizationId = formInfo.organizationId,
                stationId = formInfo.stationId,
                userId = formInfo.createUser
            };

            operateInfoList = flowCommon.GetNodeOperateList(db, nodeId, userInfo, true);

            return operateInfoList;
        }

        #endregion 取得表单节点操作人信息

        #region 判断流程条件公式是否成立

        /// <summary>
        ///  判断流程条件公式是否成立
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="conditionInfoList">条件信息列表</param>
        /// <param name="formulaInfoList">公式信息列表</param>
        /// <returns>false：条件不成立 true：条件成立</returns>
        private bool CheckFormula(TargetNavigationDBEntities db, List<LinkConditionModel> conditionInfoList, List<LinkFormulaModel> formulaInfoList)
        {
            if (conditionInfoList.Count == 0 || formulaInfoList.Count == 0)
            {
                return false;
            }

            //流程条件公式
            var formula = new StringBuilder();

            foreach (var item in formulaInfoList)
            {
                //条件ID和操作符不能同时为空或同时不为空
                if ((!item.conditionId.HasValue && string.IsNullOrEmpty(item.operate)) || (item.conditionId.HasValue && !string.IsNullOrEmpty(item.operate)))
                {
                    return false;
                }

                if (item.operate != null)
                {
                    switch (item.operate)
                    {
                        case "|":
                            formula.Append("||");
                            break;

                        case "&":
                            formula.Append("&&");
                            break;

                        default:
                            formula.Append(item.operate);
                            break;
                    }
                }

                if (item.conditionId != null)
                {
                    //取得流程ID对应的条件信息
                    var conInfo = conditionInfoList.Where(p => p.conditionId == item.conditionId && p.linkId == item.linkId).FirstOrDefault();

                    //判断该条件结果
                    if (CheckConditionResult(db, conInfo))
                    {
                        //添加验证通过的条件ID（测试标志更新用）
                        verifiedConditionIdList.Add(conInfo.conditionId.Value);

                        formula.Append("true");
                    }
                    else
                    {
                        formula.Append("false");
                    }
                }
            }

            //判断流程条件公式结果
            var result = StringUtils.StringCalculate(formula.ToString());
            if (result == null)
            {
                return false;
            }

            return (bool)result;
        }

        #endregion 判断流程条件公式是否成立

        #region 判断流程条件结果

        /// <summary>
        ///  判断流程条件结果
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="conditionInfo">条件信息</param>
        /// <returns>false：条件不成立 true：条件成立</returns>
        private bool CheckConditionResult(TargetNavigationDBEntities db, LinkConditionModel conditionInfo)
        {
            var flag = false;

            //条件类型
            switch (conditionInfo.type)
            {
                #region 组织架构

                case (int)ConstVar.ConditionType.Organization:
                    //属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                    {
                        flag = conditionInfo.targetId.Contains(formInfo.organizationId) ? true : false;
                    }
                    //不属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.NotBelong)
                    {
                        flag = conditionInfo.targetId.Contains(formInfo.organizationId) ? false : true;
                    }
                    break;

                #endregion 组织架构

                #region 岗位

                case (int)ConstVar.ConditionType.Station:
                    //属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                    {
                        flag = conditionInfo.targetId.Contains(formInfo.stationId) ? true : false;
                    }
                    //不属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.NotBelong)
                    {
                        flag = conditionInfo.targetId.Contains(formInfo.stationId) ? false : true;
                    }
                    break;

                #endregion 岗位

                #region 人力资源

                case (int)ConstVar.ConditionType.User:
                    //属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                    {
                        flag = conditionInfo.targetId.Contains(formInfo.createUser) ? true : false;
                    }
                    //不属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.NotBelong)
                    {
                        flag = conditionInfo.targetId.Contains(formInfo.createUser) ? false : true;
                    }
                    break;

                #endregion 人力资源

                #region 控件

                case (int)ConstVar.ConditionType.Control:
                    //比较流程条件的控件值
                    flag = CompareControlValue(db, conditionInfo);
                    break;

                #endregion 控件
            }

            return flag;
        }

        #endregion 判断流程条件结果

        #region 比较流程条件的控件值

        /// <summary>
        /// 比较流程条件的控件值
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="conditionInfo">流程条件信息</param>
        /// <returns>比较结果</returns>
        private bool CompareControlValue(TargetNavigationDBEntities db, LinkConditionModel conditionInfo)
        {
            var flag = false;

            //取得表单控件ID对应的控件值
            var controlValue = formInfo.controlValue.Where(p => p.controlId == conditionInfo.controlId).FirstOrDefault().rowNumberList[0].detailValue[0];
            //表单对应的模版ID
            var templateId = formInfo.templateId;
            //取得控件信息
            var controlInfo = flowCommon.GetControlInfoById(db, conditionInfo.controlId, templateId);

            if (controlInfo.controlType == (int)ConstVar.ControlType.Date || controlInfo.controlType == (int)ConstVar.ControlType.DateTime)
            {
                if (string.IsNullOrWhiteSpace(controlValue))
                {
                    controlValue = DEFAULT_DATE;
                }
                var conditonDateTime = Convert.ToDateTime(conditionInfo.value.Trim());
                var formDateTime = Convert.ToDateTime(controlValue.Trim());
                var result = DateTime.Compare(formDateTime, conditonDateTime);

                //等于
                if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Equal)
                {
                    flag = result == 0 ? true : false;
                }
                //大于
                if (conditionInfo.condition == (int)ConstVar.ConditionOperate.More)
                {
                    flag = result > 0 ? true : false;
                }
                //小于
                if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Less)
                {
                    flag = result < 0 ? true : false;
                }
            }
            else if (controlInfo.controlType == (int)ConstVar.ControlType.MoneyLower || controlInfo.controlType == (int)ConstVar.ControlType.NumberInput)
            {
                if (string.IsNullOrWhiteSpace(controlValue))
                {
                    controlValue = DEFAULT_NUMBER;
                }
                var conditionNum = Convert.ToDecimal(conditionInfo.value.Trim());
                var formNum = Convert.ToDecimal(controlValue.Trim());
                var result = formNum - conditionNum;
                //等于
                if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Equal)
                {
                    flag = result == 0 ? true : false;
                }
                //大于
                if (conditionInfo.condition == (int)ConstVar.ConditionOperate.More)
                {
                    flag = result > 0 ? true : false;
                }
                //小于
                if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Less)
                {
                    flag = result < 0 ? true : false;
                }
            }

            return flag;
        }

        #endregion 比较流程条件的控件值

        #region 更新节点出口测试标志

        /// <summary>
        /// 更新节点出口测试标志
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="linkId">节点出口ID</param>
        private void UpdateLinkTestFlag(TargetNavigationDBEntities db, int linkId)
        {
            var link = db.tblNodeLink.Where(p => p.linkId == linkId && p.templateId == formInfo.templateId).FirstOrDefault();

            if (link != null)
            {
                link.testFlag = true;
            }
        }

        #endregion 更新节点出口测试标志

        #region 更新条件公式中验证通过的条件为测试通过

        /// <summary>
        /// 更新条件公式中验证通过的条件为测试通过
        /// </summary>
        /// <param name="db">数据库上下文</param>
        private void UpdateVerifiedConditionTestFlag(TargetNavigationDBEntities db)
        {
            var conditionList = db.tblLinkCondition.Where(p => verifiedConditionIdList.Contains(p.conditionId));

            foreach (var item in conditionList)
            {
                item.testFlag = true;
            }
        }

        #endregion 更新条件公式中验证通过的条件为测试通过

        #region 更新节点操作人测试标志

        /// <summary>
        /// 更新节点操作人测试标志
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="operateIdList">节点操作ID</param>
        private void UpdateOperateTestFlag(TargetNavigationDBEntities db, int?[] operateIdList)
        {
            var operateList = db.tblNodeOperate.Where(p => operateIdList.Contains(p.operateId));

            foreach (var item in operateList)
            {
                item.testFlag = true;
            }
        }

        #endregion 更新节点操作人测试标志

        #endregion 私有方法
    }
}