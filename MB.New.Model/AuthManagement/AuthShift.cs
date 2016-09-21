namespace MB.New.Model
{
    public class AuthShift
    {
        /// <summary>
        /// 权限Id
        /// </summary>
        public int authId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string authName { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string auth { get; set; }

        /// <summary>
        /// 转出者
        /// </summary>
        public int turnUserId { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        public int acceptUserId { get; set; }

        /// <summary>
        /// 计划状态  1.待提交 2.待审批  3.已审批  4.待确认  5.已完成
        /// </summary>
        public int[] planData { get; set; }

        /// <summary>
        /// 目标状态  1.待提交  2.待审核  3.进行中  4.待确认  5.已完成  6.已超时
        /// </summary>
        public int[] objectiveData { get; set; }

        /// <summary>
        /// 流程  1.待提交  2.已提交  3.待查阅  4.待审批  5.已处理  6.已办结
        /// </summary>
        public int[] FlowData { get; set; }

        /// <summary>
        /// 个人文档  1.我的共享  2.他人共享 3.公司文档权限
        /// </summary>
        public int[] userDocumentData { get; set; }
    }
}