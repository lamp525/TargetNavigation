namespace MB.New.Model
{
    public class NodeOperateModel
    {
        //节点操作人ID
        public int? operateId { get; set; }

        //节点ID
        public int? nodeId { get; set; }

        //节点类型
        public int? nodeType { get; set; }

        // 类型 1：操作者部门 2：操作者岗位 3：操作者 4：上级岗位 5：所有人
        public int? type { get; set; }

        //条件 1:属于 2：不属于
        public int? condition { get; set; }

        //会签 0：审批 1：会签 2：抄送 3：提交
        public int? countersign { get; set; }

        //批次条件类型  1：申请人组织架构 2：申请人岗位 3：申请人
        public int? batchType { get; set; }

        //批次条件 1:属于 2：不属于
        public int? batchCondition { get; set; }

        //操作人目标ID
        public int?[] targetId { get; set; }

        //批次目标ID
        public int?[] batchTargetId { get; set; }

        //操作人Id集合
        public int?[] userIds { get; set; }

        //操作人结果
        public string[] targetName { get; set; }

        //批次条件结果
        public string[] batchTargetName { get; set; }

        //排序
        public int? orderNum { get; set; }

        //测试标志
        public bool? testFlag { get; set; }
    }
}