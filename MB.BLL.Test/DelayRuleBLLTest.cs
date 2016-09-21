using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
namespace MB.BLL.Test
{
    [TestClass]
    public class DelayRuleBLLTest
    {
        /// <summary>
        /// 根据状态获取列表（计划延时，有效工时）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [TestMethod]
        public void TestMethod1()
        {
            DataUtility.InsertDataBase("DelayRuleBllTestData.xlsx", "DelayRuleTest001");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            using (var db = new TargetNavigationDBEntities())
            {
                var stationModel = delayBll.GetDelayList(1);
                foreach (var item in stationModel)
                {
                    Assert.AreEqual(item.deductionMode, 1);
                    Assert.AreEqual(item.deductionNum, 1); ;
                    Assert.AreEqual(item.delayEndTime, 20150831);
                    Assert.AreEqual(item.delayStartTime, 20150820);
                    Assert.AreEqual(item.ruleId, 1);
                    Assert.AreEqual(item.ruletype, 1);
                }

            }
        }
        /// <summary>
        /// 获取自定义激励规则（自定义）
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestYearPlan()
        {
            DataUtility.InsertDataBase("PlanCompleteStatisticBllTest.xlsx", "SavePlan_Test002");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            using (var db = new TargetNavigationDBEntities())
            {
                var stationModel = delayBll.GetValueCustList();
                foreach (var item in stationModel)
                {
                    Assert.AreEqual(item.customEndTime, 110);
                    Assert.AreEqual(item.customId, 1); ;
                    Assert.AreEqual(item.customStartTime, 27);
                    Assert.AreEqual(item.customType, 12);
                    Assert.AreEqual(item.deductionMode, 12);
                    Assert.AreEqual(item.deductionNum, 12);
                }

            }
        }
        /// <summary>
        /// 获取激励规则对象
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestWorkTime()
        {
            DataUtility.InsertDataBase("PlanCompleteStatisticBllTest.xlsx", "SavePlan_Test004");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            using (var db = new TargetNavigationDBEntities())
            {
                var stationModel = delayBll.GetValueIncentive();
                Assert.AreEqual(stationModel.incentiveType, 110);
                Assert.AreEqual(stationModel.maxAverage, 400);
                Assert.AreEqual(stationModel.maxValue, 4);
                Assert.AreEqual(stationModel.maxValueType, 4);
                Assert.AreEqual(stationModel.minAverage, 4);
                Assert.AreEqual(stationModel.minValue, 4);
                Assert.AreEqual(stationModel.minValueType, 4);
                Assert.AreEqual(stationModel.standardTime, 4);
            }
        }
        /// <summary>
        /// 更新或添加激励规则
        /// </summary>
        /// <param name="model"></param>
        [TestMethod]
        public void AddUpdate()
        {
            DataUtility.InsertDataBase("PlanCompleteStatisticBllTest.xlsx", "SavePlan_Test004");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            var newModel = new ValueIncentiveModel
            {
                incentiveType = 1,
                maxAverage = 20,
                maxValue = 30,
                maxValueType = 30,
                minAverage = 10,
                minValue = 10,
                minValueType = 2,
                standardTime = 1
            };
            using (var db = new TargetNavigationDBEntities())
            {
                delayBll.AddOrUpdateValueIncentive(newModel);
                var date = db.tblValueIncentive.FirstOrDefault();
                Assert.AreEqual(date.incentiveType, 1);
                Assert.AreEqual(date.maxAverage, 20);
                Assert.AreEqual(date.maxValue, 30);
                Assert.AreEqual(date.maxValueType, 30);
                Assert.AreEqual(date.minAverage, 10);
                Assert.AreEqual(date.minValue, 10);
                Assert.AreEqual(date.minValueType, 2);
                Assert.AreEqual(date.standardTime, 1);
                Assert.AreEqual(date.valueId, 1);

            }
        }
        /// <summary>
        /// 新增自定义规则
        /// </summary>
        /// <param name="id"></param>
        [TestMethod]
        public void AddRALY()
        {
            DataUtility.InsertDataBase("PlanCompleteStatisticBllTest.xlsx", "SavePlan_Test004");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            var newModel = new ValueIncentiveCustomModel
            {
                customEndTime = 1,
                customId = 1,
                customStartTime = 1,
                customType = 1,
                deductionMode = 1,
                deductionNum = 1
            };
            List<ValueIncentiveCustomModel> list = new List<ValueIncentiveCustomModel>();
            list.Add(newModel);
            using (var db = new TargetNavigationDBEntities())
            {
                delayBll.AddValueCust(list);
                var date = db.tblValueIncentiveCustom.FirstOrDefault(); 
                    Assert.AreEqual(date.customEndTime, 110);
                    Assert.AreEqual(date.customId, 110);
                    Assert.AreEqual(date.customStartTime, 110);
                    Assert.AreEqual(date.customType, 110);
                    Assert.AreEqual(date.deductionMode, 110);
                    Assert.AreEqual(date.deductionNum, 110); 
            }
        }
        /// <summary>
        /// 删除自定义
        /// </summary>
        /// <param name="valueIncentList"></param>
        [TestMethod]
        public void dELETE()
        {
            DataUtility.InsertDataBase("PlanCompleteStatisticBllTest.xlsx", "SavePlan_Test004");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            var newModel = new ValueIncentiveCustomModel
            {

            };
            List<ValueIncentiveCustomModel> list = new List<ValueIncentiveCustomModel>();
            list.Add(newModel);
            using (var db = new TargetNavigationDBEntities())
            {
                delayBll.AddValueCust(list);
                //foreach (var item in stationModel)
                //{
                //    Assert.AreEqual(item.Id, 110);
                //    Assert.AreEqual(item.effectiveTime, 400);
                //    Assert.AreEqual(item.workTime, 4);
                //}

            }
        }
        /// <summary>
        /// 添加激励规则(计划延期，审批超时)
        /// </summary>
        /// <param name="delayRuleList"></param>
        /// <returns></returns>
        [TestMethod]
        public void aDDlIST()
        {
            DataUtility.InsertDataBase("PlanCompleteStatisticBllTest.xlsx", "SavePlan_Test004");
            MB.BLL.DelayRuleBLL delayBll = new DelayRuleBLL();
            var newModel = new DelayRuleModel
            {

            };
            List<DelayRuleModel> list = new List<DelayRuleModel>();
            list.Add(newModel);
            using (var db = new TargetNavigationDBEntities())
            {
                delayBll.AddDelayRule(list);
                //foreach (var item in stationModel)
                //{
                //    Assert.AreEqual(item.Id, 110);
                //    Assert.AreEqual(item.effectiveTime, 400);
                //    Assert.AreEqual(item.workTime, 4);
                //}

            }
        }


    }
}
