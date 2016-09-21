using System;

namespace MB.Model
{
    public class FormFlowModel
    {
        //流程ID
        public int formFlowId { get; set; }

        //表单ID
        public int? formId { get; set; }

        //流程节点ID
        public int? nodeId { get; set; }

        //操作类型
        public int? result { get; set; }

        //节点意见
        public string contents { get; set; }

        //委托人
        public int? entrustUser { get; set; }

        //创建用户
        public int createUser { get; set; }

        //创建时间
        public DateTime createTime { get; set; }

        //修改用户
        public int updateUser { get; set; }

        //修改时间
        public DateTime updateTime { get; set; }

        //删除标志
        public bool deleteFlag { get; set; }
    }
}