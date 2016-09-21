using System;

namespace MB.Model
{
    public class GanttChartModel
    {
        /// <summary>目标Id</summary>
        public int id { get; set; }

        /// <summary>父目标Id</summary>
        public int? pid { get; set; }

        /// <summary>目标名称</summary>
        public string name { get; set; }

        /// <summary>临时存储开始时间 </summary>
        public DateTime? tempStart { get; set; }

        /// <summary>临时存储结束时间</summary>
        public DateTime? tempEnd { get; set; }

        /// <summary>目标开始时间</summary>
        public string start { get; set; }

        /// <summary>目标结束时间</summary>
        public string end { get; set; }

        /// <summary>项目进度</summary>
        public int? process { get; set; }

        /// <summary>true：有子目标 false：无子目标</summary>
        public bool haschild { get; set; }

        /// <summary>状态值</summary>
        public int status { get; set; }

        /// <summary>临时存储父目标开始时间</summary>
        public DateTime? tempParentStart { get; set; }

        /// <summary>父目标开始时间 </summary>
        public string parentStart { get; set; }

        /// <summary>临时存储父目标结束时间</summary>
        public DateTime? tempParentEnd { get; set; }

        /// <summary>父目标结束时间 </summary>
        public string parentEnd { get; set; }

        /// <summary>子目标开始时间 </summary>
        public string childStart { get; set; }

        /// <summary>子目标结束时间 </summary>
        public string childEnd { get; set; }

        /// <summary>责任人Id</summary>
        public int? responsibleUser { get; set; }

        /// <summary>责任人名称</summary>
        public string responsibleUserName { get; set; }

        /// <summary>确认人Id</summary>
        public int? confirmUser { get; set; }

        /// <summary>确认人名称 </summary>
        public string confirmUserName { get; set; }

        /// <summary>true：允许拖动 false：不允许拖动</summary>
        public bool objectiveStatus { get; set; }
    }
}