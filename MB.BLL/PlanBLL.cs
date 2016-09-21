using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MB.Common;
using MB.DAL;
using MB.Model;
using System.Threading.Tasks;



namespace MB.BLL
{
    public class PlanBLL : IPlanBLL
    {
        #region 变量区域

        //定义状态常量
        private const string Uncommitted = "待提交";

        private const string WaitSubmit = "待审核";
        private const string HasSubmit = "已审核";
        private const string WaitConfirm = "待确认";
        private const string HasConfirm = "已确认";
        private const string HasComplete = "已完成";
        private const string HasStop = "已中止";
        #endregion 变量区域

        #region 查询计划信息列表（包括循环计划）

        /// <summary>
        /// 查询计划信息列表（包括循环计划）
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>计划列表</returns>
        public List<PlanInfo> GetPlanList(int userId, string condition, string coopCondition, DateTime start, DateTime end)
        {
            var list = new List<PlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from c in db.vPlanList
                        //from c in db.tblPlan
                        //    join b in db.tblProject on c.projectId equals b.projectId into group1
                        //    from b in group1.DefaultIfEmpty()
                        //    join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                        //    from d in group2.DefaultIfEmpty()
                        //    join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                        //    from e in group3.DefaultIfEmpty()
                        //    join f in db.tblUser on c.responsibleUser equals f.userId into group4
                        //    from f in group4.DefaultIfEmpty()
                        //    join create in db.tblUser on c.createUser equals create.userId into group5
                        //    from create in group5.DefaultIfEmpty()
                        //    where !c.deleteFlag
                        select new PlanInfo
                        {
                            planId = c.planId,
                            parentPlan = c.parentPlan,
                            executionModeId = c.executionModeId,
                            responsibleOrganization = c.responsibleOrganization,
                            responsibleUser = c.responsibleUser,
                            responsibleUserName = c.responsibleUserName,
                            confirmOrganization = c.confirmOrganization,
                            confirmUser = c.confirmUser,
                            confirmUserName = c.confirmUserName,
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
                            status = c.status,
                            stop = c.stop,
                            createTime = c.createTime,
                            updateTime = c.updateTime,
                            initial = c.initial,
                            withSub = c.withSub,
                            archive = c.archive,
                            archiveTime = c.archiveTime,
                            autoStart = c.autoStart,
                            // eventOutputId = c.eventOutputId,
                            eventOutput = c.eventOutput,
                            createUserName = c.createUserName,
                            responsibleUserImage = (c.responsibleUserImage == null || c.responsibleUserImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.responsibleUserImage),
                            confirmUserImage = (c.confirmUserImage == null || c.confirmUserImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.confirmUserImage),
                            isLoopPlan = 0,
                            IsCollPlan = 0,
                            isFronPlan = c.withFront.Value ? 1 : 0,
                            withFront = c.withFront,
                            realTime = c.quantity == null ? 0 : c.quantity * c.time,
                            effectiveTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,

                            projectId = c.projectId,
                            parentProject = c.parentProject,
                            projectName = c.projectName,
                            proOrderNumber = c.orderNumber,

                            organizationId = c.organizationId,
                            parentOrganization = c.parentOrganization,
                            schemaName = c.schemaName,
                            organizationName = c.organizationName,
                            orgOrderNumber = c.orderNumber,

                            executionId = c.executionId,
                            executionMode = c.executionMode
                        }).Where(condition).Where("endTime >=@0 And endTime <@1", start, end).ToList();
                //if (coopCondition != "1")
                //{
                //    //查询协作计划
                //    var collPlanList = (from c in db.vPlanCooperation
                //                        //from c in db.tblPlan
                //                        //                join b in db.tblProject on c.projectId equals b.projectId into group1
                //                        //                from b in group1.DefaultIfEmpty()
                //                        //                join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                //                        //                from d in group2.DefaultIfEmpty()
                //                        //                join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                //                        //                from e in group3.DefaultIfEmpty()
                //                        //                join f in db.tblUser on c.responsibleUser equals f.userId into group4
                //                        //                from f in group4.DefaultIfEmpty()
                //                        //                join g in db.tblPlanCooperation on c.planId equals g.planId into group5
                //                        //                from g in group5.DefaultIfEmpty()
                //                        where c.userId == userId
                //                        select new PlanInfo
                //                        {
                //                            planId = c.planId,
                //                            parentPlan = c.parentPlan,
                //                            executionModeId = c.executionModeId,
                //                            responsibleOrganization = c.responsibleOrganization,
                //                            responsibleUser = c.userId,
                //                            responsibleUserName = c.responsibleUserName,
                //                            smallImage = (c.bigImage == null || c.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.bigImage),
                //                            confirmOrganization = c.confirmOrganization,
                //                            confirmUser = c.confirmUser,
                //                            confirmUserName = c.confirmUserName,
                //                            startTime = c.startTime,
                //                            endTime = c.endTime.Value,
                //                            workTime = c.workTime,
                //                            comment = c.comment,
                //                            alert = c.alert,
                //                            importance = c.importance,
                //                            urgency = c.urgency,
                //                            difficulty = c.difficulty,
                //                            progress = c.progress,
                //                            quantity = c.quantity,
                //                            time = c.time,
                //                            completeQuantity = c.completeQuantity,
                //                            completeQuality = c.completeQuality,
                //                            completeTime = c.completeTime,
                //                            status = c.status,
                //                            stop = c.stop,
                //                            initial = c.initial,
                //                            withSub = c.withSub,
                //                            archive = c.archive,
                //                            archiveTime = c.archiveTime,
                //                            autoStart = c.autoStart,
                //                            //eventOutputId = c.eventOutputId,
                //                            eventOutput = c.eventOutput,
                //                            createTime = c.createTime,
                //                            updateTime = c.updateTime,
                //                            isLoopPlan = 0,
                //                            IsCollPlan = 1,
                //                            isFronPlan = c.withFront.Value ? 1 : 0,
                //                            withFront = c.withFront,
                //                            realTime = c.quantity == null ? 0 : c.quantity * c.time,
                //                            effectiveTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,

                //                            projectId = c.projectId,
                //                            parentProject = c.parentProject,
                //                            projectName = c.projectName,
                //                            proOrderNumber = c.orderNumber,

                //                            organizationId = c.organizationId,
                //                            parentOrganization = c.parentOrganization,
                //                            schemaName = c.schemaName,
                //                            organizationName = c.organizationName,
                //                            orgOrderNumber = c.orderNumber,

                //                            executionId = c.executionId,
                //                            executionMode = c.executionMode
                //                        }).Where(coopCondition).Where("endTime >=@0 And endTime <@1", start, end).ToList();
                //    if (collPlanList.Count > 0)
                //    {
                //        list.AddRange(collPlanList);
                //    }
                //}

                var stringList = new List<string>();
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        item.endTimeNew = item.endTime == null ? "" : item.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                        item.createTimeNew = item.createTime == null ? "" : item.createTime.ToString("yyyy-MM-dd HH:mm");
                        stringList.Clear();
                        item.organizationNameNew = GetOrgStringByOrgId(db, item.organizationId, stringList);
                        stringList.Clear();
                        item.projectNameNew = GetProListByOrgId(db, item.projectId, stringList);
                    }
                }
            }
            return list;
        }

        public List<PlanInfo> GetPlanListDESC(int userId)
        {
            DateTime New = DateTime.Now;
            int weeknow = Convert.ToInt32(New.DayOfWeek);
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = (7 - weeknow);

            //本周最后一天
            string LastDay = New.AddDays(daydiff).ToString("yyyy-MM-dd");
            DateTime weekDay = DateTime.Parse(LastDay);
            var list = new List<PlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //list = (from b in db.tblPlan
                //        join c in db.tblProject on b.projectId equals c.projectId
                //        join d in db.tblOrganization on b.organizationId equals d.organizationId
                //        join e in db.tblExecutionMode on b.executionModeId equals e.executionId
                //        join f in db.tblUser on b.responsibleUser equals f.userId 
                //        where b.responsibleUser == userId  
                //        select new PlanInfo
                //        {
                //            planId = b.planId,
                //            parentPlan = b.parentPlan,
                //            executionModeId = b.executionModeId,
                //            responsibleOrganization = b.responsibleOrganization,
                //            responsibleUser = f.userId,
                //            responsibleUserName = f.userName,
                //            confirmOrganization = b.confirmOrganization,
                //            confirmUser = b.confirmUser,
                //            confirmUserName = db.tblUser.Where(p => p.userId == b.confirmUser).FirstOrDefault().userName,
                //            startTime = b.startTime,
                //            endTime = b.endTime.Value,
                //            workTime = b.workTime,
                //            comment = b.comment,
                //            alert = b.alert,
                //            importance = b.importance,
                //            urgency = b.urgency,
                //            difficulty = b.difficulty,
                //            progress = b.progress,
                //            quantity = b.quantity,
                //            time = b.time,
                //            completeQuantity = b.completeQuantity,
                //            completeQuality = b.completeQuality,
                //            completeTime = b.completeTime,
                //            status = c.status,
                //            stop = b.stop,
                //            initial = b.initial,
                //            withSub = b.withSub,
                //            archive = b.archive,
                //            archiveTime = b.archiveTime,
                //            autoStart = b.autoStart,
                //            //eventOutputId = c.eventOutputId,
                //            eventOutput = b.eventOutput,
                //            isLoopPlan = 0,
                //            IsCollPlan = 0, 
                //            projectId = b.projectId,
                //            parentProject = c.parentProject,
                //            projectName = c.projectName,
                //            proOrderNumber = c.orderNumber,

                //            organizationId = d.organizationId,
                //            parentOrganization = d.parentOrganization,
                //            schemaName = d.schemaName,
                //            organizationName = d.organizationName,
                //            orgOrderNumber = d.orderNumber,

                //            executionId = e.executionId,
                //            executionMode = e.executionMode
                //        }).ToList();
                list = (from c in db.vPlanList
                        where !c.deleteFlag && c.responsibleUser == userId && c.status == 0 && c.status == 30
                        select new PlanInfo
                        {
                            planId = c.planId,
                            parentPlan = c.parentPlan,
                            executionModeId = c.executionModeId,
                            responsibleOrganization = c.responsibleOrganization,
                            responsibleUser = c.responsibleUser,
                            responsibleUserName = c.responsibleUserName,
                            confirmOrganization = c.confirmOrganization,
                            confirmUser = c.confirmUser,
                            confirmUserName = c.confirmUserName,
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
                            status = c.status,
                            stop = c.stop,
                            createTime = c.createTime,
                            updateTime = c.updateTime,
                            initial = c.initial,
                            withSub = c.withSub,
                            archive = c.archive,
                            archiveTime = c.archiveTime,
                            autoStart = c.autoStart,
                            // eventOutputId = c.eventOutputId,
                            eventOutput = c.eventOutput,
                            createUserName = c.createUserName, 
                            responsibleUserImage = (c.responsibleUserImage == null || c.responsibleUserImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.responsibleUserImage),
                            confirmUserImage = (c.confirmUserImage == null || c.confirmUserImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.confirmUserImage),
                            isLoopPlan = 0,
                            IsCollPlan = 0,
                            isFronPlan = c.withFront.Value ? 1 : 0,
                            withFront = c.withFront,
                            realTime = c.quantity == null ? 0 : c.quantity * c.time,
                            effectiveTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,

                            projectId = c.projectId,
                            parentProject = c.parentProject,
                            projectName = c.projectName,
                            proOrderNumber = c.orderNumber,

                            organizationId = c.organizationId,
                            parentOrganization = c.parentOrganization,
                            schemaName = c.schemaName,
                            organizationName = c.organizationName,
                            orgOrderNumber = c.orderNumber,

                            executionId = c.executionId,
                            executionMode = c.executionMode
                        }).ToList();

            }
            return list;
        }

        #endregion 查询计划信息列表（包括循环计划）

        #region 获取计划日程化普通计划列表(时间筛选字段不同：updateTime)

        /// <summary>
        /// 查询计划信息列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>计划列表</returns>
        public List<PlanInfo> GetCalendarPlanList(int userId, string condition, string coopCondition, DateTime start, DateTime end)
        {
            var list = new List<PlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from c in db.vPlanList
                        //from c in db.tblPlan
                        //    join b in db.tblProject on c.projectId equals b.projectId into group1
                        //    from b in group1.DefaultIfEmpty()
                        //    join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                        //    from d in group2.DefaultIfEmpty()
                        //    join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                        //    from e in group3.DefaultIfEmpty()
                        //    join f in db.tblUser on c.responsibleUser equals f.userId into group4
                        //    from f in group4.DefaultIfEmpty()
                        //    join create in db.tblUser on c.createUser equals create.userId into group5
                        //    from create in group5.DefaultIfEmpty()
                        //    where !c.deleteFlag
                        select new PlanInfo
                        {
                            planId = c.planId,
                            parentPlan = c.parentPlan,
                            executionModeId = c.executionModeId,
                            responsibleOrganization = c.responsibleOrganization,
                            responsibleUser = c.responsibleUser,
                            responsibleUserName = c.responsibleUserName,
                            confirmOrganization = c.confirmOrganization,
                            confirmUser = c.confirmUser,
                            confirmUserName = c.confirmUserName,
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
                            status = c.status,
                            stop = c.stop,
                            createTime = c.createTime,
                            updateTime = c.updateTime,
                            initial = c.initial,
                            withSub = c.withSub,
                            archive = c.archive,
                            archiveTime = c.archiveTime,
                            autoStart = c.autoStart,
                            //eventOutputId = c.eventOutputId,
                            eventOutput = c.eventOutput,
                            createUserName = c.createUserName,
                            responsibleUserImage = (c.responsibleUserImage == null || c.responsibleUserImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.responsibleUserImage),
                            confirmUserImage = (c.confirmUserImage == null || c.confirmUserImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.confirmUserImage),
                            isLoopPlan = 0,
                            IsCollPlan = 0,
                            isFronPlan = c.withFront.Value ? 1 : 0,
                            withFront = c.withFront,
                            realTime = c.quantity == null ? 0 : c.quantity * c.time,
                            effectiveTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,

                            projectId = c.projectId,
                            parentProject = c.parentProject,
                            projectName = c.projectName,
                            proOrderNumber = c.orderNumber,

                            organizationId = c.organizationId,
                            parentOrganization = c.parentOrganization,
                            schemaName = c.schemaName,
                            organizationName = c.organizationName,
                            orgOrderNumber = c.orderNumber,

                            executionId = c.executionId,
                            executionMode = c.executionMode
                        }).Where(condition).Where("updateTime >=@0 And updateTime <@1", start, end).ToList();
                //if (coopCondition != "1")
                //{
                //    //查询协作计划
                //    var collPlanList = (from c in db.vPlanCooperation
                //                        //from c in db.tblPlan
                //                        //                join b in db.tblProject on c.projectId equals b.projectId into group1
                //                        //                from b in group1.DefaultIfEmpty()
                //                        //                join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                //                        //                from d in group2.DefaultIfEmpty()
                //                        //                join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                //                        //                from e in group3.DefaultIfEmpty()
                //                        //                join f in db.tblUser on c.responsibleUser equals f.userId into group4
                //                        //                from f in group4.DefaultIfEmpty()
                //                        //                join g in db.tblPlanCooperation on c.planId equals g.planId into group5
                //                        //                from g in group5.DefaultIfEmpty()
                //                        where c.userId == userId
                //                        select new PlanInfo
                //                        {
                //                            planId = c.planId,
                //                            parentPlan = c.parentPlan,
                //                            executionModeId = c.executionModeId,
                //                            responsibleOrganization = c.responsibleOrganization,
                //                            responsibleUser = c.userId,
                //                            responsibleUserName = c.responsibleUserName,
                //                            smallImage = (c.bigImage == null || c.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + c.bigImage),
                //                            confirmOrganization = c.confirmOrganization,
                //                            confirmUser = c.confirmUser,
                //                            confirmUserName = c.confirmUserName,
                //                            startTime = c.startTime,
                //                            endTime = c.endTime.Value,
                //                            workTime = c.workTime,
                //                            comment = c.comment,
                //                            alert = c.alert,
                //                            importance = c.importance,
                //                            urgency = c.urgency,
                //                            difficulty = c.difficulty,
                //                            progress = c.progress,
                //                            quantity = c.quantity,
                //                            time = c.time,
                //                            completeQuantity = c.completeQuantity,
                //                            completeQuality = c.completeQuality,
                //                            completeTime = c.completeTime,
                //                            status = c.status,
                //                            stop = c.stop,
                //                            initial = c.initial,
                //                            withSub = c.withSub,
                //                            archive = c.archive,
                //                            archiveTime = c.archiveTime,
                //                            autoStart = c.autoStart,
                //                            //eventOutputId = c.eventOutputId,
                //                            eventOutput = c.eventOutput,
                //                            createTime = c.createTime,
                //                            updateTime = c.updateTime,
                //                            isLoopPlan = 0,
                //                            IsCollPlan = 1,
                //                            isFronPlan = c.withFront.Value ? 1 : 0,
                //                            withFront = c.withFront,
                //                            realTime = c.quantity == null ? 0 : c.quantity * c.time,
                //                            effectiveTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,

                //                            projectId = c.projectId,
                //                            parentProject = c.parentProject,
                //                            projectName = c.projectName,
                //                            proOrderNumber = c.orderNumber,

                //                            organizationId = c.organizationId,
                //                            parentOrganization = c.parentOrganization,
                //                            schemaName = c.schemaName,
                //                            organizationName = c.organizationName,
                //                            orgOrderNumber = c.orderNumber,

                //                            executionId = c.executionId,
                //                            executionMode = c.executionMode
                //                        }).Where(coopCondition).Where("updateTime >=@0 And updateTime <@1", start, end).ToList();
                //    if (collPlanList.Count > 0)
                //    {
                //        list.AddRange(collPlanList);
                //    }
                //}

                var stringList = new List<string>();
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        item.endTimeNew = item.endTime == null ? "" : item.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                        item.createTimeNew = item.createTime == null ? "" : item.createTime.ToString("yyyy-MM-dd HH:mm");
                        stringList.Clear();
                        item.organizationNameNew = GetOrgStringByOrgId(db, item.organizationId, stringList);
                        stringList.Clear();
                        item.projectNameNew = GetProListByOrgId(db, item.projectId, stringList);
                    }
                }
            }
            return list;
        }

        #endregion 获取计划日程化普通计划列表(时间筛选字段不同：updateTime)

        #region 获取循环计划列表

        /// <summary>
        /// 获取循环计划列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="condition">筛选条件</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="flag">0:我的日程 1：取下属待审核列表 2:不是取下属待确认列表</param>
        /// <returns>循环计划列表</returns>
        public List<PlanInfo> GetLoopPlanList(int userId, string condition, DateTime start, DateTime end, int flag)
        {
            var planList = new List<PlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //查询循环计划
                if (flag == 2)   //待确认的循环计划列表
                {
                    var loopList = (from c in db.tblLoopPlan
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
                                    join create in db.tblUser on c.createUser equals create.userId into group6
                                    from create in group6.DefaultIfEmpty()
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
                                        confirmUserName = db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault().userName,
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
                                        createUserName = create.userName,
                                        bigImage = (create.bigImage == null || create.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + create.bigImage),
                                        loopStatus = c.loopStatus,
                                        createTime = c.createTime,
                                        updateTime = s.createTime,
                                        loopYear = c.loopYear,
                                        loopMonth = c.loopMonth,
                                        loopWeek = c.loopWeek,
                                        loopTime = c.loopTime,
                                        submitId = s.submitId,
                                        loopType = c.loopType,
                                        unitTime = c.unitTime == null ? 0 : c.unitTime.Value,

                                        projectId = c.projectId,
                                        parentProject = b.parentProject,
                                        projectName = b.projectName,

                                        organizationId = c.organizationId,
                                        parentOrganization = d.parentOrganization,
                                        schemaName = d.schemaName,
                                        organizationName = d.organizationName,

                                        executionId = e.executionId,
                                        executionMode = e.executionMode
                                    }).Where(condition).Where("endTime >=@0", end).ToList();
                    planList.AddRange(loopList);
                }
                else
                {
                    var loopList = (from c in db.tblLoopPlan
                                    join b in db.tblProject on c.projectId equals b.projectId into group1
                                    from b in group1.DefaultIfEmpty()
                                    join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                                    from d in group2.DefaultIfEmpty()
                                    join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                                    from e in group3.DefaultIfEmpty()
                                    join f in db.tblUser on c.responsibleUser equals f.userId into group4
                                    from f in group4.DefaultIfEmpty()
                                    join create in db.tblUser on c.createUser equals create.userId into group5
                                    from create in group5.DefaultIfEmpty()
                                    where !c.deleteFlag && c.loopStatus.Value
                                    select new PlanInfo
                                    {
                                        loopId = c.loopId,
                                        executionModeId = c.executionModeId,
                                        responsibleOrganization = c.responsibleOrganization,
                                        responsibleUser = c.responsibleUser,
                                        responsibleUserName = f.userName,
                                        confirmOrganization = c.confirmOrganization,
                                        confirmUser = c.confirmUser,
                                        confirmUserName = db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault().userName,
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
                                        createUserName = create.userName,
                                        bigImage = (create.bigImage == null || create.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + create.bigImage),
                                        loopStatus = c.loopStatus,
                                        createTime = c.createTime,
                                        updateTime = c.updateTime,
                                        loopYear = c.loopYear,
                                        loopMonth = c.loopMonth,
                                        loopWeek = c.loopWeek,
                                        loopTime = c.loopTime,
                                        loopType = c.loopType,
                                        unitTime = c.unitTime == null ? 0 : c.unitTime.Value,

                                        projectId = c.projectId,
                                        parentProject = b.parentProject,
                                        projectName = b.projectName,

                                        organizationId = c.organizationId,
                                        parentOrganization = d.parentOrganization,
                                        schemaName = d.schemaName,
                                        organizationName = d.organizationName,

                                        executionId = e.executionId,
                                        executionMode = e.executionMode
                                    }).Where(condition).Where("endTime >=@0", end).ToList();

                    if (loopList.Count() > 0 && flag == 0)     //我的日程
                    {
                        var today = start.Date;
                        var tomDay = today.AddDays(1);
                        var dayOfWeek = (int)start.DayOfWeek;
                        var year = start.Year;
                        var month = start.Month;
                        var day = start.Day;
                        loopList.ForEach(p =>
                        {
                            if (p.status == (int)ConstVar.LoopPlanStatus.checkPass)   //审核通过状态
                            {
                                if (p.loopType == (int)ConstVar.LoopPlanType.day)    //日循环
                                {
                                    if (db.tblLoopplanSubmit.Where(a => a.createTime >= today && a.createTime < tomDay).Count() <= 0)
                                    {
                                        planList.Add(p);
                                    }
                                }
                                else if (p.loopType == (int)ConstVar.LoopPlanType.week)  //周循环
                                {
                                    if (Convert.ToInt32(p.loopWeek) == dayOfWeek && db.tblLoopplanSubmit.Where(a => a.createTime >= today && a.createTime < tomDay).Count() <= 0)
                                    {
                                        planList.Add(p);
                                    }
                                }
                                else if (p.loopType == (int)ConstVar.LoopPlanType.month)  //月循环
                                {
                                    if (p.loopMonth == day.ToString().PadLeft(2, '0') && db.tblLoopplanSubmit.Where(a => a.createTime.Year == year && a.createTime.Month == month).Count() <= 0)
                                    {
                                        planList.Add(p);
                                    }
                                }
                                else        //年循环
                                {
                                    if (p.loopYear == month.ToString().PadLeft(2, '0') + day.ToString().PadLeft(2, '0') && db.tblLoopplanSubmit.Where(a => a.createTime.Year == year).Count() <= 0)
                                    {
                                        planList.Add(p);
                                    }
                                }
                            }
                            else       //待提交状态
                            {
                                planList.Add(p);
                            }
                        });
                    }
                    else          //下属日程
                    {
                        if (flag == 1)     //待审核状态的列表
                        {
                            planList.AddRange(loopList);
                        }
                    }
                }

                if (planList.Count() > 0)
                {
                    var stringList = new List<string>();
                    foreach (var item in planList)
                    {
                        item.endTimeNew = item.endTime == null ? "" : item.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                        item.createTimeNew = item.createTime == null ? "" : item.createTime.ToString("yyyy-MM-dd HH:mm");
                        stringList.Clear();
                        item.organizationNameNew = GetOrgStringByOrgId(db, item.organizationId, stringList);
                        stringList.Clear();
                        item.projectNameNew = GetProListByOrgId(db, item.projectId, stringList);
                    }
                }
            }
            return planList;
        }

        #endregion 获取循环计划列表

        #region 查询下属计划

        /// <summary>
        /// 查询下属计划
        /// </summary>
        /// <param name="userId">上司计划Id</param>
        /// <returns></returns>
        public List<PlanInfo> GetUnderPlanList(int userId, string condition)
        {
            var underListPlans = new List<PlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //查询下属职位Id集合
                var underStationIds = from user in db.tblUser
                                      join userStation in db.tblUserStation on user.userId equals userStation.userId
                                      join station in db.tblStation on userStation.stationId equals station.parentStation
                                      where user.userId == userId
                                      select station.stationId;

                if (underStationIds.Count() > 0)
                {
                    //便利查询每个职位Id上的UserId集合
                    foreach (var item in underStationIds)
                    {
                        var userIdList = db.tblUserStation.Where(p => p.stationId == item);
                        //遍历查询每个下属的计划列表
                        foreach (var user in userIdList)
                        {
                            var underList = GetPlanList(user.userId, condition, "", DateTime.MinValue, DateTime.MaxValue);
                            if (underList.Count() > 0)
                            {
                                //遍历往下属计划集合中添数据
                                foreach (var planModel in underList)
                                {
                                    underListPlans.Add(planModel);
                                }
                            }
                        }
                    }
                }
            }
            return underListPlans;
        }

        #endregion 查询下属计划

        #region 自定义排序

        /// <summary>
        /// 自定义排序
        /// </summary>
        /// <param name="orderList">排序的列表集合</param>
        /// <param name="planList">计划列表</param>
        /// <returns>计划列表集合</returns>
        public List<PlanInfo> GetPlanListOrderByCustom(List<Sort> orderList, List<PlanInfo> planList)
        {
            if (planList.Count() > 0)
            {
                foreach (var item in orderList)
                {
                    switch (item.type)
                    {
                        //组织架构排序
                        case 0:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.orgOrderNumber) : planList.OrderByDescending(p => p.orgOrderNumber)).ToList();
                            break;
                        //项目分类排序
                        case 1:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.proOrderNumber) : planList.OrderByDescending(p => p.proOrderNumber)).ToList();
                            break;
                        //重要度排序
                        case 2:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.importance) : planList.OrderByDescending(p => p.importance)).ToList();
                            break;
                        //紧急度排序
                        case 3:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.urgency) : planList.OrderByDescending(p => p.urgency)).ToList();
                            break;
                        //责任人排序
                        case 4:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.responsibleUserName) : planList.OrderByDescending(p => p.responsibleUserName)).ToList();
                            break;
                        //确认人排序
                        case 5:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.confirmUserName) : planList.OrderByDescending(p => p.confirmUserName)).ToList();
                            break;
                        //完成时间排序
                        case 6:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.endTime) : planList.OrderByDescending(p => p.endTime)).ToList();
                            break;
                        //状态值排序
                        case 7:
                            planList = (item.direct == 1 ? planList.OrderBy(p => p.status) : planList.OrderByDescending(p => p.status)).ToList();
                            break;
                        //默认排序
                        case 8:
                            planList = planList.OrderByDescending(p => p.importance).OrderByDescending(p => p.urgency).OrderByDescending(p => p.endTime).ToList();
                            break;
                    }
                }
            }
            return planList;
        }

        #endregion 自定义排序

        #region 获取组织信息 === （获取常用组织信息）

        /// <summary>
        /// 获取组织信息==>递归查询
        /// </summary>
        /// <returns></returns>
        public List<OrganizationInfo> GetOrgListByOrgId(int? OrganizationId, ref List<OrganizationInfo> orgInfo)
        {
            var db = new TargetNavigationDBEntities();
            //OrganizationInfo organizationInfo = new OrganizationInfo();
            var first = (from org in db.tblOrganization
                         where org.organizationId == OrganizationId
                         select new OrganizationInfo
                         {
                             organizationId = org.organizationId,
                             parentOrganization = org.parentOrganization,
                             organizationName = org.organizationName
                         }).FirstOrDefault<OrganizationInfo>();
            orgInfo.Add(first);
            if (first.parentOrganization == 0)
            {
                return orgInfo;
            }
            return GetOrgListByOrgId(first.parentOrganization, ref orgInfo);
        }

        #endregion 获取组织信息 === （获取常用组织信息）

        #region 获取组织信息 === （获取常用组织信息）

        /// <summary>
        /// 获取组织信息==>递归查询
        /// </summary>
        /// <returns></returns>
        ///
        public string GetOrgString(int? OrganizationId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                List<string> stringList = new List<string>();
                return GetOrgStringByOrgId(db, OrganizationId, stringList);
            }
        }

        public string GetOrgStringByOrgId(TargetNavigationDBEntities db, int? OrganizationId, List<string> orgStringList)
        {
            var orgString = string.Empty;
            var first = (from org in db.tblOrganization
                         where org.organizationId == OrganizationId
                         select new OrganizationInfo
                         {
                             organizationId = org.organizationId,
                             organizationName = org.organizationName,
                             parentOrganization = org.parentOrganization
                         }).FirstOrDefault<OrganizationInfo>();
            if (first != null)
            {
                orgStringList.Add(first.organizationName);
                if (first.parentOrganization == null)
                {
                    if (orgStringList.Count() > 0)
                    {
                        orgStringList.Reverse();
                        foreach (var item in orgStringList)
                        {
                            orgString = orgString + item + "-";
                        }
                        orgString = orgString.Substring(0, orgString.LastIndexOf('-'));
                    }
                    return orgString;
                }
                else
                {
                    return GetOrgStringByOrgId(db, first.parentOrganization, orgStringList);
                }
            }
            return orgString;
        }

        public List<string> GetMoreOrgNameById(int? orgId, ref List<OrganizationInfo> OrgNameList, ref int count)
        {
            count++;
            List<string> OrgNameByThree = new List<string>();
            var db = new TargetNavigationDBEntities();
            var Name = (from org in db.tblOrganization
                        where org.organizationId == orgId
                        select new OrganizationInfo
                        {
                            organizationId = org.organizationId,
                            organizationName = org.organizationName
                        }).FirstOrDefault<OrganizationInfo>();
            OrgNameList.Add(Name);
            if (count <= 3)
            {
                return GetMoreOrgNameById(Name.parentOrganization, ref OrgNameList, ref count);
            }
            else
            {
                foreach (OrganizationInfo rtginfo in OrgNameList)
                {
                    OrgNameByThree.Add(rtginfo.organizationName);
                }
                return OrgNameByThree;
            }
        }

        #endregion 获取组织信息 === （获取常用组织信息）

        #region 获取项目信息 === （获取常用项目信息）

        /// <summary>
        /// 获取项目信息==>递归查询
        /// </summary>
        /// <returns></returns>
        ///
        public string GetPorString(int? projectId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                List<string> proList = new List<string>();
                return GetProListByOrgId(db, projectId, proList);
            }
        }

        public string GetProListByOrgId(TargetNavigationDBEntities db, int? projectId, List<string> proList)
        {
            var proString = string.Empty;
            //OrganizationInfo organizationInfo = new OrganizationInfo();
            var first = (from pro in db.tblProject
                         where pro.projectId == projectId
                         select new ProjectInfo
                         {
                             projectId = pro.projectId,
                             projectName = pro.projectName,
                             parentProject = pro.parentProject,
                             withSub = pro.withSub
                         }).FirstOrDefault<ProjectInfo>();
            if (first != null)
            {
                proList.Add(first.projectName);
                if (first.parentProject == null)
                {
                    if (proList.Count() > 0)
                    {
                        proList.Reverse();
                        foreach (var item in proList)
                        {
                            proString = proString + item + "-";
                        }
                        proString = proString.Substring(0, proString.LastIndexOf('-'));
                    }
                    return proString;
                }
                else
                {
                    return GetProListByOrgId(db, first.parentProject, proList);
                }
            }
            return proString;
        }

        #endregion 获取项目信息 === （获取常用项目信息）

        #region 获取最近5条计划-->查询orgnazationId

        /// <summary>
        /// 获取最近5条计划===（根据创建时间排序）
        /// </summary>
        public List<OrganizationInfo> GetOrgId(int userId)
        {
            var orgList = new List<OrganizationInfo>();
            var db = new TargetNavigationDBEntities();
            orgList = (from org in db.tblOrganization
                       join tus in db.tblUserStation on userId equals tus.userId
                       join ts in db.tblStation on tus.stationId equals ts.stationId
                       orderby tus.stationId
                       where tus.userId == userId
                       select new OrganizationInfo
                       {
                           organizationId = org.organizationId
                       }).ToList();
            return orgList;
        }

        #endregion 获取最近5条计划-->查询orgnazationId

        #region

        /// <summary>
        /// 获取所有组织信息-->parentId=0;
        /// </summary>
        /// <returns></returns>
        public List<OrganizationInfo> GetAllOrg()
        {
            var orgList = new List<OrganizationInfo>();
            var db = new TargetNavigationDBEntities();

            orgList = (from org in db.tblOrganization
                       where org.parentOrganization == 0
                       select new OrganizationInfo
                       {
                           organizationName = org.organizationName
                       }).ToList();
            return orgList;
        }

        /// <summary>
        /// 获取组织信息==>递归查询
        /// </summary>
        /// <returns></returns>
        public List<string> GetLastOrgListByOrgId(int? organizationId, List<string> orgList)
        {
            var db = new TargetNavigationDBEntities();
            //OrganizationInfo organizationInfo = new OrganizationInfo();
            var first = (from org in db.tblOrganization
                         where org.parentOrganization == organizationId
                         select new OrganizationInfo
                         {
                             organizationId = org.organizationId,
                             organizationName = org.organizationName
                         }).FirstOrDefault<OrganizationInfo>();
            orgList.Add(first.organizationName);
            if (first.parentOrganization == 0)
            {
                return orgList;
            }
            return GetLastOrgListByOrgId(first.parentOrganization, orgList);
        }

        #endregion

        #region 获取最近5条计划-->查询projectId

        /// <summary>
        /// 获取最近5条计划===（根据创建时间排序）
        /// </summary>
        public List<PlanInfo> GetProId(int userId)
        {
            var proList = new List<PlanInfo>();
            var db = new TargetNavigationDBEntities();
            proList = (from p in db.tblPlan
                       orderby p.createTime descending
                       where p.createUser == userId
                       select new PlanInfo
                       {
                           projectId = p.projectId
                       }).Take(5).ToList();
            return proList;
        }

        /// <summary>
        /// 获取所有项目信息-->第一层
        /// </summary>
        /// <returns></returns>
        public List<ProjectInfo> GetAllPro()
        {
            var proList = new List<ProjectInfo>();
            var db = new TargetNavigationDBEntities();

            proList = (from pro in db.tblProject
                       where pro.parentProject == 0
                       select new ProjectInfo
                       {
                           projectName = pro.projectName
                       }).ToList();
            return proList;
        }

        /// <summary>
        /// 获取组织信息==>递归查询
        /// </summary> 获取项目信息 === 下一层
        /// <returns></returns>
        public List<string> GetLastProListByProId(int? projectId, List<string> proList)
        {
            var db = new TargetNavigationDBEntities();
            //OrganizationInfo organizationInfo = new OrganizationInfo();
            var first = (from pro in db.tblProject
                         where pro.parentProject == projectId
                         select new ProjectInfo
                         {
                             projectId = pro.projectId,
                             projectName = pro.projectName
                         }).FirstOrDefault<ProjectInfo>();
            proList.Add(first.projectName);
            if (first.parentProject == 0)
            {
                return proList;
            }
            return GetLastProListByProId(first.parentProject, proList);
        }

        #endregion

        #region  新建计划

        /// <summary>
        /// 新建计划
        /// </summary>
        /// <param name="newPlan">数据模型</param>
        /// <param name="userid">用户ID</param>
        public Dictionary<int, int> Save(List<NewPlan> newPlan, int userid, int status)
        {
            var planResultInfo = new Dictionary<int, int>();
            using (var db = new TargetNavigationDBEntities())
            {
                CommonWorkTime commonWORK = new CommonWorkTime();
                int i = 0;
                int stat = status;
                foreach (var plan in newPlan)
                {
                    if (plan.isTmp == 0 && status == 10)
                    {
                        status = 20;
                        plan.import = 1;
                        plan.go = 1;
                        plan.difficulty = 1;

                    }
                    else
                    {
                        plan.import = 0;
                        plan.go = 0;
                        plan.difficulty = 0;
                    }
                    if (plan.roundType == 0)
                    {
                        var planInfo = new PlanInfo
                        {
                            planId = commonWORK.GetPlanidByTblName("tblPlan"),
                            parentPlan = plan.planId,
                            organizationId = plan.department,
                            projectId = plan.project,
                            executionModeId = plan.runMode,
                            eventOutput = plan.output,
                            startTime = plan.roundTime,
                            endTime = plan.endTime,
                            responsibleUser = plan.responsibleUser,
                            confirmUser = plan.confirmUser,
                            meetingId = plan.meetingId,
                            initial = plan.isTmp,
                            status = status,
                            workTime = plan.workTime,
                            createUser = userid,
                            updateUser = userid,
                            createTime = DateTime.Now,
                            updateTime = DateTime.Now,
                            progress = 0,
                            stop = 0,
                            importance = plan.import,
                            urgency = plan.go,
                            difficulty = plan.difficulty,
                            withSub = false,
                            withFront = false,
                            deleteFlag = false,
                            //标签
                            keyword = plan.keyword
                        };
                        //planResultInfo.Add(plan.roundType.Value, planInfo.planId);
                        planResultInfo.Add(planInfo.planId, plan.roundType.Value);
                        i = planInfo.planId;
                        //保存计划信息
                        AddPlan(userid, planInfo, db);
                        //子计划
                        if (plan.children != null)
                        {
                            tblPlan tPlan = db.tblPlan.Where(p => p.planId == i).FirstOrDefault<tblPlan>();
                            tPlan.withSub = true;
                            AddChildren(plan.children, userid, i, stat, db);
                        }
                    }
                    else
                    {
                        var loopPlanInfo = new LoopPlanInfo
                        {
                            loopId = commonWORK.GetPlanidByTblName("tblLoopPlan"),
                            organizationId = plan.department,
                            projectId = plan.project,
                            executionModeId = plan.runMode,
                            eventOutput = plan.output,
                            loopType = plan.roundType,
                            startTime = plan.roundTime,
                            endTime = plan.endTime,
                            responsibleUser = plan.responsibleUser,
                            confirmUser = plan.confirmUser,
                            status = status,
                            unitTime = plan.workTime,
                            createUser = userid,
                            updateUser = userid,
                            createTime = DateTime.Now,
                            updateTime = DateTime.Now,
                            importance = plan.import,
                            urgency = plan.go,
                            difficulty = plan.difficulty,
                            loopStatus = false,
                            deleteFlag = false,
                            //标签
                            keyword = plan.keyword
                        };
                        planResultInfo.Add(plan.roundType.Value, loopPlanInfo.loopId.Value);
                        i = Convert.ToInt32(loopPlanInfo.loopId);
                        //保存循环计划信息
                        AddLoopPlan(userid, loopPlanInfo, db);
                    }
                    //计划前提表
                    if (plan.premise.Length > 0)
                    {
                        tblPlan tPlan = db.tblPlan.Where(p => p.planId == i).FirstOrDefault<tblPlan>();
                        tPlan.withFront = true;
                        foreach (var p in plan.premise)
                        {
                            AddPlanFront(i, p, userid, db);
                        }
                    }
                    //计划协作表
                    foreach (var p in plan.partner)
                    {
                        AddPlanCooperation(i, p, userid, db);
                    }
                }
                db.SaveChanges();
            }

            return planResultInfo;
        }

        #endregion

        #region  新建子计划

        /// <summary>
        /// 新建子计划
        /// </summary>
        /// <param name="newPlan">数据模型</param>
        /// <param name="userid">用户ID</param>
        /// <param name="parentPlanId">父项目ID</param>
        public void AddChildren(List<NewPlan> newPlan, int userid, int parentPlanId, int status, TargetNavigationDBEntities db)
        {
            CommonWorkTime commonWORK = new CommonWorkTime();
            int i = 0;
            foreach (var plan in newPlan)
            {
                if (plan.isTmp == 0 && status == 10)
                {
                    status = 20;
                    plan.import = 1;
                    plan.go = 1;
                    plan.difficulty = 1;
                }
                else if (plan.isTmp == 1 && status == 10)
                {
                    status = 10;
                    plan.import = 1;
                    plan.go = 1;
                    plan.difficulty = 1;
                }
                else
                {
                    plan.import = 0;
                    plan.go = 0;
                    plan.difficulty = 0;
                }
                if (plan.roundType == 0)
                {
                    var planInfo = new PlanInfo
                    {
                        planId = commonWORK.GetPlanidByTblName("tblPlan"),
                        parentPlan = parentPlanId,
                        organizationId = plan.department,
                        projectId = plan.project,
                        executionModeId = plan.runMode,
                        eventOutput = plan.output,
                        startTime = plan.roundTime,
                        endTime = plan.endTime,
                        responsibleUser = plan.responsibleUser,
                        confirmUser = plan.confirmUser,
                        initial = plan.isTmp,
                        status = status,
                        workTime = plan.workTime,
                        meetingId = plan.meetingId,
                        createUser = userid,
                        updateUser = userid,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        importance = plan.import,
                        urgency = plan.go,
                        difficulty = plan.difficulty,
                        progress = 0,
                        withSub = false,
                        withFront = false,
                        stop = 0
                    };
                    i = planInfo.planId;
                    AddPlan(userid, planInfo, db);
                    // PlanOperate.AddPlanOperate(planInfo, Operate.新建计划, db);
                    //子计划
                    if (plan.children != null)
                    {
                        tblPlan tPlan = db.tblPlan.Where(p => p.planId == i).FirstOrDefault<tblPlan>();
                        tPlan.withSub = true;
                        AddChildren(plan.children, userid, i, status, db);
                    }
                }
                else
                {
                    var loopPlanInfo = new LoopPlanInfo
                    {
                        loopId = commonWORK.GetPlanidByTblName("tblLoopPlan"),
                        organizationId = plan.department,
                        projectId = plan.project,
                        executionModeId = plan.runMode,
                        eventOutput = plan.output,
                        loopType = plan.roundType,
                        startTime = plan.roundTime,
                        endTime = plan.endTime,
                        responsibleUser = plan.responsibleUser,
                        confirmUser = plan.confirmUser,
                        status = status,
                        //time = plan.workTime,
                        createUser = userid,
                        updateUser = userid,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        importance = plan.import,
                        urgency = plan.go,
                        difficulty = plan.difficulty
                    };
                    i = Convert.ToInt32(loopPlanInfo.loopId);
                    AddLoopPlan(userid, loopPlanInfo, db);
                    PlanOperate.AddLoopPlanOperate(loopPlanInfo, Operate.新建循环计划, db);
                }
                #region---------------前提，协作，子计划
                ////计划前提表
                if (plan.premise.Length > 0)
                {
                    tblPlan tPlan = db.tblPlan.Where(p => p.planId == i).FirstOrDefault<tblPlan>();
                    tPlan.withFront = true;
                    foreach (var p in plan.premise)
                    {
                        AddPlanFront(i, p, userid, db);
                    }
                }
                ////计划协作表
                foreach (var p in plan.partner)
                {
                    AddPlanCooperation(i, p, userid, db);
                }
                #endregion
            }
        }

        #endregion

        #region 添加一条计划信息

        /// <summary>
        /// 添加一条计划信息
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <returns>受影响行数</returns>
        public void AddPlan(int userId, PlanInfo planInfo, TargetNavigationDBEntities db)
        {
            //计划信息表新增
            var plan = new tblPlan
            {
                planId = planInfo.planId,
                parentPlan = planInfo.parentPlan,
                projectId = planInfo.projectId,
                organizationId = planInfo.organizationId,
                executionModeId = planInfo.executionModeId,
                //eventOutputId = planInfo.eventOutputId,
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
                status = planInfo.status,
                stop = planInfo.stop,
                initial = planInfo.initial,
                withSub = planInfo.withSub,
                archive = planInfo.archive,
                archiveTime = planInfo.archiveTime,
                autoStart = planInfo.autoStart,
                createUser = planInfo.createUser,
                createTime = planInfo.createTime,
                updateUser = planInfo.updateUser,
                updateTime = planInfo.updateTime,
                //标签
                keyword = planInfo.keyword != null && planInfo.keyword.Length > 0 ? string.Join(",", planInfo.keyword) : null
            };
            if (plan.status == 10)
            {
                plan.planGenerateTime = DateTime.Now;
            }
            else if (plan.status == 20)
            {
                plan.planGenerateTime = DateTime.Now;
                plan.auditTime = DateTime.Now;
            }
            db.tblPlan.Add(plan);
            //操作日志
            this.AddPlanOperate(planInfo.planId, planInfo.eventOutput, (int)ConstVar.PlanOperateStatus.createPlan, userId, db);
        }

        #endregion

        #region 添加一条计划前提信息

        /// <summary>
        /// 添加计划前提表
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="planFrontId"></param>
        /// <param name="userid"></param>
        /// <param name="db"></param>
        public void AddPlanFront(int planId, int planFrontId, int userid, TargetNavigationDBEntities db)
        {
            var planFront = new tblPlanFront
            {
                planId = planId,
                planFront = planFrontId,
                //createUser = userid,
                //updateUser = userid,
                //updateTime = DateTime.Now
            };
            db.tblPlanFront.Add(planFront);
        }

        #endregion

        #region 添加一条计划协作信息

        /// <summary>
        /// 添加计划协作表
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="partnerId"></param>
        /// <param name="userid"></param>
        /// <param name="db"></param>
        public void AddPlanCooperation(int planId, int partnerId, int userid, TargetNavigationDBEntities db)
        {
            var planCooperation = new tblPlanCooperation
            {
                planId = planId,
                userId = partnerId
            };
            db.tblPlanCooperation.Add(planCooperation);
        }

        #endregion
        #region

        /// <summary>
        /// 根据计划删除协作计划
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <param name="userId">协作人ID</param>
        /// <param name="db"></param>
        public void DeletePlanCooperation(int planId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var planCooperation = db.tblPlanCooperation.Where(p => p.planId == planId && p.userId == userId).FirstOrDefault<tblPlanCooperation>();
                if (planCooperation != null)
                {
                    db.tblPlanCooperation.Remove(planCooperation);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region 添加一条循环计划

        /// <summary>
        /// 添加一条循环计划
        /// </summary>
        /// <param name=loopPlanInfo></param>
        public void AddLoopPlan(int userId, LoopPlanInfo loopPlanInfo, TargetNavigationDBEntities db)
        {
            var loopPlan = new tblLoopPlan
            {
                loopId = Convert.ToInt32(loopPlanInfo.loopId),
                projectId = loopPlanInfo.projectId,
                organizationId = loopPlanInfo.organizationId,
                executionModeId = loopPlanInfo.executionModeId,
                eventOutputId = loopPlanInfo.eventOutputId,
                eventOutput = loopPlanInfo.eventOutput,
                responsibleOrganization = loopPlanInfo.responsibleOrganization,
                responsibleUser = loopPlanInfo.responsibleUser,
                confirmOrganization = loopPlanInfo.confirmOrganization,
                confirmUser = loopPlanInfo.confirmUser,
                startTime = loopPlanInfo.startTime,
                endTime = loopPlanInfo.endTime,
                comment = loopPlanInfo.comment,
                status = loopPlanInfo.status,
                loopType = loopPlanInfo.loopType,
                loopStatus = loopPlanInfo.loopStatus,
                importance = loopPlanInfo.importance,
                urgency = loopPlanInfo.urgency,
                difficulty = loopPlanInfo.difficulty,
                //unitTime = loopPlanInfo.time,
                //completeQuantity = loopPlanInfo.completeQuantity,
                //completeQuality = loopPlanInfo.completeQuality,
                //completeTime = loopPlanInfo.completeTime,
                //lastDate = loopPlanInfo.lastDate,
                createUser = loopPlanInfo.createUser,
                createTime = loopPlanInfo.createTime,
                updateUser = loopPlanInfo.updateUser,
                updateTime = loopPlanInfo.updateTime,
                deleteFlag = loopPlanInfo.deleteFlag,
                //标签
                keyword = loopPlanInfo.keyword != null && loopPlanInfo.keyword.Length > 0 ? string.Join(",", loopPlanInfo.keyword) : null
            };
            db.tblLoopPlan.Add(loopPlan);
            //添加日志
            this.AddLoopPlanOperate(db, loopPlan.loopId, loopPlan.eventOutput, (int)ConstVar.LoopPlanOperateStatus.create, userId);
        }

        #endregion

        #region 根据Id查询计划信息通用方法
        /// <summary>
        /// 根据Id查询计划信息通用方法
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="db">连接数据库上下文</param>
        /// <returns>实体对象</returns>
        public tblPlan GetPlanInfoById(int planId, TargetNavigationDBEntities db)
        {
            return db.tblPlan.Where(a => a.planId == planId).FirstOrDefault<tblPlan>();
        }

        /// <summary>
        /// 根据计划Id返回确认人
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public int GetPlanById(int planId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                return db.tblPlan.Where(a => a.planId == planId).FirstOrDefault<tblPlan>().confirmUser.Value;
            }
        }

        #endregion

        #region 修改状态的操作:撤销，修改

        /// <summary>
        /// 修改状态的操作:撤销，修改
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="status">撤销修改后的计划状态</param>
        /// <param name="userId">用户Id</param>
        /// <param name="flag">1、修改 0、撤销</param>
        public int ChangePlanStatus(int planId, int status, int userId, int flag)
        {
            var confirmUser = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                int result;
                var firstData = GetPlanInfoById(planId, db);
                if (firstData == null) return confirmUser;
                confirmUser = firstData.confirmUser.Value;
                //修改状态
                if (status == 1)
                {
                    firstData.stop = 0;
                }
                else
                {
                    firstData.status = status;
                }
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                var opera = new PlanOperateInfo();
                if (flag == 0)
                {
                    result = (int)ConstVar.PlanOperateStatus.cancelCheck;
                }
                else
                {
                    firstData.auditTime = null;   //审核时间清空
                    result = (int)ConstVar.PlanOperateStatus.update;
                }
                //添加日志
                this.AddPlanOperate(planId, string.Empty, result, userId, db);
                db.SaveChanges();
            }
            return confirmUser;
        }

        #endregion

        #region 循环计划申请修改

        /// <summary>
        /// 循环计划申请修改
        /// </summary>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="userId">用户Id</param>
        public bool UpdateLoopPlan(int loopId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = this.GetLoopPlanInfo(db, loopId);
                if (firstData == null) return false;
                //修改状态
                firstData.status = (int)ConstVar.LoopPlanStatus.update;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                //添加日志
                this.AddLoopPlanOperate(db, loopId, string.Empty, (int)ConstVar.LoopPlanOperateStatus.update, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region 提交操作

        /// <summary>
        /// 提交操作
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="status">提交成功后的状态</param>
        public int SubmitPlan(int userId, int planId, int initial)
        {
            var confirmUser = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = GetPlanInfoById(planId, db);
                if (firstData == null) return confirmUser = 0;
                if (initial == 0)
                {
                    firstData.status = 20;
                    firstData.importance = 1;
                    firstData.urgency = 1;
                    firstData.difficulty = 1;
                    firstData.auditTime = DateTime.Now;
                }
                else
                {
                    firstData.status = 10;
                    firstData.auditTime = null;
                }
                confirmUser = firstData.confirmUser.Value;
                firstData.planGenerateTime = DateTime.Now;
                firstData.progress = 0;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                //添加日志
                this.AddPlanOperate(planId, string.Empty, (int)ConstVar.PlanOperateStatus.submit, userId, db);
                db.SaveChanges();
            }
            return confirmUser;
        }

        #endregion

        #region 提交循环计划

        /// <summary>
        /// 提交循环计划
        /// </summary>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="userId">操作用户Id</param>
        public bool SubmitLoopPlan(int loopId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = this.GetLoopPlanInfo(db, loopId);
                if (firstData == null) return false;
                firstData.status = (int)ConstVar.LoopPlanStatus.unCheck;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                //添加日志
                this.AddLoopPlanOperate(db, loopId, string.Empty, (int)ConstVar.LoopPlanOperateStatus.submitPlan, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region  计划提交确认（更改状态为“待确认”，插入操作日志，未提交附件和提交失败返回false）

        /// <summary>
        /// 计划提交确认
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="planId">计划ID</param>
        /// <param name="quantity">完成数量(自评）</param>
        /// <param name="time">完成时间(自评)</param>
        /// <returns></returns>
        public ReturnConfirm Confirming(int userId, int planId, int quantity/*完成数量(自评)*/, int time /*完成时间(自评)*/)
        {
            var planConfirm = new ReturnConfirm();
            using (var db = new TargetNavigationDBEntities())
            {
                tblPlanAttachment planAttach = db.tblPlanAttachment.Where(a => a.planId == planId).FirstOrDefault<tblPlanAttachment>();
                if (planAttach != null)
                {
                    tblPlan plan = GetPlanInfoById(planId, db);
                    if (plan != null)
                    {
                        planConfirm.confirmUser = plan.confirmUser;
                        plan.status = Convert.ToInt32(PlanStatus.Confirming);
                        plan.quantity = quantity;
                        plan.time = time;
                        plan.updateUser = userId;
                        plan.updateTime = DateTime.Now;
                        plan.progress = 90;
                        plan.submitTime = DateTime.Now;
                        plan.confirmTime = null;
                        //添加日志
                        this.AddPlanOperate(planId, "提交确认", (int)ConstVar.PlanOperateStatus.submit, userId, db);
                        db.SaveChanges();
                        planConfirm.result = 0;
                        return planConfirm;//成功
                    }
                    else
                    {
                        planConfirm.result = 1;
                        return planConfirm;//查无此计划
                    }
                }
                else
                {
                    planConfirm.result = 2;
                    return planConfirm;//请上传附件
                }
            }
        }

        #endregion

        #region 循环计划提交确认

        /// <summary>
        /// 循环计划提交确认
        /// </summary>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="quantity">循环计划提交数量</param>
        /// <param name="userId">操作用户Id</param>
        /// <returns></returns>
        public void ConfirmingLoopPlan(int loopId, int quantity, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var submitObj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var submitId = db.prcGetPrimaryKey("tblLoopplanSubmit", submitObj).FirstOrDefault().Value;
                var loopSubmitModel = new tblLoopplanSubmit
                {
                    submitId = submitId,
                    loopId = loopId,
                    number = quantity,
                    submitTime = DateTime.Now,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now
                };
                db.tblLoopplanSubmit.Add(loopSubmitModel);

                //添加日志
                this.AddLoopPlanOperate(db, loopId, string.Empty, (int)Operate.提交, userId);
                db.SaveChanges();
            }
        }

        #endregion

        #region 计划转办

        /// <summary>
        /// 计划转办
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="responseUser">转办后的责任人</param>
        /// <param name="confirmUser">转办后的确认人</param>
        public bool ChangePlan(int planId, int userId, int responseUser, int confirmUser)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = GetPlanInfoById(planId, db);
                if (firstData == null) return false;
                //修改责任人和确认人
                firstData.responsibleUser = responseUser;
                firstData.confirmUser = confirmUser;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                //添加日志
                this.AddPlanOperate(planId, string.Empty, (int)ConstVar.PlanOperateStatus.change, userId, db);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region 审核操作
        /// <summary>
        /// 审核操作
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="importance">重要度</param>
        /// <param name="urgency">紧急度</param>
        /// <param name="difficulty">困难度</param>
        /// <param name="isTure">是否通过</param>
        /// <param name="userId">用户Id</param>
        /// <param name="msg">审批意见</param>
        /// <returns>操作成功与否的标志</returns>
        public int ExaminePlan(int planId, int? importance, int? urgency, int? difficulty, bool isTure, int userId, string msg)
        {
            var returnUser = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                int result;
                var firstData = GetPlanInfoById(planId, db);
                if (firstData == null)
                {
                    return returnUser;
                }
                returnUser = firstData.responsibleUser.Value;
                firstData.updateTime = DateTime.Now;
                firstData.auditTime = DateTime.Now;
                firstData.updateUser = userId;
                if (isTure)
                {
                    result = (int)ConstVar.PlanOperateStatus.checkPass;
                    //待审核->通过
                    if (firstData.status == 10 && firstData.stop == 0)
                    {
                        firstData.importance = importance;
                        firstData.urgency = urgency;
                        firstData.difficulty = difficulty;
                        firstData.status = 20;
                        db.SaveChanges();
                    }
                    //申请修改->通过
                    else if (firstData.status == 25 && firstData.stop == 0)
                    {
                        firstData.status = 0;
                        firstData.importance = 0;
                        firstData.urgency = 0;
                        firstData.difficulty = 0;
                        firstData.progress = 0;
                    }
                    //申请中止->审核通过
                    else if (firstData.stop == 10)
                    {
                        firstData.stop = 90;
                    }
                }
                else
                {
                    result = (int)ConstVar.PlanOperateStatus.checkNoPass;
                    //申请中止->审核未通过
                    if (firstData.stop == 10)
                    {
                        firstData.stop = 0;
                    }
                    //申请修改->未通过
                    else if (firstData.status == 25 && firstData.stop == 0)
                    {
                        firstData.status = 20;
                    }
                    //待审核->未通过
                    else if (firstData.status == 10 && firstData.stop == 0)
                    {
                        firstData.status = 15;
                    }
                }
                //添加日志
                this.AddPlanOperate(planId, msg, result, userId, db);
                db.SaveChanges();
            }
            return returnUser;
        }

        #endregion

        #region 循环计划审批

        /// <summary>
        /// 循环计划审批
        /// </summary>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="importance">重要度</param>
        /// <param name="urgency">紧急度</param>
        /// <param name="difficulty">困难度</param>
        /// <param name="message">审批意见</param>
        /// <param name="type">true:审核通过  false：审核不通过</param>
        /// <param name="userId">用户Id</param>
        /// <returns>true：操作成功 false：操作失败</returns>
        public bool ExamineLoopPlan(int loopId, int? importance, int? urgency, int? difficulty, string message, bool type, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var loopPlan = db.tblLoopPlan.Where(p => p.loopId == loopId).FirstOrDefault();
                if (loopPlan == null) return false;
                var result = (int)ConstVar.LoopPlanOperateStatus.checkPass;
                if (type)  //审核通过
                {
                    //更新循环计划信息表
                    loopPlan.importance = importance;
                    loopPlan.urgency = urgency;
                    loopPlan.difficulty = difficulty;
                    loopPlan.status = (int)ConstVar.LoopPlanStatus.checkPass;
                    loopPlan.loopStatus = true;
                    loopPlan.updateUser = userId;
                    loopPlan.updateTime = DateTime.Now;
                }
                else      //审核不通过
                {
                    loopPlan.status = (int)ConstVar.LoopPlanStatus.unSubmit;
                    loopPlan.updateUser = userId;
                    loopPlan.updateTime = DateTime.Now;
                    result = (int)ConstVar.LoopPlanOperateStatus.checkNoPass;
                }
                //操作表中插入数据
                this.AddLoopPlanOperate(db, loopId, message, result, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region 计划确认

        /// <summary>
        /// 计划确认
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="useId">用户Id</param>
        /// <param name="planAttachments">附件集合</param>
        /// <param name="completeQuantity">完成数量</param>
        /// <param name="completeQuality">完成质量</param>
        ///<param name="completeQuality">确认后的状态</param>
        /// <param name="status">完成时间</param>
        public void ConfirmPlan(int planId, int useId, decimal? completeQuantity, decimal? completeQuality, decimal? completeTime, bool isTrue, string msg)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                int result;
                var firstData = GetPlanInfoById(planId, db);
                if (firstData != null)
                {
                    firstData.updateUser = useId;
                    firstData.updateTime = DateTime.Now;
                    firstData.confirmTime = DateTime.Now;
                    if (isTrue)   //确认通过
                    {
                        firstData.completeQuantity = completeQuantity;
                        firstData.completeQuality = completeQuality;
                        firstData.completeTime = completeTime;
                        firstData.progress = 100;
                        firstData.status = 90;
                        result = (int)ConstVar.PlanOperateStatus.confirmPass;
                    }
                    else   //确认不通过
                    {
                        firstData.status = 40;
                        result = (int)ConstVar.PlanOperateStatus.confirmNoPass;
                        //List<planAttachment> planAttacList = PlanFileData(planId);
                        /* 确认不通过的时候删除附件，2015-5-11日注释
                        FileUpload file = new FileUpload();
                        foreach (var item in planAttacList)
                        {
                            file.DeletePlanAttachmentById(item.attachmentId, UploadFilePath.Plan, db);
                        }
                         */
                    }
                    //添加日志
                    this.AddPlanOperate(planId, msg, result, useId, db);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region 循环计划确认

        /// <summary>
        /// 循环计划确认
        /// </summary>
        /// <param name="submitId">循环计划提交Id</param>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="completeQuantity">完成数量</param>
        /// <param name="completeQuality">完成质量</param>
        /// <param name="completeTime">完成时间</param>
        /// <param name="message">操作意见</param>
        /// <param name="type">true：确认通过 false：确认不通过</param>
        /// <param name="userId">用户Id</param>
        /// <returns>true：操作成功 false：操作失败</returns>
        public bool ConfirmLoopPlan(int submitId, int loopId, decimal? completeQuantity, decimal? completeQuality, decimal? completeTime, string message, bool type, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var loopSubmitPlan = db.tblLoopplanSubmit.Where(p => p.submitId == submitId).FirstOrDefault();
                if (loopSubmitPlan == null) return false;
                var result = (int)ConstVar.LoopPlanOperateStatus.confirmPass;
                if (type)   //确认通过
                {
                    loopSubmitPlan.completeQuantity = completeQuantity;
                    loopSubmitPlan.completeQuality = completeQuality;
                    loopSubmitPlan.completeTime = completeTime;
                    loopSubmitPlan.confirmTime = DateTime.Now;
                    loopSubmitPlan.updateUser = userId;
                    loopSubmitPlan.updateTime = DateTime.Now;
                }
                else      //确认不通过
                {
                    db.tblLoopplanSubmit.Remove(loopSubmitPlan);
                    result = (int)ConstVar.LoopPlanOperateStatus.confirmNoPass;
                }
                //插入操作日志
                this.AddLoopPlanOperate(db, loopId, message, result, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region  添加附件

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="planAttachments">附件集合</param>
        /// <param name="planIdNew">计划Id</param>
        /// <param name="useId">用户Id</param>
        public void AddPlanAttachment(List<string> planAttachments, int planIdNew, int useId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var operId = db.prcGetPrimaryKey("tblPlanOperate", obj).FirstOrDefault().Value;
                if (planAttachments.Count() > 0)
                {
                    foreach (var item in planAttachments)
                    {
                        var ext = System.IO.Path.GetExtension(item);
                        var saveName = DateTime.Now.Ticks.ToString();
                        var displayName = System.IO.Path.GetFileNameWithoutExtension(item);
                        var attachment = new tblPlanAttachment
                        {
                            attachmentId = operId,
                            planId = planIdNew,
                            displayName = displayName,
                            saveName = saveName,
                            extension = ext,
                            createUser = useId,
                            createTime = DateTime.Now,
                            updateUser = useId,
                            updateTime = DateTime.Now,
                            deleteFlag = false
                        };
                        db.tblPlanAttachment.Add(attachment);
                        db.SaveChanges();
                    }
                }
            }
        }

        #endregion

        #region 即时更新计划进程

        /// <summary>
        /// 即时更新计划进程
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="planId">计划Id</param>
        /// <param name="newProcess">进程</param>
        public void UpdateProcess(int userId, int planId, int newProcess)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = GetPlanInfoById(planId, db);
                firstData.progress = newProcess;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                //添加日志
                this.AddPlanOperate(planId, string.Empty, (int)ConstVar.PlanOperateStatus.updateProcess, userId, db);
                db.SaveChanges();
            }
        }

        #endregion

        #region 计划分解

        /// <summary>
        /// 计划分解
        /// </summary>
        /// <param name="parentPlanId">父级计划Id</param>
        public void UpdateParentPlan(int parentPlanId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var parentPirstData = GetPlanInfoById(parentPlanId, db);
                parentPirstData.withSub = true;
                parentPirstData.updateUser = userId;
                parentPirstData.updateTime = DateTime.Now;
                db.SaveChanges();
            }
        }

        #endregion

        #region 中止计划或者开始计划:0、运行中   90、已中止

        /// <summary>
        /// 中止计划或者开始计划:0、运行中   90、已中止
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="stop">修改后的stop状态值</param>
        /// <param name="userId">用户Id</param>
        /// <returns>返回确认人Id</returns>
        public int StopOrStartPlan(int planId, int stop, int userId)
        {
            var confirmUser = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                int result;
                var firstData = GetPlanInfoById(planId, db);
                if (firstData == null) return confirmUser;
                confirmUser = firstData.confirmUser.Value;
                firstData.stop = stop;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                var opera = new PlanOperateInfo();
                if (stop == 0)
                {
                    result = (int)ConstVar.PlanOperateStatus.reStart;
                }
                else
                {
                    result = (int)ConstVar.PlanOperateStatus.stop;
                }
                //添加日志
                this.AddPlanOperate(planId, string.Empty, result, userId, db);
                db.SaveChanges();
            }
            return confirmUser;
        }

        #endregion

        #region 中止循环计划

        /// <summary>
        /// 中止循环计划
        /// </summary>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public bool StopLoopPlan(int loopId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = this.GetLoopPlanInfo(db, loopId);
                if (firstData == null) return false;
                firstData.loopStatus = false;
                firstData.updateUser = userId;
                firstData.updateTime = DateTime.Now;
                //添加循环计划日志
                this.AddLoopPlanOperate(db, loopId, string.Empty, (int)ConstVar.LoopPlanOperateStatus.stop, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region 筛选计划

        /// <summary>
        /// 筛选计划
        /// </summary>
        /// <param name="status">状态集合</param>
        /// <param name="startTime">计划开始时间</param>
        /// <param name="endTime">计划结束时间</param>
        /// <param name="userIds">确认人和责任人Id集合</param>
        /// <param name="organizationIds">部门Id集合</param>
        /// <param name="projectIds">项目Id集合</param>
        /// <param name="planList">计划列表</param>
        ///  <returns>筛选后的计划列表</returns>
        public List<PlanInfo> FilterPlan(int[] status, DateTime startTime, DateTime endTime, int[] userIds, int[] organizationIds, int[] projectIds, List<PlanInfo> planList)
        {
            //根据筛选条件对list进行筛选条件并列的进行或判断，不并列的进行与判断
            planList = planList.Where(p => (status.Length <= 0 ? true : (p.status == status[0] || p.status == status[1] || p.status == status[2]
                || p.status == status[3] || p.status == status[4] || p.status == status[5] || p.status == status[6] || p.status == status[7]
                || p.status == status[8] || p.status == status[9] || p.status == status[10] || p.status == status[11] || p.stop == status[12]))
                 && (startTime == DateTime.Now.AddDays(1).Date ? true : (p.createTime > startTime && p.createTime < endTime))
            && (userIds.Length <= 0 ? true : (p.confirmUser == userIds[0] || p.responsibleUser == userIds[0] || p.confirmUser == userIds[1]
            || p.responsibleUser == userIds[1] || p.confirmUser == userIds[2] || p.responsibleUser == userIds[2] || p.confirmUser == userIds[3]
            || p.responsibleUser == userIds[3] || p.confirmUser == userIds[4] || p.responsibleUser == userIds[4] || p.confirmUser == userIds[5]
            || p.responsibleUser == userIds[5] || p.confirmUser == userIds[6] || p.responsibleUser == userIds[6] || p.confirmUser == userIds[7]
            || p.responsibleUser == userIds[7] || p.confirmUser == userIds[8] || p.responsibleUser == userIds[8] || p.confirmUser == userIds[9]
            || p.responsibleUser == userIds[9]))
            && (organizationIds.Length <= 0 ? true : (p.organizationId == organizationIds[0] || p.organizationId == organizationIds[1]
            || p.organizationId == organizationIds[2] || p.organizationId == organizationIds[3] || p.organizationId == organizationIds[4]
            || p.organizationId == organizationIds[5]))
            && (projectIds.Length <= 0 ? true : (p.projectId == projectIds[0] || p.projectId == projectIds[1] || p.projectId == projectIds[2]
            || p.projectId == projectIds[3] || p.projectId == projectIds[4] || p.projectId == projectIds[5]))).ToList();
            return planList;
        }

        #endregion

        #region 根据时间查询计划状态数量

        /// <summary>
        /// 根据时间查询计划状态数量
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="userId">用户Id</param>
        /// <param name="operate">操作Id</param>
        /// <returns></returns>
        public List<PlanStatusCount> GetPlanStatusInfo(int year, int month, int userId, int operate)
        {
            var planCountList = new List<PlanStatusCount>();
            int count = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                //先查出计划状态和状态名的列表
                var statusList = new List<PlanStatusCount>();
                if (operate != 10)
                {
                    statusList = (from c in db.tblPlan
                                  where c.stop == 0 && c.endTime.Value.Year == year && c.endTime.Value.Month == month && c.responsibleUser == userId && !c.deleteFlag
                                  select new PlanStatusCount
                                  {
                                      status = c.status,
                                      statusName = (c.status == 0 || c.status == 15) ? Uncommitted : ((c.status == 10 || c.status == 25) ? WaitSubmit : ((c.status == 20 || c.status == 40) ? HasSubmit : (c.status == 30 ? WaitConfirm : HasComplete)))
                                  }).ToList();
                }
                else
                {
                    statusList = (from c in db.tblPlan
                                  where c.stop == 0 && c.endTime.Value.Year == year && c.endTime.Value.Month == month && !c.deleteFlag
                                  select new PlanStatusCount
                                  {
                                      status = c.status,
                                      statusName = (c.status == 0 || c.status == 15) ? Uncommitted : ((c.status == 10 || c.status == 25) ? WaitSubmit : ((c.status == 20 || c.status == 40) ? HasSubmit : (c.status == 30 ? WaitConfirm : HasComplete)))
                                  }).ToList();
                }

                if (statusList.Count() > 0)
                {
                    //然后根据相同的状态名分组，分组后是集合套集合的结构
                    var lists = statusList.GroupBy(p => p.statusName);
                    foreach (var items in lists)
                    {
                        //集合里面的一组数据整合成一个PlanStatusCount对象，统计count值
                        var planCountModel = new PlanStatusCount();
                        planCountModel.statusCount = items.Count();
                        foreach (var item in items)
                        {
                            planCountModel.statusName = item.statusName;
                            //让存在两个状态值的只指定更小的状态值，方便后面排序
                            if (item.statusName == Uncommitted)
                            {
                                planCountModel.status = 0;
                            }
                            else if (item.statusName == WaitSubmit)
                            {
                                planCountModel.status = 10;
                            }
                            else if (item.statusName == HasSubmit)
                            {
                                planCountModel.status = 20;
                            }
                            else if (item.statusName == WaitConfirm)
                            {
                                planCountModel.status = 30;
                            }
                            else if (item.statusName == HasComplete)
                            {
                                planCountModel.status = 90;
                            }
                            break;
                        }
                        planCountList.Add(planCountModel);
                    }
                }
                var stopList = new List<PlanStatusCount>();
                if (operate != 10)
                {
                    stopList = (from c in db.tblPlan
                                where c.stop != 0 && c.endTime.Value.Year == year && c.endTime.Value.Month == month && c.responsibleUser == userId && !c.deleteFlag
                                group c by c.stop into g
                                select new PlanStatusCount
                                {
                                    status = g.Key,
                                    statusName = HasStop,
                                    statusCount = g.Count()
                                }).ToList();
                }
                else
                {
                    stopList = (from c in db.tblPlan
                                where c.stop != 0 && c.endTime.Value.Year == year && c.endTime.Value.Month == month && !c.deleteFlag
                                group c by c.stop into g
                                select new PlanStatusCount
                                {
                                    status = g.Key,
                                    statusName = HasStop,
                                    statusCount = g.Count()
                                }).ToList();
                }

                if (stopList.Count() > 0)
                {
                    //申请中止的count
                    count = stopList.Where(p => p.status == 10).FirstOrDefault() == null ? 0 : stopList.Where(p => p.status == 10).FirstOrDefault().statusCount;
                    if (stopList.Where(p => p.status == 90).FirstOrDefault() != null)
                    {
                        planCountList.Add(stopList.Where(p => p.status == 90).FirstOrDefault());
                    }
                }
            }
            //补足数据库中不存在的状态统计数据
            AddEmptyCount(ref planCountList, 0, Uncommitted);
            AddEmptyCount(ref planCountList, 10, WaitSubmit);
            AddEmptyCount(ref planCountList, 20, HasSubmit);
            AddEmptyCount(ref planCountList, 30, WaitConfirm);
            AddEmptyCount(ref planCountList, 90, HasComplete);
            AddEmptyCount(ref planCountList, 90, HasStop);
            planCountList.Where(p => p.status == 10).FirstOrDefault().statusCount += count;
            return planCountList.OrderBy(p => p.status).ToList();
        }
        /// <summary>
        /// 个人首页饼图数据获取
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="userId">用户ID</param>
        /// <param name="isweek">是否是周状态统计</param>
        /// <returns></returns>
        public List<PlanStatusCount> GetPlanStatusByUserIndex(int year, int month, int userId, int isweek)
        {
            var planCountList = new List<PlanStatusCount>();
            int count = 0;

            using (var db = new TargetNavigationDBEntities())
            {
                //先查出计划状态和状态名的列表
                var statusList = new List<PlanStatusCount>();
                if (isweek != 1)
                {
                    statusList = (from c in db.tblPlan
                                  where c.stop == 0 && c.endTime.Value.Year == year && c.endTime.Value.Month == month && c.responsibleUser == userId && !c.deleteFlag
                                  select new PlanStatusCount
                                  {
                                      status = c.status,
                                      statusName = (c.status == 0 || c.status == 15) ? Uncommitted : ((c.status == 10 || c.status == 25) ? WaitSubmit : ((c.status == 20 || c.status == 40) ? HasSubmit : (c.status == 30 ? WaitConfirm : HasComplete)))
                                  }).ToList();
                }
                else
                {
                    var thisWeekDay = GetMondayDate(DateTime.Now).AddDays(7);
                    var Monday = GetMondayDate(DateTime.Now);
                    statusList = (from c in db.tblPlan
                                  where c.stop == 0 && (c.endTime >= Monday && c.endTime <= thisWeekDay) && c.responsibleUser == userId && !c.deleteFlag
                                  select new PlanStatusCount
                                  {
                                      status = c.status,
                                      statusName = (c.status == 0 || c.status == 15) ? Uncommitted : ((c.status == 10 || c.status == 25) ? WaitSubmit : ((c.status == 20 || c.status == 40) ? HasSubmit : (c.status == 30 ? WaitConfirm : HasComplete)))
                                  }).ToList();
                }
                if (statusList.Count() > 0)
                {
                    //然后根据相同的状态名分组，分组后是集合套集合的结构
                    var lists = statusList.GroupBy(p => p.statusName);
                    foreach (var items in lists)
                    {
                        //集合里面的一组数据整合成一个PlanStatusCount对象，统计count值
                        var planCountModel = new PlanStatusCount();
                        planCountModel.statusCount = items.Count();
                        foreach (var item in items)
                        {
                            planCountModel.statusName = item.statusName;
                            //让存在两个状态值的只指定更小的状态值，方便后面排序
                            if (item.statusName == Uncommitted)
                            {
                                planCountModel.status = 0;
                            }
                            else if (item.statusName == WaitSubmit)
                            {
                                planCountModel.status = 10;
                            }
                            else if (item.statusName == HasSubmit)
                            {
                                planCountModel.status = 20;
                            }
                            else if (item.statusName == WaitConfirm)
                            {
                                planCountModel.status = 30;
                            }
                            else if (item.statusName == HasComplete)
                            {
                                planCountModel.status = 90;
                            }
                            break;
                        }
                        planCountList.Add(planCountModel);
                    }
                }
                var stopList = new List<PlanStatusCount>();
                if (isweek != 1)
                {
                    stopList = (from c in db.tblPlan
                                where c.stop != 0 && c.endTime.Value.Year == year && c.endTime.Value.Month == month && c.responsibleUser == userId && !c.deleteFlag
                                group c by c.stop into g
                                select new PlanStatusCount
                                {
                                    status = g.Key,
                                    statusName = HasStop,
                                    statusCount = g.Count()
                                }).ToList();
                }
                else
                {
                    var thismonday = GetMondayDate(DateTime.Now);
                    var thisweekday = GetMondayDate(DateTime.Now).AddDays(7);
                    stopList = (from c in db.tblPlan
                                where c.stop != 0 && (c.endTime >= thismonday && c.endTime <= thisweekday) && c.responsibleUser == userId && !c.deleteFlag
                                group c by c.stop into g
                                select new PlanStatusCount
                                {
                                    status = g.Key,
                                    statusName = HasStop,
                                    statusCount = g.Count()
                                }).ToList();
                }


                if (stopList.Count() > 0)
                {
                    //申请中止的count
                    count = stopList.Where(p => p.status == 10).FirstOrDefault() == null ? 0 : stopList.Where(p => p.status == 10).FirstOrDefault().statusCount;
                    if (stopList.Where(p => p.status == 90).FirstOrDefault() != null)
                    {
                        planCountList.Add(stopList.Where(p => p.status == 90).FirstOrDefault());
                    }
                }
            }
            //补足数据库中不存在的状态统计数据
            AddEmptyCount(ref planCountList, 0, Uncommitted);
            AddEmptyCount(ref planCountList, 10, WaitSubmit);
            AddEmptyCount(ref planCountList, 20, HasSubmit);
            AddEmptyCount(ref planCountList, 30, WaitConfirm);
            AddEmptyCount(ref planCountList, 90, HasComplete);
            AddEmptyCount(ref planCountList, 90, HasStop);
            planCountList.Where(p => p.status == 10).FirstOrDefault().statusCount += count;
            return planCountList.OrderBy(p => p.status).ToList();
        }
        /// <summary>
        /// 计算某日起始日期（礼拜一的日期）
        /// </summary>
        /// <param name="someDate">该周中任意一天</param>
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns>
        private DateTime GetMondayDate(DateTime someDate)
        {
            var dayDiff = (someDate.DayOfWeek == DayOfWeek.Sunday) ? (someDate.DayOfWeek + 7 - 1) : (someDate.DayOfWeek - 1);

            return someDate.Subtract(new TimeSpan((int)dayDiff, 0, 0, 0));
        }


        public void AddEmptyCount(ref List<PlanStatusCount> planCountList, int status, string statusName)
        {
            if (planCountList.Where(p => p.statusName == statusName).FirstOrDefault() == null)
            {
                var planStop = new PlanStatusCount
                {
                    status = status,
                    statusName = statusName,
                    statusCount = 0
                };
                planCountList.Add(planStop);
            }
        }

        #endregion

        #region 根据用户Id查询用户信息

        /// <summary>
        /// 根据用户Id查询用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public UserInfo GetUserInfoById(int userId)
        {
            FileUpload file = new FileUpload();
            string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();
            var userModel = new UserInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                userModel = (from user in db.tblUser
                             where user.userId == userId
                             select new UserInfo
                             {
                                 userId = user.userId,
                                 userName = user.userName,
                                 bigImage = string.IsNullOrEmpty(user.originalImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + user.bigImage
                             }).FirstOrDefault();
                if (userModel != null)
                {
                    var stat = (from station in db.tblUserStation
                                join stationName in db.tblStation on station.stationId equals stationName.stationId
                                where station.userId == userModel.userId
                                select new UserInfo
                                {
                                    stationName = stationName.stationName
                                }).FirstOrDefault();
                    if (stat != null)
                    {
                        userModel.stationName = stat.stationName;
                    }
                    else { userModel.stationName = "无"; }
                }
            }
            return userModel;
        }

        #endregion

        #region 统计用户的今日未完成计划和超时计划的数量

        /// <summary>
        /// 统计用户的今日未完成计划和超时计划的数量
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>实体对象</returns>
        public UserPlanCountInfo StatisticsUserPlan(int userId)
        {
            var userPlanCount = new UserPlanCountInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                //今日未完成
                userPlanCount.todayUnfinished = db.tblPlan.Where(p => p.responsibleUser == userId && p.endTime != null && (p.endTime.Value.Year == DateTime.Now.Year && p.endTime.Value.Month == DateTime.Now.Month && p.endTime.Value.Day == DateTime.Now.Day) && (p.status == 20 || p.status == 40) && p.stop == 0 && !p.deleteFlag).Count();
                //今日计划总数
                userPlanCount.todayPlanTotal = db.tblPlan.Where(p => p.responsibleUser == userId && (p.endTime != null && (p.endTime.Value.Year == DateTime.Now.Year && p.endTime.Value.Month == DateTime.Now.Month && p.endTime.Value.Day == DateTime.Now.Day)) && !p.deleteFlag).Count();
                //超时计划
                userPlanCount.overTimePlan = db.tblPlan.Where(p => p.responsibleUser == userId && (p.endTime == null ? true : p.endTime < DateTime.Now) && (p.status == 20 || p.status == 40) && p.stop == 0 && !p.deleteFlag).Count();
            }
            return userPlanCount;
        }

        #endregion

        #region  根据PlanId获取计划操作

        ///<summary>
        /// 根据PlanId获取计划操作
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<PlanOperateInfo> GetPlanOperateByPlanID(int planId)
        {
            TargetNavigationDBEntities db = new TargetNavigationDBEntities();
            List<tblPlanOperate> PlanList = db.tblPlanOperate.Where(p => p.planId == planId).OrderByDescending(p => p.operateId).ToList();
            List<PlanOperateInfo> PlanOList = new List<PlanOperateInfo>();
            foreach (tblPlanOperate operate in PlanList)
            {
                PlanOperateInfo planOpInfo = new PlanOperateInfo();
                planOpInfo.operateId = operate.planId;
                planOpInfo.message = operate.message;
                planOpInfo.reviewTime = operate.reviewTime;
                planOpInfo.reviewUser = operate.reviewUser;
                planOpInfo.result = operate.result;
                PlanOList.Add(planOpInfo);
            }
            return PlanOList;
        }

        #endregion

        #region 归类到计划

        /// <summary>
        /// 归类到计划
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="parentPlanId">父计划Id</param>
        public void ClassificatePlan(int planId, int parentPlanId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var plan = GetPlanInfoById(planId, db);
                plan.parentPlan = parentPlanId;
                plan.updateTime = DateTime.Now;
                db.SaveChanges();
            }
        }

        #endregion

        #region 根据计划Id获取该计划的前提计划

        /// <summary>
        /// 根据计划Id获取该计划的前提计划
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns>前提计划列表</returns>
        public List<FrontPlan> GetFrontPlan(int planId)
        {
            var frontPlanList = new List<FrontPlan>();
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                var query = db.tblPlanFront.Where(p => p.planId == planId);
                if (query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        var frontPlan = (from c in db.tblPlan
                                         where c.planId == item.planFront
                                         select new FrontPlan
                                         {
                                             planId = c.planId,
                                             eventOutput = c.eventOutput,
                                             process = c.progress
                                         }).FirstOrDefault();
                        if (frontPlan != null)
                        {
                            frontPlanList.Add(frontPlan);
                        }
                    }
                }
            }
            return frontPlanList;
        }

        #endregion

        #region 根据用户Id查询上级，自己以及下属的用户信息

        /// <summary>
        /// 根据用户Id查询上级，自己以及下属的用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>人员列表</returns>
        public List<UserInfo> GetUserIdListByUserId(int userId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //用户自己的信息
                var userModel = (from u in db.tblUser
                                 where u.userId == userId && !u.deleteFlag && u.workStatus == 1
                                 select new UserInfo
                                 {
                                     id = u.userId,
                                     name = u.userName,
                                     img = u.bigImage
                                 }).FirstOrDefault();
                if (userModel != null)
                {
                    //查询自己的职位Id
                    var meStationList = db.tblUserStation.Where(p => p.userId == userId).ToList();
                    foreach (var meStation in meStationList)
                    {
                        //查询直接上级信息
                        var parentUser = (from pUser in db.tblUser
                                          join us in db.tblUserStation on pUser.userId equals us.userId into group1
                                          from us in group1.DefaultIfEmpty()
                                          join s in db.tblStation on us.stationId equals s.stationId into group2
                                          from s in group2.DefaultIfEmpty()
                                          join cs in db.tblStation on s.stationId equals cs.parentStation into group3
                                          from cs in group3.DefaultIfEmpty()
                                          where cs.stationId == meStation.stationId && !pUser.deleteFlag && pUser.workStatus == 1
                                          select new UserInfo
                                          {
                                              id = pUser.userId,
                                              name = pUser.userName,
                                              img = pUser.bigImage
                                          }).FirstOrDefault();

                        if (parentUser != null) userList.Add(parentUser);

                        //获取下属用户信息
                        var underStationIds = db.tblStation.Where(p => p.parentStation == meStation.stationId && !p.deleteFlag);
                        if (underStationIds.Count() > 0)
                        {
                            foreach (var item in underStationIds)
                            {
                                var underUsers = from user in db.tblUser
                                                 join userstation in db.tblUserStation on user.userId equals userstation.userId
                                                 where userstation.stationId == item.stationId && !user.deleteFlag && user.workStatus == 1
                                                 select new UserInfo
                                                 {
                                                     id = user.userId,
                                                     name = user.userName,
                                                     img = user.bigImage
                                                 };
                                foreach (var user in underUsers)
                                {
                                    userList.Add(user);
                                }
                            }
                        }
                    }
                    userList.Add(userModel);
                }
            }

            return userList;
        }

        public List<UserInfo> GetUserIdUpListByUserId(int userId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //用户自己的信息
                var userModel = new UserInfo();
                userModel.id = userId;
                userModel.name = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().userName;
                userModel.img = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().smallImage;
                //查询自己的职位Id
                var meStationList = db.tblUserStation.Where(p => p.userId == userId).ToList();
                foreach (var meStation in meStationList)
                {
                    var stationId = meStation == null ? 0 : meStation.stationId;
                    //查询直接上级信息
                    var parentUser = new UserInfo();
                    //查询上级职位Id
                    var parentStationId = db.tblStation.Where(p => p.stationId == stationId).FirstOrDefault() == null ? 0 : db.tblStation.Where(p => p.stationId == stationId).FirstOrDefault().parentStation;
                    parentUser.id = db.tblUserStation.Where(p => p.stationId == parentStationId).FirstOrDefault() == null ? 0 : db.tblUserStation.Where(p => p.stationId == parentStationId).FirstOrDefault().userId;
                    parentUser.name = db.tblUser.Where(p => p.userId == parentUser.id).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == parentUser.id).FirstOrDefault().userName;
                    parentUser.userName = parentUser.name;
                    parentUser.userId = parentUser.userId;
                    parentUser.img = db.tblUser.Where(p => p.userId == parentUser.id).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == parentUser.id).FirstOrDefault().smallImage;
                    if (parentUser.id != 0)
                    {
                        userList.Add(parentUser);
                    }
                }
                userList.Add(userModel);
            }
            return userList;
        }

        public List<UserInfo> GetUserIdDownListByUserId(int userId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //用户自己的信息
                var userModel = new UserInfo();
                userModel.id = userId;
                userModel.name = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().userName;
                userModel.img = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().smallImage;
                //查询自己的职位Id
                var meStation = db.tblUserStation.Where(p => p.userId == userId).FirstOrDefault();
                var stationId = meStation == null ? 0 : meStation.stationId;
                //获取下属用户信息
                var underStationIds = db.tblStation.Where(p => p.parentStation == stationId && !p.deleteFlag);
                if (underStationIds.Count() > 0)
                {
                    foreach (var item in underStationIds)
                    {
                        var underUsers = from user in db.tblUser
                                         join userstation in db.tblUserStation on user.userId equals userstation.userId
                                         where userstation.stationId == item.stationId && !user.deleteFlag
                                         select new UserInfo
                                         {
                                             id = user.userId,
                                             name = user.userName,
                                             img = user.smallImage
                                         };
                        foreach (var user in underUsers)
                        {
                            userList.Add(user);
                        }
                    }
                }
                userList.Add(userModel);
            }
            return userList;
        }

        #endregion

        #region 人员模糊查询

        /// <summary>
        /// 输入任意数字进行模糊查询
        /// </summary>
        /// <param name="word">筛选字段</param>
        /// <param name="hasImage">是否需要返回头像</param>
        /// <returns></returns>
        public List<UserInfo> SelectUserList(string word, bool hasImage)
        {
            TargetNavigationDBEntities db = new TargetNavigationDBEntities();
            var userList = new List<UserInfo>();
            userList = (from us in db.tblUser
                        where us.userName.IndexOf(word) != -1 && !us.deleteFlag && us.workStatus == 1
                        select new UserInfo
                        {
                            userId = us.userId,
                            userName = us.userName,
                            id = us.userId,
                            name = us.userName,
                            img = us.smallImage
                        }).ToList();

            return userList;
        }

        public List<PlanInfo> SelectFrontPlanList(string word)
        {
            TargetNavigationDBEntities db = new TargetNavigationDBEntities();
            var Planinfo = new List<PlanInfo>();
            Planinfo = (from us in db.tblPlan
                        where us.eventOutput.IndexOf(word) != -1
                        select new PlanInfo
                        {
                            planId = us.planId,
                            eventOutput = us.eventOutput,
                            id = us.planId,
                            name = us.eventOutput,
                            responsibleUser = us.responsibleUser,
                            responsibleUserName = db.tblUser.Where(p => p.userId == us.responsibleUser).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == us.responsibleUser).FirstOrDefault().userName,
                            endTime = us.endTime,
                            importance = us.importance,
                            urgency = us.urgency,
                            projectName = db.tblProject.Where(p => p.projectId == us.projectId).FirstOrDefault() == null ? "无" : db.tblProject.Where(p => p.projectId == us.projectId).FirstOrDefault().projectName,
                            organizationName = db.tblOrganization.Where(p => p.organizationId == us.organizationId).FirstOrDefault() == null ? "无" : db.tblOrganization.Where(p => p.organizationId == us.organizationId).FirstOrDefault().organizationName,
                            progress = us.progress
                        }).ToList();

            return Planinfo;
        }

        #endregion

        #region 递归绑定部门树形结构

        /// <summary>
        /// 绑定部门的组织架构
        /// </summary>
        /// <returns>部门树形结构数据</returns>
        public List<OrganizationModel> GetDepartmentList()
        {
            var departmentList = new List<OrganizationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //获取第一级的部门
                departmentList = (from org in db.tblOrganization
                                  where org.parentOrganization == null
                                  select new OrganizationModel
                                  {
                                      id = org.organizationId,
                                      name = org.organizationName
                                  }).ToList();
                //根据第一级的组织绑定子部门
                GetChildDepartList(departmentList, db);
            }
            return departmentList;
        }

        /// <summary>
        /// 递归绑定子部门组织架构
        /// </summary>
        /// <param name="parentlist"></param>
        /// <param name="db">连接数据库上下文</param>
        /// <returns>树形集合</returns>
        public List<OrganizationModel> GetChildDepartList(List<OrganizationModel> parentlist, TargetNavigationDBEntities db)
        {
            foreach (var item in parentlist)
            {
                var childList = (from org in db.tblOrganization
                                 join orgChild in db.tblOrganization on org.organizationId equals orgChild.parentOrganization
                                 where org.organizationId == item.id
                                 select new OrganizationModel
                                 {
                                     id = orgChild.organizationId,
                                     name = orgChild.organizationName
                                 }).ToList();
                if (childList.Count() <= 0) continue;
                item.children = childList;
                GetChildDepartList(childList, db);
            }
            return parentlist;
        }

        #endregion

        #region 递归绑定项目的组织架构

        /// <summary>
        /// 绑定项目的组织架构
        /// </summary>
        /// <returns>项目的树形结构集合</returns>
        public List<ProjectModel> GetProjectList()
        {
            var projectList = new List<ProjectModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //获取第一级的项目分类
                projectList = (from pro in db.tblProject
                               where pro.parentProject == null
                               select new ProjectModel
                               {
                                   id = pro.projectId,
                                   name = pro.projectName
                               }).ToList();
                //根据第一级的组织绑定子项目分类
                GetChildProjectList(projectList, db);
            }
            return projectList;
        }

        /// <summary>
        /// 递归绑定子项目组织架构
        /// </summary>
        /// <param name="parentlist"></param>
        /// <param name="db">连接数据库上下文</param>
        /// <returns>树形集合</returns>
        public List<ProjectModel> GetChildProjectList(List<ProjectModel> parentlist, TargetNavigationDBEntities db)
        {
            foreach (var item in parentlist)
            {
                var childList = (from pro in db.tblProject
                                 join proChild in db.tblProject on pro.projectId equals proChild.parentProject
                                 where pro.projectId == item.id
                                 select new ProjectModel
                                 {
                                     id = proChild.projectId,
                                     name = proChild.projectName
                                 }).ToList();
                if (childList.Count() <= 0) continue;
                item.children = childList;
                GetChildProjectList(childList, db);
            }
            return parentlist;
        }

        #endregion

        #region 获取执行方式信息

        /// <summary>
        /// 获取执行方式信息
        /// </summary>
        /// <returns>执行方式列表</returns>
        public List<ExecutionModel> GetExecutionList()
        {
            var executionList = new List<ExecutionModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                executionList = (from execution in db.tblExecutionMode
                                 where !execution.deleteFlag
                                 select new ExecutionModel
                                 {
                                     id = execution.executionId,
                                     text = execution.executionMode
                                 }).ToList();
            }
            return executionList;
        }

        #endregion

        #region 获取前提计划信息

        /// <summary>
        /// 获取前提计划信息
        /// </summary>
        /// <param name="selectText">模糊查询内容</param>
        /// <param name="organizationId">部门Id</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="responsibleUser">责任人Id</param>
        /// <returns>只包含计划Id和事项输出的列表</returns>
        public List<PlanInfo> GetFrontPlanInfo(string selectText, int organizationId, int projectId, int responsibleUser)
        {
            var planList = new List<PlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                planList = (from plan in db.tblPlan
                            where plan.eventOutput.IndexOf(selectText) < 0 && plan.organizationId == organizationId
                                && plan.projectId == projectId && plan.responsibleUser == responsibleUser && plan.status != 90 && plan.stop != 90
                            select new PlanInfo
                            {
                                planId = plan.planId,
                                eventOutput = plan.eventOutput
                            }).ToList();
            }
            return planList;
        }

        #endregion

        #region 根据计划Id查询附件信息

        /// <summary>
        /// 根据计划Id查询附件信息
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns>附件集合</returns>
        public List<PlanAttachment> GetPlanAttachmentList(int planId)
        {
            var planAttachmentList = new List<PlanAttachment>();
            using (var db = new TargetNavigationDBEntities())
            {
                planAttachmentList = (from c in db.tblPlanAttachment
                                      where c.planId == planId
                                      select new PlanAttachment
                                      {
                                          attachmentId = c.attachmentId,
                                          attachmentName = c.displayName,
                                          saveName = c.saveName,
                                          extension = c.extension
                                      }).ToList();
            }
            return planAttachmentList;
        }

        #endregion

        #region 根据父计划Id查询子计划集合

        /// <summary>
        /// 根据父计划Id查询子计划集合
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns>子计划集合</returns>
        public List<ChildPlanInfo> GetChildPlanList(int planId)
        {
            var childPlanList = new List<ChildPlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                childPlanList = (from c in db.tblPlan
                                 join u in db.tblUser on c.responsibleUser equals u.userId
                                 where c.parentPlan == planId
                                 select new ChildPlanInfo
                                 {
                                     childPlanId = c.planId,
                                     importance = c.importance,
                                     urgency = c.urgency,
                                     eventOutput = c.eventOutput,
                                     responsibleUser = c.responsibleUser,
                                     responsibleUserName = u.userName,
                                     endTime = c.endTime,
                                     withSub = c.withSub,
                                     projectId = c.projectId,
                                     organizationId = c.organizationId,
                                     process = c.progress == null ? "0%" : (c.progress + "%")
                                 }).ToList();
                var stringList = new List<string>();
                foreach (var item in childPlanList)
                {
                    stringList.Clear();
                    item.projectName = GetProListByOrgId(db, item.projectId, stringList);
                    stringList.Clear();
                    item.organizationName = GetOrgStringByOrgId(db, item.organizationId, stringList);
                }
            }
            return childPlanList;
        }

        #endregion

        public List<PlanInfo> GetplanInfoByIdList(List<int> planId)
        {
            List<PlanInfo> planList = new List<PlanInfo>();

            using (var db = new TargetNavigationDBEntities())
            {
                foreach (int id in planId)
                {
                    PlanInfo plan = new PlanInfo();
                    plan = GetPlanInfoByPlanId(id, false, false, false, null);
                    if (plan != null)
                    {
                        planList.Add(plan);
                    }
                }
            }
            return planList;
        }

        #region 根据ID获取计划详情

        /// <summary>
        /// 根据ID获取计划详情
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="isloop">true:是循环计划 false：不是循环计划</param>
        /// <param name="withfront">true:前提计划  false：不是前提计划</param>
        /// <param name="collPlan">true：是写作计划 false：不是协作计划</param>
        /// <param name="submintId">循环计划提交Id</param>
        /// <returns></returns>
        public PlanInfo GetPlanInfoByPlanId(int planId, bool isloop, bool withfront, bool collPlan, int? submintId)
        {
            var planInfoNew = new PlanInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                if (isloop)
                {
                    planInfoNew = (from c in db.tblLoopPlan
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
                                   where !c.deleteFlag && c.loopId == planId && (submintId == null ? true : s.submitId == submintId.Value)
                                   select new PlanInfo
                                   {
                                       planId = c.loopId,
                                       executionModeId = c.executionModeId,
                                       responsibleOrganization = c.responsibleOrganization,
                                       responsibleUser = c.responsibleUser,
                                       responsibleUserName = f.userName,
                                       confirmOrganization = c.confirmOrganization,
                                       confirmUser = c.confirmUser,
                                       confirmUserName = db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault().userName,
                                       eventOutputId = c.eventOutputId,
                                       eventOutput = c.eventOutput,
                                       startTime = c.startTime,
                                       endTime = c.endTime.Value,

                                       comment = c.comment,
                                       importance = c.importance,
                                       urgency = c.urgency,
                                       difficulty = c.difficulty,
                                       isLoopPlan = 1,
                                       loopType = c.loopType,
                                       IsCollPlan = 0,
                                       time = c.unitTime,
                                       status = c.status,
                                       stop = 0,
                                       loopStatus = c.loopStatus,
                                       createTime = c.createTime,
                                       updateTime = c.updateTime,
                                       submitTime = s.submitTime,
                                       completeQuality = s.completeQuality,
                                       completeQuantity = s.completeQuantity,
                                       completeTime = s.completeTime,
                                       quantity = s.number,

                                       projectId = c.projectId,
                                       parentProject = b.parentProject,
                                       projectName = b.projectName,

                                       organizationId = c.organizationId,
                                       parentOrganization = d.parentOrganization,
                                       schemaName = d.schemaName,
                                       organizationName = d.organizationName,

                                       executionId = e.executionId,
                                       executionMode = e.executionMode
                                   }).FirstOrDefault();
                }
                else
                {
                    planInfoNew = (from c in db.tblPlan
                                   join b in db.tblProject on c.projectId equals b.projectId into group1
                                   from b in group1.DefaultIfEmpty()
                                   join d in db.tblOrganization on c.organizationId equals d.organizationId into group2
                                   from d in group2.DefaultIfEmpty()
                                   join e in db.tblExecutionMode on c.executionModeId equals e.executionId into group3
                                   from e in group3.DefaultIfEmpty()
                                   join f in db.tblUser on c.responsibleUser equals f.userId into group4
                                   from f in group4.DefaultIfEmpty()
                                   where !c.deleteFlag && c.planId == planId
                                   select new PlanInfo
                                   {
                                       planId = c.planId,
                                       parentPlan = c.parentPlan,
                                       executionModeId = c.executionModeId,
                                       responsibleOrganization = c.responsibleOrganization,
                                       responsibleUser = c.responsibleUser,
                                       responsibleUserName = f.userName,
                                       confirmOrganization = c.confirmOrganization,
                                       confirmUser = c.confirmUser,
                                       confirmUserName = db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == c.confirmUser).FirstOrDefault().userName,
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
                                       status = c.status,
                                       stop = c.stop,
                                       createTime = c.createTime,
                                       updateTime = c.updateTime,
                                       initial = c.initial,
                                       withSub = c.withSub,
                                       archive = c.archive,
                                       archiveTime = c.archiveTime,
                                       autoStart = c.autoStart,
                                       //eventOutputId = c.eventOutputId,
                                       eventOutput = c.eventOutput,
                                       isLoopPlan = 0,
                                       IsCollPlan = 0,
                                       isFronPlan = c.withFront.Value ? 1 : 0,
                                       withFront = c.withFront,
                                       effectiveTime = c.quantity == null ? 0 : c.quantity * c.time,
                                       realTime = (c.quantity == null || c.completeQuantity == null || c.completeQuality == null) ? 0 : c.quantity * c.time * c.completeQuantity * c.completeQuality * c.completeTime,

                                       projectId = c.projectId,
                                       parentProject = b.parentProject,
                                       projectName = b.projectName,
                                       proOrderNumber = b.orderNumber,

                                       organizationId = c.organizationId,
                                       parentOrganization = d.parentOrganization,
                                       schemaName = d.schemaName,
                                       organizationName = d.organizationName,
                                       orgOrderNumber = d.orderNumber,

                                       executionId = e.executionId,
                                       executionMode = e.executionMode
                                   }).FirstOrDefault();
                }
                if (planInfoNew != null)
                {
                    planInfoNew.endTimeNew = planInfoNew.endTime == null ? "" : planInfoNew.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                    planInfoNew.startTimeNew = planInfoNew.startTime == null ? "" : planInfoNew.startTime.Value.ToString("yyyy-MM-dd HH:mm");
                    var stringList = new List<string>();
                    stringList.Clear();
                    planInfoNew.projectName = GetProListByOrgId(db, planInfoNew.projectId, stringList);
                    stringList.Clear();
                    planInfoNew.organizationName = GetOrgStringByOrgId(db, planInfoNew.organizationId, stringList);
                    if (withfront)
                    {
                        planInfoNew.frontLists = GetFrontPlan(planId);
                    }
                    planInfoNew.collPlanList = GetCollPlanUsers(planId, db);
                    planInfoNew.planAttachmentList = isloop ? this.GetLoopPlanFileData(planId) : this.GetPlanAttachmentList(planId);

                    //标签处理
                    string keyword = null;
                    if (isloop)
                        //循环计划标签
                        keyword = db.tblLoopPlan.Where(p => p.loopId == planId).Select(p => p.keyword).FirstOrDefault();
                    else
                        //一般计划标签
                        keyword = db.tblPlan.Where(p => p.planId == planId).Select(p => p.keyword).FirstOrDefault();

                    planInfoNew.keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Split(',');
                }
            }

            return planInfoNew;
        }

        #endregion

        #region 获取计划的所有协作人信息

        /// <summary>
        /// 获取计划的所有协作人信息
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="db">数据库上下文</param>
        /// <returns></returns>
        public List<CollPlan> GetCollPlanUsers(int planId, TargetNavigationDBEntities db)
        {
            var userIds = db.tblPlanCooperation.Where(p => p.planId == planId);
            var collUsers = new List<CollPlan>();
            foreach (var item in userIds)
            {
                var collUser = new CollPlan();
                collUser.id = item.userId;
                collUser.name = db.tblUser.Where(p => p.userId == item.userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == item.userId).FirstOrDefault().userName;
                collUsers.Add(collUser);
            }
            return collUsers;
        }

        #endregion

        #region 根据计划Id获取计划审核信息

        /// <summary>
        /// 根据计划Id获取计划审核信息
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns>计划审核实体</returns>
        public PlanCheckInfo GetPlanCheckInfo(int planId)
        {
            var planInfo = new PlanCheckInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                planInfo = (from plan in db.tblPlan
                            where plan.planId == planId
                            select new PlanCheckInfo
                            {
                                planId = plan.planId,
                                status = plan.status,
                                stop = plan.stop,
                                importance = plan.importance,
                                urgency = plan.urgency,
                                difficulty = plan.difficulty
                            }).FirstOrDefault();
            }
            return planInfo;
        }

        #endregion

        #region 删除计划

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="userId">用户Id</param>
        public void DeletePlan(int planId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = GetPlanInfoById(planId, db);
                firstData.deleteFlag = true;
                firstData.updateTime = DateTime.Now;
                firstData.updateUser = userId;
                //添加日志
                this.AddPlanOperate(planId, string.Empty, (int)ConstVar.PlanOperateStatus.delete, userId, db);
                db.SaveChanges();
            }
        }

        #endregion

        #region 获取计划的自评数量和时间

        /// <summary>
        /// 获取计划的自评数量和时间
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns></returns>
        public PlanCheckInfo GetPlanConfirmInfo(int planId)
        {
            var planConfirmInfo = new PlanCheckInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                planConfirmInfo = (from c in db.tblPlan
                                   where c.planId == planId
                                   select new PlanCheckInfo
                                   {
                                       quantity = c.quantity,
                                       time = c.time
                                   }).FirstOrDefault();
            }
            return planConfirmInfo;
        }

        #endregion

        #region 分解计划

        /// <summary>
        /// 分解计划
        /// </summary>
        /// <param name="planInfo">分解的计划实体</param>
        /// <param name="userId">用户Id</param>
        public void ResolvePlan(PlanInfo planInfo, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var planId = db.prcGetPrimaryKey("tblPlan", obj).FirstOrDefault().Value;
                planInfo.planId = planId;
                var plan = new tblPlan
                {
                    planId = planInfo.planId,
                    parentPlan = planInfo.parentPlan,
                    projectId = planInfo.projectId,
                    organizationId = planInfo.organizationId,
                    executionModeId = planInfo.executionModeId,
                    eventOutput = planInfo.eventOutput,
                    responsibleUser = planInfo.responsibleUser,
                    confirmUser = planInfo.confirmUser,
                    endTime = planInfo.endTime,
                    importance = planInfo.importance,
                    urgency = planInfo.urgency,
                    difficulty = planInfo.difficulty,
                    progress = planInfo.progress,
                    status = planInfo.status,
                    initial = planInfo.initial,
                    stop = planInfo.stop,
                    archive = planInfo.archive,
                    createUser = planInfo.createUser,
                    createTime = planInfo.createTime,
                    updateUser = planInfo.updateUser,
                    updateTime = planInfo.updateTime,
                    deleteFlag = planInfo.deleteFlag
                };
                db.tblPlan.Add(plan);
                //添加日志
                this.AddPlanOperate(planInfo.parentPlan.Value, string.Empty, (int)ConstVar.PlanOperateStatus.resolve, userId, db);
                db.SaveChanges();
            }
        }

        #endregion

        #region 获取计划日志

        /// <summary>
        /// 获取计划日志
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <param name="isloop">是否为循环计划</param>
        /// <returns></returns>
        public List<PlanOperateModel> GetOperates(int planId, bool isloop)
        {
            if (isloop)
            {
                return GetLoopPlanOperates(planId);
            }
            else
            {
                return GetPlanOperates(planId);
            }
        }

        /// <summary>
        /// 获取计划日志
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns>日志集合</returns>
        public List<PlanOperateModel> GetPlanOperates(int planId)
        {
            var list = new List<PlanOperateModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from c in db.tblPlanOperate
                        join u in db.tblUser on c.reviewUser equals u.userId into group1
                        from u in group1.DefaultIfEmpty()
                        where c.planId == planId
                        select new PlanOperateModel
                        {
                            operateId = c.operateId,
                            planId = c.planId,
                            message = c.message,
                            type = c.result,
                            userId = c.reviewUser,
                            user = u.userName,
                            time = c.reviewTime
                        }).OrderByDescending(c => c.operateId).ToList();
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        item.timeNew = item.time == null ? "" : item.time.Value.ToString("yyyy-MM-dd HH:mm");
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 根据ID获取循环计划操作日志
        /// </summary>
        /// <param name="loopId">循环计划ID</param>
        /// <returns></returns>
        public List<PlanOperateModel> GetLoopPlanOperates(int loopId)
        {
            var list = new List<PlanOperateModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from c in db.tblLoopplanOperate
                        join u in db.tblUser on c.reviewUser equals u.userId into group1
                        from u in group1.DefaultIfEmpty()
                        where c.loopId == loopId
                        select new PlanOperateModel
                        {
                            operateId = c.operateId,
                            planId = c.loopId,
                            message = c.message,
                            type = c.result,
                            userId = c.reviewUser,
                            user = u.userName,
                            time = c.reviewTime
                        }).ToList<PlanOperateModel>();
            }
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    item.timeNew = item.time == null ? "" : item.time.Value.ToString("yyyy-MM-dd HH:mm");
                }
            }
            return list;
        }

        #endregion

        #region 保存计划

        /// <summary>
        /// 保存计划
        /// </summary>
        /// <param name="planList">计划列表</param>
        /// <param name="userId">用户Id</param>
        public void SavePlans(List<PlanInfo> planList, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in planList)
                {
                    //修改计划信息表
                    if (item.loopType == 0)
                    {
                        var firstdata = GetPlanInfoById(item.planId, db);
                        if (firstdata != null)
                        {
                            firstdata.organizationId = item.organizationId;
                            firstdata.projectId = item.projectId;
                            firstdata.executionModeId = item.executionModeId;
                            firstdata.eventOutput = item.eventOutput;
                            firstdata.responsibleUser = item.responsibleUser;
                            firstdata.confirmUser = item.confirmUser;
                            firstdata.initial = item.initial;
                            firstdata.endTime = item.endTime;
                            firstdata.updateUser = userId;
                            firstdata.updateTime = DateTime.Now;
                            firstdata.keyword = item.keyword != null && item.keyword.Length > 0 ? string.Join(",", item.keyword) : null;
                        }
                    }
                    //修改循环计划表
                    else
                    {
                        var firstloop = db.tblLoopPlan.Where(p => p.loopId == item.planId).FirstOrDefault();
                        if (firstloop != null)
                        {
                            firstloop.organizationId = item.organizationId;
                            firstloop.projectId = item.projectId;
                            firstloop.executionModeId = item.executionModeId;
                            firstloop.eventOutput = item.eventOutput;
                            firstloop.responsibleUser = item.responsibleUser;
                            firstloop.confirmUser = item.confirmUser;
                            firstloop.loopType = item.loopType;
                            firstloop.endTime = item.endTime;
                            firstloop.updateUser = userId;
                            firstloop.updateTime = DateTime.Now;
                            firstloop.keyword = item.keyword != null && item.keyword.Length > 0 ? string.Join(",", item.keyword) : null;
                        }
                    }
                    //修改协作计划表
                    if (item.collPlanList.Count() > 0)
                    {
                        foreach (var collplan in item.collPlanList)
                        {
                            var collfirst = db.tblPlanCooperation.Where(p => p.planId == item.planId);
                            if (collfirst.Count() > 0)
                            {
                                foreach (var model in collfirst)
                                {
                                    db.tblPlanCooperation.Remove(model);
                                }
                                var collmodel = new tblPlanCooperation
                                {
                                    planId = item.planId,
                                    userId = collplan.id
                                };
                                db.tblPlanCooperation.Add(collmodel);
                            }
                            else
                            {
                                var collmodel = new tblPlanCooperation
                                {
                                    planId = item.planId,
                                    userId = collplan.id
                                };
                                db.tblPlanCooperation.Add(collmodel);
                            }
                        }
                    }
                    //修改前提计划表
                    if (item.frontLists.Count() > 0)
                    {
                        foreach (var frontplan in item.frontLists)
                        {
                            var frontfirst = db.tblPlanFront.Where(p => p.planId == item.planId);
                            if (frontfirst.Count() > 0)
                            {
                                foreach (var model in frontfirst)
                                {
                                    db.tblPlanFront.Remove(model);
                                }
                                var frontmodel = new tblPlanFront
                                {
                                    planId = item.planId,
                                    planFront = frontplan.planId,
                                    //createUser = userId,
                                    //createTime = DateTime.Now,
                                    //updateUser = userId,
                                    //updateTime = DateTime.Now
                                };
                                db.tblPlanFront.Add(frontmodel);
                            }
                            else
                            {
                                var frontmodel = new tblPlanFront
                                {
                                    planId = item.planId,
                                    planFront = frontplan.planId,
                                    //createUser = userId,
                                    //createTime = DateTime.Now,
                                    //updateUser = userId,
                                    //updateTime = DateTime.Now
                                };
                                db.tblPlanFront.Add(frontmodel);
                            }
                        }
                    }
                    //添加日志
                    this.AddPlanOperate(item.planId, string.Empty, (int)ConstVar.PlanOperateStatus.updateSave, userId, db);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region 获取评论信息

        /// <summary>
        /// 获取评论信息
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <returns>该计划下的评论信息</returns>
        public List<DiscussModel> GetDiscussList(int planId)
        {
            var discussList = new List<DiscussModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                discussList = (from c in db.tblPlanSuggestion
                               join replyUser in db.tblUser on c.replyUser equals replyUser.userId into group1
                               from replayUser in group1.DefaultIfEmpty()
                               join createUser in db.tblUser on c.createUser equals createUser.userId into group2
                               from createUser in group2.DefaultIfEmpty()
                               where c.planId == planId && !c.deleteFlag
                               select new DiscussModel
                               {
                                   suggestionId = c.suggestionId,
                                   planId = planId,
                                   suggestion = c.suggestion,
                                   replyUser = c.replyUser.Value == null ? 0 : c.replyUser.Value,
                                   replyUserName = replayUser.userName,
                                   createUser = c.createUser,
                                   createUserName = createUser.userName,
                                   createTime = c.createTime,
                                   img = (createUser.bigImage == null || createUser.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + createUser.bigImage + "?" + DateTime.Now)
                               }).ToList();
            }
            return discussList;
        }

        #endregion

        #region 添加评论

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="content">评论内容</param>
        /// <param name="planId">计划Id</param>
        /// <returns>评论信息</returns>
        public DiscussModel AddDiscuss(string content, int planId, int userId)
        {
            var discuss = new DiscussModel();
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var suggestionId = db.prcGetPrimaryKey("tblPlanSuggestion", obj).FirstOrDefault().Value;
                var suggestModel = new tblPlanSuggestion
                {
                    suggestionId = suggestionId,
                    planId = planId,
                    suggestion = content,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblPlanSuggestion.Add(suggestModel);
                discuss.suggestionId = suggestModel.suggestionId;
                discuss.planId = planId;
                discuss.suggestion = content;
                discuss.createUser = userId;
                discuss.createUserName = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().userName;
                discuss.NewCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                discuss.img = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().bigImage;
                //添加日志
                this.AddPlanOperate(planId, content, (int)ConstVar.PlanOperateStatus.discuss, userId, db);
                db.SaveChanges();
            }
            return discuss;
        }

        #endregion

        #region 添加回复

        /// <summary>
        /// 添加回复
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="replyUser">回复人Id</param>
        /// <param name="userId">评论人Id</param>
        /// <param name="replyUserName">回复人姓名</param>
        /// <param name="content">回复信息</param>
        /// <returns></returns>
        public DiscussModel AddReplySuggestion(int planId, int userId, int replyUser, string replyUserName, string content)
        {
            var discuss = new DiscussModel();
            FileUpload file = new FileUpload();
            string path = file.ConfigPath(Convert.ToInt32(UploadFilePath.HeadImage));
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var operId = db.prcGetPrimaryKey("tblPlanOperate", obj).FirstOrDefault().Value;
                var suggestionId = db.prcGetPrimaryKey("tblPlanSuggestion", obj).FirstOrDefault().Value;
                var suggestModel = new tblPlanSuggestion
                {
                    suggestionId = suggestionId,
                    planId = planId,
                    suggestion = content,
                    createUser = userId,
                    replyUser = replyUser,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblPlanSuggestion.Add(suggestModel);
                discuss.suggestionId = suggestModel.suggestionId;
                discuss.planId = planId;
                discuss.suggestion = content;
                discuss.createUser = userId;
                discuss.replyUser = replyUser;
                discuss.replyUserName = replyUserName;
                discuss.createUserName = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().userName;
                discuss.NewCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                discuss.img = "/" + path + "/" + db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().smallImage;
                //添加日志
                this.AddPlanOperate(planId, content, (int)ConstVar.PlanOperateStatus.discuss, userId, db);
                db.SaveChanges();
            }
            return discuss;
        }

        #endregion

        #region 计划附件上传 和 删除

        /// <summary>
        /// 计划附件上传
        /// </summary>
        /// <param name="Request">前台HttpRequestBase对象</param>
        /// <param name="planId">计划ID</param>
        /// <param name="userId">用户ID</param>
        public PlanAttachment UplpadMultipleFiles(HttpPostedFileBase hpf, int planId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                FileUpload file = new FileUpload();
                AjaxUploadFileResult ajaxUpFileResult = file.UploadMultipleFiles(hpf, UploadFilePath.Plan, planId, userId, db);
                tblPlanAttachment attchmentInfo = new tblPlanAttachment();
                attchmentInfo = file.InsertPlanAttachment(ajaxUpFileResult, db);
                var planAttch = new PlanAttachment
                {
                    attachmentId = attchmentInfo.attachmentId,
                    attachmentName = attchmentInfo.displayName,
                    extension = attchmentInfo.extension,
                    saveName = attchmentInfo.saveName,
                    //附件预览标志
                    isPreviewable = attchmentInfo.isPreviewable == null ? false : attchmentInfo.isPreviewable.Value
                };
                db.SaveChanges();
                return planAttch;
            }
        }

        /// <summary>
        /// 删除计划附件
        /// </summary>
        /// <param name="id">附件ID</param>
        public void DeleteFile(int id)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                FileUpload file = new FileUpload();
                file.DeletePlanAttachmentById(id, UploadFilePath.Plan, db);
                db.SaveChanges();
            }
        }

        #endregion

        #region 循环计划附件上传 和 删除

        /// <summary>
        /// 计划附件上传
        /// </summary>
        /// <param name="Request">前台HttpRequestBase对象</param>
        /// <param name="loopId">循环计划ID</param>
        /// <param name="userId">用户ID</param>
        public PlanAttachment LoopUplpadMultipleFiles(HttpPostedFileBase hpf, int loopId, int userId)
        {
            var planAttch = new PlanAttachment();
            using (var db = new TargetNavigationDBEntities())
            {
                FileUpload file = new FileUpload();
                AjaxUploadFileResult ajaxUpFileResult = file.UploadMultipleFiles(hpf, UploadFilePath.Plan, loopId, userId, db);
                var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var attachmentId = db.prcGetPrimaryKey("tblLoopplanAttachment", obj).FirstOrDefault().Value;
                var Attachment = new tblLoopplanAttachment
                {
                    attachmentId = attachmentId,
                    loopId = loopId,
                    displayName = ajaxUpFileResult.displayName,
                    createUser = ajaxUpFileResult.createUser,
                    updateUser = ajaxUpFileResult.createUser,
                    deleteFlag = false,
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now,
                    extension = ajaxUpFileResult.extension,
                    saveName = ajaxUpFileResult.saveName
                };
                db.tblLoopplanAttachment.Add(Attachment);
                planAttch = new PlanAttachment
                {
                    attachmentId = attachmentId,
                    attachmentName = Attachment.displayName
                };
                db.SaveChanges();
            }
            return planAttch;
        }

        /// <summary>
        /// 删除循环计划附件
        /// </summary>
        /// <param name="id">附件ID</param>
        public void DeleteLoopFile(int id)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                FileUpload file = new FileUpload();
                file.DeletePlanAttachmentById(id, UploadFilePath.Plan, db);
                db.SaveChanges();
            }
        }

        #endregion

        #region 根据计划ID获取计划附件表附件信息

        /// <summary>
        /// 根据计划ID获取计划附件表附件信息
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <returns>List<planAttachment></returns>
        public List<PlanAttachment> PlanFileData(int planId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                List<PlanAttachment> attchList = new List<PlanAttachment>();
                List<tblPlanAttachment> attchmentList = db.tblPlanAttachment.Where(a => a.planId == planId).ToList<tblPlanAttachment>();
                foreach (var item in attchmentList)
                {
                    var attchment = new PlanAttachment
                    {
                        attachmentId = item.attachmentId,
                        attachmentName = item.displayName,
                        extension = item.extension,
                        //附件预览标志
                        isPreviewable = item.isPreviewable == null ? false : item.isPreviewable.Value
                    };
                    attchList.Add(attchment);
                }
                return attchList;
            }
        }

        #endregion

        #region 获取循环计划附件列表

        /// <summary>
        /// 获取循环计划附件列表
        /// </summary>
        /// <param name="loopId">计划ID</param>
        /// <returns>附件列表</returns>
        public List<PlanAttachment> GetLoopPlanFileData(int loopId)
        {
            var attchList = new List<PlanAttachment>();
            using (var db = new TargetNavigationDBEntities())
            {
                attchList = (from a in db.tblLoopplanAttachment
                             where !a.deleteFlag && a.loopId == loopId
                             select new PlanAttachment
                             {
                                 attachmentId = a.attachmentId,
                                 attachmentName = a.displayName,
                                 extension = a.extension
                             }).ToList();
            }
            return attchList;
        }

        #endregion

        #region 判断是否是执行力管理员

        /// <summary>
        /// 判断是否是执行力管理员
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public bool GetExecution(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var user = (from u in db.tblUser
                            where u.userId == userId
                            select u.execution).FirstOrDefault();
                return user == null ? false : user.Value;
            }
        }

        #endregion

        #region 获取用户设置的每页显示数量

        /// <summary>
        /// 获取用户设置的每页显示数量
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>每页显示数量</returns>
        public int GetPageSize(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var userInfo = from u in db.tblPersonalSetting
                               where u.userId == userId
                               select u;
                if (userInfo.FirstOrDefault() != null)
                {
                    if (userInfo.FirstOrDefault().pageSize != null)
                    {
                        return userInfo.FirstOrDefault().pageSize.Value;
                    }
                }
                return 20;
            }
        }

        #endregion

        #region 根据用户Id查询头像信息

        /// <summary>
        /// 根据用户Id查询头像信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserImg(int userId)
        {
            var userImg = string.Empty;

            using (var db = new TargetNavigationDBEntities())
            {
                userImg = db.tblUser.Where(p => p.userId == userId).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().smallImage;
            }
            return string.IsNullOrWhiteSpace(userImg) ? "../../Images/common/portrait.png" : ("/HeadImage/" + userImg + "?" + DateTime.Now);
        }

        #endregion

        #region 删除评论

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="suggestionId">评论Id</param>
        public void DeleteComment(int suggestionId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var firstData = db.tblPlanSuggestion.Where(p => p.suggestionId == suggestionId).FirstOrDefault();
                if (firstData != null)
                {
                    db.tblPlanSuggestion.Remove(firstData);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region 计划归类

        public tblPlan GetBytblPlanId(int planid, TargetNavigationDBEntities db)
        {
            return db.tblPlan.Where(a => a.planId == planid).FirstOrDefault<tblPlan>();
        }

        public void GetplanToParentPlan(int ParentPlanId, List<PlanIdList> planId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                //var operId = db.prcGetPrimaryKey("tblNews", obj).FirstOrDefault().Value;
                var flag = false;
                foreach (var item in planId)
                {
                    tblPlan firstData = GetBytblPlanId(item.planId, db);
                    if (firstData != null)
                    {
                        firstData.parentPlan = ParentPlanId;
                        flag = true;
                    }
                }
                if (flag)
                {
                    var parentData = GetBytblPlanId(ParentPlanId, db);
                    if (parentData != null) parentData.withSub = true;
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region 获取计划日程化批量操作的计划集合

        /// <summary>
        /// 获取计划日程化批量操作的计划集合
        /// </summary>
        /// <param name="planIds">普通计划ID集合</param>
        /// <param name="loopIds">循环计划Id集合</param>
        /// <returns>计划信息集合</returns>
        public List<CheckPlanModel> GetPlanSimpleList(int?[] planIds, int?[] loopIds)
        {
            var checkPlanList = new List<CheckPlanModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (planIds != null && planIds.Length > 0)
                {
                    var planList = (from plan in db.tblPlan
                                    join user in db.tblUser on plan.responsibleUser equals user.userId into group1
                                    from user in group1.DefaultIfEmpty()
                                    where !plan.deleteFlag && planIds.Contains(plan.planId)
                                    select new CheckPlanModel
                                    {
                                        planId = plan.planId,
                                        responseName = user.userName,
                                        eventOutPut = plan.eventOutput,
                                        isLoopPlan = false
                                    }).ToList();
                    if (planList.Count() > 0)
                    {
                        checkPlanList.AddRange(planList);
                    }
                }
                else if (loopIds != null && loopIds.Length > 0)
                {
                    var loopList = (from plan in db.tblLoopPlan
                                    join user in db.tblUser on plan.responsibleUser equals user.userId into group1
                                    from user in group1.DefaultIfEmpty()
                                    where !plan.deleteFlag && planIds.Contains(plan.loopId)
                                    select new CheckPlanModel
                                    {
                                        planId = plan.loopId,
                                        responseName = user.userName,
                                        eventOutPut = plan.eventOutput,
                                        isLoopPlan = true
                                    }).ToList();
                    if (loopList.Count() > 0)
                    {
                        checkPlanList.AddRange(loopList);
                    }
                }
            }
            return checkPlanList;
        }

        #endregion

        public AddUserInfo GetAddUserInfoByUserId(int userid)
        {
            AddUserInfo AUserInfo = new AddUserInfo();
            List<OrganizationInfo> orgList = new List<OrganizationInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                List<StationInfo> OrgIdList = (from station in db.tblStation
                                               join us in db.tblUserStation on station.stationId equals us.stationId
                                               join user in db.tblUser on us.userId equals user.userId
                                               where user.userId == userid
                                               select new StationInfo
                                               {
                                                   orgId = station.organizationId
                                               }).ToList();
                foreach (var item in OrgIdList)
                {
                    OrganizationInfo orgInfo = new OrganizationInfo();
                    orgInfo.organizationName = GetOrgString(item.orgId);
                    orgInfo.organizationId = item.orgId;
                    orgList.Add(orgInfo);
                }
                AUserInfo.orgNameList = orgList;
                ProjectInfo projectInfo = new ProjectInfo();
                List<ProjectInfo> proList = new List<ProjectInfo>();
                UserInfo userIn = new UserInfo();
                projectInfo.projectId = 1;
                projectInfo.projectName = "初始计划";
                proList.Add(projectInfo);
                AUserInfo.projectList = proList;
                userIn.userId = userid;
                userIn.userName = db.tblUser.Where(p => p.userId == userid && !p.deleteFlag).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userid && !p.deleteFlag).FirstOrDefault().userName;
                AUserInfo.DownUser = userIn;
                var meStation = db.tblUserStation.Where(p => p.userId == userid).FirstOrDefault();
                var stationId = meStation == null ? 0 : meStation.stationId;
                //查询直接上级信息
                var parentUser = new UserInfo();
                //查询上级职位Id
                var parentStationId = db.tblStation.Where(p => p.stationId == stationId && !p.deleteFlag).FirstOrDefault() == null ? 0 : db.tblStation.Where(p => p.stationId == stationId && !p.deleteFlag).FirstOrDefault().parentStation;
                parentUser.userId = db.tblUserStation.Where(p => p.stationId == parentStationId).FirstOrDefault() == null ? 0 : db.tblUserStation.Where(p => p.stationId == parentStationId).FirstOrDefault().userId;
                parentUser.userName = db.tblUser.Where(p => p.userId == parentUser.userId && !p.deleteFlag).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == parentUser.userId && !p.deleteFlag).FirstOrDefault().userName;
                AUserInfo.UpUser = parentUser;
                return AUserInfo;
            }
        }

        #region 附件预览转化处理
        /// <summary>
        /// 附件预览转化处理
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public void ConvertPreviewFileAsync(string previewPath,int planId)
        {
            List<tblPlanAttachment> attachmentList = new List<tblPlanAttachment>();

            using (var db = new TargetNavigationDBEntities())
            {
                attachmentList = db.tblPlanAttachment.Where(x => x.planId == planId && x.deleteFlag == false).ToList();
            }

            if (attachmentList.Count() > 0)
            {
                foreach (var item in attachmentList)
                {
                    //System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!  AttachID: " + item.attachmentId.ToString() +"Start at :  "+ System.DateTime.Now.ToString());
                    MB.New.Common.FilePreview.ConvertFile(previewPath,MB.New.Common.EnumDefine.FileFolderType.PlanAttachment, item.saveName, item.extension);
                    //System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!!!!  AttachID: " + item.attachmentId.ToString() + "End at :  " + System.DateTime.Now.ToString());
                }

            }
        }

        #endregion


        #region 私有方法

        #region 循环操作表中插入数据

        /// <summary>
        /// 循环操作表中插入数据
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="loopId">循环计划Id</param>
        /// <param name="message">操作意见</param>
        /// <param name="result">操作类型</param>
        /// <param name="reviewUser">操作人</param>
        private void AddLoopPlanOperate(TargetNavigationDBEntities db, int loopId, string message, int result, int reviewUser)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblLoopplanOperate", obj).FirstOrDefault().Value;
            var loopOperate = new tblLoopplanOperate
            {
                operateId = operId,
                loopId = loopId,
                message = message,
                result = result,
                reviewUser = reviewUser,
                reviewTime = DateTime.Now
            };
            db.tblLoopplanOperate.Add(loopOperate);
        }

        #endregion

        #region 获取循环计划信息

        /// <summary>
        /// 获取循环计划信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="loopId"></param>
        /// <returns></returns>
        private tblLoopPlan GetLoopPlanInfo(TargetNavigationDBEntities db, int loopId)
        {
            return db.tblLoopPlan.Where(p => p.loopId == loopId && !p.deleteFlag).FirstOrDefault();
        }

        #endregion

        #region 计划日志操作

        /// <summary>
        /// 添加计划日志
        /// </summary>
        /// <param name="planId">计划Id</param>
        /// <param name="message">意见</param>
        /// <param name="result">操作类型</param>
        /// <param name="userId">操作用户</param>
        /// <param name="db">数据库上下文</param>
        private void AddPlanOperate(int planId, string message, int result, int userId, TargetNavigationDBEntities db)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblPlanOperate", obj).FirstOrDefault().Value;
            var planOperate = new tblPlanOperate
            {
                planId = planId,
                operateId = operId,
                message = message,
                result = result,
                reviewTime = DateTime.Now,
                reviewUser = userId
            };
            db.tblPlanOperate.Add(planOperate);
        }

        #endregion 计划日志操作

        #endregion
    }
}