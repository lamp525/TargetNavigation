using System.Collections.Generic;

namespace MB.Model
{
    public class NodeLinkModel
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

    public class NodeLinkInfoModel
    {
        //节点流程
        public NodeLinkModel nodeLinkMode { get; set; }

        //流程条件
        public List<LinkConditionModel> linkConditionList { get; set; }

        //条件公式
        public List<LinkFormulaModel> linkFormulaList { get; set; }
    }

    public class FlowEditModel
    {
        //流程信息
        public List<NodeLinkInfoModel> nodeLinkInfoList { get; set; }

        //删除的流程信息
        public int[] deleteLinkId { get; set; }

        //删除的条件信息
        public int[] deleteCondition { get; set; }
    }
}