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
    [UserAuthorize]
    public class TemplateCategoryController : BaseController
    {
        private ITemplateCategoryBLL TemplateCategoryBLL { get; set; }

 
        public ActionResult TemplateCategory()
        {
            return View();
        }

        //TemplateCategoryBLL temCategoryBll = new TemplateCategoryBLL();
        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <returns></returns>
        public string GetTemplateList()
        {
            var userId = Session["userId"];
            var tempList = new List<TemplateCategoryModel>();
            tempList = TemplateCategoryBLL.GetCategoryList();
            var jsonResult = new JsonResultModel(JsonResultType.success, tempList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取分类详情
        /// </summary>
        /// <param name="tempcaregoryId"></param>
        /// <returns></returns>
        public string GetTemplateInfoById(int tempcaregoryId)
        {
            var temInfo = TemplateCategoryBLL.GetCareoryById(tempcaregoryId);
            var jsonResult = new JsonResultModel(JsonResultType.success, temInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新增分类
        /// </summary>
        /// <returns></returns>
        public string AddNewCaregory()
        {
            var flag = false;
            var moveDataJson = Request.Form["data"];
            var moveDataModel = JsonConvert.DeserializeObject<TemplateCategoryModel>(moveDataJson);
            moveDataModel.creatuser = Convert.ToInt32(Session["userId"]);
            if (moveDataModel.categoryId == null)
            {
                flag = TemplateCategoryBLL.AddCareoryById(moveDataModel);
            }
            else
            {
                flag = TemplateCategoryBLL.UpdateCaregoryById(moveDataModel);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <returns></returns>
        public string UpdateCaregory()
        {
            var moveDataJson = Request.Form["data"];
            var moveDataModel = JsonConvert.DeserializeObject<TemplateCategoryModel>(moveDataJson);
            var flag = TemplateCategoryBLL.UpdateCaregoryById(moveDataModel);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="CaregoryId"></param>
        /// <returns></returns>
        public string DeleteCaregoryById(int CaregoryId)
        {
            var flag = TemplateCategoryBLL.DeleteCaregory(CaregoryId);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 移动排序
        /// </summary>
        /// <returns></returns>
        public string UpateOldNum()
        {
            var moveDataJson = Request.Form["data"];
            var moveDataModel = JsonConvert.DeserializeObject<List<TemplateCategoryOlderModel>>(moveDataJson);
            var flag = false;
            TemplateCategoryBLL.UpdateOldNum(moveDataModel);
            flag = true;
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}