using MB.New.Common;
using System;
using System.Collections.Generic;

namespace MB.Facade.Plan
{
    /// <summary>
    /// 计划信息
    /// </summary>
    public class PagePlanInfoModel
    {
        /// <summary>
        /// 计划ID
        /// </summary>
        public int? planId { get; set; }

        /// <summary>
        /// 是否是协作计划：1、是 0、否
        /// </summary>
        public bool? IsCollPlan { get; set; }

        /// <summary>
        /// 是否是循环计划:1、是 0、否
        /// </summary>
        public bool? isLoop { get; set; }

        /// <summary>
        /// 有无附件
        /// </summary>
        public bool? isAttach { get; set; }

        /// <summary>
        /// 计划结果
        /// </summary>
        public string result { get; set; }

        /// <summary>
        /// 提交标志 0：保存 1：提交
        /// </summary>
        public bool? isSubmit { get; set; }

        /// <summary>
        /// 父计划ID
        /// </summary>
        public int? parentPlan { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int? organizationId { get; set; }

        /// <summary>
        /// 部门信息（包含上级）
        /// </summary>
        public string organizationInfo { get; set; }

        /// <summary>
        /// 事项输出结果
        /// </summary>
        public string eventOutput { get; set; }

        /// <summary>
        /// 执行方式
        /// </summary>
        public string executionMode { get; set; }

        /// <summary>
        /// 执行方式ID
        /// </summary>
        public int? executionModeId { get; set; }

        /// <summary>
        /// 责任人部门ID
        /// </summary>
        public int? responsibleOrganization { get; set; }

        /// <summary>
        /// 责任人ID
        /// </summary>
        public int? responsibleUser { get; set; }

        /// <summary>
        /// 责任人名
        /// </summary>
        public string responsibleUserName { get; set; }

        /// <summary>
        /// 责任人头像
        /// </summary>
        public string responsibleUserImage { get; set; }

        /// <summary>
        /// 确认人部门ID
        /// </summary>
        public int? confirmOrganization { get; set; }

        /// <summary>
        /// 确认人ID
        /// </summary>
        public int? confirmUser { get; set; }

        /// <summary>
        /// 确认人名
        /// </summary>
        public string confirmUserName { get; set; }

        /// <summary>
        /// 确认人头像
        /// </summary>
        public string confirmUserImage { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }

        /// <summary>
        /// 所需工时（天）
        /// </summary>
        public int? workTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 提醒次数
        /// </summary>
        public int? alert { get; set; }

        /// <summary>
        /// 重要度
        /// </summary>
        public int? importance { get; set; }

        /// <summary>
        /// 紧急度
        /// </summary>
        public int? urgency { get; set; }

        /// <summary>
        /// 难易度
        /// </summary>
        public int? difficulty { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        public int? progress { get; set; }

        /// <summary>
        /// 完成数量（件数）
        /// </summary>
        public int? quantity { get; set; }

        /// <summary>
        /// 完成时间（分钟）
        /// </summary>
        public int? time { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public decimal? completeQuantity { get; set; }

        /// <summary>
        /// 完成质量
        /// </summary>
        public decimal? completeQuality { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public decimal? completeTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EnumDefine.PlanStatus status { get; set; }

        /// <summary>
        /// 中止状态
        /// </summary>
        public EnumDefine.PlanStopStatus stop { get; set; }

        /// <summary>
        /// 计划审核时间
        /// </summary>
        public DateTime? auditTime { get; set; }

        /// <summary>
        /// 计划创建提交
        /// </summary>
        public DateTime? planGenerateTime { get; set; }

        /// <summary>
        /// 计划确认时间
        /// </summary>
        public DateTime? confirmTime { get; set; }

        /// <summary>
        /// 0.临时计划
        /// 1.目标计划
        /// </summary>
        public int? initial { get; set; }

        /// <summary>
        /// 有子计划
        /// </summary>
        public bool? withSub { get; set; }

        /// <summary>
        /// 归档标志
        /// </summary>
        public bool? archive { get; set; }

        /// <summary>
        /// 归档时间
        /// </summary>
        public DateTime? archiveTime { get; set; }

        /// <summary>
        /// 有前提计划
        /// </summary>
        public bool? withFront { get; set; }

        /// <summary>
        /// 自动开始
        /// </summary>
        public bool? autoStart { get; set; }

        /// <summary>
        /// 关联会议ID
        /// </summary>
        public int? meetingId { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public int createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 更新用户ID
        /// </summary>
        public int updateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }

        /// <summary>
        /// 有效工时
        /// </summary>
        public decimal? effectiveTime { get; set; }

        /// <summary>
        /// 实际工时
        /// </summary>
        public decimal? realTime { get; set; }

        /// <summary>
        /// 协作人信息
        /// </summary>
        public List<PagePartnerModel> partnerInfo { get; set; }

        ///<summary>
        ///标签
        ///</summary>
        public string[] keyword { get; set; }
    }

    /// <summary>
    /// 计划日志
    /// </summary>
    public class PagePlanLogInfoModel
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public string operateUser { get; set; }

        /// <summary>
        /// 操作日志
        /// </summary>
        public string operateInfo { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime operateTime { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string message { get; set; }
    }

    /// <summary>
    /// 计划评论
    /// </summary>
    public class PagePlanCommentModel
    {
        /// <summary>
        /// 意见ID
        /// </summary>
        public int? suggestionId { get; set; }

        /// <summary>
        /// 计划ID
        /// </summary>
        public int? planId { get; set; }

        /// <summary>
        /// 意见
        /// </summary>
        public string suggestion { get; set; }

        /// <summary>
        /// 回复人
        /// </summary>
        public int? replyUser { get; set; }

        /// <summary>
        /// 回复人名
        /// </summary>
        public string replyUserName { get; set; }

        /// <summary>
        /// 回复人头像
        /// </summary>
        public string replyUserImage { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int? createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int? updateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? updateTime { get; set; }
    }

    /// <summary>
    /// 计划协作人
    /// </summary>
    public class PagePartnerModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 截取后头像
        /// </summary>
        public string headImage { get; set; }
    }

    /// <summary>
    /// 计划默认信息
    /// </summary>
    public class PagePlanDefaultInfoModel
    {
        public int? organizationId { get; set; }

        public string organizationInfo { get; set; }

        public int? executionModeId { get; set; }

        public string executionMode { get; set; }

        public int? responsibleUser { get; set; }

        public string responsibleUserName { get; set; }

        public string responsibleUserImage { get; set; }

        public int? confirmUser { get; set; }

        public string confirmUserName { get; set; }

        public string confirmUserImage { get; set; }

        public int? initial { get; set; }
    }

    /// <summary>
    /// 循环计划信息
    /// </summary>
    public class PageLoopPlanInfoModel
    {
        /// <summary>
        /// 循环计划ID
        /// </summary>
        public int? loopId { get; set; }

        /// <summary>
        /// 提交标志 0：保存 1：提交
        /// </summary>
        public bool? isSubmit { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int? organizationId { get; set; }

        /// <summary>
        /// 部门信息（包含上级）
        /// </summary>
        public string organizationInfo { get; set; }

        /// <summary>
        /// 事项输出结果
        /// </summary>
        public string eventOutput { get; set; }

        /// <summary>
        /// 执行方式
        /// </summary>
        public string executionMode { get; set; }

        /// <summary>
        /// 执行方式ID
        /// </summary>
        public int? executionModeId { get; set; }

        /// <summary>
        /// 责任人部门ID
        /// </summary>
        public int? responsibleOrganization { get; set; }

        /// <summary>
        /// 责任人ID
        /// </summary>
        public int? responsibleUser { get; set; }

        /// <summary>
        /// 责任人名
        /// </summary>
        public string responsibleUserName { get; set; }

        /// <summary>
        /// 责任人头像
        /// </summary>
        public string responsibleUserImage { get; set; }

        /// <summary>
        /// 确认人部门ID
        /// </summary>
        public int? confirmOrganization { get; set; }

        /// <summary>
        /// 确认人ID
        /// </summary>
        public int? confirmUser { get; set; }

        /// <summary>
        /// 确认人名
        /// </summary>
        public string confirmUserName { get; set; }

        /// <summary>
        /// 确认人头像
        /// </summary>
        public string confirmUserImage { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EnumDefine.LoopPlanStatus status { get; set; }

        /// <summary>
        /// 中止状态
        /// </summary>
        public EnumDefine.LoopPlanStopStatus stop { get; set; }

        /// <summary>
        /// 循环类型
        /// </summary>
        public EnumDefine.LoopPlanLoopType loopType { get; set; }

        /// <summary>
        /// 循环标志
        /// </summary>
        public bool? loopStatus { get; set; }

        /// <summary>
        /// 重要度
        /// </summary>
        public int? importance { get; set; }

        /// <summary>
        /// 紧急度
        /// </summary>
        public int? urgency { get; set; }

        /// <summary>
        /// 难易度
        /// </summary>
        public int? difficulty { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public int createUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 更新用户ID
        /// </summary>
        public int updateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }

        public bool deleteFlag { get; set; }
        public string loopYear { get; set; }
        public string loopMonth { get; set; }
        public string loopWeek { get; set; }
        public string loopDay { get; set; }
        public string loopTime { get; set; }

        ///<summary>
        ///标签数组
        ///</summary>
        public string[] keyword { get; set; }

        /// <summary>
        /// 标签字符串信息
        /// </summary>
        public string keywordInfo { get; set; }

        /// <summary>
        /// 提交次数
        /// </summary>
        public int? submitCount { get; set; }
    }

    /// <summary>
    /// 循环计划提交信息
    /// </summary>
    public class PageLoopPlanSubmitInfoModel
    {
        /// <summary>提交ID</summary>
        public int? submitId { get; set; }

        /// <summary>循环计划ID</summary>
        public int? loopId { get; set; }

        /// <summary>单位时间</summary>
        public int? time { get; set; }

        /// <summary>完成数量</summary>
        public int? quantity { get; set; }

        /// <summary>状态</summary>
        public EnumDefine.LoopPlanSubmitStatus completeStatus { get; set; }

        /// <summary>中止状态 0：不需要做 1：需要做</summary>
        public bool? undo { get; set; }

        /// <summary>有无附件</summary>
        public bool? isAttach { get; set; }

        /// <summary>内容输出</summary>
        public string result { get; set; }

        public decimal? completeQuantity { get; set; }

        public decimal? completeQuality { get; set; }

        public decimal? completeTime { get; set; }

        public DateTime? executeTime { get; set; }

        public DateTime? submitTime { get; set; }

        public DateTime? confirmTime { get; set; }

        public int? createUser { get; set; }

        public DateTime? createTime { get; set; }

        public int? updateUser { get; set; }

        public DateTime? updateTime { get; set; }
    }

    /// <summary>
    /// 循环计划默认信息
    /// </summary>
    public class PageLoopPlanDefaultInfoModel
    {
        public int? organizationId { get; set; }

        public string organizationInfo { get; set; }

        public int? executionModeId { get; set; }

        public string executionMode { get; set; }

        public int? responsibleUser { get; set; }

        public string responsibleUserName { get; set; }

        public string responsibleUserImage { get; set; }

        public int? confirmUser { get; set; }

        public string confirmUserName { get; set; }

        public string confirmUserImage { get; set; }

        public EnumDefine.LoopPlanLoopType loopType { get; set; }
    }

    /// <summary>
    /// 执行方式
    /// </summary>
    public class PageExecutionInfoModel
    {
        public int executionId { get; set; }

        public string executionMode { get; set; }
    }

    /// <summary>
    /// 快速添加计划信息
    /// </summary>
    public class PageQuickAddModel
    {
        public DateTime? endTime { get; set; }

        public string eventOutput { get; set; }
    }

    /// <summary>
    /// 计划状态数量
    /// </summary>
    public class PagePlanStatusCountModel
    {
        /// <summary>
        /// 一般计划待提交数量
        /// </summary>
        public int planSubmitingCount { get; set; }

        /// <summary>
        /// 一般计划待审核数量
        /// </summary>
        public int planCheckingCount { get; set; }

        /// <summary>
        /// 一般计划进行中数量
        /// </summary>
        public int planCheckedCount { get; set; }

        /// <summary>
        /// 一般计划待确认数量
        /// </summary>
        public int planConfirmingCount { get; set; }

        /// <summary>
        /// 一般计划中止数量
        /// </summary>
        public int planStopCount { get; set; }

        /// <summary>
        /// 循环计划待提交数量
        /// </summary>
        public int loopSubmitingCount { get; set; }

        /// <summary>
        /// 循环计划待审核数量
        /// </summary>
        public int loopCheckingCount { get; set; }

        /// <summary>
        /// 循环计划进行中数量
        /// </summary>
        public int loopCheckedCount { get; set; }

        /// <summary>
        /// 循环计划待确认数量
        /// </summary>
        public int loopConfirmingCount { get; set; }

        /// <summary>
        /// 循环计划中止数量
        /// </summary>
        public int loopStopCount { get; set; }
    }

    /// <summary>
    /// 计划检索信息
    /// </summary>
    public class PagePlanSearchModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 分页页码
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 分组类型
        /// 1：时间
        /// 2：部门
        /// 3：人员
        /// </summary>
        public EnumDefine.PlanGroupType group { get; set; }

        /// <summary>
        /// 分组值
        /// </summary>
        public string groupValue { get; set; }

        /// <summary>
        /// 排序信息
        /// </summary>
        public List<PageSortInfoModel> sortInfo { get; set; }

        /// <summary>
        /// 筛选信息
        /// </summary>
        public PageFilterInfoModel filterInfo { get; set; }

        /// <summary>
        /// 检索时间
        /// </summary>
        public DateTime? lastTime { get; set; }
    }

    /// <summary>
    /// 排序信息
    /// </summary>
    public class PageSortInfoModel
    {
        /// <summary>
        /// 排序名
        /// 1：默认
        /// 2：重要度
        /// 3：紧急度
        /// 4：时间
        /// </summary>
        public EnumDefine.PlanSortType sortType { get; set; }

        /// <summary>
        /// 排序方式
        /// 1：升序
        /// 2：降序
        /// </summary>
        public EnumDefine.OrderType orderType { get; set; }
    }

    /// <summary>
    /// 筛选信息
    /// </summary>
    public class PageFilterInfoModel
    {
        /// <summary>
        /// 快捷筛选
        /// 1：今日未完成
        /// 2：超时计划
        /// </summary>
        public EnumDefine.PlanFastFilter fast { get; set; }

        /// <summary>
        /// 计划状态
        /// </summary>
        public List<EnumDefine.PlanPageStatus> status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? fromTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? toTime { get; set; }

        /// <summary>
        /// 责任人/确认人
        /// </summary>
        public List<int> userIds { get; set; }
    }

    /// <summary>
    /// 月有效工时
    /// </summary>
    public class PageMonthWorkTimeModel
    {
        /// <summary>
        /// 月总有效工时数
        /// </summary>
        public decimal monthTotalWorkTime { get; set; }

        /// <summary>
        /// 月平均有效工时数
        /// </summary>
        public decimal monthAvgWorkTime { get; set; }
    }

    /// <summary>
    /// 计划分组信息
    /// </summary>
    public class PagePlanGroupInfoModel
    {
        /// <summary>
        /// 分组ID（部门ID、人员ID、时间标志ID）
        /// </summary>
        public string groupId { get; set; }

        /// <summary>
        /// 分组名称（部门名称、人员名、时间）
        /// </summary>
        public string groupName { get; set; }
    }
}