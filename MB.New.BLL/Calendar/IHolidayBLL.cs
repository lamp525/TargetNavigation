using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;

namespace MB.New.BLL.Calendar
{
    public interface IHolidayBLL
    {
        /// <summary>
        /// 取得日期范围内工作日数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        int GetWorkdaysCount(TargetNavigationDBEntities db, DateTime from, DateTime to);

        /// <summary>
        /// 取得日期范围内日状态列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        List<HolidayInfoModel> GetDatesType(TargetNavigationDBEntities db, DateTime from, DateTime to);

        /// <summary>
        /// 取得指定日期的工作状态
        /// </summary>
        /// <param name="db"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        EnumDefine.DateType GetDateTypeByDay(TargetNavigationDBEntities db, DateTime target);
    }
}