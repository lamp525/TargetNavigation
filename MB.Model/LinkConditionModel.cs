using System.Collections.Generic;

namespace MB.Model
{
    public class LinkConditionModel
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
        public List<LinkConditionModel> linkConditionList { get; set; }

        /// <summary>流程公式信息</summary>
        public string linkFormulaInfo { get; set; }
    }
}