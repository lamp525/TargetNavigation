using MB.Web.Models;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class IncentiveIndexController : BaseController
    {
        private IIncentiveIndexBLL IncentiveIndexBLL { get; set; }

  

        //IncentiveIndexBLL incentiveIndexBll = new IncentiveIndexBLL();

        public ActionResult IncentiveIndex()
        {
            return View();
        }

        //获取图表数据
        public string GetIncentiveData(int type, int year, int? month, int? userId)
        {
            if (userId == null)
            {
                userId = int.Parse(Session["userId"].ToString());
            }
            var list = IncentiveIndexBLL.GetIncentiveData(type, year, month, userId.Value);
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取每月奖惩数
        /// </summary>
        /// <param name="year"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetRewardPunishNum(int? year, int? userId)
        {
            if (userId == null)
            {
                userId = int.Parse(Session["userId"].ToString());
            }
            var list = IncentiveIndexBLL.GetRewardPunishNum(year, userId.Value);
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取激励详细情况
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetRewardPunishDetail(int? userId, int? year, int? month)
        {
            if (userId == null)
            {
                userId = int.Parse(Session["userId"].ToString());
            }
            var model = IncentiveIndexBLL.GetRewardPunishDetail(year.Value, month, userId.Value);
            var jsonResult = new JsonResultModel(JsonResultType.success, model, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 下属奖励列表
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUnderReward(int? parent, int? year, int? month)
        {
            if (parent == null)
            {
                parent = int.Parse(Session["userId"].ToString());
            }
            var list = IncentiveIndexBLL.GetUnderReward(year, month, parent);
            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}