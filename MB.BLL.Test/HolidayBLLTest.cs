using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Model;
using MB.DAL;
using MB.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MB.BLL.Test
{
    public class HolidayBLLTest
    {
        HolidayBLL holiadyBll = new HolidayBLL();
        [TestMethod]
        public void GetCompletedFlowList_Test001()
        {
        //    DataUtility.InsertDataBase("HolidayBLLTestData.xlsx", "GetHoildayList");
        //    List<HolidayModel> itemlist = new List<HolidayModel>();
        //    var model = new HolidayModel
        //    {
        //         type=1,
        //          Holiday="2015-6-1", 
        //    };
        //    itemlist.Add(model);

        //    var model2 = new HolidayModel
        //    {
        //         type=1,
        //          Holiday="2015-7-30"
        //    };
        //    var list = holiadyBll.GetHolidayModel(itemlist);　
        }
    }
}
