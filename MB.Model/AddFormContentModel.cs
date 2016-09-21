using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class AddFormContentModel
    {
        //模板Id
        public int templateId { get; set; }

        //组织Id
        public int organizationId { get; set; }

        //岗位Id
        public int stationId { get; set; }

        //标题
        public string title { get; set; }

        //紧急度
        public int urgency { get; set; }

        //状态
        public int? status { get; set; }

        public int currentNode { get; set; }

        //创建用户
        public int createUser { get; set; }

        //创建时间
        public DateTime createTime { get; set; }

        public List<ControlModel> controlValue { get; set; }
    }

    public class ControlModel
    {
        //父控件Id
        public string parentControl { get; set; }

        //控件Id
        public string controlId { get; set; }

        //明细行集合
        public List<ControlDetailModel> rowNumberList { get; set; }
    }

    public class ControlDetailModel
    {
        //明细行
        public int? rowNumber { get; set; }

        //明细行值
        public string[] detailValue { get; set; }
    }
}