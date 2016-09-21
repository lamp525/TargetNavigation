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
    public class TemplateCaregoryBLLTest
    {
        TemplateCategoryBLL templateCaregorybll = new TemplateCategoryBLL();
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

        //测试获取分类列表
        [TestMethod]
        public void GetTemplateCaregotyList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetTemplateCaregotyList_Test001");
            var list = templateCaregorybll.GetCategoryList();

            //验证
            Assert.AreEqual(list.Count, 1);

        }
        //测试获取分类详情
        [TestMethod]
        public void GetCaregoryInfo_Test()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetTemplateCaregotyList_Test001");
            var list = templateCaregorybll.GetCareoryById(1);
            Assert.AreEqual(list.categoryId, 1);
            Assert.AreEqual(list.categoryName, "请假分类");
        }
        //测试新增分类
        [TestMethod]
        public void AddNewCaregory_Test()
        {
            // 清空测试数据 
            var userDocument = new TemplateCategoryModel
            {

               categoryId=1,
                categoryName="分类测试",
                 comment="测试",
                  
            };
            var flag = templateCaregorybll.AddCareoryById(userDocument);
            //验证
            var model = new  tblTemplateCategory();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblTemplateCategory.Where(p => p.categoryId == 1).FirstOrDefault();
            }
            Assert.AreEqual(flag, true);
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.categoryId, 1);
            Assert.AreEqual(model.categoryName, "请假分类");
            Assert.AreEqual(model.comment, "请假");
        }
        //测试更新分类
        [TestMethod]
        public void UpdateCaregory_Test()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetTemplateCaregotyList_Test001");
                 var userDocument = new TemplateCategoryModel
            {

               categoryId=1,
                categoryName="分类试",
                 comment="测试",
                  
            }; 
            templateCaregorybll.UpdateCaregoryById(userDocument);
            var model = new tblTemplateCategory();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblTemplateCategory.Where(p => p.categoryId == 1).FirstOrDefault();
            } 
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.categoryId, 1);
            Assert.AreEqual(model.categoryName, "分类试");
            Assert.AreEqual(model.comment, "测试");
        }
        //测试分类删除
        [TestMethod]
        public void DeleteCaregory_Test()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "GetTemplateCaregotyList_Test001");
            var flag = templateCaregorybll.DeleteCaregory(1);
            var model = new tblTemplateCategory();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblTemplateCategory.Where(p => p.categoryId == 1).FirstOrDefault();
            }
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.categoryId, 1);
            Assert.AreEqual(model.deleteFlag, true); 
        }
        //测试排序更新
        [TestMethod]
        public void UpdateOldNum()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowEntutsBLLTestData.xlsx", "UpdateOldNum");
            var List=new  List<TemplateCategoryOlderModel>();
            var test1 = new TemplateCategoryOlderModel();
            var test2 = new TemplateCategoryOlderModel();
            var test3 = new TemplateCategoryOlderModel();
            test1.categoryId = 1;
            test1.orderNum = 2;
            test2.categoryId = 2;
            test2.orderNum = 1;
            test3.categoryId = 3;
            test3.orderNum = 3;
            List.Add(test1);
            List.Add(test2);
            List.Add(test3);
            templateCaregorybll.UpdateOldNum(List);
            var model = new tblTemplateCategory();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblTemplateCategory.Where(p => p.categoryId == 1).FirstOrDefault();
            }
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.categoryId, 1);
            Assert.AreEqual(model.orderNum, 2); 
        }
    }
}
