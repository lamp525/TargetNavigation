﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MB.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;
    using System.Linq;
    
    public partial class TargetNavigationDBEntities : DbContext
    {
        public TargetNavigationDBEntities()
            : base("name=TargetNavigationDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tblAuth> tblAuth { get; set; }
        public DbSet<tblAuthResult> tblAuthResult { get; set; }
        public DbSet<tblBank> tblBank { get; set; }
        public DbSet<tblBankCard> tblBankCard { get; set; }
        public DbSet<tblBatchResult> tblBatchResult { get; set; }
        public DbSet<tblCalendar> tblCalendar { get; set; }
        public DbSet<tblCalendarUser> tblCalendarUser { get; set; }
        public DbSet<tblClient> tblClient { get; set; }
        public DbSet<tblCompanyDocument> tblCompanyDocument { get; set; }
        public DbSet<tblCompanyDocumentLog> tblCompanyDocumentLog { get; set; }
        public DbSet<tblControlItem> tblControlItem { get; set; }
        public DbSet<tblDelayRule> tblDelayRule { get; set; }
        public DbSet<tblDetailFormula> tblDetailFormula { get; set; }
        public DbSet<tblDistrict> tblDistrict { get; set; }
        public DbSet<tblDocumentShared> tblDocumentShared { get; set; }
        public DbSet<tblEntrustResult> tblEntrustResult { get; set; }
        public DbSet<tblEquipment> tblEquipment { get; set; }
        public DbSet<tblExecutionMode> tblExecutionMode { get; set; }
        public DbSet<tblFlowEntrust> tblFlowEntrust { get; set; }
        public DbSet<tblFlowNode> tblFlowNode { get; set; }
        public DbSet<tblFolderAuth> tblFolderAuth { get; set; }
        public DbSet<tblFormContent> tblFormContent { get; set; }
        public DbSet<tblFormDetail> tblFormDetail { get; set; }
        public DbSet<tblFormDuplicate> tblFormDuplicate { get; set; }
        public DbSet<tblFormFlow> tblFormFlow { get; set; }
        public DbSet<tblImage> tblImage { get; set; }
        public DbSet<tblImContacts> tblImContacts { get; set; }
        public DbSet<tblImGroups> tblImGroups { get; set; }
        public DbSet<tblImGroupUser> tblImGroupUser { get; set; }
        public DbSet<tblImLogin> tblImLogin { get; set; }
        public DbSet<tblImMessages> tblImMessages { get; set; }
        public DbSet<tblIndexDocument> tblIndexDocument { get; set; }
        public DbSet<tblIndexImage> tblIndexImage { get; set; }
        public DbSet<tblIndexModule> tblIndexModule { get; set; }
        public DbSet<tblIndexNews> tblIndexNews { get; set; }
        public DbSet<tblIndexNotice> tblIndexNotice { get; set; }
        public DbSet<tblIndexStatistics> tblIndexStatistics { get; set; }
        public DbSet<tblLinkCondition> tblLinkCondition { get; set; }
        public DbSet<tblLinkFormula> tblLinkFormula { get; set; }
        public DbSet<tblLinkResult> tblLinkResult { get; set; }
        public DbSet<tblLogin> tblLogin { get; set; }
        public DbSet<tblLoopplanAttachment> tblLoopplanAttachment { get; set; }
        public DbSet<tblLoopplanOperate> tblLoopplanOperate { get; set; }
        public DbSet<tblMeetingAttachment> tblMeetingAttachment { get; set; }
        public DbSet<tblMeetingMember> tblMeetingMember { get; set; }
        public DbSet<tblMeetingRoom> tblMeetingRoom { get; set; }
        public DbSet<tblMeetingRoomAppointment> tblMeetingRoomAppointment { get; set; }
        public DbSet<tblMeetingRoomEquipment> tblMeetingRoomEquipment { get; set; }
        public DbSet<tblMonthPlanCompleteStatistics> tblMonthPlanCompleteStatistics { get; set; }
        public DbSet<tblMonthTargetCompleteStatistics> tblMonthTargetCompleteStatistics { get; set; }
        public DbSet<tblNews> tblNews { get; set; }
        public DbSet<tblNewsDirectory> tblNewsDirectory { get; set; }
        public DbSet<tblNodeField> tblNodeField { get; set; }
        public DbSet<tblNodeLink> tblNodeLink { get; set; }
        public DbSet<tblNodeOperate> tblNodeOperate { get; set; }
        public DbSet<tblNoticeDirectory> tblNoticeDirectory { get; set; }
        public DbSet<tblObjective> tblObjective { get; set; }
        public DbSet<tblObjectiveChange> tblObjectiveChange { get; set; }
        public DbSet<tblObjectiveDocument> tblObjectiveDocument { get; set; }
        public DbSet<tblObjectiveFormula> tblObjectiveFormula { get; set; }
        public DbSet<tblObjectiveOperate> tblObjectiveOperate { get; set; }
        public DbSet<tblOperateResult> tblOperateResult { get; set; }
        public DbSet<tblOrganization> tblOrganization { get; set; }
        public DbSet<tblOrgDayWorkTime> tblOrgDayWorkTime { get; set; }
        public DbSet<tblOrgMonthWorkTime> tblOrgMonthWorkTime { get; set; }
        public DbSet<tblPerDayWorkTime> tblPerDayWorkTime { get; set; }
        public DbSet<tblPerMonthWorkTime> tblPerMonthWorkTime { get; set; }
        public DbSet<tblPersonalSetting> tblPersonalSetting { get; set; }
        public DbSet<tblPlan> tblPlan { get; set; }
        public DbSet<tblPlanAttachment> tblPlanAttachment { get; set; }
        public DbSet<tblPlanCompleteStatistics> tblPlanCompleteStatistics { get; set; }
        public DbSet<tblPlanCooperation> tblPlanCooperation { get; set; }
        public DbSet<tblPlanFront> tblPlanFront { get; set; }
        public DbSet<tblPlanObjective> tblPlanObjective { get; set; }
        public DbSet<tblPlanOperate> tblPlanOperate { get; set; }
        public DbSet<tblPlanStatistics> tblPlanStatistics { get; set; }
        public DbSet<tblPlanStatisticsOrg> tblPlanStatisticsOrg { get; set; }
        public DbSet<tblPlanSuggestion> tblPlanSuggestion { get; set; }
        public DbSet<tblPrimary> tblPrimary { get; set; }
        public DbSet<tblProject> tblProject { get; set; }
        public DbSet<tblProvince> tblProvince { get; set; }
        public DbSet<tblRewardPunish> tblRewardPunish { get; set; }
        public DbSet<tblSuggestionAttachment> tblSuggestionAttachment { get; set; }
        public DbSet<tblTemplate> tblTemplate { get; set; }
        public DbSet<tblTemplateCategory> tblTemplateCategory { get; set; }
        public DbSet<tblTemplateControl> tblTemplateControl { get; set; }
        public DbSet<tblUserContacts> tblUserContacts { get; set; }
        public DbSet<tblUserDocument> tblUserDocument { get; set; }
        public DbSet<tblUserForm> tblUserForm { get; set; }
        public DbSet<tblUserProject> tblUserProject { get; set; }
        public DbSet<tblUserStation> tblUserStation { get; set; }
        public DbSet<tblValueIncentive> tblValueIncentive { get; set; }
        public DbSet<tblValueIncentiveCustom> tblValueIncentiveCustom { get; set; }
        public DbSet<tblWeekPlanCompleteStatistics> tblWeekPlanCompleteStatistics { get; set; }
        public DbSet<tblWeekTargetCompleteStatistics> tblWeekTargetCompleteStatistics { get; set; }
        public DbSet<tblYearPlanCompleteStatistics> tblYearPlanCompleteStatistics { get; set; }
        public DbSet<tblYearTargetCompleteStatistics> tblYearTargetCompleteStatistics { get; set; }
        public DbSet<tblStation> tblStation { get; set; }
        public DbSet<tblCity> tblCity { get; set; }
        public DbSet<vOrgPersonMonthWorkTime> vOrgPersonMonthWorkTime { get; set; }
        public DbSet<vOrgPersonWeekWorkTime> vOrgPersonWeekWorkTime { get; set; }
        public DbSet<tblVersion> tblVersion { get; set; }
        public DbSet<tblUser> tblUser { get; set; }
        public DbSet<tblHoliday> tblHoliday { get; set; }
        public DbSet<tblLoopPlan> tblLoopPlan { get; set; }
        public DbSet<vPlanList> vPlanList { get; set; }
        public DbSet<tblLoopPlanCooperation> tblLoopPlanCooperation { get; set; }
        public DbSet<vLoopPlanList> vLoopPlanList { get; set; }
        public DbSet<tblLoopplanSubmit> tblLoopplanSubmit { get; set; }
    
        public virtual ObjectResult<worktimeindex> prcDepartmentIndex(Nullable<System.DateTime> time, string size)
        {
            var timeParameter = time.HasValue ?
                new ObjectParameter("time", time) :
                new ObjectParameter("time", typeof(System.DateTime));
    
            var sizeParameter = size != null ?
                new ObjectParameter("size", size) :
                new ObjectParameter("size", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<worktimeindex>("prcDepartmentIndex", timeParameter, sizeParameter);
        }
    
        public virtual ObjectResult<worktime> prcDepartmentlWorktime(Nullable<int> organizationId, Nullable<System.DateTime> time, string size)
        {
            var organizationIdParameter = organizationId.HasValue ?
                new ObjectParameter("organizationId", organizationId) :
                new ObjectParameter("organizationId", typeof(int));
    
            var timeParameter = time.HasValue ?
                new ObjectParameter("time", time) :
                new ObjectParameter("time", typeof(System.DateTime));
    
            var sizeParameter = size != null ?
                new ObjectParameter("size", size) :
                new ObjectParameter("size", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<worktime>("prcDepartmentlWorktime", organizationIdParameter, timeParameter, sizeParameter);
        }
    
        public virtual ObjectResult<worktime> prcPersonalWorktime(Nullable<int> responsibleuser, Nullable<System.DateTime> time, string size)
        {
            var responsibleuserParameter = responsibleuser.HasValue ?
                new ObjectParameter("responsibleuser", responsibleuser) :
                new ObjectParameter("responsibleuser", typeof(int));
    
            var timeParameter = time.HasValue ?
                new ObjectParameter("time", time) :
                new ObjectParameter("time", typeof(System.DateTime));
    
            var sizeParameter = size != null ?
                new ObjectParameter("size", size) :
                new ObjectParameter("size", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<worktime>("prcPersonalWorktime", responsibleuserParameter, timeParameter, sizeParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> prcGetPrimaryKey(string tableName, ObjectParameter num)
        {
            var tableNameParameter = tableName != null ?
                new ObjectParameter("tableName", tableName) :
                new ObjectParameter("tableName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("prcGetPrimaryKey", tableNameParameter, num);
        }
    
        public virtual ObjectResult<worktimeindex> prcDepartmentIndexAverage(Nullable<System.DateTime> time, string size)
        {
            var timeParameter = time.HasValue ?
                new ObjectParameter("time", time) :
                new ObjectParameter("time", typeof(System.DateTime));
    
            var sizeParameter = size != null ?
                new ObjectParameter("size", size) :
                new ObjectParameter("size", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<worktimeindex>("prcDepartmentIndexAverage", timeParameter, sizeParameter);
        }
    
        public virtual int prcDocumentDelete(Nullable<int> documentId, Nullable<int> userId, Nullable<System.DateTime> nowTime, Nullable<int> flag)
        {
            var documentIdParameter = documentId.HasValue ?
                new ObjectParameter("documentId", documentId) :
                new ObjectParameter("documentId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var nowTimeParameter = nowTime.HasValue ?
                new ObjectParameter("nowTime", nowTime) :
                new ObjectParameter("nowTime", typeof(System.DateTime));
    
            var flagParameter = flag.HasValue ?
                new ObjectParameter("flag", flag) :
                new ObjectParameter("flag", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prcDocumentDelete", documentIdParameter, userIdParameter, nowTimeParameter, flagParameter);
        }
    
        public virtual ObjectResult<NewDocumentModel> prcGetCompanyListNoAuthy(string condition, string orderString)
        {
            var conditionParameter = condition != null ?
                new ObjectParameter("condition", condition) :
                new ObjectParameter("condition", typeof(string));
    
            var orderStringParameter = orderString != null ?
                new ObjectParameter("orderString", orderString) :
                new ObjectParameter("orderString", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NewDocumentModel>("prcGetCompanyListNoAuthy", conditionParameter, orderStringParameter);
        }
    
        public virtual ObjectResult<NewDocumentModel> prcGetCompanyListWithAuthy(string userId, string condition, string orderString)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var conditionParameter = condition != null ?
                new ObjectParameter("condition", condition) :
                new ObjectParameter("condition", typeof(string));
    
            var orderStringParameter = orderString != null ?
                new ObjectParameter("orderString", orderString) :
                new ObjectParameter("orderString", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NewDocumentModel>("prcGetCompanyListWithAuthy", userIdParameter, conditionParameter, orderStringParameter);
        }
    
        public virtual ObjectResult<NewDocumentModel> prcGetUserDocumentList(string type, string userId, string condition, string orderString)
        {
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var conditionParameter = condition != null ?
                new ObjectParameter("condition", condition) :
                new ObjectParameter("condition", typeof(string));
    
            var orderStringParameter = orderString != null ?
                new ObjectParameter("orderString", orderString) :
                new ObjectParameter("orderString", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NewDocumentModel>("prcGetUserDocumentList", typeParameter, userIdParameter, conditionParameter, orderStringParameter);
        }
    
        public virtual int prcObjectiveDelete(Nullable<int> objectiveId, Nullable<int> userId, Nullable<System.DateTime> nowTime)
        {
            var objectiveIdParameter = objectiveId.HasValue ?
                new ObjectParameter("objectiveId", objectiveId) :
                new ObjectParameter("objectiveId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var nowTimeParameter = nowTime.HasValue ?
                new ObjectParameter("nowTime", nowTime) :
                new ObjectParameter("nowTime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prcObjectiveDelete", objectiveIdParameter, userIdParameter, nowTimeParameter);
        }
    
        public virtual int prcPlanGetByPageInt()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prcPlanGetByPageInt");
        }
    
        [EdmFunction("TargetNavigationDBEntities", "fGetParentOrgByOrgId")]
        public virtual IQueryable<fGetParentOrgByOrgId_Result> fGetParentOrgByOrgId(Nullable<int> orgId, Nullable<int> level)
        {
            var orgIdParameter = orgId.HasValue ?
                new ObjectParameter("orgId", orgId) :
                new ObjectParameter("orgId", typeof(int));
    
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fGetParentOrgByOrgId_Result>("[TargetNavigationDBEntities].[fGetParentOrgByOrgId](@orgId, @level)", orgIdParameter, levelParameter);
        }
    
        [EdmFunction("TargetNavigationDBEntities", "fGetParentOrgByStationId")]
        public virtual IQueryable<fGetParentOrgByStationId_Result> fGetParentOrgByStationId(Nullable<int> stationId, Nullable<int> level)
        {
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fGetParentOrgByStationId_Result>("[TargetNavigationDBEntities].[fGetParentOrgByStationId](@stationId, @level)", stationIdParameter, levelParameter);
        }
    
        [EdmFunction("TargetNavigationDBEntities", "fGetParentStationByStationId")]
        public virtual IQueryable<fGetParentStationByStationId_Result> fGetParentStationByStationId(Nullable<int> stationId, Nullable<int> level)
        {
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fGetParentStationByStationId_Result>("[TargetNavigationDBEntities].[fGetParentStationByStationId](@stationId, @level)", stationIdParameter, levelParameter);
        }
    
        [EdmFunction("TargetNavigationDBEntities", "fGetSubOrgByOrgId")]
        public virtual IQueryable<fGetSubOrgByOrgId_Result> fGetSubOrgByOrgId(Nullable<int> orgId, Nullable<int> level)
        {
            var orgIdParameter = orgId.HasValue ?
                new ObjectParameter("orgId", orgId) :
                new ObjectParameter("orgId", typeof(int));
    
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fGetSubOrgByOrgId_Result>("[TargetNavigationDBEntities].[fGetSubOrgByOrgId](@orgId, @level)", orgIdParameter, levelParameter);
        }
    
        [EdmFunction("TargetNavigationDBEntities", "fGetSubStationByStationId")]
        public virtual IQueryable<fGetSubStationByStationId_Result> fGetSubStationByStationId(Nullable<int> stationId, Nullable<int> level)
        {
            var stationIdParameter = stationId.HasValue ?
                new ObjectParameter("stationId", stationId) :
                new ObjectParameter("stationId", typeof(int));
    
            var levelParameter = level.HasValue ?
                new ObjectParameter("level", level) :
                new ObjectParameter("level", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fGetSubStationByStationId_Result>("[TargetNavigationDBEntities].[fGetSubStationByStationId](@stationId, @level)", stationIdParameter, levelParameter);
        }
    }
}