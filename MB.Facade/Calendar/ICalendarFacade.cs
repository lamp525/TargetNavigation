using System;
using System.Collections.Generic;

namespace MB.Facade.Calendar
{
    public interface ICalendarFacade
    {
        /// <summary>
        /// 获取时间段中是否存在日程
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<PageCalendarCountModel> GetWeekCalendarCount(DateTime fromDate, DateTime toDate, int userId);

        /// <summary>
        /// 新增日程
        /// </summary>
        /// <param name="calendar"></param>
        void AddCalendarInfo(PageCalendarInfoModel calendar);

        /// <summary>
        /// 修改日程
        /// </summary>
        /// <param name="calendar"></param>
        void UpdCalendarInfo(PageCalendarInfoModel calendar);

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="calendarId"></param>
        void DelCalendarInfo(int calendarId);

        /// <summary>
        /// 获取日程列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        List<PageCalendarInfoModel> GetDayCalendarInfo(int userId, string date);

        /// <summary>
        /// 获取日程详情
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        PageCalendarInfoModel GetCalendarDetailInfo(int calendarId);
    }
}