using System.Collections.Generic;

namespace MB.Model
{
    public class ObjectiveCheckModel
    {
        /// <summary>目标Id</summary>
        public int objectiveId { get; set; }

        /// <summary>操作意见</summary>
        public string message { get; set; }

        /// <summary>操作类型 6：审核通过 7：审核不通过</summary>
        public int result { get; set; }

        /// <summary>公式信息</summary>
        public ObjectiveEditFormula objectiveFormulaInfo { get; set; }
    }

    //目标规则修改
    public class ObjectiveEditFormula
    {
        /// <summary>公式 0：无公式 1：默认公式 2：自定义</summary>
        public int? formula { get; set; }

        /// <summary>最大奖励数</summary>
        public decimal? maxValue { get; set; }

        /// <summary>最大扣除数</summary>
        public decimal? minValue { get; set; }

        /// <summary>目标公式信息</summary>
        public List<ObjectiveFormula> objectiveFormula { get; set; }
    }
}