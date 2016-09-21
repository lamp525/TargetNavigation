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
using MB.Web.NotifyServiceReference;
using System.Threading.Tasks;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class FlowIndexController : BaseController
    {
        //
        // GET: /FlowIndex/

        #region 变量区域

        private IFlowIndexBLL FlowIndexBLL { get; set; }

 

        //private FlowIndexBLL flowIndexBll = new FlowIndexBLL();

        #endregion 变量区域

        //1、管理员 2、我的未完成流程  3、待审批及待查阅的列表
        public ActionResult FlowIndex(int id)
        {
            ViewBag.ShowAdmin = id;
            return View();
        }

        //加载转发窗口
        public ActionResult LoadTranspont()
        {
            return View("~/Views/Shared/PersonSelect.cshtml");
        }

        //加载意见窗口
        public ActionResult LoadSuggestion()
        {
            return View("~/Views/Shared/suggestion.cshtml");
        }

        //加载详情窗口
        public ActionResult LoadDetail()
        {
            return View("~/Views/Shared/FlowDetail.cshtml");
        }

        //获取模板分类列表
        public string GetTemplateCategoryList()
        {
            var templateCategoryList = FlowIndexBLL.GetTemplateCategoryList();
            var jsonResult = new JsonResultModel(JsonResultType.success, templateCategoryList, "huoqu");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取模板列表
        public string GetTemplateList(int categoryId)
        {
            var templateList = FlowIndexBLL.GetTemplateList(categoryId, Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, templateList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取模板html
        public string GetTemplateHtml(int templateId)
        {
            var templateModel = FlowIndexBLL.GetTemplateHtml(templateId);
            var jsonResult = new JsonResultModel(JsonResultType.success, templateModel, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //新建流程
        public string AddFlow(int flag)
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "新建失败");
            var flowModel = JsonConvert.DeserializeObject<AddFormContentModel>(dataJson);
            flowModel.createUser = Convert.ToInt32(Session["userId"]);
            flowModel.createTime = DateTime.Now;
            var returnFlag=false;
            var returnUser = FlowIndexBLL.AddFlow(flowModel, flag);
            if (returnUser.result == 0)
            {
                returnFlag = false;
            }
            else
            {
                returnFlag = true;

                if (returnUser.opreateUser.Count > 0)
                {
                    foreach (var item in returnUser.opreateUser)
                    {
                        //发送即时提醒
                        this.MBService(item.ToString(), Protocol.ClientActualTimeProtocol.FUC, Session["userName"].ToString());
                        //using (var mbService = new MBServiceClient())
                        //{
                        //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + item.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.FUC);
                        //}

                        //using (var mbService = new TaskRemindServiceClient())
                        //{
                        //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + item.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.FUC);
                        //}
                    }
                }
                else
                {
                    //发送即时提醒
                    this.MBService(returnUser.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.FUS);
                    //using (var mbService = new MBServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnUser.confirmUser.ToString() + "+" + Protocol.ClientProtocol.FUS);
                    //}
                    //using (var mbService = new TaskRemindServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnUser.confirmUser.ToString() + "+" + Protocol.ClientProtocol.FUS);
                    //}
                }

            }
            return ReturnJson(JsonResultType.success, returnFlag, "新建成功");
        }

        //获取流程首页列表
        public string GetFlowIndexJson(int admin)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var conditionJson = Request.Form["data"];
            if (conditionJson == null)
            {
                var msg = MessageUtility.GetMessage("99002");
                var failResult = new JsonResultModel(JsonResultType.error, null, msg);
                return JsonConvert.SerializeObject(failResult);
            }
            var conditionModel = JsonConvert.DeserializeObject<FormConditionModel>(conditionJson);
            var flowList = new List<UserFormModel>();
            var conditionString = string.Empty;
            var conditionStart = DateTime.MinValue;
            var conditionEnd = DateTime.MaxValue;

            #region 拼凑SQL

            //创建人
            if (conditionModel.person.Length > 0)
            {
                conditionString += "(";
                foreach (var item in conditionModel.person)
                {
                    conditionString += " createUser==" + item + " Or ";
                }
                conditionString = conditionString.Substring(0, conditionString.LastIndexOf("Or")) + " ) ";
            }
            //部门
            if (conditionModel.department.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(conditionString))
                {
                    conditionString += " And ";
                }
                conditionString += "(";
                foreach (var item in conditionModel.department)
                {
                    conditionString += " organizationId==" + item + " Or ";
                }
                conditionString = conditionString.Substring(0, conditionString.LastIndexOf("Or")) + " ) ";
            }
            //筛选时间
            if (conditionModel.time.Length > 0)
            {
                if (conditionModel.time[0] == "1")
                {
                    conditionStart = DateTime.Now.AddDays(-6).Date;
                    conditionEnd = DateTime.Now.AddDays(1).Date;
                }
                else if (conditionModel.time[0] == "2")
                {
                    conditionStart = DateTime.Now.AddMonths(-1).Date;
                    conditionEnd = DateTime.Now.AddDays(1).Date;
                }
                else
                {
                    if (conditionModel.time[2] == "")
                    {
                        conditionStart = Convert.ToDateTime(conditionModel.time[1]).Date;
                        conditionEnd = DateTime.MaxValue;
                    }
                    else if (conditionModel.time[1] == "")
                    {
                        conditionStart = DateTime.MinValue;
                        conditionEnd = Convert.ToDateTime(conditionModel.time[2]).AddDays(1).Date;
                    }
                    else
                    {
                        conditionStart = Convert.ToDateTime(conditionModel.time[1]);
                        conditionEnd = Convert.ToDateTime(conditionModel.time[2]).AddDays(1).Date;
                    }
                }
            }

            #endregion 拼凑SQL

            if (conditionModel.status.Length > 0)
            {
                foreach (var item in conditionModel.status)
                {
                    var flowTempList = new List<UserFormModel>();
                    if (item == 1 || item == 2)  //1、待提交 2、已提交
                    {
                        flowTempList = FlowIndexBLL.GetUnSubmitFlowList(userId, conditionString, conditionStart, conditionEnd, conditionModel.type, item, admin);
                    }
                    else if (item == 3)  //3、待审批
                    {
                        flowTempList = FlowIndexBLL.GetUnCheckFlowList(userId, conditionString, conditionStart, conditionEnd, conditionModel.type);
                    }
                    else if (item == 7)  //7、待查阅
                    {
                        flowTempList = FlowIndexBLL.GetUnReadFlowList(userId, conditionString, conditionStart, conditionEnd, conditionModel.type);
                    }
                    else if (item == 4)//4、已处理
                    {
                        flowTempList = FlowIndexBLL.GetCheckedFlowList(userId, conditionString, conditionStart, conditionEnd, conditionModel.type);
                    }
                    else if (item == 5) //5、已办结
                    {
                        flowTempList = FlowIndexBLL.GetCompletedFlowList(userId, conditionString, conditionStart, conditionEnd, conditionModel.type, admin);
                    }
                    else if (item == 11)  //管理员登录，11、流程中
                    {
                        flowTempList = FlowIndexBLL.GetFlowingUserFormList(conditionString, conditionStart, conditionEnd, conditionModel.type);
                    }
                    flowList.AddRange(flowTempList);
                }
            }
            else //获取所有状态的流程
            {
                flowList = FlowIndexBLL.GetAllFlowList(userId, conditionString, conditionStart, conditionEnd, conditionModel.type, admin);
            }

            //排序
            if (flowList.Count > 0)
            {
                flowList = FlowIndexBLL.GetFlowListOrderByCustom(conditionModel.sort, flowList);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, flowList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取常用联系人
        public string GetOffUser()
        {
            var text = Request.Form["text"];
            if (string.IsNullOrWhiteSpace(text))
            {
                var userId = Convert.ToInt32(Session["userId"]);
                var userList = FlowIndexBLL.GetUserIdListByUserId(userId);
                var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常", true);
                return JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var userList = FlowIndexBLL.SelectUserList(text.ToString(), true);
                var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常", true);
                return JsonConvert.SerializeObject(jsonResult);
            }
        }

        //获取流程详情
        public string GetFlowDetailListById(int formId, int nodeId)
        {
            var formDetailModel = FlowIndexBLL.GetFlowDetailListById(formId, nodeId);
            var jsonResult = new JsonResultModel(JsonResultType.success, formDetailModel, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //提交操作
        public string SubmitFlow(int templateId, int formId, int nodeId, int flag)
        {
            var returnFlag = false;
            if (flag != 0)
            {
                //先保存控件值
                var data = Request.Form["data"];
                var formContent = JsonConvert.DeserializeObject<AddFormContentModel>(data);
                returnFlag = FlowIndexBLL.SaveControlValue(formId, formContent, flag);
                if (!returnFlag) return ReturnJson(JsonResultType.error, returnFlag, "提交失败");
            }
            var returnUser = FlowIndexBLL.SubmitFlow(templateId, formId, nodeId, Convert.ToInt32(Session["userId"]));
            if (returnUser.result != 0&&returnUser!=null)
            {
                returnFlag = true;
                foreach (var item in returnUser.opreateUser)
                {
                    //发送即时提醒
                    this.MBService(item.ToString(), Protocol.ClientActualTimeProtocol.FUC);
                    //using (var mbService = new MBServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + item.ToString() + "+" + Protocol.ClientProtocol.FUC);
                    //}
                    //using (var mbService = new TaskRemindServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + item.ToString() + "+" + Protocol.ClientProtocol.FUC);
                    //}
                }
               
            }
            else
            {
                returnFlag = false;
            }
            return ReturnJson(JsonResultType.success, returnFlag, "提交成功");
        }

        //退回操作
        public string TurnBack(int nodeId, int templateId, int formId, string suggest, int isEntruct)
        {
            var flag = FlowIndexBLL.TurnBack(nodeId, templateId, formId, suggest, Convert.ToInt32(Session["userId"]), isEntruct);
            return ReturnJson(JsonResultType.success, flag, "退回成功");
        }

        //撤回操作
        public string BackFirstNode(int nodeId, int templateId, int formId)
        {
            var flag = FlowIndexBLL.BackFirstNode(nodeId, templateId, formId, Convert.ToInt32(Session["userId"]));
            return ReturnJson(JsonResultType.success, flag, "撤回成功");
        }

        //撤回提交
        public string CancelSubmit(int templateId, int formId, int nodeId)
        {
            var flag = FlowIndexBLL.CancelSubmit(templateId, formId, nodeId, Convert.ToInt32(Session["userId"]));
            return ReturnJson(JsonResultType.success, flag, "撤回成功");
        }

        //表单抄送
        public string DuplicateForm(int formId, int nodeId, int templateId)
        {
            var dataJson = Request.Form["data"];
            var data = JsonConvert.DeserializeObject<int[]>(dataJson);
            var flag = FlowIndexBLL.DuplicateForm(data, formId, nodeId, templateId, Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //表单删除
        public string deleteUserForm(int formId)
        {
            var flag = FlowIndexBLL.deleteUserForm(formId, Convert.ToInt32(Session["userId"]));
            return flag ? ReturnJson(JsonResultType.success, null, "删除成功") : ReturnJson(JsonResultType.error, null, "删除失败");
        }

        //意见
        public string AddContents(int formId, int nodeId, string contents)
        {
            FlowIndexBLL.AddContents(formId, nodeId, contents, Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //同意
        public string AgreeFormFlow(int templateId, int formId, int nodeId, int isEtruct, string suggest)
        {
            var flag = FlowIndexBLL.AgreeFormFlow(templateId, formId, nodeId, suggest, Convert.ToInt32(Session["userId"]), isEtruct);
            return ReturnJson(JsonResultType.success, flag, "审批成功");
        }

        //流程中的提交
        public string SubmitInFlow(int templateId, int formId, int nodeId, string suggest, int isEntruct, int flag)
        {
            var data = Request.Form["data"];
            var formContent = JsonConvert.DeserializeObject<AddFormContentModel>(data);
            //保存控件的值
            var returnFlag = FlowIndexBLL.SaveControlValue(formId, formContent, flag);
            if (!returnFlag) return ReturnJson(JsonResultType.success, returnFlag, "提交成功");
            returnFlag = FlowIndexBLL.SubmitInFlow(templateId, formId, nodeId, suggest, Convert.ToInt32(Session["userId"]), isEntruct);
            return ReturnJson(JsonResultType.success, returnFlag, "提交成功");
        }

        //获取饼图数据
        public string GetFlowProcessList(int year, int month, int admin)
        {
            var processList = FlowIndexBLL.GetFlowProcessList(year, month, Convert.ToInt32(Session["userId"]), admin);
            var jsonResult = new JsonResultModel(JsonResultType.success, processList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取模板详情
        public string GetTemplateInfoById(int templateId, int? nodeId, int? formId, int flag, int? operateStatus)
        {
            var templateDetail = FlowIndexBLL.GetTemplateInfoById(templateId, nodeId, formId, flag, operateStatus);
            var jsonResult = new JsonResultModel(JsonResultType.success, templateDetail, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //绑定表单创建者的部门
        public string GetorganizationList(int formId)
        {
            var organizationList = FlowIndexBLL.GetorganizationList(formId);
            var jsonResult = new JsonResultModel(JsonResultType.success, organizationList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //绑定登录用户的创建部门
        public string GetUserOrganizationList()
        {
            var organizationList = FlowIndexBLL.GetUserOrganizationList(Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, organizationList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //绑定登录用户的创建岗位
        public string GetUserStationList(int orgId)
        {
            var stationList = FlowIndexBLL.GetUserStationList(orgId, Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, stationList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //详情修改后保存控件值
        public string SaveControlValue(int formId, int flag)
        {
            var data = Request.Form["data"];
            var formContent = JsonConvert.DeserializeObject<AddFormContentModel>(data);
            var returnFlag = FlowIndexBLL.SaveControlValue(formId, formContent, flag);
            var jsonResult = new JsonResultModel(JsonResultType.success, returnFlag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取登录用户未完成流程列表
        public string GetUserUnCompleteList()
        {
            var formList = FlowIndexBLL.GetUserUnCompleteList(Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, formList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //上传流程文档
        [HttpPost]
        public string UploadDocument()
        {
            var uploadType = ConstVar.UploadType.FlowIndexFile;
            var fileModel = new UploadFileModel();
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                FileUpload upload = new FileUpload();
                fileModel = upload.UploadFile(hpf, (int)uploadType);
                //上传成功后插入表数据
            }
            return JsonConvert.SerializeObject(fileModel);
        }

        //单文件下载:flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult Download(string displayName, string saveName, int flag)
        {
            var downLoadPath = Path.Combine(FilePath.FlowIndexUploadPath, saveName);
            if (!System.IO.File.Exists(downLoadPath))
            {
                return Content("fail");
            }
            if (flag == 0)
            {
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