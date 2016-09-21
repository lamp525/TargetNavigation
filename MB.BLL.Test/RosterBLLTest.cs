using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
using MB.BLL;

namespace MB.BLL.Test
{
    [TestClass]
   public  class RosterBLLTest
    {
        RosterBLL rosterBLL = new RosterBLL();

        /// <summary>
        /// 测试用户信息详情
        /// </summary>
        [TestMethod]
        public void GetRosterInfo_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");
            var model=rosterBLL.GetRosterInfo(1);

            //验证
            Assert.AreNotEqual(model,null);
            Assert.AreEqual(model.userName,"张三");
            Assert.AreEqual(model.workStatus,1);
            Assert.AreEqual(model.sex,false);
        }

        /// <summary>
        /// 测试用户列表取得(用户)
        /// </summary>
        [TestMethod]
        public void GetRosterList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterList_Test001");
            var modelList = rosterBLL.GetRosterList();

            //验证
            Assert.AreNotEqual(modelList.Count,0);
            Assert.AreEqual(modelList[0].station[0].stationId,2);
            Assert.AreEqual(modelList[0].station[0].stationName, "市场调研");
            Assert.AreEqual(modelList[0].station[1].stationId,3);
            Assert.AreEqual(modelList[0].station[1].stationName, "前端工程师");
        }

        /// <summary>
        /// 测试某个用户列表(用户)
        /// </summary>
        [TestMethod]
        public void GetRosterList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterList_Test001");
            var modelList = rosterBLL.GetRosterList("李四");

            //验证
            Assert.AreNotEqual(modelList.Count, 0);
            Assert.AreEqual(modelList[0].station[0].stationId, 2);
            Assert.AreEqual(modelList[0].station[0].stationName, "市场调研");
            Assert.AreEqual(modelList[0].station[1].stationId, 3);
            Assert.AreEqual(modelList[0].station[1].stationName, "前端工程师");
        }

        /// <summary>
        /// 测试用户列表取得(组织)
        /// </summary>
        [TestMethod]
        public void GetRosterOrgList_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterList_Test001");
            var modelList = rosterBLL.GetRosterOrgList(1,2);

            //验证
            Assert.AreNotEqual(modelList.Count,0);
        }

        /// <summary>
        /// 测试搜索某个用户列表(组织)
        /// </summary>
        [TestMethod]
        public void GetRosterOrgList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterList_Test001");
            var modelList = rosterBLL.GetRosterOrgList(1,2,"那英");

            //验证
            Assert.AreNotEqual(modelList.Count, 0);
        }

        /// <summary>
        /// 测试根据银行卡号查找银行名称
        /// </summary>
        [TestMethod]
        public void GetBankName_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");
            var strname = rosterBLL.GetBankName("62284856785654787");

            //验证
            Assert.AreEqual(strname,"农行");
        }

        /// <summary>
        /// 根据身份证获取省份、城市、地区
        /// </summary>
        [TestMethod]
        public void GetTownByidentityCard_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");

            var strList = rosterBLL.GetTownByidentityCard("430121199109219876");

            //验证
            Assert.AreNotEqual(strList.Count,0);
            Assert.AreEqual(strList[0],"湖南");
            Assert.AreEqual(strList[1],"长沙");
        }

        /// <summary>
        /// 测试新建/更新用户信息
        /// </summary>
        [TestMethod]
        public void SaveRosterInfo_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");
            var model = new RosterModel {
              userNumber="erew2345",
                userName ="李娜",
                password = Common.StringUtils.GetAllRandom(8),
                sex = false,
                  nation ="汉",
                       political ="团员",
                       marriage = false,
                      mobile1 = "123456",
                     mobile2 = "234567",
                     address = "asgdhsag",
                    workPlace = "abc",
                      entryTime = DateTime.Now,
                     probationaryPeriod = 3,
                      positiveDate = null,
                   term = 3,
                      expiredDate = null,
                      comment = null,
                       birthday = null,
                      nature = null,
                provinceId =33,
                cityId = 3301,
                districtId =330105,
                school ="aaa",
                      professional = "计算机",
                    education =1,
                     firstWork = null,
                     qualification =null,
                     cornet =null,
                      emergencyNumber = "123456",
                identityCard = "3301056746587634765",
                bankCard ="622848454363213245",
                workStatus = 1,
                  updateTime = DateTime.Now,
                    updateUser =1
            };
            rosterBLL.SaveRosterInfo(model,1);

            var modelList = rosterBLL.GetRosterList("李娜");
            //验证
            Assert.AreNotEqual(modelList[0],null);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        [TestMethod]
        public void SaveRosterInfo_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");
            var model = new RosterModel
            {
                userId=1,
                userNumber="retw4567",
                userName = "李娜",
                password = Common.StringUtils.GetAllRandom(8),
                sex = false,
                nation = "汉",
                political = "团员",
                marriage = false,
                mobile1 = "123456",
                mobile2 = "234567",
                address = "asgdhsag",
                workPlace = "abc",
                entryTime = DateTime.Now,
                probationaryPeriod = 3,
                positiveDate = null,
                term = 3,
                expiredDate = null,
                comment =" ",
                birthday = null,
                nature = " ",
                provinceId = 33,
                cityId = 3301,
                districtId = 330105,
                school = "aaa",
                professional = "计算机",
                education = 1,
                firstWork = null,
                qualification ="abc",
                cornet = "123456",
                emergencyNumber = "123456",
                identityCard = "3301056746587634765",
                bankCard = "622848454363213245",
                workStatus = 1
               
            };
            rosterBLL.SaveRosterInfo(model, 1);

            var modelList = rosterBLL.GetRosterList("李娜");
            //验证
            Assert.AreNotEqual(modelList[0], null);
            Assert.AreEqual(modelList[0].userId,1);
            Assert.AreEqual(modelList[0].userName,"李娜");
        }

        /// <summary>
        /// 测试根据用户id更新状态
        /// </summary>
        [TestMethod]
        public void UpdateWorkStatusById_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");
            rosterBLL.UpdateWorkStatusById(1, 2, DateTime.Now);

            var model = rosterBLL.GetRosterInfo(1);
            //验证
            Assert.AreEqual(model.workStatus,2);

        }

        /// <summary>
        /// 测试删除用户
        /// </summary>
        [TestMethod]
        public void DeleteUser_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterInfo_Test001");
            rosterBLL.DeleteUser(1);

            var model = rosterBLL.GetRosterList("张三");
            //验证
            Assert.AreEqual(model.Count,0);
        }

        /// <summary>
        /// 测试导出用户信息
        /// </summary>
        [TestMethod]
        public void ExportFile_Test001()
        {
 //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterList_Test001");
            var modelList = rosterBLL.ExportFile(2);

            //验证
            Assert.AreNotEqual(modelList.Count,0);

        }

        [TestMethod]
        public void ExportFile_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("RosterBLLTestData.xlsx", "GetRosterList_Test001");
            var modelList = rosterBLL.ExportFile(2,"那英");

            //验证
            Assert.AreNotEqual(modelList.Count, 0);
            Assert.AreEqual(modelList[0].userName,"那英");

        }
    }
}
