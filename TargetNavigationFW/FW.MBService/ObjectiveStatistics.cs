using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace FW.MBService
{
    public class ObjectiveStatistics
    {
        /// <summary>
        /// 目标统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        public void Statistics(DateTime startTime, DateTime endTime, ConstVar.StatisticsType type)
        {
            var resultList = new List<CompleteStatisticsModel>();

            //取得所有未统计的目标
            var allObjective = this.GetObjectiveList(startTime, endTime).GroupBy(p => p.responsibleOrg);

            //按组织分组的用户目标信息
            IEnumerable<IGrouping<int?, tblObjective>> userObjectivePerOrgList;
            //计算各个组织中的每个人的目标信息
            foreach (var objectiveOrgGroup in allObjective)
            {
                userObjectivePerOrgList = objectiveOrgGroup.GroupBy(p => p.responsibleUser);
                foreach (var objectiveUserGroup in userObjectivePerOrgList)
                {
                    //用户目标统计信息Model
                    var userObjectiveStatistics = new CompleteStatisticsModel();
                    //统计时间
                    userObjectiveStatistics.statisticalTime = startTime.ToString("yyyyMMdd");
                    //组织ID
                    userObjectiveStatistics.organizationId = objectiveOrgGroup.Key.Value;
                    //用户ID
                    userObjectiveStatistics.userId = objectiveUserGroup.Key.Value;

                    //目标总数

                    userObjectiveStatistics.allCount = objectiveUserGroup.Count();

                    //已完成数
                    userObjectiveStatistics.completeCount = objectiveUserGroup.Where(p => p.status == (int)ConstVar.ObjectIndexStatus.hasCompleted).Count();

                    //待确认
                    userObjectiveStatistics.confirmedCount = objectiveUserGroup.Where(p => p.status == (int)ConstVar.ObjectIndexStatus.unConfirm).Count();

                    //未完成
                    userObjectiveStatistics.notCompleteCount = objectiveUserGroup.Where(p => p.status == (int)ConstVar.ObjectIndexStatus.hasChecked).Count();

                    //审核中
                    userObjectiveStatistics.examineCount = objectiveUserGroup.Where(p => p.status == (int)ConstVar.ObjectIndexStatus.unConfirm).Count();

                    //待提交
                    userObjectiveStatistics.submitCount = objectiveUserGroup.Where(p => p.status == (int)ConstVar.ObjectIndexStatus.unSubmit).Count();

                    //超时天数
                    userObjectiveStatistics.timeOut = objectiveUserGroup.Where(p => ((p.actualEndTime.HasValue ? p.actualEndTime.Value.Date : DateTime.Now.AddDays(-1).Date) - p.endTime.Value.Date).Days > 1).Count();

                    resultList.Add(userObjectiveStatistics);
                }
            }

            //保存统计数据
            this.SaveObjectiveStatistics(resultList, type);
        }

        /// <summary>
        /// 保存用户的目标统计信息
        /// </summary>
        /// <param name="objectiveStatisticsList"></param>
        /// <param name="type"></param>
        private void SaveObjectiveStatistics(List<CompleteStatisticsModel> objectiveStatisticsList, ConstVar.StatisticsType type)
        {
            switch (type)
            {
                //保存周统计数据
                case ConstVar.StatisticsType.Week:
                    this.SaveWeekData(objectiveStatisticsList);
                    break;

                //保存月统计数据
                case ConstVar.StatisticsType.Month:
                    this.SaveMonthData(objectiveStatisticsList);
                    break;

                //保存年统计数据
                case ConstVar.StatisticsType.Year:
                    this.SaveYearData(objectiveStatisticsList);
                    break;
            }
        }

        /// <summary>
        /// 统计用户的周目标信息
        /// </summary>
        /// <param name="objectiveStatisticsList"></param>
        private void SaveWeekData(List<CompleteStatisticsModel> objectiveStatisticsList)
        {
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                foreach (var item in objectiveStatisticsList)
                {
                    var info = new tblWeekTargetCompleteStatistics
                    {
                        userId = item.userId,
                        organizationId = item.organizationId,
                        statisticalTime = item.statisticalTime,
                        completeCount = item.completeCount,
                        confirmedCount = item.confirmedCount,
                        examineCount = item.examineCount,
                        notComplateCount = item.notCompleteCount,
                        objectiveCount = item.allCount,
                        stopCount = item.stopCount,
                        submitCount = item.submitCount,
                        timeOut = item.timeOut
                    };

                    db.tblWeekTargetCompleteStatistics.Add(info);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 统计用户的月目标信息
        /// </summary>
        /// <param name="objectiveStatisticsList"></param>
        private void SaveMonthData(List<CompleteStatisticsModel> objectiveStatisticsList)
        {
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                foreach (var item in objectiveStatisticsList)
                {
                    var info = new tblMonthTargetCompleteStatistics
                    {
                        userId = item.userId,
                        organizationId = item.organizationId,
                        statisticalTime = item.statisticalTime.Substring(0, 6),
                        completeCount = item.completeCount,
                        confirmedCount = item.confirmedCount,
                        examineCount = item.examineCount,
                        notComplateCount = item.notCompleteCount,
                        objectiveCount = item.allCount,
                        stopCount = item.stopCount,
                        submitCount = item.submitCount,
                        timeOut = item.timeOut
                    };

                    db.tblMonthTargetCompleteStatistics.Add(info);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 统计用户的年目标信息
        /// </summary>
        /// <param name="objectiveStatisticsList"></param>
        private void SaveYearData(List<CompleteStatisticsModel> objectiveStatisticsList)
        {
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                foreach (var item in objectiveStatisticsList)
                {
                    var info = new tblYearTargetCompleteStatistics
                    {
                        userId = item.userId,
                        organizationId = item.organizationId,
                        statisticalTime = item.statisticalTime.Substring(0, 4),
                        completeCount = item.completeCount,
                        confirmedCount = item.confirmedCount,
                        examineCount = item.examineCount,
                        notComplateCount = item.notCompleteCount,
                        objectiveCount = item.allCount,
                        stopCount = item.stopCount,
                        submitCount = item.submitCount,
                        timeOut = item.timeOut
                    };

                    db.tblYearTargetCompleteStatistics.Add(info);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 取得统计用目标列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<tblObjective> GetObjectiveList(DateTime startTime, DateTime endTime)
        {
            var list = new List<tblObjective>();
            using (var db = new TargetNavigationDBEntities())
            {
                var start = startTime.AddDays(-1).Date;
                var end = endTime.AddDays(1).Date;
                var planList = db.tblObjective.Where(p => p.deleteFlag == false && p.endTime > start && p.endTime < end).ToList();

                list.AddRange(planList);
            }

            return list;
        }
    }
}