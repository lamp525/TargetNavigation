using System;

namespace MB.Model
{
    public class ExecutionMode
    {
        public int executionId { get; set; }
        public string executionMode { get; set; }
        public int createUser { get; set; }
        public int updateUser { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public bool deleteFlag { get; set; }
    }
}