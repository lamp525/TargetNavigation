using System.Collections.Generic;

namespace MB.Model
{
    public class TemplateFlowChartModel
    {
        //模版节点及操作信息
        public List<NodePlusOperateModel> nodeInfo { get; set; }

        //节点出口信息
        public List<LinkPlusConditionModel> linkInfo { get; set; }

        //节点出口测试率
        public decimal linkTestRatio { get; set; }
    }

    public class NodePlusOperateModel
    {
        //流程节点ID
        public int? nodeId { get; set; }

        //模版ID
        public int templateId { get; set; }

        //节点名称
        public string nodeName { get; set; }

        //节点类型 1：创建   2：提交 3:审批  4：归档
        public int? nodeType { get; set; }

        //操作人信息
        public string operate { get; set; }

        //测试通过率
        public decimal testRatio { get; set; }
    }

    public class LinkPlusConditionModel
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

        //流程条件信息
        public string condition { get; set; }

        //测试标志
        public bool? testFlag { get; set; }
    }
}