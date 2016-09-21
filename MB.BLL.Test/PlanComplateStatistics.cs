using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MB.BLL.Test
{
    [TestClass]
    public class PlanComplateStatistics
    {
        FW.MBService.PlanStatistics plancount = new FW.MBService.PlanStatistics();
        /// <summary>
        /// 周计划统计
        /// </summary>
        [TestMethod]
        public void Test001()
        {
            var station = new tblWeekPlanCompleteStatistics
            {
                //parentStation = 2,
                //stationName = "需求",
                //organizationId = 1,
                //comment = "市场调研",
                //approval = false,
                //skipApproval = true
            };
            // orgManagementBll.SaveStation(station, 247);
            using (var db = new TargetNavigationDBEntities())
            {
                var stationModel = db.tblStation.FirstOrDefault();
                Assert.AreNotEqual(stationModel, null);
                Assert.AreEqual(stationModel.parentStation, 2);
                Assert.AreEqual(stationModel.stationName, "需求");
                Assert.AreEqual(stationModel.organizationId, 1);
                Assert.AreEqual(stationModel.comment, "市场调研");
                Assert.AreEqual(stationModel.approval, false);
             //   Assert.AreEqual(stationModel.skipApproval, true);
            }
        }

        /// <summary>
        /// 月计划统计
        /// </summary>
        public void Test002()
        {
            DataUtility.InsertDataBase("PlanStaticseBLLTestData.xlsx", "SavePlan_Test001");
            var station = new tblWeekPlanCompleteStatistics
            {
                completeCount = 10,
                confirmedCount = 10,
                examineCount = 10,
                notComplateCount = 10,
                organizationId = 1,
                planCount = 40,
                statisticalTime = "2015-06",
                stopPlanCount = 0,
                submitCount = 0,
                userId = 111
            };
            // orgManagementBll.SaveStation(station, 247);
            using (var db = new TargetNavigationDBEntities())
            {
                var plancount = db.tblWeekPlanCompleteStatistics.FirstOrDefault();
                Assert.AreEqual(plancount.userId, 111);




                //Assert.AreNotEqual(stationModel, null);
                //Assert.AreEqual(stationModel.parentStation, 2);
                //Assert.AreEqual(stationModel.stationName, "需求");
                //Assert.AreEqual(stationModel.organizationId, 1);
                //Assert.AreEqual(stationModel.comment, "市场调研");
                //Assert.AreEqual(stationModel.approval, false);
                //Assert.AreEqual(stationModel.skipApproval, true);
            }
        }
    }
}
