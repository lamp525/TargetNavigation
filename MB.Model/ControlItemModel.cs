namespace MB.Model
{
    public class ControlItemModel
    {
        /// <summary>项目ID</summary>
        public int? itemId { get; set; }

        /// <summary>控件ID</summary>
        public string controlId { get; set; }

        /// <summary>模板ID</summary>
        public int? templateId { get; set; }

        /// <summary>项目名</summary>
        public string itemText { get; set; }

        /// <summary>排序</summary>
        public int? orderNum { get; set; }

        /// <summary>是否选中</summary>
        public int? checkOn { get; set; }
    }
}