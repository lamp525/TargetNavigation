using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using System.Web;
using MB.DAL;

namespace MB.BLL.Test
{
    //说明：计划的筛选排序项目中JS传递模拟数据，能进行测试，这里就pass掉了

    [TestClass]
    public class UnitTest1
    {

        MB.BLL.PlanBLL t = new MB.BLL.PlanBLL();

        //测试根据启止时间统计每个状态值下的计划数
        [TestMethod] 
        public void TestMethod1()
        {
            var startTime = DateTime.Now.AddDays(-15);
            var endTime = DateTime.Now.AddDays(5);
         //   var list = t.GetPlanStatusInfo(2015, 3);
        }

        //测试树形部门绑定
        [TestMethod]
        public void TestMethod2()
        {
            var list = t.GetDepartmentList();
        }

        //测试树形项目分类绑定
        [TestMethod]
        public void TestMethod3()
        {
            var list = t.GetDepartmentList();
        }

        //测试人员模糊查询
        [TestMethod]
        public void TestMethod4()
        {
            var list = t.SelectUserList("xc",true);
        }

        //测试根据用户Id查询上级，自己以及下属的用户信息
        [TestMethod]
        public void TestMethod5()
        {
            var list = t.GetUserIdListByUserId(3);
        }

        //测试归类到计划
        [TestMethod]
        public void TestMethod6()
        {
            t.ClassificatePlan(3,2);
        }

        //测试统计用户的今日未完成计划和超时计划的数量
        [TestMethod]
        public void TestMethod7()
        {
            t.StatisticsUserPlan(3);
        }

        //测试根据用户Id查询用户信息
        [TestMethod]
        public void TestMethod8()
        {
            t.GetUserInfoById(3);
        }

        //测试中止计划或者开始计划:0、运行中   90、已中止
        [TestMethod]
        public void TestMethod9()
        {
            //t.StopOrStartPlan(3,90);
        }

        //测试分解计划：待定
        [TestMethod]
        public void TestMethod10()
        {
        }

        //测试计划确认:true、确认通过  false：确认未通过
        [TestMethod]
        public void TestMethod11()
        {
          //  t.ConfirmPlan(3,10,10,10,20,true);
        }

        //测试计划审核:true、确认审核  false：确认未审核
        [TestMethod]
        public void TestMethod12()
        {
           // t.ExaminePlan(3, 10, 10, 20, true);
        }

        //测试计划转办:true、确认审核  false：确认未审核
        [TestMethod]
        public void TestMethod13()
        {
         //   t.ExaminePlan(3, 10, 10, 20, true);
        }
    }
    [TestClass]
    public class ExecTest
    {
        MB.Common.CommonWorkTime com = new Common.CommonWorkTime();

        //测试根据用户ID获取用户功效价值
        [TestMethod]
        public void PersonalTest()
        {   
            com.getPersonalWorktime(5, DateTime.Parse("2015-3-3"),"month");
        }

        //测试根据组织ID获取部门功效价值
        [TestMethod]
        public void DepartmentTest()
        {
            com.getDepartmentlWorktime(3, DateTime.Parse("2015-3-3"), "month");
        }

        //测试获取功效价值TOP10的部门
        [TestMethod]
        public void DepartmentIndexTest()
        {
            com.getDepartmentIndex(DateTime.Parse("2015-3-3"), "year");
        }

    }
}
