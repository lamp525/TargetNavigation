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
    public class PlanObjectStatistics
    {

        [TestInitialize()]
        public void MyTestInitialize()
        {

            //  DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx.xlsx");
        }


        /// <summary>
        /// 取周计划统计
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [TestMethod]
        public void Testweek()
        {
            DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx", "GetWeekPlanStaticsts");
            PlanStatistics plans = new PlanStatistics();
            plans.Statistics(Convert.ToDateTime("2015-11-23"), Convert.ToDateTime("2015-11-27"), Common.ConstVar.StatisticsType.Week);
        }


        /// <summary>
        /// 取月计划统计
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [TestMethod]
        public void TestMoth()
        {
            DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx", "GetMonthPlanStaticsts");
            PlanStatistics plans = new PlanStatistics();
            plans.Statistics(Convert.ToDateTime("2015-11-1"), Convert.ToDateTime("2015-11-30"), Common.ConstVar.StatisticsType.Month);
        }

        /// <summary>
        /// 取年计划统计
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [TestMethod]
        public void TestYear()
        {
            DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx", "GetWeekPlanStaticsts");
            PlanStatistics plans = new PlanStatistics();
            plans.Statistics(Convert.ToDateTime("2015-1-1"), Convert.ToDateTime("2015-12-30"), Common.ConstVar.StatisticsType.Year);
        }


        /// <summary>
        /// 去目标周统计
        /// </summary>
        [TestMethod]
        public void TestObjectWeek()
        {
            DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx", "GetWeekObjectStaticts");
            ObjectiveStatistics plans = new ObjectiveStatistics();
            plans.Statistics(Convert.ToDateTime("2015-11-23"), Convert.ToDateTime("2015-11-27"), Common.ConstVar.StatisticsType.Week);
        }

        /// <summary>
        /// 去目标月统计
        /// </summary>
        [TestMethod]
        public void TestObjectMoth()
        {
            DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx", "GetMothObjectStaticts");
            ObjectiveStatistics plans = new ObjectiveStatistics();
            plans.Statistics(Convert.ToDateTime("2015-11-1"), Convert.ToDateTime("2015-11-30"), Common.ConstVar.StatisticsType.Month);
        }

        /// <summary>
        /// 去目标年统计
        /// </summary>
        [TestMethod]
        public void TestObjectYear()
        {
            DataUtility.InsertDataBase("PlanObjectStatictsDateTest.xlsx", "GetYearObjectStaticts");
            ObjectiveStatistics plans = new ObjectiveStatistics();
            plans.Statistics(Convert.ToDateTime("2015-1-1"), Convert.ToDateTime("2015-12-30"), Common.ConstVar.StatisticsType.Year);
        }


    }
}
