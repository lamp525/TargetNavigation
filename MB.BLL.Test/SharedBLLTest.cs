using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.DAL;
using MB.Model;

namespace MB.BLL.Test
{
    [TestClass]
    public class SharedBLLTest
    {
        MB.BLL.SharedBLL shareBll = new SharedBLL();

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


        //测试公司文档表新建文件夹
        [TestMethod]
        public void BuildNewFolder_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "BuildNewFolder_Test001");
            var folderName = "资本运营";
            var description = "好坑爹";
            var userId = 247;
            shareBll.BuildNewFolder(1, folderName, description, userId);
            var list = shareBll.GetFolderDirectory(null);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].name, "目录设置");
        }

        //测试个人文档表新建文件夹
        [TestMethod]
        public void BuildNewUserFolder_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "BuildNewUserFolder_Test001");
            var folderName = "资本运营";
            var description = "好坑爹";
            var userId = 247;
            shareBll.BuildNewUserFolder(null, folderName, description, userId);
            //验证
            var list = new List<tblUserDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblUserDocument.Where(p => !p.deleteFlag).ToList();
            }
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].displayName, "资本运营");
        }

        //测试新建公司文件
        [TestMethod]
        public void AddCompanyFile_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "AddCompanyFile_Test001");
            var newFileModel = new AddNewFileModel
            {
                type = 1,
                folder = 2,
                File = new List<FileModel> {
                    new FileModel{
                       // fileName="测试新建公司文件01",
                        extension="doc",
                        saveName="000001"
                    },
                    new FileModel{
                       // fileName="测试新建公司文件02",
                        extension="xls",
                        saveName="000002"
                    }
                },
                SheraUser =new int[]{247}
            };
           
            shareBll.AddCompanyFile(newFileModel, 247);

            //验证
            var docList = new List<tblCompanyDocument>();
            var logList = new List<tblCompanyDocumentLog>();
            using (var db = new TargetNavigationDBEntities())
            {
                docList = db.tblCompanyDocument.ToList();
                var docId = docList[0].documentId;
                logList = db.tblCompanyDocumentLog.Where(p => p.documentId == docId).ToList();
            }
            Assert.AreEqual(docList.Count, 2);
            Assert.AreEqual(docList[0].folder, 2);
            Assert.AreEqual(docList[0].displayName, null);
            Assert.AreEqual(docList[0].createUser, 247);
            Assert.AreEqual(logList.Count, 1);
            Assert.AreEqual(logList[0].type, 4);
        }

        //测试新建个人文件
        [TestMethod]
        public void AddUserFile_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "AddUserFile_Test001");
            var newFileModel = new AddNewFileModel
            {
                type = 2,
                folder = 2,
                File = new List<FileModel> {
                    new FileModel{
                        //fileName="测试新建公司文件01",
                        extension="doc",
                        saveName="000001"
                    },
                    new FileModel{
                        //fileName="测试新建公司文件02",
                        extension="xls",
                        saveName="000002"
                    }
                },
                SheraUser = new int[]{247}
            };
            shareBll.AddUserFile(newFileModel, 247);

            //验证
            var docList = new List<tblUserDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                docList = db.tblUserDocument.ToList();
            }
            Assert.AreEqual(docList.Count, 2);
            Assert.AreEqual(docList[0].folder, 2);          
            Assert.AreEqual(docList[0].createUser, 247);
        }

        //测试添加公司文档日志表数据
        //[TestMethod]
        public void AddCompanyDocumentLog_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "AddCompanyDocumentLog_Test001");
            using (var db = new TargetNavigationDBEntities())
            {
                var id = 2;
                var type = 2;
                var comment = "测试";
                var userId = 247;
                shareBll.AddCompanyDocumentLog(db, id, type, comment, userId);

                //验证
                var list = db.tblCompanyDocumentLog.ToList();
                Assert.AreEqual(list.Count, 2);
                Assert.AreEqual(list[1].comment, "测试");
            }

        }

        //测试获取公司文档第一级目录
        [TestMethod]
        public void GetFolderDirectory_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetFolderDirectory_Test001");
            var list = shareBll.GetFolderDirectory(null);

            //验证
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[1].id, 12);
            Assert.AreEqual(list[2].id, 13);
        }

        //测试公司文档根据上级文件夹Id获取下一级目录
        [TestMethod]
        public void GetFolderDirectory_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetFolderDirectory_Test001");
            var list = shareBll.GetFolderDirectory(12);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].id, 14);
            Assert.AreEqual(list[0].name, "测试取权限.doc");
            Assert.AreEqual(list[1].id, 15);
        }

        //测试公司文档获取第一级目录（含权限）
        [TestMethod]
        public void GetFolderDirectoryHasAuth_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetFolderDirectoryHasAuth_Test001");
            var list = shareBll.GetFolderDirectoryHasAuth(null, 248);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id, 17);
            Assert.AreEqual(list[0].name, "取目录含权限");

        }

        //测试公司文档根据上级文件夹Id获取下级目录（含权限）
        [TestMethod]
        public void GetFolderDirectoryHasAuth_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetFolderDirectoryHasAuth_Test002");
            var list = shareBll.GetFolderDirectoryHasAuth(16, 249);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id, 19);
            Assert.AreEqual(list[0].name, "取非第一级目录");

        }

        //测试模糊查查询公司文件夹
        [TestMethod]
        public void GetLikeFolderList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetLikeFolderList_Test001");
            var list = shareBll.GetLikeFolderList(1, "饼图",0);

            //验证
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].id, 52);
            Assert.AreEqual(list[0].name, "测试饼图04");
        }

        //测试模糊查查询个人文件夹
        [TestMethod]
        public void GetLikeFolderList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetLikeFolderList_Test002");
            var list = shareBll.GetLikeFolderList(2, "测试",50);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id, 46);
            Assert.AreEqual(list[0].name, "测试饼图02");
        }

        //测试根据文档Id获取文档集合,其中包含一个不存在的id(公司文档)
        [TestMethod]
        public void GetDocumentListByIds_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetDocumentListByIds_Test001");
            var documentIds = new int[3] { 21, 22, 45 };
            var list = shareBll.GetDocumentListByIds(documentIds);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 21);
            Assert.AreEqual(list[0].displayName, "测试取文档");
            Assert.AreEqual(list[0].saveName, "资本运营");
        }

        //测试根据文档Id获取文档集合,其中包含一个不存在的id（用户文档）
        [TestMethod]
        public void GetUserDocumentListByIds_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserDocumentListByIds_Test001");
            var documentIds = new int[3] { 2, 3, 45 };
            var list = shareBll.GetUserDocumentListByIds(documentIds);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].documentId, 2);
            Assert.AreEqual(list[0].displayName, "测试取个人文档");
            Assert.AreEqual(list[0].saveName, "XXC");
        }

        //测试获取用户文档一级目录
        [TestMethod]
        public void GetUserFolderDirectory_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserFolderDirectory_Test001");
            var list = shareBll.GetUserFolderDirectory(null, 247);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].id, 4);
            Assert.AreEqual(list[0].name, "测试个人目录");
        }

        //测试获取用户文档非一级目录
        [TestMethod]
        public void GetUserFolderDirectory_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserFolderDirectory_Test002");
            var list = shareBll.GetUserFolderDirectory(3, 247);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].id, 7);
            Assert.AreEqual(list[0].name, "测试非一级目录");
        }

        //测试最近5个上传文档的人员
        [TestMethod]
        public void GetLastFiveCreateUser_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetLastFiveCreateUser_Test001");
            var list = shareBll.GetLastFiveCreateUser();

            //验证
            Assert.AreEqual(list.Count, 5);
            Assert.AreEqual(list[0].id, 45);
            Assert.AreEqual(list[0].name, "郑子战");
        }

        //模糊查询
        [TestMethod]
        public void SelectUserList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var list = shareBll.SelectUserList("朱");

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id, 247);
            Assert.AreEqual(list[0].name, "朱之博");
        }

        //公司文档的复制,不含子目录
        [TestMethod]
        public void CopyDocument_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "CopyDocument_Test001");
            var documentIds = new int[] { 30, 31 };
            var folders = new int?[] { 5, 6 };
            shareBll.CopyDocument(documentIds, folders,null, 247,false);
            var list = new List<tblCompanyDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblCompanyDocument.ToList();
            }

            //验证
            Assert.AreEqual(list.Count, 6);
            Assert.AreEqual(list[2].folder, 5);
            Assert.AreEqual(list[2].displayName, "测试复制");
            Assert.AreEqual(list[2].description, "单体测试");
            Assert.AreEqual(list[2].extension, null);
            Assert.AreEqual(list[2].archive, null);
            Assert.AreEqual(list[2].archiveTime, null);
            Assert.AreEqual(list[2].isFolder, true);
            Assert.AreEqual(list[2].createUser, 247);
            Assert.AreEqual(list[2].updateUser, 247);
            Assert.AreEqual(list[2].deleteFlag, false);
        }

        //公司文档的复制,含子目录
        [TestMethod]
        public void CopyDocument_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "CopyDocument_Test002");
            var documentIds = new int[] {32,33,34};
            var folders = new int?[] { 5, 6 };
            shareBll.CopyDocument(documentIds, folders,null, 247,true);
            var list = new List<tblCompanyDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblCompanyDocument.ToList();
            }

            //验证
            Assert.AreEqual(list.Count, 10);
            Assert.AreEqual(list[2].folder, 32);
            Assert.AreEqual(list[2].displayName, "测试复制");
            Assert.AreEqual(list[2].description, "单体测试");
            Assert.AreEqual(list[2].extension,"doc");
            Assert.AreEqual(list[2].archive,false);
            Assert.AreEqual(list[2].archiveTime, null);
            Assert.AreEqual(list[2].isFolder, true);
            Assert.AreEqual(list[2].createUser, 46);
            Assert.AreEqual(list[2].updateUser, 249);
            Assert.AreEqual(list[2].deleteFlag, false);
        }

        /// <summary>
        /// 没有相关数据
        /// </summary>
        [TestMethod]
        public void CopyDocument_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "CopyDocument_Test001");
            var documentIds = new int[0];
            var folders = new int?[] { 5, 6 };
            shareBll.CopyDocument(documentIds, folders, null, 247, false);
            var list = new List<tblCompanyDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblCompanyDocument.ToList();
            }

            //验证
            Assert.AreEqual(list.Count, 2);
           
        }

        //个人文档复制，不含子目录
        [TestMethod]
        public void CopyUserDocument_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "CopyUserDocument_Test001");
            var documentIds = new int[] { 9, 10 };
            var folders = new int?[] { 5, 6 };
            shareBll.CopyUserDocument(documentIds,null, folders, 247);
            var list = new List<tblUserDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblUserDocument.ToList();
            }

            //验证
            Assert.AreEqual(list.Count, 6);
            Assert.AreEqual(list[2].folder, 5);
            Assert.AreEqual(list[2].displayName, "测试复制");
            Assert.AreEqual(list[2].description, "目标导航部门用");
            Assert.AreEqual(list[2].extension, null);
            Assert.AreEqual(list[2].isFolder, true);
            Assert.AreEqual(list[2].createUser, 247);
            Assert.AreEqual(list[2].updateUser, 247);
            Assert.AreEqual(list[2].deleteFlag, false);
        }

        //个人文档复制，含子目录
        [TestMethod]
        public void CopyUserDocument_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "CopyUserDocument_Test002");
            var documentIds = new int[] { 11, 12 };
            var folders = new int?[] { 5, 6 };
           shareBll.CopyUserDocument(documentIds, folders,null, 247);
            var list = new List<tblUserDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblUserDocument.ToList();
            }

            //验证
            Assert.AreEqual(list.Count, 6);
            Assert.AreEqual(list[0].folder, null);
            Assert.AreEqual(list[0].displayName, "测试复制");
            Assert.AreEqual(list[0].description, "目标导航部门用");
            Assert.AreEqual(list[0].extension, "doc");
            //Assert.AreEqual(list[6].archive, false);
            //Assert.AreEqual(list[6].archiveTime, null);
            Assert.AreEqual(list[0].isFolder, true);
            Assert.AreEqual(list[0].createUser, 247);
            Assert.AreEqual(list[0].updateUser, 247);
            Assert.AreEqual(list[0].deleteFlag, false);
        }

        //个人文档复制，公司文档复制到个人文档,不含子集
        [TestMethod]
        public void CopyUserDocument_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "CopyUserDocument_Test003");
            var documentIds = new int[] {48};
            var folders = new int?[] { 5, 6 };
             shareBll.CopyUserDocument( documentIds, folders,null, 51);
            var list = new List<tblUserDocument>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = db.tblUserDocument.ToList();
            }

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].folder, null);
            Assert.AreEqual(list[0].displayName, "测试饼图04");
            Assert.AreEqual(list[0].description, "文档管理");
            Assert.AreEqual(list[0].extension, "doc");
            //Assert.AreEqual(list[0].archive, false);
            //Assert.AreEqual(list[0].archiveTime, null);
            Assert.AreEqual(list[0].isFolder, true);
            Assert.AreEqual(list[0].createUser,51);
            Assert.AreEqual(list[0].updateUser, 248);
            Assert.AreEqual(list[0].deleteFlag, false);
        }

        //个人文档共享
        [TestMethod]
        public void ShareToOthers_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "ShareToOthers_Test001");
            var userIds = new int[] { 1, 110 };
            shareBll.ShareToOthers(new int[]{1}, userIds,0);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblUserDocument.Where(p => p.documentId == 17).FirstOrDefault();
                Assert.AreEqual(model.documentId,17);
                Assert.AreEqual(model.withShared, false);
                var list = db.tblDocumentShared.Where(p => p.documentId == 1).ToList();
                Assert.AreEqual(list.Count, 0);
        
            }
        }

        //无用户Id时，个人文档共享
        [TestMethod]
        public void ShareToOthers_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "ShareToOthers_Test001");
            var userIds = new int[] {};
            shareBll.ShareToOthers(new int[] { 1 }, userIds, 0);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblUserDocument.Where(p => p.documentId == 17).FirstOrDefault();
                Assert.AreEqual(model.documentId,17);
                Assert.AreEqual(model.withShared, false);
                var list = db.tblDocumentShared.Where(p => p.documentId == 1).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].userId, 1);
            
            }
        }

        // flag=1时，个人文档共享
        [TestMethod]
        public void ShareToOthers_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "ShareToOthers_Test001");
            var userIds = new int[] {1};
            shareBll.ShareToOthers(new int[] { 1,17}, userIds, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblUserDocument.Where(p => p.documentId == 17).FirstOrDefault();
                Assert.AreEqual(model.documentId, 17);
                Assert.AreEqual(model.withShared, true);
                var list = db.tblDocumentShared.Where(p => p.documentId == 1).ToList();
                Assert.AreEqual(list.Count, 0);
        

            }
        }

        //个人文档不共享
        [TestMethod]
        public void NoShareToOthers_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "NoShareToOthers_Test001");
            shareBll.NoShareToOthers(new int[] { 18 });

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblUserDocument.Where(p => p.documentId == 18).FirstOrDefault();
                Assert.AreNotEqual(model, null);
                Assert.AreEqual(model.withShared, false);
                var list = db.tblDocumentShared.Where(p => p.documentId == 18).ToList();
                Assert.AreEqual(list.Count, 0);
            }
        }

        //测试公司文档移动，含批量
        [TestMethod]
        public void MoveDocument_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "MoveDocument_Test001");
            var documentIds = new int[] { 38, 39,40,55,56,57};
            var folder = 5;
            shareBll.MoveDocument(documentIds, folder, 55);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblCompanyDocument.ToList();
                Assert.AreEqual(list.Count, 4);
                Assert.AreEqual(list[0].folder, 5);
            }
        }

      

        //测试个人文档移动，含批量
        [TestMethod]
        public void MoveUserDocument_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "MoveUserDocument_Test001");
            var documentIds = new int[] { 19, 20 };
            var folder = 5;
            shareBll.MoveUserDocument(documentIds, folder, 247);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblUserDocument.ToList();
                Assert.AreEqual(list.Count, 2);
                Assert.AreEqual(list[0].folder, 5);
            }
        }

        //测试删除（个人文档，含批量，有级联删除）
        [TestMethod]
        public void DeleteFlagDocument_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "DeleteFlagDocument_Test001");
            var documentIds = new int[] { 21, 22 };
            shareBll.DeleteFlagDocument(documentIds, 247, 2);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblUserDocument.ToList();
                Assert.AreEqual(list[0].deleteFlag, true);
                Assert.AreEqual(list[1].deleteFlag, true);
                Assert.AreEqual(list[2].deleteFlag, true);
            }
        }

        //测试删除（公司文档，含批量，有级联删除）
        [TestMethod]
        public void DeleteFlagDocument_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "DeleteFlagDocument_Test002");
            var documentIds = new int[] {40};
            shareBll.DeleteFlagDocument(documentIds, 61, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblCompanyDocument.ToList();
                Assert.AreEqual(list[0].deleteFlag, true);
                Assert.AreEqual(list[1].deleteFlag, true);
    
            }
        }

        [TestMethod]
        public void DeleteFlagDocument_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "DeleteFlagDocument_Test002");
            var documentIds = new int[] { 41 };
            shareBll.DeleteFlagDocument(documentIds, 61, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblCompanyDocument.ToList();
                Assert.AreEqual(list[0].deleteFlag, false);
                Assert.AreEqual(list[1].deleteFlag, false);
             
            }
        }

        [TestMethod]
        public void DeleteFlagDocument_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "DeleteFlagDocument_Test002");
            var documentIds = new int[] { 42};
            shareBll.DeleteFlagDocument(documentIds, 61, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblCompanyDocument.ToList();
                Assert.AreEqual(list[0].deleteFlag, false);
                Assert.AreEqual(list[1].deleteFlag, false);
                Assert.AreEqual(list[2].deleteFlag, true);
            }
        }

        //测试递归获取组织架构信息（拼接）结果
        [TestMethod]
        public void GetOrgStringByOrgId_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            using (var db = new TargetNavigationDBEntities())
            {
                var temp = shareBll.GetOrgStringByOrgId(db, 3, new List<string>());

                //验证
                Assert.AreEqual(temp, "互联网产业>目标导航事业部");
            }
        }

        //测试递归获取组织架构信息（拼接）结果,组织Id不存在的情况
        [TestMethod]
        public void GetOrgStringByOrgId_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            using (var db = new TargetNavigationDBEntities())
            {
                var temp = shareBll.GetOrgStringByOrgId(db, 1000, new List<string>());

                //验证
                Assert.AreEqual(temp, "");
            }
        }

        //测试获取岗位信息（拼接）结果
        [TestMethod]
        public void GetStationByStationId_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            using (var db = new TargetNavigationDBEntities())
            {
                var temp = shareBll.GetStationByStationId(db, 3);

                //验证
                Assert.AreEqual(temp, "能诚集团>前端开发");
            }
        }

        //测试获取岗位信息（拼接）结果,测试不存在该岗位的情况
        [TestMethod]
        public void GetStationByStationId_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            using (var db = new TargetNavigationDBEntities())
            {
                var temp = shareBll.GetStationByStationId(db, 1000);

                //验证
                Assert.AreEqual(temp, "");
            }
        }

        //测试根据上级组织Id获取下级的组织列表,测试第一级
        [TestMethod]
        public void GetOrgListById_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            var list = shareBll.GetOrgListById(null);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].id, 1);
            Assert.AreEqual(list[0].name, "互联网产业");
        }

        //测试根据上级组织Id获取下级的组织列表,测试非第一级
        [TestMethod]
        public void GetOrgListById_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            var list = shareBll.GetOrgListById(1);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id,3);
            Assert.AreEqual(list[0].name, "目标导航事业部");
        }

        //测试根据组织Id获取岗位列表(含下级)
        [TestMethod]
        public void GetStationListByOrgId_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var list = shareBll.GetStationListByOrgId(14);

            //验证
            Assert.AreEqual(list.Count, 4);
            Assert.AreEqual(list[0].organizationId, 14);
            Assert.AreEqual(list[0].organizationName, "能诚集团");
            Assert.AreEqual(list[0].stationId, 2);
            Assert.AreEqual(list[0].stationName, "软件开发");
        }

        //测试根据组织Id获取岗位列表(不含下级)
        [TestMethod]
        public void GetStationListByThisOrgId_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationListById_Test001");
            var list = shareBll.GetStationListByThisOrgId(24);

            //验证
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].organizationId, 24);
            Assert.AreEqual(list[0].organizationName, "能诚集团");
            Assert.AreEqual(list[0].stationId, 15);
            Assert.AreEqual(list[0].stationName, "需求");
        }

        //测试根据组织Id获取人员列表(含下级)
        [TestMethod]
        public void GetPersonListByOrgId_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var list = shareBll.GetPersonListByOrgId(14);

            //验证
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].organizationName, "能诚集团");
            Assert.AreEqual(list[0].userId, 247);
            Assert.AreEqual(list[0].userName, "朱之博");
            Assert.AreEqual(list[0].smallImage, "");
        }

        //测试根据组织Id获取人员列表(不含下级)
        [TestMethod]
        public void GetPersonListByThisOrgId_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetPersonListById_Test001");
            var list = shareBll.GetPersonListByThisOrgId(25);

            //验证
           Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].organizationName, "能诚集团");
            Assert.AreEqual(list[0].userId, 54);
            Assert.AreEqual(list[0].userName, "小小");
            Assert.AreEqual(list[0].smallImage, "");
        }

        //测试递归获取该组织架构下面的所有组织Id
        [TestMethod]
        public void GetAllorganizationIds_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            using (var db = new TargetNavigationDBEntities())
            {
                var list = shareBll.GetAllorganizationIds(1, db);

                //验证
                Assert.AreEqual(list.Count, 2);
                Assert.AreEqual(list[0], 1);
                Assert.AreEqual(list[1], 3);
            
            }

        }

        /// <summary>
        /// 测试删除图片
        /// </summary>
        [TestMethod]
        public void DeleteImage_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetImage_Test001");
            shareBll.DeleteImage(3);
        }

        /// <summary>
        /// 测试返回所有的该文件夹所有的上级文件夹列表
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
           var list=shareBll.getAllParentIds(63,documentList,1);

            //验证
           Assert.AreEqual(list.Count(),4);
 
        }

        /// <summary>
        ///测试 没有数据 
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(80, documentList, 1);

            //验证
            Assert.AreEqual(list.Count(), 0);

        }

        /// <summary>
        /// 测试  没有上级文件夹列表
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(60, documentList, 1);
    
            //验证
            Assert.AreEqual(list.Count(), 1);

        }

        /// <summary>
        /// 测试 返回用户文档的所有上级文档列表
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(50, documentList,2);

            //验证
            Assert.AreEqual(list.Count(), 2);

        }

        /// <summary>
        /// 没有上级文件夹列表
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test005()
        {

            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(49, documentList, 2);
            //验证
            Assert.AreEqual(list.Count(), 1);
        }

        /// <summary>
        /// 测试没有相关数据
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test006()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(60, documentList,2);

            //验证
            Assert.AreEqual(list.Count(), 0);

        }

        /// <summary>
        /// 测试获取所有的上级文件夹列表(重载)
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test007()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(63, documentList);

            //验证
            Assert.AreEqual(list.Count(), 4);

        }

        [TestMethod]
        public void getAllParentIds_Test008()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(60, documentList);

            //验证
            Assert.AreEqual(list.Count(), 1);

        }

        /// <summary>
        /// 没有相关数据
        /// </summary>
        [TestMethod]
        public void getAllParentIds_Test009()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "getAllParentIds_Test001");
            var documentList = new List<FileDirectoryModel>();
            var list = shareBll.getAllParentIds(80, documentList);

            //验证
            Assert.AreEqual(list.Count(), 0);

        }

        /// <summary>
        /// 测试权限设置模糊查询人员
        /// </summary>
        [TestMethod]
        public void GetUserListByName_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetUserListByName_Test001");
           var list=shareBll.GetUserListByName("李四");

            //验证
           Assert.AreEqual(list.Count(),1);
        }

        /// <summary>
        /// 测试获取文件共享人
        /// </summary>
        [TestMethod]
        public void GetUserList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetUserList_Test001");
            var list=shareBll.GetUserList(38);
            //验证
            Assert.AreEqual(list.Count(),4);
        }

        /// <summary>
        /// 测试 模糊查询岗位列表
        /// </summary>
        [TestMethod]
        public void GetStationListByName_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var list=shareBll.GetStationListByName("开发");
            //验证
            Assert.AreEqual(list.Count(),2);
        }

        /// <summary>
        /// 测试 模糊查询获取组织架构列表
        /// </summary>
        [TestMethod]
        public void GetOrgListByName_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");
            var list = shareBll.GetOrgListByName("互联网");
            //验证
            Assert.AreEqual(list.Count(),2);
        }

        /// <summary>
        /// 测试 拼接排序字符串
        /// </summary>
        [TestMethod]
        public void GetOrderStringBySort_Test001()
        {
            var sort = new Sort {
                type=1,
                direct=0
            };
            var str=shareBll.GetOrderStringBySort(sort);

            //验证
            Assert.AreEqual(str, " order by c.isFolder desc,c.displayName desc ");
        }
        [TestMethod]
        public void GetOrderStringBySort_Test002()
        {
            var sort = new Sort
            {
                type = 2,
                direct = 0
            };
            var str = shareBll.GetOrderStringBySort(sort);

            //验证
            Assert.AreEqual(str, " order by c.displayName desc ");
        }
        [TestMethod]
        public void GetOrderStringBySort_Test003()
        {
            var sort = new Sort
            {
                type = 3,
                direct = 0
            };
            var str = shareBll.GetOrderStringBySort(sort);

            //验证
            Assert.AreEqual(str, " order by c.createTime desc");
        }

        [TestMethod]
        public void GetOrderStringBySort_Test004()
        {
            var sort = new Sort
            {
                type = 4,
                direct = 0
            };
            var str = shareBll.GetOrderStringBySort(sort);

            //验证
            Assert.AreEqual(str, " order by isFolder desc ");
        }

        /// <summary>
        /// 获取新闻分类列表
        /// </summary>
        [TestMethod]
        public void GetNewsTypeList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsTypeList_Test008");
            var newsDir = shareBll.GetNewsDirectory(5);
            //验证
            Assert.AreEqual(newsDir.Count, 1);
           


        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNewsTypeList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsTypeList_Test008");
            var newsDir = shareBll.GetNewsDirectory(1);
            //验证
            Assert.AreEqual(newsDir.Count, 0);
        }

        /// <summary>
        /// 获取通知分类列表
        /// </summary>
        [TestMethod]
        public void GetNoticeTypeList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNoticeTypeList_Test009");
            var noticeDir = shareBll.GetNoticeDirectory(2);
            //验证
            Assert.AreEqual(noticeDir.Count, 1);

        }


        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNoticeTypeList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNoticeTypeList_Test009");
            var noticeDir = shareBll.GetNoticeDirectory(1);
            //验证
            Assert.AreEqual(noticeDir.Count, 0);
        }

        /// <summary>
        /// 测试 公司文档下载添加日志
        /// </summary>
        [TestMethod]
        public void AddDownloadLog_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "AddCompanyDocumentLog_Test001");
            shareBll.AddDownloadLog(2, 1, "下载文档到个人文档中", 247);
 
        }

        /// <summary>
        /// 测试 是否是系统管理员
        /// </summary>
        [TestMethod]
        public void GetAdmin_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "MoveDocument_Test001");
            var flag=shareBll.GetAdmin(55);

            //验证
            Assert.AreEqual(flag,false);
        }

        /// <summary>
        /// 获取公司文档日志
        /// </summary>
        [TestMethod]
        public void GetCompanyDocumenLogList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("BLLTestData.xlsx", "GetCompanyDocumenLogList_Test001");
            var list=shareBll.GetCompanyDocumenLogList(1);

            //验证
            Assert.AreEqual(list.Count(),3);
        }

      
    }
}
