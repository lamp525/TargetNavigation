using MB.New.Common;

namespace MB.New.Model
{
    public class NodeFieldModel
    {
        //流程节点ID
        public int? nodeId { get; set; }

        //模版控件ID
        public string controlId { get; set; }

        //字段状态 0：隐藏 1：只读 2：编辑
        public EnumDefine.NodeControlStatus status { get; set; }
    }
}