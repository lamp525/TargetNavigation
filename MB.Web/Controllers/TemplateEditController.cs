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
    public class TemplateEditController : BaseController
    {
        //
        // GET: /TemplateEdit/

        #region 变量区域

        /// <summary>模板编辑BLL对象</summary>
        private ITemplateEditBLL TemplateEditBLL { get; set; }

        /// <summary>流程共通对象</summary>
        private IFlowCommonBLL FlowCommonBLL { get; set; }

 

        #endregion 变量区域

        //public ActionResult TemplateEdit(int templateId)
        //{
        //    ViewBag.TemplateId = templateId;
        //    return View();
        //}

        public ActionResult TemplateEdit(int templateId, int jumpSign, int systemOrFalse)
        {
            ViewBag.TemplateId = templateId;
            ViewBag.JumpSign = jumpSign;
            ViewBag.SystemOrFalse = systemOrFalse;
            return View();
        }

        /// <summary>
        /// 获取模板详情
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        public string GetTemplateInfo(int id)
        {
            // 获取模板详情
            var templateInfo = TemplateEditBLL.GetTemplateInfoById(id);

            var jsonResult = new JsonResultModel(JsonResultType.success, templateInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取控件详情
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <returns></returns>
        public string GetTemplateControlInfo(int templateId, string controlId)
        {
            // 获取控件详情
            var controlInfo = TemplateEditBLL.GetControlInfoById(templateId, controlId);

            var jsonResult = new JsonResultModel(JsonResultType.success, controlInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 创建/编辑模板信息
        /// </summary>
        /// <returns></returns>
        public string SaveTemplate()
        {
            // 登陆用户ID
            var userId = Session["userId"];

            if (userId == null)
            {
                return AjaxCallBack.FAIL;
            }

            // 模板信息
            var templateJson = Request.Form["data"];

            if (templateJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var templateInfoModel = JsonConvert.DeserializeObject<TemplateInfoModel>(templateJson);

            // 模板ID
            var templateId = templateInfoModel.template.templateId;

            // 保存并使用的场合，检查节点设置和流程设置
            if (templateInfoModel.template.status == 1)
            {
                // 检查节点设置
                if (!TemplateEditBLL.CheckNode(templateId))
                {
                    var jsonResult = new JsonResultModel(JsonResultType.error, null, "节点设置不正确");
                    return JsonConvert.SerializeObject(jsonResult);
                }

                // 检查流程设置
                if (!TemplateEditBLL.CheckFlow(templateId))
                {
                    var jsonResult = new JsonResultModel(JsonResultType.error, null, "流程设置不正确");
                    return JsonConvert.SerializeObject(jsonResult);
                }
            }

            // 模板ID为空时，创建模板
            if (templateId == null)
            {
                TemplateEditBLL.AddTemplateInfo(templateInfoModel, Convert.ToInt32(userId));
            }
            else
            {
                // 模板ID不为空时，更新模板
                TemplateEditBLL.UpdateTemplateInfo(templateInfoModel, Convert.ToInt32(userId));

                //插入模版新增未设置的控件的字段权限
                FlowCommonBLL.InsertDefaultNodeField(templateId.Value);
            }

            var result = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(result);
        }
    }
}