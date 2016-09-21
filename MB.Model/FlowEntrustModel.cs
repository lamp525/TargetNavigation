using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class FlowEntrustModel
    {
        public int entrustId { get; set; }
        public int entrustUser { get; set; }
        public int mandataryUser { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public Nullable<int> number { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
        public bool deleteFlag { get; set; }
        public List<TemplateModel> entrusList { get; set; }
        public string entrustuserName { get; set; }
        public string mandataryUserName { get; set; }
        public int[] templateId { get; set; }
        public bool isComplate { get; set; }

        public int?[] status { get; set; }

        //创建时间
        public string[] time { get; set; }

        //创建人
        public int?[] person { get; set; }
    }
}