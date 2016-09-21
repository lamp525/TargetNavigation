using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using MB.Web.Models;
using MB.Web.NotifyServiceReference;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class PlanController : BaseController
    {
        //
        // GET: /Plan/

        //实例化PlanBll类对象
        private IPlanBLL PlanBLL { get; set; }

        private ITagManagementBLL TagManagementBLL { get; set; }

        private delegate void CustomHandler(string previewPath, int planId);

        //返回计划首页 id:1、无筛选跳转 2、待提交  3、待审核  4、已审核  5、待确认 6、已完成 7、已中止 8、今日未完成  9、超时计划 10、计划管控
        public ActionResult Index(int id)
        {
            if (Session["userId"] == null)
            {
                return View("~/Views/Login/Login.cshtml");
            }
            int userId = Convert.ToInt32(Session["userId"]);
            //获取人员列表
            var personList = PlanBLL.GetUserIdListByUserId(userId);
            //获取登录用户信息
            var userInfo = PlanBLL.GetUserInfoById(userId);
            //统计用户今日未完成和超时计划的数量
            var userPlanCount = PlanBLL.StatisticsUserPlan(userId);

            ViewBag.UserInfo = userInfo;
            ViewBag.PersonList = personList;
            ViewBag.UserPlanCount = userPlanCount;
            //指示用户界面的显示
            if (id == 10)
            {
                ViewBag.Type = true;
            }
            else
            {
                ViewBag.Type = false;
            }
            ViewBag.OperateId = id;
            return View();
        }

        //返回计划列表局部视图
        public ActionResult GetPlanList()
        {
            if (Session["userId"] == null)
            {
                return View("~/Views/Login/Login.cshtml");
            }
            var userId = Convert.ToInt32(Session["userId"]);
            var planList = PlanBLL.GetPlanList(userId, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue);
            //计划列表执行默认排序
            planList = planList.OrderBy(p => p.completeTime).OrderByDescending(p => p.urgency).OrderByDescending(p => p.importance).ToList();
            ViewBag.PlanList = planList;

            return View("PlanList");
        }

        //加载审核窗
        public ActionResult GetCheckView(int height)
        {
            ViewBag.Height = height;
            return View("CheckInfo");
        }

        //加载确认窗
        public ActionResult GetConfirmView(int height)
        {
            //读取配置文件中的量质时最大值
            ViewBag.CompleteQuantity = ConfigurationManager.AppSettings["maxQuantity"];
            ViewBag.CompleteQuality = ConfigurationManager.AppSettings["maxQuality"];
            ViewBag.CompleteTime = ConfigurationManager.AppSettings["maxTime"];
            ViewBag.Height = height;
            return View("ConfirmInfo");
        }

        //加载提交确认窗口
        public ActionResult GetSubmitConfirmView(int height)
        {
            ViewBag.Height = height;
            return View("SubmitConfirm");
        }

        //加载讨论框视图
        public ActionResult DiscussView(int height, int planId)
        {
            if (Session["userId"] == null) return View("~/Views/Login/Login.cshtml");
            var userId = Convert.ToInt32(Session["userId"]);
            //讨论组的信息
            var discussList = PlanBLL.GetDiscussList(planId);
            //登录者头像
            var userImg = PlanBLL.GetUserImg(userId);
            ViewBag.UserId = userId;
            ViewBag.UserImg = userImg;
            ViewBag.DiscussList = discussList;
            ViewBag.Height = height;
            return View("Discuss");
        }

        //获取子计划列表
        public string GetChildPlanList(int planId)
        {
            var childPlanList = PlanBLL.GetChildPlanList(planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, childPlanList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public ActionResult GetPlanInfo()
        {
            return View("PlanInfo");
        }

        //获取部门信息列表
        public string GetOrganizationInfo()
        {
            var departList = PlanBLL.GetDepartmentList();
            var jsonResult = new JsonResultModel(JsonResultType.success, departList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取常用联系人
        public string GetOfferUsers()
        {
            var text = Request.Form["text"];
            var personList = new List<UserInfo>();
            if (string.IsNullOrWhiteSpace(text))
            {
                int userId = Convert.ToInt32(Session["userId"]);
                //获取常用联系人列表
                personList = PlanBLL.GetUserIdListByUserId(userId);
            }
            else
            {
                personList = PlanBLL.SelectUserList(text, true);
            }
            FileUpload file = new FileUpload();
            //string path = file.ConfigPath(Convert.ToInt32(UploadFilePath.HeadImage));
            foreach (var item in personList)
            {
                if (!string.IsNullOrWhiteSpace(item.img))
                {
                    item.img = "/HeadImage/" + item.img;
                }
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, personList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取项目信息列表
        public string GetProjectInfo()
        {
            var projectList = PlanBLL.GetProjectList();
            var jsonResult = new JsonResultModel(JsonResultType.success, projectList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //组织信息获取===（最常用组织信息）
        //（快捷选择）
        public string GetOrgList(int userId)
        {
            userId = int.Parse(Session["userId"].ToString());
            //获取组织ID
            var orgIds = PlanBLL.GetOrgId(userId);
            List<OrganizationInfo> orgInfo = new List<OrganizationInfo>();

            foreach (var item in orgIds)
            {
                ViewBag.OrgInfo = PlanBLL.GetOrgListByOrgId(item.organizationId, ref orgInfo);
            }

            return "";
        }

        //组织信息获取
        public string GetAllOrgList()
        {
            //获取第一层
            var orgList = PlanBLL.GetAllOrg();//一级目录
            var listString = new List<string>();

            foreach (var item in orgList)
            {
                //根据上级organazationID 查询下一级与parentId相等的组织信息
                ViewBag.LastOrgaList = PlanBLL.GetLastOrgListByOrgId(item.organizationId, listString);
                if (item.withSub == true)
                {
                    break;
                }
            }

            return "";
        }

        //项目信息获取
        public string GetAllProList()
        {
            //获取第一层
            var proList = PlanBLL.GetAllPro();
            var listString = new List<string>();

            foreach (var item in proList)
            {
                //根据上级organazationID 查询下一级与parentId相等的组织信息
                ViewBag.LastOrgaList = PlanBLL.GetLastProListByProId(item.projectId, listString);
                if (item.withSub == true)
                {
                    break;
                }
            }

            return "";
        }

        //新建计划
        //public string SavePlan()
        //{
        //    int userId = Convert.ToInt32(Session["userId"]);
        //    PlanInfo planInfo = new PlanInfo();
        //    LoopPlanInfo loopPlanInfo = new LoopPlanInfo();

        //    //判断是否为新建计划
        //    if (planInfo.planId != 1)
        //    {
        //        planBll.AddPlan(userId, planInfo);
        //    }

        //    //判断是否为新建循环计划
        //    if (loopPlanInfo.loopId != 1)
        //    {
        //        planBll.AddLoopPlan(userId, loopPlanInfo);
        //    }

        //    //判断是否为新建子计划
        //    if (planInfo.planId != 1)
        //    {
        //    }

        //    return AjaxCallBack.OK;

        //}

        //根据时间获取状态数量
        public string GetStatusCountByTime(int year, int month, int operate)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var status = PlanBLL.GetPlanStatusInfo(year, month, userId, operate);
            var jsonResult = new JsonResultModel(JsonResultType.success, status, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //只修改状态的操作:撤销，修改
        public string ChangePlanStatus(int planId, int status, int flag)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var confirmUser = PlanBLL.ChangePlanStatus(planId, status, userId, flag);
            //申请修改成功，发送即时提醒
            if (flag == 1 && confirmUser != 0)
            {
                this.MBService(confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PAU, Session["userName"].ToString());
            }
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "申请修改失败!");
        }

        //申请修改循环计划
        public string UpdateLoopPlan(int loopId)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = PlanBLL.UpdateLoopPlan(loopId, userId);
            return ReturnJson(JsonResultType.success, flag, "申请修改失败!");
        }

        //分解操作
        public string Resolve()
        {
            var planList = Request.Form["planList"];
            var plans = JsonConvert.DeserializeObject<List<PlanInfo>>(planList);
            var userId = Convert.ToInt32(Session["userId"]);
            var parentPlanId = 0;
            if (plans.Count() > 0)
            {
                foreach (var item in plans)
                {
                    item.endTime = Convert.ToDateTime(item.oldEndTime);
                    item.progress = 0;
                    item.status = 20;
                    item.stop = 0;
                    item.archive = false;
                    item.createUser = userId;
                    item.createTime = DateTime.Now;
                    item.updateUser = userId;
                    item.updateTime = DateTime.Now;
                    item.initial = 0;
                    item.deleteFlag = false;
                    parentPlanId = item.parentPlan.Value;
                    PlanBLL.ResolvePlan(item, userId);
                }
            }
            //更改父计划withSub的值
            PlanBLL.UpdateParentPlan(parentPlanId, userId);
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "分解出错");
        }

        //计划提交
        public string SubmitPlan(int planId, int initial)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var confirm = PlanBLL.SubmitPlan(userId, planId, initial);
            var flag = false;
            //TODO: 计划标签处理
            if (confirm != 0)
            {
                flag = true;
                if (initial == 0)//审核提交
                {
                    //发送即时提醒
                    this.MBService(userId.ToString(), Protocol.ClientActualTimeProtocol.PNS);

                    //using (var mbService = new TaskRemindServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" +userId.ToString() + Protocol.ClientProtocol.PNS);
                    //}
                }
                else
                {
                    //发送即时提醒
                    this.MBService(confirm.ToString(), Protocol.ClientActualTimeProtocol.PUC, Session["userName"].ToString());

                    //using (var mbService = new TaskRemindServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirm.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.PUC);
                    //}
                }

                //保存一般计划标签
                TagManagementBLL.SavePlanTag(userId, planId);
            }
            else
            {
                flag = false;
            }
            return ReturnJson(JsonResultType.success, flag, "计划提交失败");
        }

        //循环计划提交
        public string SubmitLoopPlan(int loopId)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = PlanBLL.SubmitLoopPlan(loopId, userId);

            //TODO: 计划标签处理
            if (flag == true)
            {
                //保存循环计划标签
                TagManagementBLL.SavePlanTag(userId, loopId);
            }
            return ReturnJson(JsonResultType.success, flag, "计划提交失败");
        }

        /// <summary>
        /// 提交确认
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <param name="quantity">完成数量（自评）</param>
        /// <param name="time">完成时间（自评）</param>
        /// <returns></returns>
        public string Confirming(int planId, int quantity, int time)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            {
                var planConfirm = PlanBLL.Confirming(userId, planId, quantity, time);
                if (planConfirm.result.Equals(0))
                {
                    //发送即时提醒
                    this.MBService(planConfirm.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PUA, Session["userName"].ToString());

                    //using (var mbService = new TaskRemindServiceClient())
                    //{
                    //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + planConfirm.confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.PUA);
                    //}

                    //TODO: 附件预览处理

                    CustomHandler handler = new CustomHandler(PlanBLL.ConvertPreviewFileAsync);

                    string previewPath = System.Web.HttpContext.Current.Server.MapPath("~") + MB.New.Common.ConstVar.PreviewPath;
                    handler.BeginInvoke(previewPath, planId, null, null);

                    //PlanBLL.ConvertPreviewFileAsync(planId);

                    var jsonResult = new JsonResultModel(JsonResultType.success, null, "计划提交成功");
                    return JsonConvert.SerializeObject(jsonResult);
                }
                else if (planConfirm.result.Equals(1))
                {
                    var jsonResult = new JsonResultModel(JsonResultType.success, "查无此计划", "查无此计划");
                    return JsonConvert.SerializeObject(jsonResult);
                }
                else
                {
                    var jsonResult = new JsonResultModel(JsonResultType.success, "请上传附件", "请上传附件");
                    return JsonConvert.SerializeObject(jsonResult);
                }
            }
        }

        //循环计划提交确认
        public string ConfirmingLoopPlan(int loopId, int quantity)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            PlanBLL.ConfirmingLoopPlan(loopId, quantity, userId);
            return ReturnJson(JsonResultType.success, null, "提交失败");
        }

        /// <summary>
        /// 根据计划ID获取附件表信息
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <returns></returns>
        public string FileData(int planId)
        {
            List<PlanAttachment> attchList = PlanBLL.PlanFileData(planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, attchList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取循环计划附件信息
        public string LoopFileData(int planId)
        {
            var fileList = PlanBLL.GetLoopPlanFileData(planId);
            return ReturnJson(JsonResultType.success, fileList, "获取附件列表失败!");
        }

        //批量提交
        public string BatchSubmitPlan()
        {
            try
            {
                var userId = Convert.ToInt32(Session["userId"]);
                var planIds = JsonConvert.DeserializeObject<string[]>(Request.Form["planIdList"]);
                if (planIds.Length > 0)
                {
                    for (int i = 0; i < planIds.Length; i++)
                    {
                        var confirmUser = PlanBLL.SubmitPlan(userId, Convert.ToInt32(planIds[i]), 10);

                        //TODO: 计划标签处理
                        if (confirmUser != 0)
                        {
                            //发送即时提醒
                            this.MBService(confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PUC, Session["userName"].ToString());

                            //using (var mbService = new TaskRemindServiceClient())
                            //{
                            //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.PUC);
                            //}

                            //保存一般计划标签
                            TagManagementBLL.SavePlanTag(userId, Convert.ToInt32(planIds[i]));
                        }
                    }
                }
                return AjaxCallBack.OK;
            }
            catch
            {
                return AjaxCallBack.FAIL;
            }
        }

        //批量删除
        public string BatchDelete()
        {
            try
            {
                var userId = Convert.ToInt32(Session["userId"]);
                var planIds = JsonConvert.DeserializeObject<string[]>(Request.Form["planIdList"]);
                if (planIds.Length > 0)
                {
                    for (int i = 0; i < planIds.Length; i++)
                    {
                        PlanBLL.DeletePlan(Convert.ToInt32(planIds[i]), userId);
                    }
                    //TODO: 计划标签处理
                    //删除一般计划标签
                    TagManagementBLL.RemovePlanTag(userId, planIds.Select(p => Convert.ToInt32(p)).ToArray());
                }
                return AjaxCallBack.OK;
            }
            catch (Exception)
            {
                return AjaxCallBack.FAIL;
            }
        }

        //计划审核
        public string ExaminePlan(int planId, int importance, int urgency, int difficulty, bool isTrue, string message)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var sendUser = PlanBLL.ExaminePlan(planId, importance, urgency, difficulty, isTrue, userId, message);
            if (sendUser != 0)  //审核通过
            {
                //发送即时提醒
                this.MBService(sendUser.ToString(), Protocol.ClientActualTimeProtocol.PNS);

                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + sendUser.ToString() + "+" + Protocol.ClientProtocol.PNS);
                //}
            }
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "审核出错！");
        }

        //批量审核 isTrue=true : 审核通过  isTrue=false : 审核不通过
        public string batchExaminePlan(int importance, int urgency, int difficulty, bool isTrue, string message)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var planIds = JsonConvert.DeserializeObject<string[]>(Request.Form["planIdList"]);
            if (planIds.Length > 0)
            {
                for (int i = 0; i < planIds.Length; i++)
                {
                    var confirmUser = PlanBLL.ExaminePlan(Convert.ToInt32(planIds[i]), importance, urgency, difficulty, isTrue, userId, message);
                    if (confirmUser != 0)  //审核通过
                    {
                        //发送即时提醒
                        this.MBService(confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PNS);

                        //using (var mbService = new TaskRemindServiceClient())
                        //{
                        //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Protocol.ClientProtocol.PNS);
                        //}
                    }
                }
            }
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "批量审核出错！");
        }

        //计划确认 isTrue= true : 确认通过  isTrue = false : 确认不通过
        public string ConfirmPlan(int planId, string completeQuantity, string completeQuality, string completeTime, bool isTrue, string msg)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            PlanBLL.ConfirmPlan(planId, userId, Convert.ToDecimal(completeQuantity), Convert.ToDecimal(completeQuality), Convert.ToDecimal(completeTime), isTrue, msg);
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "计划确认出错");
        }

        //批量确认 isTrue=true : 确认通过-》完成  isTrue=false : 确认不通过
        public string batchConfirmPlan(List<int> planIdList, decimal completeQuantity, decimal completeQuality, decimal completeTime, int status, bool isTrue)
        {
            try
            {
                foreach (var planId in planIdList)
                {
                    var userId = Convert.ToInt32(Session["userId"]);
                    // planBll.ConfirmPlan(planId, completeQuantity, completeQuality, completeTime, status, isTrue);
                }
                return AjaxCallBack.OK;
            }
            catch
            {
                return AjaxCallBack.FAIL;
            }
        }

        //计划转办
        public string ChangeToDo(int planId, int responseUser, int confirmUser)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = PlanBLL.ChangePlan(planId, userId, responseUser, confirmUser);
            return ReturnJson(JsonResultType.success, flag, "转办失败");
        }

        //即时更新计划进程
        public string UpdateProcess(int planId, int newProcess)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            PlanBLL.UpdateProcess(userId, planId, newProcess);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //中止计划,开始计划:0、运行中   10、申请中止
        public string StopOrStartPlan(int planId, int stop)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var confirmUser = PlanBLL.StopOrStartPlan(planId, stop, userId);
            //申请中止，发送即时提醒
            if (stop == 10 && confirmUser != 0)
            {
                this.MBService(confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PAS, Session["userName"].ToString());
            }
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "申请中止失败");
        }

        //中止循环计划
        public string StopLoopPlan(int loopId)
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var flag = PlanBLL.StopLoopPlan(loopId, userId);
            return ReturnJson(JsonResultType.success, flag, "申请中止失败");
        }

        //计划筛选排序
        public ActionResult SelectOrSort(int pageIndex)
        {
            if (Session["userId"] == null)
            {
                return View("~/Views/Login/Login.cshtml");
            }
            var userId = Convert.ToInt32(Session["userId"]);
            string paramList = Request.Form["params"];
            Session["paramList"] = paramList;
            ConditionModel condition = JsonConvert.DeserializeObject<ConditionModel>(paramList);
            var planList = new List<PlanInfo>();
            var conditionNew = string.Empty;
            var conditionStart = DateTime.MinValue;
            var conditionEnd = DateTime.MaxValue;
            var coopCondition = string.Empty;
            //不是快捷筛选
            if (condition.soontype.Length <= 0)
            {
                #region 拼凑sql

                if ((condition.stop.Length > 0 && (condition.stop.Contains(10) || condition.stop.Contains(90))) || condition.status.Length > 0)
                {
                    conditionNew += " (";
                    for (int i = 0; i < condition.stop.Length; i++)
                    {
                        if (condition.stop[i] == 10 || condition.stop[i] == 90)
                        {
                            conditionNew += " stop==" + condition.stop[i] + " Or ";
                        }
                    }
                    if (condition.status.Length > 0)
                    {
                        for (int i = 0; i < condition.status.Length; i++)
                        {
                            conditionNew += " (status==" + condition.status[i] + " And stop==0) Or ";
                        }
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                if (condition.person.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += " (";
                    for (int i = 0; i < condition.person.Length; i++)
                    {
                        conditionNew += " responsibleUser==" + condition.person[i] + " Or confirmUser==" + condition.person[i] + " Or ";
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                if (condition.department.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += "(";
                    for (int i = 0; i < condition.department.Length; i++)
                    {
                        conditionNew += " organizationId==" + condition.department[i] + " Or ";
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                if (condition.project.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(conditionNew))
                    {
                        conditionNew += " And ";
                    }
                    conditionNew += "(";
                    for (int i = 0; i < condition.project.Length; i++)
                    {
                        conditionNew += " projectId==" + condition.project[i] + " Or ";
                    }
                    conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                }
                //筛选时间
                if (condition.time.Length > 0)
                {
                    if (condition.time[0] == "1")
                    {
                        conditionStart = DateTime.Now.AddDays(-6).Date;
                        conditionEnd = DateTime.Now.AddDays(1).Date;
                        //planList = planList.Where(p => p.endTime >= DateTime.Now.AddDays(-6).Date && p.endTime < DateTime.Now.AddDays(1).Date).ToList();
                    }
                    else if (condition.time[0] == "2")
                    {
                        conditionStart = DateTime.Now.AddMonths(-1).Date;
                        conditionEnd = DateTime.Now.AddDays(1).Date;
                        //conditionNew += " endTime>='" + DateTime.Now.AddMonths(-1).Date + "' And endTime <'" + DateTime.Now.AddDays(1).Date + "' ";
                        // planList = planList.Where(p => p.endTime >= DateTime.Now.AddMonths(-1).Date && p.endTime < DateTime.Now.AddDays(1).Date).ToList();
                    }
                    else
                    {
                        if (condition.time[2] == "")
                        {
                            conditionStart = Convert.ToDateTime(condition.time[1]).Date;
                            conditionEnd = DateTime.MaxValue;
                            //conditionNew += " endTime>='" + Convert.ToDateTime(condition.time[1]).Date + "' ";
                            //planList = planList.Where(p => p.endTime >= Convert.ToDateTime(condition.time[1]).Date).ToList();
                        }
                        else if (condition.time[1] == "")
                        {
                            conditionStart = DateTime.MinValue;
                            conditionEnd = Convert.ToDateTime(condition.time[2]).Date;
                            // conditionNew += " endTime<'" + Convert.ToDateTime(condition.time[2]).Date + "' ";
                            //planList = planList.Where(p => p.endTime < Convert.ToDateTime(condition.time[2]).AddDays(1).Date).ToList();
                        }
                        else
                        {
                            conditionStart = Convert.ToDateTime(condition.time[1]);
                            conditionEnd = Convert.ToDateTime(condition.time[2]).AddDays(1).Date;
                            // conditionNew += " endTime>='" + Convert.ToDateTime(condition.time[1]).Date + "' And endTime <'" + Convert.ToDateTime(condition.time[2]).AddDays(1).Date + "' ";
                            // planList = planList.Where(p => p.endTime >= Convert.ToDateTime(condition.time[1]).Date && p.endTime < Convert.ToDateTime(condition.time[2]).AddDays(1).Date).ToList();
                        }
                    }
                }

                coopCondition = conditionNew;
                if (!string.IsNullOrWhiteSpace(conditionNew) && condition.whoPlan != -1)
                {
                    conditionNew += " And ";
                }
                if (condition.whoPlan == 0)
                {
                    conditionNew += " responsibleUser==" + userId + " ";
                }
                else if (condition.whoPlan == 1)
                {
                    conditionNew += " confirmUser==" + userId + " ";
                }

                #endregion 拼凑sql

                //筛选自己或者下属的计划集合
                if (condition.whoPlan == 0)
                {
                    planList = PlanBLL.GetPlanList(userId, conditionNew, coopCondition, conditionStart, conditionEnd);
                    var planListNotNeed = planList.Where(p => ((p.status == 0 && p.stop == 0) || (p.status == 15 && p.stop == 0)) && p.responsibleUser != userId).ToList();
                    if (planListNotNeed.Count() > 0)
                    {
                        foreach (var item in planListNotNeed)
                        {
                            planList.Remove(item);
                        }
                    }
                }
                else if (condition.whoPlan == 1)
                {
                    planList = PlanBLL.GetPlanList(userId, conditionNew, "1", conditionStart, conditionEnd);
                    var planListNotNeed = planList.Where(p => (p.status == 0 && p.stop == 0) || (p.status == 15 && p.stop == 0)).ToList();
                    if (planListNotNeed.Count() > 0)
                    {
                        foreach (var item in planListNotNeed)
                        {
                            planList.Remove(item);
                        }
                    }
                }
                //计划管控
                else if (condition.whoPlan == -1)
                {
                    planList = PlanBLL.GetPlanList(userId, conditionNew, "1", conditionStart, conditionEnd);
                }
            }
            else
            {
                var flag = condition.soontype;
                if (condition.whoPlan == 0)
                {
                    conditionNew += " responsibleUser==" + userId + " ";
                }
                else if (condition.whoPlan == 1)
                {
                    conditionNew += " confirmUser==" + userId + " ";
                }

                var modelList = new List<PlanInfo>();
                if (flag.Length > 0)
                {
                    for (int i = 0; i < flag.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionNew) && condition.whoPlan != -1)
                        {
                            conditionNew += " And ";
                        }
                        conditionNew += " ( ";
                        if (flag[i] == 1)
                        {
                            conditionStart = DateTime.Now.Date;
                            conditionEnd = DateTime.Now.AddDays(1).Date;
                            conditionNew += "(status==20 Or status==40) And stop==0";
                            //planList = planList.Where(p => (p.endTime == null ? true : (p.endTime.Value.Year == DateTime.Now.Year && p.endTime.Value.Month == DateTime.Now.Month && p.endTime.Value.Day == DateTime.Now.Day)) && (((p.status == 20 || p.status == 40) && p.stop == 0 && !p.deleteFlag && p.isLoopPlan == 0) || (p.isLoopPlan == 1 && p.status == 20&&p.loopStatus.Value))).ToList();
                        }
                        else
                        {
                            conditionEnd = DateTime.Now;
                            conditionNew += "(status==20 Or status==40) And stop==0";
                            //planList = planList.Where(p => (p.endTime == null ? true : p.endTime < DateTime.Now) && (p.status == 20 || p.status == 40) && p.stop == 0 && !p.deleteFlag && p.isLoopPlan == 0).ToList();
                        }
                        conditionNew += " ) ";
                    }
                }
                planList = PlanBLL.GetPlanList(userId, conditionNew, "1", conditionStart, conditionEnd);
            }

            ////筛选结束后进行排序
            condition.sorts.Reverse();
            planList = PlanBLL.GetPlanListOrderByCustom(condition.sorts, planList);
            var pageCount = planList.Count();
            //获取分页数据
            var pageSize = PlanBLL.GetPageSize(userId);
            var currentPageCount = pageIndex * pageSize;
            if (currentPageCount > pageCount)
            {
                ViewBag.PageFlag = "false";  //已经到底的标志
            }
            else
            {
                ViewBag.PageFlag = "true"; //没有到底
            }
            planList = planList.Take(currentPageCount).ToList();
            if (currentPageCount > pageCount)
            {
                ViewBag.CurrentPageCount = pageCount;
            }
            else
            {
                ViewBag.CurrentPageCount = currentPageCount;
            }
            ViewBag.PageCount = pageCount;
            ViewBag.WhoPlan = condition.whoPlan;
            ViewBag.PlanList = planList;

            return View("PlanList");
        }

        public List<PlanInfo> SelectOrSortNpoi()
        {
            try
            {
                var userId = Convert.ToInt32(Session["userId"]);
                string paramList = Session["paramList"].ToString();

                ConditionModel condition = JsonConvert.DeserializeObject<ConditionModel>(paramList);
                var planList = new List<PlanInfo>();
                var conditionNew = string.Empty;
                var conditionStart = DateTime.MinValue;
                var conditionEnd = DateTime.MaxValue;
                var coopCondition = string.Empty;
                //不是快捷筛选
                if (condition.soontype.Length <= 0)
                {
                    #region 拼凑sql

                    if ((condition.stop.Length > 0 && (condition.stop.Contains(10) || condition.stop.Contains(90))) || condition.status.Length > 0)
                    {
                        conditionNew += " (";
                        for (int i = 0; i < condition.stop.Length; i++)
                        {
                            if (condition.stop[i] == 10 || condition.stop[i] == 90)
                            {
                                conditionNew += " stop==" + condition.stop[i] + " Or ";
                            }
                        }
                        if (condition.status.Length > 0)
                        {
                            for (int i = 0; i < condition.status.Length; i++)
                            {
                                conditionNew += " (status==" + condition.status[i] + " And stop==0) Or ";
                            }
                        }
                        conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                    }
                    if (condition.person.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionNew))
                        {
                            conditionNew += " And ";
                        }
                        conditionNew += " (";
                        for (int i = 0; i < condition.person.Length; i++)
                        {
                            conditionNew += " responsibleUser==" + condition.person[i] + " Or confirmUser==" + condition.person[i] + " Or ";
                        }
                        conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                    }
                    if (condition.department.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionNew))
                        {
                            conditionNew += " And ";
                        }
                        conditionNew += "(";
                        for (int i = 0; i < condition.department.Length; i++)
                        {
                            conditionNew += " organizationId==" + condition.department[i] + " Or ";
                        }
                        conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                    }
                    if (condition.project.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionNew))
                        {
                            conditionNew += " And ";
                        }
                        conditionNew += "(";
                        for (int i = 0; i < condition.project.Length; i++)
                        {
                            conditionNew += " projectId==" + condition.project[i] + " Or ";
                        }
                        conditionNew = conditionNew.Substring(0, conditionNew.LastIndexOf("Or")) + " ) ";
                    }
                    //筛选时间
                    if (condition.time.Length > 0)
                    {
                        if (condition.time[0] == "1")
                        {
                            conditionStart = DateTime.Now.AddDays(-6).Date;
                            conditionEnd = DateTime.Now.AddDays(1).Date;
                            //planList = planList.Where(p => p.endTime >= DateTime.Now.AddDays(-6).Date && p.endTime < DateTime.Now.AddDays(1).Date).ToList();
                        }
                        else if (condition.time[0] == "2")
                        {
                            conditionStart = DateTime.Now.AddMonths(-1).Date;
                            conditionEnd = DateTime.Now.AddDays(1).Date;
                            //conditionNew += " endTime>='" + DateTime.Now.AddMonths(-1).Date + "' And endTime <'" + DateTime.Now.AddDays(1).Date + "' ";
                            // planList = planList.Where(p => p.endTime >= DateTime.Now.AddMonths(-1).Date && p.endTime < DateTime.Now.AddDays(1).Date).ToList();
                        }
                        else
                        {
                            if (condition.time[2] == "")
                            {
                                conditionStart = Convert.ToDateTime(condition.time[1]).Date;
                                conditionEnd = DateTime.MaxValue;
                                //conditionNew += " endTime>='" + Convert.ToDateTime(condition.time[1]).Date + "' ";
                                //planList = planList.Where(p => p.endTime >= Convert.ToDateTime(condition.time[1]).Date).ToList();
                            }
                            else if (condition.time[1] == "")
                            {
                                conditionStart = DateTime.MinValue;
                                conditionEnd = Convert.ToDateTime(condition.time[2]).Date;
                                // conditionNew += " endTime<'" + Convert.ToDateTime(condition.time[2]).Date + "' ";
                                //planList = planList.Where(p => p.endTime < Convert.ToDateTime(condition.time[2]).AddDays(1).Date).ToList();
                            }
                            else
                            {
                                conditionStart = Convert.ToDateTime(condition.time[1]);
                                conditionEnd = Convert.ToDateTime(condition.time[2]).AddDays(1).Date;
                                // conditionNew += " endTime>='" + Convert.ToDateTime(condition.time[1]).Date + "' And endTime <'" + Convert.ToDateTime(condition.time[2]).AddDays(1).Date + "' ";
                                // planList = planList.Where(p => p.endTime >= Convert.ToDateTime(condition.time[1]).Date && p.endTime < Convert.ToDateTime(condition.time[2]).AddDays(1).Date).ToList();
                            }
                        }
                    }

                    coopCondition = conditionNew;
                    if (!string.IsNullOrWhiteSpace(conditionNew) && condition.whoPlan != -1)
                    {
                        conditionNew += " And ";
                    }
                    if (condition.whoPlan == 0)
                    {
                        conditionNew += " responsibleUser==" + userId + " ";
                    }
                    else if (condition.whoPlan == 1)
                    {
                        conditionNew += " confirmUser==" + userId + " ";
                    }

                    #endregion 拼凑sql

                    //筛选自己或者下属的计划集合
                    if (condition.whoPlan == 0)
                    {
                        planList = PlanBLL.GetPlanList(userId, conditionNew, coopCondition, conditionStart, conditionEnd);
                        var planListNotNeed = planList.Where(p => ((p.status == 0 && p.stop == 0) || (p.status == 15 && p.stop == 0)) && p.responsibleUser != userId).ToList();
                        if (planListNotNeed.Count() > 0)
                        {
                            foreach (var item in planListNotNeed)
                            {
                                planList.Remove(item);
                            }
                        }
                    }
                    else if (condition.whoPlan == 1)
                    {
                        planList = PlanBLL.GetPlanList(userId, conditionNew, "1", conditionStart, conditionEnd);
                        var planListNotNeed = planList.Where(p => (p.status == 0 && p.stop == 0) || (p.status == 15 && p.stop == 0)).ToList();
                        if (planListNotNeed.Count() > 0)
                        {
                            foreach (var item in planListNotNeed)
                            {
                                planList.Remove(item);
                            }
                        }
                    }
                    //计划管控
                    else if (condition.whoPlan == -1)
                    {
                        planList = PlanBLL.GetPlanList(userId, conditionNew, "1", conditionStart, conditionEnd);
                    }
                }
                else
                {
                    var flag = condition.soontype;
                    if (condition.whoPlan == 0)
                    {
                        conditionNew += " responsibleUser==" + userId + " ";
                    }
                    else if (condition.whoPlan == 1)
                    {
                        conditionNew += " confirmUser==" + userId + " ";
                    }

                    var modelList = new List<PlanInfo>();
                    if (flag.Length > 0)
                    {
                        for (int i = 0; i < flag.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(conditionNew) && condition.whoPlan != -1)
                            {
                                conditionNew += " And ";
                            }
                            conditionNew += " ( ";
                            if (flag[i] == 1)
                            {
                                conditionStart = DateTime.Now.Date;
                                conditionEnd = DateTime.Now.AddDays(1).Date;
                                conditionNew += "(status==20 Or status==40) And stop==0";
                                //planList = planList.Where(p => (p.endTime == null ? true : (p.endTime.Value.Year == DateTime.Now.Year && p.endTime.Value.Month == DateTime.Now.Month && p.endTime.Value.Day == DateTime.Now.Day)) && (((p.status == 20 || p.status == 40) && p.stop == 0 && !p.deleteFlag && p.isLoopPlan == 0) || (p.isLoopPlan == 1 && p.status == 20&&p.loopStatus.Value))).ToList();
                            }
                            else
                            {
                                conditionEnd = DateTime.Now;
                                conditionNew += "(status==20 Or status==40) And stop==0";
                                //planList = planList.Where(p => (p.endTime == null ? true : p.endTime < DateTime.Now) && (p.status == 20 || p.status == 40) && p.stop == 0 && !p.deleteFlag && p.isLoopPlan == 0).ToList();
                            }
                            conditionNew += " ) ";
                        }
                    }
                    planList = PlanBLL.GetPlanList(userId, conditionNew, "1", conditionStart, conditionEnd);
                }

                ////筛选结束后进行排序
                condition.sorts.Reverse();
                planList = PlanBLL.GetPlanListOrderByCustom(condition.sorts, planList);

                return planList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //获取前提计划
        public string GetFrontPlans(int planId)
        {
            var fronPlanList = PlanBLL.GetFrontPlan(planId);
            return JsonConvert.SerializeObject(fronPlanList);
        }

        //模糊查询计划
        public string GetSelectPlan()
        {
            PlanBLL planBll = new PlanBLL();
            var seachtext = Request.Form["key"];
            var planinfo = new List<PlanInfo>();
            if (!string.IsNullOrEmpty(seachtext))
            {
                planinfo = planBll.SelectFrontPlanList(seachtext.ToString());
            }
            //foreach (var item in planinfo)
            //{
            //    GetChildPlanList(item.planId);
            //}
            var jsonResult = new JsonResultModel(JsonResultType.success, planinfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //计划归类
        public string PlantoParentPlan(int ParentId)
        {
            List<PlanIdList> planidList = new List<PlanIdList>();
            planidList = JsonConvert.DeserializeObject<List<PlanIdList>>(Request.Form["planid"]);
            PlanBLL.GetplanToParentPlan(ParentId, planidList);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取计划详情
        public string GetPlanInfoByPlanId(int planId, int isloop, int withfront, int collPlan)
        {
            var planInfoNew = PlanBLL.GetPlanInfoByPlanId(planId, (isloop == 1 ? true : false), (withfront == 1 ? true : false), (collPlan == 1 ? true : false), null);
            var jsonResult = new JsonResultModel(JsonResultType.success, planInfoNew, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //保存计划
        public string SaveUpdatePlan()
        {
            var userId = Convert.ToInt32(Session["userId"]);
            var plans = Request.Form["plans"];
            var planList = JsonConvert.DeserializeObject<List<PlanInfo>>(plans);
            PlanBLL.SavePlans(planList, userId);
            return ReturnJson(JsonResultType.success, AjaxCallBack.OK, "操作失败");
        }

        //获取附件信息
        public string GetPlanAttachmentList(int planId)
        {
            var planAttachmentList = PlanBLL.GetPlanAttachmentList(planId);
            var jsonResult = new JsonResultModel(JsonResultType.success, planAttachmentList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取计划审核信息
        public string GetPlanCheckInfo(int planId)
        {
            var jsonResult = new JsonResultModel(JsonResultType.success, PlanBLL.GetPlanCheckInfo(planId), "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //筛选今日未完成或者超时计划:flag--1:今日未完成 2：超时计划
        public ActionResult GetUnCompleteOrOver()
        {
            if (Session["userId"] == null)
            {
                return View("~/Views/Login/Login.cshtml");
            }
            var userId = Convert.ToInt32(Session["userId"]);
            var terms = Request.Form["terms"];
            var flag = JsonConvert.DeserializeObject<string[]>(terms);
            var planList = PlanBLL.GetPlanList(userId, "responsibleUser==" + userId, string.Empty, DateTime.MinValue, DateTime.MaxValue);
            var planNewList = new List<PlanInfo>();
            var modelList = new List<PlanInfo>();
            if (planList.Count() > 0)
            {
                //计划列表执行默认排序
                planList = planList.OrderBy(p => p.completeTime).OrderByDescending(p => p.urgency).OrderByDescending(p => p.importance).ToList();
                if (flag.Length > 0)
                {
                    for (int i = 0; i < flag.Length; i++)
                    {
                        if (flag[i] == "1")
                        {
                            modelList = planList.Where(p => (p.endTime == null ? true : (p.endTime.Value.Year == DateTime.Now.Year && p.endTime.Value.Month == DateTime.Now.Month && p.endTime.Value.Day == DateTime.Now.Day)) && ((p.status != 90 && p.status != 30 && p.stop != 90 && p.isLoopPlan == 0) || (p.isLoopPlan == 1 && p.status != 20))).ToList();
                        }
                        else
                        {
                            modelList = planList.Where(p => (p.endTime == null ? true : p.endTime.Value < DateTime.Now) && (p.status == 20 || p.status == 40) && p.isLoopPlan == 0).ToList();
                        }
                        planNewList.AddRange(modelList);
                    }
                }
                else
                {
                    planNewList = planList;
                }
            }
            ViewBag.PlanList = planNewList;
            return View("PlanList");
        }

        ////根据重要度，紧急度和完成时间排序
        //public ActionResult OrderData(string orderField, string orderDirect)
        //{
        //    try
        //    {
        //        var orderList = new List<Order>();
        //        var orderModel = new Order();
        //        switch (orderField)
        //        {
        //            case "importance":
        //                orderModel.orderField = Order.OrderField.importance;
        //                break;
        //            case "urgency":
        //                orderModel.orderField = Order.OrderField.urgency;
        //                break;
        //            case "completeTime":
        //                orderModel.orderField = Order.OrderField.completeTime;
        //                break;
        //            default:
        //                break;
        //        }
        //        orderModel.flag = orderDirect == "true" ? true : false;
        //        orderList.Add(orderModel);
        //        var planList = planBll.GetPlanListOrderByCustom(orderList);
        //        ViewBag.PlanList = planList;
        //        return View("PlanList");

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //获取自己或者下属的计划列表,flag:1、自己  0、下属

        //选择自己的还是下属计划

        public ActionResult GetMeOrUnderPlans(int flag)
        {
            try
            {
                if (Session["userId"] == null)
                {
                    return View("~/Views/Login/Login.cshtml");
                }
                var userId = Convert.ToInt32(Session["userId"]);
                var planList = new List<PlanInfo>();
                if (flag == 1)
                {
                    planList = PlanBLL.GetPlanList(userId, string.Empty, string.Empty, DateTime.MinValue, DateTime.MaxValue);
                }
                else if (flag == 0)
                {
                    planList = PlanBLL.GetUnderPlanList(userId, string.Empty);
                }
                ViewBag.PlanList = planList;
                return View("PlanList");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetPlanConfirmInfo(int planId)
        {
            var model = PlanBLL.GetPlanConfirmInfo(planId);

            var jsonResult = new JsonResultModel(JsonResultType.success, model, "正常");
            return JsonConvert.SerializeObject(jsonResult);
            //var data = JsonConvert.SerializeObject(planBll.GetPlanConfirmInfo(planId));
            //return data;
        }

        //人员-模糊查询
        public ActionResult SelectUserList(string word)
        {
            ViewBag.userList = PlanBLL.SelectUserList(word, true);
            return View();
        }

        public FileStreamResult NpoiExcel()
        {
            List<PlanInfo> planList = new List<PlanInfo>();
            planList = SelectOrSortNpoi();
            List<string> OrgName = new List<string>();
            List<OrganizationInfo> OrgList = new List<OrganizationInfo>();
            //int count = 0;
            //int Cell = 0;
            //BindSubALLPlan(dt);
            string path = string.Format(HttpRuntime.AppDomainAppPath, "temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            XSSFSheet sheet = null;
            XSSFWorkbook workbook = null;
            using (FileStream fs = new FileStream(HttpRuntime.AppDomainAppPath + "Template\\工作任务MOD.xlsx", FileMode.Open))
            {
                workbook = new XSSFWorkbook(fs);
            }
            // 获取excel的第一个sheet
            sheet = (XSSFSheet)workbook.GetSheetAt(0);
            int row = 2;
            if (planList.Count != 0)
            {
                foreach (PlanInfo planinfo in planList)
                {
                    row++;
                    sheet.CopyRow(row - 1, row);
                    sheet.GetRow(row).Cells[0].SetCellValue(row - 2);
                    //项目名
                    sheet.GetRow(row).Cells[2].SetCellValue(PlanBLL.GetPorString(planinfo.projectId));
                    //组织名
                    sheet.GetRow(row).Cells[1].SetCellValue(PlanBLL.GetOrgString(planinfo.organizationId));
                    //OrgName = planBll.GetMoreOrgNameById(planinfo.organizationId, ref OrgList, ref count);
                    //foreach (string Name in OrgName)
                    //{
                    //    Cell++;
                    //    sheet.GetRow(row).Cells[Cell].SetCellValue(Name);
                    //}
                    //事项输出结果

                    sheet.GetRow(row).Cells[4].SetCellValue(planinfo.eventOutput);
                    //执行方式
                    sheet.GetRow(row).Cells[3].SetCellValue(planinfo.executionMode);
                    //责任人
                    sheet.GetRow(row).Cells[6].SetCellValue(planinfo.responsibleUserName);
                    //确认人
                    sheet.GetRow(row).Cells[7].SetCellValue(planinfo.confirmUserName);
                    //计划完成时间
                    if (planinfo.endTime != null)
                    {
                        sheet.GetRow(row).Cells[5].SetCellValue(DateTime.Parse(planinfo.endTime.ToString()).ToString("yyyy-MM-dd"));
                    }
                    //完成情况

                    sheet.GetRow(row).Cells[8].SetCellValue(planinfo.progress.ToString());
                    //计划状态
                    switch (planinfo.status)
                    {
                        case 0:
                            sheet.GetRow(row).Cells[9].SetCellValue("未提交");
                            break;

                        case 10:
                            sheet.GetRow(row).Cells[9].SetCellValue("审核中");
                            break;

                        case 15:
                            sheet.GetRow(row).Cells[9].SetCellValue("审核未通过");
                            break;

                        case 20:
                            sheet.GetRow(row).Cells[9].SetCellValue("审核通过");
                            break;

                        case 25:
                            sheet.GetRow(row).Cells[9].SetCellValue("申请修改");
                            break;

                        case 30:
                            sheet.GetRow(row).Cells[9].SetCellValue("等待确认");
                            break;

                        case 40:
                            sheet.GetRow(row).Cells[9].SetCellValue("确认未通过");
                            break;

                        case 90:
                            sheet.GetRow(row).Cells[9].SetCellValue("已完成");
                            break;
                    }
                    if (planinfo.effectiveTime != null && planinfo.completeQuality != null && planinfo.completeQuantity != null && planinfo.completeTime != null)
                    {
                        sheet.GetRow(row).Cells[10].SetCellValue(planinfo.effectiveTime.Value.ToString());
                    }
                    else
                    {
                        sheet.GetRow(row).Cells[10].SetCellValue("0.0");
                    }
                    if (planinfo.time != null && planinfo.quantity != null)
                    {
                        sheet.GetRow(row).Cells[11].SetCellValue((planinfo.time * planinfo.quantity).ToString());
                    }
                }
                sheet.ShiftRows(3, sheet.LastRowNum, -1);
            }
            //sheet.RemoveRow(sheet.GetRow(3));
            string CopyPaht = path + "temp\\计划事项管控表" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            FileStream ms = new FileStream(CopyPaht, FileMode.Create, FileAccess.ReadWrite);
            workbook.Write(ms);
            return File(new FileStream(CopyPaht, FileMode.Open), "application/octet-stream", HttpUtility.UrlEncode("计划事项管控表" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
        }

        //flag:0、存数据 1、导出
        public ActionResult NpoiExcelByPlanId(int flag)
        {
            List<int> planIdList = new List<int>();
            if (flag == 0)
            {
                var planIds = Request.Form["planIdList"];
                planIdList = JsonConvert.DeserializeObject<List<int>>(planIds);
                TempData["planIdList"] = planIdList;
                return Content("success");
            }
            else
            {
                planIdList = TempData["planIdList"] as List<int>;
            }
            List<PlanInfo> planList = new List<PlanInfo>();

            planList = PlanBLL.GetplanInfoByIdList(planIdList);
            List<string> OrgName = new List<string>();
            List<OrganizationInfo> OrgList = new List<OrganizationInfo>();
            //BindSubALLPlan(dt);
            string path = string.Format(HttpRuntime.AppDomainAppPath, "temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            XSSFSheet sheet = null;
            XSSFWorkbook workbook = null;
            using (FileStream fs = new FileStream(HttpRuntime.AppDomainAppPath + "Template\\工作任务MOD.xlsx", FileMode.Open))
            {
                workbook = new XSSFWorkbook(fs);
            }
            // 获取excel的第一个sheet
            sheet = (XSSFSheet)workbook.GetSheetAt(0);
            int row = 2;
            if (planList.Count != 0)
            {
                foreach (PlanInfo planinfo in planList)
                {
                    row++;
                    sheet.CopyRow(row - 1, row);
                    sheet.GetRow(row).Cells[0].SetCellValue(row - 2);
                    //项目名
                    sheet.GetRow(row).Cells[2].SetCellValue(PlanBLL.GetPorString(planinfo.projectId));
                    //组织名
                    sheet.GetRow(row).Cells[1].SetCellValue(PlanBLL.GetOrgString(planinfo.organizationId));
                    //OrgName = planBll.GetMoreOrgNameById(planinfo.organizationId, ref OrgList, ref count);
                    //foreach (string Name in OrgName)
                    //{
                    //    Cell++;
                    //    sheet.GetRow(row).Cells[Cell].SetCellValue(Name);
                    //}
                    //事项输出结果

                    sheet.GetRow(row).Cells[4].SetCellValue(planinfo.eventOutput);
                    //执行方式
                    sheet.GetRow(row).Cells[3].SetCellValue(planinfo.executionMode);
                    //责任人
                    sheet.GetRow(row).Cells[6].SetCellValue(planinfo.responsibleUserName);
                    //确认人
                    sheet.GetRow(row).Cells[7].SetCellValue(planinfo.confirmUserName);
                    //计划完成时间
                    if (planinfo.endTime != null)
                    {
                        sheet.GetRow(row).Cells[5].SetCellValue(DateTime.Parse(planinfo.endTime.ToString()).ToString("yyyy-MM-dd"));
                    }
                    //完成情况

                    sheet.GetRow(row).Cells[8].SetCellValue(planinfo.progress.ToString());
                    //计划状态
                    switch (planinfo.status)
                    {
                        case 0:
                            sheet.GetRow(row).Cells[9].SetCellValue("未提交");
                            break;

                        case 10:
                            sheet.GetRow(row).Cells[9].SetCellValue("审核中");
                            break;

                        case 15:
                            sheet.GetRow(row).Cells[9].SetCellValue("审核未通过");
                            break;

                        case 20:
                            sheet.GetRow(row).Cells[9].SetCellValue("审核通过");
                            break;

                        case 25:
                            sheet.GetRow(row).Cells[9].SetCellValue("申请修改");
                            break;

                        case 30:
                            sheet.GetRow(row).Cells[9].SetCellValue("等待确认");
                            break;

                        case 40:
                            sheet.GetRow(row).Cells[9].SetCellValue("确认未通过");
                            break;

                        case 90:
                            sheet.GetRow(row).Cells[9].SetCellValue("已完成");
                            break;
                    }
                    if (planinfo.effectiveTime != null && planinfo.completeQuality != null && planinfo.completeQuantity != null && planinfo.completeTime != null)
                    {
                        sheet.GetRow(row).Cells[10].SetCellValue(planinfo.effectiveTime.Value.ToString());
                    }
                    else
                    {
                        sheet.GetRow(row).Cells[10].SetCellValue("0.0");
                    }
                    if (planinfo.time != null && planinfo.quantity != null)
                    {
                        sheet.GetRow(row).Cells[11].SetCellValue((planinfo.time * planinfo.quantity).ToString());
                    }
                }
                sheet.ShiftRows(3, sheet.LastRowNum, -1);
            }
            //sheet.RemoveRow(sheet.GetRow(3));
            string CopyPaht = path + "temp\\计划事项管控表" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            FileStream ms = new FileStream(CopyPaht, FileMode.Create, FileAccess.ReadWrite);
            workbook.Write(ms);
            return File(new FileStream(CopyPaht, FileMode.Open), "application/octet-stream", HttpUtility.UrlEncode("计划事项管控表" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
        }

        //绑定执行方式
        public string GetExecutionList()
        {
            var Execution = PlanBLL.GetExecutionList();
            var jsonResult = new JsonResultModel(JsonResultType.success, Execution, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        // 获取计划日志
        public string GetPlanOperates(int planId, int isloop)
        {
            var jsonResult = new JsonResultModel(JsonResultType.success, PlanBLL.GetOperates(planId, isloop == 1 ? true : false), "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //上传普通计划附件
        [HttpPost]
        public string PlanUplpadMultipleFiles(int pid)
        {
            int userid = Convert.ToInt32(Session["userId"]);//用户ID
            List<PlanAttachment> planAttach = new List<PlanAttachment>();
            //int count = planAttach.Count;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0 || hpf == null)
                {
                    continue;
                }

                planAttach.Add(PlanBLL.UplpadMultipleFiles(hpf, pid, userid));
                //planAttach = planBll.UplpadMultipleFiles(hpf, pid, userid);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, planAttach, "正常");
            return ReturnJson(JsonResultType.success, planAttach, "上传出错");
        }

        //上传循环计划附件
        [HttpPost]
        public string LoopPlanUplpadMultipleFiles(int pid)
        {
            var userid = Convert.ToInt32(Session["userId"]);//用户ID
            var planAttach = new List<PlanAttachment>();
            var count = planAttach.Count;
            foreach (string file in Request.Files)
            {
                var hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0 || hpf == null)
                {
                    continue;
                }
                planAttach.Add(PlanBLL.LoopUplpadMultipleFiles(hpf, pid, userid));
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, planAttach, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 根据附件ID删除计划附件
        /// </summary>
        /// <param name="id">附件ID</param>
        [HttpPost]
        public void DeletePlanFileById(int id)
        {
            PlanBLL.DeleteFile(id);
        }

        /// <summary>
        /// 删除循环计划附件
        /// </summary>
        /// <param name="id">附件ID</param>
        [HttpPost]
        public void DeleteLoopPlanFileById(int id)
        {
            PlanBLL.DeleteLoopFile(id);
        }

        //添加评论
        public string AddDiscuss(string content, int planId)
        {
            int userId = Convert.ToInt32(Session["userId"]);//用户ID
            var discuss = PlanBLL.AddDiscuss(content, planId, userId);
            if (string.IsNullOrWhiteSpace(discuss.img))
            {
                discuss.img = "../../Images/common/portrait.png";
            }
            else
            {
                discuss.img = "/HeadImage/" + discuss.img + "?" + DateTime.Now;
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, discuss, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //添加回复
        public string AddReplySuggestion(int planId, int replyUser, string replyUserName, string content)
        {
            int userId = Convert.ToInt32(Session["userId"]);//用户ID
            var replyList = PlanBLL.AddReplySuggestion(planId, userId, replyUser, replyUserName, content);
            if (string.IsNullOrWhiteSpace(replyList.img))
            {
                replyList.img = "../../Images/common/portrait.png";
            }
            else
            {
                replyList.img = "/HeadImage/" + replyList.img + "?" + DateTime.Now;
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, replyList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //单文件下载:flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult Download(string displayName, string saveName, int flag)
        {
            var downLoadPath = Path.Combine(new FileUpload().ConfigPath(Convert.ToInt32(UploadFilePath.Plan)), saveName);
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

        //全部下载 flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult MultiDownload(int planId, int flag)
        {
            List<CompressInfo> lstFile = new List<CompressInfo>();
            var planAttachmentList = PlanBLL.GetPlanAttachmentList(planId);
            bool fileExists = false;
            string fileName = null;

            foreach (var item in planAttachmentList)
            {
                var downLoadPath = Path.Combine(new FileUpload().ConfigPath(Convert.ToInt32(UploadFilePath.Plan)), item.saveName);
                if (!fileExists && System.IO.File.Exists(downLoadPath))
                {
                    fileName = item.attachmentName;
                    fileExists = true;
                }
                lstFile.Add(new CompressInfo { path = downLoadPath, display = item.attachmentName });
            }
            if (!fileExists)
            {
                return JavaScript("noFile();");
            }
            if (flag == 0)
            {
                return Content("success");
            }

            SharpZipLibrary sharpZip = new SharpZipLibrary();
            //return File(sharpZip.Compress(lstFile), "application/octet-stream", Server.UrlEncode(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(string.Format("{0}等.zip", fileName)))));
            return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}等.zip", fileName), System.Text.Encoding.UTF8));
        }

        #region 删除协作人

        /// <summary>
        /// 删除协作人
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <param name="userId">协作人ID</param>
        public void DeletePlanCooperation(int planId, int userId)
        {
            PlanBLL.DeletePlanCooperation(planId, userId);
        }

        #endregion 删除协作人

        //删除评论
        public string DeleteComment(int suggestionId)
        {
            PlanBLL.DeleteComment(suggestionId);
            return AjaxCallBack.OK;
        }

        public string GetAddUserinfo()
        {
            AddUserInfo addUserinfo = PlanBLL.GetAddUserInfoByUserId(int.Parse(Session["UserId"].ToString()));
            var jsonResult = new JsonResultModel(JsonResultType.success, addUserinfo, "正常");
            return JsonConvert.SerializeObject(jsonResult);
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

        #endregion 私有方法
    }
}