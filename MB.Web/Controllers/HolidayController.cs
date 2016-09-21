using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class HolidayController : BaseController
    {
        //
        // GET: /Holiday/

        public ActionResult Holiday()
        {
            return View();
        }

        public string GetHolidayByYear(string thisyear)
        {
            HolidayBLL holidayBll = new HolidayBLL();
            var dataJson = Request.Form["data"];
            var List = new SaveHolidayModel();
            List.holiday = new List<string>();
            List.workday = new List<string>();
            var HolidayList = holidayBll.GetHolidayModel(DateTime.Parse(dataJson + "-01-01"));
            foreach (var item in HolidayList)
            {
                if (item.type == 1)
                {
                    List.holiday.Add(item.Holiday.ToString("yyyy-MM-dd"));
                }
                else
                {
                    List.workday.Add(item.Holiday.ToString("yyyy-MM-dd"));
                }
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, List, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string AddNewHoliday()
        {
            HolidayBLL holidayBll = new HolidayBLL();
            var dataJson = Request.Form["data"];
            var Holiday = JsonConvert.DeserializeObject<SaveHolidayModel>(dataJson);
            var saveList = new List<HolidayModel>();
            foreach (var item in Holiday.holiday)
            {
                var i = new HolidayModel();
                i.Holiday = Convert.ToDateTime(item);
                i.type = 1;
                i.updateTime = DateTime.Now.ToShortDateString();
                i.creatTime = DateTime.Now.ToShortDateString();
                i.creatUserId = Convert.ToInt32(Session["userId"].ToString());
                i.updateUserId = Convert.ToInt32(Session["userId"].ToString());
                saveList.Add(i);
            }
            foreach (var item in Holiday.workday)
            {
                var i = new HolidayModel();
                i.Holiday = Convert.ToDateTime(item);
                i.type = 2;
                i.updateTime = DateTime.Now.ToShortDateString();
                i.creatTime = DateTime.Now.ToShortDateString();
                i.creatUserId = Convert.ToInt32(Session["userId"].ToString());
                i.updateUserId = Convert.ToInt32(Session["userId"].ToString());
                saveList.Add(i);
            }
            foreach (var item in Holiday.defday)
            {
                var i = new HolidayModel();
                i.Holiday = Convert.ToDateTime(item);
                i.type = 3;
                i.updateTime = DateTime.Now.ToShortDateString();
                i.creatTime = DateTime.Now.ToShortDateString();
                i.creatUserId = Convert.ToInt32(Session["userId"].ToString());
                i.updateUserId = Convert.ToInt32(Session["userId"].ToString());
                saveList.Add(i);
            }
            var flag = holidayBll.AddNewHoliday(saveList);

            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}