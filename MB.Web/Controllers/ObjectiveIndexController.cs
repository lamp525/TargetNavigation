using MB.Web.Models;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;
using MB.Web.NotifyServiceReference;
using System.ServiceModel;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class ObjectiveIndexController : BaseController
    {
        //
        // GET: /ObjectiveIndex/
        public delegate void AddHandler(string msg);
        private IObjectiveIndexBLL ObjectiveIndexBLL { get; set; }

        private ITagManagementBLL TagManagementBLL { get; set; }

 
        public ActionResult ObjectiveIndex()
        {
            return View();
        }

        //加载公式视图
        public ActionResult GetObjectiveFormulaHtml()
        {
            return View("~/Views/ObjectiveShared/ObjectiveFormula.cshtml");
        }

        //获取目标列表
        public string GetObjectiveList()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var conditionJson = Request.Form["data"];
            var conditionModel = JsonConvert.DeserializeObject<ObjectiveConditionModel>(conditionJson);

            var objectiveInfo = new ObjectiveInfo();
            //筛选条件拼凑字符串
            var conditionNew = string.Empty;
            //筛选的开始时间
            var conditionStart = DateTime.MinValue;
            //筛选的结束时间
            var conditionEnd = DateTime.MaxValue;

            #region 拼凑SQL

            if (conditionModel.soonSelect == null)  //不是快捷筛选
            {
                if (conditionModel.status.Length > 0)
                {
                    conditionNew += " ( ";
                    foreach (var item in conditionModel.status)
                    {
                        conditionNew += " status==" + item + " Or ";
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                if (conditionModel.department.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " ( ";
                    foreach (var item in conditionModel.department)
                    {
                        conditionNew += " responsibleOrg==" + item + " Or ";
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                if (conditionModel.person.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " ( ";
                    if (conditionModel.objectiveType == 1)  //我的目标的情况
                    {
                        foreach (var item in conditionModel.person)
                        {
                            conditionNew += " confirmUser==" + item + " Or ";
                        }
                    }
                    else      //下属目标的情况
                    {
                        foreach (var item in conditionModel.person)
                        {
                            conditionNew += " responsibleUser==" + item + " Or authorizedUser==" + item + " Or ";
                        }
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                if (conditionModel.objectiveType == 1)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " (responsibleUser==" + userId + " Or authorizedUser==" + userId + ") ";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " confirmUser==" + userId + " ";
                }
                //筛选时间
                if (conditionModel.startTime.Length > 0)
                {
                    if (conditionModel.startTime[0] == "1") //近一周
                    {
                        conditionStart = DateTime.Now.AddDays(-6).Date;
                        conditionEnd = DateTime.Now.AddDays(1).Date;
                    }
                    else if (conditionModel.startTime[0] == "2")    //近一月
                    {
                        conditionStart = DateTime.Now.AddMonths(-1).Date;
                        conditionEnd = DateTime.Now.AddDays(1).Date;
                    }
                    else     //自定义
                    {
                        if (conditionModel.startTime[2] == "")
                        {
                            conditionStart = Convert.ToDateTime(conditionModel.startTime[1]).Date;
                            conditionEnd = DateTime.MaxValue;
                        }
                        else if (conditionModel.startTime[1] == "")
                        {
                            conditionStart = DateTime.MinValue;
                            conditionEnd = Convert.ToDateTime(conditionModel.startTime[2]).AddDays(1).Date;
                        }
                        else
                        {
                            conditionStart = Convert.ToDateTime(conditionModel.startTime[1]);
                            conditionEnd = Convert.ToDateTime(conditionModel.startTime[2]).AddDays(1).Date;
                        }
                    }
                }
            }
            else  //快捷筛选
            {
                if (conditionModel.soonSelect != 6)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " status==" + conditionModel.soonSelect + " ";
                }
                else  //超时目标
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " status==3 ";
                    conditionEnd = DateTime.Now.Date;
                }
                if (conditionModel.objectiveType == 1)  //我的目标
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " responsibleUser==" + userId + " ";
                }
                else  //下属目标
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " confirmUser==" + userId + " ";
                }
            }

            #endregion 拼凑SQL

            if (conditionModel.soonSelect != null & conditionModel.soonSelect == 6)  //超时目标
            {
                objectiveInfo = ObjectiveIndexBLL.GetObjectiveOverTimeList(conditionNew, conditionEnd, userId);
            }
            else
            {
                objectiveInfo = ObjectiveIndexBLL.GetObjectiveList(conditionNew, conditionStart, conditionEnd, userId);
            }
            return ReturnJson(JsonResultType.success, objectiveInfo, "获取列表失败");
        }

        //获取目标子列表
        public string GetChildrenObjectiveList(int objectiveId)
        {
            var objectiveChildren = ObjectiveIndexBLL.GetChildrenObjectiveList(objectiveId, Convert.ToInt32(Session["userId"]));
            return ReturnJson(JsonResultType.success, objectiveChildren, "获取子目标失败");
        }

        //获取甘特图列表
        public string GetGanttChartObjectiveList(string year, string month)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var thisStart = DateTime.MinValue;
            var thisEnd = DateTime.MaxValue;
            if (string.IsNullOrWhiteSpace(month))  //年筛选
            {
                thisStart = Convert.ToDateTime(year + "-01-01");
                thisEnd = thisStart.AddYears(1);
            }
            else      //月筛选
            {
                thisStart = Convert.ToDateTime(year + month.PadLeft('0') + "-01");
                thisEnd = thisStart.AddMonths(1);
            }

            var objectiveList = ObjectiveIndexBLL.GetGanttChartObjectiveList(thisStart, thisEnd, userId);
            return ReturnJson(JsonResultType.success, objectiveList, "获取甘特图数据失败");
        }

        //获取甘特图子目标
        public string GetGanttChartChildObjectiveList(int objectiveId)
        {
            var objectiveChildren = ObjectiveIndexBLL.GetGanttChartChildObjectiveList(objectiveId);
            return ReturnJson(JsonResultType.success, objectiveChildren, "获取子目标失败");
        }

        //判断该目标能否移动
        public string CheckObjectiveMove(int objectiveId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.CheckObjectiveMove(objectiveId);
            return ReturnJson(JsonResultType.success, flag, "该目标移动失败");
        }

        //获取饼图数据统计
        public string GetObjectiveProcessList(int year, int month)
        {
            var list = ObjectiveIndexBLL.GetObjectiveProcessList(year, month, Convert.ToInt32(Session["userId"]));
            return ReturnJson(JsonResultType.success, list, "获取饼图数据失败");
        }

        //获取列表上方的状态统计
        public string GetObjectiveStatusList(int flag)
        {
            var list = ObjectiveIndexBLL.GetObjectiveStatusList(Convert.ToInt32(Session["userId"]), flag);
            return ReturnJson(JsonResultType.success, list, "获取状态统计数据失败");
        }

        //甘特图拖动更新
        public string MoveObjectiveGanttChart(int objectiveId, DateTime fromTime, DateTime toTime, int flag)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var returnFlag = ObjectiveIndexBLL.MoveObjectiveGanttChart(objectiveId, fromTime, toTime, userId, flag);
            return ReturnJson(JsonResultType.success, returnFlag, "更新甘特图数据失败");
        }

        //删除目标
        public string DeleteObjective(int objectiveId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.DeleteObjective(objectiveId, userId);

            if (flag)
            {
                //TODO: 目标标签
                //删除目标标签
                TagManagementBLL.RemoveObjectiveTag(userId, objectiveId);
            }

            return ReturnJson(JsonResultType.success, flag, "删除失败");
        }

        //授权
        public string AuthorizeObjective(int objectiveId, int authorizedUser)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.AuthorizeObjective(objectiveId, authorizedUser, userId);
            return ReturnJson(JsonResultType.success, flag, "授权失败");
        }

        //取消授权
        public string CancelAuthorizeObjective(int objectiveId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.CancelAuthorizeObjective(objectiveId, userId);
            return ReturnJson(JsonResultType.success, flag, "取消授权失败");
        }

        //撤销
        public string RevokeObjective(int objectiveId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.RevokeObjective(objectiveId, userId);
            return ReturnJson(JsonResultType.success, flag, "撤销失败");
        }

        //新建目标 flag:1:提交目标 2：保存目标
        public string NewObjective(int flag)
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "新建目标失败");
            var objectiveModel = JsonConvert.DeserializeObject<AddNewObjectiveModel>(dataJson);
            var userId = Convert.ToInt32(Session["userId"]);

            int objectiveId;
            var confirm = ObjectiveIndexBLL.NewObjective(objectiveModel, userId, flag, out objectiveId);

            //TODO: 目标标签
            //新建目标提交成功
            if (flag == 1 && confirm.result == 0)
            {
                //保存目标标签
                TagManagementBLL.SaveObjectiveTag(userId, objectiveId);
                try
                {
                    //发送即时提醒
                    this.MBService(confirm.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUC, Session["userName"].ToString());
                    //using (var mbService = new MBServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirm.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                    //}

                    //using (var mbService = new TaskRemindServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirm.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                    //}

                }
                catch (Exception)
                {
                    return ReturnJson(JsonResultType.success, confirm.result, "新建失败");
                }
            }
            else
            {
                //发送即时提醒
                this.MBService(userId.ToString(), Protocol.ClientActualTimeProtocol.OUS, Session["userName"].ToString());
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + userId.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}
                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + userId.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}

            }

            return ReturnJson(JsonResultType.success, confirm.result, "新建失败");
        }

        //待提交状态的保存提交操作，1:提交目标 2：保存目标
        public string UpdateObjective(int flag, int operateFlag)
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "新建目标失败");
            var objectiveModel = JsonConvert.DeserializeObject<AddNewObjectiveModel>(dataJson);
            var userId = Convert.ToInt32(Session["userId"]);
            var returnFlag = ObjectiveIndexBLL.UpdateObjective(objectiveModel, userId, flag, operateFlag);

            //TODO: 目标标签
            //目标提交成功
            if (flag == 1 && returnFlag.result == 0)
            {
                //发送即时提醒
                this.MBService(returnFlag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUC, Session["userName"].ToString());
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                //}
                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                //}

                //保存目标标签
                TagManagementBLL.SaveObjectiveTag(userId, objectiveModel.objectiveId.Value);
            }
            else
            {
                //发送即时提醒
                this.MBService(returnFlag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUS);
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}
                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}

            }

            return ReturnJson(JsonResultType.success, returnFlag.result, "操作失败");
        }

        //分解目标 flag:1:提交目标 2：保存目标
        public string SplitObjective(int flag)
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "分解目标失败");
            var objectiveModel = JsonConvert.DeserializeObject<AddNewObjectiveModel>(dataJson);
            var userId = Convert.ToInt32(Session["userId"]);

            int objectiveId;
            var returnFlag = ObjectiveIndexBLL.SplitObjective(objectiveModel, userId, flag, out objectiveId);

            //TODO: 目标标签
            //分解目标提交成功
            if (flag == 1 && returnFlag.result == 0)
            {
                //发送即时提醒
                this.MBService(returnFlag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUC, Session["userName"].ToString());
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                //}
                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                //}
                //保存目标标签
                TagManagementBLL.SaveObjectiveTag(userId, objectiveId);
            }
            else
            {
                //发送即时提醒
                this.MBService(returnFlag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUS);
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}

                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}

            }
            return ReturnJson(JsonResultType.success, returnFlag.result, "分解失败");
        }

        //修改 flag:1、保存 2、提交
        public string EditObjective(int flag)
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "修改失败");
            var objectiveModel = JsonConvert.DeserializeObject<AddNewObjectiveModel>(dataJson);
            int userId = Convert.ToInt32(Session["userId"]);
            var returnFlag = ObjectiveIndexBLL.EditObjective(objectiveModel, userId, flag);

            //TODO: 目标标签
            //修改目标提交成功
            if (flag == 2 && returnFlag.result == 0)
            {
                //发送即时提醒
                this.MBService(returnFlag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUC, Session["userName"].ToString());
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                //}

                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUC);
                //}

                //保存目标标签
                TagManagementBLL.SaveObjectiveTag(userId, objectiveModel.objectiveId.Value);
            }
            else {
                //发送即时提醒
                this.MBService(returnFlag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUS);

                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}
                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnFlag.confirmUser.ToString() + "+" + Protocol.ClientProtocol.OUS);
                //}

            }

            return ReturnJson(JsonResultType.success, returnFlag.result, "修改失败");
        }

        //展开
        public string ExpandObjective(int objectiveId)
        {
            var objectiveModel = ObjectiveIndexBLL.ExpandObjective(objectiveId);
            return ReturnJson(JsonResultType.success, objectiveModel, "展开失败");
        }

        //删除目标文档
        public string DeleteDocument(int objectiveId, int attachmentId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.DeleteDocument(objectiveId, attachmentId, userId);
            return ReturnJson(JsonResultType.success, flag, "删除文档失败");
        }

        //获取目标详情
        public string GetObjectInfo(int objectiveId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var objectiveModel = ObjectiveIndexBLL.GetObjectInfo(objectiveId);
            return ReturnJson(JsonResultType.success, objectiveModel, "获取目标详情失败");
        }

        //获取待提交状态的目标详情
        public string GetSimpleObjectInfo(int objectiveId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var objectiveModel = ObjectiveIndexBLL.GetSimpleObjectInfo(objectiveId);
            return ReturnJson(JsonResultType.success, objectiveModel, "获取目标详情失败");
        }

        //审核 result:操作类型：6：审核通过 7：审核不通过
        public string ApproveObjective()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "审核失败");
            var checkModel = JsonConvert.DeserializeObject<ObjectiveCheckModel>(dataJson);
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.ApproveObjective(checkModel, userId);

            if (flag.result == 0)
            {
                //发送即时提醒
                this.MBService(flag.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.ONS);

                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + flag.confirmUser + "+" + Protocol.ClientProtocol.ONS);
                //}
                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + flag.confirmUser + "+" + Protocol.ClientProtocol.ONS);
                //}
            }
            return ReturnJson(JsonResultType.success, flag, "审核失败");
        }

        //提交确认
        public string SubmitObjectiveExecuteResult(int objectiveId, string actualValue)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var confirmUser = ObjectiveIndexBLL.SubmitObjectiveExecuteResult(objectiveId, actualValue, userId);
            var flag = false;
            if (confirmUser != 0)
            {
                flag = true;

                //发送即时提醒
                this.MBService(confirmUser.ToString(), Protocol.ClientActualTimeProtocol.OUA, Session["userName"].ToString());
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUA);
                //}

                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.OUA);
                //}

            }
            return ReturnJson(JsonResultType.success, flag, "提交失败");
        }

        //确认 result:操作类型 11：确认通过 12：确认不通过
        public string ConfirmObjective(int objectiveId, string message, int result)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            var flag = ObjectiveIndexBLL.ConfirmObjective(objectiveId, message, result, userId);
            return ReturnJson(JsonResultType.success, flag, "确认失败");
        }

        //更新目标进度
        public string UpdateObjectiveProcess(int objectiveId, int newProcess)
        {
            var flag = ObjectiveIndexBLL.UpdateObjectiveProcess(objectiveId, newProcess, Convert.ToInt32(Session["userId"]));
            return ReturnJson(JsonResultType.success, flag, "更新进度失败");
        }

        //获取目标公式信息
        public string GetObjectiveFormula(int objectiveId)
        {
            var formulaList = ObjectiveIndexBLL.GetObjectFormula(objectiveId);
            return ReturnJson(JsonResultType.success, formulaList, "获取目标公式失败");
        }

        //获取目标日志列表
        public string GetObjectiveNewLogs(int objectiveId)
        {
            var logList = ObjectiveIndexBLL.GetObjectiveNewLogs(objectiveId);
            return ReturnJson(JsonResultType.success, logList, "获取日志列表失败");
        }

        //获取目标文档列表
        public string GetObjectiveNewDocuments(int objectiveId)
        {
            var docList = ObjectiveIndexBLL.GetObjectiveNewDocuments(objectiveId);
            return ReturnJson(JsonResultType.success, docList, "获取文档列表失败");
        }

        //上传目标文档
        [HttpPost]
        public string UploadDocument(int objectiveId)
        {
            var uploadType = ConstVar.UploadType.ObjectiveFile;
            var fileModel = new UploadFileModel();
            var userId = Convert.ToInt32(Session["userId"]);
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                FileUpload upload = new FileUpload();
                fileModel = upload.UploadFile(hpf, (int)uploadType);
                //上传成功后插入表数据
                ObjectiveIndexBLL.InsertObjectiveDoc(objectiveId, fileModel, userId);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, fileModel, "上传文档失败");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //单文件下载:flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult Download(string displayName, string saveName, int flag, int objectiveId)
        {
            var downLoadPath = Path.Combine(new FileUpload().ConfigPath(Convert.ToInt32(ConstVar.UploadType.ObjectiveFile)), saveName);
            if (!System.IO.File.Exists(downLoadPath))
            {
                return Content("fail");
            }
            if (flag == 0)
            {
                //添加日志
                ObjectiveIndexBLL.AddDownLoadOperate(objectiveId, Convert.ToInt32(Session["userId"]));
                return Content("success");
            }
            //return File(downLoadPath, "application/octet-stream", Server.UrlEncode(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(displayName))));
            return File(downLoadPath, "application/octet-stream", HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8));
        }

        //返回JSON串
        public string ReturnJson(JsonResultType type, object content, string errorMsg)
        {
            var jsonResult = new JsonResultModel(type, content, errorMsg);
            return JsonConvert.SerializeObject(jsonResult);
        }

        #region 私有方法
        /// <summary>
        /// 发送及时提醒
        /// </summary>
        /// <param name="returnUserId">确认人或者责任人</param>
        /// <param name="userName">用户名字</param>
        /// <param name="protocol">客户端请求协议</param>
        /// <returns></returns>
        private async Task MBService(string returnUserId, string protocol, string userName = null)
        {
            //发送即时提醒
            using (var mbService = new TaskRemindServiceClient())
            {
                //如果
                if (string.IsNullOrEmpty(userName))
                {
                    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnUserId + "+" + protocol);
                }
                else
                {
                    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnUserId + "+" + userName + "+" + protocol);
                }

            }
        }
        #endregion

    }
}