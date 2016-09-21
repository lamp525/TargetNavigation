using System.Collections.Generic;

namespace MB.New.Model
{
    public class RewardPunishDetailModel
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