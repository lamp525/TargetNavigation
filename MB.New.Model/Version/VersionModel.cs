using System;

namespace MB.New.Model.Version
{
    public class VersionModel
    {
        public int versionId { get; set; }
        public string number { get; set; }
        public string content { get; set; }
        public DateTime? updateTime { get; set; }
    }
}