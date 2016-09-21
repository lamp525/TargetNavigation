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
    public class DocumentManagementController : BaseController
    {
        #region 变量区域

        private IDocumentManagementBLL DocumentManagementBLL { get; set; }
        private ISharedBLL SharedBLL { get; set; }
        private ITagManagementBLL TagManagementBLL { get; set; }

        //
        // GET: /DocumentManagement/
     

        #endregion 变量区域

        public ActionResult DocumentManagement()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var admin = SharedBLL.GetAdmin(userId);
            ViewBag.Admin = admin;
            return View();
        }

        //返回日志视图
        public ActionResult GetLogList()
        {
            return View("~/Views/DocumentShare/log.cshtml");
        }

        //获取文档列表JSON
        public string GetCompanyDocumentList()
        {
            var docList = new List<DocumentModel>();
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<DocConditionModel>(conditionJson);
            //初始化条件
            var conditionString = " c.deleteFlag='false' ";
            if (conditionModel != null)
            {
                #region 拼凑sql

                if (!string.IsNullOrWhiteSpace(conditionModel.docName))
                {
                    conditionString += " and c.displayName like '%" + conditionModel.docName + "%' ";
                }
                if (conditionModel.folder != null)
                {
                    conditionString += " and c.folder=" + conditionModel.folder + " ";
                }
                else
                {
                    conditionString += " and c.folder is null ";
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
            docList = DocumentManagementBLL.GetCompanyDocumentList(conditionString, conditionModel.sorts);
            var jsonResult = new JsonResultModel(JsonResultType.success, docList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取文件夹的权限信息
        public string GetAuthorityList(int documentId)
        {
            var authorithList = DocumentManagementBLL.GetAuthorityList(documentId);
            var jsonResult = new JsonResultModel(JsonResultType.success, authorithList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //权限设置
        public string SetAuthority()
        {
            var deleteAuthorityIdsJson = Request.Form["deleteAuthorityIds"];
            var deleteAuthorityIds = JsonConvert.DeserializeObject<int[]>(deleteAuthorityIdsJson);
            var authorityJson = Request.Form["data"];
            if (authorityJson != null)
            {
                var authorityInfo = JsonConvert.DeserializeObject<AuthorityInfoModel>(authorityJson);
                var userId = Session["userId"];
                if (userId != null)
                {
                    DocumentManagementBLL.SetAuthority(deleteAuthorityIds, authorityInfo, Convert.ToInt32(userId));
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //拼接部门信息
        public string GetOrgInfoById()
        {
            var data = Request.Form["data"];
            var orgIds = JsonConvert.DeserializeObject<int[]>(data);
            var orgNames = DocumentManagementBLL.GetOrgInfoById(orgIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, orgNames, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //拼接部门信息
        public string GetStationInfoById()
        {
            var data = Request.Form["data"];
            var stationIds = JsonConvert.DeserializeObject<int[]>(data);
            var stationNames = DocumentManagementBLL.GetStationInfoById(stationIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, stationNames, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取人员列表
        public string GetLastFiveCreateUser()
        {
            var data = Request.Form["text"];
            var userList = new List<UserInfo>();
            if (string.IsNullOrWhiteSpace(data))   //查询最近5个上传文档的人员
            {
                userList = SharedBLL.GetLastFiveCreateUser();
            }
            else              //模糊查询
            {
                userList = SharedBLL.SelectUserList(data);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //公司文档删除
        public string DeleteFlagDocument()
        {
            var dataJson = Request.Form["data"];
            var documentIds = JsonConvert.DeserializeObject<int[]>(dataJson);
            var userId = Session["userId"];
            if (userId != null)
            {
                SharedBLL.DeleteCompanyDocument(documentIds, Convert.ToInt32(userId), 1);

                //TODO: 公司文档标签
                //删除公司文档标签
                TagManagementBLL.RemoveCompanyDocumentTag(Convert.ToInt32(userId), documentIds);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取公司文档日志
        public string GetCompanyDocumenLogList(int documentId)
        {
            var logList = SharedBLL.GetCompanyDocumenLogList(documentId);
            var jsonResult = new JsonResultModel(JsonResultType.success, logList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //单文件下载:flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult Download(int documentId, string displayName, string saveName, bool isFolder, int flag)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var path = displayName;
            if (isFolder)
            {
                var lstFile = new List<CompressInfo>();
                var docList = new List<DocumentModel>();

                docList = SharedBLL.GetAllDocumentInFolder(documentId, int.Parse(Session["userId"].ToString()), docList, ref path);

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
                        var downLoadPath = Path.Combine(FilePath.DocumentUpLoadPath, item.saveName);
                        lstFile.Add(new CompressInfo { path = downLoadPath, display = item.displayName, folder = item.path });
                    }
                }
                SharedBLL.AddDownloadLog(documentId, 1, string.Empty, userId);
                SharpZipLibrary sharpZip = new SharpZipLibrary();
                //return File(sharpZip.Compress(lstFile), "application/octet-stream", Server.UrlEncode(string.Format("{0}.zip", Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(fileName)))));
                return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}.zip", fileName), System.Text.Encoding.UTF8));
            }
            else
            {
                var downLoadPath = Path.Combine(FilePath.DocumentUpLoadPath, saveName);
                if (!System.IO.File.Exists(downLoadPath))
                {
                    return JavaScript("noFile();");
                }
                if (flag == 0)
                {
                    return Content("success");
                }
                SharedBLL.AddDownloadLog(documentId, 1, string.Empty, userId);
                //return File(downLoadPath, "application/octet-stream", Server.UrlEncode(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(displayName))));
                return File(downLoadPath, "application/octet-stream", HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8));
            }
        }

        //批量下载 flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult MultiDownload(int flag)
        {
            var docList = new List<DocumentModel>();
            var lstFile = new List<CompressInfo>();
            var userId = Convert.ToInt32(Session["userId"]);
            if (flag == 0)
            {
                var documentIds = JsonConvert.DeserializeObject<int[]>(Request.Form["data"]);
                if (documentIds.Length > 0)
                {
                    //记录日志
                    foreach (var item in documentIds)
                    {
                        SharedBLL.AddDownloadLog(item, 1, string.Empty, userId);
                    }
                }
                TempData["documentIds"] = documentIds;
                docList = SharedBLL.GetDocumentListByIds(documentIds);
            }
            else
            {
                docList = SharedBLL.GetDocumentListByIds(TempData["documentIds"] as int[]);
            }

            bool fileExists = false;
            string fileName = null;

            foreach (var item in docList)
            {
                if (item.saveName != null)
                {
                    var downLoadPath = Path.Combine(FilePath.DocumentUpLoadPath, item.saveName);
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
            //return File(sharpZip.Compress(lstFile), "application/octet-stream", Server.UrlEncode(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(string.Format("{0}等.zip", fileName)))));
            return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}等.zip", fileName), System.Text.Encoding.UTF8));
        }
    }
}