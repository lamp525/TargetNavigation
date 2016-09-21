using System;

namespace MB.Model
{
    public class TemplateCategoryModel
    {
        public int? categoryId { get; set; }
        public string categoryName { get; set; }
        public bool? system { get; set; }
        public int? orderNum { get; set; }
        public int creatuser { get; set; }
        public DateTime creattime { get; set; }
        public string comment { get; set; }
        public int upateuser { get; set; }
        public DateTime updateTime { get; set; }

        //tree用
        public int id { get; set; }

        public string name { get; set; }
        public bool isParent { get; set; }
        public bool isCategory { get; set; }
    }
}