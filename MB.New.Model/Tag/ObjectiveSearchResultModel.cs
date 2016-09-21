using System;

namespace MB.New.Model
{
    public class ObjectiveSearchResultModel
    {
        ///<summary>是否是下属目标</summary>
        public bool? isSubordinateObjective { get; set; }

        /// <summary>目标Id</summary>
        public int objectiveId { get; set; }

        /// <summary>父目标Id</summary>
        public int? parentObjective { get; set; }

        /// <summary>显示变更Flag 0：显示  1：显示</summary>
        public bool displayChangeFlag { get; set; }

        /// <summary>目标名称</summary>
        public string objectiveName { get; set; }

        /// <summary>目标对象 1：组织架构 2：人员</summary>
        public int? objectiveType { get; set; }

        /// <summary>目标对象名称</summary>
        public string objectiveTypeName { get; set; }

        /// <summary>判定类型 1：时间 2：数字</summary>
        public int? valueType { get; set; }

        /// <summary>奖励基数</summary>
        public decimal? bonus { get; set; }

        /// <summary>权重</summary>
        public decimal? weight { get; set; }

        /// <summary>指标值</summary>
        public string objectiveValue { get; set; }

        /// <summary>理想值</summary>
        public string expectedValue { get; set; }

        /// <summary>实际值</summary>
        public string actualValue { get; set; }

        /// <summary>最小开始时间</summary>
        public DateTime? minStartTime { get; set; }

        /// <summary>最大开始时间</summary>
        public DateTime? maxStartTime { get; set; }

        /// <summary>预计开始时间</summary>
        public DateTime startTime { get; set; }

        /// <summary>实际结束时间</summary>
        public DateTime? actualEndTime { get; set; }

        /// <summary>最小结束时间</summary>
        public DateTime? minEndTime { get; set; }

        /// <summary>最大结束时间</summary>
        public DateTime? maxEndTime { get; set; }

        /// <summary>预计结束时间</summary>
        public DateTime endTime { get; set; }

        /// <summary>警戒时间</summary>
        public DateTime? alarmTime { get; set; }

        /// <summary>责任人Id</summary>
        public int? responsibleUser { get; set; }

        /// <summary>责任人名称</summary>
        public string responsibleUserName { get; set; }

        /// <summary>责任人部门Id </summary>
        public int? responsibleOrg { get; set; }

        /// <summary>责任人部门名称</summary>
        public string responsibleOrgName { get; set; }

        /// <summary>确认人Id</summary>
        public int? confirmUser { get; set; }

        /// <summary>确认人名称 </summary>
        public string confirmUserName { get; set; }

        /// <summary>被授权人Id</summary>
        public int? authorizedUser { get; set; }

        /// <summary>被授权人名称</summary>
        public string authorizedUserName { get; set; }

        /// <summary>考核类型 1：金额 2：时间 3：数字</summary>
        public int? checkType { get; set; }

        /// <summary>状态 1：待提交 2：待审核 3：审核通过（进行中） 4：待确认 5：已完成</summary>
        public int status { get; set; }

        /// <summary>备注项目</summary>
        public string description { get; set; }

        /// <summary>进度</summary>
        public int? progress { get; set; }

        /// <summary>公式 0：无公式 1：默认公式 2：自定义</summary>
        public int? formula { get; set; }

        /// <summary>最大奖励数</summary>
        public decimal? maxValue { get; set; }

        /// <summary>最大扣除数</summary>
        public decimal? minValue { get; set; }

        /// <summary>true：存在子目标  false：不存在子目标</summary>
        public bool isHasChild { get; set; }

        /// <summary>修改时间</summary>
        public DateTime updateTime { get; set; }

        /// <summary>目标创建人</summary>
        public string createUser { get; set; }

        /// <summary>创建时间</summary>
        public DateTime createTime { get; set; }

        /// <summary>创建者头像</summary>
        public string bigImage { get; set; }

        ///<summary>标签</summary>
        public string[] keyword { get; set; }
    }
}