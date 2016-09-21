using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface IPlanBLL
    {
        /// 查询计划信息列表（包括循环计划）
        List<PlanInfo> GetPlanList(int userId, string condition, string coopCondition, DateTime start, DateTime end);

        List<PlanInfo> GetPlanListDESC(int userId);

        /// 获取循环计划列表
        List<PlanInfo> GetLoopPlanList(int userId, string condition, DateTime start, DateTime end, int flag);

        /// 查询下属计划
        List<PlanInfo> GetUnderPlanList(int userId, string condition);

        /// 自定义排序
        List<PlanInfo> GetPlanListOrderByCustom(List<Sort> orderList, List<PlanInfo> planList);

        /// 获取组织信息==>递归查询
        List<OrganizationInfo> GetOrgListByOrgId(int? OrganizationId, ref List<OrganizationInfo> orgInfo);

        /// 获取组织信息==>递归查询
        string GetOrgString(int? OrganizationId);

        string GetOrgStringByOrgId(TargetNavigationDBEntities db, int? OrganizationId, List<string> orgStringList);

        List<string> GetMoreOrgNameById(int? orgId, ref List<OrganizationInfo> OrgNameList, ref int count);

        /// 获取项目信息==>递归查询
        string GetPorString(int? projectId);

        string GetProListByOrgId(TargetNavigationDBEntities db, int? projectId, List<string> proList);
        /// <summary>
        /// 获取用户首页饼图数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="userId"></param>
        /// <param name="isweek"></param>
        /// <returns></returns>
        List<PlanStatusCount> GetPlanStatusByUserIndex(int year, int month, int userId, int isweek);
        /// 获取最近5条计划===（根据创建时间排序）
        List<OrganizationInfo> GetOrgId(int userId);

        /// 获取所有组织信息-->parentId=0;
        List<OrganizationInfo> GetAllOrg();

        /// 获取组织信息==>递归查询
        List<string> GetLastOrgListByOrgId(int? organizationId, List<string> orgList);

        /// 获取最近5条计划===（根据创建时间排序）
        List<PlanInfo> GetProId(int userId);

        /// 获取所有项目信息-->第一层
        List<ProjectInfo> GetAllPro();

        /// 获取组织信息==>递归查询
        List<string> GetLastProListByProId(int? projectId, List<string> proList);

        /// 新建计划
        Dictionary<int, int> Save(List<NewPlan> newPlan, int userid, int status);

        /// 新建子计划
        void AddChildren(List<NewPlan> newPlan, int userid, int parentPlanId, int status, TargetNavigationDBEntities db);

        /// 添加一条计划信息 void AddPlan(int userId, PlanInfo planInfo, TargetNavigationDBEntities db)

        /// 添加计划前提表
        void AddPlanFront(int planId, int planFrontId, int userid, TargetNavigationDBEntities db);

        /// 添加计划协作表
        void AddPlanCooperation(int planId, int partnerId, int userid, TargetNavigationDBEntities db);

        /// 根据计划删除协作计划;
        void DeletePlanCooperation(int planId, int userId);

        /// 添加一条循环计划
        void AddLoopPlan(int userId, LoopPlanInfo loopPlanInfo, TargetNavigationDBEntities db);

        /// 根据Id查询计划信息通用方法
        tblPlan GetPlanInfoById(int planId, TargetNavigationDBEntities db);

        /// 修改状态的操作: 审核，确认，撤销，修改
        int ChangePlanStatus(int planId, int status, int userId, int flag);

        /// 提交操作
        int SubmitPlan(int userId, int planId, int initial);

        /// 计划提交确认
        ReturnConfirm Confirming(int userId, int planId, int quantity/*完成数量(自评)*/, int time /*完成时间(自评)*/);

        /// 计划转办
        bool ChangePlan(int planId, int userId, int responseUser, int confirmUser);

        /// 审核操作
        int ExaminePlan(int planId, int? importance, int? urgency, int? difficulty, bool isTure, int userId, string msg);

        /// 循环计划审批
        bool ExamineLoopPlan(int loopId, int? importance, int? urgency, int? difficulty, string message, bool type, int userId);

        /// 计划确认
        void ConfirmPlan(int planId, int useId, decimal? completeQuantity, decimal? completeQuality, decimal? completeTime, bool isTrue, string msg);

        /// 循环计划确认
        bool ConfirmLoopPlan(int submitId, int loopId, decimal? completeQuantity, decimal? completeQuality, decimal? completeTime, string message, bool type, int userId);

        /// 添加附件
        void AddPlanAttachment(List<string> planAttachments, int planIdNew, int useId);

        /// 即时更新计划进
        void UpdateProcess(int userId, int planId, int newProcess);

        /// 计划分解
        void UpdateParentPlan(int parentPlanId, int userId);

        /// 中止计划或者开始计划:0、运行中   90、已中止
        int StopOrStartPlan(int planId, int stop, int userId);

        /// 筛选计划
        List<PlanInfo> FilterPlan(int[] status, DateTime startTime, DateTime endTime, int[] userIds, int[] organizationIds, int[] projectIds, List<PlanInfo> planList);

        /// 根据时间查询计划状态数量
        List<PlanStatusCount> GetPlanStatusInfo(int year, int month, int userId, int operate);

        void AddEmptyCount(ref List<PlanStatusCount> planCountList, int status, string statusName);

        /// 根据用户Id查询用户信息
        UserInfo GetUserInfoById(int userId);

        /// 统计用户的今日未完成计划和超时计划的数量
        UserPlanCountInfo StatisticsUserPlan(int userId);

        /// 根据PlanId获取计划操作
        List<PlanOperateInfo> GetPlanOperateByPlanID(int planId);

        /// 归类到计划
        void ClassificatePlan(int planId, int parentPlanId);

        /// 根据计划Id获取该计划的前提计划
        List<FrontPlan> GetFrontPlan(int planId);

        /// 根据用户Id查询上级，自己以及下属的用户信息
        List<UserInfo> GetUserIdListByUserId(int userId);

        List<UserInfo> GetUserIdUpListByUserId(int userId);

        List<UserInfo> GetUserIdDownListByUserId(int userId);

        /// 输入任意数字进行模糊查询
        List<UserInfo> SelectUserList(string word, bool hasImage);

        List<PlanInfo> SelectFrontPlanList(string word);

        /// 绑定部门的组织架构
        List<OrganizationModel> GetDepartmentList();

        /// 递归绑定子部门组织架构
        List<OrganizationModel> GetChildDepartList(List<OrganizationModel> parentlist, TargetNavigationDBEntities db);

        /// 绑定项目的组织架构
        List<ProjectModel> GetProjectList();

        /// 递归绑定子项目组织架构
        List<ProjectModel> GetChildProjectList(List<ProjectModel> parentlist, TargetNavigationDBEntities db);

        /// 获取执行方式信息
        List<ExecutionModel> GetExecutionList();

        /// 获取前提计划信息
        List<PlanInfo> GetFrontPlanInfo(string selectText, int organizationId, int projectId, int responsibleUser);

        /// 根据计划Id查询附件信息
        List<PlanAttachment> GetPlanAttachmentList(int planId);

        /// 根据父计划Id查询子计划集合
        List<ChildPlanInfo> GetChildPlanList(int planId);

        List<PlanInfo> GetplanInfoByIdList(List<int> planId);

        /// 根据ID获取计划详情
        PlanInfo GetPlanInfoByPlanId(int planId, bool isloop, bool withfront, bool collPlan, int? submintId);

        /// 获取计划的所有协作人信息
        List<CollPlan> GetCollPlanUsers(int planId, TargetNavigationDBEntities db);

        /// 根据计划Id获取计划审核信息
        PlanCheckInfo GetPlanCheckInfo(int planId);

        /// 删除计划
        void DeletePlan(int planId, int userId);

        /// 获取计划的自评数量和时间
        PlanCheckInfo GetPlanConfirmInfo(int planId);

        /// 分解计划
        void ResolvePlan(PlanInfo planInfo, int userId);

        /// 获取计划日志
        List<PlanOperateModel> GetOperates(int planId, bool isloop);

        /// 获取计划日志
        List<PlanOperateModel> GetPlanOperates(int planId);

        /// 根据ID获取循环计划操作日志
        List<PlanOperateModel> GetLoopPlanOperates(int loopId);

        /// 保存计划
        void SavePlans(List<PlanInfo> planList, int userId);

        /// 获取评论信息
        List<DiscussModel> GetDiscussList(int planId);

        /// 添加评论
        DiscussModel AddDiscuss(string content, int planId, int userId);

        /// 添加回复
        DiscussModel AddReplySuggestion(int planId, int userId, int replyUser, string replyUserName, string content);

        /// 计划附件上传
        PlanAttachment UplpadMultipleFiles(HttpPostedFileBase hpf, int planId, int userId);

        /// 删除计划附件
        void DeleteFile(int id);

        /// 根据计划ID获取计划附件表附件信息
        List<PlanAttachment> PlanFileData(int planId);

        /// 判断是否是执行力管理员
        bool GetExecution(int userId);

        /// <summary>
        /// 获取用户设置的每页显示数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetPageSize(int userId);

        /// 根据用户Id查询头像信息
        string GetUserImg(int userId);

        /// 删除评论
        void DeleteComment(int suggestionId);

        //计划归类
        tblPlan GetBytblPlanId(int planid, TargetNavigationDBEntities db);

        void GetplanToParentPlan(int ParentPlanId, List<PlanIdList> planId);

        AddUserInfo GetAddUserInfoByUserId(int userid);

        //获取计划日程化批量操作的计划列表
        List<CheckPlanModel> GetPlanSimpleList(int?[] planIds, int?[] loopIds);

        //循环计划提交确认
        void ConfirmingLoopPlan(int loopId, int quantity, int userId);

        //获取循环计划附件列表
        List<PlanAttachment> GetLoopPlanFileData(int loopId);

        //上传循环计划附件
        PlanAttachment LoopUplpadMultipleFiles(HttpPostedFileBase hpf, int loopId, int userId);

        //删除循环计划附件
        void DeleteLoopFile(int id);

        //中止循环计划
        bool StopLoopPlan(int loopId, int userId);

        //循环计划申请修改
        bool UpdateLoopPlan(int loopId, int userId);

        //提交循环计划
        bool SubmitLoopPlan(int loopId, int userId);

        //获取计划日程化计划列表
        List<PlanInfo> GetCalendarPlanList(int userId, string condition, string coopCondition, DateTime start, DateTime end);
        /// <summary>
        /// 根据Id返回确认人
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        int GetPlanById(int planId);

        /// <summary>
        /// 附件预览转化处理
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        void ConvertPreviewFileAsync(string previewPath,int planId);
    }
}