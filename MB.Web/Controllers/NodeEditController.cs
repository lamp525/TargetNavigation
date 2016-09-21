using MB.Web.Models;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class NodeEditController : BaseController
    {
        //
        // GET: /NodeEdit/

        #region 变量区域

        /// <summary>节点编辑模版对象</summary>
        private INodeEditBLL NodeEditBLL { get; set; }

        /// <summary>流程共通对象</summary>
        private IFlowCommonBLL FlowCommonBLL { get; set; }
 

        #endregion 变量区域

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string GetNodeInfoList(int templateId)
        {
            var nodeInfoList = FlowCommonBLL.GetNodeInfoList(templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, nodeInfoList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #region Deleted

        ///// <summary>
        ///// 取得流程节点
        ///// </summary>
        ///// <param name="templateId">模版ID</param>
        ///// <returns></returns>
        //public string GetNodeList(int templateId)
        //{
        //    //取得流程节点信息
        //    var flowNodeInfo = flowCommonBLL.GetNodeList(templateId);

        //    var jsonResult = new JsonResultModel(JsonResultType.success, flowNodeInfo, "正常");
        //    return JsonConvert.SerializeObject(jsonResult);
        //}

        ///// <summary>
        ///// 取得节点操作人信息
        ///// </summary>
        ///// <param name="id">节点ID</param>
        ///// <returns></returns>
        //public string GetNodeOperateList(int id)
        //{
        //    //取得节点操作人信息
        //    var nodeOperateInfo = flowCommonBLL.GetTemplateNodeOperateList(id);

        //    var jsonResult = new JsonResultModel(JsonResultType.success, nodeOperateInfo, "正常");

        //    return JsonConvert.SerializeObject(jsonResult);
        //}

        #endregion Deleted

        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <param name="templateId">节点ID</param>
        /// <returns></returns>
        public string GetNodeFieldList(int? nodeId, int templateId)
        {
            //取得节点字段信息
            var nodeFieldInfo = NodeEditBLL.GetNodeFieldList(nodeId, templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, nodeFieldInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 检查节点的删除标志
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        public string CheckDeleteFlag(int nodeId)
        {
            var deleteFlag = NodeEditBLL.CheckDeleteFlag(nodeId);
            var jsonResult = new JsonResultModel(JsonResultType.success, deleteFlag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 保存节点设置
        /// </summary>
        /// <param name="templateId">模版ID</param>
        /// <returns></returns>
        public string SaveFlowNode(int templateId)
        {
            // 节点信息
            var nodeJson = Request.Form["data"];

            if (nodeJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var nodeEditModel = JsonConvert.DeserializeObject<NodeEditModel>(nodeJson);

            //检查节点设置
            var message = string.Empty;
            if (!FlowCommonBLL.CheckNode(nodeEditModel.nodeInfo, out message))
            {
                var jsonResult = new JsonResultModel(JsonResultType.error, null, message);
                return JsonConvert.SerializeObject(jsonResult);
            }

            //保存节点设置
            NodeEditBLL.SaveFlowNode(nodeEditModel.nodeInfo, nodeEditModel.deleteNode, nodeEditModel.deleteOperateId);

            //判断模版测试标志
            var testFlag = FlowCommonBLL.CheckTemplateTestStatus(templateId);
            //更新模版测试标志
            FlowCommonBLL.UpdateTemplateTestFlag(templateId, testFlag);

            var result = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(result);
        }
    }
}