namespace MB.Model
{
    public class PlanAttachment
    {
        public int attachmentId { get; set; }
        public string attachmentName { get; set; }
        public string saveName { get; set; }
        public string extension { get; set; }
        public bool isPreviewable { get; set; }
    }
}