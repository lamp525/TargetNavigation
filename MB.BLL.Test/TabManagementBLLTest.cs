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
   public class TabManagementBLLTest
    {
 //       TabManagementBLL tabManagementBll = new TabManagementBLL();

 //       [TestInitialize()]
 //       public void MyTestInitialize()
 //       {
 //           // 导入共通数据
 //           //DataUtility.InsertDataBase("BLLTestData.xlsx");
 //       }

 //       [TestCleanup()]
 //       public void MyTestCleanup()
 //       {
 //       }

 //       //测试查询 无条件
 //       [TestMethod]
 //       public void GetTabList_Test001()
 //       {
 ////导入测试数据
 //           DataUtility.InsertDataBase("TabManagementBLLTestData.xlsx", "GetTabList_Test001");

 //           var list = tabManagementBll.GetSystemTabList();
 //           //验证
 //           Assert.AreNotEqual(list.Count,0);
 //           Assert.AreEqual(list[0].tabName,"标签1");
 //       }

 //       //测试查询 有条件
 //       [TestMethod]
 //       public void GetTabList_Test002()
 //       {
 //           //导入测试数据
 //           DataUtility.InsertDataBase("TabManagementBLLTestData.xlsx", "GetTabList_Test001");

 //           var list = tabManagementBll.GetSystemTabList("标签1");
 //           //验证
 //           Assert.AreNotEqual(list.Count, 0);
 //           Assert.AreEqual(list[0].tabName, "标签1");
 //           Assert.AreEqual(list[0].tabId, 1);
 //           Assert.AreEqual(list[0].num,1);
 //       }

 //       //测试标签 新建
 //       [TestMethod]
 //       public void SaveTab_Test001()
 //       {
 //           //导入测试数据
 //           DataUtility.InsertDataBase("TabManagementBLLTestData.xlsx", "GetTabList_Test001");

 //           var model = new TabModel
 //           {
 //               tabId=10,
 //               tabName = "标签4",
 //               num = 1,
 //               systemTab = true
 //           };

 //           tabManagementBll.SaveSystemTab(model);

 //           var list = tabManagementBll.GetSystemTabList("标签4");

 //           //验证
 //           Assert.AreEqual(list.Count, 1);
 //           Assert.AreEqual(list[0].tabName, "标签4");
 //       }

 //       //测试标签 更新
 //       [TestMethod]
 //       public void SaveTab_Test002()
 //       {
 //           //导入测试数据
 //           DataUtility.InsertDataBase("TabManagementBLLTestData.xlsx", "GetTabList_Test001");

 //           var model = new TabModel
 //           {
 //               tabId = 2,
 //               tabName = "标签02",
 //               num = 1,
 //               systemTab = true
 //           };

 //           tabManagementBll.SaveSystemTab(model);

 //           var list = tabManagementBll.GetSystemTabList("标签02");

 //           //验证
 //           Assert.AreEqual(list.Count, 1);
 //           Assert.AreEqual(list[0].tabName, "标签02");
 //       }

 //       //测试 删除
 //       [TestMethod]
 //       public void DeleteTab_Test()
 //       {
 //           //导入测试数据
 //           DataUtility.InsertDataBase("TabManagementBLLTestData.xlsx", "GetTabList_Test001");
 //           var id = new int[]{2,3};

 //           tabManagementBll.DeleteSystemTab(id);

 //           var list = tabManagementBll.GetSystemTabList();
 //           //验证
 //           Assert.AreNotEqual(list.Count,0);
 //           Assert.AreNotEqual(list[0].tabId,2);

 //       }

 //       [TestMethod]
 //       public void ImportPlanTestData()
 //       {
 //           var result = false;
 //           var loop = 107100;
 //           var tag = "计划,测试,plan,目标导航";
 //           for( int i = 107065; i<loop ; i ++)
 //           {
 //               this.tabManagementBll.SavePlanTag(i, tag, Common.ConstVar.TagSaveMode.TempAndCache);
                
 //           }
 //           result = true;

 //           Assert.IsTrue(result);
 //       }

 //       [TestMethod]
 //       public void ImportDocTestData()
 //       {
 //           var result = false;
 //           var loop = 100;
 //           var tag = "文档,测试,doc,document,目标导航";
 //           for (int i = 1; i < loop; i++)
 //           {
 //               this.tabManagementBll.SaveCompanyDocumentTag(i, tag, Common.ConstVar.TagSaveMode.CacheOnly);
 //               this.tabManagementBll.SaveUserDocumentTag(i, tag, Common.ConstVar.TagSaveMode.CacheOnly);

 //           }
 //           result = true;

 //           Assert.IsTrue(result);
 //       }

 //       [TestMethod]
 //       public void importObjectiveTestData()
 //       {
 //           var result = false;

 //           var loop = 200;
 //           var tag = "目标,测试,doc,document,目标导航";
 //           for (int i = 1; i < loop; i++)
 //           {
 //               this.tabManagementBll.SaveObjectiveTag(i, tag, Common.ConstVar.TagSaveMode.TempAndCache);
 //           }

 //           result = true;

 //           Assert.IsTrue(result);
 //       }

 //       [TestMethod]
 //       public void importNewsTestData()
 //       {
 //           var result = false;

 //           var loop = 200;
 //           var tag = "新闻,测试,news";
 //           for (int i = 1; i < loop; i++)
 //           {
 //               this.tabManagementBll.SaveNewsTag(i, tag, Common.ConstVar.TagSaveMode.TempAndCache);
 //           }

 //           result = true;

 //           Assert.IsTrue(result);
 //       }

    }
}
