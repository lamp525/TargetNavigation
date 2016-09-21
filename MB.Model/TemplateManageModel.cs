namespace MB.Model
{
    public class TemplateManageModel
    {
        public int templateId { get; set; }
        public string templateName { get; set; }
        public string description { get; set; }
        public int? status { get; set; }
        public bool? system { get; set; }
        public int? categoryId { get; set; }
        public int creatUser { get; set; }
        public int updateUser { get; set; }
        public int noedId { get; set; }

        public bool IsUse { get; set; }
        public bool? testFlag { get; set; }
    }
}