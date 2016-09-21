namespace MB.Model
{
    public class UploadImageModel
    {
        public string headFileName { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public float ratioW { get; set; }
        public float ratioH { get; set; }
        public int rx { get; set; }
        public int ry { get; set; }
    }
}