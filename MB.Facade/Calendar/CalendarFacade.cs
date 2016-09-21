using MB.DAL;
using MB.New.BLL.Calendar;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;

namespace MB.Facade.Calendar
{
    public class CalendarFacade : ICalendarFacade
    {
        /// <summary>
        /// 根据用户ID和时间获取日程信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<PageCalendarInfoModel> GetDayCalendarInfo(int userId, string date)
        {
            ICalendarBLL calendarBLL = new CalendarBLL();

            List<PageCalendarInfoModel> pageModel = new List<PageCalendarInfoModel>();
            List<CalendarInfoModel> calendarInfoModel = null;

            using (var db = new TargetNavigationDBEntities())
            {
                calendarInfoModel = calendarBLL.GetCalendarByDate(db, DateTime.Parse(date), userId);
            }

            ModelMapping.MultiMapping(calendarInfoModel, pageModel);

            return pageModel;
        }

        /// <summary>
        /// 获取日程详情
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public PageCalendarInfoModel GetCalendarDetailInfo(int calendarId)
        {
            ICalendarBLL calendarBLL = new CalendarBLL();

            PageCalendarInfoModel pageModel = new PageCalendarInfoModel();
            CalendarInfoModel calendarInfoModel = null;

            using (var db = new TargetNavigationDBEntities())
            {
                calendarInfoModel = calendarBLL.GetCalendarById(db, calendarId);
            }

            ModelMapping.SingleMapping(calendarInfoModel, pageModel);
            return pageModel;
        }

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="calendarId"></param>
        public void DelCalendarInfo(int calendarId)
        {
            ICalendarBLL calendarBLL = new CalendarBLL();
            using (var db = new TargetNavigationDBEntities())
            {
                calendarBLL.DelCalendarById(db, calendarId);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 新增日程
        /// </summary>
        /// <param name="calendar"></param>
        public void AddCalendarInfo(PageCalendarInfoModel calendar)
        {
            ICalendarBLL calendarBLL = new CalendarBLL();
            CalendarInfoModel calendarInfoModel = new CalendarInfoModel();
            ModelMapping.SingleMapping(calendar, calendarInfoModel);

            using (var db = new TargetNavigationDBEntities())
            {
                calendarBLL.AddCalendarInfo(db, calendarInfoModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改日程
        /// </summary>
        /// <param name="calendar"></param>
        public void UpdCalendarInfo(PageCalendarInfoModel calendar)
        {
            ICalendarBLL calendarBLL = new CalendarBLL();
            CalendarInfoModel calendarmodel = new CalendarInfoModel();
            ModelMapping.SingleMapping(calendar, calendarmodel);
            using (var db = new TargetNavigationDBEntities())
            {
                calendarBLL.UpdCalendarInfo(db, calendarmodel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取时间段中是否存在日程
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PageCalendarCountModel> GetWeekCalendarCount(DateTime fromDate, DateTime toDate, int userId)
        {
            ICalendarBLL calendarBLL = new CalendarBLL();
            List<PageCalendarCountModel> pageModel = new List<PageCalendarCountModel>();
            List<CalendarCountModel> calendarCountModel = null;

            using (var db = new TargetNavigationDBEntities())
            {
                calendarCountModel = calendarBLL.GetWeekCalendarCount(db, fromDate, toDate, userId);
            }

            ModelMapping.MultiMapping(calendarCountModel, pageModel);
            return pageModel;
        }
    }
}