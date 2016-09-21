using System;
using System.Collections.Generic;

namespace MB.Model
{
    //目标列表实体
    public class ObjectiveInfo
    {
        /// <summary>登录用户</summary>
        public int userId { get; set; }

        public List<ObjectiveIndexModel> ObjectiveList { get; set; }
    }

    public class ObjectiveIndexModel
    {
        ///是否是下属目标
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

        /// <summary>目标文档信息</summary>
        public List<ObjectiveDocumentInfo> documentInfo { get; set; }

        /// <summary>目标日志信息</summary>
        public List<ObjectiveOperateLog> operateLog { get; set; }

        /// <summary>目标变更信息</summary>
        public ObjectiveChangeInfo ChangeInfo { get; set; }

        /// <summary>目标公式信息</summary>
        public List<ObjectiveFormula> objectiveFormula { get; set; }

        /// <summary>子目标集合 </summary>
        public List<ChildObjective> childrenObjective { get; set; }
    }

    //目标文档
    public class ObjectiveDocumentInfo
    {
        /// <summary>附件ID</summary>
        public int attachmentId { get; set; }

        /// <summary>文档分类ID </summary>
        public int documenType { get; set; }

        /// <summary>表示名</summary>
        public string displayName { get; set; }

        /// <summary>保存名</summary>
        public string saveName { get; set; }

        /// <summary>文件后缀名</summary>
        public string extension { get; set; }
    }

    //目标操作日志
    public class ObjectiveOperateLog
    {
        /// <summary>操作ID </summary>
        public int operateId { get; set; }

        /// <summary>目标名称</summary>
        public string objectiveName { get; set; }

        /// <summary>操作意见</summary>
        public string message { get; set; }

        /// <summary>操作类型 </summary>
        public int? result { get; set; }

        /// <summary>操作人</summary>
        public int? reviewUser { get; set; }

        /// <summary>操作人名称</summary>
        public string reviewUserName { get; set; }

        /// <summary>操作时间</summary>
        public DateTime? reviewTime { get; set; }
    }

    //目标变更信息
    public class ObjectiveChangeInfo
    {
        /// <summary>目标名变更</summary>
        public bool objectiveNameUpdate { get; set; }

        /// <summary>权重变更</summary>
        public bool weightUpdate { get; set; }

        /// <summary>指标值变更</summary>
        public bool objectiveValueUpdate { get; set; }

        /// <summary>理想值变更</summary>
        public bool expectedValueUpdate { get; set; }

        /// <summary>实际值变更</summary>
        public bool actualValueUpdate { get; set; }

        /// <summary>奖励基数变更 </summary>
        public bool bonusUpdate { get; set; }

        /// <summary>开始时间变更</summary>
        public bool startTimeUpdate { get; set; }

        /// <summary>结束时间变更</summary>
        public bool endTimeUpdate { get; set; }

        /// <summary>警戒时间变更</summary>
        public bool alarmTimeUpdate { get; set; }
    }

    //目标公式信息
    public class ObjectiveFormula
    {
        /// <summary>公式Id</summary>
        public int? formulaId { get; set; }

        /// <summary>公式编号</summary>
        public int formulaNum { get; set; }

        /// <summary>排序</summary>
        public int orderNum { get; set; }

        /// <summary>字段</summary>
        public int? field { get; set; }

        /// <summary>操作符 </summary>
        public string operate { get; set; }

        /// <summary>具体值</summary>
        public string numValue { get; set; }

        /// <summary>具体值类型 true:时间  false:数字</summary>
        public bool? valueType { get; set; }
    }

    //子目标信息
    public class ChildObjective
    {
        /// <summary>子目标Id</summary>
        public int objectiveId { get; set; }

        /// <summary>父目标Id</summary>
        public int parentObjectiveId { get; set; }

        /// <summary>子目标名称</summary>
        public string objectiveName { get; set; }

        /// <summary>目标对象</summary>
        public int? objectiveType { get; set; }

        /// <summary>权重</summary>
        public decimal? weight { get; set; }

        /// <summary>责任部门名</summary>
        public string responsibleOrgName { get; set; }

        /// <summary>责任人名</summary>
        public string responsibleUserName { get; set; }
    }
}