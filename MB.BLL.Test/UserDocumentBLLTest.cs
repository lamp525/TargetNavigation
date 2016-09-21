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
    public class UserDocumentBLLTest
    {

        MB.BLL.UserDocumentBLL userDocBll = new BLL.UserDocumentBLL();

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

        //测试获取公司文档列表（含权限）及默认排序
        [TestMethod]
        public void GetCompanyDocumentList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetCompanyDocumentList1_Test001");
            var sort = new Sort {type=1,direct=0 };
            var list = userDocBll.GetCompanyDocumentList("c.folder is null", sort, 110);

            //验证
            Assert.AreEqual(list.Count,1);
            Assert.AreEqual(list[0].documentId, 44);
            Assert.AreEqual(list[0].folder, null);
            Assert.AreEqual(list[0].displayName, "测试权限02");
            Assert.AreEqual(list[0].description, "单体测试"); 
            Assert.AreEqual(list[0].isFolder, false);
            Assert.AreEqual(list[0].createUser, 47);
            Assert.AreEqual(list[0].createUserName, null);
            Assert.AreEqual(list[0].power,null);
        }

        //测试获取个人文档列表
        [TestMethod]
        public void GetUserDocumentList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserDocumentList_Test001");
            var sort = new Sort { type = 1, direct = 0 };
            var list = userDocBll.GetUserDocumentList("c.folder is null", 2,sort, 247);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 25);
            Assert.AreEqual(list[0].folder, null);
            Assert.AreEqual(list[0].displayName, "测试取列表02");
            Assert.AreEqual(list[0].description, "文档管理");
            Assert.AreEqual(list[0].isFolder, true);
            Assert.AreEqual(list[0].withShared,true);
            Assert.AreEqual(list[0].createUser, 247);
            Assert.AreEqual(list[0].createUserName, "徐晓催");
            Assert.AreEqual(list[0].power, null);
        }

        //测试获取我的共享列表
        [TestMethod]
        public void GetUserDocumentList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserDocumentList_Test002");
            var sort = new Sort { type = 1, direct = 0 };
            var list = userDocBll.GetUserDocumentList("c.folder is null", 3, sort, 247);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 29);
            Assert.AreEqual(list[0].folder, null);
            Assert.AreEqual(list[0].displayName, "测试取列表02");
            Assert.AreEqual(list[0].description, "文档管理");
            Assert.AreEqual(list[0].isFolder, true);
            Assert.AreEqual(list[0].withShared, true);
            Assert.AreEqual(list[0].createUser, 247);
            Assert.AreEqual(list[0].createUserName, "徐晓催");
            Assert.AreEqual(list[0].power, null);
        }

        //测试获取他人共享列表
        [TestMethod]
        public void GetUserDocumentList_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserDocumentList_Test003");
            var sort = new Sort { type = 1, direct = 0 };
            var list = userDocBll.GetUserDocumentList("c.folder is null", 4, sort, 247);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 33);
            Assert.AreEqual(list[0].folder, null);
            Assert.AreEqual(list[0].displayName, "测试取列表02");
            Assert.AreEqual(list[0].description, "文档管理");
            Assert.AreEqual(list[0].isFolder, true);
            Assert.AreEqual(list[0].withShared, true);
            Assert.AreEqual(list[0].createUser, 5);
            Assert.AreEqual(list[0].createUserName, "朱之博");
            Assert.AreEqual(list[0].power, null);
        }

        //测试获取文档统计信息（饼图）
        [TestMethod]
        public void GetDocumentStatisticsInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetDocumentStatisticsInfo_Test001");
            var list = userDocBll.GetDocumentStatisticsInfo(247);

            //验证
            Assert.AreEqual(list[0].count,1);
            Assert.AreEqual(list[1].count, 0);
            Assert.AreEqual(list[2].count, 1);
         
        }

        //测试新建用户文档(数据库操作，不包含上传动作)
        [TestMethod]
        public void InsertUserDocument_Test001()
        {
            // 清空测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "InsertUserDocument_Test001");
            var userDocument = new UserDocument
            {
                documentId = 39,
                displayName = "测试新建文档",
                description = "描述",
                saveName = "存储名",
                extension = "后缀名",
                withShared = false,
                isFolder = false,
                createUser = 247,
                createTime = DateTime.Now,
                updateUser = 247,
                updateTime = DateTime.Now,
                deleteFlag = false
            };
            var flag=userDocBll.InsertUserDocument(userDocument);

            //验证
            var model = new tblUserDocument();
            using (var db = new TargetNavigationDBEntities())
            {
                model = db.tblUserDocument.Where(p=>p.documentId==39).FirstOrDefault();
            }
            Assert.AreEqual(flag,true);
            Assert.AreNotEqual(model,null);
            Assert.AreEqual(model.documentId, 39);
            Assert.AreEqual(model.displayName, "测试新建文档");
            Assert.AreEqual(model.description, "描述");
            Assert.AreEqual(model.saveName, "存储名");
            Assert.AreEqual(model.extension, "后缀名");
            Assert.AreEqual(model.withShared, false);
            Assert.AreEqual(model.isFolder, false);
            Assert.AreEqual(model.createUser, 247);
            Assert.AreEqual(model.updateUser, 247);
            Assert.AreEqual(model.deleteFlag, false);
        }

        //新建用户文档 返回错误信息
        //[TestMethod]
        //public void InsertUserDocument_Test002()
        //{
        //    var model = new UserDocument
        //    {

        //        documentId = 39,
        //        displayName = "测试新建文档"

        //    };
        //    var flag = userDocBll.InsertUserDocument(model);
        //    Assert.AreEqual(flag, false);
        //}

       
    }
}
