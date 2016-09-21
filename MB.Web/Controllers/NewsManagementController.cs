using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class NewsManagementController : BaseController
    {
        private INewsManagementBLL NewsManagementBLL { get; set; }
        private ITagManagementBLL TagManagementBLL { get; set; }

 

        public ActionResult NewsManagement()
        {
            ViewBag.num = int.Parse(ConfigurationManager.AppSettings["pageNum"].ToString());
            ViewBag.author = NewsManagementBLL.author(int.Parse(Session["userId"].ToString()));
            return View();
        }

        //返回新建分类视图
        public ActionResult GetAddDir()
        {
            return View("~/Views/NewsManagement/_buildingAddDirectory.cshtml");
        }

        #region 发布管理

        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="currentPage">手动输入页码</param>
        /// <param name="dirId">分类Id</param>
        /// <returns>返回JSON</returns>
        public string GetNewsList(int currentPage, string dirId)
        {
            var newsInfo = new List<NewsinfoModel>();
            int directoryId = int.Parse(dirId);
            newsInfo = NewsManagementBLL.GetNewsList(currentPage, directoryId);
            var jsonResult = new JsonResultModel(JsonResultType.success, newsInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取通知列表
        /// </summary>
        /// <param name="dirId">分类ID</param>
        /// <returns></returns>
        public string GetNoticeList(int currentPage, string dirId)
        {
            int directoryId = int.Parse(dirId);
            var noticeList = new List<NewsinfoModel>();
            noticeList = NewsManagementBLL.GetNoticeList(currentPage, directoryId);
            var jsonResult = new JsonResultModel(JsonResultType.success, noticeList, "正常");

            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="newId">新闻ID</param>
        /// <returns></returns>
        public string DeleteNews()
        {
            var userId = Session["userId"];
            if (userId == null)
            {
                return AjaxCallBack.FAIL;
            }

            var conditionJson = Request.Form["data"];
            var newId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            if (newId.Count() > 0)
            {
                NewsManagementBLL.DeleteNews(newId);

                //TODO: 新闻标签
                //移除新闻标签
                TagManagementBLL.RemoveNewsTag(Convert.ToInt32(userId), newId);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量发布
        /// </summary>
        /// <returns></returns>
        public string PublishNews()
        {
            var conditionJson = Request.Form["data"];
            var newId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            if (newId.Count() > 0)
            {
                NewsManagementBLL.PublishNews(newId, userId);

                //TODO: 新闻标签
                //保存新闻标签
                foreach (var id in newId)
                {
                    TagManagementBLL.SaveNewsTag(userId, id);
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量取消发布
        /// </summary>
        /// <returns></returns>
        public string UnPublishNews()
        {
            var conditionJson = Request.Form["data"];
            var newId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            if (newId.Count() > 0)
            {
                NewsManagementBLL.UnPublishNews(newId, userId);

                //TODO: 新闻标签
                //移除新闻标签
                TagManagementBLL.RemoveNewsTag(userId, newId);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <returns></returns>
        public string SetTopNews()
        {
            var conditionJson = Request.Form["data"];
            var newId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            if (newId.Count() > 0)
            {
                NewsManagementBLL.SetTopNews(newId, userId);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 批量取消置顶
        /// </summary>
        /// <returns></returns>
        public string SetUnTopNews()
        {
            var conditionJson = Request.Form["data"];
            var newId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            if (newId.Count() > 0)
            {
                NewsManagementBLL.SetUnTopNews(newId, userId);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 发布管理

        #region 类别管理

        /// <summary>
        /// 获取新闻分类列表
        /// </summary>
        /// <param name="parentdir"></param>
        /// <returns></returns>
        public string GetNewsTypeList(int? id)
        {
            var newsInfo = NewsManagementBLL.GetNewsTypeList(id);
            var jsonResult = new JsonResultModel(JsonResultType.success, newsInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取通知分类列表
        /// </summary>
        /// <param name="parentdir"></param>
        /// <returns></returns>
        public string GetNoticeTypeList(int? id)
        {
            var newsInfo = NewsManagementBLL.GetNoticeTypeList(id);
            var jsonResult = new JsonResultModel(JsonResultType.success, newsInfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 更新新闻分类排序
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        public string OrderNewsType()
        {
            //try
            //{
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<List<NewsDirModel>>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            NewsManagementBLL.OrderNewsType(conditionModel, userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
            //}
            //catch
            //{
            //    var jsonResult = new JsonResultModel(JsonResultType.success, null, "更新排序不正常");
            //    return JsonConvert.SerializeObject(jsonResult);
            //}
        }

        /// <summary>
        /// 更新通知分类排序
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        public string OrderNoticeType()
        {
            //try
            //{
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<List<NewsDirModel>>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            NewsManagementBLL.OrderNoticeType(conditionModel, userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
            //}
            //catch
            //{
            //    var jsonResult = new JsonResultModel(JsonResultType.success, null, "更新排序不正常");
            //    return JsonConvert.SerializeObject(jsonResult);
            //}
        }

        /// <summary>
        /// 批量删除新闻分类
        /// </summary>
        /// <param name="directoryId">分类ID</param>
        /// <returns></returns>
        public string DeleteNewsType()
        {
            var conditionJson = Request.Form["data"];
            var directoryId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            var newsInfo = new List<NewsDirModel>();
            int num = directoryId.Count();
            foreach (var item in directoryId)
            {
                //根据分类ID查询有无子分类
                newsInfo = NewsManagementBLL.GetNewsTypeList(item);

                if (newsInfo.Count == 0)
                {
                    num--;
                }
                else
                {
                    num++;
                }
            }
            if (num == 0)
            {
                NewsManagementBLL.DeleteNewsType(directoryId);
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "无法删除信息");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        /// <summary>
        /// 批量删除通知分类
        /// </summary>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        public string DeleteNoticeType()
        {
            var conditionJson = Request.Form["data"];
            var directoryId = JsonConvert.DeserializeObject<int[]>(conditionJson);
            var newsInfo = new List<NewsDirModel>();
            int num = directoryId.Count();
            foreach (var item in directoryId)
            {
                //根据分类ID查询有无子分类
                newsInfo = NewsManagementBLL.GetNoticeTypeList(item);

                if (newsInfo.Count == 0)
                {
                    num--;
                }
                else
                {
                    num++;
                }
            }
            if (num == 0)
            {
                NewsManagementBLL.DeleteNoticeType(directoryId);
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "删除不正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        /// <summary>
        /// 新建/更新新闻分类
        /// </summary>
        /// <param name="newsdir"></param>
        /// <returns></returns>
        public string SaveNewsType()
        {
            //try
            //{
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<NewsDirModel>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            NewsManagementBLL.SaveNewsType(conditionModel, userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
            //}
            //catch
            //{
            //    var jsonResult = new JsonResultModel(JsonResultType.success, null, "错误");
            //    return JsonConvert.SerializeObject(jsonResult);
            //}
        }

        /// <summary>
        /// 新建/更新通知分类
        /// </summary>
        /// <param name="newdir"></param>
        /// <returns></returns>
        public string SaveNoticeType()
        {
            //try
            //{
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<NewsDirModel>(conditionJson);
            int userId = int.Parse(Session["userId"].ToString());
            NewsManagementBLL.SaveNoticeType(conditionModel, userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
            //}
            //catch
            //{
            //    var jsonResult = new JsonResultModel(JsonResultType.success, null, "新建/更新不正常");
            //    return JsonConvert.SerializeObject(jsonResult);
            //}
        }

        #endregion 类别管理

        #region 添加新闻

        /// <summary>
        /// 点击发布
        /// </summary>
        /// <param name="newsdir">添加/更新新闻</param>
        /// <param name="newId"></param>
        /// <returns></returns>

        public string publish()
        {
            var conditionJson = Request.Unvalidated.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<NewsinfoModel>(conditionJson);
            var userId = int.Parse(Session["userId"].ToString());
            //状态为已发布
            conditionModel.publish = true;
            var newId = NewsManagementBLL.SaveNews(conditionModel, userId);

            //TODO: 新闻标签
            //保存新闻标签
            TagManagementBLL.SaveNewsTag(userId, newId);

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 点击保存
        /// </summary>
        /// <param name="newsinfo">添加/更新新闻</param>
        /// <param name="newId"></param>
        /// <returns></returns>
        public string Save()
        {
            var conditionJson = Request.Unvalidated.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<NewsinfoModel>(conditionJson);

            var userId = int.Parse(Session["userId"].ToString());
            //发布状态为待发布
            conditionModel.publish = false;
            var newId = NewsManagementBLL.SaveNews(conditionModel, userId);

            //TODO: 文档标签
            //保存新闻标签
            TagManagementBLL.RemoveNewsTag(userId, newId);

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #endregion 添加新闻

        /// <summary>
        /// 获取新闻图片列表
        /// </summary>
        /// <returns></returns>
        public string GetImageList()
        {
            var imgList = NewsManagementBLL.GetImageList();
            var jsonResult = new JsonResultModel(JsonResultType.success, imgList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public string InsertNewsImage()
        {
            //新闻图片
            var uploadType = ConstVar.UploadType.NewsImage;

            var fileModel = new UploadFileModel();
            var imgModel = new ImageInfoModel();

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                FileUpload upload = new FileUpload();
                fileModel = upload.UploadFile(hpf, (int)uploadType);
                imgModel.saveName = fileModel.saveName;
                imgModel.extension = fileModel.extension;
                NewsManagementBLL.AddNewsImage(imgModel);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, fileModel, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="imgId"></param>
        /// <returns></returns>
        public string DeleteImage(int imgId)
        {
            if (imgId != 0)
            {
                NewsManagementBLL.DeleteImage(imgId);
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "不正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        /// <summary>
        /// 新闻详情
        /// </summary>
        /// <param name="newId"></param>
        /// <returns></returns>
        public string GetNewsInfo(int newId)
        {
            if (newId != 0)
            {
                var newinfo = NewsManagementBLL.GetNewsInfo(newId);
                var jsonResult = new JsonResultModel(JsonResultType.success, newinfo, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "不正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
        }
    }
}