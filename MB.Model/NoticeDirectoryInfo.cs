using System;

namespace MB.Model
{
    public class NoticeDirectoryInfo
    {
        public int directoryId { get; set; }
        public string directoryName { get; set; }
        public int? parentDirectory { get; set; }
        public int creatUser { get; set; }
        public DateTime creatTime { get; set; }
        public int updateUser { get; set; }
        public DateTime updateTime { get; set; }
    }
}