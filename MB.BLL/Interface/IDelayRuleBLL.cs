using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IDelayRuleBLL
    {
        /// 根据状态获取列表（计划延时，有效工时）
        List<DelayRuleModel> GetDelayList(int type);

        /// 获取自定义激励规则（自定义）
        List<ValueIncentiveCustomModel> GetValueCustList();

        /// 获取激励规则对象
        ValueIncentiveModel GetValueIncentive();

        /// 更新或添加激励规则
        void AddOrUpdateValueIncentive(ValueIncentiveModel model);

        /// 新增自定义规则
        void AddValueCust(List<ValueIncentiveCustomModel> List);

        /// 删除自定义
        void DeleteValueCust(int[] id);

        /// 添加激励规则(计划延期，审批超时)
        bool AddDelayRule(List<DelayRuleModel> delayRuleList);

        /// 删除激励规则
        bool DeleteDelayRule(int[] id);
    }
}