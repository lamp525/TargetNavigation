namespace MB.New.Model
{
    public class FileInfoModel
    {
        /// <summary>
        /// 上传用户ID
        /// </summary>
        public int? uploadUserId { get; set; }

        /// <summary>
        /// 文件所属对象ID
        /// </summary>
        public int? targetId { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        public int? fileId { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// 存储名
        /// </summary>
        public string saveName { get; set; }

        /// <summary>
        /// 后缀名
        /// </summary>
        public string extension { get; set; }

        /// <summary>
        /// 预览标志
        /// </summary>
        public bool? isPreviewable { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long? fileSize { get; set; }
    }
}