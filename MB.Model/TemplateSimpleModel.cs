using System.Collections.Generic;

namespace MB.Model
{
    public class TemplateSimpleModel
    {
        /// <summary>分类Id</summary>
        public int categoryId { get; set; }

        /// <summary>分类名</summary>
        public string categoryName { get; set; }

        /// <summary>分类下的模板集合</summary>
        public List<TemplateModel> templateList { get; set; }
    }
}