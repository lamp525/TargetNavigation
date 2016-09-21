using System.Collections.Generic;
using MB.Model;
using MB.Model.UserIndexModels;

namespace MB.BLL
{
    public interface IIncentiveIndexBLL
    {
        /// 获取每月奖惩数
        List<RewardPunishNum> GetRewardPunishNum(int? year, int userId);

        /// 获取激励详细情况
        RewardPunishDetail GetRewardPunishDetail(int year, int? month, int userId);

        /// 获取图表数据
        List<IncentiveDataModel> GetIncentiveData(int type, int year, int? month, int userId);

        /// 下属奖励列表
        List<UnderReward> GetUnderReward(int? year, int? month, int? userId);

        UserIndexIncentiveInfoModel RewardMoneyByMonthAndWeek(int userId, int? year, int? month);
    }
}