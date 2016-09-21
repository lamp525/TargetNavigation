namespace MB.Model
{
    public class IndexTargetModel
    {
        /// <summary>模块ID</summary>
        public int? moduleId { get; set; }

        /// <summary>目标ID</summary>
        public int targetId { get; set; }

        /// <summary>目标名</summary>
        public string targetName { get; set; }

        /// <summary>统计子组织</summary>
        public bool withSub { get; set; }
    }
}