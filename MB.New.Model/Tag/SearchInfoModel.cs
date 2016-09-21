namespace MB.New.Model
{
    public class SearchInfoModel
    {
        /// <summary>检索类型</summary>
        public int? type { get; set; }

        /// <summary>检索关键字</summary>
        public string[] keyword { get; set; }
    }
}