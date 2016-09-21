using System;

namespace MB.Model
{
    public class PlanOperateTimeModel
    {
        //计划
        public int planId { get; set; }

        public Nullable<int> parentPlan { get; set; }
        public Nullable<int> executionModeId { get; set; }

        //责任部门
        public Nullable<int> responsibleOrganization { get; set; }

        //责任人Id
        public Nullable<int> responsibleUser { get; set; }

        //责任人昵称
        public string responsibleUserName { get; set; }

        //确认部门
        public Nullable<int> confirmOrganization { get; set; }

        //确认人Id
        public Nullable<int> confirmUser { get; set; }

        //确认人昵称
        public string confirmUserName { get; set; }

        //计划生成时间
        public DateTime? planGenerateTime { get; set; }

        //审核时间
        public DateTime? auditTime { get; set; }

        //提交时间
        public DateTime? submitTime { get; set; }

        //确认时间
        public DateTime? confirmTime { get; set; }

        //状态  0：未提交 10：审核中 15：审核未通过 20：审核通过 25：申请修改 30：等待确认 40：确认未通过 90：已完成
        public int? status { get; set; }

        //计划完成时间
        public DateTime? endTime { get; set; }
    }
}