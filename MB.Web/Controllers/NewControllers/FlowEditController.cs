using MB.Facade.Flow;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class FlowEditController : BaseController
    {
        private IFlowEditFacade facade { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        #region 节点设置

        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public string GetNodeInfo(int templateId)
        {
            var result = facade.GetNodeInfo(templateId);

            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public string GetNodeFieldInfo(int templateId, int? nodeId)
        {
            var result = facade.GetNodeFieldInfo(templateId, nodeId);

            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 检查节点的删除标志
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        public string CheckDeleteFlag(int nodeId)
        {
            var result = facade.CheckDeleteFlag(nodeId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 保存节点设置
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string SaveFlowNode()
        {
            var info = JsonConvert.DeserializeObject<PageNodeEditModel>(Request.Form["data"]);
            var result = facade.SaveFlowNode(info);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "流程设置保存成功！");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 节点设置

        #region 流程设置

        /// <summary>
        /// 取得流程节点信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string GetFlowNodeInfo(int templateId)
        {
            var result = facade.GetFlowNodeInfo(templateId);

            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得节点流程信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string GetNodeLinkInfo(int templateId)
        {
            var result = facade.GetNodeLinkInfo(templateId);

            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得流程条件信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <param name="linkId">节点出口ID</param>
        /// <returns></returns>
        public string GetLinkConditionRelatedInfo(int templateId, int? linkId)
        {
            var result = facade.GetLinkConditionRelatedInfo(templateId, linkId);

            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 保存流程设置
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string SaveNodeLink(int templateId)
        {
            var info = JsonConvert.DeserializeObject<PageFlowEditModel>(Request.Form["data"]);
            var result = facade.SaveNodeLink(info);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "流程设置保存成功！");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 流程设置
    }
}