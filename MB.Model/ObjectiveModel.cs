using System;

namespace MB.Model
{
    public class ObjectiveModel
    {
        /// <summary>目标Id</summary>
        public int objectiveId { get; set; }

        /// <summary>指标值</summary>
        public string objectiveValue { get; set; }

        /// <summary>理想值</summary>
        public string expectedValue { get; set; }

        /// <summary>实际值</summary>
        public string actualValue { get; set; }

        /// <summary>公式 0：无公式 1：默认公式 2：自定义</summary>
        public int? formula { get; set; }

        /// <summary>奖励基数</summary>
        public decimal? bonus { get; set; }

        /// <summary>最大奖励数</summary>
        public int? maxValue { get; set; }

        /// <summary>最大扣除数</summary>
        public int? minValue { get; set; }

        /// <summary>考核类型 1:金额 2:时间 3:数字</summary>
        public int? checkType { get; set; }

        /// <summary>开始时间</summary>
        public DateTime? actualStartTime { get; set; }

        /// <summary>实际结束时间</summary>
        public DateTime? actualEndTime { get; set; }

        /// <summary>警戒时间</summary>
        public DateTime? alarmTime { get; set; }

        /// <summary>权重</summary>
        public decimal? weight { get; set; }

        /// <summary>
        /// 预计结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
    }
}