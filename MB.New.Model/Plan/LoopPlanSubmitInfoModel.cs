using MB.New.Common;
using System;

namespace MB.New.Model
{
    public class LoopPlanSubmitInfoModel
    {
        /// <summary>提交ID</summary>
        public int? submitId { get; set; }

        /// <summary>循环计划ID</summary>
        public int? loopId { get; set; }

        /// <summary>单位时间</summary>
        public int? time { get; set; }

        /// <summary>完成数量</summary>
        public int? quantity { get; set; }

        /// <summary>状态</summary>
        public EnumDefine.LoopPlanSubmitStatus completeStatus { get; set; }

        /// <summary>中止状态 0：不需要做 1：需要做</summary>
        public bool? undo { get; set; }

        /// <summary>有无附件</summary>
        public bool? isAttach { get; set; }

        /// <summary>内容输出</summary>
        public string result { get; set; }

        public decimal? completeQuantity { get; set; }

        public decimal? completeQuality { get; set; }

        public decimal? completeTime { get; set; }

        public DateTime? executeTime { get; set; }

        public DateTime? submitTime { get; set; }

        public DateTime? confirmTime { get; set; }

        public int? createUser { get; set; }

        public DateTime? createTime { get; set; }

        public int? updateUser { get; set; }

        public DateTime? updateTime { get; set; }
    }
}