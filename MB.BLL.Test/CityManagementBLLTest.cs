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
   public  class CityManagementBLLTest
    {
        CityManagementBLL cityManagementBll = new CityManagementBLL();

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

        //测试城市的添加
        [TestMethod]
        public void SaveCity_Test001()
        {
            //导入数据
            DataUtility.InsertDataBase("cityManagementBLLTestData.xlsx", "SaveCity_Test002");

            var model = new CityManagement
            {
                cityName = "南京",
                zipCode = "320000",
                provinceId = 32
            };
            cityManagementBll.SaveCity(model);

            var list = cityManagementBll.GetCityList(model.cityName);
            //验证
            Assert.AreNotEqual(list[0],null);
        }

        //测试修改城市
        [TestMethod]
        public void SaveCity_Test002()
        {
            //导入数据
            DataUtility.InsertDataBase("cityManagementBLLTestData.xlsx", "SaveCity_Test002");

            var model = new CityManagement
            {
                cityId=2,
                cityName = "无锡",
                zipCode = "320200",
                provinceId = 32
            };
            cityManagementBll.SaveCity(model);

            var list = cityManagementBll.GetCityList(model.cityName);
            //验证
            Assert.AreNotEqual(list[0], null);
            Assert.AreEqual(list[0].zipCode,"320200");
        }

        //删除
        [TestMethod]
        public void DeleteCity_Test001()
        {
            //导入数据
            DataUtility.InsertDataBase("cityManagementBLLTestData.xlsx", "SaveCity_Test002");
            cityManagementBll.DeleteCity(2);

            var list = cityManagementBll.GetCityList("无锡");

            //验证
            Assert.AreEqual(list.Count,0);
        }

        //测试查询
        [TestMethod]
        public void GetCityList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("cityManagementBLLTestData.xlsx", "SaveCity_Test002");
            var list = cityManagementBll.GetCityList("无锡");

            //验证
            Assert.AreEqual(list.Count,1);
        }
    }
}
