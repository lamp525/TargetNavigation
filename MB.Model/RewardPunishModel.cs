using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class RewardPunishModel
    {
        /// <summary>
        /// 奖惩Id
        /// </summary>
        public int rewardId { get; set; }

        /// <summary>
        /// 奖惩人
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 奖惩类型 1:计划完成  2.计划审核 3.计划确认  4.流程审批  5:目标完成  6.目标审核 7.目标确认
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 计划Id或流程Id、目标Id
        /// </summary>
        public int targetId { get; set; }

        //超时天数
        public int timeOutDay { get; set; }

        /// <summary>
        /// 方式  1.金额  2.比例
        /// </summary>
        public int? deductionMode { get; set; }

        //数量
        public decimal? deductionNum { get; set; }

        /// <summary>
        /// 延时日期
        /// </summary>
        public DateTime? delayTime { get; set; }

        /// <summary>
        /// 奖惩对象
        /// </summary>
        public string targetName { get; set; }

        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime statisticeTime { get; set; }
    }

    /// <summary>
    /// 每月奖惩数
    /// </summary>
    public class RewardPunishNum
    {
        //年份
        public int year { get; set; }

        //月份
        public int month { get; set; }

        //数量
        public decimal number { get; set; }
    }

    /// <summary>
    /// 每月奖惩数
    /// </summary>
    public class RewardPunishDetail
    {
        /// <summary>
        /// 平均有效工时
        /// </summary>
        public decimal avgTime { get; set; }

        /// <summary>
        /// 有效工时奖惩金额
        /// </summary>
        public decimal timeReward { get; set; }

        public List<RewardPunishModel> detail { get; set; }
    }
}