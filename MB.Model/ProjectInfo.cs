using System;

namespace MB.Model
{
    public class ProjectInfo
    {
        public int projectId { get; set; }
        public Nullable<int> parentProject { get; set; }
        public string projectName { get; set; }
        public Nullable<bool> withSub { get; set; }
        public int orderNumber { get; set; }
        public int status { get; set; }
    }
}