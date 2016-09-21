using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.DAL;
using FW.TaskRemindService.Common;
using FW.TaskRemindService.Model;

namespace FW.TaskRemindService
{
    public class TaskRemindBLL
    {
        #region 登录验证
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回用户Id</returns>
        public int UserLogin(string userName, string userPass)
        {
            //获取用户信息
            var userInfo = this.UserLogin(userName);
            if (userInfo == null) return 0;
            return userInfo.password == EncryptHelper.PwdEncrypt(userPass, userInfo.randomKey) ? userInfo.userId : 0;
        }
        #endregion

        #region 根据用户名获取用户登录信息
        /// <summary>
        /// 根据用户名获取用户登录信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户信息</returns>
        private UserLoginModel UserLogin(string userName)
        {
            var user = new UserLoginModel();
            using (var db = new TargetNavigationDBEntities())
            {
                user = (from u in db.tblUser
                        where u.userName == userName && !u.deleteFlag
                        select new UserLoginModel
                        {
                            userId = u.userId,
                            userName = u.userName,
                            password = u.password,
                            randomKey = u.randomKey
                        }).FirstOrDefault();
            }
            return user;
        }
        #endregion

        #region 根据用户Id获取首次登录计划消息
        /// <summary>
        /// 根据用户Id获取首次登录计划消息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>首次登录计划信息</returns>
        public string GetClientFirstLoginInfo(int userId)
        {
            var clientMsg = string.Empty;
            var today = DateTime.Now.Date;
            using (var db = new TargetNavigationDBEntities())
            {
                var planList = db.tblPlan.Where(p => !p.deleteFlag && p.responsibleUser == userId && p.endTime.Value.Year == today.Year && p.endTime.Value.Month == today.Month && p.endTime.Value.Day == today.Day).ToList();
                //今天已经存在计划
                if (planList.Count() > 0)
                {
                    //拼凑今天待完成计划的事项输出结果
                    var planEventInfo = string.Empty;
                    planList.ForEach(p =>
                    {
                        if (!string.IsNullOrWhiteSpace(planEventInfo))
                        {
                            planEventInfo += "*";
                        }
                        planEventInfo += p.eventOutput;
                    });
                    //拼接发送包
                    clientMsg = this.AssemSendMsg(new string[]{
                        Protocol.ClientLoginProtocol.PHC,userId.ToString(),planEventInfo
                    });
                }
                else
                {
                    clientMsg = this.AssemSendMsg(new string[]{
                        Protocol.ClientLoginProtocol.PUB,userId.ToString()
                    });
                }
            }
            return clientMsg;
        }
        #endregion

        #region 获取客户端明日待办事项
        /// <summary>
        /// 获取客户端明日待办事项
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetClientTomorrowMsg(int userId)
        {
            var tomorrowMsg = string.Empty;
            using (var db = new TargetNavigationDBEntities())
            {
                //计划未提交

                //计划未审核

                //计划未提交确认

                //计划未确认

                //目标未提交

                //目标为审核

                //目标为提交确认

                //目标未确认

                //流程未提交

                //流程为审核
            }
            return tomorrowMsg;
        }
        #endregion

        #region 获取客户端最新版本号
        /// <summary>
        /// 获取客户端最新版本号
        /// </summary>
        /// <returns></returns>
        public string GetNewestVersionNum()
        {
            var newestVersionNum = string.Empty;
            using (var db = new TargetNavigationDBEntities())
            {
                var newestVersion = db.tblVersion.Where(p => p.flag == 0).OrderByDescending(a => a.updateTime).FirstOrDefault();
                if (newestVersion != null) newestVersionNum = newestVersion.number;
            }
            return newestVersionNum;
        }
        #endregion

        #region 根据用户Id获取客户端下班前计划信息
        /// <summary>
        /// 根据用户Id获取客户端下班前计划信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public string GetClientBeforeOLMsg(int userId)
        {
            var clientMsg = string.Empty;
            var today = DateTime.Now.Date;
            using (var db = new TargetNavigationDBEntities())
            {
                //今日计划列表
                var planList = db.tblPlan.Where(p => !p.deleteFlag && p.responsibleUser == userId && p.endTime.Value.Year == today.Year && p.endTime.Value.Month == today.Month && p.endTime.Value.Day == today.Day).ToList();
                //当天没有提计划
                if (planList.Count() <= 0)
                {
                    clientMsg = Protocol.ClientBeforeOLProtocol.PNB;
                }
                //有计划
                else
                {
                    var hasCompletedPlans = planList.Where(p => p.status == 90 && p.stop == 0).ToList();
                    //有工时
                    if (hasCompletedPlans.Count() > 0)
                    {
                        //计算有效工时之和
                        var workTime = this.GetTotalWorkTime(hasCompletedPlans);
                        //不足8小时
                        if (workTime < 480)
                        {
                            clientMsg = Protocol.ClientBeforeOLProtocol.TUE;
                        }
                        //满8小时，不足12小时
                        else if (workTime >= 480 && workTime < 720)
                        {
                            clientMsg = Protocol.ClientBeforeOLProtocol.THE;
                        }
                        //超过12小时
                        else
                        {
                            clientMsg = Protocol.ClientBeforeOLProtocol.TOE;
                        }
                    }
                    //无工时
                    else
                    {
                        var unSubmitPlans = planList.Where(p => (p.status == 20 || p.status == 40) && p.stop == 0).ToList();
                        if (unSubmitPlans.Count > 0)
                        {
                            var planEventInfo = string.Empty;
                            unSubmitPlans.ForEach(p =>
                            {
                                if (!string.IsNullOrWhiteSpace(planEventInfo))
                                {
                                    planEventInfo += "*";
                                }
                                planEventInfo += p.eventOutput;
                            });
                            clientMsg = this.AssemSendMsg(new string[]{
                               Protocol.ClientBeforeOLProtocol.PNC,planEventInfo
                           });
                        }
                    }
                }
            }
            return string.IsNullOrWhiteSpace(clientMsg)?clientMsg:this.AssemSendMsg(new string[] { Protocol.OperateProtocol.OWR, clientMsg });
        }
        #endregion

        #region 拼接发送包
        /// <summary>
        /// 拼接发送包
        /// </summary>
        /// <param name="msg">信息集合</param>
        /// <returns></returns>
        public string AssemSendMsg(string[] msg)
        {
            var sendMsg = new StringBuilder();
            if (msg.Length > 0)
            {
                foreach (var item in msg)
                {
                    if (sendMsg.Length > 0)
                    {
                        sendMsg.Append("+");
                    }
                    sendMsg.Append(item);
                }
            }
            return sendMsg.ToString();
        }
        #endregion

        #region 获取客户端随机请求信息
        /// <summary>
        /// 获取客户端随机请求信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="type">1、昨日工时 2、本周工时 3、计划未审核 4、计划未确认</param>
        /// <returns></returns>
        public string GetClientRandomInfo(int userId, int type)
        {
            var clientMsg = string.Empty;
            using (var db = new TargetNavigationDBEntities())
            {
                switch (type)
                {
                    case (int)ConstVar.RequestType.YWorkTime:
                        var yesterday = DateTime.Now.AddDays(-1).Date;
                        if (yesterday.DayOfWeek != DayOfWeek.Sunday)
                        {
                            var plans = db.tblPlan.Where(p => !p.deleteFlag && p.responsibleUser == userId && p.endTime.Value.Year == yesterday.Year && p.endTime.Value.Month == yesterday.Month && p.endTime.Value.Day == yesterday.Day && p.status == 90 && p.stop == 0).ToList();
                            if (plans.Count > 0)
                            {
                                //计算出昨日总工时
                                var workTime = this.GetTotalWorkTime(plans);
                                //工时不足
                                if (workTime < 480)
                                {
                                    clientMsg = Protocol.ClientRandomProtocol.TYN;
                                }
                            }
                            else
                            {
                                //工时为零
                                clientMsg = Protocol.ClientRandomProtocol.TYZ;
                            }
                        }

                        break;
                    case (int)ConstVar.RequestType.WWorkTime:
                        var thisweekMonday = DateUtility.GetWeekFirstDay(DateTime.Now);
                        var thisWeekNum = DateUtility.GetWeekOfYear(thisweekMonday);
                        var thisweekWorkTime = (from p in db.vOrgPersonWeekWorkTime
                                                where p.responsibleUser == userId && p.weekOfYear == thisWeekNum
                                                select new
                                                {
                                                    weekWorkTime = p.workTimes
                                                }).ToList().Sum(s => s.weekWorkTime);
                        decimal weekAvgWorkTime = 0;
                        //获取周平均
                        if (thisweekWorkTime != null && DateTime.Today.DayOfWeek != 0)
                        {
                            weekAvgWorkTime = Math.Round((decimal)thisweekWorkTime / (int)DateTime.Today.DayOfWeek, 1);
                        }
                        if (weekAvgWorkTime <= 0)
                        {
                            clientMsg = Protocol.ClientRandomProtocol.TWZ;
                        }
                        else if (weekAvgWorkTime < 480)
                        {
                            clientMsg = Protocol.ClientRandomProtocol.TWN;
                        }
                        break;
                    case (int)ConstVar.RequestType.unCheck:
                        var planUnCheck = db.tblPlan.Where(p => !p.deleteFlag && p.confirmUser == userId && (((p.status == 10 || p.status == 25) && p.stop == 0) || p.stop == 10));
                        if (planUnCheck.Count() > 0)
                        {
                            clientMsg = Protocol.ClientRandomProtocol.PUC;
                        }
                        break;
                    case (int)ConstVar.RequestType.unConfirm:
                        var planUnConfirm = db.tblPlan.Where(p => !p.deleteFlag && p.confirmUser == userId && p.status == 30 && p.stop == 0);
                        if (planUnConfirm.Count() > 0)
                        {
                            clientMsg = Protocol.ClientRandomProtocol.PUA;
                        }
                        break;
                    default:
                        break;
                }
            }
            return string.IsNullOrWhiteSpace(clientMsg)?clientMsg: this.AssemSendMsg(new string[] { Protocol.OperateProtocol.CAM, clientMsg });
        }
        #endregion

        #region 根据计划列表计算出总工时
        /// <summary>
        /// 根据计划列表计算出总工时
        /// </summary>
        /// <param name="plans">计划列表</param>
        /// <returns></returns>
        private decimal GetTotalWorkTime(List<tblPlan> plans)
        {
            return plans.Sum(p => p.quantity.Value * p.time.Value * p.completeQuality.Value * p.completeQuantity.Value * p.completeTime.Value);
        }
        #endregion
    }
}
