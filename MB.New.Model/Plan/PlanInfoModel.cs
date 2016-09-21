using MB.New.Common;
using System;
using System.Collections.Generic;

namespace MB.New.Model
{
    public class PlanInfoModel
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
        public List<UserInfoSimpleModel> partnerInfo { get; set; }

        ///<summary>
        ///标签数组
        ///</summary>
        public string[] keyword { get; set; }

        /// <summary>
        /// 标签字符串信息
        /// </summary>
        public string keywordInfo { get; set; }
    }
}