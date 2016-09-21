using System;

namespace MB.New.Model
{
    public class ProjectInfoModel
    {
        public int projectId { get; set; }
        public int? parentProject { get; set; }
        public string projectName { get; set; }
        public bool? withSub { get; set; }
        public int orderNumber { get; set; }
        public int status { get; set; }
    }
}