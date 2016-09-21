using MB.DAL;
using MB.New.Model;
using System;
using System.Collections.Generic;

namespace MB.New.BLL.WorkTime
{
    public interface IWorkTimeBLL
    {
        /// <summary>
        /// 获取个人月有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="yearMonth">格式为【yyyyMM】</param>
        /// <returns></returns>
        decimal GetPerWorkTimeByMonth(TargetNavigationDBEntities db, int userId, string yearMonth);

        /// <summary>
        /// 获取个人周有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        decimal GetPerWorkTimeByWeek(TargetNavigationDBEntities db, int userId, int year, int weekOfYear);

        /// <summary>
        /// 获取个人天有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        decimal GetPerWorkTimeByDay(TargetNavigationDBEntities db, int userId, DateTime day);

        /// <summary>
        /// 获取个人月实际工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="yearMonth">格式为【yyyyMM】</param>
        /// <returns></returns>
        decimal GetPerSelfTimeByMonth(TargetNavigationDBEntities db, int userId, string yearMonth);

        /// <summary>
        /// 获取个人周实际工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        decimal GetPerSelfWorkTimeByWeek(TargetNavigationDBEntities db, int userId, int year, int weekOfYear);

        /// <summary>
        /// 获取个人天实际工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        decimal GetPerSelfTimeByDay(TargetNavigationDBEntities db, int userId, DateTime day);

        /// <summary>
        /// 获取个人月有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="yearMonth">格式为【yyyyMM】</param>
        /// <returns></returns>
        WorkTimeStatisticsModel GetPerTimeByMonth(TargetNavigationDBEntities db, int userId, string yearMonth);

        /// <summary>
        /// 获取个人周有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        WorkTimeStatisticsModel GetPerTimeByWeek(TargetNavigationDBEntities db, int userId, int year, int weekOfYear);

        /// <summary>
        /// 获取个人天有效工时
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        WorkTimeStatisticsModel GetPerTimeByDay(TargetNavigationDBEntities db, int userId, DateTime day);

        /// <summary>
        /// 获取所有人月有效工时排名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="yearMonth">格式为【yyyyMM】</param>
        /// <returns></returns>
        List<UserOrgWorkTimeModel> GetOrgWorkTimeByMonth(TargetNavigationDBEntities db, string yearMonth);

        /// <summary>
        /// 获取所有人周有效工时排名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="yearMonth">格式为【yyyyMM】</param>
        /// <returns></returns>
        List<UserOrgWorkTimeModel> GetOrgWorkTimeByWeek(TargetNavigationDBEntities db, int year, int weekOfYear);
    }
}