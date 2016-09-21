using MB.DAL;
using MB.New.Model;
using System;
using System.Collections.Generic;

namespace MB.New.BLL.Calendar
{
    public interface ICalendarBLL
    {
        /// <summary>
        /// 获取时间段内是否有日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<CalendarCountModel> GetWeekCalendarCount(TargetNavigationDBEntities db, DateTime fromDate, DateTime toDate, int userId);

        /// <summary>
        /// 新增日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendar"></param>
        void AddCalendarInfo(TargetNavigationDBEntities db, CalendarInfoModel calendar);

        /// <summary>
        /// 更新日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendar"></param>
        void UpdCalendarInfo(TargetNavigationDBEntities db, CalendarInfoModel calendar);

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendarId"></param>
        void DelCalendarById(TargetNavigationDBEntities db, int calendarId);

        /// <summary>
        /// 获取日程详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        CalendarInfoModel GetCalendarById(TargetNavigationDBEntities db, int calendarId);

        /// <summary>
        /// 获取本日日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<CalendarInfoModel> GetCalendarByDate(TargetNavigationDBEntities db, DateTime date, int userId);
    }
}