using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class Incentivecontrast : IIncentivecontrast
    {
        /// <summary>
        /// 年执行力
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="organizationId"></param>
        /// <param name="statisticalTime"></param>
        /// <returns></returns>
        /// //   effectiveTimeSum = b.Sum(c => c.effectiveTime),
        //   workTimeSum = b.Sum(c => c.workTime)
        public List<ContrastModel> GetYearPlanCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime)
        {
            ContrastModel ConditionList = new ContrastModel();
            var List = new List<ContrastModel>();
            var putList = new List<ContrastModel>();
            var l = new List<ContrastModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        List = (from year in db.tblYearPlanCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.userId == id && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.planCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.planCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.userName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                if (organizationId.Count() != 0)
                {
                    foreach (var orgid in organizationId)
                    {
                        List = (from year in db.tblYearPlanCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.organizationId == orgid && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.planCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.planCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.organizationName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                l = (from p in putList
                     group p by p.name into g
                     select new ContrastModel
                     {
                         completeCount = g.Sum(p => p.completeCount),
                         planCount = g.Sum(p => p.planCount),
                         timeoutCount = g.Sum(p => p.timeoutCount),
                         name = g.Key
                     }).ToList();
            }
            return l;
        }

        /// <summary>
        /// 月执行力
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="organizationId"></param>
        /// <param name="statisticalTime"></param>
        /// <returns></returns>
        public List<ContrastModel> GetMonthPlanCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime)
        {
            ContrastModel ConditionList = new ContrastModel();
            var List = new List<ContrastModel>();
            var putList = new List<ContrastModel>();
            var l = new List<ContrastModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        List = (from year in db.tblMonthPlanCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.userId == id && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.planCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.planCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.userName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                if (organizationId.Count() != 0)
                {
                    foreach (var orgid in organizationId)
                    {
                        List = (from year in db.tblMonthPlanCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.organizationId == orgid && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.planCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.planCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.organizationName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                l = (from p in putList
                     group p by p.name into g
                     select new ContrastModel
                     {
                         completeCount = g.Sum(p => p.completeCount),
                         planCount = g.Sum(p => p.planCount),
                         timeoutCount = g.Sum(p => p.timeoutCount),
                         name = g.Key
                     }).ToList();
            }
            return l;
        }

        //日工效查询
        /// <summary>
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="organizationId"></param>
        /// <param name="statisticalTime"></param>
        /// <returns></returns>
        public List<ContrastModel> GetDayPlanCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime)
        {
            ContrastModel ConditionList = new ContrastModel();
            var List = new List<ContrastModel>();
            var putList = new List<ContrastModel>();
            var l = new List<ContrastModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        List = (from year in db.tblWeekPlanCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.userId == id && (Convert.ToInt32(year.statisticalTime) <= Convert.ToInt32(statisticalTime) || Convert.ToInt32(year.statisticalTime) >= Convert.ToInt32(endTime))
                                group year by new { year.completeCount, year.planCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.planCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.userName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                if (organizationId.Count() != 0)
                {
                    foreach (var orgid in organizationId)
                    {
                        List = (from year in db.tblWeekPlanCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.organizationId == orgid && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.planCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.planCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.organizationName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                l = (from p in putList
                     group p by p.name into g
                     select new ContrastModel
                     {
                         completeCount = g.Sum(p => p.completeCount),
                         planCount = g.Sum(p => p.planCount),
                         timeoutCount = g.Sum(p => p.timeoutCount),
                         name = g.Key
                     }).ToList();
            }
            return l;
        }

        /// <summary>
        /// 获取流程执行力数据SQL（人员对比）
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="creatTime"></param>
        /// <returns></returns>
        public List<FormDuplicateTimeModel> GetTimeMoldeByUserid(int[] userid, DateTime creatTime, DateTime endTime)
        {
            FormDuplicateTimeModel modelList = new FormDuplicateTimeModel();
            var list = new List<FormDuplicateTimeModel>();
            var putlist = new List<FormDuplicateTimeModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        list = (from a in db.tblFormDuplicate
                                join b in db.tblFormFlow on a.formId equals b.formId
                                join user in db.tblUser on a.userId equals user.userId
                                where a.createTime >= creatTime && a.createTime <= endTime && a.userId == id
                                select new FormDuplicateTimeModel
                                {
                                    createStartTime = a.createTime,
                                    userName = user.userName,
                                    creatTrueTime = b.createTime,
                                    userId = a.userId
                                }).ToList();
                        foreach (var item in list)
                        {
                            putlist.Add(item);
                        }
                    }
                }
            }
            return putlist;
        }

        /// <summary>
        /// 人员工效价值对比
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="statisticalTime"></param>
        /// <returns></returns>
        public List<PerWorkTimeModel> GetMothPerWorkList(int[] userid, string statisticalTime, string endTime)
        {
            PerWorkTimeModel modelList = new PerWorkTimeModel();
            var list = new List<PerWorkTimeModel>();
            var putlist = new List<PerWorkTimeModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        if (statisticalTime.Length == 8)
                        {
                            list = (from a in db.tblPerDayWorkTime
                                    join user in db.tblUser on a.userId equals user.userId
                                    where a.userId == id && (a.statisticalTime == statisticalTime || a.statisticalTime == endTime)
                                    group a by new { a.effectiveTime, a.workTime, a.userId, user.userName } into b
                                    select new PerWorkTimeModel
                                    {
                                        id = b.Key.userId,
                                        name = b.Key.userName,
                                        effectiveTimeSum = b.Sum(c => c.effectiveTime),
                                        workTimeSum = b.Sum(c => c.workTime)
                                    }).ToList();
                        }
                        else
                        {
                            list = (from a in db.tblPerMonthWorkTime
                                    join user in db.tblUser on a.userId equals user.userId
                                    where a.userId == id && (a.statisticalTime == statisticalTime || a.statisticalTime == endTime)
                                    group a by new { a.effectiveTime, a.workTime, a.userId, user.userName } into b
                                    select new PerWorkTimeModel
                                    {
                                        id = b.Key.userId,
                                        name = b.Key.userName,
                                        effectiveTimeSum = b.Sum(c => c.effectiveTime),
                                        workTimeSum = b.Sum(c => c.workTime)
                                    }).ToList();
                        }
                        foreach (var item in list)
                        {
                            putlist.Add(item);
                        }
                    }
                }
            }
            return putlist;
        }

        /// <summary>
        /// 部门工效价值对比
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="statisticalTime"></param>
        /// <returns></returns>
        public List<PerWorkTimeModel> GetMothPerWorkListByDepment(int[] orgId, string statisticalTime, string endTime)
        {
            PerWorkTimeModel modelList = new PerWorkTimeModel();
            var list = new List<PerWorkTimeModel>();
            var putlist = new List<PerWorkTimeModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var orgid in orgId)
                {
                    list = (from a in db.tblOrgDayWorkTime
                            join org in db.tblOrganization on a.organizationId equals org.organizationId
                            where a.organizationId == orgid && (a.statisticalTime == statisticalTime || a.statisticalTime == endTime)
                            group a by new { a.effectiveTime, a.workTime, a.organizationId, org.organizationName } into b
                            select new PerWorkTimeModel
                            {
                                id = b.Key.organizationId,
                                name = b.Key.organizationName,
                                effectiveTimeSum = b.Sum(c => c.effectiveTime),
                                workTimeSum = b.Sum(c => c.workTime)
                            }).ToList();
                    foreach (var item in list)
                    {
                        putlist.Add(item);
                    }
                }
            }
            return putlist;
        }

        //   目标对比
        public List<ContrastModel> GetYearCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime)
        {
            ContrastModel ConditionList = new ContrastModel();
            var List = new List<ContrastModel>();
            var putList = new List<ContrastModel>();
            var l = new List<ContrastModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        List = (from year in db.tblYearTargetCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.userId == id && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.objectiveCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.objectiveCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.userName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                if (organizationId.Count() != 0)
                {
                    foreach (var orgid in organizationId)
                    {
                        List = (from year in db.tblYearTargetCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.organizationId == orgid && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.objectiveCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.objectiveCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.organizationName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                l = (from p in putList
                     group p by p.name into g
                     select new ContrastModel
                     {
                         completeCount = g.Sum(p => p.completeCount),
                         planCount = g.Sum(p => p.planCount),
                         timeoutCount = g.Sum(p => p.timeoutCount),
                         name = g.Key
                     }).ToList();
            }
            return l;
        }

        public List<ContrastModel> GetMonthCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime)
        {
            ContrastModel ConditionList = new ContrastModel();
            var List = new List<ContrastModel>();
            var putList = new List<ContrastModel>();
            var l = new List<ContrastModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        List = (from year in db.tblMonthTargetCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.userId == id && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.objectiveCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.objectiveCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.userName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                if (organizationId.Count() != 0)
                {
                    foreach (var orgid in organizationId)
                    {
                        List = (from year in db.tblMonthTargetCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.organizationId == orgid && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.objectiveCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.objectiveCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.organizationName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                l = (from p in putList
                     group p by p.name into g
                     select new ContrastModel
                     {
                         completeCount = g.Sum(p => p.completeCount),
                         planCount = g.Sum(p => p.planCount),
                         timeoutCount = g.Sum(p => p.timeoutCount),
                         name = g.Key
                     }).ToList();
            }
            return l;
        }

        public List<ContrastModel> GetdayCompleteStatisticsList(int[] userid, int[] organizationId, string statisticalTime, string endTime)
        {
            ContrastModel ConditionList = new ContrastModel();
            var List = new List<ContrastModel>();
            var putList = new List<ContrastModel>();
            var l = new List<ContrastModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userid.Count() != 0)
                {
                    foreach (var id in userid)
                    {
                        List = (from year in db.tblWeekTargetCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.userId == id && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.objectiveCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.objectiveCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.userName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                if (organizationId.Count() != 0)
                {
                    foreach (var orgid in organizationId)
                    {
                        List = (from year in db.tblWeekTargetCompleteStatistics
                                join org in db.tblOrganization on year.organizationId equals org.organizationId
                                join user in db.tblUser on year.userId equals user.userId
                                where year.organizationId == orgid && (year.statisticalTime == statisticalTime || year.statisticalTime == endTime)
                                group year by new { year.completeCount, year.objectiveCount, year.timeOut, user.userName, org.organizationName } into b
                                select new ContrastModel
                                {
                                    completeCount = b.Sum(c => c.completeCount),
                                    orgName = b.Key.organizationName,
                                    userName = b.Key.userName,
                                    planCount = b.Sum(c => c.objectiveCount),
                                    timeoutCount = b.Sum(c => c.timeOut),
                                    name = b.Key.organizationName
                                }).ToList();
                        foreach (var item in List)
                        {
                            putList.Add(item);
                        }
                    }
                }
                l = (from p in putList
                     group p by p.name into g
                     select new ContrastModel
                     {
                         completeCount = g.Sum(p => p.completeCount),
                         planCount = g.Sum(p => p.planCount),
                         timeoutCount = g.Sum(p => p.timeoutCount),
                         name = g.Key
                     }).ToList();
            }
            return l;
        }
    }
}