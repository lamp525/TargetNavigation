using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using System.Web;
using MB.DAL;

namespace MB.BLL.Test
{
    [TestClass]
    public class DocumentManagementBLLTest
    {
        MB.BLL.DocumentManagementBLL docBll = new MB.BLL.DocumentManagementBLL();

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

        //测试获取公司文档第一级目录的文档及默认降序
        [TestMethod]
        public void GetCompanyDocumentList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetCompanyDocumentList_Test001");
            var sort = new Sort {type=1,direct=0 };
            List<DocumentModel> list = docBll.GetCompanyDocumentList("c.folder is null", sort);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].documentId, 1);
            Assert.AreEqual(list[0].displayName, "目标导航.doc");
            Assert.AreEqual(list[0].description, "目标导航部门用");
            Assert.AreEqual(list[0].isFolder,true);
            Assert.AreEqual(list[0].createUser, 247);
            Assert.AreEqual(list[0].createUserName,null);
        }

        //测试根据文档名称筛选获取公司文档及按照名称降序
        [TestMethod]
        public void GetCompanyDocumentList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetCompanyDocumentList_Test002");
            var sort = new Sort { type = 2, direct = 0 };
            List<DocumentModel> list = docBll.GetCompanyDocumentList("c.displayName like '%doc%'", sort);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 2);
            Assert.AreEqual(list[0].displayName, "请假制度.doc");
            Assert.AreEqual(list[0].description, "请假制度");
            Assert.AreEqual(list[0].isFolder, false);
            Assert.AreEqual(list[0].createUser, 247);
            Assert.AreEqual(list[0].createUserName, null);
        }

        //测试加上所有筛选条件获取公司文档及按照创建时间降序
        [TestMethod]
        public void GetCompanyDocumentList_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetCompanyDocumentList_Test003");
            var sort = new Sort { type = 3, direct = 1 };
            List<DocumentModel> list = docBll.GetCompanyDocumentList("c.displayName like '%doc%' and c.folder=1 and c.createUser in (247,249) and c.createTime >='2015-5-1' and c.createTime <'2015-6-30' and c.deleteFlag='false'", sort);
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 7);
            Assert.AreEqual(list[0].displayName, "编辑.doc");
            Assert.AreEqual(list[0].description, "软件开发");
            Assert.AreEqual(list[0].isFolder, true);
            Assert.AreEqual(list[0].createUser, 247);
            Assert.AreEqual(list[0].createUserName, null);
        }

        //测试获取文件夹权限列表
        [TestMethod]
        public void GetAuthorityList_Test001()
        { 
            //导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx","GetAuthorityList_Test001");
            var model = docBll.GetAuthorityList(8);
            Assert.AreEqual(model.AuthorityList.Count,3);
            Assert.AreEqual(model.AuthorityList[0].resultId.Length,3);
            Assert.AreEqual(model.displayName, "编辑1.doc");
            Assert.AreEqual(model.description, "软件开发");
        }

        //测试没有设置过权限的文件夹的空权限列表
        [TestMethod]
        public void GetAuthorityList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetAuthorityList_Test002");
            var model = docBll.GetAuthorityList(9);
            Assert.AreEqual(model.AuthorityList, null);
            Assert.AreEqual(model.displayName, "测试取权限.doc");
            Assert.AreEqual(model.description, "单体测试");
        }

        //测试设置文件夹权限
        [TestMethod]
        public void SetAuthority_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetAuthorityList_Test001");
            var deleteIds = new int[]{1,2 };
            var authy = new AuthorityInfoModel
            {
                documentId = 8,
                displayName = "编辑1.doc",
                description = "软件开发",
                AuthorityList = new List<AuthorityModel> { 
                    new AuthorityModel{
                        type=1,
                        power=4,
                        targetId=new int?[]{7,8,9},
                    },
                    new AuthorityModel{
                        type=2,
                        power=2,
                        targetId=new int?[]{10,11,12}
                    },
                    new AuthorityModel{
                        type=3,
                        power=1,
                        targetId=new int?[]{110,44,45}
                    }
                }
            };
            docBll.SetAuthority(deleteIds, authy, 247);
            //验证
            var model = docBll.GetAuthorityList(8);
            Assert.AreEqual(model.AuthorityList.Count, 6);
            Assert.AreEqual(model.AuthorityList[0].resultId.Length,1);
            Assert.AreEqual(model.displayName, "编辑1.doc");
            Assert.AreEqual(model.description, "软件开发");
        }

        //测试拼接部门信息
        [TestMethod]
        public void GetOrgInfoById_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            var orgIds =new int[]{1,2,3,4};
            var orgNames=docBll.GetOrgInfoById(orgIds);

            //验证
            Assert.AreEqual(orgNames[2], "互联网产业>目标导航事业部");
            Assert.AreEqual(orgNames[3], "互联网+>传统行业");
        }

        //测试拼接岗位信息
        [TestMethod]
        public void GetStationInfoById_Test001()
        {
           //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var stationIds = new int[] {2,3};
           var  stationNames = docBll.GetStationInfoById(stationIds);
           //验证
           Assert.AreEqual(stationNames[0], "能诚集团>软件开发");
           Assert.AreEqual(stationNames[1], "能诚集团>前端开发");
        }
    }
}
