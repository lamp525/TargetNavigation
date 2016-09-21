using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MB.New.BLL.Plan
{
    public class PlanBLL : IPlanBLL
    {
        #region 一般计划

        #region 取得计划协作人信息

        /// <summary>
        /// 取得计划协作人信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<UserInfoSimpleModel> GetPlanPartnerInfo(TargetNavigationDBEntities db, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = (from pc in db.tblPlanCooperation
                          join u in db.tblUser
                          on pc.userId equals u.userId
                          where pc.planId == planId
                          select new UserInfoSimpleModel
                          {
                              userId = u.userId,
                              userName = u.userName,
                              headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                          }).ToList();

            return result;
        }

        #endregion 取得计划协作人信息

        #region 添加计划协作人

        /// <summary>
        /// 添加计划协作人信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="partnerId"></param>
        /// <param name="planId"></param>
        public void InsPlanPartnerInfo(TargetNavigationDBEntities db, int partnerId, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var collplanModel = new tblPlanCooperation
            {
                planId = planId,
                userId = partnerId
            };
            db.tblPlanCooperation.Add(collplanModel);
        }

        /// <summary>
        /// 删除协作人
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        public void DeletePlanPartnerInfo(TargetNavigationDBEntities db, int planId)
        {
            var hasModel = db.tblPlanCooperation.Where(p => p.planId == planId).ToList();
            if (hasModel != null)
            {
                foreach (var item in hasModel)
                    db.tblPlanCooperation.Remove(item);
            }
        }

        #endregion 添加计划协作人

        #region 取得用户最新计划信息

        /// <summary>
        /// 取得用户最新计划信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlanInfoModel GetLatestPlanInfo(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            PlanInfoModel result = null;

            //取得用户最新的计划信息
            var info = db.vPlanList.Where(x => x.responsibleUser == userId && x.deleteFlag != true).OrderByDescending(x => x.updateTime).FirstOrDefault();

            if (info != null)
            {
                result = new PlanInfoModel
                {
                    organizationId = info.organizationId,
                    executionModeId = info.executionModeId,
                    executionMode = info.executionMode,
                    responsibleUser = info.responsibleUser,
                    responsibleUserName = info.responsibleUserName,
                    responsibleUserImage = string.IsNullOrEmpty(info.responsibleUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + info.responsibleUserImage,
                    confirmUser = info.confirmUser,
                    confirmUserName = info.confirmUserName,
                    confirmUserImage = string.IsNullOrEmpty(info.confirmUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + info.confirmUserImage,
                    initial = info.initial
                };
            }

            return result;
        }

        #endregion 取得用户最新计划信息

        #region 获取我的计划列表

        /// <summary>
        /// 获取我的计划列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo">筛选集合</param>
        /// <returns></returns>
        public List<PlanInfoModel> GetMyPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 取得我的计划列表
            var planList = GetPlanListByCondition(db, searchInfo, EnumDefine.PlanListType.Mine);

            return planList;
        }

        #endregion 获取我的计划列表

        #region 取得我的计划状态数量

        /// <summary>
        /// 取得我的计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public PlanStatusCountModel GetMyPlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetPlanStatusInfo(db, EnumDefine.PlanListType.Mine, userId, fromTime, toTime);
        }

        #endregion 取得我的计划状态数量

        #region 取得下属计划状态数量

        /// <summary>
        /// 取得下属计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public PlanStatusCountModel GetSubordinatePlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetPlanStatusInfo(db, EnumDefine.PlanListType.Subordinate, userId, fromTime, toTime);
        }

        #endregion 取得下属计划状态数量

        #region 根据文件ID取得附件信息

        /// <summary>
        /// 根据文件ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileInfoModel GetPlanAttachInfoByFileId(TargetNavigationDBEntities db, int fileId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var fileModel = (from attach in db.tblPlanAttachment
                             where attach.attachmentId == fileId && !attach.deleteFlag
                             select new FileInfoModel
                             {
                                 uploadUserId = attach.createUser,
                                 targetId = attach.planId,
                                 fileId = attach.attachmentId,
                                 displayName = attach.displayName,
                                 saveName = attach.saveName,
                                 extension = attach.extension,
                                 isPreviewable = attach.isPreviewable
                                 //filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, attach.saveName)
                             }).FirstOrDefault();

            fileModel.filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, fileModel.saveName);

            return fileModel;
        }

        #endregion 根据文件ID取得附件信息

        #region 根据计划ID取得附件信息

        /// <summary>
        /// 根据计划ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<FileInfoModel> GetPlanAttachInfoByPlanId(TargetNavigationDBEntities db, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planFileList = (from attach in db.tblPlanAttachment
                                where attach.planId == planId && !attach.deleteFlag
                                orderby attach.displayName
                                select new FileInfoModel
                                {
                                    uploadUserId = attach.createUser,
                                    targetId = attach.planId,
                                    fileId = attach.attachmentId,
                                    displayName = attach.displayName,
                                    saveName = attach.saveName,
                                    extension = attach.extension,
                                    isPreviewable = attach.isPreviewable
                                    //filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, attach.saveName)
                                }).ToList();

            planFileList.ForEach(x => x.filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, x.saveName));

            return planFileList;
        }

        #endregion 根据计划ID取得附件信息

        #region 根据计划ID取得详情

        /// <summary>
        /// 根据计划ID取得详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        public PlanInfoModel GetPlanInfoById(TargetNavigationDBEntities db, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = db.vPlanList.Where(p => p.planId == planId).Select(p =>
                                new PlanInfoModel
                                {
                                    planId = p.planId,
                                    executionModeId = p.executionModeId,
                                    organizationId = p.organizationId,
                                    responsibleOrganization = p.responsibleOrganization,
                                    responsibleUser = p.responsibleUser,
                                    responsibleUserName = p.responsibleUserName,
                                    responsibleUserImage = string.IsNullOrEmpty(p.responsibleUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + p.responsibleUserImage,
                                    confirmUserImage = string.IsNullOrEmpty(p.confirmUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + p.confirmUserImage,
                                    confirmOrganization = p.confirmOrganization,
                                    confirmUser = p.confirmUser,
                                    confirmUserName = p.confirmUserName,
                                    eventOutput = p.eventOutput,
                                    endTime = p.endTime,
                                    workTime = p.workTime,
                                    comment = p.comment,
                                    alert = p.alert,
                                    importance = p.importance,
                                    urgency = p.urgency,
                                    difficulty = p.difficulty,
                                    progress = p.progress,
                                    quantity = p.quantity,
                                    time = p.time,
                                    completeQuantity = p.completeQuantity,
                                    completeQuality = p.completeQuality,
                                    completeTime = p.completeTime,
                                    status = (EnumDefine.PlanStatus)p.status,
                                    stop = (EnumDefine.PlanStopStatus)p.stop,
                                    createTime = p.createTime,
                                    updateTime = p.updateTime,
                                    initial = p.initial,
                                    withSub = p.withSub,
                                    archive = p.archive,
                                    archiveTime = p.archiveTime,
                                    autoStart = p.autoStart,
                                    withFront = p.withFront,
                                    executionMode = p.executionMode,
                                    isAttach = p.isAttach,
                                    result = p.result,
                                    keywordInfo = p.keyword,
                                }).FirstOrDefault();
            return planInfo;
        }

        #endregion 根据计划ID取得详情

        #region 删除附件信息

        /// <summary>
        /// 删除计划附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachId"></param>
        public void DelPlanAttachInfoByAttachId(TargetNavigationDBEntities db, int attachId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var planAttach = db.tblPlanAttachment.Where(p => p.attachmentId == attachId && !p.deleteFlag).FirstOrDefault();
            if (planAttach != null) db.tblPlanAttachment.Remove(planAttach);
        }

        #endregion 删除附件信息

        #region 删除计划附件信息

        /// <summary>
        /// 删除计划附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        public void DelPlanAttachInfoByPlanId(TargetNavigationDBEntities db, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planAttachs = db.tblPlanAttachment.Where(p => p.planId == planId && !p.deleteFlag);
            if (planAttachs.Any())
            {
                foreach (var attach in planAttachs)
                {
                    db.tblPlanAttachment.Remove(attach);
                }
            }
        }

        #endregion 删除计划附件信息

        #region 取得下属计划列表信息

        /// <summary>
        /// 取得下属计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PlanInfoModel> GetSubordinatePlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 取得下属计划列表
            var planList = GetPlanListByCondition(db, searchInfo, EnumDefine.PlanListType.Subordinate);

            return planList;
        }

        #endregion 取得下属计划列表信息

        #region 提交计划

        /// <summary>
        /// 提交计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="isInitial">true:临时计划 false：目标计划</param>
        public void SubmitPlanById(TargetNavigationDBEntities db, int userId, int planId, bool isInitial)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            //临时计划
            if (isInitial)
            {
                planInfo.status = (int)EnumDefine.PlanStatus.CheckPassed;
                planInfo.importance = 1;
                planInfo.urgency = 1;
                planInfo.difficulty = 1;
                planInfo.auditTime = DateTime.Now;
            }
            //目标计划
            else
            {
                planInfo.status = (int)EnumDefine.PlanStatus.Checking;
                planInfo.auditTime = null;
            }
            planInfo.planGenerateTime = DateTime.Now;
            planInfo.progress = 0;
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 提交计划

        #region 删除计划

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public void DeletePlan(TargetNavigationDBEntities db, int userId, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            planInfo.deleteFlag = true;
            planInfo.updateTime = DateTime.Now;
            planInfo.updateUser = userId;
        }

        #endregion 删除计划

        #region 转办计划

        /// <summary>
        /// 转办计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="responsibleUser"></param>
        /// <param name="confirmUser"></param>
        public void TurnPlan(TargetNavigationDBEntities db, int userId, int planId, int responsibleUser, int confirmUser)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            //修改责任人和确认人
            planInfo.responsibleUser = responsibleUser;
            planInfo.confirmUser = confirmUser;
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 转办计划

        #region 申请修改计划

        /// <summary>
        /// 申请修改计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public void AlterPlan(TargetNavigationDBEntities db, int userId, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            //判断临时计划
            if (planInfo.initial.Value == 0)
            {
                planInfo.status = (int)EnumDefine.PlanStatus.UnSubmit;
            }
            //一般计划
            else if (planInfo.initial == 1)
            {
                planInfo.status = (int)EnumDefine.PlanStatus.RequestEdit;
            }
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 申请修改计划

        #region 申请中止

        /// <summary>
        /// 申请中止
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public void StopPlan(TargetNavigationDBEntities db, int userId, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            planInfo.stop = (int)EnumDefine.PlanStopStatus.Checking;
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 申请中止

        #region 重新开始计划

        /// <summary>
        /// 重新开始计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public void RestartPlan(TargetNavigationDBEntities db, int userId, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            planInfo.stop = (int)EnumDefine.PlanStopStatus.Running;
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 重新开始计划

        #region 提交确认

        /// <summary>
        /// 提交确认
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        public void SubmitConfirmPlan(TargetNavigationDBEntities db, int userId, PlanOperateModel planOperate)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (planOperate == null)
            {
                throw new ArgumentNullException("planOperate");
            }

            //非结果类事项
            if (planOperate.isAttach != null && !planOperate.isAttach.Value)
            {
                var planAttachs = db.tblPlanAttachment.Where(p => p.planId == planOperate.planId && !p.deleteFlag);
                if (planAttachs.Any())
                {
                    foreach (var attach in planAttachs)
                    {
                        db.tblPlanAttachment.Remove(attach);
                        //服务器上删除
                        FileUtility.Delete(EnumDefine.FileFolderType.PlanAttachment, attach.saveName);
                    }
                }
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planOperate.planId);
            if (planInfo == null) return;
            planInfo.status = (int)EnumDefine.PlanStatus.Confirming;
            planInfo.quantity = planOperate.quantity;
            planInfo.time = planOperate.time;
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
            planInfo.progress = planOperate.progress;
            planInfo.submitTime = DateTime.Now;
            planInfo.confirmTime = null;
            planInfo.result = planOperate.result;
            planInfo.isAttach = planOperate.isAttach;
        }

        #endregion 提交确认

        #region 更新计划进度

        /// <summary>
        /// 更新计划进度
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="progress">计划进度</param>
        /// <returns></returns>
        public void UpdateProgress(TargetNavigationDBEntities db, int userId, int planId, int progress)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;
            planInfo.progress = progress;
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 更新计划进度

        #region 审批计划

        /// <summary>
        /// 审批计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        public void ApprovePlan(TargetNavigationDBEntities db, int userId, PlanOperateModel planOperate)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planOperate.planId);
            if (planInfo == null) return;
            //审批通过
            if (planOperate.isApprove)
            {
                //申请中止场合
                if (planInfo.stop == (int)EnumDefine.PlanStopStatus.Checking)
                {
                    planInfo.stop = (int)EnumDefine.PlanStopStatus.Stopped;
                }
                //申请修改场合
                else if (planInfo.status == (int)EnumDefine.PlanStatus.RequestEdit)
                {
                    planInfo.status = (int)EnumDefine.PlanStatus.UnSubmit;
                    planInfo.importance = 0;
                    planInfo.urgency = 0;
                    planInfo.difficulty = 0;
                    planInfo.progress = 0;
                }
                //正常场合
                else
                {
                    planInfo.importance = planOperate.importance;
                    planInfo.urgency = planOperate.urgency;
                    planInfo.difficulty = planOperate.difficulty;
                    planInfo.status = (int)EnumDefine.PlanStatus.CheckPassed;
                }
            }
            //不通过
            else
            {
                //申请中止场合
                if (planInfo.stop == (int)EnumDefine.PlanStopStatus.Checking)
                {
                    planInfo.stop = (int)EnumDefine.PlanStopStatus.Running;
                }
                //申请修改场合
                else if (planInfo.status == (int)EnumDefine.PlanStatus.RequestEdit)
                {
                    planInfo.status = (int)EnumDefine.PlanStatus.CheckPassed;
                }
                //正常场合
                else
                {
                    planInfo.status = (int)EnumDefine.PlanStatus.CheckDenied;
                }
            }
            planInfo.updateTime = DateTime.Now;
            planInfo.auditTime = DateTime.Now;
            planInfo.updateUser = userId;
        }

        #endregion 审批计划

        #region 确认计划

        /// <summary>
        /// 确认计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        public void ConfirmPlan(TargetNavigationDBEntities db, int userId, PlanOperateModel planOperate)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planOperate.planId);
            if (planInfo == null) return;

            //确认通过
            if (planOperate.isApprove)
            {
                planInfo.completeQuantity = planOperate.completeQuantity;
                planInfo.completeQuality = planOperate.completeQuality;
                planInfo.completeTime = planOperate.completeTime;
                planInfo.progress = planOperate.progress;
                planInfo.status = (int)EnumDefine.PlanStatus.Complete;
            }
            //不通过
            else
            {
                planInfo.status = (int)EnumDefine.PlanStatus.ConfirmDenied;
            }
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
            planInfo.confirmTime = DateTime.Now;
        }

        #endregion 确认计划

        #region 撤销计划上次操作

        /// <summary>
        /// 撤销计划上次操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <param name="status"></param>
        public void RevokePlan(TargetNavigationDBEntities db, int userId, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planInfo = GetPlanInfoByIdForUpd(db, planId);
            if (planInfo == null) return;

            switch (planInfo.stop)
            {
                //申请中止的场合
                case (int)EnumDefine.PlanStopStatus.Checking:
                    planInfo.stop = (int)EnumDefine.PlanStopStatus.Running;
                    break;
                //其他场合
                case (int)EnumDefine.PlanStopStatus.Running:
                    switch (planInfo.status)
                    {
                        //待审核
                        case (int)EnumDefine.PlanStatus.Checking:
                        //审核通过
                        case (int)EnumDefine.PlanStatus.CheckPassed:
                            planInfo.status = (int)EnumDefine.PlanStatus.UnSubmit;
                            break;
                        //申请修改
                        case (int)EnumDefine.PlanStatus.RequestEdit:
                            planInfo.status = (int)EnumDefine.PlanStatus.CheckPassed;
                            break;
                        //待确认
                        case (int)EnumDefine.PlanStatus.Confirming:
                            planInfo.status = (int)EnumDefine.PlanStatus.CheckPassed;
                            planInfo.time = 0;
                            planInfo.quantity = 0;
                            break;

                        default:
                            break;
                    }
                    break;
            }
            planInfo.updateUser = userId;
            planInfo.updateTime = DateTime.Now;
        }

        #endregion 撤销计划上次操作

        #region 添加计划附件

        /// <summary>
        /// 添加计划附件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachInfo"></param>
        /// <returns>附件ID</returns>
        public int InsPlanAttachInfo(TargetNavigationDBEntities db, FileInfoModel attachInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var attachId = DBUtility.GetPrimaryKeyByTableName(db, "tblPlanAttachment");
            var tblAttach = new tblPlanAttachment
            {
                attachmentId = attachId,
                planId = attachInfo.targetId,
                displayName = attachInfo.displayName,
                saveName = attachInfo.saveName,
                extension = attachInfo.extension,
                isPreviewable = attachInfo.isPreviewable,
                createUser = attachInfo.uploadUserId.Value,
                createTime = DateTime.Now,
                updateUser = attachInfo.uploadUserId.Value,
                updateTime = DateTime.Now,
                deleteFlag = false
            };
            db.tblPlanAttachment.Add(tblAttach);
            return attachId;
        }

        #endregion 添加计划附件

        #region 用户计划完成情况

        /// <summary>
        /// 用户计划完成情况
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public PlanCompleteCountModel GetUserPlanCompleteInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var resultModel = new PlanCompleteCountModel();
            //排除已中止的计划
            var condition = new StringBuilder().Append(" deleteFlag==false AND responsibleUser== ").Append(userId).Append(" AND stop!= ").Append((int)EnumDefine.PlanStopStatus.Stopped);
            var values = new List<object>();

            //结束时间
            if (fromTime != null)
            {
                condition.Append(" AND endTime>=@");
                condition.Append(values.Count);
                values.Add(fromTime.Value);
            }
            // 结束时间
            if (toTime != null)
            {
                condition.Append(" AND endTime<=@");
                condition.Append(values.Count);
                values.Add(toTime.Value);
            }
            var planList = db.tblPlan.Where(condition.ToString(), values.ToArray()).ToList();

            if (planList.Count > 0)
            {
                //已完成数量
                resultModel.completeCount = planList.Where(p => p.status == (int)EnumDefine.PlanStatus.Complete && p.stop == (int)EnumDefine.PlanStopStatus.Running).Count();
                //未完成数量
                resultModel.notCompleteCount = planList.Count - resultModel.completeCount;
            }
            return resultModel;
        }

        #endregion 用户计划完成情况

        #region 更新计划

        /// <summary>
        /// 更新计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        /// <returns>计划ID</returns>
        public void UpdPlanInfo(TargetNavigationDBEntities db, PlanInfoModel planInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (planInfo == null)
            {
                throw new ArgumentNullException("planInfo");
            }

            var info = db.tblPlan.Where(p => p.planId == planInfo.planId).FirstOrDefault();

            if (info == null) return;

            info.parentPlan = planInfo.parentPlan;
            info.organizationId = planInfo.organizationId;
            info.executionModeId = planInfo.executionModeId;
            info.eventOutput = planInfo.eventOutput;
            info.responsibleOrganization = planInfo.responsibleOrganization;
            info.responsibleUser = planInfo.responsibleUser;
            info.confirmOrganization = planInfo.confirmOrganization;
            info.confirmUser = planInfo.confirmUser;
            info.startTime = planInfo.startTime;
            info.endTime = planInfo.endTime;
            info.workTime = planInfo.workTime;
            info.comment = planInfo.comment;
            info.alert = planInfo.alert;
            info.importance = planInfo.importance;
            info.urgency = planInfo.urgency;
            info.difficulty = planInfo.difficulty;
            info.progress = planInfo.progress;
            info.quantity = planInfo.quantity;
            info.time = planInfo.time;
            info.completeQuantity = planInfo.completeQuantity;
            info.completeQuality = planInfo.completeQuality;
            info.completeTime = planInfo.completeTime;
            info.meetingId = planInfo.meetingId;
            info.status = (int)planInfo.status;
            info.stop = (int)planInfo.stop;
            info.initial = planInfo.initial;
            info.withSub = planInfo.withSub;
            info.archive = planInfo.archive;
            info.archiveTime = planInfo.archiveTime;
            info.autoStart = planInfo.autoStart;
            info.updateUser = planInfo.updateUser;
            info.updateTime = planInfo.updateTime;
            info.planGenerateTime = planInfo.planGenerateTime;
            info.auditTime = planInfo.auditTime;
            //标签
            info.keyword = planInfo.keyword != null && planInfo.keyword.Length > 0 ? string.Join(",", planInfo.keyword) : null;
        }

        #endregion 更新计划

        #region 添加计划

        /// <summary>
        /// 添加计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        /// <returns>计划ID</returns>
        public int InsPlanInfo(TargetNavigationDBEntities db, PlanInfoModel planInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (planInfo == null)
            {
                throw new ArgumentNullException("planInfo");
            }

            //计划信息表新增
            var planId = DBUtility.GetPrimaryKeyByTableName(db, "tblPlan");
            var planModel = new tblPlan
            {
                planId = planId,
                parentPlan = planInfo.parentPlan,
                organizationId = planInfo.organizationId,
                executionModeId = planInfo.executionModeId,
                eventOutput = planInfo.eventOutput,
                responsibleOrganization = planInfo.responsibleOrganization,
                responsibleUser = planInfo.responsibleUser,
                confirmOrganization = planInfo.confirmOrganization,
                confirmUser = planInfo.confirmUser,
                startTime = planInfo.startTime,
                endTime = planInfo.endTime,
                workTime = planInfo.workTime,
                comment = planInfo.comment,
                alert = planInfo.alert,
                importance = planInfo.importance,
                urgency = planInfo.urgency,
                difficulty = planInfo.difficulty,
                progress = planInfo.progress,
                quantity = planInfo.quantity,
                time = planInfo.time,
                completeQuantity = planInfo.completeQuantity,
                completeQuality = planInfo.completeQuality,
                completeTime = planInfo.completeTime,
                meetingId = planInfo.meetingId,
                status = (int)planInfo.status,
                stop = (int)planInfo.stop,
                initial = planInfo.initial,
                withSub = planInfo.withSub,
                archive = planInfo.archive,
                archiveTime = planInfo.archiveTime,
                autoStart = planInfo.autoStart,
                createUser = planInfo.createUser,
                createTime = planInfo.createTime,
                updateUser = planInfo.updateUser,
                updateTime = planInfo.updateTime,
                planGenerateTime = planInfo.planGenerateTime,
                auditTime = planInfo.auditTime,
                //标签
                keyword = planInfo.keyword != null && planInfo.keyword.Length > 0 ? string.Join(",", planInfo.keyword) : null
            };

            db.tblPlan.Add(planModel);
            return planId;
        }

        #endregion 添加计划

        #region 取得我的一般计划分组信息

        /// <summary>
        /// 取得我的一般计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PlanGroupInfoModel> GetMyPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetPlanGourpInfo(db, userId, groupType, EnumDefine.PlanListType.Mine);
        }

        #endregion 取得我的一般计划分组信息

        #region 取得下属一般计划分组信息

        /// <summary>
        /// 取得下属一般计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PlanGroupInfoModel> GetSubordinatePlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetPlanGourpInfo(db, userId, groupType, EnumDefine.PlanListType.Subordinate);
        }

        #endregion 取得下属一般计划分组信息

        #region 取得执行方法列表信息

        /// <summary>
        /// 取得执行方法列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<ExecutionInfoModel> GetExecutionInfo(TargetNavigationDBEntities db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.tblExecutionMode.Where(x => x.deleteFlag != true).Select(x => new ExecutionInfoModel
            {
                executionId = x.executionId,
                executionMode = x.executionMode
            }).ToList();

            return result;
        }

        #endregion 取得执行方法列表信息

        #endregion 一般计划

        #region 循环计划

        #region 取得用户最新循环计划信息

        /// <summary>
        /// 取得用户最新循环计划信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public LoopPlanInfoModel GetLatestLoopPlanInfo(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            LoopPlanInfoModel result = null;

            //取得用户最新的计划信息
            var info = db.vLoopPlanList.Where(x => x.responsibleUser == userId && x.deleteFlag != true).OrderByDescending(x => x.updateTime).FirstOrDefault();

            if (info != null)
            {
                result = new LoopPlanInfoModel
                {
                    organizationId = info.organizationId,
                    executionModeId = info.executionModeId,
                    executionMode = info.executionMode,
                    responsibleUser = info.responsibleUser,
                    responsibleUserName = info.responsibleUserName,
                    responsibleUserImage = string.IsNullOrEmpty(info.responsibleUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + info.responsibleUserImage,
                    confirmUser = info.confirmUser,
                    confirmUserName = info.confirmUserName,
                    confirmUserImage = string.IsNullOrEmpty(info.confirmUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + info.confirmUserImage,
                };
            }

            return result;
        }

        #endregion 取得用户最新循环计划信息

        #region 删除循环计划附件信息

        /// <summary>
        /// 删除循环计划附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachId"></param>
        public void DelLoopPlanAttachInfoByAttachId(TargetNavigationDBEntities db, int attachId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var loopAttach = db.tblLoopplanAttachment.Where(p => p.attachmentId == attachId && !p.deleteFlag).FirstOrDefault();
            if (loopAttach != null) db.tblLoopplanAttachment.Remove(loopAttach);
        }

        #endregion 删除循环计划附件信息

        #region 根据循环计划提交ID删除附件信息

        /// <summary>
        /// 根据循环计划提交ID删除附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="submitId"></param>
        public void DelLoopPlanAttachInfoBySubmitId(TargetNavigationDBEntities db, int submitId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopAttachs = db.tblLoopplanAttachment.Where(p => p.submitId == submitId && !p.deleteFlag);
            //删除附件
            DelLoopPlanAttachInfo(db, loopAttachs);
        }

        #endregion 根据循环计划提交ID删除附件信息

        #region 根据循环计划ID删除附件信息

        /// <summary>
        /// 根据循环计划ID删除附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        public void DelLoopPlanAttachInfoByLoopId(TargetNavigationDBEntities db, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopAttachs = db.tblLoopplanAttachment.Where(p => p.loopId == loopId && !p.deleteFlag);
            //删除附件
            DelLoopPlanAttachInfo(db, loopAttachs);
        }

        #endregion 根据循环计划ID删除附件信息

        #region 根据文件ID取得附件信息

        /// <summary>
        /// 根据文件ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileInfoModel GetLoopPlanAttachInfoByFileId(TargetNavigationDBEntities db, int fileId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var fileModel = (from attach in db.tblLoopplanAttachment
                             where attach.attachmentId == fileId && !attach.deleteFlag
                             select new FileInfoModel
                             {
                                 uploadUserId = attach.createUser,
                                 targetId = attach.loopId,
                                 fileId = attach.attachmentId,
                                 displayName = attach.displayName,
                                 saveName = attach.saveName,
                                 extension = attach.extension,
                                 isPreviewable = attach.isPreviewable
                                 //filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, attach.saveName)
                             }).FirstOrDefault();

            fileModel.filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, fileModel.saveName);

            return fileModel;
        }

        #endregion 根据文件ID取得附件信息

        #region 根据循环计划ID取得附件信息

        /// <summary>
        /// 根据循环计划ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public List<FileInfoModel> GetLoopPlanAttachInfoByLoopId(TargetNavigationDBEntities db, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopFileList = (from attach in db.tblLoopplanAttachment
                                where attach.loopId == loopId && !attach.deleteFlag
                                orderby attach.displayName
                                select new FileInfoModel
                                {
                                    uploadUserId = attach.createUser,
                                    targetId = attach.loopId,
                                    fileId = attach.attachmentId,
                                    displayName = attach.displayName,
                                    saveName = attach.saveName,
                                    extension = attach.extension,
                                    isPreviewable = attach.isPreviewable
                                    //filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, attach.saveName)
                                }).ToList();

            loopFileList.ForEach(x => x.filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, x.saveName));

            return loopFileList;
        }

        #endregion 根据循环计划ID取得附件信息

        #region 根据循环计划提交ID取得附件信息

        /// <summary>
        /// 根据循环计划提交ID取得附件信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="submitId"></param>
        /// <returns></returns>
        public List<FileInfoModel> GetLoopPlanAttachInfoBySubmitId(TargetNavigationDBEntities db, int submitId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopFileList = (from attach in db.tblLoopplanAttachment
                                where attach.submitId == submitId && !attach.deleteFlag
                                orderby attach.displayName
                                select new FileInfoModel
                                {
                                    uploadUserId = attach.createUser,
                                    targetId = attach.loopId,
                                    fileId = attach.attachmentId,
                                    displayName = attach.displayName,
                                    saveName = attach.saveName,
                                    extension = attach.extension,
                                    isPreviewable = attach.isPreviewable,
                                    // filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, attach.saveName)
                                }).ToList();

            loopFileList.ForEach(x => x.filePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.PlanAttachment, x.saveName));

            return loopFileList;
        }

        #endregion 根据循环计划提交ID取得附件信息

        #region 根据循环计划ID取得详情

        /// <summary>
        /// 根据循环计划ID取得详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        public LoopPlanInfoModel GetLoopPlanInfoById(TargetNavigationDBEntities db, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlanInfo = db.vLoopPlanList.Where(p => p.loopId == loopId).Select(p =>
                new LoopPlanInfoModel
                {
                    loopId = loopId,
                    organizationId = p.organizationId,
                    executionModeId = p.executionModeId,
                    executionMode = p.executionMode,
                    eventOutput = p.eventOutput,
                    responsibleOrganization = p.responsibleOrganization,
                    responsibleUser = p.responsibleUser,
                    responsibleUserName = p.responsibleUserName,
                    responsibleUserImage = string.IsNullOrEmpty(p.responsibleUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + p.responsibleUserImage,
                    confirmUserImage = string.IsNullOrEmpty(p.confirmUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + p.confirmUserImage,
                    confirmOrganization = p.confirmOrganization,
                    confirmUser = p.confirmUser,
                    confirmUserName = p.confirmUserName,
                    startTime = p.startTime,
                    endTime = p.endTime,
                    comment = p.comment,
                    status = (EnumDefine.LoopPlanStatus)p.status,
                    stop = (EnumDefine.LoopPlanStopStatus)p.stop,
                    loopType = (EnumDefine.LoopPlanLoopType)p.loopType,
                    loopStatus = p.loopStatus,
                    importance = p.importance,
                    urgency = p.urgency,
                    difficulty = p.difficulty,
                    createUser = p.createUser,
                    createTime = p.createTime,
                    updateUser = p.updateUser,
                    updateTime = p.updateTime,
                    deleteFlag = p.deleteFlag,
                    loopYear = p.loopYear,
                    loopMonth = p.loopMonth,
                    loopWeek = p.loopWeek,
                    loopTime = p.loopTime,
                    keywordInfo = p.keyword,
                }).FirstOrDefault();
            return loopPlanInfo;
        }

        #endregion 根据循环计划ID取得详情

        #region 取得我的循环计划列表信息

        /// <summary>
        /// 取得我的循环计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<LoopPlanInfoModel> GetMyLoopPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            // 取得我的循环计划列表
            var loopPlanList = GetLoopPlanListByCondition(db, searchInfo, EnumDefine.PlanListType.Mine);

            return loopPlanList;
        }

        #endregion 取得我的循环计划列表信息

        #region 取得下属循环计划列表信息

        /// <summary>
        /// 取得下属循环计划列表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<LoopPlanInfoModel> GetSubordinateLoopPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            // 取得下属的循环计划列表
            var loopPlanList = GetLoopPlanListByCondition(db, searchInfo, EnumDefine.PlanListType.Subordinate);

            return loopPlanList;
        }

        #endregion 取得下属循环计划列表信息

        #region 添加循环计划附件

        /// <summary>
        /// 添加循环计划附件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="attachInfo"></param>
        /// <returns>附件ID</returns>
        public int InsLoopPlanAttachInfo(TargetNavigationDBEntities db, FileInfoModel attachInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var attachId = DBUtility.GetPrimaryKeyByTableName(db, "tblLoopplanAttachment");
            var loopId = db.tblLoopplanSubmit.Where(p => p.submitId == attachInfo.targetId).Select(p => p.loopId).FirstOrDefault();
            var tblLoopAttach = new tblLoopplanAttachment
            {
                attachmentId = attachId,
                submitId = attachInfo.targetId,
                loopId = loopId,
                displayName = attachInfo.displayName,
                saveName = attachInfo.saveName,
                extension = attachInfo.extension,
                isPreviewable = attachInfo.isPreviewable,
                createUser = attachInfo.uploadUserId.Value,
                createTime = DateTime.Now,
                updateUser = attachInfo.uploadUserId.Value,
                updateTime = DateTime.Now,
                deleteFlag = false
            };
            db.tblLoopplanAttachment.Add(tblLoopAttach);
            return attachId;
        }

        #endregion 添加循环计划附件

        #region 提交循环计划

        /// <summary>
        /// 提交循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void SubmitLoopPlan(TargetNavigationDBEntities db, int userId, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopId, db);
            if (loopPlan == null) return;
            loopPlan.status = (int)EnumDefine.LoopPlanStatus.Checking;
            loopPlan.updateUser = userId;
            loopPlan.updateTime = DateTime.Now;
        }

        #endregion 提交循环计划

        #region 删除循环计划

        /// <summary>
        /// 删除循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void DeleteLoopPlan(TargetNavigationDBEntities db, int userId, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopId, db);
            if (loopPlan == null) return;
            loopPlan.deleteFlag = true;
            loopPlan.updateUser = userId;
            loopPlan.updateTime = DateTime.Now;
        }

        #endregion 删除循环计划

        #region 转办循环计划

        /// <summary>
        /// 转办循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        ///  <param name="responsibleUser"></param>
        /// <param name="confirmUser"></param>
        public void TurnLoopPlan(TargetNavigationDBEntities db, int userId, int loopId, int responsibleUser, int confirmUser)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopId, db);
            if (loopPlan == null) return;
            //修改责任人和确认人
            loopPlan.responsibleUser = responsibleUser;
            loopPlan.confirmUser = confirmUser;
            loopPlan.updateUser = userId;
            loopPlan.updateTime = DateTime.Now;
        }

        #endregion 转办循环计划

        #region 申请修改循环计划

        /// <summary>
        /// 申请修改循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void AlterLoopPlan(TargetNavigationDBEntities db, int userId, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopId, db);
            if (loopPlan == null) return;
            loopPlan.status = (int)EnumDefine.LoopPlanStatus.RequestEdit;
            loopPlan.updateUser = userId;
            loopPlan.updateTime = DateTime.Now;
        }

        #endregion 申请修改循环计划

        #region 申请中止循环计划

        /// <summary>
        /// 申请中止循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void StopLoopPlan(TargetNavigationDBEntities db, int userId, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopId, db);
            if (loopPlan == null) return;
            loopPlan.stop = (int)EnumDefine.LoopPlanStopStatus.Checking;
            loopPlan.updateUser = userId;
            loopPlan.updateTime = DateTime.Now;
        }

        #endregion 申请中止循环计划

        #region 重新开始循环计划

        /// <summary>
        /// 重新开始循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void RestartLoopPlan(TargetNavigationDBEntities db, int userId, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopId, db);
            if (loopPlan == null) return;
            loopPlan.stop = (int)EnumDefine.LoopPlanStopStatus.Running;
            loopPlan.updateUser = userId;
            loopPlan.updateTime = DateTime.Now;
        }

        #endregion 重新开始循环计划

        #region 循环计划提交确认

        /// <summary>
        /// 循环计划提交确认
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopPlanOperate"></param>
        public void SubmitConfirmLoopPlan(TargetNavigationDBEntities db, int userId, LoopPlanOperateModel loopPlanOperate)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetLoopPlanSubmintInfoById(loopPlanOperate.submitId.Value, db);
            if (loopPlan == null) return;

            loopPlan.unitTime = loopPlanOperate.unitTime;
            loopPlan.isAttach = loopPlanOperate.isAttach;
            loopPlan.number = loopPlanOperate.number;
            loopPlan.completeStatus = (int)EnumDefine.LoopPlanSubmitStatus.Confirming;
            loopPlan.result = loopPlanOperate.result;
            loopPlan.submitTime = DateTime.Now;
            loopPlan.updateTime = DateTime.Now;
            loopPlan.updateUser = userId;
        }

        #endregion 循环计划提交确认

        #region 审批循环计划

        /// <summary>
        /// 审批循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        public void ApproveLoopPlan(TargetNavigationDBEntities db, int userId, LoopPlanOperateModel loopPlanOperate)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetTblLoopPlanInfoById(loopPlanOperate.loopId.Value, db);
            if (loopPlan == null) return;
            //审批通过
            if (loopPlanOperate.isApprove)
            {
                //申请中止场合
                if (loopPlan.stop == (int)EnumDefine.LoopPlanStopStatus.Checking)
                {
                    loopPlan.stop = (int)EnumDefine.LoopPlanStopStatus.Stopped;
                }
                //申请修改场合
                else if (loopPlan.status == (int)EnumDefine.LoopPlanStatus.RequestEdit)
                {
                    loopPlan.status = (int)EnumDefine.LoopPlanStatus.UnSubmit;
                    loopPlan.importance = 0;
                    loopPlan.urgency = 0;
                    loopPlan.difficulty = 0;
                }
                //正常场合
                else
                {
                    loopPlan.importance = loopPlanOperate.importance;
                    loopPlan.urgency = loopPlanOperate.urgency;
                    loopPlan.difficulty = loopPlanOperate.difficulty;
                    loopPlan.status = (int)EnumDefine.LoopPlanStatus.CheckPassed;
                }
            }
            //不通过
            else
            {
                //申请中止场合
                if (loopPlan.stop == (int)EnumDefine.LoopPlanStopStatus.Checking)
                {
                    loopPlan.stop = (int)EnumDefine.LoopPlanStopStatus.Running;
                }
                //申请修改场合
                else if (loopPlan.status == (int)EnumDefine.LoopPlanStatus.RequestEdit)
                {
                    loopPlan.status = (int)EnumDefine.LoopPlanStatus.CheckPassed;
                }
                //正常场合
                else
                {
                    loopPlan.status = (int)EnumDefine.LoopPlanStatus.CheckDenied;
                }
            }
            loopPlan.updateTime = DateTime.Now;
            loopPlan.updateUser = userId;
        }

        #endregion 审批循环计划

        #region 确认循环计划

        /// <summary>
        /// 确认循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planOperate"></param>
        public void ConfirmLoopPlan(TargetNavigationDBEntities db, int userId, LoopPlanOperateModel loopPlanOperate)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlan = GetLoopPlanSubmintInfoById(loopPlanOperate.submitId.Value, db);
            if (loopPlan == null) return;

            //确认通过
            if (loopPlanOperate.isApprove)
            {
                loopPlan.completeStatus = (int)EnumDefine.LoopPlanSubmitStatus.Complete;
                loopPlan.completeQuality = loopPlanOperate.completeQuality;
                loopPlan.completeQuantity = loopPlanOperate.completeQuantity;
                loopPlan.completeTime = loopPlanOperate.completeTime;
            }
            //不通过
            else
            {
                loopPlan.completeStatus = (int)EnumDefine.LoopPlanSubmitStatus.UnSubmit;
            }
            loopPlan.confirmTime = DateTime.Now;
            loopPlan.updateTime = DateTime.Now;
            loopPlan.updateUser = userId;
        }

        #endregion 确认循环计划

        #region 撤销循环计划上次操作

        /// <summary>
        /// 撤销循环计划上次操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        /// <param name="status"></param>
        public void RevokeLoopPlan(TargetNavigationDBEntities db, int userId, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopInfo = GetTblLoopPlanInfoById(loopId, db);
            if (loopInfo == null) return;

            switch (loopInfo.stop)
            {
                //申请中止的场合
                case (int)EnumDefine.LoopPlanStopStatus.Checking:
                    loopInfo.stop = (int)EnumDefine.LoopPlanStopStatus.Running;
                    break;
                //其他场合
                case (int)EnumDefine.LoopPlanStopStatus.Running:
                    switch (loopInfo.status)
                    {
                        //待审核
                        case (int)EnumDefine.LoopPlanStatus.Checking:
                        //审核通过
                        case (int)EnumDefine.LoopPlanStatus.CheckPassed:
                            loopInfo.status = (int)EnumDefine.LoopPlanStatus.UnSubmit;
                            break;
                        //申请修改
                        case (int)EnumDefine.LoopPlanStatus.RequestEdit:
                            loopInfo.status = (int)EnumDefine.LoopPlanStatus.CheckPassed;
                            break;

                        default:
                            break;
                    }
                    break;
            }
            loopInfo.updateUser = userId;
            loopInfo.updateTime = DateTime.Now;
        }

        #endregion 撤销循环计划上次操作

        #region 撤销循环计划完成情况的上次操作

        /// <summary>
        /// 撤销循环计划完成情况的上次操作
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="submitId"></param>
        public void RevokeLoopPlanSubmit(TargetNavigationDBEntities db, int userId, int submitId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopInfo = GetLoopPlanSubmintInfoById(submitId, db);
            if (loopInfo == null) return;
            loopInfo.completeStatus = (int)EnumDefine.LoopPlanSubmitStatus.UnSubmit;
            loopInfo.number = 0;
            loopInfo.unitTime = 0;
            loopInfo.updateUser = userId;
            loopInfo.updateTime = DateTime.Now;
        }

        #endregion 撤销循环计划完成情况的上次操作

        #region 取得我的循环计划状态数量

        /// <summary>
        /// 取得我的循环计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public PlanStatusCountModel GetMyLoopPlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetLoopPlanStatusInfo(db, EnumDefine.PlanListType.Mine, userId, fromTime, toTime);
        }

        #endregion 取得我的循环计划状态数量

        #region 取得下属循环计划状态数量

        /// <summary>
        /// 取得下属循环计划状态数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public PlanStatusCountModel GetSubordinateLoopPlanStatusInfo(TargetNavigationDBEntities db, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetLoopPlanStatusInfo(db, EnumDefine.PlanListType.Subordinate, userId, fromTime, toTime);
        }

        #endregion 取得下属循环计划状态数量

        #region 添加循环计划

        /// <summary>
        /// 添加循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        /// <returns>循环计划ID</returns>
        public int InsLoopPlanInfo(TargetNavigationDBEntities db, LoopPlanInfoModel loopPlanInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopId = DBUtility.GetPrimaryKeyByTableName(db, "tblLoopPlan");
            //计划信息表新增
            var loopPlanModel = new tblLoopPlan
            {
                loopId = loopId,
                organizationId = loopPlanInfo.organizationId,
                executionModeId = loopPlanInfo.executionModeId,
                eventOutput = loopPlanInfo.eventOutput,
                responsibleOrganization = loopPlanInfo.responsibleOrganization,
                responsibleUser = loopPlanInfo.responsibleUser,
                confirmOrganization = loopPlanInfo.confirmOrganization,
                confirmUser = loopPlanInfo.confirmUser,
                startTime = loopPlanInfo.startTime,
                endTime = loopPlanInfo.endTime,
                comment = loopPlanInfo.comment,
                loopYear = loopPlanInfo.loopYear,
                loopMonth = loopPlanInfo.loopMonth,
                loopWeek = loopPlanInfo.loopWeek,
                loopTime = loopPlanInfo.loopTime,
                status = (int)loopPlanInfo.status,
                stop = (int)loopPlanInfo.stop,
                loopType = (int)loopPlanInfo.loopType,
                loopStatus = loopPlanInfo.loopStatus,
                importance = loopPlanInfo.importance,
                urgency = loopPlanInfo.urgency,
                difficulty = loopPlanInfo.difficulty,
                createUser = loopPlanInfo.createUser,
                createTime = loopPlanInfo.createTime,
                updateUser = loopPlanInfo.updateUser,
                updateTime = loopPlanInfo.updateTime,
                deleteFlag = loopPlanInfo.deleteFlag,
                //标签
                keyword = loopPlanInfo.keyword != null && loopPlanInfo.keyword.Length > 0 ? string.Join(",", loopPlanInfo.keyword) : null
            };

            db.tblLoopPlan.Add(loopPlanModel);
            return loopId;
        }

        #endregion 添加循环计划

        #region 更新循环计划

        /// <summary>
        /// 更新循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planInfo"></param>
        public void UpdLoopPlanInfo(TargetNavigationDBEntities db, LoopPlanInfoModel loopPlanInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (loopPlanInfo == null)
            {
                throw new ArgumentNullException("loopPlanInfo");
            }

            var info = db.tblLoopPlan.Where(p => p.loopId == loopPlanInfo.loopId).FirstOrDefault();

            if (info == null) return;

            info.importance = loopPlanInfo.importance;
            info.comment = loopPlanInfo.comment;
            info.confirmOrganization = loopPlanInfo.confirmOrganization;
            info.confirmUser = loopPlanInfo.confirmUser;
            info.createTime = loopPlanInfo.createTime;
            info.deleteFlag = loopPlanInfo.deleteFlag;
            info.createUser = loopPlanInfo.createUser;
            info.difficulty = loopPlanInfo.difficulty;
            info.endTime = loopPlanInfo.endTime;
            info.eventOutput = loopPlanInfo.eventOutput;
            info.executionModeId = loopPlanInfo.executionModeId;
            info.loopMonth = loopPlanInfo.loopMonth;
            info.loopStatus = loopPlanInfo.loopStatus;
            info.loopTime = loopPlanInfo.loopTime;
            info.loopType = (int)loopPlanInfo.loopType;
            info.loopWeek = loopPlanInfo.loopWeek;
            info.loopYear = loopPlanInfo.loopYear;
            info.organizationId = loopPlanInfo.organizationId;
            info.responsibleOrganization = loopPlanInfo.responsibleOrganization;
            info.responsibleUser = loopPlanInfo.responsibleUser;
            info.startTime = loopPlanInfo.startTime;
            info.status = (int)loopPlanInfo.status;
            info.updateTime = DateTime.Now;
            info.updateUser = loopPlanInfo.createUser;
            info.urgency = loopPlanInfo.urgency;
            info.stop = (int)loopPlanInfo.stop;
            //标签
            info.keyword = loopPlanInfo.keyword != null && loopPlanInfo.keyword.Length > 0 ? string.Join(",", loopPlanInfo.keyword) : null;
        }

        #endregion 更新循环计划

        #region 添加循环计划完成信息

        /// <summary>
        /// 添加循环计划完成信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="submitInfo"></param>
        /// <returns></returns>
        public int InsLoopPlanSubmitInfo(TargetNavigationDBEntities db, LoopPlanSubmitInfoModel submitInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            //提交Id
            var submitId = DBUtility.GetPrimaryKeyByTableName(db, "tblLoopplanSubmit");
            //完成信息
            var loopSubmitInfo = new tblLoopplanSubmit
            {
                submitId = submitId,
                unitTime = submitInfo.time,
                number = submitInfo.quantity,
                completeStatus = (int)submitInfo.completeStatus,
                result = submitInfo.result,
                isAttach = submitInfo.isAttach,
                createUser = submitInfo.createUser.Value,
                createTime = DateTime.Now,
                updateUser = submitInfo.createUser.Value,
                updateTime = DateTime.Now
            };
            db.tblLoopplanSubmit.Add(loopSubmitInfo);
            return submitId;
        }

        #endregion 添加循环计划完成信息

        #region 取得我的循环计划分组信息

        /// <summary>
        /// 取得我的循环计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PlanGroupInfoModel> GetMyLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetLoopPlanGourpInfo(db, userId, groupType, EnumDefine.PlanListType.Mine);
        }

        #endregion 取得我的循环计划分组信息

        #region 取得下属循环计划分组信息

        /// <summary>
        /// 取得下属循环计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PlanGroupInfoModel> GetSubordinateLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            return GetLoopPlanGourpInfo(db, userId, groupType, EnumDefine.PlanListType.Subordinate);
        }

        #endregion 取得下属循环计划分组信息

        /// <summary>
        /// 取得循环计划提交信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public List<LoopPlanSubmitInfoModel> GetLoopPlanSubmitInfo(TargetNavigationDBEntities db, int loopId, int pageNum)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = db.tblLoopplanSubmit.Where(x => x.loopId == loopId)
                .OrderByDescending(x => x.executeTime)
                .Take(ConstVar.LoopPlanSubmitPageNum * pageNum)
                .Select(x => new LoopPlanSubmitInfoModel
                {
                    loopId = x.loopId,
                    submitId = x.submitId,
                    time = x.unitTime,
                    quantity = x.number,
                    completeStatus = (EnumDefine.LoopPlanSubmitStatus)x.completeStatus,
                    undo = x.undo,
                    result = x.result,
                    isAttach = x.isAttach,
                    completeQuality = x.completeQuality,
                    completeQuantity = x.completeQuantity,
                    completeTime = x.completeTime,
                    executeTime = x.executeTime,
                    submitTime = x.submitTime,
                    confirmTime = x.confirmTime
                }).ToList().OrderBy(x => x.executeTime).Take(ConstVar.LoopPlanSubmitPageNum).ToList();

            return result;
        }

        /// <summary>
        /// 生成空循环计划提交信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="generateNumber"></param>
        /// <returns></returns>
        public List<LoopPlanSubmitInfoModel> GenerateLoopPlanEmptySubmitInfo(EnumDefine.LoopPlanLoopType type, DateTime startTime, int generateNumber)
        {
            var result = new List<LoopPlanSubmitInfoModel>();

            for (var i = 0; i < generateNumber; i++)
            {
                var infoModel = new LoopPlanSubmitInfoModel();
                infoModel.completeStatus = EnumDefine.LoopPlanSubmitStatus.UnSubmit;
                infoModel.undo = true;

                switch (type)
                {
                    case EnumDefine.LoopPlanLoopType.Day:
                        infoModel.executeTime = startTime.AddDays(1 * i);
                        break;

                    case EnumDefine.LoopPlanLoopType.Month:
                        infoModel.executeTime = startTime.AddMonths(1 * i);
                        break;

                    case EnumDefine.LoopPlanLoopType.Week:
                        infoModel.executeTime = startTime.AddDays(7 * i);

                        break;

                    case EnumDefine.LoopPlanLoopType.Year:
                        infoModel.executeTime = startTime.AddYears(1 * i);

                        break;
                }

                result.Add(infoModel);
            }

            return result;
        }

        #endregion 循环计划

        #region 协作计划

        #region 取得协作计划

        /// <summary>
        /// 取得协作计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PlanInfoModel> GetCooperationPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 取得协作计划列表
            var planList = GetPlanListByCondition(db, searchInfo, EnumDefine.PlanListType.Cooperation);

            return planList;
        }

        #endregion 取得协作计划

        #region 取得协作循环计划

        /// <summary>
        /// 取得协作循环计划
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<LoopPlanInfoModel> GetCooperationLoopPlanList(TargetNavigationDBEntities db, PlanSearchModel searchInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            //取得循环计划中的协作计划列表
            var cooperationList = GetLoopPlanListByCondition(db, searchInfo, EnumDefine.PlanListType.Cooperation);
            return cooperationList;
        }

        #endregion 取得协作循环计划

        #region 取得协作一般计划分组信息

        /// <summary>
        /// 取得协作一般计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PlanGroupInfoModel> GetCooperationPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var planGroupList = GetPlanGourpInfo(db, userId, groupType, EnumDefine.PlanListType.Cooperation);
            return planGroupList;
        }

        #endregion 取得协作一般计划分组信息

        #region 取得协作循环计划分组信息

        /// <summary>
        /// 取得协作循环计划分组信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PlanGroupInfoModel> GetCooperationLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var loopPlanGroupList = GetLoopPlanGourpInfo(db, userId, groupType, EnumDefine.PlanListType.Cooperation);
            return loopPlanGroupList;
        }

        #endregion 取得协作循环计划分组信息

        #endregion 协作计划

        #region 私有方法

        #region 取计划列表共通方法

        /// <summary>
        /// 取计划列表共通方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        private List<PlanInfoModel> GetPlanListByCondition(TargetNavigationDBEntities db, PlanSearchModel searchInfo, EnumDefine.PlanListType listType)
        {
            List<PlanInfoModel> planInfo = null;

            StringBuilder condition = new StringBuilder();
            List<object> values = new List<object>();
            List<int> cooperationPlans = null;

            // 删除标志
            condition.Append(" deleteFlag == false ");
            if (searchInfo.lastTime != null)
            {
                condition.Append(" AND updateTime<=@0 ");
                values.Add(searchInfo.lastTime.Value);
            }

            //列表类型处理
            condition.Append(GetListTypeSql(listType, searchInfo.userId, values));

            // 筛选处理
            condition.Append(GetSearchSql(searchInfo.filterInfo, values));

            //分组处理
            condition.Append(GetGroupSql(listType, searchInfo.group, searchInfo.groupValue, values));

            var planQuery = db.vPlanList.Where(condition.ToString(), values.ToArray());

            // 协作人计划筛选
            if (listType == EnumDefine.PlanListType.Cooperation)
            {
                // 协作人
                cooperationPlans = db.tblPlanCooperation.Where(p => p.userId == searchInfo.userId).Select(p => p.planId).ToList();
                planQuery = planQuery.Where(p => cooperationPlans.Contains(p.planId));
            }

            //排序处理
            planQuery = GetOrderSql(planQuery, searchInfo.sortInfo);

            //分页处理
            if (searchInfo.page > 0)
            {
                //每页显示计划数量
                var pageNum = searchInfo.group == EnumDefine.PlanGroupType.None ? ConstVar.PlanListPageNum : ConstVar.PlanGroupPageNum;
                planQuery = planQuery.Skip((searchInfo.page - 1) * pageNum).Take(pageNum);
            }

            planInfo = planQuery.Select(c => new PlanInfoModel
            {
                planId = c.planId,
                parentPlan = c.parentPlan,

                executionModeId = c.executionId,
                executionMode = c.executionMode,

                organizationId = c.organizationId,

                responsibleOrganization = c.responsibleOrganization,
                responsibleUser = c.responsibleUser,
                responsibleUserName = c.responsibleUserName,
                responsibleUserImage = string.IsNullOrEmpty(c.responsibleUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + c.responsibleUserImage,

                confirmOrganization = c.confirmOrganization,
                confirmUser = c.confirmUser,
                confirmUserName = c.confirmUserName,
                confirmUserImage = string.IsNullOrEmpty(c.confirmUserImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + c.confirmUserImage,

                startTime = c.startTime,
                endTime = c.endTime.Value,
                workTime = c.workTime,
                comment = c.comment,
                alert = c.alert,

                importance = c.importance,
                urgency = c.urgency,
                difficulty = c.difficulty,

                progress = c.progress,
                quantity = c.quantity,
                time = c.time,

                completeQuantity = c.completeQuantity,
                completeQuality = c.completeQuality,
                completeTime = c.completeTime,

                status = (EnumDefine.PlanStatus)c.status,
                stop = (EnumDefine.PlanStopStatus)c.stop,
                createTime = c.createTime,
                updateTime = c.updateTime,
                initial = c.initial,
                withSub = c.withSub,
                archive = c.archive,
                archiveTime = c.archiveTime,
                autoStart = c.autoStart,
                eventOutput = c.eventOutput,
                isLoop = false,
                IsCollPlan = true,
                withFront = c.withFront,
                effectiveTime = c.quantity == null ? 0 : c.quantity * c.time,
                realTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,
                isAttach = c.isAttach
            }).ToList();

            return planInfo;
        }

        #endregion 取计划列表共通方法

        #region 取循环计划列表共通方法

        /// <summary>
        /// 取循环计划列表共通方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="searchInfo"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        private List<LoopPlanInfoModel> GetLoopPlanListByCondition(TargetNavigationDBEntities db, PlanSearchModel searchInfo, EnumDefine.PlanListType listType)
        {
            List<LoopPlanInfoModel> loopPlanList = null;

            StringBuilder condition = new StringBuilder();
            List<object> values = new List<object>();
            List<int> cooperationPlans = null;

            // 计划删除标志
            condition.Append(" deleteFlag == false ");
            if (searchInfo.lastTime != null)
            {
                condition.Append(" AND updateTime<=@0 ");
                values.Add(searchInfo.lastTime.Value);
            }
            //列表类型处理
            condition.Append(GetListTypeSql(listType, searchInfo.userId, values));

            // 筛选处理
            condition.Append(GetLoopPlanSearchSql(searchInfo.filterInfo, values));

            //分组处理
            condition.Append(GetGroupSql(listType, searchInfo.group, searchInfo.groupValue, values));

            var loopPlanQuery = db.vLoopPlanList.Where(condition.ToString(), values.ToArray());

            // 协作人计划筛选
            if (listType == EnumDefine.PlanListType.Cooperation)
            {
                // 协作人
                cooperationPlans = db.tblLoopPlanCooperation.Where(p => p.userId == searchInfo.userId).Select(p => p.loopId).ToList();
                loopPlanQuery = loopPlanQuery.Where(p => cooperationPlans.Contains(p.loopId));
            }

            //分页处理
            if (searchInfo.page > 0)
            {
                //每页显示计划数量
                var pageNum = searchInfo.group == EnumDefine.PlanGroupType.None ? ConstVar.PlanListPageNum : ConstVar.PlanGroupPageNum;
                loopPlanQuery = loopPlanQuery.Skip((searchInfo.page - 1) * pageNum).Take(pageNum);
            }

            loopPlanList = loopPlanQuery.Select(c => new LoopPlanInfoModel
            {
                loopId = c.loopId,
                isLoop = true,
                executionModeId = c.executionModeId,
                responsibleOrganization = c.responsibleOrganization,
                responsibleUser = c.responsibleUser,
                responsibleUserName = c.responsibleUserName,
                confirmOrganization = c.confirmOrganization,
                confirmUser = c.confirmUser,
                confirmUserName = c.confirmUserName,
                eventOutput = c.eventOutput,
                startTime = c.startTime,
                endTime = c.endTime.Value,
                comment = c.comment,
                importance = c.importance,
                urgency = c.urgency,
                difficulty = c.difficulty,
                status = (EnumDefine.LoopPlanStatus)c.status,
                stop = (EnumDefine.LoopPlanStopStatus)c.stop,
                loopStatus = c.loopStatus,
                createTime = c.createTime,
                updateTime = c.updateTime,
                loopYear = c.loopYear,
                loopMonth = c.loopMonth,
                loopWeek = c.loopWeek,
                loopTime = c.loopTime,
                loopType = (EnumDefine.LoopPlanLoopType)c.loopType,

                organizationId = c.organizationId,
                executionMode = c.executionMode,
                submitCount = c.submitCount
            }).ToList();

            return loopPlanList;
        }

        #endregion 取循环计划列表共通方法

        #region 获取列表类型SQL

        /// <summary>
        /// 获取列表类型SQL
        /// </summary>
        /// <param name="listType"></param>
        /// <param name="userId"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private string GetListTypeSql(EnumDefine.PlanListType listType, int userId, List<object> values)
        {
            var condition = new StringBuilder();
            switch (listType)
            {
                case EnumDefine.PlanListType.Mine:

                    // 责任人
                    condition.Append(" AND responsibleUser == @").Append(values.Count);
                    values.Add(userId);

                    break;

                case EnumDefine.PlanListType.Subordinate:

                    // 确认人
                    condition.Append(" AND confirmUser == @").Append(values.Count);
                    values.Add(userId);

                    // 责任人在职
                    condition.Append(" AND responsibleUserDeleteFlag == false AND responsibleUserWorkStatus == ");
                    condition.Append((int)EnumDefine.WorkStatus.OnWork);

                    break;

                case EnumDefine.PlanListType.Cooperation:

                    // 责任人在职
                    condition.Append(" AND responsibleUserDeleteFlag == false AND responsibleUserWorkStatus == ");
                    condition.Append((int)EnumDefine.WorkStatus.OnWork);

                    break;

                default:
                    break;
            }
            return condition.ToString();
        }

        #endregion 获取列表类型SQL

        #region 获取一般计划条件筛选SQL

        /// <summary>
        /// 获取一般计划条件筛选SQL
        /// </summary>
        /// <param name="planFilterInfo"></param>
        /// <returns></returns>
        private string GetSearchSql(PlanFilterInfoModel planFilterInfo, List<object> values)
        {
            if (planFilterInfo == null)
            {
                return string.Empty;
            }

            var condition = new StringBuilder();

            switch (planFilterInfo.fast)
            {
                // 快捷筛选:今日未完成
                case EnumDefine.PlanFastFilter.UnDone:
                    //AND (status==20 Or status==40) And stop==0 AND endTime>=@0 AND endTime<=@1
                    condition.Append(" AND (status==").Append((int)EnumDefine.PlanStatus.CheckPassed);
                    condition.Append(" Or status== ").Append((int)EnumDefine.PlanStatus.ConfirmDenied);
                    condition.Append(" ) And stop== ").Append((int)EnumDefine.PlanStopStatus.Running);
                    condition.Append(" AND endTime>=@").Append(values.Count);
                    condition.Append(" AND endTime<@").Append(values.Count + 1);
                    //筛选今天
                    values.AddRange(new List<object> { DateTime.Today, DateTime.Today.AddDays(1) });
                    break;
                //快捷筛选：超时计划
                case EnumDefine.PlanFastFilter.OverTime:
                    // AND (status==20 Or status==40) And stop==0 AND endTime <@0
                    condition.Append(" AND (status== ").Append((int)EnumDefine.PlanStatus.CheckPassed);
                    condition.Append(" Or status== ").Append((int)EnumDefine.PlanStatus.ConfirmDenied);
                    condition.Append(" ) And stop== ").Append((int)EnumDefine.PlanStopStatus.Running);
                    condition.Append(" AND endTime<@").Append(values.Count);
                    values.Add(DateTime.Today);
                    break;
                //非快捷筛选
                case EnumDefine.PlanFastFilter.None:
                    //状态筛选
                    condition.Append(GetStatusSql(planFilterInfo.status));
                    //时间筛选
                    condition.Append(GetTimeSql(planFilterInfo.fromTime, planFilterInfo.toTime, values));
                    //人员筛选
                    condition.Append(GetPersonSql(planFilterInfo.userIds));
                    break;

                default:
                    return null;
            }

            return condition.ToString();
        }

        #endregion 获取一般计划条件筛选SQL

        #region 获取循环计划条件筛选SQL

        /// <summary>
        /// 获取循环计划条件筛选SQL
        /// </summary>
        /// <param name="planFilterInfo"></param>
        /// <returns></returns>
        private string GetLoopPlanSearchSql(PlanFilterInfoModel planFilterInfo, List<object> values)
        {
            if (planFilterInfo == null)
            {
                return string.Empty;
            }

            var condition = new StringBuilder();

            switch (planFilterInfo.fast)
            {
                // 快捷筛选:今日未完成
                case EnumDefine.PlanFastFilter.UnDone:
                    condition.Append(" completeStatus== ").Append((int)EnumDefine.LoopPlanSubmitStatus.UnSubmit);
                    condition.Append(" AND createTime>=@").Append(values.Count).Append(" AND createTime<@").Append(values.Count + 1);
                    values.AddRange(new List<object> { DateTime.Today, DateTime.Today.AddDays(1) });
                    break;
                //快捷筛选：超时计划
                case EnumDefine.PlanFastFilter.OverTime:
                    condition.Append(" completeStatus== ").Append((int)EnumDefine.LoopPlanSubmitStatus.UnSubmit);
                    condition.Append(" AND createTime<@").Append(values.Count);
                    values.Add(DateTime.Today);
                    break;
                //非快捷筛选
                case EnumDefine.PlanFastFilter.None:
                    //状态筛选
                    condition.Append(GetLoopPlanStatusSql(planFilterInfo.status, values));

                    //时间筛选
                    condition.Append(GetTimeSql(planFilterInfo.fromTime, planFilterInfo.toTime, values));
                    //人员筛选
                    condition.Append(GetPersonSql(planFilterInfo.userIds));
                    break;

                default:
                    return null;
            }

            return condition.ToString();
        }

        #endregion 获取循环计划条件筛选SQL

        #region 一般计划状态筛选SQL

        /// <summary>
        /// 一般计划状态筛选SQL
        /// </summary>
        /// <param name="pageStatus"></param>
        /// <returns></returns>
        private string GetStatusSql(List<EnumDefine.PlanPageStatus> pageStatus)
        {
            if (pageStatus == null || pageStatus.Count == 0)
            {
                return string.Empty;
            }

            var condition = new StringBuilder();

            condition.Append(" AND ( ");
            foreach (var status in pageStatus)
            {
                condition.Append(" ( ");
                switch (status)
                {
                    //待提交
                    case EnumDefine.PlanPageStatus.UnSubmit:
                        //(status==0 Or status==15) And stop==0
                        condition.Append(" (status== ");
                        condition.Append((int)EnumDefine.PlanStatus.UnSubmit);
                        condition.Append(" Or status== ");
                        condition.Append((int)EnumDefine.PlanStatus.CheckDenied);
                        condition.Append(" ) And stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Running);
                        break;
                    //待审核
                    case EnumDefine.PlanPageStatus.Checking:
                        //((status==10 Or status==25) And stop==0) Or stop==10
                        condition.Append(" ((status==");
                        condition.Append((int)EnumDefine.PlanStatus.Checking);
                        condition.Append(" Or status== ");
                        condition.Append((int)EnumDefine.PlanStatus.RequestEdit);
                        condition.Append(" ) And stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Running);
                        condition.Append(" ) Or stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Checking);
                        break;
                    //进行中
                    case EnumDefine.PlanPageStatus.Running:
                        //(status==20 Or status==40) And stop==0
                        condition.Append(" (status== ");
                        condition.Append((int)EnumDefine.PlanStatus.CheckPassed);
                        condition.Append(" Or status== ");
                        condition.Append((int)EnumDefine.PlanStatus.ConfirmDenied);
                        condition.Append(" ) And stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Running);
                        break;
                    //待确认
                    case EnumDefine.PlanPageStatus.Confirming:
                        //status==30 And stop==0
                        condition.Append(" status== ");
                        condition.Append((int)EnumDefine.PlanStatus.Confirming);
                        condition.Append(" And stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Running);
                        break;
                    //已完成
                    case EnumDefine.PlanPageStatus.Complete:
                        //status==90 And stop==0
                        condition.Append(" status== ");
                        condition.Append((int)EnumDefine.PlanStatus.Complete);
                        condition.Append(" And stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Running);
                        break;
                    //已中止
                    case EnumDefine.PlanPageStatus.Stop:
                        condition.Append(" stop== ");
                        condition.Append((int)EnumDefine.PlanStopStatus.Stopped);
                        break;

                    default:
                        break;
                }
                condition.Append(" ) Or ");
            }
            condition = condition.Remove(condition.Length - 3, 3);
            condition.Append(" ) ");

            return condition.ToString();
        }

        #endregion 一般计划状态筛选SQL

        #region 循环计划状态筛选SQL

        /// <summary>
        /// 循环计划状态筛选SQL
        /// </summary>
        /// <param name="pageStatus"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private string GetLoopPlanStatusSql(List<EnumDefine.PlanPageStatus> pageStatus, List<object> values)
        {
            if (pageStatus == null || pageStatus.Count == 0)
            {
                return string.Empty;
            }

            var condition = new StringBuilder();

            condition.Append(" AND ( ");
            foreach (var status in pageStatus)
            {
                condition.Append(" ( ");
                switch (status)
                {
                    //待提交
                    case EnumDefine.PlanPageStatus.UnSubmit:
                        //(status==0 Or status==15) And stop==0
                        condition.Append(" (status== ").Append((int)EnumDefine.LoopPlanStatus.UnSubmit);
                        condition.Append(" Or status== ").Append((int)EnumDefine.LoopPlanStatus.CheckDenied);
                        condition.Append(" ) And stop== ").Append((int)EnumDefine.LoopPlanStopStatus.Running);
                        condition.Append(" And endTime>=@").Append(values.Count);
                        values.Add(DateTime.Today);
                        break;
                    //待审核
                    case EnumDefine.PlanPageStatus.Checking:
                        //((status==10 Or status==25) And stop==0) Or stop==10
                        condition.Append(" ((status==").Append((int)EnumDefine.LoopPlanStatus.Checking);
                        condition.Append(" Or status== ").Append((int)EnumDefine.LoopPlanStatus.RequestEdit);
                        condition.Append(" ) And stop== ").Append((int)EnumDefine.LoopPlanStopStatus.Running);
                        condition.Append(" ) Or stop== ").Append((int)EnumDefine.LoopPlanStopStatus.Checking);
                        condition.Append(" And endTime>=@").Append(values.Count);
                        values.Add(DateTime.Today);
                        break;
                    //进行中
                    case EnumDefine.PlanPageStatus.Running:
                        //status==0
                        condition.Append(" status== ").Append((int)EnumDefine.LoopPlanStatus.CheckPassed);
                        condition.Append(" And endTime>=@").Append(values.Count);
                        values.Add(DateTime.Today);
                        break;
                    //待确认
                    case EnumDefine.PlanPageStatus.Confirming:
                        //完成表:submitCount>0
                        condition.Append(" submitCount>0 ");
                        condition.Append(" And endTime>=@").Append(values.Count);
                        values.Add(DateTime.Today);
                        break;
                    //已完成
                    case EnumDefine.PlanPageStatus.Complete:
                        //完成表:endTime<DateTime.Today
                        condition.Append("  endTime<@").Append(values.Count);
                        values.Add(DateTime.Today);
                        break;
                    //已中止
                    case EnumDefine.PlanPageStatus.Stop:
                        condition.Append(" stop== ").Append((int)EnumDefine.LoopPlanStopStatus.Stopped);
                        condition.Append(" And endTime>=@").Append(values.Count);
                        values.Add(DateTime.Today);
                        break;

                    default:
                        break;
                }
                condition.Append(" ) Or ");
            }
            //去掉condition中多余字段"Or"
            condition = condition.Remove(condition.Length - 3, 3);
            condition.Append(" ) ");

            return condition.ToString();
        }

        #endregion 循环计划状态筛选SQL

        #region 时间筛选SQL

        /// <summary>
        /// 时间筛选SQL
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">结束时间</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        private string GetTimeSql(DateTime? fromTime, DateTime? toTime, List<object> value)
        {
            // 开始时间和结束时间为空，返回
            if (fromTime == null && toTime == null)
            {
                return string.Empty;
            }
            var condition = new StringBuilder();

            // 开始时间不为空，加上开始时间条件
            if (fromTime != null)
            {
                condition.Append(" AND endTime>=@");
                condition.Append(value.Count);
                value.Add(fromTime.Value);
            }

            // 结束时间不为空，加上结束时间条件
            if (toTime != null)
            {
                condition.Append(" AND endTime<=@");
                condition.Append(value.Count);
                value.Add(toTime.Value);
            }

            return condition.ToString();
        }

        #endregion 时间筛选SQL

        #region 人员筛选SQL

        /// <summary>
        /// 时间筛选SQL
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">结束时间</param>
        /// <returns></returns>
        private string GetPersonSql(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return string.Empty;
            }

            var condition = new StringBuilder();

            condition.Append(" And (");
            foreach (var userId in userIds)
            {
                condition.Append(" responsibleUser==");
                condition.Append(userId);
                condition.Append(" Or ");
                condition.Append(" confirmUser== ");
                condition.Append(userId);
                condition.Append(" Or ");
            }

            condition = condition.Remove(condition.Length - 3, 3);
            condition.Append(")");

            return condition.ToString();
        }

        #endregion 人员筛选SQL

        #region 排序SQL

        /// <summary>
        /// 排序SQL
        /// </summary>
        /// <returns></returns>
        private IQueryable<vPlanList> GetOrderSql(IQueryable<vPlanList> planQuery, List<PlanSortInfoModel> sortInfo)
        {
            if (sortInfo == null || sortInfo.Count == 0)
            {
                return planQuery;
            }

            //反转改变排序优先级
            sortInfo.Reverse();
            foreach (var order in sortInfo)
            {
                switch (order.sortType)
                {
                    case EnumDefine.PlanSortType.Default:
                        planQuery = planQuery.OrderByDescending(p => p.endTime).ThenByDescending(p => p.urgency).ThenByDescending(p => p.importance);
                        break;

                    case EnumDefine.PlanSortType.Importance:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.importance) : planQuery.OrderByDescending(p => p.importance);
                        break;

                    case EnumDefine.PlanSortType.Urgency:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.urgency) : planQuery.OrderByDescending(p => p.urgency);
                        break;

                    case EnumDefine.PlanSortType.Time:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.endTime) : planQuery.OrderByDescending(p => p.endTime);
                        break;

                    case EnumDefine.PlanSortType.ResponsibleUser:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.responsibleUser) : planQuery.OrderByDescending(p => p.responsibleUser);
                        break;

                    case EnumDefine.PlanSortType.ConfirmUser:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.confirmUser) : planQuery.OrderByDescending(p => p.confirmUser);
                        break;

                    case EnumDefine.PlanSortType.Status:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.status) : planQuery.OrderByDescending(p => p.status);
                        break;

                    case EnumDefine.PlanSortType.Organization:
                        planQuery = order.orderType == EnumDefine.OrderType.Asc ? planQuery.OrderBy(p => p.organizationId) : planQuery.OrderByDescending(p => p.organizationId);
                        break;

                    default:
                        break;
                }
            }

            return planQuery;
        }

        #endregion 排序SQL

        #region 分组SQl

        /// <summary>
        /// 分组SQl
        /// </summary>
        /// <param name="group"></param>
        /// <param name="groupValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private string GetGroupSql(EnumDefine.PlanListType listType, EnumDefine.PlanGroupType group, string groupValue, List<object> values)
        {
            if (group == EnumDefine.PlanGroupType.None || string.IsNullOrWhiteSpace(groupValue))
            {
                return string.Empty;
            }

            var condition = new StringBuilder();

            condition.Append(" AND ");
            switch (group)
            {
                case EnumDefine.PlanGroupType.Time:
                    var startTime = DateTime.Parse(groupValue);
                    var endTime = startTime.AddDays(1);
                    condition.Append(" endTime>=@");
                    condition.Append(values.Count);
                    condition.Append(" AND endTime<@");
                    condition.Append(values.Count + 1);
                    values.AddRange(new List<object> { startTime, endTime });
                    break;

                case EnumDefine.PlanGroupType.Organization:
                    condition.Append(" organizationId== ");
                    condition.Append(groupValue);
                    break;

                case EnumDefine.PlanGroupType.User:

                    switch (listType)
                    {
                        // 下属计划、协作计划根据责任人分组
                        case EnumDefine.PlanListType.Subordinate:
                        case EnumDefine.PlanListType.Cooperation:
                            condition.Append(" responsibleUser== ");
                            condition.Append(groupValue);
                            break;
                        // 我的计划根据确认人分组
                        case EnumDefine.PlanListType.Mine:
                            condition.Append(" confirmUser== ");
                            condition.Append(groupValue);
                            break;
                    }

                    break;

                default:
                    condition.Clear();
                    break;
            }

            return condition.ToString();
        }

        #endregion 分组SQl

        #region 根据Id查询一般计划信息通用方法

        /// <summary>
        /// 根据Id查询一般计划信息通用方法
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="db">连接数据库上下文</param>
        /// <returns>实体对象</returns>
        private tblPlan GetPlanInfoByIdForUpd(TargetNavigationDBEntities db, int planId)
        {
            return db.tblPlan.Where(a => a.planId == planId).FirstOrDefault<tblPlan>();
        }

        #endregion 根据Id查询一般计划信息通用方法

        #region 取得计划状态数量通用方法

        /// <summary>
        /// 取得计划状态数量通用方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="listType"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        private PlanStatusCountModel GetPlanStatusInfo(TargetNavigationDBEntities db, EnumDefine.PlanListType listType, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            PlanStatusCountModel planStatusCountModel = new PlanStatusCountModel();
            List<int> cooperationPlans = new List<int>();
            var condition = new StringBuilder().Append(" deleteFlag == false ");
            var values = new List<object>();
            switch (listType)
            {
                case EnumDefine.PlanListType.Mine:
                    condition.Append(" AND responsibleUser == @0 ");
                    values.Add(userId);
                    break;

                case EnumDefine.PlanListType.Subordinate:
                    condition.Append(" AND confirmUser == @0 ");
                    // 责任人在职
                    condition.Append(" AND responsibleUserDeleteFlag == false AND responsibleUserWorkStatus == ").Append((int)EnumDefine.WorkStatus.OnWork);
                    values.Add(userId);
                    break;

                case EnumDefine.PlanListType.Cooperation:
                    // 协作计划Id
                    cooperationPlans = db.tblPlanCooperation.Where(p => p.userId == userId).Select(p => p.planId).ToList();

                    // 责任人在职
                    condition.Append(" AND responsibleUserDeleteFlag == false AND responsibleUserWorkStatus == ");
                    condition.Append((int)EnumDefine.WorkStatus.OnWork);
                    break;

                default:
                    break;
            }

            if (fromTime != null)
            {
                condition.Append(" AND endTime >=@");
                condition.Append(values.Count);
                values.Add(fromTime.Value);
            }
            if (toTime != null)
            {
                condition.Append(" AND endTime <=@");
                condition.Append(values.Count);
                values.Add(toTime.Value);
            }

            var statusList = db.vPlanList.Where(condition.ToString(), values.ToArray());
            if (listType == EnumDefine.PlanListType.Cooperation)
            {
                statusList = statusList.Where(p => cooperationPlans.Contains(p.planId));
            }
            foreach (var item in statusList)
            {
                switch (item.stop)
                {
                    case (int)EnumDefine.PlanStopStatus.Checking:
                        planStatusCountModel.planCheckingCount++;
                        break;

                    case (int)EnumDefine.PlanStopStatus.Stopped:
                        planStatusCountModel.planStopCount++;
                        break;

                    case (int)EnumDefine.PlanStopStatus.Running:
                        switch (item.status)
                        {
                            // 待提交
                            case (int)EnumDefine.PlanStatus.UnSubmit:
                            case (int)EnumDefine.PlanStatus.CheckDenied:
                                planStatusCountModel.planSubmitingCount++;
                                break;
                            // 待审核
                            case (int)EnumDefine.PlanStatus.RequestEdit:
                            case (int)EnumDefine.PlanStatus.Checking:
                                planStatusCountModel.planCheckingCount++;
                                break;
                            // 已审核
                            case (int)EnumDefine.PlanStatus.CheckPassed:
                            case (int)EnumDefine.PlanStatus.ConfirmDenied:
                                planStatusCountModel.planCheckedCount++;
                                break;
                            // 待确认
                            case (int)EnumDefine.PlanStatus.Confirming:
                                planStatusCountModel.planConfirmingCount++;
                                break;
                            //已完成
                            case (int)EnumDefine.PlanStatus.Complete:
                                planStatusCountModel.planCompeteCount++;
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
            return planStatusCountModel;
        }

        #endregion 取得计划状态数量通用方法

        #region 取得一般计划分组信息共通方法

        /// <summary>
        /// 取得一般计划分组信息共通方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        private List<PlanGroupInfoModel> GetPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType, EnumDefine.PlanListType listType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            List<PlanGroupInfoModel> planGroupList = null;
            switch (groupType)
            {
                //时间
                case EnumDefine.PlanGroupType.Time:
                    planGroupList = new List<PlanGroupInfoModel>{
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Earlier.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Yesterday.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Today.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Tomorrow.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Later.ToString()}
                    };
                    break;
                //部门
                case EnumDefine.PlanGroupType.Organization:
                    switch (listType)
                    {
                        //我的计划场合
                        case EnumDefine.PlanListType.Mine:
                            planGroupList = (from p in db.tblPlan
                                             where !p.deleteFlag && p.responsibleUser == userId
                                             group p by p.organizationId into g
                                             select new PlanGroupInfoModel
                                             {
                                                 groupId = g.Key.ToString(),
                                                 groupName = db.tblOrganization.Where(o => o.organizationId == g.Key.Value).FirstOrDefault().organizationName
                                             }).ToList();
                            break;
                        //下属计划场合
                        case EnumDefine.PlanListType.Subordinate:
                            planGroupList = (from p in db.tblPlan
                                             where !p.deleteFlag && p.confirmUser == userId
                                             group p by p.organizationId into g
                                             select new PlanGroupInfoModel
                                             {
                                                 groupId = g.Key.ToString(),
                                                 groupName = db.tblOrganization.Where(o => o.organizationId == g.Key.Value).FirstOrDefault().organizationName
                                             }).ToList();
                            break;
                        //协作计划场合
                        case EnumDefine.PlanListType.Cooperation:
                            planGroupList = (from p in db.tblPlan
                                             join cp in db.tblPlanCooperation on p.planId equals cp.planId
                                             where !p.deleteFlag && cp.userId == userId
                                             group p by p.organizationId into g
                                             select new PlanGroupInfoModel
                                             {
                                                 groupId = g.Key.ToString(),
                                                 groupName = db.tblOrganization.Where(o => o.organizationId == g.Key.Value).FirstOrDefault().organizationName
                                             }).ToList();
                            break;
                    }
                    break;
                //人员
                case EnumDefine.PlanGroupType.User:
                    switch (listType)
                    {
                        //我的计划场合
                        case EnumDefine.PlanListType.Mine:
                            planGroupList = (from p in db.tblPlan
                                             where !p.deleteFlag && p.responsibleUser == userId
                                             group p by p.confirmUser into g
                                             select new PlanGroupInfoModel
                                             {
                                                 groupId = g.Key.ToString(),
                                                 groupName = db.tblUser.Where(u => u.userId == g.Key.Value).FirstOrDefault().userName
                                             }).ToList();
                            break;
                        //下属计划场合
                        case EnumDefine.PlanListType.Subordinate:
                            planGroupList = (from p in db.tblPlan
                                             where !p.deleteFlag && p.confirmUser == userId
                                             group p by p.responsibleUser into g
                                             select new PlanGroupInfoModel
                                             {
                                                 groupId = g.Key.ToString(),
                                                 groupName = db.tblUser.Where(u => u.userId == g.Key.Value).FirstOrDefault().userName
                                             }).ToList();
                            break;
                        //协作计划场合
                        case EnumDefine.PlanListType.Cooperation:
                            planGroupList = (from p in db.tblPlan
                                             join cp in db.tblPlanCooperation on p.planId equals cp.planId
                                             where !p.deleteFlag && cp.userId == userId
                                             group p by p.responsibleUser into g
                                             select new PlanGroupInfoModel
                                             {
                                                 groupId = g.Key.ToString(),
                                                 groupName = db.tblUser.Where(u => u.userId == g.Key.Value).FirstOrDefault().userName
                                             }).ToList();
                            break;
                    }

                    break;

                default:
                    break;
            }
            return planGroupList;
        }

        #endregion 取得一般计划分组信息共通方法

        #region 删除循环计划附件信息通用方法

        /// <summary>
        /// 根据loopId删除循环计划附件信息通用方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="targetId"></param>
        private void DelLoopPlanAttachInfo(TargetNavigationDBEntities db, IQueryable<tblLoopplanAttachment> loopAttachs)
        {
            if (loopAttachs.Any())
            {
                foreach (var attach in loopAttachs)
                {
                    db.tblLoopplanAttachment.Remove(attach);
                }
            }
        }

        #endregion 删除循环计划附件信息通用方法

        #region 根据Id查询循环计划信息通用方法

        /// <summary>
        /// 根据Id查询循环计划信息通用方法
        /// </summary>
        /// <param name="loopId"></param>
        /// <param name="db">连接数据库上下文</param>
        /// <returns>实体对象</returns>
        private tblLoopPlan GetTblLoopPlanInfoById(int loopId, TargetNavigationDBEntities db)
        {
            return db.tblLoopPlan.Where(p => p.loopId == loopId).FirstOrDefault<tblLoopPlan>();
        }

        #endregion 根据Id查询循环计划信息通用方法

        #region 根据提交Id查询循环计划提交信息通用方法

        /// <summary>
        /// 根据Id查询循环计划信息通用方法
        /// </summary>
        /// <param name="submitId"></param>
        /// <param name="db">连接数据库上下文</param>
        /// <returns>实体对象</returns>
        private tblLoopplanSubmit GetLoopPlanSubmintInfoById(int submitId, TargetNavigationDBEntities db)
        {
            return db.tblLoopplanSubmit.Where(p => p.submitId == submitId).FirstOrDefault<tblLoopplanSubmit>();
        }

        #endregion 根据提交Id查询循环计划提交信息通用方法

        #region 取得循环计划状态数量通用方法

        /// <summary>
        /// 取得循环计划状态数量通用方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="listType"></param>
        /// <param name="userId"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        private PlanStatusCountModel GetLoopPlanStatusInfo(TargetNavigationDBEntities db, EnumDefine.PlanListType listType, int userId, DateTime? fromTime = null, DateTime? toTime = null)
        {
            PlanStatusCountModel planStatusCountModel = new PlanStatusCountModel();
            List<int> cooperationPlans = new List<int>();
            var condition = new StringBuilder().Append(" deleteFlag == false ");
            var values = new List<object>();

            //列表类型筛选
            condition.Append(GetListTypeSql(listType, userId, values));

            if (fromTime != null)
            {
                condition.Append(" AND endTime >=@");
                condition.Append(values.Count);
                values.Add(fromTime.Value);
            }
            if (toTime != null)
            {
                condition.Append(" AND endTime <=@");
                condition.Append(values.Count);
                values.Add(toTime.Value);
            }

            //取得循环计划列表
            var statusList = db.vLoopPlanList.Where(condition.ToString(), values.ToArray()).ToList();
            if (statusList.Count <= 0) return planStatusCountModel;
            //筛选出协作计划的场合
            if (listType == EnumDefine.PlanListType.Cooperation)
            {
                // 协作人
                cooperationPlans = db.tblLoopPlanCooperation.Where(p => p.userId == userId).Select(p => p.loopId).ToList();
                statusList = statusList.Where(p => cooperationPlans.Contains(p.loopId)).ToList();
            }
            //过滤掉时间过期已完成的循环计划
            var completeList = statusList.Where(p => p.endTime < DateTime.Today).ToList();
            if (completeList.Count > 0)
            {
                planStatusCountModel.loopCompletedCount = completeList.Count;
                completeList.ForEach(p => statusList.Remove(p));
            }
            foreach (var item in statusList)
            {
                switch (item.stop)
                {
                    //申请中止
                    case (int)EnumDefine.LoopPlanStopStatus.Checking:
                        planStatusCountModel.planCheckingCount++;
                        break;
                    //已中止
                    case (int)EnumDefine.LoopPlanStopStatus.Stopped:
                        planStatusCountModel.planStopCount++;
                        break;
                    //进行中
                    case (int)EnumDefine.LoopPlanStopStatus.Running:
                        switch (item.status)
                        {
                            // 待提交
                            case (int)EnumDefine.LoopPlanStatus.UnSubmit:
                            case (int)EnumDefine.LoopPlanStatus.CheckDenied:
                                planStatusCountModel.planSubmitingCount++;
                                break;
                            // 待审核
                            case (int)EnumDefine.LoopPlanStatus.RequestEdit:
                            case (int)EnumDefine.LoopPlanStatus.Checking:
                                planStatusCountModel.planCheckingCount++;
                                break;
                            // 已审核
                            case (int)EnumDefine.LoopPlanStatus.CheckPassed:
                                planStatusCountModel.planCheckedCount++;
                                //统计循环计划待确认的数量
                                var loopPlanSubmitList = db.tblLoopplanSubmit.Where(p => p.loopId == item.loopId).ToList();
                                if (loopPlanSubmitList.Where(p => p.completeStatus == (int)EnumDefine.LoopPlanSubmitStatus.Confirming).Any())
                                {
                                    planStatusCountModel.planConfirmingCount++;
                                }
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
            return planStatusCountModel;
        }

        #endregion 取得循环计划状态数量通用方法

        #region 取得循环计划分组信息共通方法

        /// <summary>
        /// 取得循环计划分组信息共通方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        private List<PlanGroupInfoModel> GetLoopPlanGourpInfo(TargetNavigationDBEntities db, int userId, EnumDefine.PlanGroupType groupType, EnumDefine.PlanListType listType)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            List<PlanGroupInfoModel> loopPlanGroupList = null;
            switch (groupType)
            {
                //时间
                case EnumDefine.PlanGroupType.Time:
                    loopPlanGroupList = new List<PlanGroupInfoModel>{
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Earlier.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Yesterday.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Today.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Tomorrow.ToString()},
                        new PlanGroupInfoModel{groupId=EnumDefine.TimeGroup.Later.ToString()}
                    };
                    break;
                //部门
                case EnumDefine.PlanGroupType.Organization:
                    switch (listType)
                    {
                        //我的计划场合
                        case EnumDefine.PlanListType.Mine:
                            //loopPlanGroupList = db.tblLoopPlan.Where(p => !p.deleteFlag && p.responsibleUser == userId).GroupBy(p => p.organizationId).Select(p =>
                            //     new PlanGroupInfoModel
                            //     {
                            //         groupId = p.Key.ToString(),
                            //         groupName = db.tblOrganization.Where(o => o.organizationId == p.Key.Value).FirstOrDefault().organizationName
                            //     }).ToList();
                            loopPlanGroupList = (from p in db.tblLoopPlan
                                                 where !p.deleteFlag && p.responsibleUser == userId
                                                 group p by p.organizationId into g
                                                 select new PlanGroupInfoModel
                                                 {
                                                     groupId = g.Key.ToString(),
                                                     groupName = db.tblOrganization.Where(o => o.organizationId == g.Key.Value).FirstOrDefault().organizationName
                                                 }).ToList();
                            break;
                        //下属计划场合
                        case EnumDefine.PlanListType.Subordinate:
                            loopPlanGroupList = (from p in db.tblLoopPlan
                                                 where !p.deleteFlag && p.confirmUser == userId
                                                 group p by p.organizationId into g
                                                 select new PlanGroupInfoModel
                                                 {
                                                     groupId = g.Key.ToString(),
                                                     groupName = db.tblOrganization.Where(o => o.organizationId == g.Key.Value).FirstOrDefault().organizationName
                                                 }).ToList();
                            break;
                        //协作计划场合
                        case EnumDefine.PlanListType.Cooperation:
                            loopPlanGroupList = (from p in db.tblLoopPlan
                                                 join cp in db.tblLoopPlanCooperation on p.loopId equals cp.loopId
                                                 where !p.deleteFlag && cp.userId == userId
                                                 group p by p.organizationId into g
                                                 select new PlanGroupInfoModel
                                                 {
                                                     groupId = g.Key.ToString(),
                                                     groupName = db.tblOrganization.Where(o => o.organizationId == g.Key.Value).FirstOrDefault().organizationName
                                                 }).ToList();
                            break;
                    }

                    break;
                //人员
                case EnumDefine.PlanGroupType.User:
                    switch (listType)
                    {
                        //我的计划场合
                        case EnumDefine.PlanListType.Mine:
                            loopPlanGroupList = (from p in db.tblLoopPlan
                                                 where !p.deleteFlag && p.responsibleUser == userId
                                                 group p by p.confirmUser into g
                                                 select new PlanGroupInfoModel
                                                 {
                                                     groupId = g.Key.ToString(),
                                                     groupName = db.tblUser.Where(u => u.userId == g.Key.Value).FirstOrDefault().userName
                                                 }).ToList();
                            break;
                        //下属计划场合
                        case EnumDefine.PlanListType.Subordinate:
                            loopPlanGroupList = (from p in db.tblLoopPlan
                                                 where !p.deleteFlag && p.confirmUser == userId
                                                 group p by p.responsibleUser into g
                                                 select new PlanGroupInfoModel
                                                 {
                                                     groupId = g.Key.ToString(),
                                                     groupName = db.tblUser.Where(u => u.userId == g.Key.Value).FirstOrDefault().userName
                                                 }).ToList();
                            break;
                        //协作计划场合
                        case EnumDefine.PlanListType.Cooperation:
                            loopPlanGroupList = (from p in db.tblLoopPlan
                                                 join cp in db.tblLoopPlanCooperation on p.loopId equals cp.loopId
                                                 where !p.deleteFlag && cp.userId == userId
                                                 group p by p.responsibleUser into g
                                                 select new PlanGroupInfoModel
                                                 {
                                                     groupId = g.Key.ToString(),
                                                     groupName = db.tblUser.Where(u => u.userId == g.Key.Value).FirstOrDefault().userName
                                                 }).ToList();
                            break;
                    }

                    break;

                default:
                    break;
            }
            return loopPlanGroupList;
        }

        #endregion 取得循环计划分组信息共通方法

        #endregion 私有方法
    }
}