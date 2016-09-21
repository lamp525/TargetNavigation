using MB.New.Common;
using System;
using System.Collections.Generic;

namespace MB.New.Model
{
    /// <summary>
    /// 计划检索信息
    /// </summary>
    public class PlanSearchModel
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
        /// -1：不分组
        /// 1：时间
        /// 2：部门
        /// 3：确认人
        /// 4：责任人
        /// </summary>
        public EnumDefine.PlanGroupType group { get; set; }

        /// <summary>
        /// 分组值
        /// </summary>
        public string groupValue { get; set; }

        /// <summary>
        /// 排序信息
        /// </summary>
        public List<PlanSortInfoModel> sortInfo { get; set; }

        /// <summary>
        /// 筛选信息
        /// </summary>
        public PlanFilterInfoModel filterInfo { get; set; }

        /// <summary>
        /// 检索时间
        /// </summary>
        public DateTime? lastTime { get; set; }
    }

    /// <summary>
    /// 排序信息
    /// </summary>
    public class PlanSortInfoModel
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
    public class PlanFilterInfoModel
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
}