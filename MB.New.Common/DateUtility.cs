using System;
using System.Globalization;

namespace MB.New.Common
{
    public class DateUtility
    {
        /// <summary>
        /// 得到本周第一天(以星期日为第一天)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekFirstDay(DateTime datetime)
        {
            // 星期日为第一天
            int weeknow = (int)datetime.DayOfWeek;

            // 本周第一天
            string firstDay = datetime.AddDays(0 - weeknow).ToString("yyyy-MM-dd");

            return DateTime.Parse(firstDay);
        }

        /// <summary>
        /// 得到本周最后一天(以星期六为最后一天)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekLastDay(DateTime datetime)
        {
            // 星期六为最后一天
            int weeknow = (int)datetime.DayOfWeek;

            //本周最后一天
            string LastDay = datetime.AddDays((7 - weeknow) - 1).ToString("yyyy-MM-dd");

            return DateTime.Parse(LastDay);
        }

        /// <summary>
        /// 得到本月第一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetMonthFirstDay(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, 1);
        }

        /// <summary>
        /// 得到本月最后一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(DateTime datetime)
        {
            return (new DateTime(datetime.Year, datetime.Month, 1)).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 根据日期获取今年第几周
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(DateTime datetime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(datetime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            return weekOfYear;
        }

        /// <summary>
        /// 根据全年第几周取得周日期范围
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="weekIndex">第几周</param>
        /// <returns></returns>
        public static DateTime[] GetWeekRange(int year, int weekIndex)
        {
            // 第一周开始，小于1为错误
            if (weekIndex < 1)
            {
                return null;
            }

            int allDays = (weekIndex - 1) * 7;
            //确定当年第一天
            DateTime firstDate = new DateTime(year, 1, 1);
            int firstDayOfWeek = (int)firstDate.DayOfWeek;

            firstDayOfWeek = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;

            //周开始日
            int startAddDays = allDays - firstDayOfWeek;
            DateTime weekRangeStart = firstDate.AddDays(startAddDays);

            //周结束日
            int endAddDays = allDays - firstDayOfWeek + 6;
            DateTime weekRangeEnd = firstDate.AddDays(endAddDays);

            // 当年不存在指定的周
            if (weekRangeStart.Year > year ||
             (weekRangeStart.Year == year && weekRangeEnd.Year > year))
            {
                return null;
            }

            return new DateTime[] { weekRangeStart, weekRangeEnd };
        }
    }
}