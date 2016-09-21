using System.Collections.Generic;

namespace MB.Model
{
    public class PlanCheckModel
    {
        /// <summary>普通计划Id</summary>
        public int?[] planId { get; set; }

        /// <summary>循环计划信息 </summary>
        public List<LoopPlanSimpleModel> loopInfo { get; set; }

        /// <summary>重要度</summary>
        public int? importance { get; set; }

        /// <summary>紧急度</summary>
        public int? urgency { get; set; }

        /// <summary>难易度</summary>
        public int? difficulty { get; set; }

        /// <summary>完成数量</summary>
        public decimal? completeQuantity { get; set; }

        /// <summary>完成质量</summary>
        public decimal? completeQuality { get; set; }

        /// <summary>完成时间</summary>
        public decimal? completeTime { get; set; }

        /// <summary>审核意见</summary>
        public string message { get; set; }

        /// <summary>true：通过 false：不通过</summary>
        public bool type { get; set; }
    }

    public class LoopPlanSimpleModel
    {
        /// <summary>循环计划提交Id</summary>
        public int? submitId { get; set; }

        /// <summary>循环计划Id</summary>
        public int loopId { get; set; }
    }
}