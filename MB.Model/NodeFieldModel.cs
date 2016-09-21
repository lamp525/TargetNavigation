namespace MB.Model
{
    public class NodeFieldEditModel
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
        public int status { get; set; }
    }

    public class NodeFieldModel
    {
        //流程节点ID
        public int? nodeId { get; set; }

        //模版控件ID
        public string controlId { get; set; }

        //字段状态 0：隐藏 1：只读 2：编辑
        public int status { get; set; }
    }
}