using System;

namespace MB.Model
{
    public class Project
    {
        public int projectId { get; set; }
        public int parentProject { get; set; }
        public string projectName { get; set; }
        public Nullable<bool> withSub { get; set; }
        public int orderNumber { get; set; }
        public int status { get; set; }
    }
}