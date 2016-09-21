using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MB.New.BLL
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



        #region 取得节点设置信息

        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public List<NodeInfoModel> GetNodeInfo(TargetNavigationDBEntities db, int templateId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<NodeInfoModel>();

            var nodeList = GetFlowNodeInfo(db, templateId);
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (var item in nodeList)
                {
                    var nodeInfo = new NodeInfoModel
                    {
                        node = item,
                        //nodeOperate = GetTemplateNodeOperateList(item.nodeId.Value)
                    };

                    result.Add(nodeInfo);
                }
            }

            return result;
        }

        #endregion 取得节点设置信息

        #region 取得流程节点信息

        /// <summary>
        /// 取得流程节点信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="templateId">模版ID</param>
        /// <returns>流程节点列表</returns>
        public List<NodeModel> GetFlowNodeInfo(TargetNavigationDBEntities db, int templateId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.tblFlowNode.Where(x => x.templateId == templateId).OrderBy(x => x.nodeType)
                .Select(x => new NodeModel
                {
                    templateId = x.templateId,
                    nodeId = x.nodeId,
                    nodeName = x.nodeName,
                    nodeType = x.nodeType
                }).ToList();

            return result;
        }

        #endregion 取得流程节点信息



        #region 根据控件类型取得控件类型名称

        /// <summary>
        /// 根据控件类型取得控件类型名称
        /// </summary>
        /// <param name="type">控件类型</param>
        /// <returns>流程节点列表</returns>
        public string GetControlTypeNameByType(EnumDefine.ControlType type)
        {
            var typeName = string.Empty;

            switch (type)
            {
                case EnumDefine.ControlType.Tag:
                    typeName = TAG;
                    break;

                case EnumDefine.ControlType.NumberInput:
                    typeName = NUMBER_INPUT;
                    break;

                case EnumDefine.ControlType.MoneyLower:
                    typeName = MONEY_LOWER;
                    break;

                case EnumDefine.ControlType.RadioButton:
                    typeName = RADION_BUTTON;
                    break;

                case EnumDefine.ControlType.CheckBox:
                    typeName = CHECK_BOX;
                    break;

                case EnumDefine.ControlType.ComboBox:
                    typeName = COMBO_BOX;
                    break;

                case EnumDefine.ControlType.BrowseHR:
                    typeName = BROWSE_HR;
                    break;

                case EnumDefine.ControlType.BrowseOrg:
                    typeName = BROWSE_ORG;
                    break;

                case EnumDefine.ControlType.BrowseDoc:
                    typeName = BROWSE_DOC;
                    break;

                case EnumDefine.ControlType.Line:
                    typeName = LINE;
                    break;

                case EnumDefine.ControlType.MultiTextBox:
                    typeName = MULTI_TEXT_BOX;
                    break;

                case EnumDefine.ControlType.Date:
                    typeName = DATE;
                    break;

                case EnumDefine.ControlType.DateRange:
                    typeName = DATE_RANGE;
                    break;

                case EnumDefine.ControlType.DateTime:
                    typeName = DATE_TIME;
                    break;

                case EnumDefine.ControlType.DateTimeRange:
                    typeName = DATE_TIME_RANGE;
                    break;

                case EnumDefine.ControlType.DetailList:
                    typeName = DETAIL_LIST;
                    break;

                case EnumDefine.ControlType.MoneyUpper:
                    typeName = MONEY_UPPER;
                    break;

                case EnumDefine.ControlType.TwoColumnRow:
                    typeName = TWO_COLUMN_ROW;
                    break;

                case EnumDefine.ControlType.ThreeColumnRow:
                    typeName = THREE_COLUMN_ROW;
                    break;
            }

            return typeName;
        }

        #endregion 根据控件类型取得控件类型名称



  
    }
}