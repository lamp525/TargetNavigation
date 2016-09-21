using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
using FW.MBService;

namespace MB.BLL.Test
{
    [TestClass]
   public  class RewardPunishTest
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // 导入共通数据
            // DataUtility.InsertDataBase("RewardPunishMBServiceTestData.xlsx");
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        }



       RewardPunish rewardPunish = new RewardPunish();

       /// <summary>
       /// 统计上个月的奖惩情况
       /// </summary>
       [TestMethod]
       public void GetRewardpunishStatistics_Test001()
       {
           using (var db = new TargetNavigationDBEntities())
           {
               //导入测试数据
               DataUtility.InsertDataBase("RewardPunishMBServiceTestData.xlsx", "Rewardpunish_Test001");
               rewardPunish.GetRewardpunishStatistics(DateTime.Now);
               var list = (from rp in db.tblRewardPunish
                          select rp).ToList();

               //验证
               Assert.AreNotEqual(list.Count(), 0);
           }
       }

        /// <summary>
        /// 循环上个月末所有计划
        /// </summary>
       //[TestMethod]
       //public void lastMonthPlan_Test001()
       //{
       //    //导入测试数据
       //    DataUtility.InsertDataBase("RewardPunishMBServiceTestData.xlsx", "GetRewardpunishStatistics_Test001");
       //    //rewardPunish.lastMonthPlan();
       //    //var list = rewardPunish.GetRewardPunish();

       //    //验证
       //    //Assert.AreNotEqual(list.Count(), 0);
       //}
    }
}
