using System.Collections.Generic;

namespace MB.Model
{
    public class TemplateControlModel
    {
        /// <summary>模板ID</summary>
        public int? templateId { get; set; }

        /// <summary>控件ID</summary>
        public string controlId { get; set; }

        /// <summary>父控件ID</summary>
        public string parentControl { get; set; }

        /// <summary>控件类型</summary>
        public int controlType { get; set; }

        /// <summary>必须 0：False 1：True</summary>
        public int? require { get; set; }

        /// <summary>标题</summary>
        public string title { get; set; }

        /// <summary>控件大小 1：小 2：标准 3：大</summary>
        public int? size { get; set; }

        /// <summary>多选 1：多选 0：单选</summary>
        public int? mutliSelect { get; set; }

        /// <summary>排列方式 1：纵向排列 0：横向排列</summary>
        public int? vertical { get; set; }

        /// <summary>分隔线类型 1：实线 2：虚线 3：双线</summary>
        public int? lineType { get; set; }

        /// <summary>默认行</summary>
        public int? defaultRowNum { get; set; }

        /// <summary>列值统计</summary>
        public int? columnStatistics { get; set; }

        /// <summary>颜色</summary>
        public string color { get; set; }

        /// <summary>描述</summary>
        public string description { get; set; }

        /// <summary>大小写金额关联ID</summary>
        public string linked { get; set; }

        /// <summary>第几列</summary>
        public int? columnIndex { get; set; }

        /// <summary>第几行</summary>
        public int? rowIndex { get; set; }

        /// <summary>加载flag</summary>
        public int loaded { get; set; }

        /// <summary>是否被使用</summary>
        public int used { get; set; }

        /// <summary>控件权限</summary>
        public int status { get; set; }

        /// <summary>控件内容</summary>
        public List<ControlNewModel> controlValue { get; set; }
    }

    public class TemplateControlInfoModel
    {
        /// <summary>模板控件信息</summary>
        public TemplateControlModel control { get; set; }

        /// <summary>明细公式列表</summary>
        public List<DetailFormulaModel> formula { get; set; }

        /// <summary>控件项目列表</summary>
        public List<ControlItemModel> item { get; set; }
    }
}