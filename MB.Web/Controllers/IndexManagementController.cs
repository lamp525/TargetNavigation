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
    public class IndexManagementController : BaseController
    {
        //
        // GET: /IndexManagement/

        #region 变量区域

        private IIndexManagementBLL IndexManagementBLL { get; set; }
 

        /// <summary>首页设置BLL对象</summary>
        //private IndexManagementBLL indexBLL = new IndexManagementBLL();

        #endregion 变量区域

        public ActionResult IndexManagement()
        {
            return View();
        }

        /// <summary>
        /// 获取首页模块列表
        /// </summary>
        /// <returns></returns>
        public string GetModuleList()
        {
            // 获取首页模块列表
            var moduleList = IndexManagementBLL.GetModuleList();

            var jsonResult = new JsonResultModel(JsonResultType.success, moduleList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取模块详情
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public string GetModuleInfo(int moduleId)
        {
            // 根据模块ID，获取模块详情
            var moduleModel = IndexManagementBLL.GetModuleInfo(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, moduleModel, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取新闻来源
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public string GetIndexNews(int moduleId)
        {
            // 根据模块ID，获取新闻模块来源
            var newsList = IndexManagementBLL.GetIndexNews(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, newsList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取通知来源
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public string GetIndexNotice(int moduleId)
        {
            // 根据模块ID，获取通知模块来源
            var noticeList = IndexManagementBLL.GetIndexNotice(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, noticeList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取文档来源
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public string GetIndexDocument(int moduleId)
        {
            // 根据模块ID，获取文档模块来源
            var docList = IndexManagementBLL.GetIndexDocument(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, docList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取轮播图片
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public string GetIndexImage(int moduleId)
        {
            // 根据模块ID，获取文档模块来源
            var imgList = IndexManagementBLL.GetIndexImage(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, imgList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取统计对象
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public string GetIndexStatistics(int moduleId)
        {
            // 根据模块ID，获取统计模块统计对象
            var orgList = IndexManagementBLL.GetIndexStatistics(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, orgList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 创建/编辑模块信息
        /// </summary>
        /// <returns></returns>
        public string SaveIndexModule()
        {
            // 登陆用户ID
            var userId = Session["userId"];

            if (userId == null)
            {
                return AjaxCallBack.FAIL;
            }

            // 模板信息
            var moduleJson = Request.Form["data"];

            if (moduleJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var moduleInfoModel = JsonConvert.DeserializeObject<IndexModuleInfoModel>(moduleJson);

            // 模块ID
            // 模板ID
            var moduleId = moduleInfoModel.module.moduleId;

            // 模块ID为空的场合，创建模块
            if (moduleId == null)
            {
                IndexManagementBLL.AddIndexModuleInfo(moduleInfoModel, Convert.ToInt32(userId));
            }
            else
            {
                // 更新模块信息
                IndexManagementBLL.UpdateIndexModuleInfo(moduleInfoModel, Convert.ToInt32(userId));
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public string DeleteModule(int moduleId)
        {
            // 删除模块
            IndexManagementBLL.DeleteModule(moduleId);

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 更新模块大小
        /// </summary>
        /// <returns></returns>
        public string SaveModuleSize()
        {
            // 登陆用户ID
            var userId = Session["userId"];

            if (userId == null)
            {
                return AjaxCallBack.FAIL;
            }

            // 模板信息
            var moduleJson = Request.Form["data"];

            if (moduleJson == null)
            {
                return AjaxCallBack.FAIL;
            }

            var moduleModel = JsonConvert.DeserializeObject<List<IndexModuleModel>>(moduleJson);

            foreach (var module in moduleModel)
            {
                // 更新模块大小
                IndexManagementBLL.UpdateModuleSize(module, Convert.ToInt32(userId));
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除首页图像
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="imageId">图片ID</param>
        /// <returns></returns>
        public string DeleteIndexImage(int moduleId, int imageId)
        {
            // 删除首页图像
            IndexManagementBLL.DeleteIndexImage(moduleId, imageId);

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}