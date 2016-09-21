using MB.Facade.Calendar;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class CalendarController : BaseController
    {
        //
        // GET: /Calendar/

        private ICalendarFacade facade { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得当前周的日程统计信息
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public string GetWeekCalendarCount(string fromDate, string toDate)
        {
            var userId = LoginUserInfo().userId;
            var weelCalendarCount = facade.GetWeekCalendarCount(DateTime.Parse(fromDate), DateTime.Parse(toDate), userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, weelCalendarCount);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 添加日程信息
        /// </summary>
        /// <returns></returns>
        public string AddCalendar()
        {
            PageCalendarInfoModel calendarInfo = JsonConvert.DeserializeObject<PageCalendarInfoModel>(Request.Form["data"]);

            calendarInfo.createUserId = LoginUserInfo().userId;
            calendarInfo.updateUserId = LoginUserInfo().userId;

            facade.AddCalendarInfo(calendarInfo);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "日程添加成功!");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 修改日程信息
        /// </summary>
        /// <returns></returns>
        public string UpdCalendar()
        {
            PageCalendarInfoModel calendarInfo = JsonConvert.DeserializeObject<PageCalendarInfoModel>(Request.Form["data"]);
            facade.UpdCalendarInfo(calendarInfo);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "日程修改成功!");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除日程信息
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string DelCalendar(int calendarId)
        {
            facade.DelCalendarInfo(calendarId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "日程删除成功!");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得选中日期的日程信息
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetDayCalendarInfo(string date)
        {
            var userId = LoginUserInfo().userId;
            var DayCalendarInfo = facade.GetDayCalendarInfo(userId, date);
            var jsonResult = new JsonResultModel(JsonResultType.success, DayCalendarInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得日程详情信息
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string GetCalendarDetailInfo(int calendarId)
        {
            var DayCalendarInfo = facade.GetCalendarDetailInfo(calendarId);
            var jsonResult = new JsonResultModel(JsonResultType.success, DayCalendarInfo);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}