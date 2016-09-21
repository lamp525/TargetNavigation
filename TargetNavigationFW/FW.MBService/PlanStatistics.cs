using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace FW.MBService
{
    public class PlanStatistics
    {
        /// <summary>
        /// 计划统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        public void Statistics(DateTime startTime, DateTime endTime, ConstVar.StatisticsType type)
        {
            var resultList = new List<CompleteStatisticsModel>();

            //取得统计时间内的所有计划
            var allPlan = this.GetPlanList(startTime, endTime).GroupBy(p => p.organizationId);

            //按组织分组的用户计划信息
            IEnumerable<IGrouping<int?, tblPlan>> userPlanPerOrgList;

            /*计算各个组织中每个人的计划信息*/
            foreach (var planOrgGroup in allPlan)
            {
                userPlanPerOrgList = planOrgGroup.GroupBy(p => p.responsibleUser);
                foreach (var planUserGroup in userPlanPerOrgList)
                {
                    //用户计划统计信息Model
                    var userPlanStatistics = new CompleteStatisticsModel();
                    //统计时间
                    userPlanStatistics.statisticalTime = startTime.ToString("yyyyMMdd");
                    //组织ID
                    userPlanStatistics.organizationId = planOrgGroup.Key.Value;

                    //用户ID
                    userPlanStatistics.userId = planUserGroup.Key.Value;

                    //计划总数
                    userPlanStatistics.allCount = planUserGroup.Count();

                    //已完成数
                    userPlanStatistics.completeCount = planUserGroup.Where(p => p.status == 90).Count();

                    //未完成数
                    userPlanStatistics.notCompleteCount = userPlanStatistics.allCount - userPlanStatistics.completeCount;

                    //已中止数
                    userPlanStatistics.stopCount = planUserGroup.Where(p => p.stop == 90).Count();

                    //超时数
                    userPlanStatistics.timeOut = planUserGroup.Where(p => (p.updateTime.AddDays(-1).Date > p.endTime.Value.Date)).Count();

                    //添加各个组织各个用户的计划统计信息
                    resultList.Add(userPlanStatistics);
                }
            }
            //保存计划统计信息
            this.SavePlanStatistics(resultList, type);
        }

        #region 私有方法

        /// <summary>
        /// 保存用户的计划统计信息
        /// </summary>
        /// <param name="planStatisticsList"></param>
        /// <param name="type"></param>
        private void SavePlanStatistics(List<CompleteStatisticsModel> planStatisticsList, ConstVar.StatisticsType type)
        {
            switch (type)
            {
                //保存周统计数据
                case ConstVar.StatisticsType.Week:
                    this.SaveWeekData(planStatisticsList);
                    break;

                //保存月统计数据
                case ConstVar.StatisticsType.Month:
                    this.SaveMonthData(planStatisticsList);
                    break;

                //保存年统计数据
                case ConstVar.StatisticsType.Year:
                    this.SaveYearData(planStatisticsList);
                    break;
            }
        }

        /// <summary>
        /// 统计用户的周计划信息
        /// </summary>
        /// <param name="planStatisticsModel"></param>
        private void SaveWeekData(List<CompleteStatisticsModel> planStatisticsList)
        {
            try
            {
                using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
                {
                    foreach (var item in planStatisticsList)
                    {
                        var info = new tblWeekPlanCompleteStatistics
                        {
                            userId = item.userId,
                            organizationId = item.organizationId,
                            statisticalTime = item.statisticalTime,
                            completeCount = item.completeCount,
                            confirmedCount = item.confirmedCount,
                            examineCount = item.examineCount,
                            notComplateCount = item.notCompleteCount,
                            planCount = item.allCount,
                            stopPlanCount = item.stopCount,
                            submitCount = item.submitCount,
                            timeOut = item.timeOut
                        };

                        db.tblWeekPlanCompleteStatistics.Add(info);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        /// <summary>
        /// 统计用户的月计划信息
        /// </summary>
        /// <param name="planStatisticsList"></param>
        private void SaveMonthData(List<CompleteStatisticsModel> planStatisticsList)
        {
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                foreach (var item in planStatisticsList)
                {
                    var info = new tblMonthPlanCompleteStatistics
                    {
                        userId = item.userId,
                        organizationId = item.organizationId,
                        statisticalTime = item.statisticalTime.Substring(0, 6),
                        completeCount = item.completeCount,
                        confirmedCount = item.confirmedCount,
                        examineCount = item.examineCount,
                        notComplateCount = item.notCompleteCount,
                        planCount = item.allCount,
                        stopPlanCount = item.stopCount,
                        submitCount = item.submitCount,
                        timeOut = item.timeOut
                    };

                    db.tblMonthPlanCompleteStatistics.Add(info);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 统计用户的年计划信息
        /// </summary>
        /// <param name="planStatisticsList"></param>
        private void SaveYearData(List<CompleteStatisticsModel> planStatisticsList)
        {
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                foreach (var item in planStatisticsList)
                {
                    var info = new tblYearPlanCompleteStatistics
                    {
                        userId = item.userId,
                        organizationId = item.organizationId,
                        statisticalTime = item.statisticalTime.Substring(0, 4),
                        completeCount = item.completeCount,
                        confirmedCount = item.confirmedCount,
                        examineCount = item.examineCount,
                        notComplateCount = item.notCompleteCount,
                        planCount = item.allCount,
                        stopPlanCount = item.stopCount,
                        submitCount = item.submitCount,
                        timeOut = item.timeOut
                    };

                    db.tblYearPlanCompleteStatistics.Add(info);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 取得统计用计划列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<tblPlan> GetPlanList(DateTime startTime, DateTime endTime)
        {
            var list = new List<tblPlan>();
            using (var db = new TargetNavigationDBEntities())
            {
                var start = startTime.AddDays(-1).Date;
                var end = endTime.AddDays(1).Date;
                var planList = db.tblPlan.Where(p => p.deleteFlag == false && p.endTime > start && p.endTime < end).ToList();

                list.AddRange(planList);
            }

            return list;
        }

        /// <summary>
        /// 计算某日起始日期（礼拜一的日期）
        /// </summary>
        /// <param name="someDate">该周中任意一天</param>
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns>
        private DateTime GetMondayDate(DateTime someDate)
        {
            var dayDiff = (someDate.DayOfWeek == DayOfWeek.Sunday) ? (someDate.DayOfWeek + 7 - 1) : (someDate.DayOfWeek - 1);

            return someDate.Subtract(new TimeSpan((int)dayDiff, 0, 0, 0));
        }

        #endregion 私有方法
    }
}