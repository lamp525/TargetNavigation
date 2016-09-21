using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Calendar
{
    public class HolidayBLL : IHolidayBLL
    {
        /// <summary>
        /// 取得日期范围内日状态列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<HolidayInfoModel> GetDatesType(TargetNavigationDBEntities db, DateTime from, DateTime to)
        {
            // 设定检索条件
            string condition = " holiday >= @0 AND holiday <= @1 ";
            object[] values = new object[] { from, to };

            return db.tblHoliday.Where(condition, values).Select(h => new HolidayInfoModel
            {
                holiday = h.holiday,
                type = (EnumDefine.DateType)h.type,
                updateUser = h.updateUser,
                updateTime = h.updateTime
            }).ToList();
        }

        /// <summary>
        /// 取得指定日期的工作状态
        /// </summary>
        /// <param name="db"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public EnumDefine.DateType GetDateTypeByDay(TargetNavigationDBEntities db, DateTime target)
        {
            // 取得对应日期的状态
            var result = GetDatesType(db, target, target);

            // 找不到对应数据的场合，返回0：不确定
            if (result == null || result.Count == 0)
            {
                return EnumDefine.DateType.None;
            }

            // 返回结果
            return result[0].type;
        }

        /// <summary>
        /// 取得日期范围内工作日数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int GetWorkdaysCount(TargetNavigationDBEntities db, DateTime from, DateTime to)
        {
            // 设定检索条件
            string condition = " holiday >= @0 AND holiday <= @1 AND type == @2 ";
            object[] values = new object[] { from, to, (int)EnumDefine.DateType.Workday };

            return db.tblHoliday.Where(condition, values).Count();
        }
    }
}