using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class TagSearchBLL : ITagSearchBLL
    {
        private const int DEFAULT_TAG_NUM = 5;

        #region 取得搜索结果信息

        /// <summary>
        /// 取得搜索结果信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public object GetSearchResult(int userId, SearchInfoModel searchInfo)
        {
            object resultInfo = null;

            if (searchInfo.type.HasValue)
            {
                switch (searchInfo.type.Value)
                {
                    case (int)ConstVar.TagSearchType.Plan:
                        resultInfo = this.GetPlanSearchResult(userId, searchInfo.keyword);
                        break;

                    case (int)ConstVar.TagSearchType.Objective:
                        resultInfo = this.GetObjectiveSearchResult(userId, searchInfo.keyword);
                        break;

                    //case (int)ConstVar.TagSearchType.Calendar:
                    //    resultInfo = this.GetCalendarSearchResult(userId, searchInfo.keyword);
                    //    break;

                    case (int)ConstVar.TagSearchType.News:
                        resultInfo = this.GetNewsSearchResult(userId, searchInfo.keyword);
                        break;

                    case (int)ConstVar.TagSearchType.Document:
                        resultInfo = this.GetDocumentSearchResult(userId, searchInfo.keyword);
                        break;
                }
            }

            return resultInfo;
        }

        /// <summary>
        /// 保存用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tags"></param>
        public void SaveRecentSearchTag(int userId, string[] tags)
        {
            TagUtility.SaveRecentSearchTag(userId, tags);
        }

        /// <summary>
        /// 取得用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        public string[] GetRecentSearchTag(int userId)
        {
            return TagUtility.GetRecentSearchTag(userId, DEFAULT_TAG_NUM);
        }

        #endregion 取得搜索结果信息

        #region 私用方法

        #region 取得计划搜索结果

        /// <summary>
        /// 取得计划搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        private List<PlanSearchResultModel> GetPlanSearchResult(int userId, string[] tagNames)
        {
            var planIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.Plan);
            var loopIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.LoopPlan);

            var planInfoList = this.FiltratePlanSearchResult(userId, planIds, loopIds);

            return planInfoList;
        }

        #endregion 取得计划搜索结果

        #region 取得目标搜索结果

        /// <summary>
        /// 取得目标搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        private List<ObjectiveIndexModel> GetObjectiveSearchResult(int userId, string[] tagNames)
        {
            var objectiveIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.Objective);

            var objectiveInfoList = this.FiltrateObjectiveSearchResult(userId, objectiveIds);

            return objectiveInfoList;
        }

        #endregion 取得目标搜索结果

        #region 取得日程搜索结果 【已删除】

        //private List<Calendar> GetCalendarSearchResult(int userId, string[] tagNames)
        //{
        //    var calendarIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.Calendar);

        //    var calendarInfoList = this.FiltrateCalendarSearchResult(userId, calendarIds);

        //    return calendarInfoList;
        //}

        #endregion 取得日程搜索结果 【已删除】

        #region 取得新闻搜索结果

        /// <summary>
        /// 取得新闻搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        private List<NewsinfoModel> GetNewsSearchResult(int userId, string[] tagNames)
        {
            var newsIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.News);

            var newsInfoList = this.FiltrateNewsSearchResult(userId, newsIds);

            return newsInfoList;
        }

        #endregion 取得新闻搜索结果

        #region 取得文档搜索结果

        /// <summary>
        /// 取得文档搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        private List<DocumentModel> GetDocumentSearchResult(int userId, string[] tagNames)
        {
            var userDocIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.UserDocument);
            var companyDocIds = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.CompanyDocument);

            var docInfoList = this.FiltrateDocumentSearchResult(userId, userDocIds, companyDocIds);

            return docInfoList;
        }

        #endregion 取得文档搜索结果

        #region 筛选计划搜索结果

        /// <summary>
        /// 筛选计划搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planIds"></param>
        /// <param name="loopIds"></param>
        /// <returns></returns>
        private List<PlanSearchResultModel> FiltratePlanSearchResult(int userId, List<int> planIds, List<int> loopIds)
        {
            var planInfoList = new List<PlanSearchResultModel>();

            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                #region 取得一般计划列表

                var normalPlanList = (from plan in db.tblPlan
                                      join exe in db.tblExecutionMode on plan.executionModeId equals exe.executionId into exeGroup
                                      from exe in exeGroup.DefaultIfEmpty()
                                      //join output in db.tblEventOutput on plan.eventOutputId equals output.outputId into outputGroup
                                      //from output in outputGroup.DefaultIfEmpty()
                                      join create in db.tblUser on plan.createUser equals create.userId into createGroup
                                      from create in createGroup.DefaultIfEmpty()
                                      join responsible in db.tblUser on plan.responsibleUser equals responsible.userId into responsibleGroup
                                      from responsible in responsibleGroup.DefaultIfEmpty()
                                      join confirm in db.tblUser on plan.confirmUser equals confirm.userId into confirmGroup
                                      from confirm in confirmGroup.DefaultIfEmpty()
                                      where !plan.deleteFlag && planIds.Contains(plan.planId) && plan.status.Value != 0 &&
                                      (plan.confirmUser.Value == userId || plan.responsibleUser.Value == userId)// || plan.createUser == userId)
                                      select new PlanSearchResultModel
                                      {
                                          planId = plan.planId,
                                          executionMode = exe.executionMode,
                                          //eventOutput = output.eventOutput,
                                          eventOutput = plan.eventOutput,
                                          endTime = plan.endTime,
                                          confirmUserId = confirm.userId,
                                          confirmUserName = confirm.userName,
                                          responsibleUserId = responsible.userId,
                                          responsibleUserName = responsible.userName,
                                          createTime = plan.createTime,
                                          createUserId = create.userId,
                                          createUserName = create.userName,
                                          difficulty = plan.difficulty,
                                          importance = plan.importance,
                                          urgency = plan.urgency,
                                          generateTime = plan.planGenerateTime.Value,
                                          isFronPlan = plan.withFront.Value ? 1 : 0,
                                          isLoopPlan = 0,
                                          IsCollPlan = 0,
                                          isSubordinatePlan = confirm.userId == userId ? true : false,
                                          stop = plan.stop.Value,
                                          initial = plan.initial,
                                          status = plan.status.Value,
                                          progress = plan.progress
                                      }).ToList();

                planInfoList.AddRange(normalPlanList);

                #endregion 取得一般计划列表

                #region 取得协作计划列表

                var cooperationPlanList = (from plan in db.tblPlan
                                           join co in db.tblPlanCooperation on plan.planId equals co.planId
                                           join exe in db.tblExecutionMode on plan.executionModeId equals exe.executionId into exeGroup
                                           from exe in exeGroup.DefaultIfEmpty()
                                           join create in db.tblUser on plan.createUser equals create.userId into createGroup
                                           from create in createGroup.DefaultIfEmpty()
                                           join responsible in db.tblUser on plan.responsibleUser equals responsible.userId into responsibleGroup
                                           from responsible in responsibleGroup.DefaultIfEmpty()
                                           join confirm in db.tblUser on plan.confirmUser equals confirm.userId into confirmGroup
                                           from confirm in confirmGroup.DefaultIfEmpty()
                                           where !plan.deleteFlag && plan.status.Value != 0 && co.userId == userId && planIds.Contains(plan.planId)
                                           select new PlanSearchResultModel
                                           {
                                               planId = plan.planId,
                                               executionMode = exe.executionMode,
                                               endTime = plan.endTime,
                                               confirmUserId = confirm.userId,
                                               confirmUserName = confirm.userName,
                                               responsibleUserId = responsible.userId,
                                               responsibleUserName = responsible.userName,
                                               createTime = plan.createTime,
                                               createUserId = create.userId,
                                               createUserName = create.userName,
                                               difficulty = plan.difficulty.Value,
                                               importance = plan.importance.Value,
                                               urgency = plan.urgency.Value,
                                               generateTime = plan.planGenerateTime.Value,
                                               isFronPlan = plan.withFront.Value ? 1 : 0,
                                               isLoopPlan = 0,
                                               IsCollPlan = 1,
                                               isSubordinatePlan = false,
                                               stop = plan.stop.Value,
                                               initial = plan.initial,
                                               status = plan.status.Value,
                                               progress = plan.progress
                                           }).ToList();

                planInfoList.AddRange(cooperationPlanList);

                #endregion 取得协作计划列表

                #region 取得循环计划列表

              
                /*
                var loopPlanList = (from c in db.tblLoopPlan
                                    join b in db.tblProject on c.projectId equals b.projectId into group1
                                    from b in group1.DefaultIfEmpty()
                                    join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                                    from d in group2.DefaultIfEmpty()
                                    join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                                    from e in group3.DefaultIfEmpty()
                                    join f in db.tblUser on c.responsibleUser equals f.userId into group4
                                    from f in group4.DefaultIfEmpty()
                                    join s in db.tblLoopplanSubmit on c.loopId equals s.loopId into group5
                                    from s in group5.DefaultIfEmpty()
                                    where !c.deleteFlag && c.loopStatus.Value && s.confirmTime == null
                                    select new PlanInfo
                                    {
                                        loopId = c.loopId,
                                        executionModeId = c.executionModeId,
                                        responsibleOrganization = c.responsibleOrganization,
                                        responsibleUser = c.responsibleUser,
                                        responsibleUserName = f.userName,
                                        confirmOrganization = c.confirmOrganization,
                                        confirmUser = c.confirmUser,
                                        confirmUserName = db.tblUser.Where(p => p.userId == c.confirmUser).Select(p => p.userName).FirstOrDefault() ?? "无",
                                        eventOutputId = c.eventOutputId,
                                        eventOutput = c.eventOutput,
                                        startTime = c.startTime,
                                        endTime = c.endTime.Value,
                                        comment = c.comment,
                                        importance = c.importance,
                                        urgency = c.urgency,
                                        difficulty = c.difficulty,
                                        isLoopPlan = 1,
                                        IsCollPlan = 0,
                                        isFronPlan = 0,
                                        status = c.status,
                                        withFront = false,
                                        stop = 0,
                                        loopStatus = c.loopStatus,
                                        createTime = c.createTime,
                                        updateTime = s.createTime,
                                        loopYear = c.loopYear,
                                        loopMonth = c.loopMonth,
                                        loopWeek = c.loopWeek,
                                        loopTime = c.loopTime,
                                        submitId = s.submitId,

                                        projectId = c.projectId,
                                        parentProject = b.parentProject,
                                        projectName = b.projectName,

                                        organizationId = c.organizationId,
                                        parentOrganization = d.parentOrganization,
                                        schemaName = d.schemaName,
                                        organizationName = d.organizationName,

                                        executionId = e.executionId,
                                        executionMode = e.executionMode
                                    }).ToList();
                */

                #endregion 取得循环计划列表
            }

            return planInfoList.OrderByDescending(p => p.generateTime).ToList();
        }

        #endregion 筛选计划搜索结果

        #region 筛选目标搜索结果

        /// <summary>
        /// 筛选目标搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveIds"></param>
        /// <returns></returns>
        private List<ObjectiveIndexModel> FiltrateObjectiveSearchResult(int userId, List<int> objectiveIds)
        {
            var objectiveInfoList = new List<ObjectiveIndexModel>();

            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                // 筛选目标搜索结果
                objectiveInfoList = (from o in db.tblObjective
                                     join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                     from resUser in group1.DefaultIfEmpty()
                                     join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                     from conUser in group2.DefaultIfEmpty()
                                     join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                                     from authorizedUser in group3.DefaultIfEmpty()
                                     join create in db.tblUser on o.createUser equals create.userId into group4
                                     from create in group4.DefaultIfEmpty()
                                     where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag && (o.confirmUser == userId || o.responsibleUser == userId) && objectiveIds.Contains(o.objectiveId) && o.status > 1
                                     orderby o.startTime descending
                                     select new ObjectiveIndexModel
                                     {
                                         objectiveId = o.objectiveId,
                                         parentObjective = o.parentObjective,
                                         objectiveName = o.objectiveName,
                                         objectiveType = o.objectiveType,
                                         bonus = o.bonus,
                                         weight = o.weight,
                                         objectiveValue = o.objectiveValue,
                                         expectedValue = o.expectedValue,
                                         startTime = o.startTime.Value,
                                         endTime = o.endTime.Value,
                                         actualEndTime = o.actualEndTime,
                                         alarmTime = o.alarmTime,
                                         responsibleUser = o.responsibleUser,
                                         responsibleUserName = resUser.userName,
                                         confirmUser = o.confirmUser,
                                         confirmUserName = conUser.userName,
                                         authorizedUser = authorizedUser.userId,
                                         authorizedUserName = authorizedUser.userName,
                                         responsibleOrg = o.responsibleOrg,
                                         checkType = o.checkType,
                                         status = o.status,
                                         progress = o.progress,
                                         updateTime = o.updateTime,
                                         createUser = create.userName,
                                         createTime = o.createTime,
                                         bigImage = string.IsNullOrEmpty(create.bigImage) ? FilePath.DefaultHeadPortrait : (FilePath.HeadImageUpLoadPath + create.bigImage),
                                         isSubordinateObjective = o.confirmUser == userId ? true : false
                                     }).ToList();
            }

            return objectiveInfoList;
        }

        #endregion 筛选目标搜索结果

        #region 筛选日程搜索结果【已删除】

        //private List<Calendar> FiltrateCalendarSearchResult(int userId, List<int> calendarIds)
        //{
        //    var calendarInfoList = new List<Calendar>();

        //    using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
        //    {
        //        calendarInfoList = (from c in db.tblCalendar
        //                            join uc in db.tblCalendarUser on c.calendarId equals uc.calendarId
        //                            where c.createUser == userId || uc.userId == userId
        //                            orderby c.createTime descending
        //                            select new Calendar
        //                            {
        //                                calendarId = c.calendarId,
        //                                comment = c.comment,
        //                                startTime = c.startTime,
        //                                endTime = c.endTime,
        //                                place = c.place
        //                                // 筛选日程搜索结果
        //                            }).ToList();
        //    }

        //    return calendarInfoList;
        //}

        #endregion 筛选日程搜索结果【已删除】

        #region 筛选新闻搜索结果

        /// <summary>
        /// 筛选新闻搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsIds"></param>
        /// <returns></returns>
        private List<NewsinfoModel> FiltrateNewsSearchResult(int userId, List<int> newsIds)
        {
            var newsInfoList = new List<NewsinfoModel>();

            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                newsInfoList = (from news in db.tblNews
                                join directory in db.tblNewsDirectory on news.directoryId equals directory.directoryId
                                where news.publish == true && newsIds.Contains(news.newId)
                                orderby news.updateTime descending
                                select new NewsinfoModel
                                {
                                    newId = news.newId,
                                    title = news.title,
                                    directoryName = directory.directoryName,
                                    UserName = db.tblUser.Where(x => x.userId == news.createUser).Select(x => x.userName).FirstOrDefault(),
                                    createTime = news.createTime,
                                    updateTime = news.updateTime
                                }).ToList();
            }

            return newsInfoList;
        }

        #endregion 筛选新闻搜索结果

        #region 筛选文档搜索结果

        /// <summary>
        /// 筛选文档搜索结果
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userDocIds"></param>
        /// <param name="companyDocIds"></param>
        /// <returns></returns>
        private List<DocumentModel> FiltrateDocumentSearchResult(int userId, List<int> userDocIds, List<int> companyDocIds)
        {
            var docInfoList = new List<DocumentModel>();

            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                #region 公司文档

                if (companyDocIds.Count > 0)
                {
                    //取得用户信息
                    var stationIds = db.tblUserStation.Where(p => p.userId == userId).Select(p => p.stationId).ToArray();
                    var orgIds = db.tblStation.Where(p => stationIds.Contains(p.stationId) && !p.deleteFlag).Select(p => p.organizationId).ToArray();
                    var docList = db.tblCompanyDocument.Where(p => !p.deleteFlag && p.isFolder == false && companyDocIds.Contains(p.documentId)).ToList();

                    var folderList = new List<int>();
                    foreach (var doc in docList)
                    {
                        //取得当前文档的所有上级文件夹信息
                        folderList.Clear();

                        var parentFolderInfo = db.tblCompanyDocument.Where(p => !p.deleteFlag && p.isFolder == true && p.documentId == doc.folder.Value).FirstOrDefault();

                        while (parentFolderInfo != null)
                        {
                            folderList.Add(parentFolderInfo.documentId);
                            parentFolderInfo = db.tblCompanyDocument.Where(p => !p.deleteFlag && p.isFolder == true && p.documentId == parentFolderInfo.folder).FirstOrDefault();
                        }

                        //判断当前文档的所属文件夹的权限（从第一级父文件夹到当前文档所在文件夹）
                        if (folderList.Count > 0)
                        {
                            var upPower = (int)ConstVar.FolderAuthPower.FullControl;
                            var sortedFolderList = folderList.OrderByDescending(p => p).ToList();
                            foreach (var id in sortedFolderList)
                            {
                                var authPower = (from fa in db.tblFolderAuth
                                                 join ar in db.tblAuthResult on fa.authId equals ar.authId
                                                 where ((fa.type == (int)ConstVar.FolderAuthType.organization && orgIds.Contains(ar.targetId)) ||
                                                           (fa.type == (int)ConstVar.FolderAuthType.station && stationIds.Contains(ar.targetId.Value)) ||
                                                           (fa.type == (int)ConstVar.FolderAuthType.user && userId == ar.targetId)) &&
                                                           fa.documentId == id
                                                 select fa.power
                                                         ).FirstOrDefault();

                                upPower = upPower > authPower ? authPower : upPower;

                                if (upPower <= (int)ConstVar.FolderAuthPower.NoAccese) break;
                            }

                            if (upPower > (int)ConstVar.FolderAuthPower.NoAccese)
                            {
                                var docInfo = new DocumentModel
                                {
                                    documentId = doc.documentId,
                                    createTime = doc.createTime,
                                    createUser = doc.createUser,
                                    createUserName = db.tblUser.Where(p => p.userId == doc.createUser).Select(p => p.userName).FirstOrDefault(),
                                    updateTime = doc.updateTime,
                                    displayName = doc.displayName,
                                    saveName = doc.saveName,
                                    extension = doc.extension,
                                    isFolder = doc.isFolder,
                                    isCompany = true,
                                    power = upPower
                                };
                                docInfoList.Add(docInfo);
                            }
                        }
                    }
                }

                #endregion 公司文档

                #region 个人文档

                if (userDocIds.Count > 0)
                {
                    //用户个人文档
                    var userDocList = (from ud in db.tblUserDocument
                                       where !ud.deleteFlag && ud.isFolder == false && userDocIds.Contains(ud.documentId) && ud.createUser == userId
                                       select new DocumentModel
                                       {
                                           documentId = ud.documentId,
                                           displayName = ud.displayName,
                                           saveName = ud.saveName,
                                           extension = ud.extension,
                                           isCompany = false,
                                           createUser = ud.createUser,
                                           createUserName = db.tblUser.Where(p => p.userId == ud.createUser).Select(p => p.userName).FirstOrDefault(),
                                           createTime = ud.createTime,
                                           updateTime = ud.updateTime,
                                           withShared = ud.withShared,
                                           power = (int)ConstVar.FolderAuthPower.FullControl
                                       }).ToList();

                    docInfoList.AddRange(userDocList);

                    //他人共享文档
                    var otherSharedDocList = (from ud in db.tblUserDocument
                                              join ds in db.tblDocumentShared on ud.documentId equals ds.documentId
                                              where !ud.deleteFlag && ud.isFolder == false && userDocIds.Contains(ds.documentId) && ds.userId == userId
                                              select new DocumentModel
                                              {
                                                  documentId = ud.documentId,
                                                  displayName = ud.displayName,
                                                  saveName = ud.saveName,
                                                  extension = ud.extension,
                                                  isCompany = false,
                                                  createUser = ud.createUser,
                                                  createUserName = db.tblUser.Where(p => p.userId == ud.createUser).Select(p => p.userName).FirstOrDefault(),
                                                  createTime = ud.createTime,
                                                  updateTime = ud.updateTime,
                                                  withShared = ud.withShared,
                                                  power = (int)ConstVar.FolderAuthPower.DownloadOnly
                                              }).ToList();

                    docInfoList.AddRange(otherSharedDocList);
                }

                #endregion 个人文档
            }

            return docInfoList.OrderByDescending(p => p.createTime).ToList();
        }

        #endregion 筛选文档搜索结果

        #endregion 私用方法
    }
}