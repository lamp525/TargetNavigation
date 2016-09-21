using System.Collections.Generic;
namespace MB.Model
{
    public class PlanModel
    {
        public int userId { get; set; }
        public int organizationId { get; set; }
        public string statisticalTime { get; set; }

        /// <summary>
        /// 总计划数量
        /// </summary>
        public int? planCount { get; set; }

        /// <summary>
        //已完成数量
        /// </summary>
        public int? completeCount { get; set; }

        /// <summary>
        /// 超时数量
        /// </summary>
        public int? timeOutCount { get; set; }

        public int name { get; set; }
        public string strname { get; set; }
    }
    public class ReturnConfirm {
        /// <summary>
        /// 返回结果
        /// </summary>
        public int result { get; set; }
        /// <summary>
        /// 返回确认人
        /// </summary>
        public int? confirmUser { get; set; }
        /// <summary>
        /// 节点操作人
        /// </summary>
        public List<int> opreateUser { get; set; }
    }
}