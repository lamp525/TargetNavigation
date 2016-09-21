using System;

namespace MB.Model
{
    public class ChildPlanInfo
    {
        //子计划Id
        public int childPlanId { get; set; }

        //重要度
        public Nullable<int> importance { get; set; }

        //紧急度
        public Nullable<int> urgency { get; set; }

        //事项输出
        public string eventOutput { get; set; }

        //责任人Id
        public Nullable<int> responsibleUser { get; set; }

        //责任人名
        public string responsibleUserName { get; set; }

        //结束时间
        public DateTime? endTime { get; set; }

        //是否有子计划
        public Nullable<bool> withSub { get; set; }

        //项目Id
        public Nullable<int> projectId { get; set; }

        //项目分类
        public string projectName { get; set; }

        //部门Id
        public Nullable<int> organizationId { get; set; }

        //部门分类
        public string organizationName { get; set; }

        //计划进程
        public string process { get; set; }
    }
}