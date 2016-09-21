using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class AddNewObjectiveModel
    {
        /// <summary>目标ID</summary>
        public int? objectiveId { get; set; }

        /// <summary>父目标ID</summary>
        public int? parentObjective { get; set; }

        /// <summary>显示变更Flag false：不显示  true：显示 </summary>
        public bool? displayChangeFlag { get; set; }

        /// <summary>目标名称 </summary>
        public string objectiveName { get; set; }

        /// <summary>判定类型</summary>
        public int? valueType { get; set; }

        /// <summary>目标对象 1：组织架构 2：人员</summary>
        public int? objectiveType { get; set; }

        /// <summary>奖励基数</summary>
        public decimal? bonus { get; set; }

        /// <summary>权重</summary>
        public decimal? weight { get; set; }

        /// <summary>实际值</summary>
        public string actualValue { get; set; }

        /// <summary>考核类型 1：金额 2：时间 3：数字</summary>
        public int? checkType { get; set; }

        /// <summary>指标值</summary>
        public string objectiveValue { get; set; }

        /// <summary>理想值 </summary>
        public string expectedValue { get; set; }

        /// <summary>备注项</summary>
        public string description { get; set; }

        /// <summary>最大奖励数</summary>
        public decimal? maxValue { get; set; }

        /// <summary>最大扣除数</summary>
        public decimal? minValue { get; set; }

        /// <summary>开始时间 </summary>
        public DateTime? startTime { get; set; }

        /// <summary>结束时间</summary>
        public DateTime? endTime { get; set; }

        /// <summary>警戒时间</summary>
        public DateTime? alarmTime { get; set; }

        /// <summary>责任部门</summary>
        public string responsibleOrg { get; set; }

        /// <summary>责任人</summary>
        public int? responsibleUser { get; set; }

        /// <summary>确认人</summary>
        public int? confirmUser { get; set; }

        /// <summary>公式 0：无公式 1：默认公式 2：自定义</summary>
        public int? formula { get; set; }

        /// <summary>状态</summary>
        public int? status { get; set; }

        /// <summary>意见</summary>
        public string message { get; set; }

        /// <summary>目标公式信息</summary>
        public List<ObjectiveFormula> objectiveFormula { get; set; }

        /// <summary>修改目标公式设置</summary>
        public ObjectiveEditFormula objectiveFormulaInfo { get; set; }

        ///<summary>目标标签</summary>
        public string[] keyword { get; set; }
    }
}