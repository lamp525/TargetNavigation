using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class IncentiveContarstController : Controller
    {
        private IIncentivecontrast Incentivecontrast { get; set; }

      
        //
        // GET: /Incentive/
        //Incentivecontrast InBll = new Incentivecontrast();
        public ActionResult IncentiveContarst()
        {
            return View();
        }

        public string GetRewardPunishNum()
        {
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<IncentiveInfoModel>(copyDataJson);
            var list = new List<ContrastModel>();
            var statTime = copyDataModel.startTime.Replace("-", "");
            var endTime = copyDataModel.endTime.Replace("-", "");
            //计划执行力
            if (copyDataModel.type == 1)
            {
                switch (copyDataModel.contrastType)
                {
                    case 1:
                        list = Incentivecontrast.GetYearPlanCompleteStatisticsList(copyDataModel.userId, copyDataModel.orgId, statTime, statTime);
                        break;

                    case 2:
                        list = Incentivecontrast.GetMonthPlanCompleteStatisticsList(copyDataModel.userId, copyDataModel.orgId, statTime, endTime);
                        break;

                    case 3:
                        list = Incentivecontrast.GetDayPlanCompleteStatisticsList(copyDataModel.userId, copyDataModel.orgId, statTime, endTime);
                        break;
                }
            }
            //流程执行力
            else if (copyDataModel.type == 2)
            {
                var itemList = new List<FormDuplicateTimeModel>();
                switch (copyDataModel.contrastType)
                {
                    case 1:
                        itemList = Incentivecontrast.GetTimeMoldeByUserid(copyDataModel.userId, DateTime.Parse(copyDataModel.startTime + "-01-01 00:00"), DateTime.Parse(copyDataModel.startTime + "-12-30 23:59"));
                        break;

                    case 2:
                        itemList = Incentivecontrast.GetTimeMoldeByUserid(copyDataModel.userId, DateTime.Parse(copyDataModel.startTime + "-01 00:00"), DateTime.Parse(copyDataModel.endTime + "-30 23:59"));

                        break;

                    case 3:
                        itemList = Incentivecontrast.GetTimeMoldeByUserid(copyDataModel.userId, DateTime.Parse(copyDataModel.startTime + " 00:00:00"), DateTime.Parse(copyDataModel.endTime + " 23:59:00"));
                        break;
                }

                foreach (var item in itemList)
                {
                    var i = new ContrastModel();
                    i.userId = item.userId;
                    i.userName = item.userName;
                    i.name = item.userName;
                    i.planCount = itemList.Where(a => a.userId == item.userId).Count();
                    i.timeoutCount = itemList.Where(b => b.userId == item.userId && b.creatTrueTime > b.createStartTime).Count();
                    i.completeCount = itemList.Where(b => b.userId == item.userId && b.creatTrueTime <= b.createStartTime).Count();
                    list.Add(i);
                }
                if (list.Count() != 0)
                {
                    var bb = list;
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        for (int j = bb.Count - 1; j > i; j--)
                        {
                            if (bb[i].userId == bb[j].userId)
                            {
                                bb.RemoveAt(j); ;
                            }
                        }
                    }
                    list = bb;
                }
            }
            //功效价值
            else if (copyDataModel.type == 3)
            {
                var lists = new List<PerWorkTimeModel>();
                var model = new ContrastModel();
                if (copyDataModel.userId.Count() != 0)
                {
                    lists = Incentivecontrast.GetMothPerWorkList(copyDataModel.userId, statTime, endTime);
                    foreach (var item in lists)
                    {
                        var i = new ContrastModel();
                        i.userId = item.id;
                        i.userName = item.name;
                        i.name = item.name;
                        i.completeCount = Convert.ToInt32(item.effectiveTimeSum);
                        i.planCount = Convert.ToInt32(item.workTimeSum);
                        list.Add(i);
                    }
                }
                else
                {
                    lists = Incentivecontrast.GetMothPerWorkListByDepment(copyDataModel.orgId, statTime, endTime);
                    foreach (var item in lists)
                    {
                        var i = new ContrastModel();
                        i.organizationId = item.id;
                        i.orgName = item.name;
                        i.name = item.name;
                        i.completeCount = Convert.ToInt32(item.effectiveTimeSum);
                        i.planCount = Convert.ToInt32(item.workTimeSum);
                        list.Add(i);
                    }
                }
            }
            //目标价值
            else if (copyDataModel.type == 4)
            {
                switch (copyDataModel.contrastType)
                {
                    case 1:
                        list = Incentivecontrast.GetYearCompleteStatisticsList(copyDataModel.userId, copyDataModel.orgId, statTime, endTime);
                        break;

                    case 2:
                        list = Incentivecontrast.GetMonthCompleteStatisticsList(copyDataModel.userId, copyDataModel.orgId, statTime, endTime);
                        break;

                    case 3:
                        list = Incentivecontrast.GetdayCompleteStatisticsList(copyDataModel.userId, copyDataModel.orgId, statTime, endTime);
                        break;
                }
            }
            foreach (var item in list)
            {
                if (item != null)
                {
                    if (copyDataModel.type == 3)
                    {
                        item.comcompleteRate = Math.Round((decimal)item.planCount / (decimal)item.completeCount * 100, 2);
                    }
                    else
                    {
                        item.comcompleteRate = Math.Round((decimal)item.completeCount / (decimal)item.planCount * 100, 2);
                        item.timeoutRate = Math.Round((decimal)(item.timeoutCount) / (decimal)(item.planCount) * 100, 2);
                    }
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}