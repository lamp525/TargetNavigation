using System;
using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IIncentivecontrast
    {
        /// 年执行力
        List<ContrastModel> GetYearPlanCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime);

        /// 月执行力
        List<ContrastModel> GetMonthPlanCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime);

        //日工效查询
        List<ContrastModel> GetDayPlanCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime);

        /// 获取流程执行力数据SQL（人员对比）
        List<FormDuplicateTimeModel> GetTimeMoldeByUserid(int[] userid, DateTime creatTime, DateTime endTime);

        /// 人员工效价值对比
        List<PerWorkTimeModel> GetMothPerWorkList(int[] userid, string statisticalTime, string endTime);

        /// 部门工效价值对比
        List<PerWorkTimeModel> GetMothPerWorkListByDepment(int[] orgId, string statisticalTime, string endTime);

        //   目标对比
        List<ContrastModel> GetYearCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime);

        List<ContrastModel> GetMonthCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime);

        List<ContrastModel> GetdayCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime);
    }
}