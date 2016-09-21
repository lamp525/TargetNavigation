using System;

namespace MB.Model
{
    public class LoopPlanSubmitModel
    {
        //提交Id
        public int submitId { get; set; }

        public int loopId { get; set; }
        public int number { get; set; }
        public int createUser { get; set; }
        public DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public DateTime updateTime { get; set; }
        public DateTime? confirmTime { get; set; }
    }
}