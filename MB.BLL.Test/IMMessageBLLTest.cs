using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MB.BLL.Test
{
    [TestClass]
    public class IMMessageBLLTest
    {
        MB.BLL.IMMessageBLL msgBll = new MB.BLL.IMMessageBLL();

        [TestMethod]
        public void GetConversationList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IMMessageBLLTestData.xlsx");

            msgBll.GetConversationList(44);
        }
    }
}