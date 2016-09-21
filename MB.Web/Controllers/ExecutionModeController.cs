using MB.Web.Models;
using System;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class ExecutionModeController : BaseController
    {
        private IExecutionModeBLL ExecutionModeBLL { get; set; }

       
        //
        // GET: /RunModelManage/

        public ActionResult ExecutionMode()
        {
            return View();
        }

        /// <summary>
        /// 获取执行方式列表
        /// </summary>
        /// <returns></returns>
        public string GetRunModel()
        {
            var HolidayList = ExecutionModeBLL.GetExecutionList();
            var jsonResult = new JsonResultModel(JsonResultType.success, HolidayList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新增执行方式
        /// </summary>
        /// <returns></returns>
        public string AddNewExecutionMode(string executionMode)
        {
            ExecutionMode model = new ExecutionMode();
            model.executionMode = executionMode;
            model.createUser = Convert.ToInt32(Session["userId"]);
            model.updateUser = Convert.ToInt32(Session["userId"]);
            var flag = ExecutionModeBLL.AddNewExecution(model);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除执行方式
        /// </summary>
        /// <returns></returns>
        public string DeleteExecutionMode(string id)
        {
            ExecutionModeBLL.DeleteExection(Convert.ToInt32(id));
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 更新执行方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string UpdateExecutionMode(string id, string model)
        { 
            ExecutionMode UpdateModel = new ExecutionMode();
            UpdateModel.executionId =Convert.ToInt32(id);
            UpdateModel.executionMode = model;
            var flag = ExecutionModeBLL.Update(UpdateModel);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}