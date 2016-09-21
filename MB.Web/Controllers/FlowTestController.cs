using MB.Web.Models;
using System.Web.Mvc;
using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class FlowTestController : BaseController
    {
        //
        // GET: /FlowTest/

        #region 变量区域

        //private FlowTestBLL flowTest = new FlowTestBLL();
        private IFlowTestBLL FlowTestBLL { get; set; }

      

        #endregion 变量区域

        public ActionResult Index()
        {
            return View();
        }

        #region 取得创建用户的岗位信息

        /// <summary>
        /// 取得创建用户的岗位信息
        /// </summary>
        /// <param name="userId">创建用户ID</param>
        /// <param name="orgId">创建用户部门ID</param>
        /// <returns></returns>
        public string GetCreateUserStationList(int userId, int orgId)
        {
            var stationList = FlowTestBLL.GetCreateUserStationList(userId, orgId);
            var jsonResult = new JsonResultModel(JsonResultType.success, stationList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 取得创建用户的岗位信息

        #region 取得流程的有效节点信息

        /// <summary>
        /// 取得流程的有效节点信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string GetValidNodeInfoList(int templateId)
        {
            var nodeList = FlowTestBLL.GetValidNodeInfoList(templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, nodeList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 取得流程的有效节点信息

        #region 显示模版流程图信息

        /// <summary>
        /// 显示模版流程图信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string DisplayFlowChart(int templateId)
        {
            var templateFlowChartInfo = FlowTestBLL.GetTemplateFlowChartInfo(templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, templateFlowChartInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 显示模版流程图信息

        #region 流程测试结束处理

        /// <summary>
        /// 流程测试结束处理
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string StopFlowTest(int templateId)
        {
            var isPassTest = FlowTestBLL.GetFlowTestFlag(templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, isPassTest, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 流程测试结束处理

        #region 表单操作

        /// <summary>
        /// 提交新建的表单
        /// </summary>
        /// <returns></returns>
        public string SubmitNewForm()
        {
            var data = Request.Form["data"];
            var formModel = JsonConvert.DeserializeObject<AddFormContentModel>(data);

            //判断表单创建者是否有效
            if (!FlowTestBLL.CheckFormCreateUser(formModel.createUser, formModel.currentNode))
            {
                var checkResult = new JsonResultModel(JsonResultType.error, null, "选择的用户没有权限创建该表单！");
                return JsonConvert.SerializeObject(checkResult);
            }

            var result = FlowTestBLL.FormOperate(formModel, ConstVar.LinkStatus.Pass);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 提交流程中的表单
        /// </summary>
        /// <returns></returns>
        public string SubmitFlowForm()
        {
            var data = Request.Form["data"];
            var formModel = JsonConvert.DeserializeObject<AddFormContentModel>(data);
            var result = FlowTestBLL.FormOperate(formModel, ConstVar.LinkStatus.Pass);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 同意操作
        /// </summary>
        /// <returns></returns>
        public string AgreeFlowForm()
        {
            var data = Request.Form["data"];
            var formModel = JsonConvert.DeserializeObject<AddFormContentModel>(data);
            var result = FlowTestBLL.FormOperate(formModel, ConstVar.LinkStatus.Pass);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 退回操作
        /// </summary>
        /// <returns></returns>
        public string SendBackFlowForm()
        {
            var data = Request.Form["data"];
            var formModel = JsonConvert.DeserializeObject<AddFormContentModel>(data);
            var result = FlowTestBLL.FormOperate(formModel, ConstVar.LinkStatus.Deny);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 表单操作
    }
}