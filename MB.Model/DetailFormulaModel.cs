namespace MB.Model
{
    public class DetailFormulaModel
    {
        /// <summary>公式ID</summary>
        public string formulaId { get; set; }

        /// <summary>模板ID</summary>
        public int? templateId { get; set; }

        /// <summary>排序</summary>
        public int orderNum { get; set; }

        /// <summary>明细列表ID</summary>
        public string detailControl { get; set; }

        /// <summary>控件ID</summary>
        public string controlId { get; set; }

        /// <summary>表示名</summary>
        public string displayText { get; set; }

        /// <summary>操作符</summary>
        public string operate { get; set; }
    }
}