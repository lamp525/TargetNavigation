using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class UserDocumentController : BaseController
    {
        //
        // GET: /UserDocument/

        #region 变量区域

        private IUserDocumentBLL UserDocumentBLL { get; set; }
        private ISharedBLL SharedBLL { get; set; }
        private ITagManagementBLL TagManagementBLL { get; set; }

     
        #endregion 变量区域

        public ActionResult UserDocument()
        {
            //PlanBLL planBLL = new PlanBLL();
            //if (Session["userId"].ToString() != null)
            //{
            //    var userId = Convert.ToInt32(Session["userId"]);
            //    //获取登录用户信息
            //    var userInfo = planBLL.GetUserInfoById(userId);
            //    //统计用户今日未完成和超时计划的数量
            //    var userPlanCount = planBLL.StatisticsUserPlan(userId);
            //    var admin = new SharedBLL().GetAdmin(userId);
            //    ViewBag.Admin = admin;
            //    ViewBag.UserInfo = userInfo;
            //    ViewBag.UserPlanCount = userPlanCount;
            //}
            //else
            //{
            //    return Redirect(" ../Views/Login/Login.cshtml");
            //}
            ////GetUserInfoById();
            //GetOutTimePlanNUmById();
            //ViewBag.Type = true;
            return View();
        }

        //获取文档列表
        public string GetDocumentList()
        {
            var userId = Session["userId"];
            var docList = new List<DocumentModel>();
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<DocConditionModel>(conditionJson);
            //初始化条件
            var conditionString = " c.deleteFlag='false' ";
            if (userId != null)
            {
                if (conditionModel != null)
                {
                    #region 拼凑sql

                    if (!string.IsNullOrWhiteSpace(conditionModel.docName))
                    {
                        conditionString += " and c.displayName like '%" + conditionModel.docName + "%' ";
                    }

                    if (conditionModel.type != 3 && conditionModel.type != 4)
                    {
                        if (conditionModel.folder != null)
                        {
                            conditionString += " and c.folder=" + conditionModel.folder + " ";
                        }
                        else
                        {
                            conditionString += " and c.folder is null ";  //这种写法有待测试
                        }
                    }
                    if (conditionModel.person.Length > 0)
                    {
                        conditionString += " and c.createUser in ( ";
                        foreach (var item in conditionModel.person)
                        {
                            conditionString += " " + item + ", ";
                        }
                        conditionString = conditionString.Substring(0, conditionString.LastIndexOf(',')) + ")";
                    }
                    //筛选时间
                    if (conditionModel.time.Length > 0)
                    {
                        //近一周
                        if (conditionModel.time[0] == "1")
                        {
                            conditionString += " and c.createTime >=' " + DateTime.Now.AddDays(-6).Date + "' and c.createTime <' " + DateTime.Now.AddDays(1).Date + "' ";
                            //conditionStart = DateTime.Now.AddDays(-6).Date;
                            //conditionEnd = DateTime.Now.AddDays(1).Date;
                        }
                        //近一月
                        else if (conditionModel.time[0] == "2")
                        {
                            conditionString += " and c.createTime >=' " + DateTime.Now.AddMonths(-1).Date + "' and c.createTime <' " + DateTime.Now.AddDays(1).Date + "' ";
                            //conditionStart = DateTime.Now.AddMonths(-1).Date;
                            //conditionEnd = DateTime.Now.AddDays(1).Date;
                        }
                        //自定义
                        else
                        {
                            if (conditionModel.time[2] == "")
                            {
                                conditionString += " and c.createTime >=' " + Convert.ToDateTime(conditionModel.time[1]).Date + "' ";
                                //conditionStart = Convert.ToDateTime(conditionModel.time[1]).Date;
                                //conditionEnd = DateTime.MaxValue;
                            }
                            else if (conditionModel.time[1] == "")
                            {
                                conditionString += " and c.createTime <' " + Convert.ToDateTime(conditionModel.time[2]).Date + "' ";
                                //conditionStart = DateTime.MinValue;
                                //conditionEnd = Convert.ToDateTime(conditionModel.time[2]).Date;
                            }
                            else
                            {
                                conditionString += " and c.createTime >=' " + Convert.ToDateTime(conditionModel.time[1]) + "' and c.createTime <' " + Convert.ToDateTime(conditionModel.time[2]).AddDays(1).Date + "' ";
                                //conditionStart = Convert.ToDateTime(conditionModel.time[1]);
                                //conditionEnd = Convert.ToDateTime(conditionModel.time[2]).AddDays(1).Date;
                            }
                        }
                    }

                    #endregion 拼凑sql
                }
                //获取公司文档列表
                if (conditionModel.type == 1)
                {
                    docList = UserDocumentBLL.GetCompanyDocumentList(conditionString, conditionModel.sorts, Convert.ToInt32(userId));
                }
                //获取我的文档列表
                else if (conditionModel.type == 2)
                {
                    docList = UserDocumentBLL.GetUserDocumentList(conditionString, conditionModel.type, conditionModel.sorts, Convert.ToInt32(userId));
                }
                //获取我的共享列表
                else if (conditionModel.type == 3)
                {
                    docList = UserDocumentBLL.GetUserDocumentList(conditionString, conditionModel.type, conditionModel.sorts, Convert.ToInt32(userId));
                }
                //获取他人共享列表
                else if (conditionModel.type == 4)
                {
                    docList = UserDocumentBLL.GetUserDocumentList(conditionString, conditionModel.type, conditionModel.sorts, Convert.ToInt32(userId));
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, docList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取饼图统计信息
        public string GetDocumentStatisticsInfo()
        {
            var userId = Session["userId"];
            var statisticList = new List<DocumentStatisticsModel>();
            if (userId != null)
            {
                statisticList = UserDocumentBLL.GetDocumentStatisticsInfo(Convert.ToInt32(userId));
            }
            foreach (var item in statisticList)
            {
                switch (item.id)
                {
                    case 1:
                        item.text = "公司文档";
                        break;

                    case 2:
                        item.text = "我的文档";
                        break;

                    case 3:
                        item.text = "我的共享";
                        break;

                    case 4:
                        item.text = "他人共享";
                        break;
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, statisticList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //删除 flag:1、公司文档 2、用户文档
        public string DeleteFlagDocument()
        {
            var flag = false;

            var userId = Session["userId"];
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<DocDeleteModel>(conditionJson);
            if (userId != null)
            {
                flag = SharedBLL.DeleteFlagDocument(conditionModel.documentIds, Convert.ToInt32(userId), conditionModel.flag);

                //TODO: 文档标签
                //删除文档标签
                //公司文档
                if (conditionModel.flag == 1)
                {
                    TagManagementBLL.RemoveCompanyDocumentTag(Convert.ToInt32(userId), conditionModel.documentIds);
                }
                //用户文档
                else
                {
                    TagManagementBLL.RemoveUserDocumentTag(Convert.ToInt32(userId), conditionModel.documentIds);
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //单文件下载:flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult Download(int documentId, string displayName, string saveName, bool isFolder, int flag, int type)
        {
            SharedBLL sharedBll = new SharedBLL();
            var userId = Convert.ToInt32(Session["userId"]);
            var path = displayName;
            // 0：公司文档 1：个人文档
            var uploadPath = type == 0 ? FilePath.DocumentUpLoadPath : FilePath.MineUpLoadPath;
            //var downLoadPath = Path.Combine(FilePath.DocumentUpLoadPath, saveName);
            //if (!System.IO.File.Exists(downLoadPath))
            //{
            //    return JavaScript("noFile();");
            //}
            //if (flag == 0)
            //{
            //    return Content("success");
            //}
            //return File(new FileStream(downLoadPath, FileMode.Open), "application/octet-stream", Server.UrlEncode(displayName));
            if (isFolder)
            {
                var lstFile = new List<CompressInfo>();
                var docList = new List<DocumentModel>();
                if (type == 0)
                {
                    docList = sharedBll.GetAllDocumentInFolder(documentId, int.Parse(Session["userId"].ToString()), docList, ref path);
                }
                else
                {
                    docList = SharedBLL.GetUserDocumentInFolder(documentId, docList, ref path);
                }
                if (docList.Count <= 0)
                {
                    return JavaScript("noFile();");
                }
                if (flag == 0)
                {
                    return Content("success");
                }
                var fileName = displayName;
                foreach (var item in docList)
                {
                    if (item.saveName != null)
                    {
                        var downLoadPath = Path.Combine(uploadPath, item.saveName);
                        lstFile.Add(new CompressInfo { path = downLoadPath, display = item.displayName, folder = item.path });
                    }
                }
                if (type == 0)
                {
                    sharedBll.AddDownloadLog(documentId, 1, string.Empty, userId);
                }
                SharpZipLibrary sharpZip = new SharpZipLibrary();
                return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}.zip", fileName), System.Text.Encoding.UTF8));
            }
            else
            {
                var downLoadPath = Path.Combine(uploadPath, saveName);
                if (!System.IO.File.Exists(downLoadPath))
                {
                    return JavaScript("noFile();");
                }
                if (flag == 0)
                {
                    return Content("success");
                }
                if (type == 0) //加日志
                {
                    sharedBll.AddDownloadLog(documentId, 1, string.Empty, userId);
                }
                return File(downLoadPath, "application/octet-stream", HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8));
            }
        }

        //批量下载 flag  1、进行下载  0、不下载检测文件是否存在   type:1、公司文档 2、用户文档
        public ActionResult MultiDownload(int flag, int type)
        {
            SharedBLL sharedBll = new SharedBLL();
            var userId = Convert.ToInt32(Session["userId"]);

            var docList = new List<DocumentModel>();
            var lstFile = new List<CompressInfo>();
            int[] documentIds;
            // 0：公司文档 1：个人文档
            var uploadPath = type == 0 ? FilePath.DocumentUpLoadPath : FilePath.MineUpLoadPath;

            if (flag == 0)
            {
                if (type == 0)
                {
                    documentIds = JsonConvert.DeserializeObject<int[]>(Request.Form["data"]);
                    if (documentIds.Length > 0)
                    {
                        //记录日志
                        foreach (var item in documentIds)
                        {
                            sharedBll.AddDownloadLog(item, 1, string.Empty, userId);
                        }
                    }
                    TempData["documentIds"] = documentIds;
                    docList = sharedBll.GetDocumentListByIds(documentIds, int.Parse(Session["userId"].ToString()));
                }
                else
                {
                    documentIds = JsonConvert.DeserializeObject<int[]>(Request.Form["data"]);
                    TempData["documentIds"] = documentIds;
                    docList = SharedBLL.GetUserDocumentListByIds(documentIds);
                }
            }
            else
            {
                if (type == 0)
                {
                    docList = sharedBll.GetDocumentListByIds(TempData["documentIds"] as int[], int.Parse(Session["userId"].ToString()));
                }
                else
                {
                    docList = SharedBLL.GetUserDocumentListByIds(TempData["documentIds"] as int[]);
                }
            }

            bool fileExists = false;
            string fileName = null;

            foreach (var item in docList)
            {
                if (item.saveName != null)
                {
                    var downLoadPath = Path.Combine(uploadPath, item.saveName);
                    if (!fileExists && System.IO.File.Exists(downLoadPath))
                    {
                        fileName = item.displayName;
                        fileExists = true;
                    }
                    lstFile.Add(new CompressInfo { path = downLoadPath, display = item.displayName, folder = item.path });
                }
            }
            if (!fileExists)
            {
                return Content("error");
            }
            else if (fileExists && flag == 0)
            {
                return Content("success");
            }
            SharpZipLibrary sharpZip = new SharpZipLibrary();
            //return File(sharpZip.Compress(lstFile), "application/octet-stream", Server.UrlEncode(string.Format("{0}等.zip", fileName)));
            return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}等.zip", fileName), System.Text.Encoding.UTF8));
        }

        //上传用户文档
        [HttpPost]
        public string InsertUserDocument(int type)
        {
            // 1：公司文档 2：个人文档
            var uploadType = type == 1 ? ConstVar.UploadType.CompanyDocument : ConstVar.UploadType.UserDocument;
            var fileModel = new UploadFileModel();
            try
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                    FileUpload upload = new FileUpload();
                    fileModel = upload.UploadFile(hpf, (int)uploadType);
                }
                var jsonResult = new JsonResultModel(JsonResultType.success, fileModel, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }
            catch
            {
                return AjaxCallBack.FAIL;
            }
        }

        /// <summary>
        /// 根据附件ID删除计划附件
        /// </summary>
        /// <param name="id">附件ID</param>
        [HttpPost]
        public void DeleteFile(string savename, int type)
        {
            UserDocumentBLL.DeleteFile(savename, type);
        }

        //检查是否要创建上传文件夹，如果没有就创建
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    //TODO：处理异常
                    result = false;
                }
            }
            return result;
        }

        private void GetOutTimePlanNUmById()
        {
            UserPlanCountInfo userPlanCount = new UserPlanCountInfo();
            PlanBLL planBLL = new PlanBLL();
            if (Session["userId"].ToString() != null)
            {
                userPlanCount = planBLL.StatisticsUserPlan(int.Parse(Session["userId"].ToString()));
            }
            ViewBag.OutTimePlan = userPlanCount;
        }

        //获取人员列表
        public string GetLastFiveCreateUser()
        {
            SharedBLL sharedBll = new SharedBLL();
            var data = Request.Form["text"];
            var userList = new List<UserInfo>();
            if (string.IsNullOrWhiteSpace(data))   //查询最近5个上传文档的人员
            {
                userList = sharedBll.GetLastFiveCreateUser();
            }
            else              //模糊查询
            {
                userList = sharedBll.SelectUserList(data);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}