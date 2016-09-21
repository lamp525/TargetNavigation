using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
using MB.BLL;

namespace MB.BLL.Test
{
    [TestClass]
    public class IncentiveIndexTest
    {
        IncentiveIndexBLL incentiveIndexBll = new IncentiveIndexBLL();


        /// <summary>
        /// 获取每月奖惩数
        /// </summary>
        [TestMethod]
        public void GetRewardPunishNum_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("IncentiveIndexBLLTestData.xlsx", "GetRewardPunish_Test001");

            var list = incentiveIndexBll.GetRewardPunishNum(2015, 110);

            //验证
            Assert.AreEqual(list.Count(), 2);
            Assert.AreEqual(list[0].month, 8);
            Assert.AreEqual(list[0].number, 80);
        }

        /// <summary>
        /// 获取激励详细情况 年的场合
        /// </summary>
        [TestMethod]
        public void GetRewardPunishDetail_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("IncentiveIndexBLLTestData.xlsx", "GetRewardPunish_Test001");
            var model = incentiveIndexBll.GetRewardPunishDetail(2015, null, 110);

            //验证
            Assert.AreNotEqual(model, null);
        }

        /// <summary>
        /// 获取激励详细情况  月的场合
        /// </summary>
        [TestMethod]
        public void GetRewardPunishDetail_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("IncentiveIndexBLLTestData.xlsx", "GetRewardPunish_Test001");
            var model = incentiveIndexBll.GetRewardPunishDetail(2015, 8, 110);

            //验证
            Assert.AreNotEqual(model, null);
        }

       




    }
}
