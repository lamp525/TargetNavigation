using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.WorkTime
{
    public class WorkTimeBLL : IWorkTimeBLL
    {
        /// <summary>
        /// 获取月有效工时排行
        /// </summary>
        /// <param name="db"></param>
        /// <param name="yearMonth"></param>
        /// <returns></returns>
        public List<UserOrgWorkTimeModel> GetOrgWorkTimeByMonth(TargetNavigationDBEntities db, string yearMonth)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (string.IsNullOrEmpty(yearMonth))
            {
                throw new ArgumentNullException("yearMonth");
            }

            // 年月设置
            var condition = "YearMonth == @0";
            var value = new object[] { yearMonth };

            var orgWorkTimeList = db.vOrgPersonMonthWorkTime.Where(condition, value).GroupBy(p => p.responsibleUser).Select(g => new UserOrgWorkTimeModel
            {
                headImage = string.IsNullOrEmpty(g.Max(p => p.userImage)) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + g.Max(p => p.userImage),
                userId = g.Key,
                userName = g.Max(p => p.userName),
                workTime = Math.Round(g.Sum(p => p.workTimes) / 60, 1)
            }).ToList();

            return orgWorkTimeList;
        }

        /// <summary>
        /// 获取周有效工时排行
        /// </summary>
        /// <param name="db"></param>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        public List<UserOrgWorkTimeModel> GetOrgWorkTimeByWeek(TargetNavigationDBEntities db, int year, int weekOfYear)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var condition = "weekOfYear == @0  And Year == @1";
            var value = new object[] { weekOfYear, year };
            var orgWorkTimeList = db.vOrgPersonWeekWorkTime.Where(condition, value).GroupBy(p => p.responsibleUser).Select(g => new UserOrgWorkTimeModel
            {
                headImage = string.IsNullOrEmpty(g.Max(p => p.userImage)) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + g.Max(p => p.userImage),
                userId = g.Key,
                userName = g.Max(p => p.userName),
                workTime = Math.Round(g.Sum(p => p.workTimes) / 60, 1)
            }).ToList();

            return orgWorkTimeList;
        }

        /// <summary>
        /// 获取个人天自评工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public decimal GetPerSelfTimeByDay(TargetNavigationDBEntities db, int userId, DateTime day)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var workTime = GetPerTimeByDay(db, userId, day);

            // 取不到对应数据，返回0
            if (workTime == null)
            {
                return 0;
            }

            return workTime.actualWorkTime.HasValue ? workTime.actualWorkTime.Value : 0;
        }

        /// <summary>
        /// 获取个人月自评工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="yearMonth"></param>
        /// <returns></returns>
        public decimal GetPerSelfTimeByMonth(TargetNavigationDBEntities db, int userId, string yearMonth)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var workTime = GetPerTimeByMonth(db, userId, yearMonth);

            // 取不到对应数据，返回0
            if (workTime == null)
            {
                return 0;
            }

            return workTime.actualWorkTime.HasValue ? workTime.actualWorkTime.Value : 0;
        }

        /// <summary>
        /// 获取个人周自评工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        public decimal GetPerSelfWorkTimeByWeek(TargetNavigationDBEntities db, int userId, int year, int weekOfYear)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var workTime = GetPerTimeByWeek(db, userId, year, weekOfYear);

            // 取不到对应数据，返回0
            if (workTime == null)
            {
                return 0;
            }

            return workTime.actualWorkTime.HasValue ? workTime.actualWorkTime.Value : 0;
        }

        /// <summary>
        /// 获取个人天有效工时/实际工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public WorkTimeStatisticsModel GetPerTimeByDay(TargetNavigationDBEntities db, int userId, DateTime day)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 取得当日范围
            var fromTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            var toTime = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

            // 设置检索条件（责任人、完成时间范围、状态为已完成、未删除）
            var condition = "responsibleUser == @0 AND endTime >= @1 AND endTime <= @2 AND status == @3 AND deleteFlag == false ";
            var value = new object[] { userId, fromTime, toTime, (int)EnumDefine.PlanStatus.Complete };

            var workTime = db.tblPlan.Where(condition, value).GroupBy(p => p.responsibleUser).Select(g => new WorkTimeStatisticsModel
            {
                // 实际工时
                actualWorkTime = g.Sum(p => p.time * p.quantity),
                // 有效工时
                effectiveWorkTime = g.Sum(p => p.completeTime * p.completeQuantity * p.completeQuality * p.time * p.quantity)
            }).FirstOrDefault();

            return workTime;
        }

        /// <summary>
        /// 获取个人月有效工时/实际工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public WorkTimeStatisticsModel GetPerTimeByMonth(TargetNavigationDBEntities db, int userId, string yearMonth)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (string.IsNullOrEmpty(yearMonth))
            {
                throw new ArgumentNullException("yearMonth");
            }

            // 责任人、年月值设定
            var condition = "responsibleUser == @0 and YearMonth == @1";
            var value = new object[] { userId, yearMonth };

            // 取得当前年月责任人的有效工时、实际工时
            var workTime = db.vOrgPersonMonthWorkTime.Where(condition, value).GroupBy(o => new { o.responsibleUser, o.YearMonth }).Select(g => new WorkTimeStatisticsModel
            {
                // 实际工时
                actualWorkTime = g.Sum(o => o.actualTimes),
                // 有效工时
                effectiveWorkTime = g.Sum(o => o.workTimes)
            }).FirstOrDefault();

            return workTime;
        }

        /// <summary>
        /// 获取个人周有效工时/实际工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public WorkTimeStatisticsModel GetPerTimeByWeek(TargetNavigationDBEntities db, int userId, int year, int weekOfYear)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 责任人、年周值设定
            var condition = "responsibleUser == @0 AND Year == @1 AND weekOfYear == @2";
            var value = new object[] { userId, year, weekOfYear };

            // 取得当前年周责任人的有效工时、实际工时
            var workTime = db.vOrgPersonWeekWorkTime.Where(condition, value).GroupBy(o => new { o.responsibleUser, o.Year, o.weekOfYear }).Select(g => new WorkTimeStatisticsModel
            {
                // 实际工时
                actualWorkTime = g.Sum(o => o.actualTimes),
                // 有效工时
                effectiveWorkTime = g.Sum(o => o.workTimes)
            }).FirstOrDefault();

            return workTime;
        }

        /// <summary>
        /// 获取个人天有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public decimal GetPerWorkTimeByDay(TargetNavigationDBEntities db, int userId, DateTime day)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var workTime = GetPerTimeByDay(db, userId, day);

            // 取不到对应数据，返回0
            if (workTime == null)
            {
                return 0;
            }

            return workTime.effectiveWorkTime.HasValue ? workTime.effectiveWorkTime.Value : 0;
        }

        /// <summary>
        /// 获取个人月有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="thisMonthFirst"></param>
        /// <returns></returns>
        public decimal GetPerWorkTimeByMonth(TargetNavigationDBEntities db, int userId, string yearMonth)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (string.IsNullOrEmpty(yearMonth))
            {
                throw new ArgumentNullException("yearMonth");
            }

            var workTime = GetPerTimeByMonth(db, userId, yearMonth);

            // 取不到对应数据，返回0
            if (workTime == null)
            {
                return 0;
            }

            return workTime.effectiveWorkTime.HasValue ? workTime.effectiveWorkTime.Value : 0;
        }

        /// <summary>
        /// 获取个人周有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public decimal GetPerWorkTimeByWeek(TargetNavigationDBEntities db, int userId, int year, int weekOfYear)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var workTime = GetPerTimeByWeek(db, userId, year, weekOfYear);

            // 取不到对应数据，返回0
            if (workTime == null)
            {
                return 0;
            }

            return workTime.effectiveWorkTime.HasValue ? workTime.effectiveWorkTime.Value : 0;
        }
    }
}