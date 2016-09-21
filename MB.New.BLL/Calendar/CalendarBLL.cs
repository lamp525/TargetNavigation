using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Calendar
{
    public class CalendarBLL : ICalendarBLL
    {
        /// <summary>
        /// 获取本日日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CalendarInfoModel> GetCalendarByDate(TargetNavigationDBEntities db, DateTime date, int userId)
        {
            // 开始时间
            DateTime startime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            // 结束时间
            DateTime endtime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            // 条件设定
            var condition = " createUser == @0 and startTime <= @1 and endTime >= @2";
            var value = new object[] { userId, endtime, startime };

            var calendarInfoList = this.GetCalendarInfo(db, condition, value);

            return calendarInfoList;
        }

        /// <summary>
        /// 获取日程详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public CalendarInfoModel GetCalendarById(TargetNavigationDBEntities db, int calendarId)
        {
            // 条件设定
            var condition = "calendarId == @0";
            var value = new object[] { calendarId };

            var calendarInfo = this.GetCalendarInfo(db, condition, value).FirstOrDefault();

            return calendarInfo;
        }

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendarId"></param>
        public void DelCalendarById(TargetNavigationDBEntities db, int calendarId)
        {
            var data = db.tblCalendar.Where(c => c.calendarId == calendarId).FirstOrDefault();

            if (data != null)
            {
                db.tblCalendar.Remove(data);
            }
        }

        /// <summary>
        /// 新增日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendar"></param>
        public void AddCalendarInfo(TargetNavigationDBEntities db, CalendarInfoModel calendar)
        {
            int pk = DBUtility.GetPrimaryKeyByTableName(db, "tblCalendar");

            var newCalendar = new tblCalendar
            {
                calendarId = pk,
                comment = calendar.comment,
                endTime = calendar.endTime,
                startTime = calendar.startTime,
                place = calendar.place,
                createTime = DateTime.Now,
                createUser = calendar.createUserId,
                updateTime = DateTime.Now,
                updateUser = calendar.updateUserId
            };

            db.tblCalendar.Add(newCalendar);
        }

        /// <summary>
        /// 更新日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="calendar"></param>
        public void UpdCalendarInfo(TargetNavigationDBEntities db, CalendarInfoModel calendar)
        {
            tblCalendar data = db.tblCalendar.Where(c => c.calendarId == calendar.calendarId).FirstOrDefault();

            data.updateUser = calendar.updateUserId;
            data.updateTime = DateTime.Now;
            data.startTime = calendar.startTime;
            data.endTime = calendar.endTime;
            data.comment = calendar.comment;
            data.place = calendar.place;
        }

        /// <summary>
        /// 获取时间段内是否有日程
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CalendarCountModel> GetWeekCalendarCount(TargetNavigationDBEntities db, DateTime fromDate, DateTime toDate, int userId)
        {
            var list = new List<CalendarCountModel>();
            CalendarCountModel calendarCountModel = null;

            TimeSpan span = toDate - fromDate;
            DateTime from = fromDate;
            DateTime to = fromDate;

            // 循环日期范围
            for (int i = 0; i <= span.Days; i++)
            {
                calendarCountModel = new CalendarCountModel();

                // 日期设定
                calendarCountModel.date = from.AddDays(i);

                // 开始时间
                var fromthisdate = new DateTime(calendarCountModel.date.Year, calendarCountModel.date.Month, calendarCountModel.date.Day, 0, 0, 0);
                // 结束时间
                var tothisdate = new DateTime(calendarCountModel.date.Year, calendarCountModel.date.Month, calendarCountModel.date.Day, 23, 59, 59);

                // 取得指定日期的数量
                var calendarList = db.tblCalendar.Where(c => c.createUser == userId && c.startTime <= tothisdate && c.endTime >= fromthisdate).Count();

                // 设定是否存在
                calendarCountModel.isZero = (calendarList == 0);

                list.Add(calendarCountModel);
            }

            return list;
        }

        /// <summary>
        /// 根据条件取得日历信息列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private List<CalendarInfoModel> GetCalendarInfo(TargetNavigationDBEntities db, string condition, object[] value)
        {
            var calendarList = db.tblCalendar.Where(condition, value).Select(c => new CalendarInfoModel
            {
                calendarId = c.calendarId,
                comment = c.comment,
                createTime = c.createTime,
                updateUserId = c.updateUser,
                createUserId = c.createUser,
                createUserName = db.tblUser.Where(u => u.userId == c.createUser).FirstOrDefault().userName,
                endTime = c.endTime,
                place = c.place,
                startTime = c.startTime,
                updateTime = c.updateTime
            }).ToList();

            return calendarList;
        }
    }
}