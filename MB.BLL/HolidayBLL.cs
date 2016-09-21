using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class HolidayBLL : IHolidayBLL
    {
        /// <summary>
        /// 节假日查询
        /// </summary>
        /// <param name="MothYearList"></param>
        /// <returns></returns>
        public List<HolidayModel> GetHolidayModel(DateTime MothYearList)
        {
            var list = new List<HolidayModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from itm in db.tblHoliday
                        where itm.holiday.Year == MothYearList.Year
                        select new HolidayModel
                        {
                            type = itm.type,
                            Holiday = itm.holiday,
                            //holidayId = itm.holidayId
                        }).ToList();
            }
            return list;
        }

        /// <summary>
        /// 添加新假日并删除重复
        /// </summary>
        /// <param name="MothYearList"></param>
        /// <returns></returns>
        public bool AddNewHoliday(List<HolidayModel> MothYearList)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in MothYearList)
                {
                    if (item.type == 3)
                    {
                        var oldHoliday = db.tblHoliday.Where(b => b.holiday == item.Holiday).ToList();
                        if (oldHoliday != null)
                        {
                            foreach (var oldModel in oldHoliday)
                            {
                                db.tblHoliday.Remove(oldModel);
                            }
                         
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var oldHoliday = db.tblHoliday.Where(b => b.holiday == item.Holiday).ToList();
                        if (oldHoliday != null)
                        {
                            foreach (var hasModel in oldHoliday)
                            {
                                db.tblHoliday.Remove(hasModel);
                            }
                        }
                        System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        item.holidayId = db.prcGetPrimaryKey("tblHoliday", obj).FirstOrDefault().Value;
                        var addmodel = new tblHoliday
                        {
                            type = item.type,
                            holiday = Convert.ToDateTime(item.Holiday),
                            //createTime = Convert.ToDateTime(item.creatTime),
                            //createUser = item.creatUserId,
                            //holidayId = Convert.ToInt32(item.holidayId),
                            updateTime = Convert.ToDateTime(item.updateTime),
                            updateUser = item.updateUserId
                        };
                        db.tblHoliday.Add(addmodel);
                    }
                }
                db.SaveChanges();
            }
            return false;
        }
    }
}