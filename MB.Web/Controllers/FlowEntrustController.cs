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
    public class FlowEntrustController : BaseController
    {
        //
        // GET: /FlowEntrust/
        private IFlowEntrustBLL FlowEntrustBLL { get; set; }

      

        public ActionResult FlowEntrust()
        {
            return View();
        }

        //FlowEntrustBLL flowBll = new FlowEntrustBLL();
        /// <summary>
        /// 获取委托列表
        /// </summary>
        /// <returns></returns>
        public string GetFlowEntrustList()
        {
            var userId = Session["userId"];
            var docList = new List<FlowEntrustModel>();
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<FlowEntrustModel>(conditionJson);
            string conditionString = " deleteFlag=false ";
            var conditionStart = DateTime.MinValue;
            var conditionEnd = DateTime.MaxValue;

            var conditionstutsStart = DateTime.MinValue;
            var conditionstutsEnd = DateTime.MaxValue;

            if (userId != null)
            {
                if (conditionModel != null)
                {
                    if (conditionModel.status.Length > 0 && conditionModel.status.Length < 2)
                    {
                        if (conditionModel.status[0] == 0)
                        {
                            conditionstutsStart = DateTime.Now;
                        }
                        else if (conditionModel.status[0] == 1)
                        {
                            conditionstutsEnd = DateTime.Now;
                        }
                    }

                    if (conditionModel.person.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionString))
                        {
                            conditionString += " And ";
                        }
                        conditionString += " (";
                        foreach (var item in conditionModel.person)
                        {
                            conditionString += " mandataryUser==" + item + " Or ";
                        }
                        conditionString = conditionString.Substring(0, conditionString.LastIndexOf("Or")) + " ) ";
                    }
                    //筛选时间
                    if (conditionModel.time.Length > 0)
                    {
                        //近一周
                        if (conditionModel.time[0] == "1")
                        {
                            conditionStart = DateTime.Now.AddDays(-6).Date;
                            conditionEnd = DateTime.Now.AddDays(1).Date;
                            //conditionStart = DateTime.Now.AddDays(-6).Date;
                            //conditionEnd = DateTime.Now.AddDays(1).Date;
                        }
                        //近一月
                        else if (conditionModel.time[0] == "2")
                        {
                            conditionStart = DateTime.Now.AddMonths(-1).Date;
                            conditionEnd = DateTime.Now.AddDays(1).Date;
                            //conditionStart = DateTime.Now.AddMonths(-1).Date;
                            //conditionEnd = DateTime.Now.AddDays(1).Date;
                        }
                        //自定义
                        else
                        {
                            if (conditionModel.time[2] == "")
                            {
                                conditionStart = Convert.ToDateTime(conditionModel.time[2]).Date;
                                conditionEnd = DateTime.MaxValue;
                                //conditionStart = Convert.ToDateTime(conditionModel.time[1]).Date;
                                //conditionEnd = DateTime.MaxValue;
                            }
                            else if (conditionModel.time[1] == "")
                            {
                                conditionStart = DateTime.MinValue;
                                conditionEnd = Convert.ToDateTime(conditionModel.time[1]).Date;
                                //conditionStart = DateTime.MinValue;
                                //conditionEnd = Convert.ToDateTime(conditionModel.time[2]).Date;
                            }
                            else
                            {
                                conditionStart = Convert.ToDateTime(conditionModel.time[1]);
                                conditionEnd = Convert.ToDateTime(conditionModel.time[2]).AddDays(1).Date;
                                //conditionStart = Convert.ToDateTime(conditionModel.time[1]);
                                //conditionEnd = Convert.ToDateTime(conditionModel.time[2]).AddDays(1).Date;
                            }
                        }
                    }
                }
            }
            docList = FlowEntrustBLL.GetFlowEntrustList(conditionString, Convert.ToInt32(userId), conditionStart, conditionEnd, conditionstutsStart, conditionstutsEnd);

            var jsonResult = new JsonResultModel(JsonResultType.success, docList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetFlowEbyId(int id)
        {
            var FlowE = FlowEntrustBLL.GetFlowById(id);
            var jsonResult = new JsonResultModel(JsonResultType.success, FlowE, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新增委托
        /// </summary>
        /// <returns></returns>
        public string AddFlowE()
        {
            var moveDataJson = Request.Form["data"];
            var moveDataModel = JsonConvert.DeserializeObject<FlowEntrustModel>(moveDataJson);
            moveDataModel.createUser = Convert.ToInt32(Session["userId"]);
            var flag = FlowEntrustBLL.AddNewFlowE(moveDataModel);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 收回委托
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public string UpdateFlowE(int id)
        {
            var flag = FlowEntrustBLL.UpdateFlowE(id, Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <returns></returns>
        public string GetCaregoryList()
        {
            List<TemplateCategoryModel> temCList = FlowEntrustBLL.GetCategoryModel();
            var jsonResult = new JsonResultModel(JsonResultType.success, temCList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取表单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTemListById(int parent)
        {
            List<TemplateModel> temList = FlowEntrustBLL.GetTemById(parent);
            var jsonResult = new JsonResultModel(JsonResultType.success, temList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}