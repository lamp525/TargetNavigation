using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Model;
using MB.BLL;
using MB.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MB.BLL.Test
{
    [TestClass]
    public class ObjectiveIndexBLLTest
    {
        private ObjectiveIndexBLL objectiveBll = new ObjectiveIndexBLL();

        #region 目标首页获取列表(GetObjectiveList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        //[TestMethod]
        //public void GetObjectiveList_Test001()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectiveList_Test001");

        //    var start = DateTime.MinValue;
        //    var end = DateTime.MaxValue;
        //    var list = objectiveBll.GetObjectiveList(string.Empty,start,end);

        //    Assert.AreEqual(list.Count,0);
        //}

        /// <summary>
        /// 获取我的目标
        /// </summary>
        //[TestMethod]
        //public void GetObjectiveList_Test002()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectiveList_Test002");

        //    var start = DateTime.MinValue;
        //    var end = DateTime.MaxValue;
        //    var conditionString = " responsibleUser==247 Or authorizedUser==247 ";
        //    var list = objectiveBll.GetObjectiveList(conditionString, start, end);

        //    Assert.AreEqual(list.Count, 2);
        //    Assert.AreEqual(list[0].objectiveId, 1);
        //    Assert.AreEqual(list[0].parentObjective, null);
        //    Assert.AreEqual(list[0].objectiveName, "我要成为亿万富翁");
        //    Assert.AreEqual(list[0].objectiveType, 2);
        //    Assert.AreEqual(list[0].bonus, 200);
        //    Assert.AreEqual(list[0].weight, 12);
        //    Assert.AreEqual(list[0].objectiveValue, "1500");
        //    Assert.AreEqual(list[0].expectedValue, "1500");
        //    Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-15"));
        //    Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-10"));
        //    Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-1"));
        //    Assert.AreEqual(list[0].responsibleUser, 247);
        //    Assert.AreEqual(list[0].responsibleUserName, "徐晓催");
        //    Assert.AreEqual(list[0].confirmUser, 110);
        //    Assert.AreEqual(list[0].confirmUserName, "梁良");
        //    Assert.AreEqual(list[0].objectiveTypeName, "互联网产业 - 目标导航事业部 ");
        //    Assert.AreEqual(list[0].authorizedUser, null);
        //    Assert.AreEqual(list[0].authorizedUserName, null);
        //    Assert.AreEqual(list[0].responsibleOrg, 10);
        //    Assert.AreEqual(list[0].checkType, 3);
        //    Assert.AreEqual(list[0].status, 1);
        //    Assert.AreEqual(list[0].progress, 20);
        //}

        /// <summary>
        /// 获取下属目标
        /// </summary>
        //[TestMethod]
        //public void GetObjectiveList_Test003()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectiveList_Test002");

        //    var start = DateTime.MinValue;
        //    var end = DateTime.MaxValue;
        //    var conditionString = " confirmUser==110 ";
        //    var list = objectiveBll.GetObjectiveList(conditionString, start, end);

        //    Assert.AreEqual(list.Count, 2);
        //    Assert.AreEqual(list[0].objectiveId, 1);
        //    Assert.AreEqual(list[0].parentObjective, null);
        //    Assert.AreEqual(list[0].objectiveName, "我要成为亿万富翁");
        //    Assert.AreEqual(list[0].objectiveType, 2);
        //    Assert.AreEqual(list[0].bonus, 200);
        //    Assert.AreEqual(list[0].weight, 12);
        //    Assert.AreEqual(list[0].objectiveValue, "1500");
        //    Assert.AreEqual(list[0].expectedValue, "1500");
        //    Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-15"));
        //    Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-10"));
        //    Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-1"));
        //    Assert.AreEqual(list[0].responsibleUser, 247);
        //    Assert.AreEqual(list[0].responsibleUserName, "徐晓催");
        //    Assert.AreEqual(list[0].confirmUser, 110);
        //    Assert.AreEqual(list[0].confirmUserName, "梁良");
        //    Assert.AreEqual(list[0].objectiveTypeName, "互联网产业 - 目标导航事业部 ");
        //    Assert.AreEqual(list[0].authorizedUser, null);
        //    Assert.AreEqual(list[0].authorizedUserName, null);
        //    Assert.AreEqual(list[0].responsibleOrg, 10);
        //    Assert.AreEqual(list[0].checkType, 3);
        //    Assert.AreEqual(list[0].status, 1);
        //    Assert.AreEqual(list[0].progress, 20);
        //}

        #endregion

        #region 根据当前目标Id获取子目标列表(GetChildrenObjectiveList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        //[TestMethod]
        //public void GetChildrenObjectiveList_Test001()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetChildrenObjectiveList_Test001");
        //    var list = objectiveBll.GetChildrenObjectiveList(1);

        //    Assert.AreEqual(list.Count, 0);
        //}

        /// <summary>
        /// db中有相关数据
        /// </summary>
        /// <param name="objectiveId">目标Id</param>
        //[TestMethod]
        //public void GetChildrenObjectiveList_Test002()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectiveList_Test002");
        //    var list = objectiveBll.GetChildrenObjectiveList(1);

        //    Assert.AreEqual(list.Count, 1);
        //    Assert.AreEqual(list[0].objectiveId, 3);
        //    Assert.AreEqual(list[0].parentObjective, 1);
        //    Assert.AreEqual(list[0].objectiveName, "我要成为亿万富翁");
        //    Assert.AreEqual(list[0].objectiveType, 2);
        //    Assert.AreEqual(list[0].bonus, 200);
        //    Assert.AreEqual(list[0].weight, 12);
        //    Assert.AreEqual(list[0].objectiveValue, "1500");
        //    Assert.AreEqual(list[0].expectedValue, "1500");
        //    Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-15"));
        //    Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-10"));
        //    Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-1"));
        //    Assert.AreEqual(list[0].responsibleUser, 247);
        //    Assert.AreEqual(list[0].responsibleUserName, "徐晓催");
        //    Assert.AreEqual(list[0].confirmUser, 110);
        //    Assert.AreEqual(list[0].confirmUserName, "梁良");
        //    Assert.AreEqual(list[0].objectiveTypeName, "互联网产业 - 目标导航事业部 ");
        //    Assert.AreEqual(list[0].authorizedUser, 247);
        //    Assert.AreEqual(list[0].authorizedUserName, "徐晓催");
        //    Assert.AreEqual(list[0].responsibleOrg, 10);
        //    Assert.AreEqual(list[0].checkType, 3);
        //    Assert.AreEqual(list[0].status, 1);
        //    Assert.AreEqual(list[0].progress, 20);
        //}
        #endregion

        #region 获取甘特图列表(GetGanttChartObjectiveList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        //[TestMethod]
        //public void GetGanttChartObjectiveList_Test001()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetGanttChartObjectiveList_Test001");

        //    var list = objectiveBll.GetGanttChartObjectiveList(DateTime.Now,DateTime.Now.AddDays(6),247);
        //    Assert.AreEqual(list.Count,0);
        //}

        /// <summary>
        /// 获取甘特图列表
        /// </summary>
        [TestMethod]
        public void GetGanttChartObjectiveList_Test002()
        {
            //DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetGanttChartObjectiveList_Test002");

            //var list = objectiveBll.GetGanttChartObjectiveList(Convert.ToDateTime("2015-8-10"), Convert.ToDateTime("2015-10-1"), 247,1);
            //Assert.AreEqual(list.Count, 3);
            //Assert.AreEqual(list[0].objectiveId, 6);
            //Assert.AreEqual(list[0].parentObjective, 1);
            //Assert.AreEqual(list[0].objectiveName, "我要成为亿万富翁");
            //Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-10-15"));
            //Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-11-10"));
            //Assert.AreEqual(list[0].responsibleUser, 247);
            //Assert.AreEqual(list[0].responsibleUserName, "徐晓催");
            //Assert.AreEqual(list[0].confirmUser, 110);
            //Assert.AreEqual(list[0].confirmUserName, "梁良");
            //Assert.AreEqual(list[0].status, 1);
            //Assert.AreEqual(list[0].progress, 20);
        }
        #endregion

        #region 删除目标(DeleteObjective)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void DeleteObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "DeleteObjective_Test001");

            var flag = objectiveBll.DeleteObjective(1,247);

            Assert.AreEqual(flag,false);
        }

        /// <summary>
        /// 删除目标
        /// </summary>
        [TestMethod]
        public void DeleteObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "DeleteObjective_Test002");

            var flag = objectiveBll.DeleteObjective(7, 247);

            using (var db=new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.Where(p => p.responsibleUser == 247).ToList();
                Assert.AreEqual(list.Count,0);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId==7).FirstOrDefault();
                Assert.AreNotEqual(operateModel,null);
                Assert.AreEqual(operateModel.objectiveId,7);
                Assert.AreEqual(operateModel.message,"");
                Assert.AreEqual(operateModel.result, 2);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 目标授权(AuthorizeObjective)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void AuthorizeObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "AuthorizeObjective_Test001");

            var flag = objectiveBll.AuthorizeObjective(1,111,247);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        /// 目标授权
        /// </summary>
        [TestMethod]
        public void AuthorizeObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "AuthorizeObjective_Test002");

            var flag = objectiveBll.AuthorizeObjective(8, 111, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.Where(p => p.responsibleUser == 247).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].authorizedUser, 111);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 8).FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 8);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 3);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 取消授权(CancelAuthorizeObjective)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void CancelAuthorizeObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "CancelAuthorizeObjective_Test001");

            var flag = objectiveBll.CancelAuthorizeObjective(9,247);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        /// 取消授权
        /// </summary>
        [TestMethod]
        public void CancelAuthorizeObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "CancelAuthorizeObjective_Test002");

            var flag = objectiveBll.CancelAuthorizeObjective(9, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.Where(p => p.objectiveId == 9).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].authorizedUser, null);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 9).FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 9);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 18);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 撤销操作(RevokeObjective)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void RevokeObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "RevokeObjective_Test001");

            var flag = objectiveBll.RevokeObjective(10,247);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        /// 撤销操作:待审核
        /// </summary>
        [TestMethod]
        public void RevokeObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "RevokeObjective_Test002");

            var flag = objectiveBll.RevokeObjective(10, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.Where(p => p.objectiveId == 10).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].status, 1);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 10).FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 10);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 5);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        /// 撤销操作:待确认
        /// </summary>
        [TestMethod]
        public void RevokeObjective_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "RevokeObjective_Test003");

            var flag = objectiveBll.RevokeObjective(11, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.Where(p => p.objectiveId == 11).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].status, 3);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 11).FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 11);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 5);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 新建目标提交保存操作(NewObjective)
        /// <summary>
        /// 保存操作,默认公式
        /// </summary>
        [TestMethod]
        public void NewObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "NewObjective_Test001");

            var addObjectiveModel = new AddNewObjectiveModel
            {
                parentObjective=null,
                objectiveName="就是逗你玩",
                valueType=2,
                objectiveType=2,
                bonus=500,
                weight=20,
                actualValue="2000",
                checkType=2,
                objectiveValue="2500",
                expectedValue="2400",
                description="不要给我装",
                maxValue=250,
                minValue=230,
                startTime=Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg=10,
                responsibleUser=247,
                confirmUser=110,
                formula=1
            };
            int id;
            objectiveBll.NewObjective(addObjectiveModel,247,2,out id);

            //验证
            using (var db=new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 1);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count,1);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 1);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
        }

        /// <summary>
        /// 提交操作,默认公式，确认人是登录用户
        /// </summary>
        [TestMethod]
        public void NewObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "NewObjective_Test002");

            var addObjectiveModel = new AddNewObjectiveModel
            {
                parentObjective = null,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };
            int id;
            objectiveBll.NewObjective(addObjectiveModel, 110, 1,out id);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 3);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 3);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 1);
                Assert.AreEqual(operateList[0].reviewUser, 110);
            }
        }

        /// <summary>
        /// 提交操作,自定义公式，确认人不是登录用户
        /// </summary>
        [TestMethod]
        public void NewObjective_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "NewObjective_Test003");

            var addObjectiveModel = new AddNewObjectiveModel
            {
                parentObjective = null,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 2,
                objectiveFormula = new List<ObjectiveFormula> { 
                    new ObjectiveFormula
                    {
                        formulaNum=1,
                        orderNum=1,
                        field=2,
                    },
                    new ObjectiveFormula
                    {
                        formulaNum=1,
                        orderNum=2,
                        operate=">",
                        numValue="20"
                    },
                    new ObjectiveFormula
                    {
                        formulaNum=1,
                        orderNum=3,
                        numValue="20"
                    }
                }
            };
            int id;
            objectiveBll.NewObjective(addObjectiveModel, 247, 1,out id);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 2);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 2);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 1);
                Assert.AreEqual(operateList[0].reviewUser, 247);

                var formulaList = db.tblObjectiveFormula.ToList();
                Assert.AreEqual(formulaList.Count, 3);
                Assert.AreEqual(formulaList[0].field, 2);
                Assert.AreEqual(formulaList[0].orderNum, 1);
                Assert.AreEqual(formulaList[0].operate, null);
                Assert.AreEqual(formulaList[0].numValue, null);
            }
        }
        #endregion

        #region 分解目标提交保存操作(SplitObjective)
        /// <summary>
        /// 保存操作,默认公式
        /// </summary>
        [TestMethod]
        public void SplitObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "SplitObjective_Test001");

            var addObjectiveModel = new AddNewObjectiveModel
            {
                parentObjective = 1,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };
            int id;
            objectiveBll.SplitObjective(addObjectiveModel, 247, 2,out  id);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].parentObjective, 1);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 1);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 2);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 1);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
        }

        /// <summary>
        /// 提交操作,默认公式，确认人是登录用户
        /// </summary>
        [TestMethod]
        public void SplitObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "SplitObjective_Test002");

            var addObjectiveModel = new AddNewObjectiveModel
            {
                parentObjective = 1,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };
            int id;
            objectiveBll.SplitObjective(addObjectiveModel, 110, 1,out id);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].parentObjective, 1);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 3);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 4);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 1);
                Assert.AreEqual(operateList[0].reviewUser, 110);
            }
        }

        /// <summary>
        /// 提交操作,自定义公式，确认人不是登录用户
        /// </summary>
        [TestMethod]
        public void SplitObjective_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "SplitObjective_Test003");

            var addObjectiveModel = new AddNewObjectiveModel
            {
                parentObjective = 1,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 2,
                objectiveFormula = new List<ObjectiveFormula> { 
                    new ObjectiveFormula
                    {
                        formulaNum=1,
                        orderNum=1,
                        field=2,
                    },
                    new ObjectiveFormula
                    {
                        formulaNum=1,
                        orderNum=2,
                        operate=">",
                        numValue="20"
                    },
                    new ObjectiveFormula
                    {
                        formulaNum=1,
                        orderNum=3,
                        numValue="20"
                    }
                }
            };
            int id;
            objectiveBll.SplitObjective(addObjectiveModel, 247, 1, out id);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].parentObjective, 1);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 2);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 3);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 1);
                Assert.AreEqual(operateList[0].reviewUser, 247);

                var formulaList = db.tblObjectiveFormula.ToList();
                Assert.AreEqual(formulaList.Count, 3);
                Assert.AreEqual(formulaList[0].field, 2);
                Assert.AreEqual(formulaList[0].orderNum, 1);
                Assert.AreEqual(formulaList[0].operate, null);
                Assert.AreEqual(formulaList[0].numValue, null);
            }
        }
        #endregion

        #region 修改目标数据(EditObjective)
        /// <summary>
        ///模型为空
        /// </summary>
        [TestMethod]
        public void EditObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test001");
            
            var flag = objectiveBll.EditObjective(new AddNewObjectiveModel(), 247, 1);

            Assert.AreEqual(flag, false);
        }
        /// <summary>
        ///db中没有数据
        /// </summary>
        [TestMethod]
        public void EditObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test002");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 1,
                parentObjective = null,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247, 1);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        ///保存：待提交
        /// </summary>
        [TestMethod]
        public void EditObjective_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test003");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 12,
                parentObjective = null,
                displayChangeFlag=false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 12);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 1);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        ///保存：待审核
        /// </summary>
        [TestMethod]
        public void EditObjective_Test004()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test004");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 13,
                parentObjective = null,
                displayChangeFlag = false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 13);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 1);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 1);
                Assert.AreEqual(operateList[0].objectiveId, 13);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 8);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        ///保存：审核通过
        /// </summary>
        [TestMethod]
        public void EditObjective_Test005()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test005");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 14,
                parentObjective = null,
                displayChangeFlag = false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247, 1);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 14);
                Assert.AreEqual(list[0].parentObjective, 1);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 3);
                Assert.AreEqual(list[0].status, 1);

                var changeList = db.tblObjectiveChange.ToList();
                Assert.AreEqual(changeList.Count, 1);
                Assert.AreEqual(changeList[0].objectiveId, 14);
                //Assert.AreEqual(changeList[0].objectiveName, "我要成为亿万富翁");
                //Assert.AreEqual(changeList[0].objectiveNameUpdate, true);
                //Assert.AreEqual(changeList[0].bonus, 200);
                //Assert.AreEqual(changeList[0].bonusUpdate, true);
                //Assert.AreEqual(changeList[0].weight, 12);
                //Assert.AreEqual(changeList[0].weightUpdate, true);
                //Assert.AreEqual(changeList[0].objectiveValue, "1500");
                //Assert.AreEqual(changeList[0].objectiveValueUpdate, true);
                //Assert.AreEqual(changeList[0].expectedValue, "1500");
                //Assert.AreEqual(changeList[0].expectedValueUpdate,true);
                //Assert.AreEqual(changeList[0].actualValue, "1600");
                //Assert.AreEqual(changeList[0].actualValueUpdate, true);
                //Assert.AreEqual(changeList[0].objectiveType, 2);
                //Assert.AreEqual(changeList[0].responsibleOrg, 10);
                //Assert.AreEqual(changeList[0].responsibleUser, 247);
                //Assert.AreEqual(changeList[0].confirmUser, 110);
                //Assert.AreEqual(changeList[0].checkType, 3);
                //Assert.AreEqual(changeList[0].startTime, Convert.ToDateTime("2015-10-15"));
                //Assert.AreEqual(changeList[0].startTimeUpdate,true);
                //Assert.AreEqual(changeList[0].endTime, Convert.ToDateTime("2015-11-10"));
                //Assert.AreEqual(changeList[0].endTimeUpdate, true);
                //Assert.AreEqual(changeList[0].alarmTime, Convert.ToDateTime("2015-9-1"));
                //Assert.AreEqual(changeList[0].alarmTimeUpdate,true);
                //Assert.AreEqual(changeList[0].progress, 20);
                //Assert.AreEqual(changeList[0].createUser, 247);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 1);
                Assert.AreEqual(operateList[0].objectiveId, 14);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 8);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        ///提交：待提交
        /// </summary>
        [TestMethod]
        public void EditObjective_Test006()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test006");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 15,
                parentObjective = null,
                displayChangeFlag = false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247,2);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 15);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 2);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 1);
                Assert.AreEqual(operateList[0].objectiveId, 15);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 4);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        ///提交：待审核,确认人不是登录用户
        /// </summary>
        [TestMethod]
        public void EditObjective_Test007()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test007");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 16,
                parentObjective = null,
                displayChangeFlag = false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247, 2);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 16);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 2);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 2);
                Assert.AreEqual(operateList[0].objectiveId, 16);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 4);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        ///提交：待审核,确认人是登录用户
        /// </summary>
        [TestMethod]
        public void EditObjective_Test008()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test007");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 16,
                parentObjective = null,
                displayChangeFlag = false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 110, 2);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 16);
                Assert.AreEqual(list[0].parentObjective, null);
                Assert.AreEqual(list[0].displayChangeFlag, false);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 2);
                Assert.AreEqual(list[0].status, 3);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 3);
                Assert.AreEqual(operateList[0].objectiveId, 16);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 4);
                Assert.AreEqual(operateList[0].reviewUser, 110);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        ///提交：审核通过
        /// </summary>
        [TestMethod]
        public void EditObjective_Test009()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "EditObjective_Test009");
            var addObjectiveModel = new AddNewObjectiveModel
            {
                objectiveId = 17,
                parentObjective = null,
                displayChangeFlag = false,
                objectiveName = "就是逗你玩",
                valueType = 2,
                objectiveType = 2,
                bonus = 500,
                weight = 20,
                actualValue = "2000",
                checkType = 2,
                objectiveValue = "2500",
                expectedValue = "2400",
                description = "不要给我装",
                maxValue = 250,
                minValue = 230,
                startTime = Convert.ToDateTime("2015-8-16"),
                endTime = Convert.ToDateTime("2015-9-16"),
                alarmTime = Convert.ToDateTime("2015-9-10"),
                //responsibleOrg = 10,
                responsibleUser = 247,
                confirmUser = 110,
                formula = 1
            };

            var flag = objectiveBll.EditObjective(addObjectiveModel, 247, 2);

            //验证
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 17);
                Assert.AreEqual(list[0].parentObjective, 1);
                Assert.AreEqual(list[0].displayChangeFlag, true);
                Assert.AreEqual(list[0].objectiveName, "就是逗你玩");
                Assert.AreEqual(list[0].objectiveType, 2);
                Assert.AreEqual(list[0].bonus, 500);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2400");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-8-16"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-9-16"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-10"));
                Assert.AreEqual(list[0].responsibleUser, 247);
                Assert.AreEqual(list[0].confirmUser, 110);
                Assert.AreEqual(list[0].responsibleOrg, 10);
                Assert.AreEqual(list[0].checkType, 3);
                Assert.AreEqual(list[0].status, 2);

                var changeList = db.tblObjectiveChange.ToList();
                //Assert.AreEqual(changeList.Count, 1);
                //Assert.AreEqual(changeList[0].objectiveId, 17);
                //Assert.AreEqual(changeList[0].objectiveName, "我要成为亿万富翁");
                //Assert.AreEqual(changeList[0].objectiveNameUpdate, true);
                //Assert.AreEqual(changeList[0].bonus, 200);
                //Assert.AreEqual(changeList[0].bonusUpdate, true);
                //Assert.AreEqual(changeList[0].weight, 12);
                //Assert.AreEqual(changeList[0].weightUpdate, true);
                //Assert.AreEqual(changeList[0].objectiveValue, "1500");
                //Assert.AreEqual(changeList[0].objectiveValueUpdate, true);
                //Assert.AreEqual(changeList[0].expectedValue, "1500");
                //Assert.AreEqual(changeList[0].expectedValueUpdate, true);
                //Assert.AreEqual(changeList[0].actualValue, "1600");
                //Assert.AreEqual(changeList[0].actualValueUpdate, true);
                //Assert.AreEqual(changeList[0].objectiveType, 2);
                //Assert.AreEqual(changeList[0].responsibleOrg, 10);
                //Assert.AreEqual(changeList[0].responsibleUser, 247);
                //Assert.AreEqual(changeList[0].confirmUser, 110);
                //Assert.AreEqual(changeList[0].checkType, 3);
                //Assert.AreEqual(changeList[0].startTime, Convert.ToDateTime("2015-10-15"));
                //Assert.AreEqual(changeList[0].startTimeUpdate, true);
                //Assert.AreEqual(changeList[0].endTime, Convert.ToDateTime("2015-11-10"));
                //Assert.AreEqual(changeList[0].endTimeUpdate, true);
                //Assert.AreEqual(changeList[0].alarmTime, Convert.ToDateTime("2015-9-1"));
                //Assert.AreEqual(changeList[0].alarmTimeUpdate, true);
                //Assert.AreEqual(changeList[0].progress, 20);
                Assert.AreEqual(changeList[0].createUser, 247);

                var operateList = db.tblObjectiveOperate.ToList();
                Assert.AreEqual(operateList.Count, 2);
                Assert.AreEqual(operateList[0].objectiveId, 17);
                Assert.AreEqual(operateList[0].message, "");
                Assert.AreEqual(operateList[0].result, 4);
                Assert.AreEqual(operateList[0].reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 删除目标文档(DeleteDocument)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void DeleteDocument_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "DeleteDocument_Test001");

            var flag = objectiveBll.DeleteDocument(1, 1, 247);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        /// 删除目标文档
        /// </summary>
        [TestMethod]
        public void DeleteDocument_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "DeleteDocument_Test002");

            var flag = objectiveBll.DeleteDocument(18, 1, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjectiveDocument.Where(p => p.objectiveId == 18).ToList();
                Assert.AreEqual(list.Count, 0);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 18).FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 18);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 16);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 文件上传成功后数据库插数据(InsertObjectiveDoc)
        /// <summary>
        /// 数据库中插入上传文件的信息
        /// </summary>
        [TestMethod]
        public void InsertObjectiveDoc_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "InsertObjectiveDoc_Test001");

            var file = new UploadFileModel
            {
                displayName = "目标首页",
                saveName = "215651156564",
                extension = ".doc"
            };
            objectiveBll.InsertObjectiveDoc(1, file, 247);

            //验证
            using (var db=new TargetNavigationDBEntities())
            {
                var list = db.tblObjectiveDocument.Where(p => p.objectiveId == 1).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 1);
                Assert.AreEqual(list[0].displayName, "目标首页");
                Assert.AreEqual(list[0].saveName, "215651156564");
                Assert.AreEqual(list[0].extension, ".doc");
                Assert.AreEqual(list[0].createUser, 247);
                var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 1).FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 1);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 15);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
        } 
        #endregion

        #region 甘特图拖动更新(MoveObjectiveGanttChart)
        /// <summary>
        /// db中没有数据
        /// </summary>
        //[TestMethod]
        //public void MoveObjectiveGanttChart_Test001()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "MoveObjectiveGanttChart_Test001");

        //    var flag = objectiveBll.MoveObjectiveGanttChart(1, DateTime.Now, DateTime.Now.AddDays(6), 247);

        //    Assert.AreEqual(flag, false);
       // }

        /// <summary>
        /// 甘特图拖动更新
        /// </summary>
        //[TestMethod]
        //public void MoveObjectiveGanttChart_Test002()
        //{
        //    DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "MoveObjectiveGanttChart_Test002");
        //    DateTime start = DateTime.Now;
        //    DateTime end = DateTime.Now.AddDays(6);
        //    var flag = objectiveBll.MoveObjectiveGanttChart(19, start, end, 247);

        //    using (var db=new TargetNavigationDBEntities())
        //    {
        //        var list = db.tblObjective.Where(p => p.objectiveId == 19).ToList();
        //        Assert.AreEqual(list.Count, 1);
        //        var operateModel = db.tblObjectiveOperate.Where(p => p.objectiveId == 19).FirstOrDefault();
        //        Assert.AreNotEqual(operateModel, null);
        //        Assert.AreEqual(operateModel.objectiveId, 19);
        //        Assert.AreEqual(operateModel.message, "");
        //        Assert.AreEqual(operateModel.result, 8);
        //        Assert.AreEqual(operateModel.reviewUser, 247);
        //    }
        //    Assert.AreEqual(flag, true);
        //} 
        #endregion

        #region 获取目标详情(GetObjectInfo)
        /// <summary>
        /// db中没有数据
        /// </summary>
        [TestMethod]
        public void GetObjectInfo_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectInfo_Test001");

            var objectiveModel = objectiveBll.GetObjectInfo(1);

            Assert.AreEqual(objectiveModel,null);

        }

        /// <summary>
        /// 获取目标详情,没有变更信息
        /// </summary>
        //[TestMethod]
        //public void GetObjectInfo_Test002()
       // {
            //DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectInfo_Test002");

            //var objectiveModel = objectiveBll.GetObjectInfo(20);

            ////验证目标信息
            //Assert.AreNotEqual(objectiveModel, null);
            //Assert.AreEqual(objectiveModel.objectiveId,20);
            //Assert.AreEqual(objectiveModel.parentObjective,1);
            //Assert.AreEqual(objectiveModel.displayChangeFlag,false);
            //Assert.AreEqual(objectiveModel.valueType,2);
            //Assert.AreEqual(objectiveModel.objectiveName,"我要成为亿万富翁");
            //Assert.AreEqual(objectiveModel.objectiveType,2);
            //Assert.AreEqual(objectiveModel.bonus,200);
            //Assert.AreEqual(objectiveModel.weight,12);
            //Assert.AreEqual(objectiveModel.objectiveValue,"1500");
            //Assert.AreEqual(objectiveModel.expectedValue,"1500");
            //Assert.AreEqual(objectiveModel.actualValue,"1600");
            //Assert.AreEqual(objectiveModel.startTime,Convert.ToDateTime("2015/10/15"));
            //Assert.AreEqual(objectiveModel.endTime,Convert.ToDateTime("2015/11/10"));
            //Assert.AreEqual(objectiveModel.alarmTime,Convert.ToDateTime("2015/9/1"));
            //Assert.AreEqual(objectiveModel.responsibleUser,247);
            //Assert.AreEqual(objectiveModel.responsibleUserName,"徐晓催");
            //Assert.AreEqual(objectiveModel.responsibleOrg,10);
            //Assert.AreEqual(objectiveModel.responsibleOrgName,"目标导航事业部");
            //Assert.AreEqual(objectiveModel.confirmUser,110);
            //Assert.AreEqual(objectiveModel.confirmUserName,"梁良");
            //Assert.AreEqual(objectiveModel.checkType,3);
            //Assert.AreEqual(objectiveModel.status,3);

            ////验证变更信息
            //Assert.AreEqual(objectiveModel.ChangeInfo,null);

            ////验证文档信息
            //Assert.AreEqual(objectiveModel.documentInfo.Count,3);
            //Assert.AreEqual(objectiveModel.documentInfo[0].documenType, 1);
            //Assert.AreEqual(objectiveModel.documentInfo[0].displayName, "目标首页");
            //Assert.AreEqual(objectiveModel.documentInfo[0].saveName, "2351515545");
            //Assert.AreEqual(objectiveModel.documentInfo[0].extension, ".doc");

            ////验证日志信息
            //Assert.AreEqual(objectiveModel.operateLog.Count, 3);
            //Assert.AreEqual(objectiveModel.operateLog[0].operateId, 1);
            //Assert.AreEqual(objectiveModel.operateLog[0].message, "");
            //Assert.AreEqual(objectiveModel.operateLog[0].result, 2);
            //Assert.AreEqual(objectiveModel.operateLog[0].reviewUser, 247);
            //Assert.AreEqual(objectiveModel.operateLog[0].reviewTime, Convert.ToDateTime("2015/8/19"));

            ////验证公式信息
            //Assert.AreEqual(objectiveModel.objectiveFormula.Count, 3);
            //Assert.AreEqual(objectiveModel.objectiveFormula[0].formulaId, 1);
            //Assert.AreEqual(objectiveModel.objectiveFormula[0].formulaNum, 1);
            //Assert.AreEqual(objectiveModel.objectiveFormula[0].orderNum, 1);
            //Assert.AreEqual(objectiveModel.objectiveFormula[0].field, 2);
            //Assert.AreEqual(objectiveModel.objectiveFormula[0].operate," ");
            //Assert.AreEqual(objectiveModel.objectiveFormula[0].numValue,"");
        //}

        /// <summary>
        /// 获取目标详情,有变更信息
        /// </summary>
        [TestMethod]
        public void GetObjectInfo_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectInfo_Test003");

            var objectiveModel = objectiveBll.GetObjectInfo(21);

            Assert.AreNotEqual(objectiveModel, null);
            //验证目标信息
            Assert.AreNotEqual(objectiveModel, null);
            Assert.AreEqual(objectiveModel.objectiveId, 21);
            Assert.AreEqual(objectiveModel.parentObjective, 1);
            Assert.AreEqual(objectiveModel.displayChangeFlag, true);
            Assert.AreEqual(objectiveModel.valueType, 2);
            Assert.AreEqual(objectiveModel.objectiveName, "我要成为亿万富翁");
            Assert.AreEqual(objectiveModel.objectiveType, 2);
            Assert.AreEqual(objectiveModel.bonus, 200);
            Assert.AreEqual(objectiveModel.weight, 12);
            Assert.AreEqual(objectiveModel.objectiveValue, "1500");
            Assert.AreEqual(objectiveModel.expectedValue, "1500");
            Assert.AreEqual(objectiveModel.actualValue, "1600");
            Assert.AreEqual(objectiveModel.startTime, Convert.ToDateTime("2015/10/15"));
            Assert.AreEqual(objectiveModel.endTime, Convert.ToDateTime("2015/11/10"));
            Assert.AreEqual(objectiveModel.alarmTime, Convert.ToDateTime("2015/9/1"));
            Assert.AreEqual(objectiveModel.responsibleUser, 247);
            Assert.AreEqual(objectiveModel.responsibleUserName, "徐晓催");
            Assert.AreEqual(objectiveModel.responsibleOrg, 10);
            Assert.AreEqual(objectiveModel.responsibleOrgName, "目标导航事业部");
            Assert.AreEqual(objectiveModel.confirmUser, 110);
            Assert.AreEqual(objectiveModel.confirmUserName, "梁良");
            Assert.AreEqual(objectiveModel.checkType, 3);
            Assert.AreEqual(objectiveModel.status, 2);

            //验证变更信息
            Assert.AreNotEqual(objectiveModel.ChangeInfo, null);
            Assert.AreEqual(objectiveModel.ChangeInfo.objectiveNameUpdate, false);
            Assert.AreEqual(objectiveModel.ChangeInfo.weightUpdate, true);
            Assert.AreEqual(objectiveModel.ChangeInfo.objectiveValueUpdate, true);
            Assert.AreEqual(objectiveModel.ChangeInfo.expectedValueUpdate, true);
            Assert.AreEqual(objectiveModel.ChangeInfo.bonusUpdate, false);
            Assert.AreEqual(objectiveModel.ChangeInfo.startTimeUpdate, false);
            Assert.AreEqual(objectiveModel.ChangeInfo.endTimeUpdate, true);
            Assert.AreEqual(objectiveModel.ChangeInfo.alarmTimeUpdate, false);

            //验证文档信息
            Assert.AreEqual(objectiveModel.documentInfo.Count, 3);
            Assert.AreEqual(objectiveModel.documentInfo[0].documenType, 1);
            Assert.AreEqual(objectiveModel.documentInfo[0].displayName, "目标首页");
            Assert.AreEqual(objectiveModel.documentInfo[0].saveName, "2351515545");
            Assert.AreEqual(objectiveModel.documentInfo[0].extension, ".doc");

            //验证日志信息
            Assert.AreEqual(objectiveModel.operateLog.Count, 3);
            Assert.AreEqual(objectiveModel.operateLog[0].operateId, 4);
            Assert.AreEqual(objectiveModel.operateLog[0].message, "");
            Assert.AreEqual(objectiveModel.operateLog[0].result, 2);
            Assert.AreEqual(objectiveModel.operateLog[0].reviewUser, 247);
            Assert.AreEqual(objectiveModel.operateLog[0].reviewTime, Convert.ToDateTime("2015/8/19"));

            //验证公式信息
            Assert.AreEqual(objectiveModel.objectiveFormula.Count, 3);
            Assert.AreEqual(objectiveModel.objectiveFormula[0].formulaId, 4);
            Assert.AreEqual(objectiveModel.objectiveFormula[0].formulaNum, 1);
            Assert.AreEqual(objectiveModel.objectiveFormula[0].orderNum, 1);
            Assert.AreEqual(objectiveModel.objectiveFormula[0].field, 2);
            Assert.AreEqual(objectiveModel.objectiveFormula[0].operate, " ");
            Assert.AreEqual(objectiveModel.objectiveFormula[0].numValue, "");

        } 
        #endregion

        #region 目标审核(ApproveObjective)
        /// <summary>
        /// db中没有数据
        /// </summary>
        [TestMethod]
        public void ApproveObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "ApproveObjective_Test001");

            //var flag = objectiveBll.ApproveObjective(1,"",6,247);

            //Assert.AreEqual(flag, false);
        }
        /// <summary>
        /// 审核通过
        /// </summary>
        [TestMethod]
        public void ApproveObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "ApproveObjective_Test002");

            //var flag = objectiveBll.ApproveObjective(22, "审核通过", 6, 110);
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].status,3);
                var operateModel = db.tblObjectiveOperate.FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 22);
                Assert.AreEqual(operateModel.message, "审核通过");
                Assert.AreEqual(operateModel.result, 6);
                Assert.AreEqual(operateModel.reviewUser, 110);
            }

            //Assert.AreEqual(flag, true);
        }

        /// <summary>
        /// 审核不通过
        /// </summary>
        [TestMethod]
        public void ApproveObjective_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "ApproveObjective_Test003");

            //var flag = objectiveBll.ApproveObjective(23,"不OK",7,110);
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].objectiveId, 23);
                Assert.AreEqual(list[0].objectiveName, "我要成为亿万富翁");
                Assert.AreEqual(list[0].bonus, 200);
                Assert.AreEqual(list[0].weight, 20);
                Assert.AreEqual(list[0].objectiveValue, "2500");
                Assert.AreEqual(list[0].expectedValue, "2500");
                Assert.AreEqual(list[0].startTime, Convert.ToDateTime("2015-10-15"));
                Assert.AreEqual(list[0].endTime, Convert.ToDateTime("2015-11-20"));
                Assert.AreEqual(list[0].alarmTime, Convert.ToDateTime("2015-9-1"));
                Assert.AreEqual(list[0].status, 1);
                var operateModel = db.tblObjectiveOperate.FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 23);
                Assert.AreEqual(operateModel.message, "不OK");
                Assert.AreEqual(operateModel.result, 7);
                Assert.AreEqual(operateModel.reviewUser, 110);
            }

            //Assert.AreEqual(flag, true);
        } 
        #endregion  

        #region 提交确认(SubmitObjectiveExecuteResult)
        /// <summary>
        /// db中没有数据
        /// </summary>
        [TestMethod]
        public void SubmitObjectiveExecuteResult_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "SubmitObjectiveExecuteResult_Test001");

            var flag = objectiveBll.SubmitObjectiveExecuteResult(1,"2000",247);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        /// 提交确认
        /// </summary>
        [TestMethod]
        public void SubmitObjectiveExecuteResult_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "SubmitObjectiveExecuteResult_Test002");

            var flag = objectiveBll.SubmitObjectiveExecuteResult(24, "2000", 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].status,4);
                Assert.AreEqual(list[0].actualValue, "2000");
                var operateModel = db.tblObjectiveOperate.FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 24);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 17);
                Assert.AreEqual(operateModel.reviewUser, 247);
            }
            Assert.AreEqual(flag, true);
        } 
        #endregion

        #region 确认(ConfirmObjective)
        /// <summary>
        /// db中没有数据
        /// </summary>
        [TestMethod]
        public void ConfirmObjective_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "ConfirmObjective_Test001");

            var flag = objectiveBll.ConfirmObjective(1,"",11,110);

            Assert.AreEqual(flag, false);
        }

        /// <summary>
        /// 确认通过
        /// </summary>
        [TestMethod]
        public void ConfirmObjective_Test002()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "ConfirmObjective_Test002");

            var flag = objectiveBll.ConfirmObjective(25,"",11,110);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].status, 5);
                var operateModel = db.tblObjectiveOperate.FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 25);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 11);
                Assert.AreEqual(operateModel.reviewUser, 110);
            }
            Assert.AreEqual(flag, true);
        }

        /// <summary>
        /// 确认不通过
        /// </summary>
        [TestMethod]
        public void ConfirmObjective_Test003()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "ConfirmObjective_Test003");

            var flag = objectiveBll.ConfirmObjective(26, "", 12, 110);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblObjective.ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].status, 3);
                var operateModel = db.tblObjectiveOperate.FirstOrDefault();
                Assert.AreNotEqual(operateModel, null);
                Assert.AreEqual(operateModel.objectiveId, 26);
                Assert.AreEqual(operateModel.message, "");
                Assert.AreEqual(operateModel.result, 12);
                Assert.AreEqual(operateModel.reviewUser, 110);
            }
            Assert.AreEqual(flag, true);
        }
        #endregion

        #region 获取饼图数量统计(GetObjectiveProcessList)
        /// <summary>
        /// 统计饼图数据
        /// </summary>
        [TestMethod]
        public void GetObjectiveProcessList_Test001()
        {
            DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectiveProcessList_Test001");
            var list = objectiveBll.GetObjectiveProcessList(2015, 10, 247);
            Assert.AreEqual(list.Count,5);
            Assert.AreEqual(list[0].id, 1);
            Assert.AreEqual(list[0].text, "待提交");
            Assert.AreEqual(list[0].count, 1);
            Assert.AreEqual(list[1].id, 2);
            Assert.AreEqual(list[1].text, "待审核");
            Assert.AreEqual(list[1].count, 2);
            Assert.AreEqual(list[2].id, 3);
            Assert.AreEqual(list[2].text, "已审核");
            Assert.AreEqual(list[2].count, 2);
            Assert.AreEqual(list[3].id, 4);
            Assert.AreEqual(list[3].text, "待确认");
            Assert.AreEqual(list[3].count, 2);
            Assert.AreEqual(list[4].id, 5);
            Assert.AreEqual(list[4].text, "已完成");
            Assert.AreEqual(list[4].count, 3);
            
        } 
        #endregion

    //    #region 获取目标不同状态的数量统计信息（GetObjectiveStatusList）
    //    /// <summary>
    //    /// 获取目标不同状态的数量统计信息
    //    /// </summary>
    //    [TestMethod]
    //    public void GetObjectiveStatusList_Test001()
    //    {
    //        DataUtility.InsertDataBase("ObjectiveIndexBLLTestData.xlsx", "GetObjectiveStatusList_Test001");
    //        var list = objectiveBll.GetObjectiveStatusList(247);
    //        Assert.AreEqual(list.Count, 6);
    //        Assert.AreEqual(list[0].id, 1);
    //        Assert.AreEqual(list[0].text, "待提交");
    //        Assert.AreEqual(list[0].count, 1);
    //        Assert.AreEqual(list[1].id, 2);
    //        Assert.AreEqual(list[1].text, "待审核");
    //        Assert.AreEqual(list[1].count, 2);
    //        Assert.AreEqual(list[2].id, 3);
    //        Assert.AreEqual(list[2].text, "已审核");
    //        Assert.AreEqual(list[2].count, 2);
    //        Assert.AreEqual(list[3].id, 4);
    //        Assert.AreEqual(list[3].text, "待确认");
    //        Assert.AreEqual(list[3].count, 2);
    //        Assert.AreEqual(list[4].id, 5);
    //        Assert.AreEqual(list[4].text, "已完成");
    //        Assert.AreEqual(list[4].count, 4);
    //        Assert.AreEqual(list[5].id, 6);
    //        Assert.AreEqual(list[5].text, "已超时");
    //        Assert.AreEqual(list[5].count, 2);
    //    } 
    //    #endregion
    }
}
