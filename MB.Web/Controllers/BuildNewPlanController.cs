using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;
using MB.Web.NotifyServiceReference;
using System.Threading.Tasks;
using MB.Facade.Plan;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class BuildNewPlanController : BaseController
    {
        private IPlanBLL PlanBLL { get; set; }
        private ITagManagementBLL TagManagementBLL { get; set; }
        private PlanFacade planFacade = new PlanFacade();
      

        //
        // GET: /BuildNewPlan/
        //PlanBLL planBll = new PlanBLL();
        public ActionResult _buildNewPlan()
        {
            //GetExecutionList();
            //GetOrgList();
            //GetAllProList();
            //GetUserByUserId();
            //GetIpUserByUserId();
            return View();
        }

        /// <summary>
        /// 绑定组织
        /// </summary>
        /// <returns></returns>
        public string GetOrgList()
        {
            //获取组织ID
            var departList = PlanBLL.GetDepartmentList();
            var jsonResult = new JsonResultModel(JsonResultType.success, departList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 绑定项目
        /// </summary>
        /// <returns></returns>
        public string GetAllProList()
        {
            var projectList = PlanBLL.GetProjectList();

            var jsonResult = new JsonResultModel(JsonResultType.success, projectList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 绑定下级及模糊查询
        /// </summary>
        /// <returns></returns>
        public string GetUserByUserId()
        {
            var seachtext = Request.Form["text"];
            var UserList = new List<UserInfo>();
            MB.Common.FileUpload file = new FileUpload();
            if (!string.IsNullOrEmpty(seachtext))
            {
                UserList = PlanBLL.SelectUserList(seachtext.ToString(), true);
            }
            else
            {
                UserList = PlanBLL.GetUserIdUpListByUserId(int.Parse(Session["userId"].ToString()));
            }
            foreach (var item in UserList)
            {
                if (!string.IsNullOrEmpty(item.img))
                {
                    //item.img = "../" + ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString() + "/" + item.img;
                    item.img = "/" + ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString() + "/" + item.img;
                }
                else
                {
                    item.img = "/Images/common/portrait.png";
                }
            }
            // ViewBag.MyDUser = UserList;
            var jsonResult = new JsonResultModel(JsonResultType.success, UserList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 绑定上级及模糊查询
        /// </summary>
        /// <returns></returns>
        public string GetIpUserByUserId()
        {
            var seachtext = Request.Form["text"];
            var UserList = new List<UserInfo>();
            MB.Common.FileUpload file = new FileUpload();
            if (!string.IsNullOrEmpty(seachtext))
            {
                UserList = PlanBLL.SelectUserList(seachtext.ToString(), true);
            }
            else
            {
                UserList = PlanBLL.GetUserIdDownListByUserId(int.Parse(Session["userId"].ToString()));
            }
            foreach (var item in UserList)
            {
                if (!string.IsNullOrEmpty(item.img))
                {
                    //item.img = "../" + ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString() + "/" + item.img;
                    item.img = "/" + ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString() + "/" + item.img;
                }
                else
                {
                    item.img = "/Images/common/portrait.png";
                }
            }
            //// ViewBag.MyUUser = UserList;
            var jsonResult = new JsonResultModel(JsonResultType.success, UserList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 绑定执行方式
        /// </summary>
        /// <returns></returns>
        public string GetExecutionList()
        {
            var Execution = PlanBLL.GetExecutionList();
            //   ViewBag.Execution = Execution;
            var jsonResult = new JsonResultModel(JsonResultType.success, Execution, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        public string GetXXUser()
        {
            var seachtext = Request.Form["text"];
            var UserList = new List<UserInfo>();
            if (!string.IsNullOrEmpty(seachtext))
            {
                UserList = PlanBLL.SelectUserList(seachtext.ToString(), true);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, UserList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 提交新建计划
        /// </summary>
        /// <param name="submit">0：保存计划、10：提交计划</param>
        /// <returns></returns>
        public string SavePlan(int submit)
        {
            PlanInfo planinfo = new PlanInfo();
            // CommonWorkTime commonWORK = new CommonWorkTime();
            var s = Request.Form["param"];
            var newPlan = JsonConvert.DeserializeObject<List<NewPlan>>(s);
            int userid = int.Parse(Session["userId"].ToString());

            //提交方法
            var result = PlanBLL.Save(newPlan, userid, submit);

            //TODO: 计划标签处理
            if (submit == 10)
            {
                newPlan.ForEach(p => {
                    //临时计划
                    if (p.isTmp.Value==0)
                    {
                        this.MBService(userid.ToString(), Protocol.ClientActualTimeProtocol.PNS);
                    }
                    else
                    {
                        //发送即时提醒
                        this.MBService(p.confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PUC, Session["userName"].ToString());
                    }
                });
                foreach (var item in result)
                {

                    //var confirmUser = PlanBLL.GetPlanById(item.Key);
                    //发送即时提醒
                    //this.MBService(confirmUser.ToString(), Protocol.ClientProtocol.PUC, Session["userName"].ToString());
                    //一般计划
                    if (item.Value == 0)
                    {
                        //保存一般计划标签
                        TagManagementBLL.SavePlanTag(userid, item.Key);
                    }
                    else
                    {
                        //保存循环计划标签
                        TagManagementBLL.SaveLoopPlanTag(userid, item.Key);
                    }
                }
            }
            else
            {
                //发送即时提醒

                this.MBService(userid.ToString(), Protocol.ClientActualTimeProtocol.PUS);
               

            }

            var msg = submit == 0 ? "计划保存成功" : "计划提交成功";
            var jsonResult = new JsonResultModel(JsonResultType.success, null, msg);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //计划提交
        public string SubmitPlan(int planId, int statues)
        {
            var userId = Convert.ToInt32(Session["userId"]);
           var confirmUser= PlanBLL.SubmitPlan(userId, planId, statues);

           if (confirmUser != 0)
           {
               if (statues == 0)//审核提交
               {
                   //发送即时提醒
                   this.MBService(userId.ToString(), Protocol.ClientActualTimeProtocol.PNS);
                   //using (var mbService = new MBServiceClient())
                   //{
                   //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + userId.ToString() + "+"+Protocol.ClientProtocol.PNS);
                   //}

                   //using (var mbService = new TaskRemindServiceClient())
                   //{
                   //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + userId.ToString() + Protocol.ClientProtocol.PNS);
                   //}

               }
               else
               {
                   //发送即时提醒

                   this.MBService(confirmUser.ToString(), Protocol.ClientActualTimeProtocol.PUC, Session["userName"].ToString());
                   //using (var mbService = new MBServiceClient())
                   //{
                   //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.PUC);
                   //}

                   //using (var mbService = new TaskRemindServiceClient())
                   //{
                   //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.PUC);
                   //}

               }
           }

            //TODO: 计划标签处理
            //保存一般计划标签
            TagManagementBLL.SavePlanTag(userId, planId);

            return AjaxCallBack.OK;
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