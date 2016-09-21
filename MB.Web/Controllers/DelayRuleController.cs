using MB.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class DelayRuleController : BaseController
    {
        private IDelayRuleBLL DelayRuleBLL { get; set; }

       
        //
        // GET: /DelayRule/

        public ActionResult DelayRule()
        {
            return View();
        }

        //DelayRuleBLL delayBll = new DelayRuleBLL();
        public string GetLoginUser()
        {
            string loginId = Session["userId"].ToString();
            var jsonResult = new JsonResultModel(JsonResultType.success, loginId, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取激励
        /// </summary>
        /// <param name="type">1.计划延时2.有效工时</param>
        /// <returns></returns>
        public string GetDelay(int type)
        {
            var List = new List<DelayRuleModel>();
            List = DelayRuleBLL.GetDelayList(type);
            var jsonResult = new JsonResultModel(JsonResultType.success, List, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string GetValueCustList()
        {
            var List = new List<ValueIncentiveCustomModel>();
            List = DelayRuleBLL.GetValueCustList();
            var jsonResult = new JsonResultModel(JsonResultType.success, List, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string GetValueIncentive()
        {
            var List = new ValueIncentiveModel();
            List = DelayRuleBLL.GetValueIncentive();
            var jsonResult = new JsonResultModel(JsonResultType.success, List, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string UpdateAndDelete()
        {
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<DelayRuleAddModel>(copyDataJson);
            if (copyDataModel.DeleteId.Count() > 0)
            {
                DelayRuleBLL.DeleteDelayRule(copyDataModel.DeleteId);
            }
            var Flag = DelayRuleBLL.AddDelayRule(copyDataModel.rule);
            var jsonResult = new JsonResultModel(JsonResultType.success, Flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string ValueUpdateAndDelete()
        {
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<ValueDelayRuleModel>(copyDataJson);
            if (copyDataModel.DeleteId.Count() > 0)
            {
                DelayRuleBLL.DeleteValueCust(copyDataModel.DeleteId);
            }
            if (copyDataModel.value != null)
            {
                DelayRuleBLL.AddOrUpdateValueIncentive(copyDataModel.value);
            }
            if (copyDataModel.custom != null)
            {
                DelayRuleBLL.AddValueCust(copyDataModel.custom);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, true, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}