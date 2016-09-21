using System;
using MB.New.Common;

namespace MB.New.Model
{
    /// <summary>
    /// 计划日志用模型
    /// </summary>
    public class PlanLogModel
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public int logId { get; set; }

        /// <summary>
        /// 计划或循环计划Id
        /// </summary>
        public int planId { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public EnumDefine.PlanOperateStatus type { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public int operateUser { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime operateTime { get; set; }
    }
}