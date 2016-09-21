using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class FlowCommonBLL : IFlowCommonBLL
    {
        #region 常量区域

        #region 控件名称

        private const string TAG = "标签";

        private const string TEXT_BOX = "文本输入框";

        private const string NUMBER_INPUT = "数字输入框";

        private const string MONEY_LOWER = "金额小写";

        private const string RADION_BUTTON = "单选框";

        private const string CHECK_BOX = "复选框";

        private const string COMBO_BOX = "下拉列表";

        private const string BROWSE_HR = "浏览人力资源";

        private const string BROWSE_ORG = "浏览组织架构";

        private const string BROWSE_DOC = "浏览文件";

        private const string LINE = "分割线";

        private const string MULTI_TEXT_BOX = "多行文本框";

        private const string DATE = "日期";

        private const string DATE_RANGE = "日期区间";

        private const string DATE_TIME = "日期时间";

        private const string DATE_TIME_RANGE = "日期时间区间";

        private const string DETAIL_LIST = "明细列表";

        private const string MONEY_UPPER = "金额大写";

        private const string TWO_COLUMN_ROW = "一行两列";

        private const string THREE_COLUMN_ROW = "一行三列";

        #endregion 控件名称

        #region 控件默认值

        private const string DEFAULT_DATE = "0001-01-01";

        private const string DEFAULT_NUMBER = "0";

        #endregion 控件默认值

        #region 流程相关

        //设置文本颜色
        private const string SET_TEXT_COLOR = "<font color='{0}'>{1}</font>";

        //文本颜色
        private const string COLOR_OPERATOR_TESTED = "#80FFFF";

        private const string COLOR_OPERATOR_NOT_TESTED = "#FFFFFF";
        private const string COLOR_CONDITION_TESTED = "#80FFFF";
        private const string COLOR_CONDITION_NOT_TESTED = "#FFFFFF";

        //节点操作人信息
        private const string NODE_OPERATE_INFO = "{0}、条件：{1}{2}{3}。<br />   操作者： {4}{5}可以{6}。<br />";

        //操作类型
        private const string CREATE = "创建";

        private const string APPROVAL = "审批";
        private const string COUNTER_SIGN = "会签";
        private const string SUBMIT = "提交";
        private const string ARCHIVE = "归档";
        private const string READ = "查看";

        //流程操作人人
        private const string OPERATOR_ORG = "操作者部门";

        private const string OPERATOR_STATION = "操作者岗位";
        private const string OPERATOR_USER = "操作者";
        private const string OPERATOR_SUPERIOR = "上级岗位";
        private const string OPERATOR_ALL = "所有人";

        //流程申请人
        private const string APPLICANT_ORG = "申请人部门";

        private const string APPLICANT_STATION = "申请人岗位";
        private const string APPLICANT_USER = "申请人";

        //条件操作
        private const string BELONG = "属于";

        private const string NOT_BELONG = "不属于";
        private const string EQUAL = "等于";
        private const string MORE = "大于";
        private const string LESS = "小于";

        //公式
        private const string FORMULA_OR = "或";

        private const string FORMULA_AND = "且";

        //分割字符
        private const string SPLIT_CHAR = "、";

        private const string AND_CHAR = "和";

        //流程条件
        private const string NO_FLOW_CONDITION = "流程条件为空";

        private const string FLOW_CHART_OR = " 或 ";
        private const string FLOW_CHART_AND = " 且 ";

        //批次条件
        private const string NO_BATCH_CONDITION = "空";

        private const string MESSAGE_NODE_SET_ERROR = "节点【{0}】：第{1}条操作者记录设置不正确！";

        #endregion 流程相关

        #endregion 常量区域

        #region 节点设置验证

        /// <summary>
        /// 验证节点设置是否正确
        /// </summary>
        /// <param name="nodeInfoList">流程节点信息</param>
        /// <param name="message">提示错误信息</param>
        /// <returns>true：设置正确 flase：设置错误</returns>
        public bool CheckNode(List<NodeInfoModel> nodeInfoList, out string message)
        {
            message = string.Empty;
            // 流程节点列表不为空时
            if (nodeInfoList.Count > 0)
            {
                foreach (var nodeInfo in nodeInfoList)
                {
                    if (!CheckOperatorNum(nodeInfo.node, nodeInfo.nodeOperate, out message))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion 节点设置验证

        #region 流程设置验证

        /// <summary>
        /// 验证流程设置是否正确
        /// </summary>
        /// <param name="nodeLink">节点流程列表</param>
        /// <returns>true：设置正确 flase：设置错误</returns>
        public bool CheckFlow(List<NodeLinkModel> nodeLink)
        {
            // 节点流程列表为空时
            if (nodeLink.Count == 0)
            {
                return false;
            }

            var haveCreateNode = false;
            var haveEndNode = false;

            foreach (var item in nodeLink)
            {
                // 入口为"创建"类型节点
                if (item.nodeEntryType == (int)ConstVar.NodeType.Create)
                {
                    haveCreateNode = true;
                }

                // 出口为"归档"类型节点
                if (item.nodeExitType == (int)ConstVar.NodeType.End)
                {
                    haveEndNode = true;
                }

                // 入口为"归档" 或 出口为"创建"
                if (item.nodeEntryType == (int)ConstVar.NodeType.End || item.nodeExitType == (int)ConstVar.NodeType.Create)
                {
                    return false;
                }

                // 入口节点类型为"提交"或出口节点类型为"归档"时，必须为通过
                if ((item.nodeEntryType == (int)ConstVar.NodeType.Submit || item.nodeExitType == (int)ConstVar.NodeType.End) && item.status == 0)
                {
                    return false;
                }

                // 除"创建"和"归档"节点外，其他节点必须同时存在于入口和出口节点中
                if (item.nodeEntryType == (int)ConstVar.NodeType.Submit || item.nodeEntryType == (int)ConstVar.NodeType.Approval)
                {
                    var result = nodeLink.Where(p => p.nodeExitId == item.nodeEntryId).ToList().Count();

                    if (result == 0)
                    {
                        return false;
                    }
                }

                // 入口节点类型为"审批"时，必须同时存在通过和不通过
                if (item.nodeEntryType == (int)ConstVar.NodeType.Approval)
                {
                    var status = item.status == 0 ? 1 : 0;
                    var result = nodeLink.Where(p => p.nodeEntryId == item.nodeEntryId && p.status == status).ToList().Count();

                    if (result == 0)
                    {
                        return false;
                    }
                }
            }

            // 入口节点中没有"创建"类型的节点
            if (!haveCreateNode)
            {
                return false;
            }

            // 出口节点中没有"归档"类型的节点
            if (!haveEndNode)
            {
                return false;
            }

            return true;
        }

        #endregion 流程设置验证

        #region 插入模版新增未设置控件的字段权限

        /// <summary>
        ///  插入模版新增未设置控件的字段权限
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public void InsertDefaultNodeField(int templateId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var nodeList = GetNodeList(templateId);

                foreach (var node in nodeList)
                {
                    var fieldControl = (from field in db.tblNodeField
                                        where field.nodeId == node.nodeId
                                        select field.controlId
                                ).ToList();

                    var templateControl = (from control in db.tblTemplateControl
                                           where control.templateId == templateId && !fieldControl.Contains(control.controlId)
                                           select control.controlId).ToList();

                    if (templateControl.Count > 0)
                    {
                        var fieldStatus = node.nodeType == (int)ConstVar.NodeType.Create ? (int)ConstVar.nodeControlStatus.edit : (int)ConstVar.nodeControlStatus.readOnly;
                        foreach (var id in templateControl)
                        {
                            //插入节点字段表
                            var nodeField = new tblNodeField
                            {
                                controlId = id,
                                nodeId = node.nodeId.Value,
                                status = fieldStatus
                            };
                            db.tblNodeField.Add(nodeField);
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        #endregion 插入模版新增未设置控件的字段权限

        #region 取得节点设置信息

        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public List<NodeInfoModel> GetNodeInfoList(int templateId)
        {
            var nodeInfoLsit = new List<NodeInfoModel>();

            var nodeList = GetNodeList(templateId);
            if (nodeList.Count > 0)
            {
                foreach (var node in nodeList)
                {
                    var nodeInfo = new NodeInfoModel
                    {
                        node = node,
                        nodeOperate = GetTemplateNodeOperateList(node.nodeId.Value)
                    };

                    nodeInfoLsit.Add(nodeInfo);
                }
            }

            return nodeInfoLsit;
        }

        #endregion 取得节点设置信息

        #region 取得流程节点信息

        /// <summary>
        /// 取得流程节点信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns>流程节点列表</returns>
        public List<NodeModel> GetNodeList(int templateId)
        {
            var nodeList = new List<NodeModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                nodeList = (from f in db.tblFlowNode
                            where f.templateId == templateId
                            orderby f.nodeType
                            select new NodeModel
                            {
                                nodeId = f.nodeId,
                                nodeName = f.nodeName,
                                nodeType = f.nodeType
                            }).ToList();
            }

            return nodeList;
        }

        #endregion 取得流程节点信息

        #region 取得节点流程

        /// <summary>
        /// 取得节点流程
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="templateId">模版ID</param>
        /// <returns>节点流程信息</returns>
        public List<NodeLinkModel> GetNodeLinkList(TargetNavigationDBEntities db, int templateId)
        {
            var linkList = new List<NodeLinkModel>();

            linkList = (from link in db.tblNodeLink
                        join entry in db.tblFlowNode
                        on link.nodeEntry equals entry.nodeId into entry
                        from entryNode in entry.DefaultIfEmpty()
                        join exit in db.tblFlowNode
                        on link.nodeExit equals exit.nodeId into exit
                        from exitNode in exit.DefaultIfEmpty()
                        where link.templateId == templateId
                        select new NodeLinkModel
                        {
                            templateId = link.templateId,
                            linkId = link.linkId,
                            nodeEntryId = entryNode.nodeId,
                            nodeEntryName = entryNode.nodeName,
                            nodeEntryType = entryNode.nodeType,
                            nodeExitId = exitNode.nodeId,
                            nodeExitName = exitNode.nodeName,
                            nodeExitType = exitNode.nodeType,
                            status = link.status,
                            testFlag = link.testFlag
                        }).ToList();

            return linkList;
        }

        /// <summary>
        /// 取得节点流程
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns>节点流程信息</returns>
        public List<NodeLinkModel> GetNodeLinkList(int templateId)
        {
            var linkList = new List<NodeLinkModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                linkList = this.GetNodeLinkList(db, templateId);
            }
            return linkList;
        }

        #endregion 取得节点流程

        #region 取得流程条件信息

        /// <summary>
        /// 取得流程条件信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="linkId">节点出口ID</param>
        /// <param name="templateId">模版ID</param>
        /// <returns>流程条件信息</returns>
        public List<LinkConditionModel> GetLinkConditionInfo(TargetNavigationDBEntities db, int linkId, int templateId)
        {
            var conditionInfoList = new List<LinkConditionModel>();

            //取得流程条件信息
            conditionInfoList = (from p in db.tblLinkCondition
                                 where p.linkId == linkId
                                 select new LinkConditionModel
                                 {
                                     conditionId = p.conditionId,
                                     linkId = p.linkId,
                                     type = p.type,
                                     condition = p.condition,
                                     controlId = p.controlId,
                                     value = p.value,
                                     testFlag = p.testFlag
                                 }).ToList();

            if (conditionInfoList.Count > 0)
            {
                //流程条件结果取得
                foreach (var info in conditionInfoList)
                {
                    //取得条件结果目标ID
                    var targetId = (from r in db.tblLinkResult where r.conditionId == info.conditionId select r.targetId).ToArray();
                    info.targetId = targetId;
                    //条件类型 1：组织架构 2：岗位 3：人力资源 4：控件
                    switch (info.type)
                    {
                        case (int)ConstVar.ConditionType.Organization:
                            info.targetName = GetOrganizaiotnNameById(db, targetId);
                            break;

                        case (int)ConstVar.ConditionType.Station:
                            info.targetName = GetStationNameById(db, targetId);
                            break;

                        case (int)ConstVar.ConditionType.User:
                            info.targetName = GetUserNameByUserIdList(db, targetId);
                            break;

                        case (int)ConstVar.ConditionType.Control:
                            var controlInfo = this.GetControlInfoById(db, info.controlId, templateId);
                            if (controlInfo != null)
                            {
                                info.controlTitle = controlInfo.controlTitle;
                                info.controlType = controlInfo.controlType;
                            }
                            break;
                    }
                }
            }

            return conditionInfoList;
        }

        #endregion 取得流程条件信息

        #region 取得流程条件相关信息

        /// <summary>
        /// 取得流程条件相关信息
        /// </summary>
        /// <param name="linkId">节点出口ID</param>
        /// <param name="templateId">模版ID</param>
        /// <returns>流程条件信息</returns>
        public LinkConditionInfoModel GetLinkConditionRelatedInfo(int? linkId, int templateId)
        {
            var LinkConditionInfo = new LinkConditionInfoModel();

            using (var db = new TargetNavigationDBEntities())
            {
                //取得模版控件信息
                var controlInfoList = this.GetTemplateControlInfo(db, templateId);

                //取得流程条件
                var conditionInfoList = new List<LinkConditionModel>();
                //取得流程公式
                var formulaInfoList = new List<LinkFormulaModel>();

                if (linkId.HasValue)
                {
                    //取得流程条件
                    conditionInfoList = this.GetLinkConditionInfo(db, linkId.Value, templateId);
                    //取得流程公式
                    formulaInfoList = this.GetLinkFormulaInfo(db, linkId.Value);
                }

                //组合流程公式
                var formulaInfo = this.CombineConditionFormula(db, formulaInfoList);
                LinkConditionInfo.controlInfoList = controlInfoList;
                LinkConditionInfo.linkConditionList = conditionInfoList;
                LinkConditionInfo.linkFormulaInfo = formulaInfo;
            }
            return LinkConditionInfo;
        }

        #endregion 取得流程条件相关信息

        #region 取得模版节点操作人信息

        /// <summary>
        /// 取得模版节点操作人信息
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>节点操作人信息</returns>
        public List<NodeOperateModel> GetTemplateNodeOperateList(int nodeId)
        {
            var nodeOperateInfo = new List<NodeOperateModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                //取得节点操作人信息
                nodeOperateInfo = (from n in db.tblNodeOperate
                                   where n.nodeId == nodeId
                                   select new NodeOperateModel
                                   {
                                       operateId = n.operateId,
                                       nodeId = n.nodeId,
                                       type = n.type,
                                       condition = n.condition,
                                       countersign = n.countersign,
                                       batchType = n.batchType,
                                       batchCondition = n.batchCondition,
                                       orderNum = n.orderNum,
                                       testFlag = n.testFlag
                                   }).ToList();

                if (nodeOperateInfo.Count > 0)
                {
                    foreach (var item in nodeOperateInfo)
                    {
                        //节点操作人结果取得
                        //取得节点操作人目标ID
                        var targetId = (from p in db.tblOperateResult where p.operateId == item.operateId select p.targetId).ToArray();
                        item.targetId = targetId;
                        //类型 1：操作者部门 2：操作者岗位 3：操作者 4：上级岗位 5：所有人
                        switch (item.type)
                        {
                            case (int)ConstVar.NodeOperatorType.Organization:
                                item.targetName = GetOrganizaiotnNameById(db, targetId);
                                break;

                            case (int)ConstVar.NodeOperatorType.Station:
                                item.targetName = GetStationNameById(db, targetId);
                                break;

                            case (int)ConstVar.NodeOperatorType.User:
                                item.targetName = GetUserNameByUserIdList(db, targetId);
                                break;

                            case (int)ConstVar.NodeOperatorType.Superior:
                            case (int)ConstVar.NodeOperatorType.All:
                                break;
                        }

                        //批次条件结果取得
                        //取得批次条件结果目标ID
                        var batchTarget = (from b in db.tblBatchResult where b.operateId == item.operateId select b.targetId).ToArray();
                        item.batchTargetId = batchTarget;
                        //批次条件 1：申请人部门 2：申请人岗位 3：申请人
                        switch (item.batchType)
                        {
                            case (int)ConstVar.BatchType.Organization:
                                item.batchTargetName = GetOrganizaiotnNameById(db, batchTarget);
                                break;

                            case (int)ConstVar.BatchType.Station:
                                item.batchTargetName = GetStationNameById(db, batchTarget);
                                break;

                            case (int)ConstVar.BatchType.User:
                                item.batchTargetName = GetUserNameByUserIdList(db, batchTarget);
                                break;
                        }
                    }
                }
            }

            return nodeOperateInfo;
        }

        #endregion 取得模版节点操作人信息

        #region 组合流程条件信息

        /// <summary>
        ///  组合流程条件信息
        /// </summary>
        /// <param name="conditionList">流程条件信息</param>
        /// <param name="formulaInfoList">流程条件信息</param>
        /// <param name="isTest">是否为测试画面</param>
        /// <returns>流程操作信息</returns>
        public string CombineConditionInfo(List<LinkConditionModel> conditionList, List<LinkFormulaModel> formulaInfoList, bool isTest = false)
        {
            var displayInfoText = new StringBuilder();
            foreach (var item in formulaInfoList)
            {
                //操作符不为空
                if (item.operate != null)
                {
                    switch (item.operate)
                    {
                        case "|":
                            displayInfoText.Append(FLOW_CHART_OR);
                            break;

                        case "&":
                            displayInfoText.Append(FLOW_CHART_AND);
                            break;

                        default:
                            displayInfoText.Append(item.operate);
                            break;
                    }
                }

                //条件ID不为空
                if (item.conditionId != null)
                {
                    var conditionText = new StringBuilder();

                    //取得流程ID对应的条件信息
                    var conInfo = conditionList.Where(p => p.conditionId == item.conditionId && p.linkId == item.linkId).FirstOrDefault();

                    switch (conInfo.type.Value)
                    {
                        //部门
                        case (int)ConstVar.ConditionType.Organization:
                            conditionText.Append(APPLICANT_ORG);

                            if (conInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                                conditionText.Append(BELONG);
                            else
                                conditionText.Append(NOT_BELONG);

                            conditionText.Append(string.Join(SPLIT_CHAR, conInfo.targetName));
                            break;

                        //岗位
                        case (int)ConstVar.ConditionType.Station:
                            conditionText.Append(APPLICANT_STATION);

                            if (conInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                                conditionText.Append(BELONG);
                            else
                                conditionText.Append(NOT_BELONG);

                            conditionText.Append(string.Join(SPLIT_CHAR, conInfo.targetName));
                            break;

                        //人员
                        case (int)ConstVar.ConditionType.User:
                            conditionText.Append(APPLICANT_USER);

                            if (conInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                                conditionText.Append(BELONG);
                            else
                                conditionText.Append(NOT_BELONG);

                            conditionText.Append(string.Join(SPLIT_CHAR, conInfo.targetName));
                            break;

                        //控件
                        case (int)ConstVar.ConditionType.Control:
                            conditionText.Append(conInfo.controlTitle);

                            if (conInfo.condition == (int)ConstVar.ConditionOperate.Equal)
                                conditionText.Append(EQUAL);
                            else if (conInfo.condition == (int)ConstVar.ConditionOperate.More)
                                conditionText.Append(MORE);
                            else
                                conditionText.Append(LESS);

                            conditionText.Append(conInfo.value);
                            break;
                    }

                    var text = conditionText.ToString();

                    //测试画面用
                    if (isTest)
                    {
                        //字体颜色
                        var fontColor = conInfo.testFlag.Value ? COLOR_CONDITION_TESTED : COLOR_CONDITION_NOT_TESTED;
                        text = string.Format(SET_TEXT_COLOR, fontColor, text);
                    }

                    displayInfoText.Append(text);
                }
            }

            if (displayInfoText.ToString() == string.Empty)
                displayInfoText.Append(NO_FLOW_CONDITION);

            return displayInfoText.ToString();
        }

        #endregion 组合流程条件信息

        #region 组合模版节点操作人信息

        /// <summary>
        ///  组合模版节点操作人信息
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <param name="nodeOperateInfoList">节点操作人信息列表</param>
        /// <param name="isTest">是否为测试画面</param>
        /// <returns>模版节点操作人信息</returns>
        public string CombineTemplateOperateInfo(int? nodeType, List<NodeOperateModel> nodeOperateInfoList, bool isTest = false)
        {
            //节点类型
            nodeType = nodeType.HasValue ? nodeType.Value : 0;

            var operateInfo = new StringBuilder();
            var serialNo = 0;
            foreach (var item in nodeOperateInfoList)
            {
                serialNo += 1;
                //批次条件类型
                var batchType = string.Empty;
                switch (item.batchType.Value)
                {
                    case (int)ConstVar.BatchType.Organization:
                        batchType = APPLICANT_ORG;
                        break;

                    case (int)ConstVar.BatchType.Station:
                        batchType = APPLICANT_STATION;
                        break;

                    case (int)ConstVar.BatchType.User:
                        batchType = APPLICANT_USER;
                        break;
                }

                //批次条件
                var batchCondition = string.Empty;
                //批次条件结果
                var batchTargetName = string.Empty;
                if (item.batchTargetName != null && item.batchTargetName.Length > 0)
                {
                    batchTargetName = string.Join(SPLIT_CHAR, item.batchTargetName);
                    var index = batchTargetName.LastIndexOf(SPLIT_CHAR);
                    if (index > 0)
                    {
                        Regex regex = new Regex(SPLIT_CHAR);
                        batchTargetName = regex.Replace(batchTargetName, AND_CHAR, 1, index);
                    }

                    if (item.batchCondition.Value == (int)ConstVar.ConditionOperate.Belong)
                        batchCondition = BELONG;
                    else
                        batchCondition = NOT_BELONG;
                }
                else
                    batchCondition = NO_BATCH_CONDITION;

                //操作条件类型
                var nodeOperateType = string.Empty;
                switch (item.type.Value)
                {
                    case (int)ConstVar.NodeOperatorType.Organization:
                        nodeOperateType = OPERATOR_ORG + BELONG;
                        break;

                    case (int)ConstVar.NodeOperatorType.Station:
                        nodeOperateType = OPERATOR_STATION + BELONG;
                        break;

                    case (int)ConstVar.NodeOperatorType.User:
                        nodeOperateType = OPERATOR_USER + BELONG;
                        break;

                    case (int)ConstVar.NodeOperatorType.Superior:
                        nodeOperateType = OPERATOR_SUPERIOR;
                        break;

                    case (int)ConstVar.NodeOperatorType.All:
                        nodeOperateType = OPERATOR_ALL;
                        break;
                }

                //操作人
                var targetName = string.Empty;
                if (item.targetName != null && item.targetName.Length > 0)
                {
                    targetName = string.Join(SPLIT_CHAR, item.targetName);
                    var index = targetName.LastIndexOf(SPLIT_CHAR);
                    if (index > 0)
                    {
                        Regex regex = new Regex(SPLIT_CHAR);
                        targetName = regex.Replace(targetName, AND_CHAR, 1, index);
                    }
                }

                //操作类型
                var operateType = string.Empty;
                switch (item.countersign.Value)
                {
                    case (int)ConstVar.NodeStatus.Approval:
                        if (nodeType == (int)ConstVar.NodeType.Create)
                            operateType = CREATE;
                        else
                            operateType = APPROVAL;
                        break;

                    case (int)ConstVar.NodeStatus.Countersign:
                        operateType = COUNTER_SIGN;
                        break;

                    case (int)ConstVar.NodeStatus.Duplicate:
                        operateType = READ;
                        break;

                    case (int)ConstVar.NodeStatus.Submit:
                        operateType = SUBMIT;
                        break;

                    case (int)ConstVar.NodeStatus.Archive:
                        operateType = ARCHIVE;
                        break;
                }

                var info = string.Format(NODE_OPERATE_INFO, serialNo, batchType, batchCondition, batchTargetName, nodeOperateType, targetName, operateType);

                if (isTest)
                {
                    //字体颜色
                    var fontColor = item.testFlag.Value ? COLOR_OPERATOR_TESTED : COLOR_OPERATOR_NOT_TESTED;
                    info = string.Format(SET_TEXT_COLOR, fontColor, info);
                }

                operateInfo.Append(info);
            }

            return operateInfo.ToString();
        }

        #endregion 组合模版节点操作人信息

        #region 取得表单节点操作人信息

        /// <summary>
        /// 取得表单节点操作人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点ID</param>
        ///  <param name="applicantUserId">表单申请人ID</param>
        ///  <param name="formId">表单ID</param>
        /// <returns>节点操作人信息</returns>
        public List<NodeOperateModel> GetNodeOperateList(TargetNavigationDBEntities db, int nodeId, int applicantUserId, int formId = 0)
        {
            //表单申请人信息
            var userInfo = GetCreateUserInfo(db, applicantUserId);

            if (formId != 0)
            {
                //取得表单信息
                var formInfo = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (formInfo != null)
                {
                    userInfo.stationId = formInfo.stationId;
                    userInfo.organizationId = formInfo.organizationId;
                }
            }

            //取得节点操作人信息
            var operateInfoList = GetNodeOperateList(db, nodeId, userInfo, false);

            return operateInfoList;
        }

        /// <summary>
        /// 取得表单节点操作人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点ID</param>
        ///  <param name="userInfo">表单申请人信息</param>
        ///  <param name="IsTest">是否为测试画面</param>
        /// <returns>节点操作人信息</returns>
        public List<NodeOperateModel> GetNodeOperateList(TargetNavigationDBEntities db, int nodeId, UserInfoSimpleModel userInfo, bool IsTest)
        {
            var operateInfoList = new List<NodeOperateModel>();

            //取得节点操作信息
            var operateList = (from n in db.tblNodeOperate
                               join f in db.tblFlowNode
                               on n.nodeId equals f.nodeId
                               where n.nodeId == nodeId
                               orderby n.orderNum
                               select new NodeOperateModel
                               {
                                   operateId = n.operateId,
                                   nodeId = n.nodeId,
                                   nodeType = f.nodeType,
                                   type = n.type,
                                   condition = n.condition,
                                   countersign = n.countersign,
                                   batchType = n.batchType,
                                   batchCondition = n.batchCondition,
                                   orderNum = n.orderNum
                               }).ToList();

            if (operateList.Count > 0)
            {
                //节点类型
                var nodeType = db.tblFlowNode.Where(p => p.nodeId == nodeId).FirstOrDefault().nodeType;

                //节点类型为创建的场合
                if (nodeType == (int)ConstVar.NodeType.Create)
                {
                    operateInfoList.AddRange(this.FiltrateOperateInfo(db, nodeType.Value, operateList, userInfo));
                }
                //节点类型为提交、审批、归档的场合
                else
                {
                    //审批的操作人信息(批次条件按申请人、岗位、部门、无排序）
                    //var approvalInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Approval).OrderByDescending(p => p.batchType).ThenByDescending(p => p.type).ToList();
                    var approvalInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Approval).ToList();
                    if (approvalInfo.Count > 0)
                    {
                        operateInfoList.AddRange(this.FiltrateOperateInfo(db, nodeType.Value, approvalInfo, userInfo));
                    }

                    //会签的操作人信息(批次条件按申请人、岗位、部门、无排序）
                    //var countersignInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Countersign).OrderByDescending(p => p.batchType).ThenByDescending(p => p.type).ToList();
                    var countersignInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Countersign).ToList();
                    if (countersignInfo.Count > 0)
                    {
                        operateInfoList.AddRange(this.FiltrateOperateInfo(db, nodeType.Value, countersignInfo, userInfo));
                    }

                    //提交的操作人信息(批次条件按申请人、岗位、部门、无排序）
                    //var submitInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Submit).OrderByDescending(p => p.batchType).ThenByDescending(p => p.type).ToList();
                    var submitInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Submit).ToList();
                    if (submitInfo.Count > 0)
                    {
                        operateInfoList.AddRange(this.FiltrateOperateInfo(db, nodeType.Value, submitInfo, userInfo));
                    }

                    //抄送的操作人信息(批次条件按申请人、岗位、部门、无排序）
                    //var duplicateInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Duplicate).OrderByDescending(p => p.batchType).ThenByDescending(p => p.type).ToList();
                    var duplicateInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Duplicate).ToList();
                    if (duplicateInfo.Count > 0)
                    {
                        operateInfoList.AddRange(this.FiltrateOperateInfo(db, nodeType.Value, duplicateInfo, userInfo));
                    }

                    //归档的操作人信息(批次条件按申请人、岗位、部门、无排序）
                    //var archiveInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Archive).OrderByDescending(p => p.batchType).ThenByDescending(p => p.type).ToList();
                    var archiveInfo = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Archive).ToList();
                    if (archiveInfo.Count > 0)
                    {
                        operateInfoList.AddRange(this.FiltrateOperateInfo(db, nodeType.Value, archiveInfo, userInfo));
                    }
                }
            }

            //筛选节点操作人中冗余的抄送人
            if (!IsTest)
            {
                operateInfoList = RemoveRedundancyOperateUser(operateInfoList);
            }

            //取得所有节点操作人名
            foreach (var info in operateInfoList)
            {
                info.targetName = this.GetUserNameByUserIdList(db, info.userIds);
            }

            return operateInfoList;
        }

        #endregion 取得表单节点操作人信息

        #region 取得下一操作节点ID

        /// <summary>
        /// 取得下一操作节点ID
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点ID</param>
        /// <param name="formId">表单ID</param>
        /// <param name="status">节点出口状态</param>
        /// <param name="userId">当前登录用户ID</param>
        /// <returns>下一操作节点</returns>
        public int GetNextOperateNodeId(TargetNavigationDBEntities db, int nodeId, int formId, ConstVar.LinkStatus status, int userId = 0)
        {
            var nextNodeId = 0;
            //取得表单信息
            var formInfo = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
            if (formInfo != null)
            {
                //表单申请人信息
                var userInfo = GetCreateUserInfo(db, formInfo.createUser);
                userInfo.stationId = formInfo.stationId;
                userInfo.organizationId = formInfo.organizationId;

                //取得表单ID对应的模版ID
                var templateId = formInfo.templateId;
                //取得入口节点为当前节点的节点流程
                var nextLinkList = GetNodeLinkList(db, templateId).Where(p => p.status == (int)status && p.nodeEntryId == nodeId).ToList();

                if (nextLinkList.Count > 0)
                {
                    var validLinkList = new List<NodeLinkModel>();
                    var noFormulaLinkList = new List<NodeLinkModel>();
                    foreach (var item in nextLinkList)
                    {
                        //取得节点出口对应的流程条件信息
                        var conditionInfo = GetLinkConditionInfo(db, item.linkId.Value, templateId);
                        //取得节点出口对应的条件公式信息
                        var formulaInfo = GetLinkFormulaInfo(db, item.linkId.Value);
                        // 判断流程条件公式是否成立
                        if (this.CheckFormula(db, formId, conditionInfo, formulaInfo, userInfo))
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
                        nextNodeId = validLinkList[0].nodeExitId.Value;
                    }
                    else
                    {
                        if (noFormulaLinkList.Count == 1 && validLinkList.Count == 0)
                        {
                            nextNodeId = noFormulaLinkList[0].nodeExitId.Value;
                        }
                    }
                }
            }

            //判断是否跳过下一操作节点
            if (CheckSkipNextNode(db, nodeId, nextNodeId, formId, userId))
            {
                nextNodeId = GetNextOperateNodeId(db, nextNodeId, formId, status, userId);
            }

            return nextNodeId;
        }

        #endregion 取得下一操作节点ID

        #region 取得流程条件公式信息

        /// <summary>
        /// 取得流程条件公式信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="linkId">节点出口ID</param>
        /// <returns>条件公式信息</returns>
        public List<LinkFormulaModel> GetLinkFormulaInfo(TargetNavigationDBEntities db, int linkId)
        {
            var formulaList = new List<LinkFormulaModel>();

            formulaList = (from f in db.tblLinkFormula
                           where f.linkId == linkId
                           orderby f.linkId, f.orderNum
                           select new LinkFormulaModel
                           {
                               formulaId = f.formulaId,
                               conditionId = f.conditionId,
                               linkId = f.linkId,
                               displayText = f.displayText,
                               operate = f.operate,
                               orderNum = f.orderNum
                           }).ToList();

            return formulaList;
        }

        #endregion 取得流程条件公式信息

        #region 取得表单创建用户信息

        /// <summary>
        ///  取得表单创建用户信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="userId">用户ID</param>
        /// <returns>表单申请人信息</returns>
        public UserInfoSimpleModel GetCreateUserInfo(TargetNavigationDBEntities db, int userId)
        {
            //取得表单创建者信息
            var createUserInfo = (from u in db.tblUser
                                  where u.userId == userId && !u.deleteFlag && u.workStatus == (int)ConstVar.workStatus.OnWork
                                  select new UserInfoSimpleModel
                                  {
                                      userId = u.userId,
                                      userName = u.userName,
                                      smallImage = u.smallImage
                                  }).FirstOrDefault();

            return createUserInfo;
        }

        #endregion 取得表单创建用户信息

        #region 根据控件类型取得控件类型名称

        /// <summary>
        /// 根据控件类型取得控件类型名称
        /// </summary>
        /// <param name="controlType">控件类型</param>
        /// <returns>流程节点列表</returns>
        public string GetControlTypeNameByType(int controlType)
        {
            var typeName = string.Empty;

            switch (controlType)
            {
                case (int)ConstVar.ControlType.Tag:
                    typeName = TAG;
                    break;

                case (int)ConstVar.ControlType.NumberInput:
                    typeName = NUMBER_INPUT;
                    break;

                case (int)ConstVar.ControlType.MoneyLower:
                    typeName = MONEY_LOWER;
                    break;

                case (int)ConstVar.ControlType.RadioButton:
                    typeName = RADION_BUTTON;
                    break;

                case (int)ConstVar.ControlType.CheckBox:
                    typeName = CHECK_BOX;
                    break;

                case (int)ConstVar.ControlType.ComboBox:
                    typeName = COMBO_BOX;
                    break;

                case (int)ConstVar.ControlType.BrowseHR:
                    typeName = BROWSE_HR;
                    break;

                case (int)ConstVar.ControlType.BrowseOrg:
                    typeName = BROWSE_ORG;
                    break;

                case (int)ConstVar.ControlType.BrowseDoc:
                    typeName = BROWSE_DOC;
                    break;

                case (int)ConstVar.ControlType.Line:
                    typeName = LINE;
                    break;

                case (int)ConstVar.ControlType.MultiTextBox:
                    typeName = MULTI_TEXT_BOX;
                    break;

                case (int)ConstVar.ControlType.Date:
                    typeName = DATE;
                    break;

                case (int)ConstVar.ControlType.DateRange:
                    typeName = DATE_RANGE;
                    break;

                case (int)ConstVar.ControlType.DateTime:
                    typeName = DATE_TIME;
                    break;

                case (int)ConstVar.ControlType.DateTimeRange:
                    typeName = DATE_TIME_RANGE;
                    break;

                case (int)ConstVar.ControlType.DetailList:
                    typeName = DETAIL_LIST;
                    break;

                case (int)ConstVar.ControlType.MoneyUpper:
                    typeName = MONEY_UPPER;
                    break;

                case (int)ConstVar.ControlType.TwoColumnRow:
                    typeName = TWO_COLUMN_ROW;
                    break;

                case (int)ConstVar.ControlType.ThreeColumnRow:
                    typeName = THREE_COLUMN_ROW;
                    break;
            }

            return typeName;
        }

        #endregion 根据控件类型取得控件类型名称

        #region 根据用户ID取得用户名

        /// <summary>
        ///  根据用户ID取得用户名
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="userId">用户ID</param>
        /// <returns>用户名</returns>
        public string GetUserNameById(TargetNavigationDBEntities db, int userId)
        {
            var userName = (from u in db.tblUser
                            where u.userId == userId
                            select u.userName
                                ).FirstOrDefault();

            return userName;
        }

        #endregion 根据用户ID取得用户名

        #region 取得操作人的被委托人信息

        /// <summary>
        /// 取得操作人的被委托人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="templateId">模版ID</param>
        /// <param name="userId">操作人ID</param>
        /// <returns></returns>
        public int GetMandataryUserInfo(TargetNavigationDBEntities db, int templateId, int userId)
        {
            var currentDate = System.DateTime.Now;

            var mandataryUser = (from entrust in db.tblFlowEntrust
                                 join result in db.tblEntrustResult
                                 on entrust.entrustId equals result.entrustId
                                 where result.templateId == templateId && entrust.entrustUser == userId &&
                                 currentDate >= entrust.startTime && currentDate <= entrust.endTime
                                 select entrust.mandataryUser
                                                 ).FirstOrDefault();

            mandataryUser = mandataryUser == 0 ? userId : mandataryUser;

            return mandataryUser;
        }

        #endregion 取得操作人的被委托人信息

        //#region 字符串运算
        ///// <summary>
        ///// 将字符串中的条件表达式按照正常的符号判断
        ///// </summary>
        ///// <param name="expression">条件表达式如：(true||flase)&&true  /param>
        ///// <returns>返回计算的值，可以为任意合法的数据类型</returns>
        //public object StringCalculate(string expression)
        //{
        //    object retvar = null;

        //    var provider = new Microsoft.CSharp.CSharpCodeProvider();
        //    var cp = new System.CodeDom.Compiler.CompilerParameters(new string[] { @"System.dll" });
        //    var builder = new StringBuilder("using System;class CalcExp{public static object Calc(){ return \"expression\";}}");
        //    builder.Replace("\"expression\"", expression);
        //    var code = builder.ToString();
        //    var results = provider.CompileAssemblyFromSource(cp, new string[] { code });
        //    if (results.Errors.HasErrors)
        //    {
        //        retvar = null;
        //    }
        //    else
        //    {
        //        System.Reflection.Assembly a = results.CompiledAssembly;
        //        Type t = a.GetType("CalcExp");
        //        retvar = t.InvokeMember("Calc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static |
        //            System.Reflection.BindingFlags.InvokeMethod, System.Type.DefaultBinder, null, null);
        //    }
        //    return retvar;
        //}

        //#endregion

        #region 根据控件ID取得控件信息

        /// <summary>
        ///  根据控件ID取得控件信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="templateId">模版ID</param>
        /// <returns>控件信息</returns>
        public ControlSimpleModel GetControlInfoById(TargetNavigationDBEntities db, string controlId, int templateId)
        {
            var info = (from t in db.tblTemplateControl
                        where t.controlId == controlId && t.templateId == templateId
                        select new ControlSimpleModel
                        {
                            controlId = t.controlId,
                            controlType = t.controlType,
                            controlTitle = t.title
                        }).FirstOrDefault();

            return info;
        }

        #endregion 根据控件ID取得控件信息

        #region 取得流程的有效节点信息

        /// <summary>
        /// 取得流程的有效节点信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public List<NodeModel> GetValidNodeInfoList(int templateId)
        {
            var nodeInfoList = new List<NodeModel>();

            var linkList = this.GetNodeLinkList(templateId).Where(p => p.status.Value == (int)ConstVar.LinkStatus.Pass).ToList();
            if (linkList.Count > 0)
            {
                foreach (var link in linkList)
                {
                    var entry = nodeInfoList.Where(p => p.nodeId == link.nodeEntryId).FirstOrDefault();
                    if (entry == null)
                    {
                        var nodeEntry = new NodeModel
                        {
                            nodeId = link.nodeEntryId,
                            nodeName = link.nodeEntryName,
                            nodeType = link.nodeEntryType,
                            templateId = link.templateId.Value
                        };
                        nodeInfoList.Add(nodeEntry);
                    }

                    var exit = nodeInfoList.Where(p => p.nodeId == link.nodeExitId).FirstOrDefault();
                    if (exit == null)
                    {
                        var nodeExit = new NodeModel
                        {
                            nodeId = link.nodeExitId,
                            nodeName = link.nodeExitName,
                            nodeType = link.nodeExitType,
                            templateId = link.templateId.Value
                        };
                        nodeInfoList.Add(nodeExit);
                    }
                }
            }
            return nodeInfoList;
        }

        #endregion 取得流程的有效节点信息

        #region 判断模版测试状态

        /// <summary>
        /// 判断模版测试状态
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public bool CheckTemplateTestStatus(int templateId)
        {
            //取得流程的有效节点信息
            var nodeList = GetValidNodeInfoList(templateId);

            if (nodeList.Count == 0) return false;

            foreach (var node in nodeList)
            {
                var notTestOperateNum = this.GetTemplateNodeOperateList(node.nodeId.Value).Where(p => !p.testFlag.Value).Count();
                if (notTestOperateNum > 0) return false;
            }

            var notTestLinkNum = this.GetNodeLinkList(templateId).Where(p => !p.testFlag.Value).Count();
            if (notTestLinkNum > 0) return false;

            return true;
        }

        #endregion 判断模版测试状态

        #region 更新模版测试标志

        /// <summary>
        /// 更新模版测试标志
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <param name="testFlag">模版测试标志</param>
        public void UpdateTemplateTestFlag(int templateId, bool testFlag)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var info = db.tblTemplate.Where(p => p.templateId == templateId).FirstOrDefault();

                if (info != null)
                {
                    info.testFlag = testFlag;
                }
                db.SaveChanges();
            }
        }

        #endregion 更新模版测试标志

        #region 私有方法

        #region 判断是否跳过下一操作节点

        /// <summary>
        /// 判断是否跳过下一操作节点
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">当前节点ID</param>
        /// <param name="nextNodeId">下一节点ID</param>
        /// <param name="formId">当前表单ID</param>
        /// <param name="userId">当前登录用户ID</param>
        /// <returns>True：需要跳过 False：不需要跳过</returns>
        private bool CheckSkipNextNode(TargetNavigationDBEntities db, int nodeId, int nextNodeId, int formId, int userId)
        {
            //流程图处理中取下一节点的场合
            if (userId == 0)
            {
                return false;
            }

            //不存在下一操作节点的场合
            if (nextNodeId == 0)
            {
                return false;
            }

            //取得下一节点信息
            var nextNodeInfo = db.tblFlowNode.Where(p => p.nodeId == nextNodeId).FirstOrDefault();
            //节点类型为不审批的场合
            if (nextNodeInfo.nodeType != (int)ConstVar.NodeType.Approval)
            {
                return false;
            }

            //取得当前节点不包括登录用户的未操作会签人
            var operatorInfo = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId && p.userId != userId && p.alreadyRead == (int)ConstVar.FormDuplicateStatus.countersign).ToList();
            //当前节点还有其他未操作会签人的场合
            if (operatorInfo.Count > 0)
            {
                return false;
            }

            //取得表单信息
            var formInfo = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();

            //取得下一节点的操作人
            var operateList = this.GetNodeOperateList(db, nextNodeId, formInfo.createUser, formId);
            if (operateList.Count == 1)
            {
                if (operateList[0].userIds.Length == 1 && operateList[0].countersign != (int)ConstVar.NodeStatus.Duplicate)
                {
                    //下一节点的唯一操作人为登陆用户的场合
                    if (operateList[0].userIds[0] == userId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion 判断是否跳过下一操作节点

        #region 判断流程条件公式是否成立

        /// <summary>
        ///  判断流程条件公式是否成立
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="formId">表单ID</param>
        /// <param name="conditionInfoList">条件信息列表</param>
        /// <param name="formulaInfoList">公式信息列表</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>false：条件不成立 true：条件成立</returns>
        private bool CheckFormula(TargetNavigationDBEntities db, int formId, List<LinkConditionModel> conditionInfoList, List<LinkFormulaModel> formulaInfoList, UserInfoSimpleModel userInfo)
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
                if ((item.conditionId == null && string.IsNullOrEmpty(item.operate)) || (item.conditionId != null && !string.IsNullOrEmpty(item.operate)))
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
                    if (CheckConditionResult(db, conInfo, userInfo, formId))
                    {
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
        /// <param name="userInfo">用户信息</param>
        /// <returns>false：条件不成立 true：条件成立</returns>
        private bool CheckConditionResult(TargetNavigationDBEntities db, LinkConditionModel conditionInfo, UserInfoSimpleModel userInfo, int formId)
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
                        flag = conditionInfo.targetId.Contains(userInfo.organizationId) ? true : false;
                    }
                    //不属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.NotBelong)
                    {
                        flag = conditionInfo.targetId.Contains(userInfo.organizationId) ? false : true;
                    }
                    break;

                #endregion 组织架构

                #region 岗位

                case (int)ConstVar.ConditionType.Station:
                    //属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                    {
                        flag = conditionInfo.targetId.Contains(userInfo.stationId) ? true : false;
                    }
                    //不属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.NotBelong)
                    {
                        flag = conditionInfo.targetId.Contains(userInfo.stationId) ? false : true;
                    }
                    break;

                #endregion 岗位

                #region 人力资源

                case (int)ConstVar.ConditionType.User:
                    //属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.Belong)
                    {
                        flag = conditionInfo.targetId.Contains(userInfo.userId) ? true : false;
                    }
                    //不属于
                    if (conditionInfo.condition == (int)ConstVar.ConditionOperate.NotBelong)
                    {
                        flag = conditionInfo.targetId.Contains(userInfo.userId) ? false : true;
                    }
                    break;

                #endregion 人力资源

                #region 控件

                case (int)ConstVar.ConditionType.Control:
                    //比较流程条件的控件值
                    flag = CompareControlValue(db, conditionInfo, formId);
                    break;

                #endregion 控件
            }

            return flag;
        }

        #endregion 判断流程条件结果

        #region 筛选符合流程条件的操作人信息

        /// <summary>
        /// 筛选符合流程条件的操作人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeOperateList">操作人信息</param>
        /// <param name="userInfo">申请人信息</param>
        /// <returns>筛选后的操作人信息</returns>
        private List<NodeOperateModel> FiltrateOperateInfo(TargetNavigationDBEntities db, int nodeType, List<NodeOperateModel> nodeOperateList, UserInfoSimpleModel userInfo)
        {
            var operateInfo = new List<NodeOperateModel>();

            if (nodeOperateList.Count > 0)
            {
                foreach (var item in nodeOperateList)
                {
                    //批次条件不为空的场合
                    if (item.batchType.Value != (int)ConstVar.BatchType.None)
                    {
                        if (CheckBatchResult(db, item, userInfo))
                        {
                            operateInfo.Add(SetNodeOperateInfo(db, item, userInfo));
                            return operateInfo;
                        }
                    }
                    //批次条件为空的场合
                    else
                    {
                        operateInfo.Add(SetNodeOperateInfo(db, item, userInfo));
                        //节点类型不为创建的场合
                        if (nodeType != (int)ConstVar.NodeType.Create)
                        {
                            return operateInfo;
                        }
                    }
                }
            }

            return operateInfo;
        }

        #endregion 筛选符合流程条件的操作人信息

        #region 判断批次条件

        /// <summary>
        /// 判断批次条件
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeOperateInfo">节点操作信息</param>
        ///  <param name="userInfo">用户信息</param>
        /// <returns>true: 符合批次条件 false: 不符合批次条件/returns>
        private bool CheckBatchResult(TargetNavigationDBEntities db, NodeOperateModel nodeOperateInfo, UserInfoSimpleModel userInfo)
        {
            var result = false;
            //批次条件结果取得

            //属于的场合
            if (nodeOperateInfo.batchCondition == (int)ConstVar.ConditionOperate.Belong)
            {
                //批次条件 1：申请人组织架构 2：申请人岗位 3：申请人
                switch (nodeOperateInfo.batchType)
                {
                    case (int)ConstVar.BatchType.Organization:
                        var orgCount = db.tblBatchResult.Where(p => p.targetId == userInfo.organizationId && p.operateId == nodeOperateInfo.operateId).Count();
                        if (orgCount > 0)
                        {
                            result = true;
                        }
                        break;

                    case (int)ConstVar.BatchType.Station:
                        var stationCount = db.tblBatchResult.Where(p => p.targetId == userInfo.stationId && p.operateId == nodeOperateInfo.operateId).Count();
                        if (stationCount > 0)
                        {
                            result = true;
                        }
                        break;

                    case (int)ConstVar.BatchType.User:
                        var userCount = db.tblBatchResult.Where(p => p.targetId == userInfo.userId && p.operateId == nodeOperateInfo.operateId).Count();
                        if (userCount > 0)
                        {
                            result = true;
                        }
                        break;
                }
            }

            //不属于的场合
            if (nodeOperateInfo.batchCondition == (int)ConstVar.ConditionOperate.NotBelong)
            {
                //批次条件 1：申请人组织架构 2：申请人岗位 3：申请人
                switch (nodeOperateInfo.batchType)
                {
                    case (int)ConstVar.BatchType.Organization:
                        var orgCount = db.tblBatchResult.Where(p => p.targetId != userInfo.organizationId && p.operateId == nodeOperateInfo.operateId).Count();
                        if (orgCount > 0)
                        {
                            result = true;
                        }
                        break;

                    case (int)ConstVar.BatchType.Station:
                        var stationCount = db.tblBatchResult.Where(p => p.targetId != userInfo.stationId && p.operateId == nodeOperateInfo.operateId).Count();
                        if (stationCount > 0)
                        {
                            result = true;
                        }
                        break;

                    case (int)ConstVar.BatchType.User:
                        var userCount = db.tblBatchResult.Where(p => p.targetId != userInfo.userId && p.operateId == nodeOperateInfo.operateId).Count();
                        if (userCount > 0)
                        {
                            result = true;
                        }
                        break;
                }
            }

            return result;
        }

        #endregion 判断批次条件

        #region 剔除节点操作人中冗余的抄送人

        /// <summary>
        /// 剔除节点操作人中冗余的抄送人
        /// </summary>
        /// <param name="operateInfoList">节点操作人信息</param>
        /// <returns>节点操作人信息</returns>
        private List<NodeOperateModel> RemoveRedundancyOperateUser(List<NodeOperateModel> operateInfoList)
        {
            var operateList = new List<NodeOperateModel>();
            //节点抄送操作人信息
            var duplicateInfo = operateInfoList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Duplicate).FirstOrDefault();
            //节点其他操作人信息
            var otherInfo = operateInfoList.SkipWhile(p => p.countersign == (int)ConstVar.NodeStatus.Duplicate).FirstOrDefault();

            if (duplicateInfo != null && otherInfo != null)
            {
                //抄送人
                var duplicateUserList = duplicateInfo.userIds.ToList();
                //剩余的抄送人
                var remainUserList = duplicateInfo.userIds.ToList();
                //其他人
                var otherUserList = otherInfo.userIds;

                foreach (var id in duplicateUserList)
                {
                    //抄送人ID已经在其他操作人ID的场合
                    if (otherUserList.Contains(id))
                    {
                        //去除的操作人ID
                        remainUserList.Remove(id);
                    }
                }

                duplicateInfo.userIds = remainUserList.ToArray();

                //如果抄送人不为空,则添加该操作记录
                if (remainUserList.Count > 0)
                {
                    operateList.Add(duplicateInfo);
                }
                operateList.Add(otherInfo);
            }

            if (operateList.Count == 0)
            {
                operateList = operateInfoList;
            }

            return operateList;
        }

        #endregion 剔除节点操作人中冗余的抄送人

        #region 设置节点操作人信息

        /// <summary>
        /// 设置节点操作人信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeOperateIfo">操作人信息</param>
        /// <param name="userInfo">表单申请人信息</param>
        /// <returns>操作人信息</returns>
        private NodeOperateModel SetNodeOperateInfo(TargetNavigationDBEntities db, NodeOperateModel nodeOperateIfo, UserInfoSimpleModel userInfo)
        {
            //取得节点操作人目标ID
            var targetId = (from p in db.tblOperateResult where p.operateId == nodeOperateIfo.operateId select p.targetId).ToArray();
            var userIdList = new List<int?>();
            //类型 1：操作者部门 2：操作者岗位 3：操作者 4：上级岗位 5：所有人
            switch (nodeOperateIfo.type.Value)
            {
                case (int)ConstVar.NodeOperatorType.Organization:
                    var orgUserList = new int[0];
                    if (nodeOperateIfo.countersign == (int)ConstVar.NodeStatus.Countersign || nodeOperateIfo.countersign == (int)ConstVar.NodeStatus.Submit)
                    {
                        orgUserList = GetApprovalUserIdByOrgId(db, targetId);
                    }
                    else
                    {
                        orgUserList = GetUserIdByOrgId(db, targetId);
                    }
                    foreach (var id in orgUserList)
                    {
                        userIdList.Add(id);
                    }
                    break;

                case (int)ConstVar.NodeOperatorType.Station:
                    var stationUserList = GetUserIdByStationId(db, targetId);
                    foreach (var id in stationUserList)
                    {
                        userIdList.Add(id);
                    }
                    break;

                case (int)ConstVar.NodeOperatorType.User:
                    userIdList = targetId.ToList();
                    break;

                case (int)ConstVar.NodeOperatorType.Superior:
                    var superiorList = GetSuperiorListByStationId(db, userInfo.stationId);
                    foreach (var id in superiorList)
                    {
                        userIdList.Add(id);
                    }
                    break;
            }

            //添加操作人信息
            nodeOperateIfo.userIds = userIdList.Distinct().ToArray();

            return nodeOperateIfo;
        }

        #endregion 设置节点操作人信息

        #region 判断节点操作人数

        /// <summary>
        /// 判断节点操作人数
        /// </summary>
        /// <param name="node">节点信息</param>
        /// <param name="operateInfoList">操作人信息</param>
        /// <param name="message">提示错误信息</param>
        /// <returns>true：正确 false：错误</returns>
        private bool CheckOperatorNum(NodeModel node, List<NodeOperateModel> operateInfoList, out string message)
        {
            var result = true;
            var serialNum = 0;
            message = string.Empty;

            //节点类型不为创建
            if (node.nodeType != (int)ConstVar.NodeType.Create)
            {
                foreach (var item in operateInfoList)
                {
                    var operatorNum = 0;
                    var approvalNum = 0;

                    //操作类型不为抄送的场合
                    if (item.countersign != (int)ConstVar.NodeStatus.Duplicate)
                    {
                        using (var db = new TargetNavigationDBEntities())
                        {
                            switch (item.type.Value)
                            {
                                case (int)ConstVar.NodeOperatorType.Organization:
                                    operatorNum = GetUserIdByOrgId(db, item.targetId).Count();
                                    approvalNum = GetApprovalUserIdByOrgId(db, item.targetId).Count();
                                    break;

                                case (int)ConstVar.NodeOperatorType.Station:
                                    operatorNum = GetUserIdByStationId(db, item.targetId).Count();
                                    approvalNum = operatorNum;
                                    break;

                                case (int)ConstVar.NodeOperatorType.User:
                                    operatorNum = item.targetId.Length;
                                    approvalNum = item.targetId.Length;
                                    break;

                                case (int)ConstVar.NodeOperatorType.Superior:
                                    operatorNum = 1;
                                    approvalNum = 1;
                                    break;
                            }
                        }
                        //节点类型为归档
                        if (node.nodeType == (int)ConstVar.NodeType.End)
                        {
                            if (operatorNum == 0)
                            {
                                result = false;
                                serialNum = item.orderNum.Value;
                                break;
                            }
                        }
                        else
                        {
                            if (approvalNum == 0)
                            {
                                result = false;
                                serialNum = item.orderNum.Value;
                                break;
                            }
                        }
                    }
                }
                if (!result)
                    message = string.Format(MESSAGE_NODE_SET_ERROR, node.nodeName, serialNum);
            }

            return result;
        }

        #endregion 判断节点操作人数

        #region 根据组织ID取得组织名称

        /// <summary>
        ///  根据组织ID取得组织名称
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="orgId">组织ID</param>
        /// <returns>组织名称</returns>
        private string[] GetOrganizaiotnNameById(TargetNavigationDBEntities db, int?[] orgId)
        {
            var orgName = (from p in db.tblOrganization
                           where orgId.Contains(p.organizationId)
                           select p.organizationName
                           ).ToArray();
            return orgName;
        }

        #endregion 根据组织ID取得组织名称

        #region 根据岗位ID取得岗位名称

        /// <summary>
        ///  根据岗位ID取得岗位名称
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="stationId">岗位ID</param>
        /// <returns>岗位名称</returns>
        private string[] GetStationNameById(TargetNavigationDBEntities db, int?[] stationId)
        {
            var stationName = (from s in db.tblStation
                               where stationId.Contains(s.stationId)
                               select s.stationName
                               ).ToArray();

            return stationName;
        }

        #endregion 根据岗位ID取得岗位名称

        #region 根据用户ID列表取得用户名

        /// <summary>
        ///  根据用户ID列表取得用户名
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="userId">用户ID</param>
        /// <returns>用户名</returns>
        private string[] GetUserNameByUserIdList(TargetNavigationDBEntities db, int?[] userId)
        {
            var userList = (from u in db.tblUser
                            where userId.Contains(u.userId)
                            select u.userName
                            ).ToArray();

            return userList;
        }

        #endregion 根据用户ID列表取得用户名

        #region 取得表单控件ID对应的控件值

        /// <summary>
        ///  取得表单控件ID对应的控件值
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="formId">表单ID</param>
        /// <returns>返回控件值</returns>
        private string GetControlValueById(TargetNavigationDBEntities db, string controlId, int formId)
        {
            var controlValue = (from detail in db.tblFormDetail
                                join content in db.tblFormContent
                                on detail.detailId equals content.detailId
                                where detail.formId == formId && detail.controlId == controlId
                                select content.controlValue
                                 ).FirstOrDefault();

            return controlValue;
        }

        #endregion 取得表单控件ID对应的控件值

        #region 比较流程条件的控件值

        /// <summary>
        /// 比较流程条件的控件值
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="conditionInfo">流程条件信息</param>
        /// <param name="formId">表单ID</param>
        /// <returns>比较结果</returns>
        private bool CompareControlValue(TargetNavigationDBEntities db, LinkConditionModel conditionInfo, int formId)
        {
            var flag = false;

            //取得表单控件ID对应的控件值
            var controlValue = GetControlValueById(db, conditionInfo.controlId, formId);
            //表单对应的模版ID
            var templateId = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).Select(p => p.templateId).FirstOrDefault();
            //取得控件信息
            var controlInfo = GetControlInfoById(db, conditionInfo.controlId, templateId);

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

        #region 根据组织ID取得有审批权限岗位下的所有用户

        /// <summary>
        ///  根据组织ID取得有审批权限岗位下的所有用户
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="orgId">组织ID</param>
        /// <returns>用户信息</returns>
        private int[] GetApprovalUserIdByOrgId(TargetNavigationDBEntities db, int?[] orgId)
        {
            var userList = (from u in db.tblUser
                            join us in db.tblUserStation
                            on u.userId equals us.userId
                            join s in db.tblStation
                            on us.stationId equals s.stationId
                            join o in db.tblOrganization
                            on s.organizationId equals o.organizationId
                            where orgId.Contains(s.organizationId) && s.approval.Value && !u.deleteFlag && u.workStatus == (int)ConstVar.workStatus.OnWork
                            select u.userId
                            ).ToArray();

            return userList;
        }

        #endregion 根据组织ID取得有审批权限岗位下的所有用户

        #region 根据组织ID取得该组织下的所有用户

        /// <summary>
        ///  根据组织ID取得该组织下的所有用户
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="orgId">组织ID</param>
        /// <returns>用户信息</returns>
        private int[] GetUserIdByOrgId(TargetNavigationDBEntities db, int?[] orgId)
        {
            var userList = (from u in db.tblUser
                            join us in db.tblUserStation
                            on u.userId equals us.userId
                            join s in db.tblStation
                            on us.stationId equals s.stationId
                            join o in db.tblOrganization
                            on s.organizationId equals o.organizationId
                            where orgId.Contains(s.organizationId) && !u.deleteFlag && u.workStatus == (int)ConstVar.workStatus.OnWork
                            select u.userId
                            ).ToArray();

            return userList;
        }

        #endregion 根据组织ID取得该组织下的所有用户

        #region 根据岗位ID取得该岗位下的所有用户

        /// <summary>
        ///  根据岗位ID取得该岗位下的所有在职用户
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="stationId">组织ID</param>
        /// <returns>用户信息</returns>
        private int[] GetUserIdByStationId(TargetNavigationDBEntities db, int?[] stationId)
        {
            var userList = (from u in db.tblUser
                            join us in db.tblUserStation
                            on u.userId equals us.userId
                            join s in db.tblStation
                            on us.stationId equals s.stationId
                            where stationId.Contains(s.stationId) && !u.deleteFlag && u.workStatus == (int)ConstVar.workStatus.OnWork
                            select u.userId
                            ).ToArray();

            return userList;
        }

        #endregion 根据岗位ID取得该岗位下的所有用户

        #region 根据岗位ID取得上级岗位用户

        /// <summary>
        ///  根据表单ID取得表单申请人的上级岗位用户
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="stationId">岗位ID</param>
        /// <returns>上级岗位用户</returns>
        private List<int> GetSuperiorListByStationId(TargetNavigationDBEntities db, int stationId)
        {
            var superiorId = new List<int>();
            var parentStationId = db.tblStation.Where(p => p.stationId == stationId && !p.deleteFlag).FirstOrDefault().parentStation;
            if (parentStationId.HasValue)
            {
                //取得上级岗位用户
                superiorId = (from u in db.tblUser
                              join us in db.tblUserStation
                              on u.userId equals us.userId
                              where us.stationId == parentStationId && !u.deleteFlag && u.workStatus == (int)ConstVar.workStatus.OnWork
                              select u.userId
                              ).ToList();
            }
            return superiorId;
        }

        #endregion 根据岗位ID取得上级岗位用户

        #region 取得流程条件的模版控件信息

        /// <summary>
        ///  取得流程条件的模版控件信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="templateId">模版ID</param>
        /// <returns>模版控件信息</returns>
        private List<ControlSimpleModel> GetTemplateControlInfo(TargetNavigationDBEntities db, int templateId)
        {
            //控件类型
            var type = new int[]
            {
                //数字输入框
                (int)ConstVar.ControlType.NumberInput,
                //金额小写
                (int)ConstVar.ControlType.MoneyLower,
                //日期
                (int)ConstVar.ControlType.Date,
                //日期时间
                (int)ConstVar.ControlType.DateTime
            };

            var controlInfoList = new List<ControlSimpleModel>();

            var controlInfo = (from control in db.tblTemplateControl
                               where control.templateId == templateId && type.Contains(control.controlType)
                               select new ControlSimpleModel
                               {
                                   controlId = control.controlId,
                                   controlTitle = control.title,
                                   controlType = control.controlType,
                                   parentControl = control.parentControl
                               }
                                   ).ToList();

            foreach (var item in controlInfo)
            {
                //父控件信息
                var parentControlInfo = db.tblTemplateControl.Where(p => p.controlId == item.parentControl && p.templateId == templateId).FirstOrDefault();
                if (parentControlInfo != null)
                {
                    //类型为明细列表
                    if (parentControlInfo.controlType != (int)ConstVar.ControlType.DetailList)
                    {
                        controlInfoList.Add(item);
                    }
                }
                else
                {
                    controlInfoList.Add(item);
                }
            }

            return controlInfoList;
        }

        #endregion 取得流程条件的模版控件信息

        #region 组合流程条件公式

        /// <summary>
        ///  组合流程条件公式
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="formulaInfoList">流程公式信息列表</param>
        /// <returns>用户信息</returns>
        private string CombineConditionFormula(TargetNavigationDBEntities db, List<LinkFormulaModel> formulaInfoList)
        {
            //流程条件公式
            var formula = new StringBuilder();
            if (formulaInfoList.Count > 0)
            {
                foreach (var item in formulaInfoList)
                {
                    if (item.operate != null)
                    {
                        switch (item.operate)
                        {
                            case "|":
                                formula.Append(FORMULA_OR);
                                break;

                            case "&":
                                formula.Append(FORMULA_AND);
                                break;

                            default:
                                formula.Append(item.operate);
                                break;
                        }
                    }
                    if (item.conditionId != null)
                    {
                        formula.Append(item.displayText);
                    }
                }
            }
            return formula.ToString();
        }

        #endregion 组合流程条件公式

        #endregion 私有方法
    }
}