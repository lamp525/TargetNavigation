using System.Collections.Generic;
using MB.New.Common;

namespace MB.Facade.Flow
{
    /// <summary>
    /// 节点
    /// </summary>
    public class PageNodeModel
    {
        //流程节点ID
        public int? nodeId { get; set; }

        //模版ID
        public int templateId { get; set; }

        //节点名称
        public string nodeName { get; set; }

        //节点类型 1：创建   2：提交 3:审批  4：归档
        public int? nodeType { get; set; }

        public bool? operateEditFlag { get; set; }
    }

    /// <summary>
    /// 节点信息
    /// </summary>
    public class PageNodeInfoModel
    {
        //流程节点
        public PageNodeModel node { get; set; }

        //节点字段
        public List<PageNodeFieldModel> nodeField { get; set; }

        //节点操作人
        public List<PageNodeOperateModel> nodeOperate { get; set; }
    }

    /// <summary>
    /// 节点编辑
    /// </summary>
    public class PageNodeEditModel
    {
        /// <summary>
        ///模版ID
        /// </summary>
        public int? templateId { get; set; }

        //节点信息
        public List<PageNodeInfoModel> nodeInfo { get; set; }

        //被删除的节点ID
        public int[] deleteNode { get; set; }

        //被删除的节点操作ID
        public int[] deleteOperateId { get; set; }
    }

    /// <summary>
    /// 节点字段编辑
    /// </summary>
    public class PageNodeFieldEditModel
    {
        //模版ID
        public int templateId { get; set; }

        //流程节点ID
        public int? nodeId { get; set; }

        //模版控件ID
        public string controlId { get; set; }

        //父模版控件ID
        public string parentControl { get; set; }

        //明细控件  true为明细控件， false为主表控件
        public bool isDetail { get; set; }

        //控件标题
        public string controlTitle { get; set; }

        //控件描述
        public string controlDescription { get; set; }

        //控件类型
        public int controlType { get; set; }

        //字段状态 0：隐藏 1：只读 2：编辑
        public EnumDefine.NodeControlStatus status { get; set; }
    }

    /// <summary>
    /// 节点字段
    /// </summary>
    public class PageNodeFieldModel
    {
        //流程节点ID
        public int? nodeId { get; set; }

        //模版控件ID
        public string controlId { get; set; }

        //字段状态 0：隐藏 1：只读 2：编辑
        public int status { get; set; }
    }

    public class PageFlowEditModel
    {
        //流程信息
        public List<PageNodeLinkInfoModel> nodeLinkInfoList { get; set; }

        //删除的流程信息
        public int[] deleteLinkId { get; set; }

        //删除的条件信息
        public int[] deleteCondition { get; set; }
    }

    public class PageNodeLinkInfoModel
    {
        //节点流程
        public PageNodeLinkModel nodeLinkMode { get; set; }

        //流程条件
        public List<PageLinkConditionModel> linkConditionList { get; set; }

        //条件公式
        public List<PageLinkFormulaModel> linkFormulaList { get; set; }
    }

    /// <summary>
    /// 节点操作信息
    /// </summary>
    public class PageNodeOperateModel
    {
        //节点操作人ID
        public int? operateId { get; set; }

        //节点ID
        public int? nodeId { get; set; }

        //节点类型
        public int? nodeType { get; set; }

        // 类型 1：操作者部门 2：操作者岗位 3：操作者 4：上级岗位 5：所有人
        public int? type { get; set; }

        //条件 1:属于 2：不属于
        public int? condition { get; set; }

        //会签 0：审批 1：会签 2：抄送 3：提交
        public int? countersign { get; set; }

        //批次条件类型  1：申请人组织架构 2：申请人岗位 3：申请人
        public int? batchType { get; set; }

        //批次条件 1:属于 2：不属于
        public int? batchCondition { get; set; }

        //操作人目标ID
        public int?[] targetId { get; set; }

        //批次目标ID
        public int?[] batchTargetId { get; set; }

        //操作人Id集合
        public int?[] userIds { get; set; }

        //操作人结果
        public string[] targetName { get; set; }

        //批次条件结果
        public string[] batchTargetName { get; set; }

        //排序
        public int? orderNum { get; set; }

        //测试标志
        public bool? testFlag { get; set; }
    }

    public class PageLinkConditionModel
    {
        //序列
        public int? serialNumber { get; set; }

        //条件结果ID
        public int? conditionId { get; set; }

        //节点出口ID
        public int? linkId { get; set; }

        //条件类型 1：组织架构 2：岗位 3：人力资源 4：控件
        public int? type { get; set; }

        //控件ID
        public string controlId { get; set; }

        //控件类型
        public int? controlType { get; set; }

        //控件标题
        public string controlTitle { get; set; }

        //条件 1：属于 2：不属于 3：等于 4：大于 5：小于
        public int condition { get; set; }

        //结果（比较值）
        public string value { get; set; }

        //目标ID
        public int?[] targetId { get; set; }

        //目标名称
        public string[] targetName { get; set; }

        //测试标志
        public bool? testFlag { get; set; }
    }

    public class PageNodeLinkModel
    {
        //节点出口ID
        public int? linkId { get; set; }

        //模版ID
        public int? templateId { get; set; }

        //入口节点ID
        public int? nodeEntryId { get; set; }

        //入口节点名称
        public string nodeEntryName { get; set; }

        //入口节点类型
        public int? nodeEntryType { get; set; }

        //出口节点ID
        public int? nodeExitId { get; set; }

        //出口节点名称
        public string nodeExitName { get; set; }

        //出口节点类型
        public int? nodeExitType { get; set; }

        //状态
        public int? status { get; set; }

        //测试标志
        public bool? testFlag { get; set; }
    }

    /// <summary>
    /// 流程条件控件信息
    /// </summary>
    public class ControlSimpleModel
    {
        //控件ID
        public string controlId { get; set; }

        //父控件ID
        public string parentControl { get; set; }

        //控件类型
        public int? controlType { get; set; }

        //控件标题
        public string controlTitle { get; set; }
    }

    public class LinkConditionInfoModel
    {
        /// <summary>流程条件控件信息l列表</summary>
        public List<ControlSimpleModel> controlInfoList { get; set; }

        /// <summary>流程条件列表</summary>
        public List<PageLinkConditionModel> linkConditionList { get; set; }

        /// <summary>流程公式信息</summary>
        public string linkFormulaInfo { get; set; }
    }

    public class PageLinkFormulaModel
    {
        //序列
        public int? serialNumber { get; set; }

        //条件公式ID
        public int? formulaId { get; set; }

        //节点出口ID
        public int? linkId { get; set; }

        //流程条件ID
        public int? conditionId { get; set; }

        //表示名
        public string displayText { get; set; }

        //操作符 |,&,(,)
        public string operate { get; set; }

        //排序
        public int? orderNum { get; set; }
    }
}