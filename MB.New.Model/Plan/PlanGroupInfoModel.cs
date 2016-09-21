namespace MB.New.Model
{
    /// <summary>
    /// 计划分组信息
    /// </summary>
    public class PlanGroupInfoModel
    {
        /// <summary>
        /// 分组ID（部门ID、人员ID、时间标志ID）
        /// </summary>
        public string groupId { get; set; }

        /// <summary>
        /// 分组名称（部门名称、人员名、时间）
        /// </summary>
        public string groupName { get; set; }
    }
}