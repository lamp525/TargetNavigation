using System;
using System.Collections.Generic;

namespace MB.Model
{
    public class NewPlan
    {
        public Nullable<int> planId { get; set; }//计划ID
        public int department { get; set; }//部门id
        public Nullable<int> project { get; set; }//项目id

        //public Nullable<int> parentPlan { get; set; }//父计划ID
        public Nullable<int> runMode { get; set; }//执行方式id

        public string output { get; set; }//事项输出结果
        public Nullable<int> roundType { get; set; } //循环类型  0:无循环 1:日循环 2:周循环 3:月循环 4:年循环
        public Nullable<DateTime> roundTime { get; set; }//循环时间
        public Nullable<DateTime> endTime { get; set; }//结束时间
        public Nullable<int> responsibleUser { get; set; }  //责任人
        public Nullable<int> confirmUser { get; set; } //确认人
        public Nullable<int> isTmp { get; set; }//是否临时计划
        public Nullable<int> workTime { get; set; }//单位完成工时
        public int[] partner { get; set; }//协作人
        public int[] premise { get; set; }//前提计划
        public string[] keyword { get; set; }//标签
        public List<NewPlan> children { get; set; }//子计划
        public int import { get; set; }//重要度
        public int go { get; set; }//紧急度
        public int difficulty { get; set; }

        /// <summary>会议Id </summary>
        public int? meetingId { get; set; }

        //      department:$int
        //,project:$int
        //,runMode:$int
        //,output:$string                    //事项输出结果
        //,roundType:$int                    //循环类型  0:无循环 1:日循环 2:周循环 3:月循环 4:年循环
        //,roundTime:$string                 //循环时间
        //,endTime:$string                   //结束时间
        //,responsibleUser:$int              //责任人
        //,confirmUser:$int                  //确认人
        //,isTmp:$int                        //是否临时计划
        //,workTime:$int                     //单位完成工时
        //,partner:[$int,$int...]            //协作人
        //,premise:[$int,$int...]            //前提计划
        //,tab:[$string,$string...]
    }
}