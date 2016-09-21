using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class ObjectiveIndexBLL : IObjectiveIndexBLL
    {
        #region 变量区域

        private SharedBLL shareBll = new SharedBLL();

        #endregion 变量区域

        #region 获取目标首页列表

        /// <summary>
        /// 获取目标首页列表
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="userId">登录用户Id</param>
        /// <returns>首页列表</returns>
        public ObjectiveInfo GetObjectiveList(string condition, DateTime start, DateTime end, int userId)
        {
            var objectiveInfo = new ObjectiveInfo();
            objectiveInfo.userId = userId;
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveTempList = (from o in db.tblObjective
                                         join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                         from resUser in group1.DefaultIfEmpty()
                                         join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                         from conUser in group2.DefaultIfEmpty()
                                         join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                                         from authorizedUser in group3.DefaultIfEmpty()
                                         join create in db.tblUser on o.createUser equals create.userId into group4
                                         from create in group4.DefaultIfEmpty()
                                         orderby o.startTime descending
                                         where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag
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
                                             bigImage = (create.bigImage == null || create.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + create.bigImage)
                                         }).Where(condition).Where("(endTime >=@0 And endTime <@1 ) Or (startTime >=@0 And startTime<@1 )", start, end).ToList();

                if (objectiveTempList.Count > 0)
                {
                    objectiveInfo.ObjectiveList = new List<ObjectiveIndexModel>();
                    //剔除掉子目标
                    objectiveTempList.ForEach(p =>
                    {
                        if (p.parentObjective == null || objectiveTempList.Where(a => a.objectiveId == p.parentObjective).Count() <= 0)
                        {
                            var childList = db.tblObjective.Where(a => a.parentObjective == p.objectiveId && !a.deleteFlag);
                            p.isHasChild = childList.Count() > 0 ? true : false;
                            p.objectiveTypeName = p.objectiveType == 2 ? shareBll.GetUsernByUserId(db, p.responsibleUser.Value, p.responsibleOrg.Value) : shareBll.GetOrgStringByOrgIdNew(db, p.responsibleOrg.Value, new List<string>());
                            objectiveInfo.ObjectiveList.Add(p);
                        }
                    });
                }
            }
            return objectiveInfo;
        }

        #endregion 获取目标首页列表

        #region 获取目标首页超时列表

        /// <summary>
        /// 获取目标首页超时列表
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="userId">登录用户Id</param>
        /// <returns>首页列表</returns>
        public ObjectiveInfo GetObjectiveOverTimeList(string condition, DateTime end, int userId)
        {
            var objectiveInfo = new ObjectiveInfo();
            objectiveInfo.userId = userId;
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveTempList = (from o in db.tblObjective
                                         join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                         from resUser in group1.DefaultIfEmpty()
                                         join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                         from conUser in group2.DefaultIfEmpty()
                                         join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                                         from authorizedUser in group3.DefaultIfEmpty()
                                         join create in db.tblUser on o.createUser equals create.userId into group4
                                         from create in group4.DefaultIfEmpty()
                                         orderby o.startTime descending
                                         where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag
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
                                             bigImage = (create.bigImage == null || create.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + create.bigImage)
                                         }).Where(condition).Where("endTime <@0", end).ToList();

                if (objectiveTempList.Count > 0)
                {
                    objectiveInfo.ObjectiveList = new List<ObjectiveIndexModel>();
                    //剔除掉子目标
                    objectiveTempList.ForEach(p =>
                    {
                        if (p.parentObjective == null || objectiveTempList.Where(a => a.objectiveId == p.parentObjective).Count() <= 0)
                        {
                            var childList = db.tblObjective.Where(a => a.parentObjective == p.objectiveId && !a.deleteFlag);
                            p.isHasChild = childList.Count() > 0 ? true : false;
                            p.objectiveTypeName = p.objectiveType == 2 ? shareBll.GetUsernByUserId(db, p.responsibleUser.Value, p.responsibleOrg.Value) : shareBll.GetOrgStringByOrgIdNew(db, p.responsibleOrg.Value, new List<string>());
                            objectiveInfo.ObjectiveList.Add(p);
                        }
                    });
                }
            }
            return objectiveInfo;
        }

        #endregion 获取目标首页超时列表

        #region 获取计划日程话计划目标列表

        /// <summary>
        /// 获取计划日程话计划目标列表
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="date">计划日程化页面选择的日期</param>
        /// <param name="userId">登录用户Id</param>
        /// <returns>首页列表</returns>
        public ObjectiveInfo GetCalendarObjectiveList(string condition, DateTime date, int userId)
        {
            var objectiveInfo = new ObjectiveInfo();
            objectiveInfo.userId = userId;
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveTempList = (from o in db.tblObjective
                                         join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                         from resUser in group1.DefaultIfEmpty()
                                         join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                         from conUser in group2.DefaultIfEmpty()
                                         join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                                         from authorizedUser in group3.DefaultIfEmpty()
                                         join create in db.tblUser on o.createUser equals create.userId into group4
                                         from create in group4.DefaultIfEmpty()
                                         orderby o.startTime descending
                                         where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag
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
                                             bigImage = (create.bigImage == null || create.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + create.bigImage)
                                         }).Where(condition).Where("updateTime >=@0 And updateTime <=@1", date, date.AddDays(1)).ToList();

                if (objectiveTempList.Count > 0)
                {
                    objectiveInfo.ObjectiveList = new List<ObjectiveIndexModel>();
                    objectiveTempList.ForEach(p =>
                    {
                        var childList = db.tblObjective.Where(a => a.parentObjective == p.objectiveId && !a.deleteFlag);
                        p.isHasChild = childList.Count() > 0 ? true : false;
                        p.objectiveTypeName = p.objectiveType == 2 ? shareBll.GetUsernByUserId(db, p.responsibleUser.Value, p.responsibleOrg.Value) : shareBll.GetOrgStringByOrgIdNew(db, p.responsibleOrg.Value, new List<string>());
                        objectiveInfo.ObjectiveList.Add(p);
                    });
                }
            }
            return objectiveInfo;
        }

        #endregion 获取计划日程话计划目标列表

        #region 根据当前目标Id获取子目标列表

        /// <summary>
        /// 根据当前目标Id获取子目标列表
        /// </summary>
        /// <param name="objectiveId">父目标Id</param>
        /// <returns>子目标列表</returns>
        public ObjectiveInfo GetChildrenObjectiveList(int objectiveId, int userId)
        {
            var objectiveInfo = new ObjectiveInfo();
            objectiveInfo.userId = userId;
            using (var db = new TargetNavigationDBEntities())
            {
                objectiveInfo.ObjectiveList = (from o in db.tblObjective
                                               join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                               from resUser in group1.DefaultIfEmpty()
                                               join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                               from conUser in group2.DefaultIfEmpty()
                                               join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                                               from authorizedUser in group3.DefaultIfEmpty()
                                               join create in db.tblUser on o.createUser equals create.userId into group4
                                               from create in group4.DefaultIfEmpty()
                                               orderby o.startTime descending
                                               where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag && o.parentObjective == objectiveId
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
                                                   createUser = create.userName,
                                                   createTime = o.createTime
                                               }).ToList();
                if (objectiveInfo.ObjectiveList.Count > 0)
                {
                    objectiveInfo.ObjectiveList.ForEach(p =>
                    {
                        var childList = db.tblObjective.Where(a => a.parentObjective == p.objectiveId && !a.deleteFlag);
                        p.isHasChild = childList.Count() > 0 ? true : false;
                        p.objectiveTypeName = p.objectiveType == 1 ? shareBll.GetUsernByUserId(db, p.responsibleUser.Value, p.responsibleOrg.Value) : shareBll.GetOrgStringByOrgIdNew(db, p.responsibleOrg.Value, new List<string>());
                    });
                }
            }
            return objectiveInfo;
        }

        #endregion 根据当前目标Id获取子目标列表

        #region 目标展开数据

        /// <summary>
        /// 目标展开数据
        /// </summary>
        /// <param name="objectiveId">当前目标Id</param>
        /// <returns>目标展开数据</returns>
        public ObjectiveHasChildModel ExpandObjective(int objectiveId)
        {
            var objectiveModel = new ObjectiveHasChildModel();
            using (var db = new TargetNavigationDBEntities())
            {
                objectiveModel = (from o in db.tblObjective
                                  join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                  from resUser in group1.DefaultIfEmpty()
                                  join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                  from conUser in group2.DefaultIfEmpty()
                                  join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                                  from authorizedUser in group3.DefaultIfEmpty()
                                  orderby o.startTime descending
                                  where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag && o.objectiveId == objectiveId
                                  select new ObjectiveHasChildModel
                                  {
                                      objectiveId = o.objectiveId,
                                      parentObjective = o.parentObjective,
                                      objectiveName = o.objectiveName,
                                      objectiveType = o.objectiveType,
                                      bonus = o.bonus,
                                      weight = o.weight,
                                      objectiveValue = o.objectiveValue,
                                      expectedValue = o.expectedValue,
                                      startTime = o.startTime,
                                      endTime = o.endTime,
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
                                      progress = o.progress
                                  }).FirstOrDefault();
                //取子目标集合
                if (objectiveModel != null)
                {
                    objectiveModel.objectiveTypeName = objectiveModel.objectiveType == 2 ? shareBll.GetUsernByUserId(db, objectiveModel.responsibleUser.Value, objectiveModel.responsibleOrg.Value) : shareBll.GetOrgStringByOrgIdNew(db, objectiveModel.responsibleOrg.Value, new List<string>());
                    objectiveModel.childObjectiveList = this.GetNextObjectiveList(db, objectiveId, new List<ObjectiveHasChildModel>());
                }
            }
            return objectiveModel;
        }

        #endregion 目标展开数据

        #region 获取甘特图列表

        /// <summary>
        /// 获取甘特图列表
        /// </summary>
        /// <param name="thisStart">筛选开始时间</param>
        /// <param name="thisEnd">筛选结束时间</param>
        /// <param name="userId">登录用户Id</param>
        /// <returns>甘特图列表</returns>
        public List<GanttChartModel> GetGanttChartObjectiveList(DateTime thisStart, DateTime thisEnd, int userId)
        {
            var objectiveList = new List<GanttChartModel>();
            var time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveTempList = (from o in db.tblObjective
                                         join fObjective in db.tblObjective on o.parentObjective equals fObjective.objectiveId into group3
                                         from fObjective in group3.DefaultIfEmpty()
                                         join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                         from resUser in group1.DefaultIfEmpty()
                                         join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                         from conUser in group2.DefaultIfEmpty()
                                         orderby o.startTime descending
                                         where !o.deleteFlag && !resUser.deleteFlag && o.status != (int)ConstVar.ObjectIndexStatus.unSubmit && !conUser.deleteFlag && (o.responsibleUser == userId || o.authorizedUser == userId || o.confirmUser == userId)
                                         && ((o.startTime >= thisStart && o.startTime < thisEnd) || (o.endTime >= thisStart && o.endTime < thisEnd) || (o.startTime < thisStart && o.endTime >= thisEnd))
                                         select new GanttChartModel
                                         {
                                             id = o.objectiveId,
                                             pid = o.parentObjective,
                                             name = o.objectiveName,
                                             tempStart = o.startTime,
                                             tempEnd = o.endTime,
                                             status = o.status,
                                             tempParentStart = fObjective == null ? null : fObjective.startTime,
                                             tempParentEnd = fObjective == null ? null : fObjective.endTime,
                                             responsibleUser = o.responsibleUser,
                                             responsibleUserName = resUser.userName,
                                             confirmUser = o.confirmUser,
                                             confirmUserName = conUser.userName,
                                             process = o.progress,
                                             objectiveStatus = o.status > 3 ? false : true
                                         }).ToList();
                if (objectiveTempList.Count > 0)
                {
                    if (objectiveTempList.Count > 0)
                    {
                        //剔除掉子目标
                        objectiveTempList.ForEach(p =>
                        {
                            if (p.pid == null || objectiveTempList.Where(a => a.id == p.pid).Count() <= 0)
                            {
                                objectiveList.Add(p);
                            }
                        });
                    }
                    if (objectiveList.Count > 0)
                    {
                        objectiveList.ForEach(p =>
                        {
                            p.start = (int)(p.tempStart.Value - time).TotalSeconds + "000";
                            p.end = (int)(p.tempEnd.Value - time).TotalSeconds + "000";
                            if (p.tempParentStart == null)
                            {
                                p.parentStart = string.Empty;
                            }
                            else
                            {
                                p.parentStart = (int)(p.tempParentStart.Value - time).TotalSeconds + "000";
                            }
                            if (p.tempParentEnd == null)
                            {
                                p.parentEnd = string.Empty;
                            }
                            else
                            {
                                p.parentEnd = (int)(p.tempParentEnd.Value - time).TotalSeconds + "000";
                            }
                            var childObjectiveList = db.tblObjective.Where(a => a.parentObjective == p.id).ToList();
                            if (childObjectiveList.Count <= 0)
                            {
                                p.childStart = string.Empty;
                                p.childEnd = string.Empty;
                            }
                            else
                            {
                                p.haschild = true;
                                p.childStart = (int)(childObjectiveList.Min(x => x.startTime).Value - time).TotalSeconds + "000";
                                p.childEnd = (int)(childObjectiveList.Max(x => x.endTime).Value - time).TotalSeconds + "000";
                            }
                        });
                    }
                }
            }
            return objectiveList;
        }

        #endregion 获取甘特图列表

        #region 获取甘特图子列表

        /// <summary>
        /// 获取甘特图子列表
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>甘特图子列表</returns>
        public List<GanttChartModel> GetGanttChartChildObjectiveList(int objectiveId)
        {
            var objectiveList = new List<GanttChartModel>();
            var time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            using (var db = new TargetNavigationDBEntities())
            {
                objectiveList = (from o in db.tblObjective
                                 join fObjective in db.tblObjective on o.parentObjective equals fObjective.objectiveId into group3
                                 from fObjective in group3.DefaultIfEmpty()
                                 join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                 from resUser in group1.DefaultIfEmpty()
                                 join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                 from conUser in group2.DefaultIfEmpty()
                                 orderby o.startTime descending
                                 where !o.deleteFlag && !resUser.deleteFlag && o.status != (int)ConstVar.ObjectIndexStatus.unSubmit && !conUser.deleteFlag && o.parentObjective == objectiveId
                                 select new GanttChartModel
                                 {
                                     id = o.objectiveId,
                                     pid = o.parentObjective,
                                     name = o.objectiveName,
                                     status = o.status,
                                     tempStart = o.startTime,
                                     tempEnd = o.endTime,
                                     tempParentStart = fObjective == null ? null : fObjective.startTime,
                                     tempParentEnd = fObjective == null ? null : fObjective.endTime,
                                     responsibleUser = o.responsibleUser,
                                     responsibleUserName = resUser.userName,
                                     confirmUser = o.confirmUser,
                                     confirmUserName = conUser.userName,
                                     objectiveStatus = o.status > 3 ? false : true
                                 }).ToList();

                if (objectiveList.Count > 0)
                {
                    objectiveList.ForEach(p =>
                    {
                        p.start = (int)(p.tempStart.Value - time).TotalSeconds + "000";
                        p.end = (int)(p.tempEnd.Value - time).TotalSeconds + "000";
                        p.parentStart = (int)(p.tempParentStart.Value - time).TotalSeconds + "000";
                        p.parentEnd = (int)(p.tempParentEnd.Value - time).TotalSeconds + "000";
                        var childObjectiveList = db.tblObjective.Where(a => a.parentObjective == p.id && !a.deleteFlag).ToList();
                        if (childObjectiveList.Count <= 0)
                        {
                            p.childStart = string.Empty;
                            p.childEnd = string.Empty;
                        }
                        else
                        {
                            p.haschild = true;
                            p.childStart = (int)(childObjectiveList.Min(x => x.startTime).Value - time).TotalSeconds + "000";
                            p.childEnd = (int)(childObjectiveList.Max(x => x.endTime).Value - time).TotalSeconds + "000";
                        }
                    });
                }
            }
            return objectiveList;
        }

        #endregion 获取甘特图子列表

        #region 删除目标

        /// <summary>
        /// 删除目标
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="userId">操作人Id</param>
        /// <returns>true:操作成功 false:操作失败</returns>
        public bool DeleteObjective(int objectiveId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var objective = GetObjective(db, objectiveId);
                if (objective == null) return false;
                //存储过程级联删除目标
                db.prcObjectiveDelete(objectiveId, userId, DateTime.Now);

                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.delete, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 删除目标

        #region 目标授权

        /// <summary>
        /// 目标授权
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="authorizedUser">被授权用户</param>
        /// <param name="userId">操作人Id</param>
        /// <returns>true:操作成功 false:操作失败</returns>
        public bool AuthorizeObjective(int objectiveId, int authorizedUser, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var objective = GetObjective(db, objectiveId);
                if (objective == null) return false;
                //更新目标信息表
                objective.authorizedUser = authorizedUser;
                objective.updateTime = DateTime.Now;

                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.authorize, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 目标授权

        #region 取消授权

        /// <summary>
        /// 取消授权
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="userId">操作人Id</param>
        /// <returns>true:操作成功 false:操作失败</returns>
        public bool CancelAuthorizeObjective(int objectiveId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveModel = GetObjective(db, objectiveId);
                if (objectiveModel == null) return false;
                objectiveModel.authorizedUser = null;
                objectiveModel.updateUser = userId;
                objectiveModel.updateTime = DateTime.Now;

                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.cancelAuthorize, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 取消授权

        #region 撤销操作

        /// <summary>
        /// 撤销操作
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns>true:操作成功 false:操作失败</returns>
        public bool RevokeObjective(int objectiveId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //获取这条目标数据
                var objective = GetObjective(db, objectiveId);
                if (objective == null) return false;
                //根据目标之前的状态，完成撤销操作
                var status = objective.status;
                if (status == (int)ConstVar.ObjectIndexStatus.unChecked)
                {
                    objective.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                }
                else if (status == (int)ConstVar.ObjectIndexStatus.unConfirm)
                {
                    objective.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                }
                objective.updateUser = userId;
                objective.updateTime = DateTime.Now;
                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.revoke, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 撤销操作

        #region 新建目标提交保存操作

        /// <summary>
        /// 新建目标提交保存操作
        /// </summary>
        /// <param name="objectiveModel">目标实体对象</param>
        /// <param name="userId">操作人Id</param>
        /// <param name="flag">1:提交目标 2：保存目标</param>
        /// <param name="operateFlag">1、授权人提交 2、责任人提交</param>
        /// <param name="objectiveId">目标ID</param>
        public ReturnConfirm NewObjective(AddNewObjectiveModel objectiveModel, int userId, int flag, out int objectiveId)
        {
            objectiveId = 0;
            var confirm = new ReturnConfirm();
            confirm.result = (int)ConstVar.ReturnField.success;
            using (var db = new TargetNavigationDBEntities())
            {
                var result = 0;
                if (!int.TryParse(objectiveModel.responsibleOrg, out result))
                {
                    var orgInfo = db.tblOrganization.Where(p => p.organizationName == objectiveModel.responsibleOrg).FirstOrDefault();
                    if (orgInfo != null) result = orgInfo.organizationId;
                }
                //目标信息表中插入数据
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var id = db.prcGetPrimaryKey("tblObjective", obj).FirstOrDefault().Value;

                //Out参数objectiveId赋值
                objectiveId = id;

                var objective = new tblObjective
                {
                    objectiveId = id,
                    displayChangeFlag = false,
                    objectiveName = objectiveModel.objectiveName,
                    weight = objectiveModel.weight,
                    objectiveValue = objectiveModel.objectiveValue,
                    expectedValue = objectiveModel.expectedValue,
                    objectiveType = objectiveModel.objectiveType,
                    responsibleOrg = result,
                    responsibleUser = objectiveModel.responsibleUser,
                    confirmUser = objectiveModel.confirmUser,
                    checkType = objectiveModel.checkType,
                    bonus = objectiveModel.bonus,
                    maxValue = objectiveModel.maxValue,
                    minValue = objectiveModel.minValue,
                    startTime = objectiveModel.startTime,
                    endTime = objectiveModel.endTime,
                    alarmTime = objectiveModel.alarmTime,
                    formula = objectiveModel.formula,
                    description = objectiveModel.description,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    keyword = objectiveModel.keyword != null && objectiveModel.keyword.Length > 0 ? string.Join(",", objectiveModel.keyword) : null
                };

                //目标公式表中插入数据
                if (objectiveModel.formula == (int)ConstVar.ObjectiveFormulaType.userDefined && objectiveModel.objectiveFormula.Count > 0)
                {
                    confirm.result = this.CheckFormulaValidity(0, objectiveModel.objectiveFormula);
                    if (confirm.result != (int)ConstVar.ReturnField.success) return confirm;

                    foreach (var item in objectiveModel.objectiveFormula)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter formulaObj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        var formulaId = db.prcGetPrimaryKey("tblObjectiveFormula", formulaObj).FirstOrDefault().Value;
                        var formulaModel = new tblObjectiveFormula
                        {
                            formulaId = formulaId,
                            formulaNum = item.formulaNum,
                            objectiveId = id,
                            orderNum = item.orderNum,
                            field = item.field,
                            operate = item.operate,
                            numValue = item.numValue,
                            valueType = item.valueType
                        };
                        db.tblObjectiveFormula.Add(formulaModel);
                    };
                }

                if (flag == 1)  //提交
                {
                    confirm.confirmUser = objectiveModel.confirmUser;
                    if (objectiveModel.confirmUser != null && objectiveModel.confirmUser == userId)
                    {
                        objective.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                        //日志：创建
                        AddObjectiveOperate(db, id, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                        //日志：提交
                        AddObjectiveOperate(db, id, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                        //添加审核通过日志
                        AddObjectiveOperate(db, id, string.Empty, (int)ConstVar.ObjectiveOperaResult.checkPass, userId);
                    }
                    else
                    {
                        objective.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                        //日志：创建
                        AddObjectiveOperate(db, id, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                        //添加提交日志
                        AddObjectiveOperate(db, id, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                    }
                }
                else   //保存
                {
                    objective.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                    //添加创建日志
                    AddObjectiveOperate(db, id, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                }
                db.tblObjective.Add(objective);
                db.SaveChanges();
            }
            return confirm;
        }

        #endregion 新建目标提交保存操作

        #region 待提交目标提交保存操作

        /// <summary>
        /// 待提交目标提交保存操作
        /// </summary>
        /// <param name="objectiveModel">目标实体对象</param>
        /// <param name="userId">操作人Id</param>
        /// <param name="flag">1:提交目标 2：保存目标</param>
        /// <param name="operateFlag">1、授权人提交 2、责任人提交</param>
        public ReturnConfirm UpdateObjective(AddNewObjectiveModel objectiveModel, int userId, int flag, int operateFlag)
        {
            var returnConfirm = new ReturnConfirm();
            returnConfirm.result = (int)ConstVar.ReturnField.success;

            using (var db = new TargetNavigationDBEntities())
            {
                var result = 0;
                if (!int.TryParse(objectiveModel.responsibleOrg, out result))
                {
                    var orgInfo = db.tblOrganization.Where(p => p.organizationName == objectiveModel.responsibleOrg).FirstOrDefault();
                    if (orgInfo != null) result = orgInfo.organizationId;
                }
                //目标信息表更新数据数据
                var objectiveInfo = this.GetObjective(db, objectiveModel.objectiveId.Value);
                if (objectiveInfo == null)
                {
                    returnConfirm.result = (int)ConstVar.ReturnField.error;
                    return returnConfirm;
                }
                objectiveInfo.objectiveName = objectiveModel.objectiveName;
                objectiveInfo.weight = objectiveModel.weight;
                objectiveInfo.objectiveValue = objectiveModel.objectiveValue;
                objectiveInfo.expectedValue = objectiveModel.expectedValue;
                objectiveInfo.objectiveType = objectiveModel.objectiveType;
                objectiveInfo.responsibleOrg = result;
                objectiveInfo.responsibleUser = objectiveModel.responsibleUser;
                objectiveInfo.confirmUser = objectiveModel.confirmUser;
                objectiveInfo.checkType = objectiveModel.checkType;
                objectiveInfo.bonus = objectiveModel.bonus;
                objectiveInfo.maxValue = objectiveModel.maxValue;
                objectiveInfo.minValue = objectiveModel.minValue;
                objectiveInfo.startTime = objectiveModel.startTime;
                objectiveInfo.endTime = objectiveModel.endTime;
                objectiveInfo.alarmTime = objectiveModel.alarmTime;
                objectiveInfo.formula = objectiveModel.formula;
                objectiveInfo.description = objectiveModel.description;
                objectiveInfo.updateUser = userId;
                objectiveInfo.updateTime = DateTime.Now;
                //目标标签
                objectiveInfo.keyword = objectiveModel.keyword != null && objectiveModel.keyword.Length > 0 ? string.Join(",", objectiveModel.keyword) : null;

                //目标公式表中插入数据
                if (objectiveModel.formula == (int)ConstVar.ObjectiveFormulaType.userDefined && objectiveModel.objectiveFormula.Count > 0)
                {
                    returnConfirm.result = this.CheckFormulaValidity(0, objectiveModel.objectiveFormula);
                    if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;
                    this.AddObjectiveFormula(db, objectiveModel.objectiveId.Value, objectiveModel.objectiveFormula);
                }

                if (flag == 1)  //提交
                {
                    returnConfirm.confirmUser = objectiveModel.confirmUser;

                    if (operateFlag == 1)   //授权人的提交
                    {
                        objectiveInfo.authorizedUser = null;
                        //添加提交日志
                        AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                    }
                    else
                    {
                        if (objectiveModel.confirmUser != null && objectiveModel.confirmUser == userId)
                        {
                            objectiveInfo.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                            //日志：提交
                            AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                            //添加审核通过日志
                            AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.checkPass, userId);
                        }
                        else
                        {
                            objectiveInfo.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                            //添加提交日志
                            AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                        }
                    }
                }
                else   //保存
                {
                    returnConfirm.confirmUser = objectiveModel.responsibleUser;
                    objectiveInfo.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                }
                db.SaveChanges();
            }
            return returnConfirm;
        }

        #endregion 待提交目标提交保存操作

        #region 分解目标提交保存操作

        /// <summary>
        /// 分解目标提交保存操作
        /// </summary>
        /// <param name="objectiveModel">目标实体对象</param>
        /// <param name="userId">操作人Id</param>
        /// <param name="flag">1:提交目标 2：保存目标</param>
        /// <param name="objectiveId">目标ID</param>
        public ReturnConfirm SplitObjective(AddNewObjectiveModel objectiveModel, int userId, int flag, out int objectiveId)
        {
            objectiveId = 0;

            var returnConfirm = new ReturnConfirm();
            returnConfirm.result = (int)ConstVar.ReturnField.success;
            using (var db = new TargetNavigationDBEntities())
            {
                var result = 0;
                if (!int.TryParse(objectiveModel.responsibleOrg, out result))
                {
                    var orgInfo = db.tblOrganization.Where(p => p.organizationName == objectiveModel.responsibleOrg).FirstOrDefault();
                    if (orgInfo != null) result = orgInfo.organizationId;
                }
                //目标信息表中插入数据
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var id = db.prcGetPrimaryKey("tblObjective", obj).FirstOrDefault().Value;

                //Out参数objectiveId赋值
                objectiveId = id;

                var objective = new tblObjective
                {
                    objectiveId = id,
                    parentObjective = objectiveModel.parentObjective,
                    displayChangeFlag = false,
                    objectiveName = objectiveModel.objectiveName,
                    weight = objectiveModel.weight,
                    valueType = objectiveModel.valueType,
                    objectiveValue = objectiveModel.objectiveValue,
                    expectedValue = objectiveModel.expectedValue,
                    objectiveType = objectiveModel.objectiveType,
                    responsibleOrg = result,
                    responsibleUser = objectiveModel.responsibleUser,
                    confirmUser = objectiveModel.confirmUser,
                    checkType = objectiveModel.checkType,
                    bonus = objectiveModel.bonus,
                    maxValue = objectiveModel.maxValue,
                    minValue = objectiveModel.minValue,
                    startTime = objectiveModel.startTime,
                    endTime = objectiveModel.endTime,
                    alarmTime = objectiveModel.alarmTime,
                    formula = objectiveModel.formula,
                    description = objectiveModel.description,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    //目标标签
                    keyword = objectiveModel.keyword != null && objectiveModel.keyword.Length > 0 ? string.Join(",", objectiveModel.keyword) : null
                };

                //目标公式表中插入数据
                if (objectiveModel.formula == (int)ConstVar.ObjectiveFormulaType.userDefined && objectiveModel.objectiveFormula.Count > 0)
                {
                    returnConfirm.result = this.CheckFormulaValidity(0, objectiveModel.objectiveFormula);
                    if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;

                    foreach (var item in objectiveModel.objectiveFormula)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter formulaObj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        var formulaId = db.prcGetPrimaryKey("tblObjectiveFormula", formulaObj).FirstOrDefault().Value;
                        var formulaModel = new tblObjectiveFormula
                        {
                            formulaId = formulaId,
                            formulaNum = item.formulaNum,
                            objectiveId = objectiveId,
                            orderNum = item.orderNum,
                            field = item.field,
                            operate = item.operate,
                            numValue = item.numValue,
                            valueType = item.valueType
                        };
                        db.tblObjectiveFormula.Add(formulaModel);
                    };
                }

                if (flag == 1)  //提交
                {
                    if (objectiveModel.confirmUser != null && objectiveModel.confirmUser == userId)
                    {

                        returnConfirm.confirmUser = objectiveModel.confirmUser;
                        objective.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                        //日志：创建
                        AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                        //日志：提交
                        AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                        //添加审核通过日志
                        AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.checkPass, userId);
                    }
                    else
                    {

                        returnConfirm.confirmUser = objectiveModel.confirmUser;
                        objective.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                        //日志：创建
                        AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                        //添加提交日志
                        AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                    }
                }
                else   //保存
                {
                    returnConfirm.confirmUser = objectiveModel.responsibleUser;
                    objective.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                    //添加创建日志
                    AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                }
                //日志：父目标分解
                AddObjectiveOperate(db, objectiveModel.parentObjective.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.create, userId);
                db.tblObjective.Add(objective);
                db.SaveChanges();
            }
            return returnConfirm;
        }

        #endregion 分解目标提交保存操作

        #region 修改目标数据

        /// <summary>
        /// 修改目标数据
        /// </summary>
        /// <param name="objectiveModel">目标实体对象</param>
        /// <param name="objectiveFormulaInfo">目标公式信息</param>
        /// <param name="userId">用户Id</param>
        /// <param name="flag">1、保存 2、提交</param>
        /// <returns></returns>
        public ReturnConfirm EditObjective(AddNewObjectiveModel objectiveModel, int userId, int flag)
        {
            var returnConfirm = new ReturnConfirm();
            returnConfirm.result = (int)ConstVar.ReturnField.success;
            using (var db = new TargetNavigationDBEntities())
            {
                if (objectiveModel == null || objectiveModel.objectiveId == null)
                {
                    returnConfirm.result = (int)ConstVar.ReturnField.error;
                    return returnConfirm;
                }
                var oldObjectiveModel = GetObjective(db, objectiveModel.objectiveId.Value);
                if (oldObjectiveModel == null)
                {
                    returnConfirm.result = (int)ConstVar.ReturnField.error;
                    return returnConfirm;
                }
                oldObjectiveModel.updateUser = userId;
                oldObjectiveModel.updateTime = DateTime.Now;
                //目标标签
                oldObjectiveModel.keyword = objectiveModel.keyword != null && objectiveModel.keyword.Length > 0 ? string.Join(",", objectiveModel.keyword) : null;

                var result = 0;
                if (!int.TryParse(objectiveModel.responsibleOrg, out result))
                {
                    var orgInfo = db.tblOrganization.Where(p => p.organizationName == objectiveModel.responsibleOrg).FirstOrDefault();
                    if (orgInfo != null) result = orgInfo.organizationId;
                }
                objectiveModel.responsibleOrg = result.ToString();
                if (flag == 1) //保存的情况
                {
                    returnConfirm.confirmUser = oldObjectiveModel.responsibleUser;
                    if (oldObjectiveModel.status == (int)ConstVar.ObjectIndexStatus.unSubmit) //待提交
                    {
                        returnConfirm.result = this.updateObective(db, objectiveModel, oldObjectiveModel);
                        if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;
                    }
                    else if (oldObjectiveModel.status == (int)ConstVar.ObjectIndexStatus.unChecked) //待审核
                    {

                        oldObjectiveModel.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                        oldObjectiveModel.displayChangeFlag = true;
                        returnConfirm.result = this.updateObective(db, objectiveModel, oldObjectiveModel);
                        if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;
                        //添加日志
                        AddObjectiveOperate(db, objectiveModel.objectiveId.Value, objectiveModel.message, (int)ConstVar.ObjectiveOperaResult.update, userId);
                    }
                    else if (oldObjectiveModel.status == (int)ConstVar.ObjectIndexStatus.hasChecked)  //审核通过
                    {
                        //更新目标信息表
                        oldObjectiveModel.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                        oldObjectiveModel.displayChangeFlag = true;
                        oldObjectiveModel.actualStartTime = null;
                        //目标变更表中插入数据,更新信息表变更数据
                        returnConfirm.result = this.SubmitObjective(db, objectiveModel, oldObjectiveModel, userId);
                        if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;
                        //添加日志
                        AddObjectiveOperate(db, objectiveModel.objectiveId.Value, objectiveModel.message, (int)ConstVar.ObjectiveOperaResult.update, userId);
                    }
                }
                else     //提交的情况
                {
                    returnConfirm.confirmUser = oldObjectiveModel.confirmUser;
                    if (oldObjectiveModel.status == (int)ConstVar.ObjectIndexStatus.unSubmit) //待提交
                    {
                        returnConfirm.confirmUser = oldObjectiveModel.confirmUser;
                        //更新目标信息表
                        returnConfirm.result = this.updateObective(db, objectiveModel, oldObjectiveModel);
                        if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;
                        oldObjectiveModel.status = (int)ConstVar.ObjectIndexStatus.unChecked;

                        //添加日志
                        AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                    }
                    else if (oldObjectiveModel.status == (int)ConstVar.ObjectIndexStatus.unChecked) //待审核
                    {
                        //更新目标信息表
                        returnConfirm.result = this.updateObective(db, objectiveModel, oldObjectiveModel);
                        if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;
                        //添加日志:修改
                        this.AddObjectiveOperate(db, objectiveModel.objectiveId.Value, objectiveModel.message, (int)ConstVar.ObjectiveOperaResult.update, userId);
                        //添加日志:提交
                        this.AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);

                        //更新目标信息表
                        if (oldObjectiveModel.confirmUser == userId)
                        {

                            oldObjectiveModel.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                            //添加日志:审核通过
                            this.AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.checkPass, userId);
                        }
                    }
                    else if (oldObjectiveModel.status == (int)ConstVar.ObjectIndexStatus.hasChecked)  //审核通过
                    {
                        //更新目标信息表
                        oldObjectiveModel.displayChangeFlag = true;
                        //目标变更表中插入数据,更新信息表变更数据
                        returnConfirm.result = this.SubmitObjective(db, objectiveModel, oldObjectiveModel, userId);
                        if (returnConfirm.result != (int)ConstVar.ReturnField.success) return returnConfirm;

                        //添加日志:修改
                        this.AddObjectiveOperate(db, objectiveModel.objectiveId.Value, objectiveModel.message, (int)ConstVar.ObjectiveOperaResult.update, userId);
                        //添加日志:提交
                        AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.submit, userId);
                        //更新目标信息表
                        if (oldObjectiveModel.confirmUser == userId)      //确认人是操作用户的场合
                        {
                            oldObjectiveModel.actualStartTime = DateTime.Now;
                            //添加日志:审核通过
                            this.AddObjectiveOperate(db, objectiveModel.objectiveId.Value, string.Empty, (int)ConstVar.ObjectiveOperaResult.checkPass, userId);
                        }
                        else
                        {
                            oldObjectiveModel.actualStartTime = null;
                            oldObjectiveModel.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                        }
                    }
                }
                db.SaveChanges();
            }
            return returnConfirm;
        }

        #endregion 修改目标数据

        #region 删除目标文档

        /// <summary>
        /// 删除目标文档
        /// </summary>
        /// <param name="attachmentId">文档Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns>true:操作成功 false:操作失败</returns>
        public bool DeleteDocument(int objectiveId, int attachmentId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var docModel = db.tblObjectiveDocument.Where(p => p.attachmentId == attachmentId).FirstOrDefault();
                if (docModel == null) return false;
                db.tblObjectiveDocument.Remove(docModel);

                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.deleteDoc, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 删除目标文档

        #region 文件上传成功后数据库插数据

        /// <summary>
        /// 文件上传成功后数据库插数据
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="file">文件信息</param>
        /// <param name="userId">用户Id</param>
        public void InsertObjectiveDoc(int objectiveId, UploadFileModel file, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var attachmentId = db.prcGetPrimaryKey("tblObjectiveDocument", obj).FirstOrDefault().Value;

                var documentModel = new tblObjectiveDocument
                {
                    attachmentId = attachmentId,
                    objectiveId = objectiveId,
                    displayName = file.displayName,
                    saveName = file.saveName,
                    extension = file.extension,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now
                };
                db.tblObjectiveDocument.Add(documentModel);
                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.upload, userId);
                db.SaveChanges();
            }
        }

        #endregion 文件上传成功后数据库插数据

        #region 甘特图拖动更新

        /// <summary>
        /// 甘特图拖动更新
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">结束时间</param>
        /// <param name="userId">操作人Id</param>
        /// <param name="flag">1、整体拖动 2、改变时间长度</param>
        public bool MoveObjectiveGanttChart(int objectiveId, DateTime fromTime, DateTime toTime, int userId, int flag)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveModel = GetObjective(db, objectiveId);
                if (objectiveModel == null) return false;
                var startTime = objectiveModel.startTime;
                if (objectiveModel.status == (int)ConstVar.ObjectIndexStatus.hasChecked)
                {
                    objectiveModel.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                }
                objectiveModel.startTime = fromTime;
                objectiveModel.endTime = toTime;

                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.update, userId);
                if (flag == 1)
                {
                    var day = (fromTime - startTime.Value).Days;
                    //更新子目标的时间区间
                    UpdateChildObectiveTime(db, objectiveId, day, userId);
                }
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 甘特图子目标更新
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="day">拖动的天数</param>
        /// <param name="userId">操作人Id</param>
        public void MoveObjectiveGanttChart(TargetNavigationDBEntities db, int objectiveId, int day, int userId)
        {
            var childObjectiveList = (from o in db.tblObjective
                                      where !o.deleteFlag && o.parentObjective == objectiveId
                                      select o).ToList();
            if (childObjectiveList.Count() <= 0) return;
            foreach (var item in childObjectiveList)
            {
                if (item.status == (int)ConstVar.ObjectIndexStatus.hasChecked)
                {
                    item.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                }
                item.startTime = item.startTime.Value.AddDays(day);
                item.endTime = item.endTime.Value.AddDays(day);
                //添加日志
                AddObjectiveOperate(db, item.objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.update, userId);
                //递归更新
                MoveObjectiveGanttChart(db, item.objectiveId, day, userId);
            }
        }

        #endregion 甘特图拖动更新

        #region 获取目标详情

        /// <summary>
        /// 获取目标详情
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="flag"></param>
        /// <returns>详情实体对象</returns>
        public ObjectiveIndexModel GetObjectInfo(int objectiveId)
        {
            var objectiveModel = new ObjectiveIndexModel();
            using (var db = new TargetNavigationDBEntities())
            {
                //获取目标信息
                objectiveModel = (from o in db.tblObjective
                                  join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                  from resUser in group1.DefaultIfEmpty()
                                  join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                  from conUser in group2.DefaultIfEmpty()
                                  join r in db.tblOrganization on o.responsibleOrg equals r.organizationId into group3
                                  from r in group3.DefaultIfEmpty()
                                  orderby o.startTime descending
                                  where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag && o.objectiveId == objectiveId
                                  select new ObjectiveIndexModel
                                  {
                                      objectiveId = o.objectiveId,
                                      parentObjective = o.parentObjective,
                                      displayChangeFlag = o.displayChangeFlag,
                                      valueType = o.valueType,
                                      objectiveName = o.objectiveName,
                                      objectiveType = o.objectiveType,
                                      bonus = o.bonus,
                                      weight = o.weight,
                                      objectiveValue = o.objectiveValue,
                                      expectedValue = o.expectedValue,
                                      actualValue = o.actualValue,
                                      startTime = o.startTime.Value,
                                      endTime = o.endTime.Value,
                                      actualEndTime = o.actualEndTime,
                                      responsibleUser = o.responsibleUser,
                                      responsibleUserName = resUser.userName,
                                      responsibleOrg = o.responsibleOrg,
                                      responsibleOrgName = r.organizationName,
                                      description = o.description,
                                      alarmTime = o.alarmTime,
                                      confirmUser = o.confirmUser,
                                      confirmUserName = conUser.userName,
                                      checkType = o.checkType,
                                      maxValue = o.maxValue,
                                      minValue = o.minValue,
                                      formula = o.formula,
                                      status = o.status
                                  }).FirstOrDefault();
                if (objectiveModel != null)
                {
                    //目标标签
                    var keyword = db.tblObjective.Where(p => p.objectiveId == objectiveId).Select(p => p.keyword).FirstOrDefault();
                    objectiveModel.keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Split(',');

                    if (objectiveModel.parentObjective != null)
                    {
                        var parentObjectiveModel = db.tblObjective.Where(p => p.objectiveId == objectiveModel.parentObjective).FirstOrDefault();
                        if (parentObjectiveModel != null)
                        {
                            objectiveModel.minStartTime = parentObjectiveModel.startTime;
                            objectiveModel.maxEndTime = parentObjectiveModel.endTime;
                        }
                    }

                    var childObjectiveList = db.tblObjective.Where(p => p.parentObjective == objectiveModel.objectiveId).ToList();
                    if (childObjectiveList.Count > 0)
                    {
                        objectiveModel.maxStartTime = childObjectiveList.Min(a => a.startTime);
                        objectiveModel.minEndTime = childObjectiveList.Max(a => a.endTime);
                    }

                    //变更过且是待审核状态的目标，获取变更标志
                    if (objectiveModel.displayChangeFlag && objectiveModel.status == (int)ConstVar.ObjectIndexStatus.unChecked)
                    {
                        //获取该目标最新的变更信息
                        var query = (from x in db.tblObjectiveChange
                                     orderby x.createTime descending
                                     where x.objectiveId == objectiveId
                                     select x).FirstOrDefault();
                        if (query != null)
                        {
                            objectiveModel.ChangeInfo = new ObjectiveChangeInfo
                            {
                                objectiveNameUpdate = query.objectiveNameUpdate,
                                weightUpdate = query.weightUpdate,
                                objectiveValueUpdate = query.objectiveValueUpdate,
                                expectedValueUpdate = query.expectedValueUpdate,
                                bonusUpdate = query.bonusUpdate,
                                startTimeUpdate = query.startTimeUpdate,
                                endTimeUpdate = query.endTimeUpdate,
                                alarmTimeUpdate = query.alarmTimeUpdate
                            };
                        }
                    }

                    //获取目标的文档信息
                    objectiveModel.documentInfo = GetObjectiveDocuments(db, objectiveId);

                    //获取目标的日志信息
                    objectiveModel.operateLog = GetObjectiveLogs(db, objectiveId);

                    //获取目标的公式信息
                    objectiveModel.objectiveFormula = GetObjectiveFormula(db, objectiveId);

                    //获取子目标信息
                    objectiveModel.childrenObjective = GetThisChildrenObjectiveList(db, objectiveId);
                }
            }
            return objectiveModel;
        }

        #endregion 获取目标详情

        #region 获取待提交状态的目标详情

        /// <summary>
        /// 获取待提交状态的目标详情
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="flag"></param>
        /// <returns>详情实体对象</returns>
        public ObjectiveIndexModel GetSimpleObjectInfo(int objectiveId)
        {
            var objectiveModel = new ObjectiveIndexModel();
            using (var db = new TargetNavigationDBEntities())
            {
                //获取目标信息
                objectiveModel = (from o in db.tblObjective
                                  join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                                  from resUser in group1.DefaultIfEmpty()
                                  join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                                  from conUser in group2.DefaultIfEmpty()
                                  join r in db.tblOrganization on o.responsibleOrg equals r.organizationId into group3
                                  from r in group3.DefaultIfEmpty()
                                  orderby o.startTime descending
                                  where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag && o.objectiveId == objectiveId
                                  select new ObjectiveIndexModel
                                  {
                                      objectiveId = o.objectiveId,
                                      parentObjective = o.parentObjective,
                                      displayChangeFlag = o.displayChangeFlag,
                                      valueType = o.valueType,
                                      objectiveName = o.objectiveName,
                                      objectiveType = o.objectiveType,
                                      bonus = o.bonus,
                                      weight = o.weight,
                                      objectiveValue = o.objectiveValue,
                                      expectedValue = o.expectedValue,
                                      actualValue = o.actualValue,
                                      startTime = o.startTime.Value,
                                      endTime = o.endTime.Value,
                                      actualEndTime = o.actualEndTime,
                                      formula = o.formula,
                                      responsibleUser = o.responsibleUser,
                                      responsibleUserName = resUser.userName,
                                      responsibleOrg = o.responsibleOrg,
                                      responsibleOrgName = r.organizationName,
                                      description = o.description,
                                      alarmTime = o.alarmTime,
                                      confirmUser = o.confirmUser,
                                      confirmUserName = conUser.userName,
                                      checkType = o.checkType,
                                      status = o.status
                                  }).FirstOrDefault();

                if (objectiveModel != null)
                {
                    //目标标签
                    var keyword = db.tblObjective.Where(p => p.objectiveId == objectiveId).Select(p => p.keyword).FirstOrDefault();
                    objectiveModel.keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Split(',');

                    if (objectiveModel.parentObjective != null)
                    {
                        var parentObjective = db.tblObjective.Where(p => p.objectiveId == objectiveModel.parentObjective.Value).FirstOrDefault();
                        if (parentObjective != null)
                        {
                            objectiveModel.minStartTime = parentObjective.startTime;
                            objectiveModel.maxEndTime = parentObjective.endTime;
                        }
                        var childObjectiveList = db.tblObjective.Where(p => p.parentObjective == objectiveModel.objectiveId);
                        if (childObjectiveList.Count() > 0)
                        {
                            objectiveModel.maxStartTime = childObjectiveList.Min(x => x.startTime);
                            objectiveModel.minEndTime = childObjectiveList.Max(x => x.endTime);
                        }
                    }
                }
            }
            return objectiveModel;
        }

        #endregion 获取待提交状态的目标详情

        #region 目标审核

        /// <summary>
        /// 目标审核
        /// </summary>
        ///<param name="checkModel">审核信息实体模型</param>
        /// <param name="userId">操作人Id</param>
        /// <returns></returns>
        public ReturnConfirm ApproveObjective(ObjectiveCheckModel checkModel, int userId)
        {
            var objective = new ReturnConfirm();
            objective.confirmUser = 0;
            objective.result = (int)ConstVar.ReturnField.success;
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveModel = GetObjective(db, checkModel.objectiveId);
                if (objectiveModel == null)
                {
                    objective.result = (int)ConstVar.ReturnField.error;
                    return objective;
                }
                if (checkModel.result == (int)ConstVar.ObjectiveOperaResult.checkPass) //审核通过
                {
                    if (checkModel.objectiveFormulaInfo == null || checkModel.objectiveFormulaInfo.formula == null)
                    {
                        objective.result = (int)ConstVar.ReturnField.error;
                        return objective;
                    }
                    objective.confirmUser = objectiveModel.responsibleUser;
                    //更新目标信息表
                    objectiveModel.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                    objectiveModel.displayChangeFlag = false;
                    objectiveModel.formula = checkModel.objectiveFormulaInfo.formula;
                    objectiveModel.maxValue = checkModel.objectiveFormulaInfo.maxValue;
                    objectiveModel.minValue = checkModel.objectiveFormulaInfo.minValue;
                    objectiveModel.actualStartTime = DateTime.Now;
                    objectiveModel.updateUser = userId;
                    objectiveModel.updateTime = DateTime.Now;

                    if (checkModel.objectiveFormulaInfo.formula == (int)ConstVar.ObjectiveFormulaType.userDefined)   //自定义公式
                    {
                        //公式检查
                        objective.result = this.CheckFormulaValidity(0, checkModel.objectiveFormulaInfo.objectiveFormula);
                        if (objective.result != (int)ConstVar.ReturnField.success) return objective;
                        //更新目标公式
                        this.AddObjectiveFormula(db, checkModel.objectiveId, checkModel.objectiveFormulaInfo.objectiveFormula);
                    }

                    //添加日志
                    AddObjectiveOperate(db, checkModel.objectiveId, checkModel.message, (int)ConstVar.ObjectiveOperaResult.checkPass, userId);
                }
                else  //审核不通过
                {
                    objectiveModel.status = (int)ConstVar.ObjectIndexStatus.unSubmit;
                    objectiveModel.updateUser = userId;
                    objectiveModel.updateTime = DateTime.Now;
                    //变更表数据更新到目标信息表
                    if (objectiveModel.displayChangeFlag)
                    {
                        //获取该目标最新的变更信息
                        var query = (from x in db.tblObjectiveChange
                                     orderby x.createTime descending
                                     where x.objectiveId == checkModel.objectiveId
                                     select x).FirstOrDefault();
                        if (query != null)
                        {
                            objectiveModel.objectiveName = query.objectiveName;
                            objectiveModel.weight = query.weight;
                            objectiveModel.objectiveValue = query.objectiveValue;
                            objectiveModel.expectedValue = query.expectedValue;
                            objectiveModel.bonus = query.bonus;
                            objectiveModel.startTime = query.startTime;
                            objectiveModel.endTime = query.endTime;
                            objectiveModel.alarmTime = query.alarmTime;
                        }
                    }
                    //添加日志
                    AddObjectiveOperate(db, checkModel.objectiveId, checkModel.message, (int)ConstVar.ObjectiveOperaResult.checkNoPass, userId);
                }
                db.SaveChanges();
            }
            return objective;
        }

        #endregion 目标审核

        #region 目标提交确认

        /// <summary>
        /// 目标提交确认
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="actualValue">实际值</param>
        /// <param name="userId">操作人Id</param>
        /// <returns>操作类型：6：审核通过 7：审核不通过</returns>
        public int SubmitObjectiveExecuteResult(int objectiveId, string actualValue, int userId)
        {
            var confirmUser = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveModel = GetObjective(db, objectiveId);
                if (objectiveModel == null) return confirmUser = 0;
                objectiveModel.status = (int)ConstVar.ObjectIndexStatus.unConfirm;
                objectiveModel.actualValue = actualValue;
                confirmUser = objectiveModel.confirmUser.Value;
                //添加日志
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.submitConfirm, userId);
                db.SaveChanges();
            }
            return confirmUser;
        }

        #endregion 目标提交确认

        #region 目标确认

        /// <summary>
        /// 目标确认
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="message">操作意见</param>
        /// <param name="result">操作类型 11：确认通过 12：确认不通过</param>
        /// <param name="userId">操作人Id</param>
        /// <returns></returns>
        public bool ConfirmObjective(int objectiveId, string message, int result, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveModel = GetObjective(db, objectiveId);
                if (objectiveModel == null) return false;
                objectiveModel.updateUser = userId;
                objectiveModel.updateTime = DateTime.Now;
                if (result == (int)ConstVar.ObjectiveOperaResult.confirmPass)  //确认通过
                {
                    objectiveModel.status = (int)ConstVar.ObjectIndexStatus.hasCompleted;
                    objectiveModel.actualEndTime = DateTime.Now;
                    objectiveModel.progress = 100;
                    //添加日志
                    AddObjectiveOperate(db, objectiveId, message, (int)ConstVar.ObjectiveOperaResult.confirmPass, userId);
                }
                else
                {
                    objectiveModel.status = (int)ConstVar.ObjectIndexStatus.hasChecked;
                    //添加日志
                    AddObjectiveOperate(db, objectiveId, message, (int)ConstVar.ObjectiveOperaResult.confirmNoPass, userId);
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 目标确认

        #region 获取饼图数量统计

        /// <summary>
        /// 获取饼图数量统计
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="userId">用户Id</param>
        /// <returns>目标统计模型</returns>
        public List<FlowProcessModel> GetObjectiveProcessList(int year, int month, int userId)
        {
            var processList = new List<FlowProcessModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                processList = (from o in db.tblObjective
                               where !o.deleteFlag && ((o.startTime.Value.Year == year && o.startTime.Value.Month == month) || (o.endTime.Value.Year == year && o.endTime.Value.Month == month)) && (o.responsibleUser == userId || o.authorizedUser == userId)
                               group o by o.status into g
                               select new FlowProcessModel
                               {
                                   id = g.Key,
                                   text = (g.Key == (int)ConstVar.ObjectIndexStatus.unSubmit) ? "待提交" : ((g.Key == (int)ConstVar.ObjectIndexStatus.unChecked) ? "待审核" : (g.Key == (int)ConstVar.ObjectIndexStatus.hasChecked) ? "已审核" : (g.Key == (int)ConstVar.ObjectIndexStatus.unConfirm) ? "待确认" : "已完成"),
                                   count = g.Count()
                               }).ToList();
                processList = SupplementData(processList).OrderBy(p => p.id).ToList();
            }
            return processList;
        }

        #endregion 获取饼图数量统计

        #region 获取目标不同状态的数量统计信息

        /// <summary>
        /// 获取目标不同状态的数量统计信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="flag">1、我的目标 2、下属目标</param>
        /// <returns>不同状态的数量统计信息</returns>
        public List<FlowProcessModel> GetObjectiveStatusList(int userId, int flag)
        {
            var processList = new List<FlowProcessModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var nowTime = DateTime.Now.Date;
                if (flag == 1)
                {
                    processList = (from o in db.tblObjective
                                   where !o.deleteFlag && (o.responsibleUser == userId || o.authorizedUser == userId)
                                   group o by o.status into g
                                   select new FlowProcessModel
                                   {
                                       id = g.Key,
                                       text = (g.Key == (int)ConstVar.ObjectIndexStatus.unSubmit) ? "待提交" : ((g.Key == (int)ConstVar.ObjectIndexStatus.unChecked) ? "待审核" : (g.Key == (int)ConstVar.ObjectIndexStatus.hasChecked) ? "已审核" : (g.Key == (int)ConstVar.ObjectIndexStatus.unConfirm) ? "待确认" : "已完成"),
                                       count = g.Count()
                                   }).ToList();
                    processList = SupplementData(processList);
                    //超时列表
                    var overTimeList = db.tblObjective.Where(p => (p.responsibleUser == userId || p.authorizedUser == userId) && p.endTime.Value < nowTime && !p.deleteFlag && p.status == (int)ConstVar.ObjectIndexStatus.hasChecked).ToList();
                    var processModel = new FlowProcessModel
                    {
                        id = (int)ConstVar.ObjectIndexStatus.overTime,
                        text = "已超时",
                        count = overTimeList.Count
                    };
                    processList.Add(processModel);
                }
                else
                {
                    processList = (from o in db.tblObjective
                                   where !o.deleteFlag && o.confirmUser == userId
                                   group o by o.status into g
                                   select new FlowProcessModel
                                   {
                                       id = g.Key,
                                       text = (g.Key == (int)ConstVar.ObjectIndexStatus.unSubmit) ? "待提交" : ((g.Key == (int)ConstVar.ObjectIndexStatus.unChecked) ? "待审核" : (g.Key == (int)ConstVar.ObjectIndexStatus.hasChecked) ? "已审核" : (g.Key == (int)ConstVar.ObjectIndexStatus.unConfirm) ? "待确认" : "已完成"),
                                       count = g.Count()
                                   }).ToList();
                    processList = SupplementData(processList);
                    //超时列表
                    var overTimeList = db.tblObjective.Where(p => p.confirmUser == userId && p.endTime.Value < nowTime && !p.deleteFlag && p.status == (int)ConstVar.ObjectIndexStatus.hasChecked).ToList();
                    var processModel = new FlowProcessModel
                    {
                        id = (int)ConstVar.ObjectIndexStatus.overTime,
                        text = "已超时",
                        count = overTimeList.Count
                    };
                    processList.Add(processModel);
                }
            }
            processList = processList.OrderBy(p => p.id).ToList();
            return processList;
        }

        #endregion 获取目标不同状态的数量统计信息

        #region 更新目标进度

        /// <summary>
        /// 更新目标进度
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="newProcess">进度</param>
        /// <param name="userId">操作用户</param>
        /// <returns>true：更新成功 false：更新失败</returns>
        public bool UpdateObjectiveProcess(int objectiveId, int newProcess, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var objective = this.GetObjective(db, objectiveId);
                if (objective == null) return false;
                objective.progress = newProcess;
                objective.updateUser = userId;
                objective.updateTime = DateTime.Now;

                //操作表中插入数据
                this.AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.updateProcess, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 更新目标进度

        #region 获取目标公式信息

        /// <summary>
        /// 获取目标公式信息
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>目标公式列表</returns>
        public List<ObjectiveFormula> GetObjectFormula(int objectiveId)
        {
            var formulaList = new List<ObjectiveFormula>();
            using (var db = new TargetNavigationDBEntities())
            {
                formulaList = (from f in db.tblObjectiveFormula
                               orderby f.formulaNum, f.orderNum
                               where f.objectiveId == objectiveId
                               select new ObjectiveFormula
                               {
                                   formulaId = f.formulaId,
                                   formulaNum = f.formulaNum,
                                   orderNum = f.orderNum,
                                   field = f.field,
                                   operate = f.operate,
                                   numValue = f.numValue
                               }).ToList();
            }
            return formulaList;
        }

        #endregion 获取目标公式信息

        #region 获取目标文档信息

        /// <summary>
        /// 获取目标文档信息
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>文档列表</returns>
        public List<ObjectiveDocumentInfo> GetObjectiveNewDocuments(int objectiveId)
        {
            var docList = new List<ObjectiveDocumentInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                docList = (from d in db.tblObjectiveDocument
                           where d.objectiveId == objectiveId
                           select new ObjectiveDocumentInfo
                           {
                               attachmentId = d.attachmentId,
                               displayName = d.displayName,
                               saveName = d.saveName,
                               extension = d.extension
                           }).ToList();
            }
            return docList;
        }

        #endregion 获取目标文档信息

        #region 获取目标操作日志

        /// <summary>
        /// 获取目标操作日志
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>操作日志列表</returns>
        public List<ObjectiveOperateLog> GetObjectiveNewLogs(int objectiveId)
        {
            var logList = new List<ObjectiveOperateLog>();
            using (var db = new TargetNavigationDBEntities())
            {
                logList = (from log in db.tblObjectiveOperate
                           join u in db.tblUser on log.reviewUser equals u.userId into group1
                           from u in group1.DefaultIfEmpty()
                           join o in db.tblObjective on log.objectiveId equals o.objectiveId
                           where log.objectiveId == objectiveId
                           select new ObjectiveOperateLog
                           {
                               operateId = log.operateId,
                               objectiveName = o.objectiveName,
                               message = log.message,
                               result = log.result,
                               reviewUser = log.reviewUser,
                               reviewUserName = u.userName,
                               reviewTime = log.reviewTime
                           }).ToList();
            }
            return logList;
        }

        #endregion 获取目标操作日志

        #region 添加下载日志

        /// <summary>
        /// 添加下载日志
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="userId">用户Id</param>
        public void AddDownLoadOperate(int objectiveId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                AddObjectiveOperate(db, objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.down, userId);
                db.SaveChanges();
            }
        }

        #endregion 添加下载日志

        #region 判断该目标能否移动

        /// <summary>
        /// 判断该目标能否移动
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>true:能移动 false:不能移动</returns>
        public bool CheckObjectiveMove(int objectiveId)
        {
            var flag = true;
            using (var db = new TargetNavigationDBEntities())
            {
                var objectiveModel = this.GetObjective(db, objectiveId);
                if (objectiveModel == null) return false;
                if (objectiveModel.status == (int)ConstVar.ObjectIndexStatus.unConfirm || objectiveModel.status == (int)ConstVar.ObjectIndexStatus.hasCompleted) return false;
                flag = CheckChildObjective(db, objectiveId, flag);
            }
            return flag;
        }

        /// <summary>
        /// 递归子目标
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="flag">是否继续循环的标志</param>
        /// <returns>true:能移动 false:不能移动</returns>
        public bool CheckChildObjective(TargetNavigationDBEntities db, int objectiveId, bool flag)
        {
            var childObjective = (from o in db.tblObjective
                                  where !o.deleteFlag && o.parentObjective == objectiveId
                                  select o).ToList();
            if (childObjective.Count <= 0) return true;
            foreach (var item in childObjective)
            {
                if (item.status == (int)ConstVar.ObjectIndexStatus.unConfirm || item.status == (int)ConstVar.ObjectIndexStatus.hasCompleted) flag = false;
                if (flag)
                {
                    CheckChildObjective(db, item.objectiveId, flag);
                }
            }
            return flag;
        }

        #endregion 判断该目标能否移动

        #region 目标公式检查

        /// <summary>
        /// 目标公式检查
        /// </summary>
        /// <param name="objectiveCheckType"></param>
        /// <param name="formulaInfo"></param>
        /// <returns></returns>
        public int CheckFormulaValidity(int objectiveCheckType, List<ObjectiveFormula> formulaInfo)
        {
            int result = 0, count = 0;

            var formulaInfoByFormulaNum = formulaInfo.OrderBy(p => p.formulaNum).ThenBy(p => p.orderNum).GroupBy(p => p.formulaNum);

            foreach (var group in formulaInfoByFormulaNum)
            {
                count++;
                var sb = new StringBuilder();
                var groupList = group.Where(p => p != null).Distinct().ToList();
                groupList.RemoveRange(groupList.Count() - 2, 2);
                foreach (var item in groupList)
                {
                    //if (groupList.IndexOf(item) >= (group.Count() - 2)) break;
                    //字段
                    if (item.field.HasValue && string.IsNullOrEmpty(item.numValue))
                        sb.Append(999);

                    //操作符
                    if (!string.IsNullOrEmpty(item.operate))
                    {
                        var operate = string.Empty;
                        switch (item.operate)
                        {
                            case "|":
                                operate = "||";
                                break;

                            case "&":
                                operate = "&&";
                                break;

                            case "≥":
                                operate = ">=";
                                break;

                            case "≤":
                                operate = "<=";
                                break;

                            default:
                                operate = item.operate;
                                break;
                        }
                        sb.Append(operate);
                    }

                    //具体值
                    if (!string.IsNullOrEmpty(item.numValue) && item.valueType.HasValue && !item.field.HasValue)
                        sb.Append(111);
                }
                var retvar = StringUtils.StringCalculate(sb.ToString());
                result = (retvar == null || !(retvar is Boolean)) ? count : (int)ConstVar.ReturnField.success;

                if (result != (int)ConstVar.ReturnField.success) break;
            }

            return result;
        }

        #endregion 目标公式检查

        #region 私有方法

        #region 获取第一条目标信息

        /// <summary>
        /// 获取第一条目标信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>第一条目标信息</returns>
        private tblObjective GetObjective(TargetNavigationDBEntities db, int objectiveId)
        {
            var objective = db.tblObjective.Where(p => p.objectiveId == objectiveId && !p.deleteFlag).FirstOrDefault();
            return objective;
        }

        #endregion 获取第一条目标信息

        #region 添加目标操作表信息

        /// <summary>
        /// 添加目标操作表信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="message">操作意见</param>
        /// <param name="result">操作类型</param>
        /// <param name="reviewUser">操作人</param>
        private void AddObjectiveOperate(TargetNavigationDBEntities db, int objectiveId, string message, int result, int reviewUser)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblObjectiveOperate", obj).FirstOrDefault().Value;

            var operateModel = new tblObjectiveOperate
            {
                operateId = operId,
                objectiveId = objectiveId,
                message = message,
                result = result,
                reviewUser = reviewUser,
                reviewTime = DateTime.Now
            };
            db.tblObjectiveOperate.Add(operateModel);
        }

        #endregion 添加目标操作表信息

        #region 保存更新目标信息表

        /// <summary>
        /// 保存更新目标信息表
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveModel">新的目标模型</param>
        /// <param name="oldObjectiveModel">数据库中的数据模型</param>
        private int updateObective(TargetNavigationDBEntities db, AddNewObjectiveModel objectiveModel, tblObjective oldObjectiveModel)
        {
            oldObjectiveModel.parentObjective = objectiveModel.parentObjective;
            oldObjectiveModel.displayChangeFlag = false;
            oldObjectiveModel.objectiveName = objectiveModel.objectiveName;
            oldObjectiveModel.objectiveType = objectiveModel.objectiveType;
            oldObjectiveModel.responsibleOrg = Convert.ToInt32(objectiveModel.responsibleOrg);
            oldObjectiveModel.bonus = objectiveModel.bonus;
            oldObjectiveModel.weight = objectiveModel.weight;
            oldObjectiveModel.checkType = objectiveModel.checkType;
            oldObjectiveModel.objectiveValue = objectiveModel.objectiveValue;
            oldObjectiveModel.expectedValue = objectiveModel.expectedValue;
            oldObjectiveModel.description = objectiveModel.description;
            oldObjectiveModel.startTime = objectiveModel.startTime;
            oldObjectiveModel.endTime = objectiveModel.endTime;
            oldObjectiveModel.alarmTime = objectiveModel.alarmTime;
            oldObjectiveModel.responsibleOrg = Convert.ToInt32(objectiveModel.responsibleOrg);
            oldObjectiveModel.responsibleUser = objectiveModel.responsibleUser;
            oldObjectiveModel.confirmUser = objectiveModel.confirmUser;
            if (objectiveModel.objectiveFormulaInfo != null)
            {
                oldObjectiveModel.formula = objectiveModel.objectiveFormulaInfo.formula;
                //默认公式
                if (objectiveModel.objectiveFormulaInfo.formula == (int)ConstVar.ObjectiveFormulaType.defaultFormula)
                {
                    oldObjectiveModel.maxValue = objectiveModel.objectiveFormulaInfo.maxValue;
                    oldObjectiveModel.minValue = objectiveModel.objectiveFormulaInfo.minValue;
                }
                //自定义公式
                else if (objectiveModel.objectiveFormulaInfo.formula == (int)ConstVar.ObjectiveFormulaType.userDefined)
                {
                    var returnFlag = this.CheckFormulaValidity(0, objectiveModel.objectiveFormulaInfo.objectiveFormula);
                    if (returnFlag != (int)ConstVar.ReturnField.success) return returnFlag;

                    this.AddObjectiveFormula(db, oldObjectiveModel.objectiveId, objectiveModel.objectiveFormulaInfo.objectiveFormula);
                }
            }
            return (int)ConstVar.ReturnField.success;
        }

        #endregion 保存更新目标信息表

        #region 提交变更后的目标信息

        /// <summary>
        /// 提交变更后的目标信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveModel">变更后的数据</param>
        /// <param name="oldObjectiveModel">变更前的数据</param>
        /// <param name="userId">操作者Id</param>
        private int SubmitObjective(TargetNavigationDBEntities db, AddNewObjectiveModel objectiveModel, tblObjective oldObjectiveModel, int userId)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var changeId = db.prcGetPrimaryKey("tblObjectiveChange", obj).FirstOrDefault().Value;
            var changeModel = new tblObjectiveChange();
            changeModel.changeId = changeId;
            changeModel.objectiveId = oldObjectiveModel.objectiveId;
            changeModel.objectiveName = oldObjectiveModel.objectiveName;
            changeModel.objectiveNameUpdate = (oldObjectiveModel.objectiveName != objectiveModel.objectiveName) ? true : false;
            changeModel.weight = oldObjectiveModel.weight;
            changeModel.weightUpdate = (oldObjectiveModel.weight != objectiveModel.weight) ? true : false;
            changeModel.objectiveValue = oldObjectiveModel.objectiveValue;
            changeModel.objectiveValueUpdate = (oldObjectiveModel.objectiveValue != objectiveModel.objectiveValue) ? true : false;
            changeModel.expectedValue = oldObjectiveModel.expectedValue;
            changeModel.expectedValueUpdate = (oldObjectiveModel.expectedValue != objectiveModel.expectedValue) ? true : false;
            changeModel.objectiveType = oldObjectiveModel.objectiveType;
            changeModel.responsibleOrg = oldObjectiveModel.responsibleOrg;
            changeModel.responsibleUser = oldObjectiveModel.responsibleUser;
            changeModel.confirmUser = oldObjectiveModel.confirmUser;
            changeModel.checkType = oldObjectiveModel.checkType;
            changeModel.bonus = oldObjectiveModel.bonus;
            changeModel.bonusUpdate = (oldObjectiveModel.bonus != objectiveModel.bonus) ? true : false;
            changeModel.startTime = oldObjectiveModel.startTime;
            changeModel.startTimeUpdate = (oldObjectiveModel.startTime != objectiveModel.startTime) ? true : false;
            changeModel.endTime = oldObjectiveModel.endTime;
            changeModel.endTimeUpdate = (oldObjectiveModel.endTime != objectiveModel.endTime) ? true : false;
            changeModel.alarmTime = oldObjectiveModel.alarmTime;
            changeModel.alarmTimeUpdate = (oldObjectiveModel.alarmTime != objectiveModel.alarmTime) ? true : false;
            changeModel.progress = oldObjectiveModel.progress;
            changeModel.createUser = userId;
            changeModel.createTime = DateTime.Now;
            db.tblObjectiveChange.Add(changeModel);

            //更新目标信息表中的数据
            oldObjectiveModel.objectiveName = objectiveModel.objectiveName;
            oldObjectiveModel.weight = objectiveModel.weight;
            oldObjectiveModel.objectiveValue = objectiveModel.objectiveValue;
            oldObjectiveModel.expectedValue = objectiveModel.expectedValue;
            oldObjectiveModel.actualValue = objectiveModel.actualValue;
            oldObjectiveModel.bonus = objectiveModel.bonus;
            oldObjectiveModel.startTime = objectiveModel.startTime;
            oldObjectiveModel.endTime = objectiveModel.endTime;
            oldObjectiveModel.alarmTime = objectiveModel.alarmTime;
            oldObjectiveModel.description = objectiveModel.description;
            if (objectiveModel.objectiveFormulaInfo != null)
            {
                oldObjectiveModel.formula = objectiveModel.objectiveFormulaInfo.formula;
                //默认公式
                if (objectiveModel.objectiveFormulaInfo.formula == (int)ConstVar.ObjectiveFormulaType.defaultFormula)
                {
                    oldObjectiveModel.maxValue = objectiveModel.objectiveFormulaInfo.maxValue;
                    oldObjectiveModel.minValue = objectiveModel.objectiveFormulaInfo.minValue;
                }
                //自定义公式
                else if (objectiveModel.objectiveFormulaInfo.formula == (int)ConstVar.ObjectiveFormulaType.userDefined)
                {
                    var returnFlag = this.CheckFormulaValidity(0, objectiveModel.objectiveFormulaInfo.objectiveFormula);
                    if (returnFlag != (int)ConstVar.ReturnField.success) return returnFlag;

                    this.AddObjectiveFormula(db, oldObjectiveModel.objectiveId, objectiveModel.objectiveFormulaInfo.objectiveFormula);
                }
            }
            return (int)ConstVar.ReturnField.success;
        }

        #endregion 提交变更后的目标信息

        #region 获取目标文档信息

        /// <summary>
        /// 获取目标文档信息
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>文档列表</returns>
        private List<ObjectiveDocumentInfo> GetObjectiveDocuments(TargetNavigationDBEntities db, int objectiveId)
        {
            var docList = new List<ObjectiveDocumentInfo>();
            docList = (from d in db.tblObjectiveDocument
                       where d.objectiveId == objectiveId
                       select new ObjectiveDocumentInfo
                       {
                           attachmentId = d.attachmentId,
                           displayName = d.displayName,
                           saveName = d.saveName,
                           extension = d.extension
                       }).ToList();
            return docList;
        }

        #endregion 获取目标文档信息

        #region 获取目标操作日志

        /// <summary>
        /// 获取目标操作日志
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>操作日志列表</returns>
        private List<ObjectiveOperateLog> GetObjectiveLogs(TargetNavigationDBEntities db, int objectiveId)
        {
            var logList = new List<ObjectiveOperateLog>();
            logList = (from log in db.tblObjectiveOperate
                       join u in db.tblUser on log.reviewUser equals u.userId into group1
                       from u in group1.DefaultIfEmpty()
                       where log.objectiveId == objectiveId
                       select new ObjectiveOperateLog
                       {
                           operateId = log.operateId,
                           message = log.message,
                           result = log.result,
                           reviewUser = log.reviewUser,
                           reviewUserName = u.userName,
                           reviewTime = log.reviewTime
                       }).ToList();
            return logList;
        }

        #endregion 获取目标操作日志

        #region 获取目标公式信息

        /// <summary>
        /// 获取目标公式信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>目标公式列表</returns>
        private List<ObjectiveFormula> GetObjectiveFormula(TargetNavigationDBEntities db, int objectiveId)
        {
            var formulaList = (from f in db.tblObjectiveFormula
                               orderby f.formulaNum, f.orderNum
                               where f.objectiveId == objectiveId
                               select new ObjectiveFormula
                               {
                                   formulaId = f.formulaId,
                                   formulaNum = f.formulaNum,
                                   orderNum = f.orderNum,
                                   field = f.field,
                                   operate = f.operate,
                                   numValue = f.numValue,
                                   valueType = f.valueType
                               }).ToList();
            return formulaList;
        }

        #endregion 获取目标公式信息

        #region 补充遗漏的统计数据

        /// <summary>
        /// 补充遗漏的统计数据
        /// </summary>
        /// <param name="processList">统计集合</param>
        /// <returns>完整的统计数据</returns>
        private List<FlowProcessModel> SupplementData(List<FlowProcessModel> processList)
        {
            if (processList.Where(p => p.id == (int)ConstVar.ObjectIndexStatus.unSubmit).Count() <= 0)
            {
                var unSubmit = new FlowProcessModel();
                unSubmit.id = (int)ConstVar.ObjectIndexStatus.unSubmit;
                unSubmit.text = "待提交";
                unSubmit.count = 0;
                processList.Add(unSubmit);
            }
            if (processList.Where(p => p.id == (int)ConstVar.ObjectIndexStatus.unChecked).Count() <= 0)
            {
                var unCheck = new FlowProcessModel();
                unCheck.id = (int)ConstVar.ObjectIndexStatus.unChecked;
                unCheck.text = "待审核";
                unCheck.count = 0;
                processList.Add(unCheck);
            }
            if (processList.Where(p => p.id == (int)ConstVar.ObjectIndexStatus.hasChecked).Count() <= 0)
            {
                var hasChecked = new FlowProcessModel();
                hasChecked.id = (int)ConstVar.ObjectIndexStatus.hasChecked;
                hasChecked.text = "已审核";
                hasChecked.count = 0;
                processList.Add(hasChecked);
            }
            if (processList.Where(p => p.id == (int)ConstVar.ObjectIndexStatus.unConfirm).Count() <= 0)
            {
                var unConfirm = new FlowProcessModel();
                unConfirm.id = (int)ConstVar.ObjectIndexStatus.unConfirm;
                unConfirm.text = "待确认";
                unConfirm.count = 0;
                processList.Add(unConfirm);
            }
            if (processList.Where(p => p.id == (int)ConstVar.ObjectIndexStatus.hasCompleted).Count() <= 0)
            {
                var hasCompleted = new FlowProcessModel();
                hasCompleted.id = (int)ConstVar.ObjectIndexStatus.hasCompleted;
                hasCompleted.text = "已完成";
                hasCompleted.count = 0;
                processList.Add(hasCompleted);
            }
            return processList;
        }

        #endregion 补充遗漏的统计数据

        #region 获取子目标数据

        /// <summary>
        /// 获取子目标数据
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <returns>子目标集合</returns>
        public List<ChildObjective> GetThisChildrenObjectiveList(TargetNavigationDBEntities db, int objectiveId)
        {
            var childObjectiveList = (from o in db.tblObjective
                                      join u in db.tblUser on o.responsibleUser equals u.userId into group1
                                      from u in group1.DefaultIfEmpty()
                                      join org in db.tblOrganization on o.responsibleOrg equals org.organizationId into group2
                                      from org in group2.DefaultIfEmpty()
                                      where o.parentObjective == objectiveId && !o.deleteFlag
                                      select new ChildObjective
                                      {
                                          objectiveId = o.objectiveId,
                                          parentObjectiveId = objectiveId,
                                          objectiveName = o.objectiveName,
                                          objectiveType = o.objectiveType,
                                          weight = o.weight,
                                          responsibleOrgName = org.organizationName,
                                          responsibleUserName = u.userName
                                      }).ToList();
            return childObjectiveList;
        }

        #endregion 获取子目标数据

        #region 递归获取子目标列表

        /// <summary>
        /// 递归获取子目标列表
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="childList">子目标列表</param>
        /// <returns>子目标列表</returns>
        private List<ObjectiveHasChildModel> GetNextObjectiveList(TargetNavigationDBEntities db, int objectiveId, List<ObjectiveHasChildModel> childList)
        {
            childList = (from o in db.tblObjective
                         join resUser in db.tblUser on o.responsibleUser equals resUser.userId into group1
                         from resUser in group1.DefaultIfEmpty()
                         join conUser in db.tblUser on o.confirmUser equals conUser.userId into group2
                         from conUser in group2.DefaultIfEmpty()
                         join authorizedUser in db.tblUser on o.authorizedUser equals authorizedUser.userId into group3
                         from authorizedUser in group3.DefaultIfEmpty()
                         orderby o.startTime descending
                         where !o.deleteFlag && !resUser.deleteFlag && !conUser.deleteFlag && o.parentObjective == objectiveId
                         select new ObjectiveHasChildModel
                         {
                             objectiveId = o.objectiveId,
                             parentObjective = o.parentObjective,
                             objectiveName = o.objectiveName,
                             objectiveType = o.objectiveType,
                             bonus = o.bonus,
                             weight = o.weight,
                             objectiveValue = o.objectiveValue,
                             expectedValue = o.expectedValue,
                             startTime = o.startTime,
                             endTime = o.endTime,
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
                             progress = o.progress
                         }).ToList();
            if (childList.Count <= 0) return childList;
            foreach (var item in childList)
            {
                item.objectiveTypeName = item.objectiveType == 2 ? shareBll.GetUsernByUserId(db, item.responsibleUser.Value, item.responsibleOrg.Value) : shareBll.GetOrgStringByOrgIdNew(db, item.responsibleOrg.Value, new List<string>());
                item.childObjectiveList = GetNextObjectiveList(db, item.objectiveId, childList);
            }
            return childList;
        }

        #endregion 递归获取子目标列表

        #region 更新子目标的时间范围

        /// <summary>
        /// 更新子目标的时间范围
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="day">拖动的天数</param>
        /// <param name="userId">操作人Id</param>
        private void UpdateChildObectiveTime(TargetNavigationDBEntities db, int objectiveId, int day, int userId)
        {
            var childObjectiveList = db.tblObjective.Where(p => p.parentObjective == objectiveId && !p.deleteFlag).ToList();
            if (childObjectiveList.Count < 0) return;
            foreach (var item in childObjectiveList)
            {
                item.startTime = item.startTime.Value.AddDays(day);
                item.endTime = item.endTime.Value.AddDays(day);
                item.status = (int)ConstVar.ObjectIndexStatus.unChecked;
                item.updateUser = userId;
                item.updateTime = DateTime.Now;
                //添加日志
                AddObjectiveOperate(db, item.objectiveId, string.Empty, (int)ConstVar.ObjectiveOperaResult.update, userId);
                UpdateChildObectiveTime(db, item.objectiveId, day, userId);
            }
        }

        #endregion 更新子目标的时间范围

        #region 更新目标公式信息

        /// <summary>
        /// 更新目标公式信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="objectiveId">目标Id</param>
        /// <param name="objectiveFormula">目标公式集合</param>
        private void AddObjectiveFormula(TargetNavigationDBEntities db, int objectiveId, List<ObjectiveFormula> objectiveFormula)
        {
            //删除原有的公式信息
            var formulaList = db.tblObjectiveFormula.Where(p => p.objectiveId == objectiveId);
            if (formulaList.Count() > 0)
            {
                db.tblObjectiveFormula.RemoveRange(formulaList);
            }
            //插入新的公式
            if (objectiveFormula.Count > 0)
            {
                foreach (var item in objectiveFormula)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter formulaObj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var formulaId = db.prcGetPrimaryKey("tblObjectiveFormula", formulaObj).FirstOrDefault().Value;
                    var formulaModel = new tblObjectiveFormula
                    {
                        formulaId = formulaId,
                        formulaNum = item.formulaNum,
                        objectiveId = objectiveId,
                        orderNum = item.orderNum,
                        field = item.field,
                        operate = item.operate,
                        numValue = item.numValue,
                        valueType = item.valueType
                    };
                    db.tblObjectiveFormula.Add(formulaModel);
                };
            }
        }

        #endregion 更新目标公式信息

        #endregion 私有方法
    }
}