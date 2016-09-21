using System;

namespace MB.Model
{
    public class FormModel
    {
        public int userId { get; set; }

        /// <summary>
        /// 审批开始时间（表单抄送）
        /// </summary>
        public DateTime FDcreateTime { get; set; }

        /// <summary>
        /// 实际审批时间(表单流程)
        /// </summary>
        public DateTime FFcreateTime { get; set; }
    }
}