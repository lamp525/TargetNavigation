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
    public class TemplateBLLTest
    {
        MB.BLL.TemplateBLL userDocBll = new BLL.TemplateBLL();

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
        public void GetSystemTemplateList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "GetSystemTemplateList_Test001");
            var list = userDocBll.GetTemplateList(null, true);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].templateId, 1);
            Assert.AreEqual(list[0].templateName, "请假流程");
            Assert.AreEqual(list[0].status, 1);
            Assert.AreEqual(list[0].description, "请假");
            Assert.AreEqual(list[0].categoryId, 1);
            Assert.AreEqual(list[0].system, true);
        }
        //测试获取系统表单列
        [TestMethod]
        public void GetSystemTemplateCaregoryList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "GetSystemTemplateList_Test002");
            var list = userDocBll.GetTemplateList(1, true);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].templateId, 2);
            Assert.AreEqual(list[0].templateName, "请假流程");
            Assert.AreEqual(list[0].status, 1);
            Assert.AreEqual(list[0].description, "请假");
            Assert.AreEqual(list[0].categoryId, 1);
            Assert.AreEqual(list[0].system, true);
        } 
        //测试新建表单(数据库)
        [TestMethod]
        public void InsertUserTemplate_Test001()
        {
            // 清空测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "InsertUserTemplate_Test00");
            var userDocument = new TemplateManageModel
            {
                templateId = 1,
                categoryId = 1,
                description = "请假",
                status = 1,
                system = false,
                templateName = "请假流程"

            };
            var flag = userDocBll.AddTemplate(userDocument); 
            //验证
            var model = new tblTemplate();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblTemplate.Where(p => p.templateId == 1).FirstOrDefault();
            }
            Assert.AreEqual(flag, 1);
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.templateId, 1);
            Assert.AreEqual(model.categoryId, 1);
            Assert.AreEqual(model.description, "请假");
            Assert.AreEqual(model.status, 1);
            Assert.AreEqual(model.system, false);
            Assert.AreEqual(model.templateName, "请假流程");

        }


        //测试获取分类获取第一级目录（含权限）
        [TestMethod]
        public void GetFolderCaregory_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "GetFolderCaregory_Test001");
            var list = userDocBll.GetTemplateCategoryList(true);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].categoryId, 1);
            Assert.AreEqual(list[0].categoryName, "请假流程");

        }
        /// <summary>
        /// 模糊查询测试
        /// </summary>
        [TestMethod]
        public void GettextCaregory()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "GettextCaregory");
            var list = userDocBll.GetSelectCategoryList("分类");

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].categoryId, 2);
            Assert.AreEqual(list[0].categoryName, "请假分类");
        }
        [TestMethod]
        public void copyTemplate()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "copyTemplate");
            int id = 2;
            int[] caregory = { 20 };
            userDocBll.CopyTemplate(id, caregory, false);

            var list = new List<tblTemplate>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblTemplate.ToList();
            }
            Assert.AreNotEqual(list, null);
            Assert.AreEqual(list[0].templateId, 1);
            Assert.AreEqual(list[0].categoryId, 20);
            Assert.AreEqual(list[0].description, "请假");
            Assert.AreEqual(list[0].status, 1);
            Assert.AreEqual(list[0].system, false);
            Assert.AreEqual(list[0].templateName, "请假流程");


        }
        [TestMethod]
        public void MoveTest()
        {
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "MoveTest");
            int[] id = {1};
            int caregory =  20 ;
            userDocBll.MoveTem(id, caregory);

            var list = new List<tblTemplate>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblTemplate.ToList();
            }
            Assert.AreNotEqual(list, null);
            Assert.AreEqual(list[0].templateId, 1);
            Assert.AreEqual(list[0].categoryId, 20);
            Assert.AreEqual(list[0].description, "请假");
            Assert.AreEqual(list[0].status, 1);
            Assert.AreEqual(list[0].system, false);
            Assert.AreEqual(list[0].templateName, "请假流程");

        }

        [TestMethod]
        public void delete()
        {
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "delete");
            int[] id = { 1}; 
            userDocBll.DeleteTem(id);

            var list = new List<tblTemplate>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblTemplate.ToList();
            }
            Assert.AreNotEqual(list, null);
            Assert.AreEqual(list[0].templateId, 1);
            Assert.AreEqual(list[0].categoryId, 30);
            Assert.AreEqual(list[0].description, "请假");
            Assert.AreEqual(list[0].status, 1);
            Assert.AreEqual(list[0].system, false);
            Assert.AreEqual(list[0].templateName, "请假流程");
            Assert.AreEqual(list[0].deleteFlag, false);
        }

        [TestMethod]
        public void delete_Test002()
        {
            DataUtility.InsertDataBase("TemplateListBLLTestData.xlsx", "delete");
            int[] id = { 2};
            userDocBll.DeleteTem(id);

            var list = new List<tblTemplate>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblTemplate.ToList();
            }
            Assert.AreNotEqual(list, null);
            Assert.AreEqual(list[0].templateId, 1);
            Assert.AreEqual(list[0].categoryId, 30);
            Assert.AreEqual(list[0].description, "请假");
            Assert.AreEqual(list[0].status, 1);
            Assert.AreEqual(list[0].system, false);
            Assert.AreEqual(list[0].templateName, "请假流程");
            Assert.AreEqual(list[0].deleteFlag, false);
        }
    }
}
