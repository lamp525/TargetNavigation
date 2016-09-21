namespace MB.Model
{
    public class ImageInfoModel
    {
        /// <summary>图片ID</summary>
        public int imageId { get; set; }

        /// <summary>存储名</summary>
        public string saveName { get; set; }

        /// <summary>后缀名</summary>
        public string extension { get; set; }

        /// <summary>宽度</summary>
        public int width { get; set; }

        /// <summary>高度</summary>
        public int height { get; set; }

        /// <summary>图片地址</summary>
        public string imageUrl { get; set; }
    }
}