using MB.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class TemplateController : BaseController
    {
        //
        // GET: /Template/
        private ITemplateBLL TemplateBLL { get; set; }

  

        public ActionResult Index()
        {
            return View();
        }

        //返回新建视图
        public ActionResult GetBuild()
        {
            return View("~/Views/FlowShare/formBuild.cshtml");
        }

        //返回复制视图
        public ActionResult GetCoyp()
        {
            return View("~/Views/FlowShare/formCopy.cshtml");
        }

        //返回移动视图
        public ActionResult GetMove()
        {
            return View("~/Views/FlowShare/formMove.cshtml");
        }

        /// <summary>
        /// 获取表单列表
        /// </summary>
        /// <param name="cateoryId"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public string GetTemplateList(int? cateoryId, bool system)
        {
            var userId = Session["userId"];
            var tempList = new List<TemplateManageModel>();
            tempList = TemplateBLL.GetTemplateList(cateoryId, system);
            var jsonResult = new JsonResultModel(JsonResultType.success, tempList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public string GetTemplateCaregoryList(bool system)
        {
            var temcaregoryList = new List<TemplateCategoryModel>();
            temcaregoryList = TemplateBLL.GetTemplateCategoryList(system);
            var jsonResult = new JsonResultModel(JsonResultType.success, temcaregoryList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 表单复制
        /// </summary>
        /// <param name="tempid"></param>
        /// <param name="caregoryid"></param>
        /// <returns></returns>
        public string CopyTemplate()
        {
            var flag = false;
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<TemplateCopyModel>(copyDataJson);
            foreach (var item in copyDataModel.templateId)
            {
                flag = TemplateBLL.CopyTemplate(item, copyDataModel.caregoryid, copyDataModel.system);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 模糊查询表单分类
        /// </summary>
        /// <returns></returns>
        public string selectCaregoryList()
        {
            var text = Request.Form["text"].ToString();
            var careList = TemplateBLL.GetSelectCategoryList(text);
            var jsonResult = new JsonResultModel(JsonResultType.success, careList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="tempid"></param>
        /// <returns></returns>
        public string DeleteTemp()
        {
            var flag = 1;
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<TemplateCopyModel>(copyDataJson);
            flag = TemplateBLL.DeleteTem(copyDataModel.templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 启用或停用表单
        /// </summary>
        /// <returns></returns>
        public string Stopandstart()
        {
            var copyDataJson = Request.Form["templateId"];
            var flag = Request.Form["flag"];
            bool f = TemplateBLL.StopOrStartTem(int.Parse(copyDataJson), int.Parse(flag));
            var jsonResult = new JsonResultModel(JsonResultType.success, f, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 表单移动
        /// </summary>
        /// <returns></returns>
        public string MoveTemplate()
        {
            var moveDataJson = Request.Form["data"];
            var moveDataModel = JsonConvert.DeserializeObject<TemplateMoveModel>(moveDataJson);

            var flag = false;
            flag = TemplateBLL.MoveTem(moveDataModel.templateIds, moveDataModel.caregoryid[0]);

            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 表单新建
        /// </summary>
        /// <returns></returns>
        public string AddNewTemplate()
        {
            var moveDataJson = Request.Form["data"];
            var moveDataModel = JsonConvert.DeserializeObject<TemplateManageModel>(moveDataJson);
            int flag = TemplateBLL.AddTemplate(moveDataModel);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}