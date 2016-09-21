using System;
using MB.New.Common;

namespace MB.New.Model
{
    public class LoopPlanInfoModel
    {
        /// <summary>
        /// 循环计划ID
        /// </summary>
        public int? loopId { get; set; }

        /// <summary>
        /// 是否是循环计划:1、是 0、否
        /// </summary>
        public bool? isLoop { get; set; }

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
}