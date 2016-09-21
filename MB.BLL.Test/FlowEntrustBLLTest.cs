using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
namespace MB.BLL.Test
{
    [TestClass]
    public class FlowEntrustBLLTest
    {
        FlowEntrustBLL FlowEntrustBll = new FlowEntrustBLL();
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // 导入共通数据
            // DataUtility.InsertDataBase("import.xlsx");
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        } 
        //测试获取系统表单列
        [TestMethod]
        public void GetEnttustList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetEnttustList_Test001");
            var list = FlowEntrustBll.GetFlowEntrustList("deleteFlag=false", 110, DateTime.MinValue, DateTime.MaxValue, DateTime.MinValue, DateTime.MaxValue);

            //验证
            Assert.AreEqual(list.Count, 0);
         
        }
        [TestMethod]
        public void GetEnttustInfo_test()
        {
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetEnttustList_Test001");
            var info = FlowEntrustBll.GetFlowById(1); 
            Assert.AreEqual(info.entrustId, 1);
            Assert.AreEqual(info.mandataryUserName, "徐晓催");
        }

        [TestMethod]
        public void addNewEntust_Test()
        {
            // 清空测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetEnttustList_Test001");
            int [] id={1};
            var userDocument = new FlowEntrustModel
            {

                entrustId = 1,
                createTime = DateTime.Now,
                createUser = 1,
                deleteFlag = false,
                endTime = DateTime.Now,
                startTime = DateTime.Now,
                mandataryUser = 2,
                entrustUser = 3,
                updateTime = DateTime.Now,
                updateUser = 1,
                templateId = id
            };
            var flag = FlowEntrustBll.AddNewFlowE(userDocument);
            //验证
            var model = new tblFlowEntrust();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblFlowEntrust.Where(p => p.entrustId == 1).FirstOrDefault();
            }
            Assert.AreEqual(flag, true);
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.entrustId, 1);
            Assert.AreEqual(model.entrustUser, 110);
            Assert.AreEqual(model.mandataryUser, 247);
        }

        [TestMethod]
        public void UpdateEn_test()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetEnttustList_Test001");

            FlowEntrustBll.UpdateFlowE(1, 1);

            var model = new tblFlowEntrust();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblFlowEntrust.Where(p => p.entrustId == 1).FirstOrDefault();
            }
            Assert.AreNotEqual(model, null);
            Assert.AreNotEqual(model.endTime, DateTime.Now);
        }

         
        [TestMethod]
        public void GetFolderCaregory_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "GetFolderCaregory_Test001");
            var list = FlowEntrustBll.GetCategoryModel();

            //验证
            Assert.AreEqual(list.Count, 0);

        }


        [TestMethod]
        public void GetTemplateList_test()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetInfoTest");
            var list = FlowEntrustBll.GetTemById(1);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id, 1);
            Assert.AreEqual(list[0].name, "请假流程");
        }
    }
}
