using MB.Web.Models;
using System;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class BankInfoManageController : BaseController
    {
        private IBankCardBLL BankCardBLL { get; set; }
        private IBankBLL BankBLL { get; set; }

        //
        // GET: /bankInfoManage/

        public ActionResult bankInfoManage()
        {
            return View();
        }

        /// <summary>
        /// 获取银行列表
        /// </summary>
        /// <returns></returns>
        public string GetBank()
        {
            var List = BankBLL.GetBankList();
            var jsonResult = new JsonResultModel(JsonResultType.success, List, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取银行卡列表
        /// </summary>
        /// <returns></returns>
        public string GetBankCardList()
        {
            var List = BankCardBLL.GetBankCardList();
            var jsonResult = new JsonResultModel(JsonResultType.success, List, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新增银行卡
        /// </summary>
        /// <returns></returns>
        public string AddNewBankCard(string cardId, string bankid)
        {
            BankCardModel model = new BankCardModel();
            model.cardId = cardId;
            model.bankId = Convert.ToInt32(bankid);
            var flag = BankCardBLL.AddNewBankCard(model);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除银行卡
        /// </summary>
        /// <returns></returns>
        public string deleteBankCard(string id)
        {
            BankCardBLL.DeleteBankCard(id);
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 更新银行卡
        /// </summary>
        /// <returns></returns>
        public string updateBankCard(string cardId, string bankId, string updateCardId)
        {
            BankCardModel model = new BankCardModel();
            model.cardId = cardId;
            model.bankId = Convert.ToInt32(bankId);
            model.updateCardId = updateCardId;
            var flag = BankCardBLL.Update(model);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}