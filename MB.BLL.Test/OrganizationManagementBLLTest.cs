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
    [TestClass]
    public class OrganizationManagementBLLTest
    {
        private OrganizationManagementBLL orgManagementBll = new OrganizationManagementBLL();

        #region 获取组织架构列表(GetOrganizationList)
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetOrganizationList_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test001");

            var list = orgManagementBll.GetOrganizationList(string.Empty, null);

            Assert.AreEqual(list.Count,0);
        }

        /// <summary>
        /// DB中有相关数据，测试模糊查询及父Id为null的情况
        /// </summary>
        [TestMethod]
        public void GetOrganizationList_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");

            var list = orgManagementBll.GetOrganizationList("互联网", null);

            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].organizationId, 1);
            Assert.AreEqual(list[0].parentOrganization,null);
            Assert.AreEqual(list[0].schemaName, 1);
            Assert.AreEqual(list[0].organizationName, "互联网产业");
            Assert.AreEqual(list[0].orderNumber, 1);
            Assert.AreEqual(list[0].description, "好地方");
        }

        /// <summary>
        /// DB中有相关数据，测试模糊查询为空及父Id不为null的情况
        /// </summary>
        [TestMethod]
        public void GetOrganizationList_Test003()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationList_Test002");

            var list = orgManagementBll.GetOrganizationList(null, 1);

            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].organizationId, 3);
            Assert.AreEqual(list[0].parentOrganization, 1);
            Assert.AreEqual(list[0].schemaName, 3);
            Assert.AreEqual(list[0].organizationName, "目标导航事业部");
            Assert.AreEqual(list[0].orderNumber, 3);
            Assert.AreEqual(list[0].description, "就是干");
        } 
        #endregion

        #region 获取组织架构详情(GetOrganizationInfo)
         /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetOrganizationInfo_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationInfo_Test001");

            var model = orgManagementBll.GetOrganizationInfo(1);

            Assert.AreEqual(model, null);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetOrganizationInfo_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetOrganizationInfo_Test002");

            var model = orgManagementBll.GetOrganizationInfo(5);

            Assert.AreEqual(model.organizationId, 5);
            Assert.AreEqual(model.schemaName, 4);
            Assert.AreEqual(model.organizationName, "传统行业");
            Assert.AreEqual(model.parentOrganization, 2);
            Assert.AreEqual(model.description, "没戏");
        } 
        #endregion

        #region 新建/更新组织架构(SaveOrganization)
         /// <summary>
        /// 新建组织架构
        /// </summary>
        [TestMethod]
        public void SaveOrganization_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "SaveOrganization_Test001");

            var model = new OrgModel
            {
                parentOrganization = 16,
                schemaName = 3,
                organizationName = "智媒体",
                withSub = false,
                orderNumber = 1,
                description = "sdf"
            };
            orgManagementBll.SaveOrganization(model,247);
            //验证
            using (var db=new TargetNavigationDBEntities())
            {
                var selectModel = db.tblOrganization.FirstOrDefault();
                Assert.AreNotEqual(selectModel, null);
                Assert.AreEqual(selectModel.parentOrganization, 16);
                Assert.AreEqual(selectModel.schemaName, 3);
                Assert.AreEqual(selectModel.organizationName, "智媒体");
                Assert.AreEqual(selectModel.withSub, false);
                Assert.AreEqual(selectModel.orderNumber, 2);
                Assert.AreEqual(selectModel.description, "sdf");
            }
        }
        /// <summary>
        /// 更新组织架构
        /// </summary>
        [TestMethod]
        public void SaveOrganization_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "SaveOrganization_Test002");

            var model = new OrgModel
            {
                organizationId=6,
                parentOrganization = 17,
                schemaName = 3,
                organizationName = "智媒体",
                withSub = false,
                orderNumber = 1,
                description = "sdf"
            };
            orgManagementBll.SaveOrganization(model, 247);
            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var selectModel = db.tblOrganization.FirstOrDefault();
                Assert.AreNotEqual(selectModel, null);
                Assert.AreEqual(selectModel.organizationId, 6);
                Assert.AreEqual(selectModel.parentOrganization, 17);
                Assert.AreEqual(selectModel.schemaName, 3);
                Assert.AreEqual(selectModel.organizationName, "智媒体");
                Assert.AreEqual(selectModel.withSub, false);
                Assert.AreEqual(selectModel.orderNumber, 4);
                Assert.AreEqual(selectModel.description, "sdf");
            }
        }

        [TestMethod]
        public void SaveOrganization_Test003()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "SaveOrganization_Test002");

            var model = new OrgModel
            {
                organizationId = 18,
                parentOrganization = 17,
                schemaName = 3,
                organizationName = "智媒体",
                withSub = false,
                orderNumber = 1,
                description = "sdf"
            };
            orgManagementBll.SaveOrganization(model, 247);
            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var selectModel = db.tblOrganization.FirstOrDefault();
                Assert.AreNotEqual(selectModel, null);
                Assert.AreEqual(selectModel.organizationId, 6);
                Assert.AreEqual(selectModel.parentOrganization, 19);
                Assert.AreEqual(selectModel.schemaName, 4);
                Assert.AreEqual(selectModel.organizationName, "新能源汽车");
                Assert.AreEqual(selectModel.withSub, false);
                Assert.AreEqual(selectModel.orderNumber, 4);
                Assert.AreEqual(selectModel.description, "没吃早餐");
            }
        }
        #endregion

        #region 删除组织架构(DeleteOrganization)
        /// <summary>
        /// 有子组织无法删除，符合要求的直接删掉
        /// </summary>
        [TestMethod]
        public void DeleteOrganization_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "DeleteOrganization_Test001");
            var deleteIds = new int[] {6,8 };
            var msg = orgManagementBll.DeleteOrganization(deleteIds);

            Assert.AreEqual(msg, "1");
            using (var db=new TargetNavigationDBEntities())
            {
                var orgList = db.tblOrganization;
                Assert.AreEqual(orgList.Count(),3);
            }
        }
        /// <summary>
        /// 组织上有岗位无法删除，符合要求的直接删掉
        /// </summary>
        [TestMethod]
        public void DeleteOrganization_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "DeleteOrganization_Test002");
            var deleteIds = new int[] { 10, 11 };
            var msg = orgManagementBll.DeleteOrganization(deleteIds);

            Assert.AreEqual(msg, "2");
            using (var db = new TargetNavigationDBEntities())
            {
                var orgList = db.tblOrganization;
                Assert.AreEqual(orgList.Count(), 2);
            }
        }
        #endregion

        #region 组织架构排序(OrderOrganization)
        /// <summary>
        /// DB没有相关数据
        /// </summary>
        [TestMethod]
        public void OrderOrganization_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "OrderOrganization_Test001");
            var orgList = new List<OrgModel> { 
                new OrgModel{
                    organizationId=1,
                    orderNumber=2
                }
            };
            orgManagementBll.OrderOrganization(orgList);

            using (var db=new TargetNavigationDBEntities())
            {
                var orgListTest = db.tblOrganization;
                Assert.AreEqual(orgListTest.Count(), 0);
            }
        }
        /// <summary>
        /// DB有相关数据
        /// </summary>
        [TestMethod]
        public void OrderOrganization_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "OrderOrganization_Test002");
            var orgList = new List<OrgModel> { 
                new OrgModel{
                    organizationId=12,
                    orderNumber=1
                },
                new OrgModel{
                    organizationId=13,
                    orderNumber=2
                }
            };
            orgManagementBll.OrderOrganization(orgList);

            using (var db = new TargetNavigationDBEntities())
            {
                var orgListTest = db.tblOrganization.Where(p=>p.organizationId==12).FirstOrDefault();
                Assert.AreEqual(orgListTest.orderNumber, 1);
            }
        }
        #endregion

        #region 获取岗位列表(GetStationList)
        // <summary>
        // DB中没有相关数据
        // </summary>
        [TestMethod]
        public void GetStationList_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test001");

            var list = orgManagementBll.GetStationList(string.Empty, null,15);

            Assert.AreEqual(list.Count, 0);
        }

         //<summary>
         //DB中有相关数据，测试模糊查询情况
         //</summary>
        [TestMethod]
        public void GetStationList_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var list = orgManagementBll.GetStationList("开发", 14,2);

            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].stationId, 3);
            Assert.AreEqual(list[0].parentStation, null);
            Assert.AreEqual(list[0].stationName,"前端开发");
            Assert.AreEqual(list[0].organizationId, 14);
            Assert.AreEqual(list[0].approval, false);
            Assert.AreEqual(list[0].skipApproval, false);
        }

        [TestMethod]
        public void GetStationList_Test003()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationList_Test002");
            var list = orgManagementBll.GetStationList("开发", 14, 3);

            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].stationId, 2);
            Assert.AreEqual(list[0].parentStation, null);
            Assert.AreEqual(list[0].stationName, "软件开发");
            Assert.AreEqual(list[0].organizationId, 14);
            Assert.AreEqual(list[0].approval, false);
            Assert.AreEqual(list[0].skipApproval, false);
        }
        #endregion

        #region 获取岗位详情(GetStationInfo)
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetStationInfo_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationInfo_Test001");

            var model = orgManagementBll.GetStationInfo(1);

            Assert.AreEqual(model, null);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetStationInfo_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationInfo_Test002");

            var model = orgManagementBll.GetStationInfo(5);

            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.parentStation, 2);
            Assert.AreEqual(model.stationId, 5);
            Assert.AreEqual(model.stationName, "UI");
            Assert.AreEqual(model.organizationId, 15);
            Assert.AreEqual(model.comment, "东方时代");
            Assert.AreEqual(model.approval, false);
            Assert.AreEqual(model.skipApproval, false);
        }
        #endregion

        #region 删除岗位(DeleteStation)
        /// <summary>
        /// 有下级岗位无法删除
        /// </summary>
        [TestMethod]
        public void DeleteStation_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "DeleteStation_Test001");
            var id = new int[] {6};
            var msg = orgManagementBll.DeleteStation(id);

            Assert.AreEqual(msg, "1");
            using (var db = new TargetNavigationDBEntities())
            {
                var stationList = db.tblStation;
                Assert.AreEqual(stationList.Count(), 2);
            }
        }
        /// <summary>
        /// 岗位上有人员，无法删除
        /// </summary>
        [TestMethod]
        public void DeleteStation_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "DeleteStation_Test002");
            var id = new int[] {8};
            var msg = orgManagementBll.DeleteStation(id);

            Assert.AreEqual(msg, "1");
            using (var db = new TargetNavigationDBEntities())
            {
                var stationList = db.tblStation;
                Assert.AreEqual(stationList.Count(), 1);
            }
        }
        /// <summary>
        /// 岗位上既无下级岗位也没有人员，允许删除
        /// </summary>
        [TestMethod]
        public void DeleteStation_Test003()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "DeleteStation_Test003");
            var id = new int[] {9};
            var msg = orgManagementBll.DeleteStation(id);

            Assert.AreEqual(msg, string.Empty);
            using (var db = new TargetNavigationDBEntities())
            {
                var stationList = db.tblStation;
                Assert.AreEqual(stationList.Count(), 0);
            }
        }
        #endregion

        #region 新建/更新岗位(SaveStation)
        /// <summary>
        /// 新建岗位
        /// </summary>
        [TestMethod]
        public void SaveStation_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "SaveStation_Test001");
            var station = new StationModel
            {
                parentStation = 2,
                stationName = "需求",
                organizationId = 1,
                comment = "市场调研",
                approval = false,
                skipApproval = true
            };
            orgManagementBll.SaveStation(station, 247);
            using (var db = new TargetNavigationDBEntities())
            {
                var stationModel = db.tblStation.FirstOrDefault();
                Assert.AreNotEqual(stationModel, null);
                Assert.AreEqual(stationModel.parentStation, 2);
                Assert.AreEqual(stationModel.stationName, "需求");
                Assert.AreEqual(stationModel.organizationId, 1);
                Assert.AreEqual(stationModel.comment, "市场调研");
                Assert.AreEqual(stationModel.approval, false);
               // Assert.AreEqual(stationModel.skipApproval, true);
            }
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        [TestMethod]
        public void SaveStation_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "SaveStation_Test002");
            var station = new StationModel
            {
                stationId = 9,
                parentStation = 2,
                stationName = "需求",
                organizationId = 1,
                comment = "市场调研",
                approval = false,
                skipApproval = true
            };
            orgManagementBll.SaveStation(station, 247);
            using (var db = new TargetNavigationDBEntities())
            {
                var stationModel = db.tblStation.FirstOrDefault();
                Assert.AreNotEqual(stationModel, null);
                Assert.AreEqual(stationModel.stationId, 9);
                Assert.AreEqual(stationModel.parentStation, 2);
                Assert.AreEqual(stationModel.stationName, "需求");
                Assert.AreEqual(stationModel.organizationId, 1);
                Assert.AreEqual(stationModel.comment, "市场调研");
                Assert.AreEqual(stationModel.approval, false);
               // Assert.AreEqual(stationModel.skipApproval, true);
            }
        }
        #endregion

        #region 添加岗位人员(AddUser)
         //<summary>
         //先删除该岗位上的人员，后添加人员
         //</summary>
        [TestMethod]
        public void AddUser_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "AddUser_Test001");

            var deleteId = new int[] { 247, 110 };
            var addUser = new int[] { 249 };
            orgManagementBll.AddUser(10, addUser);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var userList = db.tblUserStation.ToList();
                Assert.AreEqual(userList.Count, 1);
                Assert.AreEqual(userList[0].stationId, 10);
                Assert.AreEqual(userList[0].userId, 249);
            }
        }
        #endregion

        #region 添加岗位手册(AddStationManual)
        /// <summary>
        /// 删除岗位手册，然后添加岗位手册
        /// </summary>
        [TestMethod]
        public void AddStationManual_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "AddStationManual_Test001");

            var deleteId = new int[] { 1 };
            var loopList = new List<LoopPlanInfo>{
                new LoopPlanInfo
            {
                stationId = 10,
                organizationId = 2,
                executionModeId = 3,
                eventOutput = "继续测试",
                confirmUser = 110,
                status = 2,
                loopWeek = "2",
                loopType =1,
                loopStatus = true
            },
             new LoopPlanInfo
            {
                stationId = 10,
                organizationId = 3,
                executionModeId = 4,
                eventOutput = "继续测试001",
                confirmUser = 110,
                status = 2,
                loopWeek = "2",
                loopType =1,
                loopStatus = true
            },
            
            };

            orgManagementBll.AddStationManual(deleteId, loopList, 247,11);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblLoopPlan.ToList();

                Assert.AreEqual(list.Count, 2);
                Assert.AreEqual(list[0].stationId, 11);
                Assert.AreEqual(list[0].organizationId, 2);
                Assert.AreEqual(list[0].executionModeId, 3);
                Assert.AreEqual(list[0].eventOutput, "继续测试");
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].loopWeek, "2");
                Assert.AreEqual(list[0].loopType, 1);
                Assert.AreEqual(list[0].loopStatus, true);
                Assert.AreEqual(list[0].createUser, 247);
                Assert.AreEqual(list[0].updateUser, 247);

                //验证循环计划完成表
                var resultList = db.tblLoopplanSubmit.ToList();
                Assert.AreEqual(resultList.Count, 2);
                Assert.AreEqual(resultList[0].completeQuantity, Convert.ToDecimal(1.0));
                Assert.AreEqual(resultList[0].completeQuality, Convert.ToDecimal(1.0));
                Assert.AreEqual(resultList[0].completeTime, Convert.ToDecimal(1.0));
                Assert.AreEqual(resultList[0].createUser, 247);
                Assert.AreEqual(resultList[0].updateUser, 247);
            }
        }
        #endregion

        #region 获取岗位手册列表(GetStationManual)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetStationManual_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationManual_Test001");

            var list = orgManagementBll.GetStationManual(13);
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// db中有相关数据
        /// </summary>
        [TestMethod]
        public void GetStationManual_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetStationManual_Test002");

            var list = orgManagementBll.GetStationManual(2);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].loopId, 2);
            Assert.AreEqual(list[0].stationId, 2);
            Assert.AreEqual(list[0].organizationId, 14);
            Assert.AreEqual(list[0].executionModeId, 3);
            Assert.AreEqual(list[0].eventOutput, "《测试》");
            Assert.AreEqual(list[0].confirmUser, 110);
        }
        #endregion

        
        /// <summary>
        /// 测试根据上级组织Id获取下级的组织列表
        /// </summary>
        [TestMethod]
        public void GetOrgListById_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetNextOrganizationById_Test001");
           var orgList=orgManagementBll.GetOrgListById(20,22);

            //验证
            Assert.AreEqual(orgList.Count,1);
        }

        [TestMethod]
        public void GetOrgListById_Test002()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetNextOrganizationById_Test001");
            var orgList = orgManagementBll.GetOrgListById(20, 21);

            //验证
            Assert.AreEqual(orgList.Count, 1);
        }

        /// <summary>
        /// 获取当前岗位下人员
        /// </summary>
        [TestMethod]
        public void GetUserList_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetUserList_Test001");
           var userList=orgManagementBll.GetUserList(14);

            //验证
           Assert.AreEqual(userList.Count(),3);
           Assert.AreEqual(userList[0].userName,"梁良");
          
        }
        
        /// <summary>
        /// 获取所有岗位人员
        /// </summary>
        [TestMethod]
        public void GetAllUser_Test001()
        {
            DataUtility.InsertDataBase("OrganizationManagementBLLTestData.xlsx", "GetAllUser_Test001");
            var userAll = orgManagementBll.GetAllUser();

            //验证
            Assert.AreEqual(userAll.Count,3);
            Assert.AreEqual(userAll[0].userName,"梁良");

        }
    }
}
