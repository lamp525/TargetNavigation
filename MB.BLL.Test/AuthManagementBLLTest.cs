using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
using System.Web;

namespace MB.BLL.Test
{
    [TestClass]
   public class AuthManagementBLLTest
    {
        AuthManagementBLL authManagementBLL = new AuthManagementBLL();

        [TestInitialize()]
        public void MyTestInitialize()
        {
            // 导入共通数据
            //DataUtility.InsertDataBase("BLLTestData.xlsx");
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        /// <summary>
        /// 测试查询
        /// </summary>
        [TestMethod]
        public void GetAuthList_Test001()
        {
            //导入数据
            DataUtility.InsertDataBase("AuthManagementBLLTestData.xlsx", "GetAuthList_Test001");
            var list = authManagementBLL.GetAuthList();

            //验证
            Assert.AreNotEqual(list.Count,0);
        }

        /// <summary>
        /// 测试新建
        /// </summary>
        [TestMethod]
        public void SaveAuth_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("AuthManagementBLLTestData.xlsx", "GetAuthList_Test001");

            var model = new AuthModel {
                authName="HR管理员",
                auth="计划制定，计划审批，流程审批"
            };
            //authManagementBLL.SaveAuth(model);
           // var list = authManagementBLL.GetAuthList("HR管理员");

            ////验证
            //Assert.AreEqual(list.Count,1);
            //Assert.AreEqual(list[0].auth, "计划制定，计划审批，流程审批");
        }

        /// <summary>
        /// 测试修改
        /// </summary>
        [TestMethod]
        public void SaveAuth_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("AuthManagementBLLTestData.xlsx", "GetAuthList_Test001");

            var model = new AuthModel
            {
                authId=1,
                authName = "管理员",
                auth = "计划制定，计划审批，流程审批"
            };
            //authManagementBLL.SaveAuth(model);
            //var list = authManagementBLL.GetAuthList("管理员");

            ////验证
            //Assert.AreEqual(list.Count, 1);
            //Assert.AreEqual(list[0].auth, "计划制定，计划审批，流程审批");
        }

        /// <summary>
        /// 测试权限转移 
        /// </summary>
        [TestMethod]
        public void AuthShift_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("AuthManagementBLLTestData.xlsx", "AuthShift_Test001");

            using(var db=new TargetNavigationDBEntities()){
            var model = new AuthShift
            {
                turnUserId = 2,
                acceptUserId = 1,
                planData =new int[]{1,2,3,4,5 },
                objectiveData = new int[] { 1, 2, 3, 4, 5 },
                FlowData = new int[] { 1, 2, 3, 4, 5 },
                userDocumentData = new int[] { 1, 2, 3 }
            };
            authManagementBLL.AuthShift(model);
            var plan = db.tblPlan.Where(c=>c.planId==1).FirstOrDefault();
            //验证
            Assert.AreEqual(plan.responsibleUser,1);
            }
        }
    }
}
