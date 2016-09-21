using System;
using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IHolidayBLL
    {
        /// 节假日查询
        List<HolidayModel> GetHolidayModel(DateTime MothYearList);

        /// 添加新假日并删除重复
        bool AddNewHoliday(List<HolidayModel> MothYearList);
    }
}