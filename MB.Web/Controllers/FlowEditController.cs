using MB.Web.Models;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class FlowEditController : BaseController
    {
        //
        // GET: /FlowEdit/

        #region 变量区域

        private IFlowEditBLL FlowEditBLL { get; set; }
        private IFlowCommonBLL FlowCommonBLL { get; set; }

       

        /// <summary>流程编辑模版对象</summary>
        //private FlowEditBLL flowEditBLL = new FlowEditBLL();

        /// <summary>流程共通对象</summary>
        //private FlowCommonBLL flowCommonBLL = new FlowCommonBLL();

        #endregion 变量区域

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得流程节点
        /// </summary>
        /// <param name="id">模版ID</param>
        /// <returns></returns>
        public string GetNodeList(int id)
        {
            //取得流程节点信息
            var flowNodeInfo = FlowCommonBLL.GetNodeList(id);

            var jsonResult = new JsonResultModel(JsonResultType.success, flowNodeInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得节点流程
        /// </summary>
        /// <param name="id">模版ID</param>
        /// <returns></returns>
        public string GetNodeLinkList(int id)
        {
            //取得节点流程信息
            var linkInfo = FlowCommonBLL.GetNodeLinkList(id);

            var jsonResult = new JsonResultModel(JsonResultType.success, linkInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得流程条件信息
        /// </summary>
        /// <param name="linkId">节点出口ID</param>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string GetLinkConditionRelatedInfo(int? linkId, int templateId)
        {
            //取得流程条件信息
            var conditionInfo = FlowCommonBLL.GetLinkConditionRelatedInfo(linkId, templateId);

            var jsonResult = new JsonResultModel(JsonResultType.success, conditionInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 保存流程设置
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string SaveNodeLink(int templateId)
        {
            //流程编辑信息
            var linkJson = Request.Form["data"];

            if (linkJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var linkEditModel = JsonConvert.DeserializeObject<FlowEditModel>(linkJson);

            //保存流程设置
            FlowEditBLL.SaveNodeLink(linkEditModel.nodeLinkInfoList, linkEditModel.deleteLinkId, linkEditModel.deleteCondition);

            //判断模版测试标志
            var testFlag = FlowCommonBLL.CheckTemplateTestStatus(templateId);
            //更新模版测试标志
            FlowCommonBLL.UpdateTemplateTestFlag(templateId, testFlag);

            var result = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(result);
        }
    }
}