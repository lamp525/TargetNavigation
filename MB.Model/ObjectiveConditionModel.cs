namespace MB.Model
{
    //目标列表筛选条件对象
    public class ObjectiveConditionModel
    {
        /// <summary>快捷查询，忽略其他筛选条件：1：待提交 2：待审核 3：审核通过 4：待确认 5：已完成 6、超时</summary>
        public int? soonSelect { get; set; }

        /// <summary>目标状态 1：待提交 2：待审核 3：审核通过（进行中、已超时） 4：待确认 5：已完成</summary>
        public int[] status { get; set; }

        /// <summary>目标对象组织架构</summary>
        public int[] department { get; set; }

        /// <summary>目标对象人员</summary>
        public int[] person { get; set; }

        /// <summary>目标预计开始时间：1、近一周  2、近一月  3、自定义</summary>
        public string[] startTime { get; set; }

        /// <summary>1、我的目标  2、下属目标</summary>
        public int objectiveType { get; set; }
    }
}