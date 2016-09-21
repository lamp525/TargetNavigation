using MB.DAL;
using MB.New.BLL.Organization;
using MB.New.BLL.Plan;
using MB.New.BLL.Tag;
using MB.New.BLL.User;
using MB.New.BLL.WorkTime;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.Facade.Plan
{
    public class PlanFacade : IPlanFacade
    {
        #region 新建画面绑定

        /// <summary>
        /// 取得默认计划信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public PagePlanDefaultInfoModel GetDefaultPlanInfo(int userId)
        {
            IOrganizationBLL Organization = new OrganizationBLL();
            IPlanBLL Plan = new PlanBLL();
            IUserBLL User = new UserBLL();

            var result = new PagePlanDefaultInfoModel();
            using (var db = new TargetNavigationDBEntities())
            {
                // 取得最新计划信息
                var info = Plan.GetLatestPlanInfo(db, userId);

                if (info != null)
                {
                    //部门信息
                    var orgInfoList = Organization.GetParentOrgByOrgId(db, info.organizationId.Value, ConstVar.PlanOrgSpliceNum).OrderByDescending(x => x.level).Select(x => x.organizationName).ToList();
                    info.organizationInfo = StringUtility.ListToString(orgInfoList, "-");

                    result = ModelMapping.JsonMapping<PlanInfoModel, PagePlanDefaultInfoModel>(info);
                }
                else
                {
                    //责任人信息
                    var responsibleUserInfo = User.GetUserDefaultInfo(db, userId);
                    result.responsibleUser = responsibleUserInfo.userId;
                    result.responsibleUserName = responsibleUserInfo.userName;
                    result.responsibleUserImage = responsibleUserInfo.headImage;

                    //确认人信息
                    var confirmUserInfo = User.GetSuperiorInfoByStationId(db, responsibleUserInfo.stationId.Value).FirstOrDefault();
                    if (confirmUserInfo != null)
                    {
                        result.confirmUser = confirmUserInfo.userId;
                        result.confirmUserName = confirmUserInfo.userName;
                        result.confirmUserImage = confirmUserInfo.headImage;
                    }

                    //执行方式信息
                    var executionModeInfo = Plan.GetExecutionInfo(db).FirstOrDefault();
                    result.executionModeId = executionModeInfo.executionId;
                    result.executionMode = executionModeInfo.executionMode;

                    //部门信息
                    var orgInfoList = Organization.GetParentOrgByOrgId(db, responsibleUserInfo.orgId.Value, ConstVar.PlanOrgSpliceNum).OrderByDescending(x => x.level).Select(x => x.organizationName).ToList();
                    result.organizationInfo = StringUtility.ListToString(orgInfoList, "-");
                    result.organizationId = responsibleUserInfo.orgId;

                    //临时标志
                    result.initial = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// 取得默认循环计划信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageLoopPlanDefaultInfoModel GetDefaultLoopPlanInfo(int userId)
        {
            IOrganizationBLL Organization = new OrganizationBLL();
            IPlanBLL Plan = new PlanBLL();
            IUserBLL User = new UserBLL();

            var result = new PageLoopPlanDefaultInfoModel();
            using (var db = new TargetNavigationDBEntities())
            {
                //取得最新的循环计划信息
                var info = Plan.GetLatestLoopPlanInfo(db, userId);
                if (info != null)
                {
                    //部门信息
                    var orgInfoList = Organization.GetParentOrgByOrgId(db, info.organizationId.Value, ConstVar.PlanOrgSpliceNum).OrderByDescending(x => x.level).Select(x => x.organizationName).ToList();
                    info.organizationInfo = StringUtility.ListToString(orgInfoList, "-");

                    result = ModelMapping.JsonMapping<LoopPlanInfoModel, PageLoopPlanDefaultInfoModel>(info);
                }
                else
                {
                    //责任人信息
                    var responsibleUserInfo = User.GetUserDefaultInfo(db, userId);
                    result.responsibleUser = responsibleUserInfo.userId;
                    result.responsibleUserName = responsibleUserInfo.userName;
                    result.responsibleUserImage = responsibleUserInfo.headImage;

                    //确认人信息
                    var confirmUserInfo = User.GetSuperiorInfoByStationId(db, responsibleUserInfo.stationId.Value).FirstOrDefault();
                    if (confirmUserInfo != null)
                    {
                        result.confirmUser = confirmUserInfo.userId;
                        result.confirmUserName = confirmUserInfo.userName;
                        result.confirmUserImage = confirmUserInfo.headImage;
                    }

                    //执行方式信息
                    var executionModeInfo = Plan.GetExecutionInfo(db).FirstOrDefault();
                    result.executionModeId = executionModeInfo.executionId;
                    result.executionMode = executionModeInfo.executionMode;

                    //部门信息
                    var orgInfoList = Organization.GetParentOrgByOrgId(db, responsibleUserInfo.orgId.Value, ConstVar.PlanOrgSpliceNum).OrderByDescending(x => x.level).Select(x => x.organizationName).ToList();
                    result.organizationInfo = StringUtility.ListToString(orgInfoList, "-");
                    result.organizationId = responsibleUserInfo.orgId;

                    //循环类型
                    result.loopType = EnumDefine.LoopPlanLoopType.Day;
                }
            }
            return result;
        }

        /// <summary>
        /// 取得执行方法列表信息
        /// </summary>
        /// <returns></returns>
        public List<PageExecutionInfoModel> GetExecutionInfo()
        {
            IPlanBLL Plan = new PlanBLL();

            var result = new List<PageExecutionInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = Plan.GetExecutionInfo(db);

                result = ModelMapping.JsonMapping<List<ExecutionInfoModel>, List<PageExecutionInfoModel>>(info);
            }

            return result;
        }

        /// <summary>
        /// 取得用户常用标签
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string[] GetMostUsedTag(int userId)
        {
            ITagManagementBLL Tag = new TagManagementBLL();

            return Tag.GetMostUsedTag(userId);
        }

        #endregion 新建画面绑定

        #region 计划（新建、更新和详情）

        /// <summary>
        /// 快捷新建计划
        /// </summary>
        /// <param name="quickAddPlan"></param>
        public bool QuickAddPlan(int userId, PageQuickAddModel quickAddPlan)
        {
            IUserBLL User = new UserBLL();
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //取得用户的默认信息
                var userInfo = User.GetUserDefaultInfo(db, userId);
                if (userInfo == null) return false;

                //取得用户的直属上级信息
                var superiorInfo = User.GetSuperiorInfoByStationId(db, userInfo.stationId.Value);
                if (superiorInfo == null || superiorInfo.Count == 0) return false;

                //当前时间
                var now = DateTime.Now;

                //计划信息设置
                var planInfo = new PlanInfoModel
                {
                    //事项输出结果
                    eventOutput = quickAddPlan.eventOutput,
                    //计划完成时间
                    endTime = quickAddPlan.endTime,
                    //部门ID
                    organizationId = userInfo.orgId,
                    //责任人
                    responsibleUser = userId,
                    //确认人
                    confirmUser = superiorInfo.First().userId,
                    //临时计划标志 0：临时计划 1：目标计划
                    initial = 1,
                    //状态 0：待提交
                    status = EnumDefine.PlanStatus.UnSubmit,
                    //中止状态 0：运行中
                    stop = EnumDefine.PlanStopStatus.Running,
                    //计划创建时间
                    planGenerateTime = now,
                    //创建用户
                    createUser = userId,
                    //创建时间
                    createTime = now,
                    //更新用户
                    updateUser = userId,
                    //更新时间
                    updateTime = now,
                    //进度
                    progress = 0,
                    //执行方式
                    executionModeId = GetExecutionInfo().Where(c => c.executionMode == "编制完成").Select(c => c.executionId).FirstOrDefault()
                };

                //添加计划信息
                var planId = Plan.InsPlanInfo(db, planInfo);

                //计划操作信息
                var logInfo = new PlanLogModel
                {
                    //计划ID
                    planId = planId,
                    //操作意见
                    message = string.Empty,
                    //操作类型
                    type = EnumDefine.PlanOperateStatus.CreatePlan,
                    //操作用户
                    operateUser = userId,
                    //操作时间
                    operateTime = now
                };

                //添加计划日志
                PlanLog.InsPlanLog(db, logInfo);

                db.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 新建/更新计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planInfo"></param>
        /// <returns></returns>
        public void AddOrUpdatePlan(int userId, PagePlanInfoModel planInfo)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            ITagManagementBLL Tag = new TagManagementBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                #region 计划和日志信息设置

                //新建操作日志信息
                PlanLogModel newPlanLogModel = null;
                //提交操作日志信息
                PlanLogModel submitPlanLogModel = null;
                //审核通过操作日志信息
                PlanLogModel approvedPlanLogModel = null;
                //修改操作日志信息
                PlanLogModel editPlanLogModel = null;

                //计划信息
                var planModel = ModelMapping.JsonMapping<PagePlanInfoModel, PlanInfoModel>(planInfo);

                //计划ID
                var planId = planModel.planId;
                //当前时间
                var now = DateTime.Now;

                //进度
                planModel.progress = 0;
                //中止状态
                planModel.stop = EnumDefine.PlanStopStatus.Running;
                //更新用户
                planModel.updateUser = userId;
                //更新时间
                planModel.updateTime = now;

                //提交计划的场合
                if (planModel.isSubmit.HasValue && planModel.isSubmit.Value)
                {
                    //提交日志信息
                    submitPlanLogModel = new PlanLogModel();
                    submitPlanLogModel.type = EnumDefine.PlanOperateStatus.Submit;
                    submitPlanLogModel.operateUser = userId;
                    submitPlanLogModel.operateTime = now;
                    submitPlanLogModel.message = string.Empty;

                    //临时计划
                    if (planModel.initial == 0)
                    {
                        //计划信息
                        planModel.status = EnumDefine.PlanStatus.CheckPassed;
                        planModel.importance = 1;
                        planModel.urgency = 1;
                        planModel.difficulty = 1;
                        planModel.planGenerateTime = now;
                        planModel.auditTime = now;

                        //审核通过日志信息
                        approvedPlanLogModel = new PlanLogModel();
                        approvedPlanLogModel.type = EnumDefine.PlanOperateStatus.CheckPass;
                        approvedPlanLogModel.operateUser = planInfo.confirmUser.Value;
                        approvedPlanLogModel.operateTime = now;
                        approvedPlanLogModel.message = "临时计划默认审核通过！";
                    }
                    //一般计划
                    else if (planModel.initial == 1)
                    {
                        //计划信息
                        planModel.status = EnumDefine.PlanStatus.Checking;
                        planModel.importance = 0;
                        planModel.urgency = 0;
                        planModel.difficulty = 0;
                        planModel.planGenerateTime = now;
                    }
                }
                //保存计划的场合
                if (planModel.isSubmit.HasValue && !planModel.isSubmit.Value)
                {
                    planModel.status = EnumDefine.PlanStatus.UnSubmit;
                }

                #endregion 计划和日志信息设置

                //新建计划的场合
                if (!planId.HasValue)
                {
                    //计划信息
                    planModel.createUser = userId;
                    planModel.createTime = now;

                    //插入计划信息
                    planId = Plan.InsPlanInfo(db, planModel);

                    //新建操作日志信息
                    newPlanLogModel = new PlanLogModel();
                    newPlanLogModel.planId = planId.Value;
                    newPlanLogModel.type = EnumDefine.PlanOperateStatus.CreatePlan;
                    newPlanLogModel.operateUser = userId;
                    newPlanLogModel.operateTime = now;
                    newPlanLogModel.message = string.Empty;

                    //插入新建操作日志信息
                    PlanLog.InsPlanLog(db, newPlanLogModel);
                }
                //修改计划的场合
                else
                {
                    //更新计划信息
                    Plan.UpdPlanInfo(db, planModel);

                    //保存计划的场合
                    if (planModel.isSubmit.HasValue && !planModel.isSubmit.Value)
                    {
                        //修改操作日志信息
                        editPlanLogModel = new PlanLogModel();
                        editPlanLogModel.planId = planId.Value;
                        editPlanLogModel.type = EnumDefine.PlanOperateStatus.UpdateSave;
                        editPlanLogModel.operateUser = userId;
                        editPlanLogModel.operateTime = now;
                        editPlanLogModel.message = string.Empty;

                        //插入修改操作日志信息
                        PlanLog.InsPlanLog(db, editPlanLogModel);
                    }
                }

                //插入提交操作日志信息
                if (submitPlanLogModel != null)
                {
                    submitPlanLogModel.planId = planId.Value;
                    PlanLog.InsPlanLog(db, submitPlanLogModel);
                }
                //插入审核通过操作日志信息
                if (approvedPlanLogModel != null)
                {
                    approvedPlanLogModel.planId = planId.Value;
                    PlanLog.InsPlanLog(db, approvedPlanLogModel);
                }

                #region 协作人处理

                //删除原有的协作人信息
                Plan.DeletePlanPartnerInfo(db, planId.Value);
                db.SaveChanges();

                //添加协作人信息
                if (planModel.partnerInfo != null && planModel.partnerInfo.Count > 0)
                {
                    foreach (var item in planModel.partnerInfo)
                    {
                        Plan.InsPlanPartnerInfo(db, item.userId, planId.Value);
                    }
                }

                #endregion 协作人处理

                db.SaveChanges();

                #region 标签处理

                //提交的场合
                if (planModel.isSubmit.HasValue && planModel.isSubmit.Value)
                {
                    //保存标签至缓存
                    Tag.SavePlanTagAsync(db, userId, planId.Value);
                }

                #endregion 标签处理
            }
        }

        /// <summary>
        /// 新建/更新循环计划
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopPlanInfo"></param>
        /// <returns></returns>
        public void AddOrUpdateLoopPlan(int userId, PageLoopPlanInfoModel loopPlanInfo)
        {
            IPlanBLL Plan = new PlanBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();
            ITagManagementBLL Tag = new TagManagementBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                #region 计划和日志信息设置

                //新建操作日志信息
                PlanLogModel newPlanLogModel = null;
                //提交操作日志信息
                PlanLogModel submitPlanLogModel = null;
                //修改操作日志信息
                PlanLogModel editPlanLogModel = null;

                //循环计划信息
                var loopPlanModel = ModelMapping.JsonMapping<PageLoopPlanInfoModel, LoopPlanInfoModel>(loopPlanInfo);

                //循环ID
                var loopId = loopPlanModel.loopId;
                //当前时间
                var now = DateTime.Now;

                //中止状态
                loopPlanModel.stop = EnumDefine.LoopPlanStopStatus.Running;
                //更新用户
                loopPlanModel.updateUser = userId;
                //更新时间
                loopPlanModel.updateTime = now;

                //提交计划的场合
                if (loopPlanModel.isSubmit.HasValue && loopPlanModel.isSubmit.Value)
                {
                    //提交日志信息
                    submitPlanLogModel = new PlanLogModel();
                    submitPlanLogModel.type = EnumDefine.PlanOperateStatus.Submit;
                    submitPlanLogModel.operateUser = userId;
                    submitPlanLogModel.operateTime = now;
                    submitPlanLogModel.message = string.Empty;

                    //循环计划信息
                    loopPlanModel.status = EnumDefine.LoopPlanStatus.Checking;
                    loopPlanModel.importance = 0;
                    loopPlanModel.urgency = 0;
                    loopPlanModel.difficulty = 0;
                }

                //保存计划的场合
                if (loopPlanModel.isSubmit.HasValue && !loopPlanModel.isSubmit.Value)
                {
                    loopPlanModel.status = EnumDefine.LoopPlanStatus.UnSubmit;
                }

                #endregion 计划和日志信息设置

                //新建循环计划的场合
                if (!loopId.HasValue)
                {
                    //循环计划信息
                    loopPlanModel.createUser = userId;
                    loopPlanModel.createTime = now;

                    //插入循环计划信息
                    loopId = Plan.InsLoopPlanInfo(db, loopPlanModel);

                    //新建操作日志信息
                    newPlanLogModel = new PlanLogModel();
                    newPlanLogModel.planId = loopId.Value;
                    newPlanLogModel.type = EnumDefine.PlanOperateStatus.CreatePlan;
                    newPlanLogModel.operateUser = userId;
                    newPlanLogModel.operateTime = now;
                    newPlanLogModel.message = string.Empty;

                    //插入新建操作日志信息
                    PlanLog.InsLoopPlanLog(db, newPlanLogModel);
                }
                //修改循环计划的场合
                else
                {
                    //更新循环计划信息
                    Plan.UpdLoopPlanInfo(db, loopPlanModel);

                    //保存循环计划计划的场合
                    if (loopPlanModel.isSubmit.HasValue && !loopPlanModel.isSubmit.Value)
                    {
                        //修改操作日志信息
                        editPlanLogModel = new PlanLogModel();
                        editPlanLogModel.planId = loopId.Value;
                        editPlanLogModel.type = EnumDefine.PlanOperateStatus.UpdateSave;
                        editPlanLogModel.operateUser = userId;
                        editPlanLogModel.operateTime = now;
                        editPlanLogModel.message = string.Empty;

                        //插入修改操作日志信息
                        PlanLog.InsLoopPlanLog(db, editPlanLogModel);
                    }
                }

                //插入提交操作日志信息
                if (submitPlanLogModel != null)
                {
                    submitPlanLogModel.planId = loopId.Value;
                    PlanLog.InsLoopPlanLog(db, submitPlanLogModel);
                }

                db.SaveChanges();

                #region 标签处理

                //提交的场合
                if (loopPlanModel.isSubmit.HasValue && loopPlanModel.isSubmit.Value)
                {
                    //保存标签至缓存
                    Tag.SavePlanTagAsync(db, userId, loopId.Value);
                }

                #endregion 标签处理
            }
        }

        /// <summary>
        /// 取得计划详情
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public PagePlanInfoModel GetPlanInfo(int planId)
        {
            IOrganizationBLL Organization = new OrganizationBLL();
            IPlanBLL Plan = new PlanBLL();

            var result = new PagePlanInfoModel();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = Plan.GetPlanInfoById(db, planId);

                if (info != null)
                {
                    //取得计划部门信息
                    var orgInfoList = Organization.GetParentOrgByOrgId(db, info.organizationId.Value, ConstVar.PlanOrgSpliceNum).OrderByDescending(x => x.level).Select(x => x.organizationName).ToList();
                    info.organizationInfo = StringUtility.ListToString(orgInfoList, "-");

                    //取得计划协作人信息
                    info.partnerInfo = Plan.GetPlanPartnerInfo(db, planId);

                    //标签处理
                    info.keyword = string.IsNullOrEmpty(info.keywordInfo) ? null : info.keywordInfo.Split(',');

                    result = ModelMapping.JsonMapping<PlanInfoModel, PagePlanInfoModel>(info);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得循环计划详情
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public PageLoopPlanInfoModel GetLoopPlanInfo(int loopId)
        {
            IOrganizationBLL Organization = new OrganizationBLL();
            IPlanBLL Plan = new PlanBLL();

            var result = new PageLoopPlanInfoModel();
            using (var db = new TargetNavigationDBEntities())
            {
                var info = Plan.GetLoopPlanInfoById(db, loopId);

                if (info != null)
                {
                    //取得计划部门信息
                    var orgInfoList = Organization.GetParentOrgByOrgId(db, info.organizationId.Value, ConstVar.PlanOrgSpliceNum).OrderByDescending(x => x.level).Select(x => x.organizationName).ToList();
                    info.organizationInfo = StringUtility.ListToString(orgInfoList, "-");

                    //标签处理
                    info.keyword = string.IsNullOrEmpty(info.keywordInfo) ? null : info.keywordInfo.Split(',');

                    result = ModelMapping.JsonMapping<LoopPlanInfoModel, PageLoopPlanInfoModel>(info);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得循环计划提交信息
        /// </summary>
        /// <param name="loopId"></param>
        /// <param name="mode"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public List<PageLoopPlanSubmitInfoModel> GetLoopPlanSubmitInfo(int loopId, EnumDefine.rollMode mode, int pageNum)
        {
            IPlanBLL Plan = new PlanBLL();

            var result = new List<PageLoopPlanSubmitInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var loopPlanInfo = Plan.GetLoopPlanInfoById(db, loopId);
                switch (mode)
                {
                    //取得默认条数的最新提交信息
                    case EnumDefine.rollMode.None:
                        var defaultList = Plan.GetLoopPlanSubmitInfo(db, loopId, pageNum);
                        if (defaultList != null)
                        {
                            var num = ConstVar.LoopPlanSubmitPageNum - defaultList.Count;

                            defaultList.AddRange(Plan.GenerateLoopPlanEmptySubmitInfo(loopPlanInfo.loopType, loopPlanInfo.startTime.Value, num));
                        }
                        result = ModelMapping.JsonMapping<List<LoopPlanSubmitInfoModel>, List<PageLoopPlanSubmitInfoModel>>(defaultList);
                        break;

                    //向上翻页取得之前的提交信息
                    case EnumDefine.rollMode.Up:
                        var upList = Plan.GetLoopPlanSubmitInfo(db, loopId, pageNum);
                        result = ModelMapping.JsonMapping<List<LoopPlanSubmitInfoModel>, List<PageLoopPlanSubmitInfoModel>>(upList);
                        break;

                    //向下翻页取得之后的提交信息
                    case EnumDefine.rollMode.Down:
                        var downList = Plan.GenerateLoopPlanEmptySubmitInfo(loopPlanInfo.loopType, loopPlanInfo.startTime.Value, ConstVar.LoopPlanSubmitPageNum);
                        result = ModelMapping.JsonMapping<List<LoopPlanSubmitInfoModel>, List<PageLoopPlanSubmitInfoModel>>(downList);
                        break;
                }
            }

            return result;
        }

        #endregion 计划（新建、更新和详情）

        #region 操作日志

        /// <summary>
        /// 取得一般计划操作日志
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<PagePlanLogInfoModel> GetPlanLogInfo(int planId)
        {
            IUserBLL User = new UserBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            var result = new List<PagePlanLogInfoModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var infoList = PlanLog.GetPlanLogByPlanId(db, planId);
                foreach (var info in infoList)
                {
                    var log = new PagePlanLogInfoModel
                    {
                        message = info.message,
                        operateTime = info.operateTime,
                        operateUser = User.GetUserInfoById(db, info.operateUser).userName,
                        operateInfo = GetOperateInfo(info)
                    };

                    result.Add(log);
                }
            }
            return result;
        }

        /// <summary>
        /// 取得循环计划操作日志
        /// </summary>
        /// <param name="loopId"></param>
        /// <returns></returns>
        public List<PagePlanLogInfoModel> GetLoopPlanLogInfo(int loopId)
        {
            IUserBLL User = new UserBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            var result = new List<PagePlanLogInfoModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var infoList = PlanLog.GetLoopLogByLoopId(db, loopId);
                foreach (var info in infoList)
                {
                    var log = new PagePlanLogInfoModel
                    {
                        message = info.message,
                        operateTime = info.operateTime,
                        operateUser = User.GetUserInfoById(db, info.operateUser).userName,
                        operateInfo = GetOperateInfo(info)
                    };

                    result.Add(log);
                }
            }
            return result;
        }

        #endregion 操作日志

        #region 计划评论

        /// <summary>
        /// 取得计划评论信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<PagePlanCommentModel> GetPlanCommentInfo(int planId)
        {
            IPlanCommentBLL PlanComment = new PlanCommentBLL();
            var result = new List<PagePlanCommentModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = PlanComment.GetPlanCommentInfo(db, planId);

                result = ModelMapping.JsonMapping<List<PlanCommentModel>, List<PagePlanCommentModel>>(info);
            }

            return result;
        }

        /// <summary>
        /// 添加计划评论
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="commentInfo"></param>
        public void AddPlanComment(int userId, PagePlanCommentModel commentInfo)
        {
            IPlanCommentBLL PlanComment = new PlanCommentBLL();
            IPlanLogBLL PlanLog = new PlanLogBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //当前时间
                var now = DateTime.Now;

                var model = new PlanCommentModel
                {
                    planId = commentInfo.planId,
                    suggestion = commentInfo.suggestion,
                    replyUser = userId,
                    createUser = userId,
                    createTime = now,
                    updateUser = userId,
                    updateTime = now,
                };

                PlanComment.InsPlanComment(db, model);

                //添加操作日志
                var planModel = new PlanLogModel
                {
                    planId = commentInfo.planId.Value,
                    message = commentInfo.suggestion,
                    operateUser = userId,
                    operateTime = now,
                    type = EnumDefine.PlanOperateStatus.Discuss
                };
                PlanLog.InsPlanLog(db, planModel);

                db.SaveChanges();
            }
        }

        #endregion 计划评论

        #region 我的计划

        /// <summary>
        /// 取得我的计划状态数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetMyPlanStatusInfo(int userId)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetMyPlanStatusInfo(db, userId, null, null);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        /// <summary>
        /// 取得我的计划列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PagePlanInfoModel> GetMyPlanList(int userId, PagePlanSearchModel searchInfo)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planInfoList = new List<PagePlanInfoModel>();
                //单个对象映射--PlanSearchModel
                var search = ModelMapping.JsonMapping<PagePlanSearchModel, PlanSearchModel>(searchInfo);
                search.userId = userId;
                var source = Plan.GetMyPlanList(db, search);
                planInfoList = ModelMapping.JsonMapping<List<PlanInfoModel>, List<PagePlanInfoModel>>(source);
                return planInfoList;
            }
        }

        /// <summary>
        /// 取得我的循环计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PageLoopPlanInfoModel> GetMyLoopPlanList(int userId, PagePlanSearchModel searchInfo)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planInfoList = new List<PageLoopPlanInfoModel>();
                //单个对象映射--PlanSearchModel
                var search = ModelMapping.JsonMapping<PagePlanSearchModel, PlanSearchModel>(searchInfo);
                var source = Plan.GetMyLoopPlanList(db, search);
                planInfoList = ModelMapping.JsonMapping<List<LoopPlanInfoModel>, List<PageLoopPlanInfoModel>>(source);
                return planInfoList;
            }
        }

        /// <summary>
        /// 取得我的一般计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PagePlanGroupInfoModel> GetMyPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetMyPlanGourpInfo(db, userId, groupType);
                var planGroupInfo = ModelMapping.JsonMapping<List<PlanGroupInfoModel>, List<PagePlanGroupInfoModel>>(source);
                return planGroupInfo;
            }
        }

        /// <summary>
        /// 取得我的循环计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PagePlanGroupInfoModel> GetMyLoopPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetMyLoopPlanGourpInfo(db, userId, groupType);
                var planGroupInfo = ModelMapping.JsonMapping<List<PlanGroupInfoModel>, List<PagePlanGroupInfoModel>>(source);
                return planGroupInfo;
            }
        }

        /// <summary>
        /// 取得我的月有效工时信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PageMonthWorkTimeModel GetMyMonthWorkTimeInfo(int userId, string yearMonth)
        {
            IWorkTimeBLL WorkTime = new WorkTimeBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var monthWorkTimeModel = new PageMonthWorkTimeModel
                {
                    monthTotalWorkTime = WorkTime.GetPerWorkTimeByMonth(db, userId, yearMonth),
                    monthAvgWorkTime = WorkTime.GetPerWorkTimeByMonth(db, userId, yearMonth) / DateTime.DaysInMonth(int.Parse(yearMonth.Substring(0, 4)), int.Parse(yearMonth.Substring(4, 2)))
                };

                return monthWorkTimeModel;
            }
        }

        /// <summary>
        /// 取得我的月计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetMyPlanPieChartInfo(int userId, string yearMonth)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //获取参数月第一天
                var fromTime = DateUtility.GetMonthFirstDay(DateTime.Parse(yearMonth));
                //获取参数月的最后一天
                var toTime = DateUtility.GetMonthLastDay(DateTime.Parse(yearMonth));
                var source = Plan.GetMyPlanStatusInfo(db, userId, fromTime, toTime);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        /// <summary>
        /// 取得我的月循环计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetMyLoopPlanPieChartInfo(int userId, string yearMonth)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //获取参数月第一天
                var fromTime = DateUtility.GetMonthFirstDay(DateTime.Parse(yearMonth));
                //获取参数月的最后一天
                var toTime = DateUtility.GetMonthLastDay(DateTime.Parse(yearMonth));
                var source = Plan.GetMyLoopPlanStatusInfo(db, userId, fromTime, toTime);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        #endregion 我的计划

        #region 下属计划

        /// <summary>
        /// 取得下属计划状态数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetSubordinatePlanStatusInfo(int userId)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetSubordinatePlanStatusInfo(db, userId, null, null);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        /// <summary>
        /// 取得下属计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PagePlanInfoModel> GetSubordinatePlanList(int userId, PagePlanSearchModel searchInfo)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planInfoList = new List<PagePlanInfoModel>();

                //单个对象映射--PlanSearchModel
                var search = ModelMapping.JsonMapping<PagePlanSearchModel, PlanSearchModel>(searchInfo);
                var source = Plan.GetSubordinatePlanList(db, search);
                planInfoList = ModelMapping.JsonMapping<List<PlanInfoModel>, List<PagePlanInfoModel>>(source);
                return planInfoList;
            }
        }

        /// <summary>
        /// 取得下属循环计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PagePlanInfoModel> GetSubordinateLoopPlanList(int userId, PagePlanSearchModel searchInfo)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var planInfoList = new List<PagePlanInfoModel>();

                //单个对象映射--PlanSearchModel
                var search = ModelMapping.JsonMapping<PagePlanSearchModel, PlanSearchModel>(searchInfo);
                var source = Plan.GetSubordinateLoopPlanList(db, search);
                planInfoList = ModelMapping.JsonMapping<List<LoopPlanInfoModel>, List<PagePlanInfoModel>>(source);
                return planInfoList;
            }
        }

        /// <summary>
        /// 取得下属一般计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PagePlanGroupInfoModel> GetSubordinatePlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetSubordinatePlanGourpInfo(db, userId, groupType);
                var planList = ModelMapping.JsonMapping<List<PlanGroupInfoModel>, List<PagePlanGroupInfoModel>>(source);
                return planList;
            }
        }

        /// <summary>
        /// 取得下属循环计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PagePlanGroupInfoModel> GetSubordinateLoopPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetSubordinateLoopPlanGourpInfo(db, userId, groupType);
                var planList = ModelMapping.JsonMapping<List<PlanGroupInfoModel>, List<PagePlanGroupInfoModel>>(source);
                return planList;
            }
        }

        /// <summary>
        /// 取得下属月有效工时信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PageMonthWorkTimeModel GetSubordinateMonthWorkTimeInfo(int userId, string yearMonth)
        {
            IWorkTimeBLL WorkTime = new WorkTimeBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //得到参数月的天数
                var MonthDay = DateTime.DaysInMonth(int.Parse(yearMonth.Substring(0, 4)), int.Parse(yearMonth.Substring(4, 2)));
                var monthWorkTimeModel = new PageMonthWorkTimeModel
                {
                    monthTotalWorkTime = WorkTime.GetPerWorkTimeByMonth(db, userId, yearMonth),
                    monthAvgWorkTime = WorkTime.GetPerWorkTimeByMonth(db, userId, yearMonth) / MonthDay
                };

                return monthWorkTimeModel;
            }
        }

        /// <summary>
        /// 取得下属月计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetSubordinatePlanPieChartInfo(int userId, string yearMonth)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //获取参数月第一天
                var fromTime = DateUtility.GetMonthFirstDay(DateTime.Parse(yearMonth));
                //获取参数月的最后一天
                var toTime = DateUtility.GetMonthLastDay(DateTime.Parse(yearMonth));
                var source = Plan.GetMyPlanStatusInfo(db, userId, fromTime, toTime);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        /// <summary>
        /// 取得下属月循环计划饼图统计信息
        /// </summary>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetSubordinateLoopPlanPieChartInfo(int userId, string yearMonth)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //获取参数月第一天
                var fromTime = DateUtility.GetMonthFirstDay(DateTime.Parse(yearMonth));
                //获取参数月的最后一天
                var toTime = DateUtility.GetMonthLastDay(DateTime.Parse(yearMonth));
                var source = Plan.GetMyLoopPlanStatusInfo(db, userId, fromTime, toTime);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        #endregion 下属计划

        #region 协作计划

        /// <summary>
        /// 取得协作计划状态数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetCooperationPlanStatusInfo(int userId)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetMyPlanStatusInfo(db, userId, null, null);
                //单个映射--PlanStatusCountModel
                var planInfo = ModelMapping.JsonMapping<PlanStatusCountModel, PagePlanStatusCountModel>(source);
                return planInfo;
            }
        }

        /// <summary>
        /// 取得协作计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PagePlanInfoModel> GetCooperationPlanList(int userId, PagePlanSearchModel searchInfo)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //单个对象映射--PlanSearchModel
                var search = ModelMapping.JsonMapping<PagePlanSearchModel, PlanSearchModel>(searchInfo);
                var source = Plan.GetCooperationPlanList(db, search);
                var planInfoList = ModelMapping.JsonMapping<List<PlanInfoModel>, List<PagePlanInfoModel>>(source);
                return planInfoList;
            }
        }

        /// <summary>
        /// 取得协作循环计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public List<PagePlanInfoModel> GetCooperationLoopPlanList(int userId, PagePlanSearchModel searchInfo)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                //单个对象映射--PlanSearchModel
                var search = ModelMapping.JsonMapping<PagePlanSearchModel, PlanSearchModel>(searchInfo);
                var source = Plan.GetCooperationLoopPlanList(db, search);
                var planInfoList = ModelMapping.JsonMapping<List<LoopPlanInfoModel>, List<PagePlanInfoModel>>(source);
                return planInfoList;
            }
        }

        /// <summary>
        /// 取得协作一般计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PagePlanGroupInfoModel> GetCooperationPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetCooperationPlanGourpInfo(db, userId, groupType);
                var planList = ModelMapping.JsonMapping<List<PlanGroupInfoModel>, List<PagePlanGroupInfoModel>>(source);
                return planList;
            }
        }

        /// <summary>
        /// 取得协作循环计划分组信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<PagePlanGroupInfoModel> GetCooperationLoopPlanGourpInfo(int userId, EnumDefine.PlanGroupType groupType)
        {
            IPlanBLL Plan = new PlanBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var source = Plan.GetCooperationLoopPlanGourpInfo(db, userId, groupType);
                var planList = ModelMapping.JsonMapping<List<PlanGroupInfoModel>, List<PagePlanGroupInfoModel>>(source);
                return planList;
            }
        }

        /// <summary>
        /// 取得月协作计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetCooperationPlanPieChartInfo(int userId, string yearMonth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得月协作循环计划饼图统计信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="yearMonth">年月 格式【“yyyyMM”】</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetCooperationLoopPlanPieChartInfo(int userId, string yearMonth)
        {
            throw new NotImplementedException();
        }

        #endregion 协作计划

        #region 私有方法

        /// <summary>
        /// 取得计划操作信息
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private string GetOperateInfo(PlanLogModel log)
        {
            string result = string.Empty;

            switch (log.type)
            {
                case EnumDefine.PlanOperateStatus.Submit:
                    result = "提交了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.CheckPass:
                    result = "审核通过了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.CheckNoPass:
                    result = "审核不通过该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.CancelSubmit:
                    result = "取消了该计划的提交。";
                    break;

                case EnumDefine.PlanOperateStatus.CancelCheck:
                    result = "取消了该计划的审核结果。";
                    break;

                case EnumDefine.PlanOperateStatus.Discuss:
                    result = "评论了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.DownLoad:
                    result = "下载了该计划附件。";
                    break;

                case EnumDefine.PlanOperateStatus.Read:
                    result = "查看了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.Change:
                    result = "转办了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.Update:
                    result = "申请修改该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.Stop:
                    result = "申请中止该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.ReStart:
                    result = "重新开始了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.Delete:
                    result = "删除了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.ConfirmPass:
                    result = "确认通过了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.ConfirmNoPass:
                    result = "确认不通过该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.UpdateProcess:
                    result = "更新了该计划进度。";
                    break;

                case EnumDefine.PlanOperateStatus.Resolve:
                    result = "分解了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.CreatePlan:
                    result = "新建了该计划。";
                    break;

                case EnumDefine.PlanOperateStatus.UpdateSave:
                    result = "修改了该计划信息。";
                    break;

                case EnumDefine.PlanOperateStatus.SubmitLoopPlan:
                    result = "提交了该循环计划结果。";
                    break;

                case EnumDefine.PlanOperateStatus.SubmitConfirm:
                    result = "提交了该计划结果。";
                    break;

                case EnumDefine.PlanOperateStatus.UpdateCheckPass:
                    result = "申请修改审核通过。";
                    break;

                case EnumDefine.PlanOperateStatus.UpdateCheckNoPass:
                    result = "申请修改审核不通过。";
                    break;

                case EnumDefine.PlanOperateStatus.StopCheckPass:
                    result = "申请中止审核通过。";
                    break;

                case EnumDefine.PlanOperateStatus.StopCheckNoPass:
                    result = "申请中止审核不通过。";
                    break;
            }

            return result;
        }

        #endregion 私有方法
    }
}