using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;
using MB.Web.Models;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class SharedController : Controller
    {
        //
        // GET: /Shared/

        #region 变量区域

        private ISharedBLL SharedBLL { get; set; }
        private IPlanBLL PlanBLL { get; set; }
        private ITagManagementBLL TagManagementBLL { get; set; }

 
        #endregion 变量区域

        public ActionResult Index()
        {
            return View();
        }

        //返回组织架构选择弹窗
        public ActionResult GetDepartmentHtml()
        {
            return View("~/Views/Shared/DepartmentSelect.cshtml");
        }

        //返回岗位选择弹窗
        public ActionResult GetStationHtml()
        {
            return View("~/Views/Shared/StationSelect.cshtml");
        }

        //返回人员选择弹窗
        public ActionResult GetPersonHtml()
        {
            return View("~/Views/Shared/PersonSelect.cshtml");
        }

        //返回新建文件夹的视图
        public ActionResult GetNewFolder()
        {
            return View("~/Views/DocumentShare/NewFile-building.cshtml");
        }

        //返回移动的视图
        public ActionResult GetMoveList()
        {
            return View("~/Views/DocumentShare/Move.cshtml");
        }

        ////返回共享视图
        //public ActionResult GetDhared()
        //{
        //    return View("~/Views/DocumentShare/Shared.cshtml");
        //}
        public ActionResult HomeIndex()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var execution = PlanBLL.GetExecution(userId);
            Session["execution"] = execution;
            return View("~/Views/HomeIndex/HomeIndex.cshtml");
        }

        //新建文件
        public string AddFile()
        {
            var userId = Session["userId"];
            if (userId == null) return AjaxCallBack.FAIL;
            var documentJson = Request.Form["param"];
            if (documentJson == null) return AjaxCallBack.FAIL;
            var documentModel = JsonConvert.DeserializeObject<AddNewFileModel>(documentJson);
            if (documentModel.type == 1)   //新建公司文件
            {
                if (documentModel != null)
                {
                    var documentId = SharedBLL.AddCompanyFile(documentModel, Convert.ToInt32(userId));
                    //TODO: 公司文档标签
                    //保存公司文档标签
                    TagManagementBLL.SaveCompanyDocumentTag(Convert.ToInt32(userId), documentId);
                }
            }
            else if (documentModel.type == 2)  //新建用户文件
            {
                if (documentModel != null)
                {
                    var documentId = SharedBLL.AddUserFile(documentModel, Convert.ToInt32(userId));
                    //TODO: 个人文档标签
                    //保存个人文档标签
                    TagManagementBLL.SaveUserDocumentTag(Convert.ToInt32(userId), documentId);
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //新建文件夹
        public string BuildNewUserFolder(int type, int? folder, string folderName, string description)
        {
            var userId = Session["userId"];
            if (userId == null) return AjaxCallBack.FAIL;
            if (type == 1)  //新建公司文件夹
            {
                SharedBLL.BuildNewFolder(folder, folderName, description, Convert.ToInt32(userId));
            }
            else if (type == 2)  //新建个人文件夹
            {
                SharedBLL.BuildNewUserFolder(folder, folderName, description, Convert.ToInt32(userId));
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //取得文档目录
        public string GetFolderList(int type, int? folder)
        {
            var directoryList = new List<FileDirectoryModel>();
            var userId = Session["userId"];
            if (type == 1) //公司文档
            {
                directoryList = SharedBLL.GetFolderDirectory(folder);
            }
            else if (type == 2) //个人文档
            {
                if (userId == null) return AjaxCallBack.FAIL;
                directoryList = SharedBLL.GetUserFolderDirectory(folder, Convert.ToInt32(userId));
            }
            else if (type == 3) //有权限的公司文档
            {
                if (userId == null) return AjaxCallBack.FAIL;
                directoryList = SharedBLL.GetFolderDirectoryHasAuth(folder, Convert.ToInt32(userId));
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, directoryList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //模糊查询文件夹列表
        public string GetLikeFolderList(int type)
        {
            var folderName = Request.Form["text"].ToString();
            var folderList = SharedBLL.GetLikeFolderList(type, folderName, int.Parse(Session["userId"].ToString()));
            var jsonResult = new JsonResultModel(JsonResultType.success, folderList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //文档复制
        public string Copy()
        {
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<CopyModel>(copyDataJson);
            var userId = Session["userId"];
            if (userId == null) return AjaxCallBack.FAIL;
            var flag = true;
            if (copyDataModel.documentType == 1)  //源文档类型：公司文档
            {
                SharedBLL.CopyDocument(copyDataModel.documentId, copyDataModel.companyFolder, copyDataModel.userFolder, Convert.ToInt32(userId), copyDataModel.withAuth);
            }
            else  //源文档类型：个人文档
            {
                SharedBLL.CopyUserDocument(copyDataModel.documentId, copyDataModel.companyFolder, copyDataModel.userFolder, Convert.ToInt32(userId));
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //文档移动
        public string Move()
        {
            var dataJson = Request.Form["data"];
            var moveModel = JsonConvert.DeserializeObject<MoveModel>(dataJson);
            var userId = Session["userId"];
            if (userId == null) return AjaxCallBack.FAIL;
            if (moveModel.documentType == 1)  //源文档类型：公司文档
            {
                if (!moveModel.withAuth)
                {
                    SharedBLL.MoveDocumentWithoutAuth(moveModel.documentId, moveModel.folder[0], Convert.ToInt32(userId));
                }
                else
                {
                    SharedBLL.MoveDocument(moveModel.documentId, moveModel.folder[0], Convert.ToInt32(userId));
                }
            }
            else if (moveModel.documentType == 2)  //源文档类型：个人文档
            {
                SharedBLL.MoveUserDocument(moveModel.documentId, moveModel.folder[0], Convert.ToInt32(userId));
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 文档批量下载（检索结果画面用）
        /// </summary>
        /// <param name="flag">0：不下载只检测文件是和if存在 1：进行文件下载</param>
        /// <returns></returns>
        public ActionResult MultiDocumentDownload(int flag)
        {
            var userId = Convert.ToInt32(Session["userId"]);

            var companyDocList = new List<DocumentModel>();
            var userDocList = new List<DocumentModel>();
            var lstFile = new List<CompressInfo>();

            if (flag == 0)
            {
                var info = JsonConvert.DeserializeObject<List<SearchDocumentOperateModel>>(Request["data"]);

                if (info.Count == 0) return JavaScript("noFile();");

                //公司文档信息
                var companyDocIds = info.Where(p => p.isCompany).Select(p => p.id).ToArray();
                if (companyDocIds.Count() > 0)
                {
                    foreach (var id in companyDocIds)
                        SharedBLL.AddDownloadLog(id, 1, string.Empty, userId);

                    companyDocList = SharedBLL.GetDocumentListByIds(companyDocIds);
                }
                TempData["companyDocIds"] = companyDocIds;

                //个人文档信息
                var userDocIds = info.Where(p => !p.isCompany).Select(p => p.id).ToArray();

                if (userDocIds.Count() > 0)
                {
                    userDocList = SharedBLL.GetUserDocumentListByIds(userDocIds);
                }
                TempData["userDocIds"] = userDocIds;
            }
            else
            {
                companyDocList = SharedBLL.GetDocumentListByIds(TempData["companyDocIds"] as int[]);
                userDocList = SharedBLL.GetUserDocumentListByIds(TempData["userDocIds"] as int[]);
            }

            bool fileExists = false;
            string fileName = null;

            foreach (var item in companyDocList)
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

            foreach (var item in userDocList)
            {
                if (item.saveName != null)
                {
                    var downLoadPath = Path.Combine(FilePath.MineUpLoadPath, item.saveName);
                    if (!fileExists && System.IO.File.Exists(downLoadPath))
                    {
                        fileName = item.displayName;
                        fileExists = true;
                    }
                    lstFile.Add(new CompressInfo { path = downLoadPath, display = item.displayName, folder = item.path });
                }
            }

            //TODO: 文档下载
            if (!fileExists)
            {
                return Content("error");
            }
            else if (fileExists && flag == 0)
            {
                return Content("success");
            }
            SharpZipLibrary sharpZip = new SharpZipLibrary();

            return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}等.zip", fileName), System.Text.Encoding.UTF8));
        }

        //获取组织架构
        public string GetOrganizationList(int? parent)
        {
            var orgList = SharedBLL.GetOrgListById(parent);
            var jsonResult = new JsonResultModel(JsonResultType.success, orgList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //模糊查询用户列表
        public string SelectUserList(string userName)
        {
            var userList = SharedBLL.SelectUserList(userName);
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //模糊查询用户信息列表
        public string SelectUserInfoList()
        {
            var text = Request.Form["text"].ToString();
            var userInfoList = SharedBLL.SelectUserInfoList(text);
            var jsonResult = new JsonResultModel(JsonResultType.success, userInfoList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //模糊查询人员列表（包含组织架构）
        public string GetUserListByNameFormUserDocument(string name)
        {
            var text = Request.Form["text"].ToString();
            var userList = SharedBLL.GetUserListByName(text);
            var user = new List<UserInfo>();
            foreach (var item in userList)
            {
                if (item.id != Convert.ToInt32(Session["userId"]))
                {
                    user.Add(item);
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, user, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string GetUserListByName(string name)
        {
            var text = Request.Form["text"].ToString();
            var userList = SharedBLL.GetUserListByName(text);
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        // 模糊查询岗位列表
        public string GetStationListByName()
        {
            var text = Request.Form["text"].ToString();
            var stationList = SharedBLL.GetStationListByName(text);
            var jsonResult = new JsonResultModel(JsonResultType.success, stationList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //模糊查询组织架构
        public string GetOrgListByName()
        {
            var text = Request.Form["text"].ToString();
            var orgist = SharedBLL.GetOrgListByName(text);
            var jsonResult = new JsonResultModel(JsonResultType.success, orgist, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取当前已有共享人
        public string GetSharedUser(int documentId)
        {
            var userList = SharedBLL.GetUserList(documentId);
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取人员列表
        public string GetUserList(int withSub, int organizationId, bool withUser = true)
        {
            var userList = new List<UserInfo>();
            if (withSub == 1)  //包含下级
            {
                userList = SharedBLL.GetPersonListByOrgId(organizationId);
            }
            else if (withSub == 0)  //不包含下级
            {
                userList = SharedBLL.GetPersonListByThisOrgId(organizationId);
            }

            if (withUser == false)
            {
                var model = userList.Where(p => p.userId == int.Parse(Session["userId"].ToString())).FirstOrDefault();

                if (model != null)
                {
                    userList.Remove(model);
                }
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //共享给他人
        public string ShareToOthers()
        {
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<SharedModel>(copyDataJson);
            int[] documentId = copyDataModel.documentId;
            int[] userId = copyDataModel.userId;
            int flags = copyDataModel.flag;
            var flag = SharedBLL.ShareToOthers(documentId, userId, flags);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //取消共享
        public string NoShareToOthers()
        {
            var copyDataJson = Request.Form["data"];
            var copyDataModel = JsonConvert.DeserializeObject<DocIdListModel>(copyDataJson);
            var flag = SharedBLL.NoShareToOthers(copyDataModel.documents);
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取岗位信息
        public string GetStationList(int withSub, int organizationId, bool withNum = false)
        {
            var stationList = new List<StationModel>();
            if (withSub == 1)  //包含下级
            {
                stationList = SharedBLL.GetStationListByOrgId(organizationId);
            }
            else if (withSub == 0)  //不包含下级
            {
                stationList = SharedBLL.GetStationListByThisOrgId(organizationId);
            }

            //获取岗位的在职用户数
            if (withNum)
            {
                foreach (var item in stationList)
                {
                    item.userNum = SharedBLL.GetUserNumByStationId(item.stationId.Value);
                }
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, stationList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //返回所有的该文件夹所有的上级文件夹列表
        public string getAllParentIds(int documentId)
        {
            var docList = new List<FileDirectoryModel>();
            docList = SharedBLL.getAllParentIds(documentId, docList);
            docList.Reverse();
            var jsonResult = new JsonResultModel(JsonResultType.success, docList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string getUserDocParentIds(int documentId, int type)
        {
            var docList = new List<FileDirectoryModel>();
            docList = SharedBLL.getAllParentIds(documentId, docList, type);
            docList.Reverse();
            var jsonResult = new JsonResultModel(JsonResultType.success, docList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取新闻分类列表
        /// </summary>
        /// <param name="parent">上级分类ID</param>
        /// <returns></returns>
        public string GetNewsTypeList(int? parent)
        {
            var typeList = SharedBLL.GetNewsDirectory(parent);

            var jsonResult = new JsonResultModel(JsonResultType.success, typeList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取通知分类列表
        /// </summary>
        /// <param name="parent">上级分类ID</param>
        /// <returns></returns>
        public string GetNoticeTypeList(int? parent)
        {
            var typeList = SharedBLL.GetNoticeDirectory(parent);

            var jsonResult = new JsonResultModel(JsonResultType.success, typeList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取公司文档目录
        /// </summary>
        /// <param name="parent">上级文件夹</param>
        /// <returns></returns>
        public string GetDocumentFolder(int? parent)
        {
            var directoryList = SharedBLL.GetFolderDirectory(parent, false);

            var jsonResult = new JsonResultModel(JsonResultType.success, directoryList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除图片库图片
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public string DeleteImage(int imageId)
        {
            SharedBLL.DeleteImage(imageId);

            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //返回操作成功的空JSON串
        public string ReturnSuccessJson()
        {
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 文件转化为.swf文件以供预览
        /// </summary>
        /// <param name="type"></param>
        /// <param name="saveName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string ConvertFile(int type, string saveName, string extension)
        {
            var filePath = Path.Combine(new FileUpload().ConfigPath(type), saveName);
            //var inputFullPath = Path.Combine(new FileUpload().ConfigPath(10), saveName) + "." + extension;
            var inputFullPath = Path.Combine(new FileUpload().ConfigPath(10), saveName) + extension;
            var outputFullPath = Path.Combine(new FileUpload().ConfigPath(10), saveName) + ".swf";
            var resultUrl = "/" + ConfigurationManager.AppSettings["ConvertFilePath"].ToString() + "/" + saveName + ".swf";

            if (!System.IO.File.Exists(filePath))
            {
                var jsonResult = new JsonResultModel(JsonResultType.error, null, "文件不存在！");
                return JsonConvert.SerializeObject(jsonResult);
            }

            if (System.IO.File.Exists(outputFullPath))
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, resultUrl, "正常");
                return JsonConvert.SerializeObject(jsonResult);
            }

            if (!System.IO.Directory.Exists(new FileUpload().ConfigPath(10)))
            {
                System.IO.Directory.CreateDirectory(new FileUpload().ConfigPath(10));
            }

            if (!System.IO.File.Exists(inputFullPath))
            {
                System.IO.File.Copy(filePath, inputFullPath);
            }

            Print2Flash3.Server2 p2fServer = new Print2Flash3.Server2();
            p2fServer.DefaultProfile.InterfaceOptions = 0;
            p2fServer.DefaultProfile.ProtectionOptions = (int)Print2Flash3.PROTECTION_OPTION.PROTENAPI;
            p2fServer.ConvertFile(inputFullPath, outputFullPath, null, null, null);

            System.IO.File.Delete(inputFullPath);

            var result = new JsonResultModel(JsonResultType.success, resultUrl, "正常");
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 取得登陆用户常用标签
        /// </summary>
        /// <returns></returns>
        public string GetMostUsedTag()
        {
            var userId = Session["userId"];
            if (userId == null) return AjaxCallBack.FAIL;

            var tag = TagManagementBLL.GetMostUsedTag(Convert.ToInt32(userId));

            var result = new JsonResultModel(JsonResultType.success, tag, "正常");

            return JsonConvert.SerializeObject(result);
        }
    }
}