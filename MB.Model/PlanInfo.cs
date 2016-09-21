using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class PlanInfo
    {
        //计划
        public int planId { get; set; }

        public Nullable<int> parentPlan { get; set; }
        public Nullable<int> executionModeId { get; set; }
        public Nullable<int> responsibleOrganization { get; set; }
        public Nullable<int> responsibleUser { get; set; }
        public string responsibleUserName { get; set; }

        public string responsibleUserImage { get; set; } 

        public string confirmUserImage { get; set; }
        public Nullable<int> confirmOrganization { get; set; }
        public Nullable<int> confirmUser { get; set; }
        public string confirmUserName { get; set; }
        public Nullable<int> eventOutputId { get; set; }
        public string eventOutput { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string endTimeNew { get; set; }
        public string startTimeNew { get; set; }
        public Nullable<int> workTime { get; set; }
        public string comment { get; set; }
        public Nullable<int> alert { get; set; }
        public Nullable<int> importance { get; set; }
        public Nullable<int> urgency { get; set; }
        public Nullable<int> difficulty { get; set; }
        public Nullable<int> progress { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<int> time { get; set; }
        public Nullable<decimal> completeQuantity { get; set; }
        public Nullable<decimal> completeQuality { get; set; }
        public Nullable<decimal> completeTime { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> stop { get; set; }
        public Nullable<int> initial { get; set; }
        public Nullable<bool> withSub { get; set; }
        public Nullable<bool> archive { get; set; }
        public Nullable<bool> withFront { get; set; }
        public Nullable<System.DateTime> archiveTime { get; set; }
        public Nullable<bool> autoStart { get; set; }
        public int? meetingId { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public string createTimeNew { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
        public bool deleteFlag { get; set; }

        //接收分解计划的完成时间
        public string oldEndTime { get; set; }

        //是否是协作计划：1、是 0、否
        public int IsCollPlan { get; set; }

        //是否是循环计划:1、是 0、否
        public int isLoopPlan { get; set; }

        //是否是前提计划
        public int isFronPlan { get; set; }

        //有效工时
        public Nullable<decimal> effectiveTime { get; set; }

        //实际工时
        public Nullable<decimal> realTime { get; set; }

        //多级的部门名称
        public string organizationNameNew { get; set; }

        //多级的项目分类
        public string projectNameNew { get; set; }

        //创建人
        public string createUserName { get; set; }

        //创建人头像
        public string bigImage { get; set; }

        //循环计划信息
        public int loopId { get; set; }

        public int? submitId { get; set; }
        public Nullable<int> loopType { get; set; }
        public Nullable<bool> loopStatus { get; set; }
        public string loopYear { get; set; }
        public string loopMonth { get; set; }
        public string loopWeek { get; set; }
        public string loopTime { get; set; }

        //单位完成时间
        public int unitTime { get; set; }

        public DateTime? submitTime { get; set; }

        //项目信息
        public Nullable<int> projectId { get; set; }

        public Nullable<int> parentProject { get; set; }
        public string projectName { get; set; }
        public Nullable<int> proOrderNumber { get; set; }

        //组织信息
        public Nullable<int> organizationId { get; set; }

        public Nullable<int> parentOrganization { get; set; }
        public int? schemaName { get; set; }
        public string organizationName { get; set; }
        public Nullable<int> orgOrderNumber { get; set; }

        //执行方式
        public Nullable<int> executionId { get; set; }

        public string executionMode { get; set; }

        //计划前提
        public int planFront { get; set; }

        //计划操作
        public int operateId { get; set; }

        public string message { get; set; }
        public int result { get; set; }
        public int reviewUser { get; set; }
        public DateTime reviewTime { get; set; }
        public List<string> ComName { get; set; }

        //协作人集合
        public List<CollPlan> collPlanList { get; set; }

        //前提计划集合
        public List<FrontPlan> frontLists { get; set; }

        //计划附件的集合
        public List<PlanAttachment> planAttachmentList { get; set; }

        public int id { get; set; }
        public string name { get; set; }

        ///<summary>标签</summary>
        public string[] keyword { get; set; }
    }

    public class CollPlan
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}