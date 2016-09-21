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
    public class WorktimeStatisticsTest
    {
        private WorktimeStatistics _wts = new WorktimeStatistics();
        [TestMethod]
     public   void StatisticsWorkTime()
        {
            bool result = true;

            DataUtility.InsertDataBase("WorktimeStatisticsTestData.xlsx");

            _wts.StatisticsWorkTime();
  

            Assert.IsTrue(result);

        }

    }
}
