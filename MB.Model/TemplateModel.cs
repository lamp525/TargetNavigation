using System.Collections.Generic;

namespace MB.Model
{
    public class TemplateModel
    {
        /// <summary>模板ID</summary>
        public int? templateId { get; set; }

        /// <summary>模板名</summary>
        public string templateName { get; set; }

        /// <summary>模板描述</summary>
        public string description { get; set; }

        /// <summary>默认标题 0：自定义标题 1：系统默认标题</summary>
        public int defaultTitle { get; set; }

        /// <summary>分类ID</summary>
        public int? categoryId { get; set; }

        /// <summary>模板状态 0：保存 1：使用</summary>
        public int? status { get; set; }

        /// <summary>模板内容</summary>
        public string contents { get; set; }

        /// <summary>系统表单 0：自定义表单 1：系统表单</summary>
        public bool? system { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public bool isCategory { get; set; }
    }

    public class TemplateInfoModel
    {
        /// <summary>模板信息</summary>
        public TemplateModel template { get; set; }

        /// <summary>控件信息</summary>
        public List<TemplateControlInfoModel> controlInfo { get; set; }

        /// <summary>被删除控件</summary>
        public string[] deleteControl { get; set; }

        /// <summary>被删除控件项目</summary>
        public int[] deleteControlItem { get; set; }
    }
}