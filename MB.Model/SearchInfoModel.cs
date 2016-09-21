namespace MB.Model
{
    public class SearchInfoModel
    {
        public int? type { get; set; }
        public string[] keyword { get; set; }
    }

    public class SearchDocumentOperateModel
    {
        public int id { get; set; }
        public bool isCompany { get; set; }
    }
}