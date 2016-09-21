namespace MB.Model
{
    public class AjaxUploadFileResult
    {
        public int releId { get; set; }//关联ID
        public int createUser { get; set; }//创建人ID
        public string displayName { get; set; }//表示名
        public string saveName { get; set; }//存储名
        public string extension { get; set; }//文件后缀

        /// <summary>是否可预览</summary>
        public bool isPreviewable { get; set; }
    }
}