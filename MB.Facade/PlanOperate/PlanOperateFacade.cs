using MB.DAL;
using MB.New.BLL.Plan;
using MB.New.BLL.Tag;
using MB.New.BLL.User;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.Facade.PlanOperate
{
    public class PlanOperateFacade : IPlanOperateFacade
    {
        #region 一般计划操作

        #region 计划附件预览转化处理

        /// <summary>
        /// 计划附件预览转化处理
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="previewPath"></param>
        /// <returns></returns>
        public void PlanAttachConvertAsync(int planId, string previewPath)
        {
            IPlanBLL Plan = new PlanBLL();
            var attachmentInfo = new List<FileInfoModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                attachmentInfo = Plan.GetPlanAttachInfoByPlanId(db, planId);
            }

            if (attachmentInfo == null || attachmentInfo.Count == 0) return;

            //取得附件的文件大小
            foreach (var item in attachmentInfo)
            {
                //文件格式处理
                if (item.extension.IndexOf(".") != 0) item.extension = "." + item.extension;

                //文件是否可转化
                if (FilePreview.IsConvertibleFile(item.extension))
                {
                    string filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, item.saveName);
                    item.fileSize = FileUtility.GetFileSize(filePath);
                }
            }
            //按文件大小从小到大排序
            attachmentInfo = attachmentInfo.OrderBy(x => x.fileSize).ToList();

            //附件预览转化处理
            foreach (var item in attachmentInfo)
            {
                FilePreview.ConvertFile(previewPath, EnumDefine.FileFolderType.PlanAttachment, item.saveName, item.extension);
            }
        }

        #endregion 计划附件预览转化处理

        #region 提交操作

        /// <summary>
        /// 提交操作
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="initial">true:临时计划 false：目标计划</param>
        /// <returns></returns>
        public void SubmitPlan(int userId, int planId, bool initial)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            ITagManagementBLL Tag = new TagManagementBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //提交计划处理
                Plan.SubmitPlanById(db, userId, planId, initial);

                //添加一般计划提交操作日志
                var submitLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Submit,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, submitLogModel);

                //临时计划的场合
                if (initial)
                {
                    //发送即时提醒
                    //this.MBService(confirmUser.ToString(), Protocol.ClientProtocol.PUC, Session["userName"].ToString());
                    //添加临时计划默认审核通过操作日志
                    var defaultCheckPassLogModel = new PlanLogModel
                    {
                        planId = planId,
                        message = "临时计划默认审核通过！",
                        type = EnumDefine.PlanOperateStatus.CheckPass,
                        operateUser = Plan.GetPlanInfoById(db, planId).confirmUser.Value,
                        operateTime = DateTime.Now,
                    };
                    PlanLog.InsPlanLog(db, defaultCheckPassLogModel);
                }
                db.SaveChanges();

                //保存标签至缓存
                Tag.SavePlanTagAsync(db, userId, planId);
            }
        }

        #endregion 提交操作

        #region 批量提交

        /// <summary>
        /// 批量提交计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planInfo">计划操作模型</param>
        public void SubmitMultiPlan(int userId, List<PagePlanOperateModel> planInfo)
        {
            foreach (var item in planInfo)
            {
                this.SubmitPlan(userId, item.planId, item.isInitial);

                //判断临时计划不推送服务提醒

                //if (confirmUser!=0)
                {
                    //发送即时提醒
                    //this.MBService(confirmUser.ToString(), Protocol.ClientProtocol.PUC, Session["userName"].ToString());

                    //if (confirmUser!=0)
                    {
                        //发送即时提醒
                        //this.MBService(confirmUser.ToString(), Protocol.ClientProtocol.PUC, Session["userName"].ToString());

                        //using (var mbService = new TaskRemindServiceClient())
                        //{
                        //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Session["userName"].ToString() + "+" + Protocol.ClientProtocol.PUC);
                        //}
                    }
                }
            }
        }

        #endregion 批量提交

        #region 删除计划

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="userId">用户Id</param>
        public void DeletePlan(int userId, int planId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            ITagManagementBLL Tag = new TagManagementBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //删除计划信息
                Plan.DeletePlan(db, userId, planId);

                //删除计划附件信息
                Plan.DelPlanAttachInfoByPlanId(db, planId);

                //取得附件信息
                var fileInfo = Plan.GetPlanAttachInfoByPlanId(db, planId);

                if (fileInfo.Count > 0 && fileInfo != null)
                {
                    foreach (var item in fileInfo)
                    {
                        //删除服务器上的附件
                        FileUtility.Delete(EnumDefine.FileFolderType.PlanAttachment, item.saveName);
                    }
                }

                //删除计划标签
                Tag.RemovePlanTagAsync(db, userId, planId);

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Delete,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);

                db.SaveChanges();
            }
        }

        #endregion 删除计划

        #region 批量删除计划

        /// <summary>
        /// 批量删除计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planIds"></param>
        public void DeleteMultiPlan(int userId, int[] planIds)
        {
            if (planIds.Length > 0)
            {
                foreach (var item in planIds)
                {
                    DeletePlan(userId, item);
                }
            }
        }

        #endregion 批量删除计划

        #region 计划转办

        /// <summary>
        /// 计划转办
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="responseUser">转办后的责任人</param>
        /// <param name="confirmUser">转办后的确认人</param>
        public void TurnPlan(int userId, int planId, int responsibleUser, int confirmUser)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            IUserBLL User = new UserBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.TurnPlan(db, userId, planId, responsibleUser, confirmUser);
                var turnUser = User.GetUserInfoById(db, userId).userName;
                var acceptUser = User.GetUserInfoById(db, responsibleUser).userName;

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = turnUser + "将该计划转办给了" + acceptUser,
                    type = EnumDefine.PlanOperateStatus.Change,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);

                db.SaveChanges();
            }
        }

        #endregion 计划转办

        #region 申请修改计划

        /// <summary>
        /// 修改状态的操作
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="userId">用户Id</param>
        public void AlterPlan(int userId, int planId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.AlterPlan(db, userId, planId);
                var planModel = Plan.GetPlanInfoById(db, planId);

                //申请修改操作日志
                var updatePlanLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Update,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, updatePlanLogModel);

                //临时计划的场合
                if (planModel.initial.Value == 0)
                {
                    //临时计划审核默认通过操作日志
                    var approvePlanLogModel = new PlanLogModel
                    {
                        planId = planId,
                        message = "临时计划默认审核通过！",
                        type = EnumDefine.PlanOperateStatus.UpdateCheckPass,
                        operateUser = planModel.confirmUser.Value,
                        operateTime = DateTime.Now
                    };
                    PlanLog.InsPlanLog(db, approvePlanLogModel);
                }

                db.SaveChanges();
            }
        }

        #endregion 申请修改计划

        #region 申请中止计划

        /// <summary>
        /// 中止计划
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public void StopPlan(int userId, int planId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.StopPlan(db, userId, planId);

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Stop,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 申请中止计划

        #region 开始计划

        /// <summary>
        /// 开始计划
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public void RestartPlan(int userId, int planId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.RestartPlan(db, userId, planId);

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.ReStart,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 开始计划

        #region 计划提交确认

        /// <summary>
        /// 计划提交确认
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="operateInfo"></param>
        public void SubmitConfirmPlan(int userId, PagePlanOperateModel operateInfo)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planModel = ModelMapping.JsonMapping<PagePlanOperateModel, PlanOperateModel>(operateInfo);
                Plan.SubmitConfirmPlan(db, userId, planModel);

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = operateInfo.planId,
                    message = operateInfo.msg,
                    type = EnumDefine.PlanOperateStatus.SubmitConfirm,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);

                db.SaveChanges();
            }
        }

        #endregion 计划提交确认

        #region 及时更新进程

        /// <summary>
        /// 即时更新计划进程
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="planId">计划Id</param>
        /// <param name="newProcess">进程</param>
        public void UpdateProgress(int userId, int planId, int Process)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.UpdateProgress(db, userId, planId, Process);

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.UpdateProcess,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 及时更新进程

        #region 审核操作

        /// <summary>
        /// 审核操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperateModel"></param>
        public void ApprovePlan(int userId, PagePlanOperateModel planOperateModel)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planModel = ModelMapping.JsonMapping<PagePlanOperateModel, PlanOperateModel>(planOperateModel);
                Plan.ApprovePlan(db, userId, planModel);
                var planInfo = Plan.GetPlanInfoById(db, planOperateModel.planId);
                var type = EnumDefine.PlanOperateStatus.CheckPass;
                //判断是否通过
                if (planOperateModel.isApprove)
                {
                    //申请中止场合
                    if (planInfo.stop == EnumDefine.PlanStopStatus.Checking)
                    {
                        type = EnumDefine.PlanOperateStatus.StopCheckPass;
                    }
                    //申请修改场合
                    else if (planInfo.status == EnumDefine.PlanStatus.RequestEdit)
                    {
                        type = EnumDefine.PlanOperateStatus.UpdateCheckPass;
                    }
                    //正常场合
                    else
                    {
                        type = EnumDefine.PlanOperateStatus.CheckPass;
                    }
                }
                else
                {
                    //申请中止场合
                    if (planInfo.stop == EnumDefine.PlanStopStatus.Checking)
                    {
                        type = EnumDefine.PlanOperateStatus.StopCheckNoPass;
                    }
                    //申请修改场合
                    else if (planInfo.status == EnumDefine.PlanStatus.RequestEdit)
                    {
                        type = EnumDefine.PlanOperateStatus.UpdateCheckNoPass;
                    }
                    //正常场合
                    else
                    {
                        type = EnumDefine.PlanOperateStatus.CheckNoPass;
                    }
                }

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planOperateModel.planId,
                    message = planOperateModel.msg,
                    type = type,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 审核操作

        #region 批量审核

        /// <summary>
        /// 批量审批计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="planOperateList">计划操作模型数组</param>

        public void ApproveMultiPlan(int userId, List<PagePlanOperateModel> planOperateList)
        {
            foreach (var item in planOperateList)
            {
                this.ApprovePlan(userId, item);
                //if (confirmUser != 0)  //审核通过
                //{
                //发送即时提醒
                //this.MBService(confirmUser.ToString(), Protocol.ClientProtocol.PNS);

                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + confirmUser.ToString() + "+" + Protocol.ClientProtocol.PNS);
                //}

                //}
            }
        }

        #endregion 批量审核

        #region 计划确认

        /// <summary>
        /// 计划确认
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="planOperate">计划操作模型</param>
        public void ConfirmPlan(int userId, PagePlanOperateModel planOperate)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planModel = ModelMapping.JsonMapping<PagePlanOperateModel, PlanOperateModel>(planOperate);
                planModel.progress = 100;
                Plan.ConfirmPlan(db, userId, planModel);
                var type = EnumDefine.PlanOperateStatus.ConfirmPass;
                //判断是否通过
                if (planOperate.isApprove)
                {
                    type = EnumDefine.PlanOperateStatus.ConfirmPass;
                }
                else
                {
                    type = EnumDefine.PlanOperateStatus.ConfirmNoPass;
                }
                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planOperate.planId,
                    message = planOperate.msg,
                    type = type,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 计划确认

        #region 计划撤销

        /// <summary>
        /// 撤销计划上次操作
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="planId">计划ID</param>
        /// <returns></returns>
        public void RevokePlan(int userId, int planId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.RevokePlan(db, userId, planId);
                var planModel = Plan.GetPlanInfoById(db, planId);

                //添加计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = planId,
                    message = string.Empty,
                    type = PlanLog.ReturnPlanOperateStatus(planModel.stop, planModel.status),
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 计划撤销

        #endregion 一般计划操作

        #region 循环计划操作

        /// <summary>
        /// 批量提交循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopIds">循环计划ID数组</param>
        public void SubmitMultiLoopPlan(int userId, int[] loopIds)
        {
            foreach (var item in loopIds)
            {
                this.SubmitLoopPlan(userId, item);
            }
        }

        /// <summary>
        /// 提交循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopId">循环计划ID</param>
        public void SubmitLoopPlan(int userId, int loopId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.SubmitLoopPlan(db, userId, loopId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.SubmitLoopPlan,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量删除循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopIds">循环计划ID数组</param>
        public void DeleteMultiLoopPlan(int userId, int[] loopIds)
        {
            foreach (var item in loopIds)
            {
                DeleteLoopPlan(userId, item);
            }
        }

        /// <summary>
        /// 删除循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopId">循环计划ID</param>
        public void DeleteLoopPlan(int userId, int loopId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            ITagManagementBLL Tag = new TagManagementBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //删除循环计划信息
                Plan.DeleteLoopPlan(db, userId, loopId);

                //删除循环计划附件信息
                Plan.DelLoopPlanAttachInfoByLoopId(db, loopId);

                //取得附件信息
                var fileInfo = Plan.GetLoopPlanAttachInfoByLoopId(db, loopId);

                if (fileInfo.Count > 0 && fileInfo != null)
                {
                    foreach (var item in fileInfo)
                    {
                        //删除服务器上的附件
                        FileUtility.Delete(EnumDefine.FileFolderType.PlanAttachment, item.saveName);
                    }
                }

                //删除循环计划标签
                Tag.RemoveLoopPlanTagAsync(db, userId, loopId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Delete,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 转办循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopId">循环计划Id</param>
        ///  <param name="responsibleUser">责任人</param>
        /// <param name="confirmUser">确认人</param>
        public void TurnLoopPlan(int userId, int loopId, int responsibleUser, int confirmUser)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            IUserBLL User = new UserBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.TurnLoopPlan(db, userId, loopId, responsibleUser, confirmUser);
                var turnUser = User.GetUserInfoById(db, userId).userName;
                var acceptUser = User.GetUserInfoById(db, responsibleUser).userName;

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = turnUser + "转办该计划给" + acceptUser,
                    type = EnumDefine.PlanOperateStatus.Change,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 申请修改循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopId">循环计划ID</param>
        public void AlterLoopPlan(int userId, int loopId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.AlterLoopPlan(db, userId, loopId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Update,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 申请中止循环计划
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="loopId">循环计划ID</param>
        public void StopLoopPlan(int userId, int loopId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.StopLoopPlan(db, userId, loopId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.Stop,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 重新开始循环计划
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="loopId">循环计划Id</param>
        public void RestartLoopPlan(int userId, int loopId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.RestartLoopPlan(db, userId, loopId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.ReStart,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 循环计划提交确认
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopPlanOperate">循环操作计划模型</param>
        public void SubmitConfirmLoopPlan(int userId, PageLoopPlanOperateModel loopPlanOperate)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var loopPlanModel = ModelMapping.JsonMapping<PageLoopPlanOperateModel, LoopPlanOperateModel>(loopPlanOperate);
                Plan.SubmitConfirmLoopPlan(db, userId, loopPlanModel);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopPlanOperate.loopId.Value,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.SubmitConfirm,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量审批循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopPlanOperate">循环操作计划模型</param>
        public void ApproveMultiLoopPlan(int userId, List<PageLoopPlanOperateModel> loopPlanOperate)
        {
            foreach (var item in loopPlanOperate)
            {
                this.ApproveLoopPlan(userId, item);
            }
        }

        /// <summary>
        /// 审批循环计划
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loopPlanOperate">循环操作计划模型</param>
        public void ApproveLoopPlan(int userId, PageLoopPlanOperateModel loopPlanOperate)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var loopPlanModel = ModelMapping.JsonMapping<PageLoopPlanOperateModel, LoopPlanOperateModel>(loopPlanOperate);
                Plan.ApproveLoopPlan(db, userId, loopPlanModel);

                //判断是否通过
                if (loopPlanOperate.isApprove)
                {
                    //添加循环计划操作日志
                    var planLogModel = new PlanLogModel
                    {
                        planId = loopPlanOperate.loopId.Value,
                        message = string.Empty,
                        type = EnumDefine.PlanOperateStatus.CheckPass,
                        operateUser = userId,
                        operateTime = DateTime.Now
                    };
                    PlanLog.InsLoopPlanLog(db, planLogModel);
                }
                else
                {
                    //添加循环计划操作日志
                    var planLogModel = new PlanLogModel
                    {
                        planId = loopPlanOperate.loopId.Value,
                        message = string.Empty,
                        type = EnumDefine.PlanOperateStatus.CheckNoPass,
                        operateUser = userId,
                        operateTime = DateTime.Now
                    };
                    PlanLog.InsLoopPlanLog(db, planLogModel);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 确认循环计划
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="loopPlanOperate">循环操作计划模型</param>
        public void ConfirmLoopPlan(int userId, PageLoopPlanOperateModel loopPlanOperate)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var loopPlanModel = ModelMapping.JsonMapping<PageLoopPlanOperateModel, LoopPlanOperateModel>(loopPlanOperate);
                Plan.ConfirmLoopPlan(db, userId, loopPlanModel);

                //判断是否通过
                if (loopPlanOperate.isApprove)
                {
                    //添加循环计划操作日志
                    var planLogModel = new PlanLogModel
                    {
                        planId = loopPlanOperate.loopId.Value,
                        message = string.Empty,
                        type = EnumDefine.PlanOperateStatus.ConfirmPass,
                        operateUser = userId,
                        operateTime = DateTime.Now
                    };
                    PlanLog.InsLoopPlanLog(db, planLogModel);
                }
                else
                {
                    //添加循环计划操作日志
                    var planLogModel = new PlanLogModel
                    {
                        planId = loopPlanOperate.loopId.Value,
                        message = string.Empty,
                        type = EnumDefine.PlanOperateStatus.ConfirmNoPass,
                        operateUser = userId,
                        operateTime = DateTime.Now
                    };
                    PlanLog.InsLoopPlanLog(db, planLogModel);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 撤销循环计划上次操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void RevokeLoopPlan(int userId, int loopId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.RevokeLoopPlan(db, userId, loopId);
                var LoopPlanModel = Plan.GetLoopPlanInfoById(db, loopId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = loopId,
                    message = string.Empty,
                    type = PlanLog.ReturnLoopPlanOperateStatus(LoopPlanModel.stop, LoopPlanModel.status),
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 撤销循环计划完成情况的上次操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="submitId"></param>
        public void RevokeLoopPlanSubmit(int userId, int submitId)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                Plan.RevokeLoopPlanSubmit(db, userId, submitId);

                //添加循环计划操作日志
                var planLogModel = new PlanLogModel
                {
                    planId = submitId,
                    message = string.Empty,
                    type = EnumDefine.PlanOperateStatus.CancelCheck,
                    operateUser = userId,
                    operateTime = DateTime.Now
                };
                PlanLog.InsLoopPlanLog(db, planLogModel);
                db.SaveChanges();
            }
        }

        #endregion 循环计划操作
    }
}