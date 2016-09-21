using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;

namespace MB.BLL.Test
{
    [TestClass]
    public class IndexManagementBLLTest
    {
        MB.BLL.IndexManagementBLL indexBll = new MB.BLL.IndexManagementBLL();

        #region 获取首页模块列表（GetModuleList）
        /// <summary>
        /// DB中没有模块信息
        /// </summary>
        [TestMethod]
        public void GetModuleList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleList_Test001");

            var list = indexBll.GetModuleList();

            Assert.AreEqual(list, null);

        }

        /// <summary>
        /// DB中没有模块信息
        /// </summary>
        [TestMethod]
        public void GetModuleList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var list = indexBll.GetModuleList();

            Assert.AreEqual(list.Count, 5);

            Assert.AreEqual(list[0].module.moduleId, 1);
            Assert.AreEqual(list[0].module.title, "新闻");
            Assert.AreEqual(list[0].module.displayTitle, true);
            Assert.AreEqual(list[0].module.linkTarget, false);
            Assert.AreEqual(list[0].module.maxRow, 10);
            Assert.AreEqual(list[0].module.width, 50);
            Assert.AreEqual(list[0].module.height, 20);
            Assert.AreEqual(list[0].module.type, 1);
            Assert.AreEqual(list[0].module.position, null);
            Assert.AreEqual(list[0].module.defaultEfficiency, null);
            Assert.AreEqual(list[0].module.topDisplay, null);
            Assert.AreEqual(list[0].module.topDisplayLine, null);
            Assert.AreEqual(list[0].module.defaultLine, null);

            Assert.AreEqual(list[0].target.Count, 3);
            Assert.AreEqual(list[0].target[0].moduleId, 1);
            Assert.AreEqual(list[0].target[0].targetId, 1);
            Assert.AreEqual(list[0].target[0].withSub, true);

            Assert.AreEqual(list[1].module.type, 2);
            Assert.AreEqual(list[1].target.Count, 2);

            Assert.AreEqual(list[2].module.type, 3);
            Assert.AreEqual(list[2].target.Count, 1);

            Assert.AreEqual(list[3].module.type, 4);
            Assert.AreEqual(list[3].target, null);
            Assert.AreEqual(list[3].image.Count, 5);
            Assert.AreEqual(list[3].image[0].imageId, 1);
            Assert.AreEqual(list[3].image[0].imageUrl, "../HeadImage/201506250712161438.jpg");
            Assert.AreEqual(list[3].image[0].width, 80);
            Assert.AreEqual(list[3].image[0].height, 60);

            Assert.AreEqual(list[4].module.type, 5);
            Assert.AreEqual(list[4].target.Count, 3);

        }
        #endregion

        #region 模块详情取得（GetModuleInfo）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetModuleInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo_Test001");

            var module = indexBll.GetModuleInfo(1);

            Assert.AreEqual(module.module, null);
            Assert.AreEqual(module.image, null);
        }

        /// <summary>
        /// 取得图片轮播模块信息
        /// </summary>
        [TestMethod]
        public void GetModuleInfo_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = indexBll.GetModuleInfo(4);

            Assert.AreEqual(moduleInfo.module.moduleId, 4);
            Assert.AreEqual(moduleInfo.module.title, "图片");
            Assert.AreEqual(moduleInfo.module.displayTitle, false);
            Assert.AreEqual(moduleInfo.module.linkTarget, null);
            Assert.AreEqual(moduleInfo.module.maxRow, null);
            Assert.AreEqual(moduleInfo.module.width, 70);
            Assert.AreEqual(moduleInfo.module.height, 30);
            Assert.AreEqual(moduleInfo.module.type, 4);
            Assert.AreEqual(moduleInfo.module.position, null);
            Assert.AreEqual(moduleInfo.module.defaultEfficiency, null);
            Assert.AreEqual(moduleInfo.module.topDisplay, null);
            Assert.AreEqual(moduleInfo.module.topDisplayLine, null);
            Assert.AreEqual(moduleInfo.module.defaultLine, null);

            Assert.AreEqual(moduleInfo.image.Count, 5);
            Assert.AreEqual(moduleInfo.image[1].imageId, 2);
            Assert.AreEqual(moduleInfo.image[1].imageUrl, "../HeadImage/201506250712161439.jpg");
            Assert.AreEqual(moduleInfo.image[1].width, 100);
            Assert.AreEqual(moduleInfo.image[1].height, 50);
        }

        /// <summary>
        /// 取得图片轮播模块以外的模块信息
        /// </summary>
        [TestMethod]
        public void GetModuleInfo_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = indexBll.GetModuleInfo(5);

            Assert.AreEqual(moduleInfo.module.moduleId, 5);
            Assert.AreEqual(moduleInfo.module.title, "统计");
            Assert.AreEqual(moduleInfo.module.displayTitle, false);
            Assert.AreEqual(moduleInfo.module.linkTarget, null);
            Assert.AreEqual(moduleInfo.module.maxRow, null);
            Assert.AreEqual(moduleInfo.module.width, 100);
            Assert.AreEqual(moduleInfo.module.height, 30);
            Assert.AreEqual(moduleInfo.module.type, 5);
            Assert.AreEqual(moduleInfo.module.position, null);
            Assert.AreEqual(moduleInfo.module.defaultEfficiency, 1);
            Assert.AreEqual(moduleInfo.module.topDisplay, 10);
            Assert.AreEqual(moduleInfo.module.topDisplayLine, 5);
            Assert.AreEqual(moduleInfo.module.defaultLine, 1);

            Assert.AreEqual(moduleInfo.image, null);
        }
        #endregion

        #region 新闻来源取得（GetIndexNews）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexNews_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetIndexNews_Test001");

            var list = indexBll.GetIndexNews(1);

            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexNews_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var list = indexBll.GetIndexNews(1);

            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].moduleId, 1);
            Assert.AreEqual(list[0].targetId, 1);
            Assert.AreEqual(list[0].withSub, true);
        }
        #endregion

        #region 通知来源取得（GetIndexNotice）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexNotice_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetIndexNotice_Test001");

            var list = indexBll.GetIndexNotice(2);

            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexNotice_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var list = indexBll.GetIndexNotice(2);

            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].moduleId, 2);
            Assert.AreEqual(list[0].targetId, 1);
            Assert.AreEqual(list[0].withSub, false);
        }
        #endregion

        #region 文档来源取得（GetIndexDocument）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexDocument_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetIndexDocument_Test001");

            var list = indexBll.GetIndexDocument(3);

            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexDocument_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var list = indexBll.GetIndexDocument(3);

            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].moduleId, 3);
            Assert.AreEqual(list[0].targetId, 2);
            Assert.AreEqual(list[0].withSub, false);
        }
        #endregion

        #region 首页图像取得（GetIndexImage）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexImage_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetIndexImage_Test001");

            var list = indexBll.GetIndexImage(4);

            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexImage_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var list = indexBll.GetIndexImage(4);

            Assert.AreEqual(list.Count, 5);
            Assert.AreEqual(list[2].imageId, 3);
            Assert.AreEqual(list[2].imageUrl, "../HeadImage/201506250712161440.jpg");
            Assert.AreEqual(list[2].width, 100);
            Assert.AreEqual(list[2].height, 50);
    
        }
        #endregion

        #region 统计对象取得（GetIndexStatistics）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexStatistics_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetIndexStatistics_Test001");

            var list = indexBll.GetIndexStatistics(5);

            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetIndexStatistics_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var list = indexBll.GetIndexStatistics(5);

            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].moduleId, 5);
            Assert.AreEqual(list[0].targetId, 1);
            Assert.AreEqual(list[0].withSub, false);
        }
        #endregion

        #region 添加模块（AddIndexModuleInfo）
        /// <summary>
        /// 添加新闻类模块
        /// </summary>
        [TestMethod]
        public void AddIndexModuleInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "AddIndexModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.title = "公司新闻";
            module.displayTitle = false;
            module.linkTarget = true;
            module.maxRow = 10;
            module.width = 50;
            module.height = 30;
            module.type = 1;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var news = new IndexTargetModel();
            news.targetId = 1;
            news.withSub = true;
            list.Add(news);

            news = new IndexTargetModel();
            news.targetId = 2;
            news.withSub = true;
            list.Add(news);

            news = new IndexTargetModel();
            news.targetId = 3;
            news.withSub = false;
            list.Add(news);
            moduleInfo.target = list;

            indexBll.AddIndexModuleInfo(moduleInfo, 1);

            var moduleList = indexBll.GetModuleList();

            Assert.AreEqual(moduleList.Count, 1);

            Assert.AreEqual(moduleList[0].module.moduleId, 1);
            Assert.AreEqual(moduleList[0].module.title, module.title);
            Assert.AreEqual(moduleList[0].module.displayTitle, module.displayTitle);
            Assert.AreEqual(moduleList[0].module.linkTarget, module.linkTarget);
            Assert.AreEqual(moduleList[0].module.maxRow, module.maxRow);
            Assert.AreEqual(moduleList[0].module.width, module.width);
            Assert.AreEqual(moduleList[0].module.height, module.height);
            Assert.AreEqual(moduleList[0].module.type, module.type);
            Assert.AreEqual(moduleList[0].module.position, module.position);
            Assert.AreEqual(moduleList[0].module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(moduleList[0].module.topDisplay, module.topDisplay);
            Assert.AreEqual(moduleList[0].module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(moduleList[0].module.defaultLine, module.defaultLine);

            Assert.AreEqual(moduleList[0].target.Count, 3);
            Assert.AreEqual(moduleList[0].target[0].moduleId, 1);
            Assert.AreEqual(moduleList[0].target[0].targetId, 1);
            Assert.AreEqual(moduleList[0].target[0].withSub, true);
        }

        /// <summary>
        /// 添加通知类模块
        /// </summary>
        [TestMethod]
        public void AddIndexModuleInfo_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "AddIndexModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.title = "最新通知";
            module.displayTitle = true;
            module.linkTarget = false;
            module.maxRow = 10;
            module.width = 50;
            module.height = 30;
            module.type = 2;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var notice = new IndexTargetModel();
            notice.targetId = 1;
            notice.withSub = false;
            list.Add(notice);

            notice = new IndexTargetModel();
            notice.targetId = 2;
            notice.withSub = true;
            list.Add(notice);
            moduleInfo.target = list;

            indexBll.AddIndexModuleInfo(moduleInfo, 1);

            var moduleList = indexBll.GetModuleList();

            Assert.AreEqual(moduleList.Count, 1);

            Assert.AreEqual(moduleList[0].module.moduleId, 1);
            Assert.AreEqual(moduleList[0].module.title, module.title);
            Assert.AreEqual(moduleList[0].module.displayTitle, module.displayTitle);
            Assert.AreEqual(moduleList[0].module.linkTarget, module.linkTarget);
            Assert.AreEqual(moduleList[0].module.maxRow, module.maxRow);
            Assert.AreEqual(moduleList[0].module.width, module.width);
            Assert.AreEqual(moduleList[0].module.height, module.height);
            Assert.AreEqual(moduleList[0].module.type, module.type);
            Assert.AreEqual(moduleList[0].module.position, module.position);
            Assert.AreEqual(moduleList[0].module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(moduleList[0].module.topDisplay, module.topDisplay);
            Assert.AreEqual(moduleList[0].module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(moduleList[0].module.defaultLine, module.defaultLine);

            Assert.AreEqual(moduleList[0].target.Count, 2);
            Assert.AreEqual(moduleList[0].target[0].moduleId, 1);
            Assert.AreEqual(moduleList[0].target[0].targetId, 1);
            Assert.AreEqual(moduleList[0].target[0].withSub, false);
        }

        /// <summary>
        /// 添加文档类模块
        /// </summary>
        [TestMethod]
        public void AddIndexModuleInfo_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "AddIndexModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.title = "相关文档";
            module.displayTitle = true;
            module.linkTarget = null;
            module.maxRow = 10;
            module.width = 50;
            module.height = 30;
            module.type = 3;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var doc = new IndexTargetModel();
            doc.targetId = 2;
            doc.withSub = false;
            list.Add(doc);
            moduleInfo.target = list;

            indexBll.AddIndexModuleInfo(moduleInfo, 1);

            var moduleList = indexBll.GetModuleList();

            Assert.AreEqual(moduleList.Count, 1);

            Assert.AreEqual(moduleList[0].module.moduleId, 1);
            Assert.AreEqual(moduleList[0].module.title, module.title);
            Assert.AreEqual(moduleList[0].module.displayTitle, module.displayTitle);
            Assert.AreEqual(moduleList[0].module.linkTarget, module.linkTarget);
            Assert.AreEqual(moduleList[0].module.maxRow, module.maxRow);
            Assert.AreEqual(moduleList[0].module.width, module.width);
            Assert.AreEqual(moduleList[0].module.height, module.height);
            Assert.AreEqual(moduleList[0].module.type, module.type);
            Assert.AreEqual(moduleList[0].module.position, module.position);
            Assert.AreEqual(moduleList[0].module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(moduleList[0].module.topDisplay, module.topDisplay);
            Assert.AreEqual(moduleList[0].module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(moduleList[0].module.defaultLine, module.defaultLine);

            Assert.AreEqual(moduleList[0].target.Count, 1);
            Assert.AreEqual(moduleList[0].target[0].moduleId, 1);
            Assert.AreEqual(moduleList[0].target[0].targetId, 2);
            Assert.AreEqual(moduleList[0].target[0].withSub, false);
        }

        /// <summary>
        /// 添加图片轮播类模块
        /// </summary>
        [TestMethod]
        public void AddIndexModuleInfo_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "AddIndexModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.title = "";
            module.displayTitle = false;
            module.linkTarget = null;
            module.maxRow = null;
            module.width = 70;
            module.height = 30;
            module.type = 4;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var image = new IndexTargetModel();
            image.targetId = 1;
            list.Add(image);

            image = new IndexTargetModel();
            image.targetId = 2;
            list.Add(image);

            image = new IndexTargetModel();
            image.targetId = 3;
            list.Add(image);

            image = new IndexTargetModel();
            image.targetId = 4;
            list.Add(image);
            moduleInfo.target = list;

            indexBll.AddIndexModuleInfo(moduleInfo, 1);

            var moduleList = indexBll.GetModuleList();

            Assert.AreEqual(moduleList.Count, 1);

            Assert.AreEqual(moduleList[0].module.moduleId, 1);
            Assert.AreEqual(moduleList[0].module.title, module.title);
            Assert.AreEqual(moduleList[0].module.displayTitle, module.displayTitle);
            Assert.AreEqual(moduleList[0].module.linkTarget, module.linkTarget);
            Assert.AreEqual(moduleList[0].module.maxRow, module.maxRow);
            Assert.AreEqual(moduleList[0].module.width, module.width);
            Assert.AreEqual(moduleList[0].module.height, module.height);
            Assert.AreEqual(moduleList[0].module.type, module.type);
            Assert.AreEqual(moduleList[0].module.position, module.position);
            Assert.AreEqual(moduleList[0].module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(moduleList[0].module.topDisplay, module.topDisplay);
            Assert.AreEqual(moduleList[0].module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(moduleList[0].module.defaultLine, module.defaultLine);

            Assert.AreEqual(moduleList[0].image.Count, 4);
            Assert.AreEqual(moduleList[0].image[0].moduleId, 1);
            Assert.AreEqual(moduleList[0].image[0].imageId, 1);
            Assert.AreEqual(moduleList[0].image[0].imageUrl, "../HeadImage/201506250712161438.jpg");
        }

        /// <summary>
        /// 添加价值激励类模块
        /// </summary>
        [TestMethod]
        public void AddIndexModuleInfo_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "AddIndexModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.title = "";
            module.displayTitle = null;
            module.linkTarget = null;
            module.maxRow = null;
            module.width = 100;
            module.height = 30;
            module.type = 5;
            module.defaultEfficiency = 2;
            module.topDisplay = 10;
            module.topDisplayLine = 3;
            module.defaultLine = 1;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var org = new IndexTargetModel();
            org.targetId = 1;
            org.withSub = false;
            list.Add(org);

            org = new IndexTargetModel();
            org.targetId = 2;
            org.withSub = true;
            list.Add(org);
            moduleInfo.target = list;

            indexBll.AddIndexModuleInfo(moduleInfo, 1);

            var moduleList = indexBll.GetModuleList();

            Assert.AreEqual(moduleList.Count, 1);

            Assert.AreEqual(moduleList[0].module.moduleId, 1);
            Assert.AreEqual(moduleList[0].module.title, module.title);
            Assert.AreEqual(moduleList[0].module.displayTitle, module.displayTitle);
            Assert.AreEqual(moduleList[0].module.linkTarget, module.linkTarget);
            Assert.AreEqual(moduleList[0].module.maxRow, module.maxRow);
            Assert.AreEqual(moduleList[0].module.width, module.width);
            Assert.AreEqual(moduleList[0].module.height, module.height);
            Assert.AreEqual(moduleList[0].module.type, module.type);
            Assert.AreEqual(moduleList[0].module.position, module.position);
            Assert.AreEqual(moduleList[0].module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(moduleList[0].module.topDisplay, module.topDisplay);
            Assert.AreEqual(moduleList[0].module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(moduleList[0].module.defaultLine, module.defaultLine);

            Assert.AreEqual(moduleList[0].target.Count, 2);
            Assert.AreEqual(moduleList[0].target[0].moduleId, 1);
            Assert.AreEqual(moduleList[0].target[0].targetId, 1);
            Assert.AreEqual(moduleList[0].target[0].withSub, false);
        }
        #endregion

        #region 更新模块（UpdateIndexModuleInfo）
        /// <summary>
        /// 更新新闻类模块
        /// </summary>
        [TestMethod]
        public void UpdateIndexModuleInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.moduleId = 1;
            module.title = "公司新闻";
            module.displayTitle = true;
            module.linkTarget = false;
            module.maxRow = 8;
            module.width = 60;
            module.height = 30;
            module.type = 1;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var news = new IndexTargetModel();
            news.moduleId = 1;
            news.targetId = 1;
            news.withSub = false;
            list.Add(news);

            news = new IndexTargetModel();
            news.moduleId = 1;
            news.targetId = 3;
            news.withSub = true;
            list.Add(news);
            moduleInfo.target = list;

            indexBll.UpdateIndexModuleInfo(moduleInfo, 1);

            var result = indexBll.GetModuleInfo(1);
            Assert.AreEqual(result.module.moduleId, 1);
            Assert.AreEqual(result.module.title, module.title);
            Assert.AreEqual(result.module.displayTitle, module.displayTitle);
            Assert.AreEqual(result.module.linkTarget, module.linkTarget);
            Assert.AreEqual(result.module.maxRow, module.maxRow);
            Assert.AreEqual(result.module.width, module.width);
            Assert.AreEqual(result.module.height, module.height);
            Assert.AreEqual(result.module.type, module.type);
            Assert.AreEqual(result.module.position, module.position);
            Assert.AreEqual(result.module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(result.module.topDisplay, module.topDisplay);
            Assert.AreEqual(result.module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(result.module.defaultLine, module.defaultLine);

            var newsList = indexBll.GetIndexNews(1);
            Assert.AreEqual(newsList.Count, 2);
        }

        /// <summary>
        /// 更新通知类模块
        /// </summary>
        [TestMethod]
        public void UpdateIndexModuleInfo_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.moduleId = 2;
            module.title = "最新通知";
            module.displayTitle = true;
            module.linkTarget = false;
            module.maxRow = 8;
            module.width = 60;
            module.height = 30;
            module.type = 2;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var notice = new IndexTargetModel();
            notice.moduleId = 2;
            notice.targetId = 1;
            notice.withSub = false;
            list.Add(notice);

            moduleInfo.target = list;

            indexBll.UpdateIndexModuleInfo(moduleInfo, 1);

            var result = indexBll.GetModuleInfo(2);
            Assert.AreEqual(result.module.moduleId, 2);
            Assert.AreEqual(result.module.title, module.title);
            Assert.AreEqual(result.module.displayTitle, module.displayTitle);
            Assert.AreEqual(result.module.linkTarget, module.linkTarget);
            Assert.AreEqual(result.module.maxRow, module.maxRow);
            Assert.AreEqual(result.module.width, module.width);
            Assert.AreEqual(result.module.height, module.height);
            Assert.AreEqual(result.module.type, module.type);
            Assert.AreEqual(result.module.position, module.position);
            Assert.AreEqual(result.module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(result.module.topDisplay, module.topDisplay);
            Assert.AreEqual(result.module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(result.module.defaultLine, module.defaultLine);

            var newsList = indexBll.GetIndexNotice(2);
            Assert.AreEqual(newsList.Count, 1);
        }

        /// <summary>
        /// 更新通知类模块
        /// </summary>
        [TestMethod]
        public void UpdateIndexModuleInfo_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.moduleId = 3;
            module.title = "公司文档";
            module.displayTitle = true;
            module.linkTarget = null;
            module.maxRow = 10;
            module.width = 60;
            module.height = 30;
            module.type = 3;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var doc = new IndexTargetModel();
            doc.moduleId = 3;
            doc.targetId = 1;
            doc.withSub = true;
            list.Add(doc);

            doc = new IndexTargetModel();
            doc.moduleId = 3;
            doc.targetId = 2;
            doc.withSub = false;
            list.Add(doc);

            moduleInfo.target = list;

            indexBll.UpdateIndexModuleInfo(moduleInfo, 1);

            var result = indexBll.GetModuleInfo(3);
            Assert.AreEqual(result.module.moduleId, 3);
            Assert.AreEqual(result.module.title, module.title);
            Assert.AreEqual(result.module.displayTitle, module.displayTitle);
            Assert.AreEqual(result.module.linkTarget, module.linkTarget);
            Assert.AreEqual(result.module.maxRow, module.maxRow);
            Assert.AreEqual(result.module.width, module.width);
            Assert.AreEqual(result.module.height, module.height);
            Assert.AreEqual(result.module.type, module.type);
            Assert.AreEqual(result.module.position, module.position);
            Assert.AreEqual(result.module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(result.module.topDisplay, module.topDisplay);
            Assert.AreEqual(result.module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(result.module.defaultLine, module.defaultLine);

            var newsList = indexBll.GetIndexDocument(3);
            Assert.AreEqual(newsList.Count, 2);
        }

        /// <summary>
        /// 更新图片轮播类模块
        /// </summary>
        [TestMethod]
        public void UpdateIndexModuleInfo_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.moduleId = 4;
            module.title = "";
            module.displayTitle = false;
            module.linkTarget = null;
            module.maxRow = null;
            module.width = 70;
            module.height = 30;
            module.type = 4;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var image = new IndexTargetModel();
            image.moduleId = 4;
            image.targetId = 1;
            list.Add(image);

            image = new IndexTargetModel();
            image.moduleId = 4;
            image.targetId = 2;
            list.Add(image);

            image = new IndexTargetModel();
            image.moduleId = 4;
            image.targetId = 3;
            list.Add(image);

            image = new IndexTargetModel();
            image.moduleId = 4;
            image.targetId = 4;
            list.Add(image);
            moduleInfo.target = list;

            indexBll.UpdateIndexModuleInfo(moduleInfo, 1);

            var result = indexBll.GetModuleInfo(4);

            Assert.AreEqual(result.module.moduleId, 4);
            Assert.AreEqual(result.module.title, module.title);
            Assert.AreEqual(result.module.displayTitle, module.displayTitle);
            Assert.AreEqual(result.module.linkTarget, module.linkTarget);
            Assert.AreEqual(result.module.maxRow, module.maxRow);
            Assert.AreEqual(result.module.width, module.width);
            Assert.AreEqual(result.module.height, module.height);
            Assert.AreEqual(result.module.type, module.type);
            Assert.AreEqual(result.module.position, module.position);
            Assert.AreEqual(result.module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(result.module.topDisplay, module.topDisplay);
            Assert.AreEqual(result.module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(result.module.defaultLine, module.defaultLine);

            Assert.AreEqual(result.image.Count, 4);
            Assert.AreEqual(result.image[0].moduleId, 4);
            Assert.AreEqual(result.image[0].imageId, 1);
            Assert.AreEqual(result.image[0].imageUrl, "../HeadImage/201506250712161438.jpg");
        }

        /// <summary>
        /// 更新价值激励类模块
        /// </summary>
        [TestMethod]
        public void UpdateIndexModuleInfo_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.moduleId = 5;
            module.title = "工效价值";
            module.displayTitle = false;
            module.linkTarget = null;
            module.maxRow = 10;
            module.width = 90;
            module.height = 30;
            module.type = 5;
            moduleInfo.module = module;

            var list = new List<IndexTargetModel>();
            var org = new IndexTargetModel();
            org.moduleId = 5;
            org.targetId = 1;
            org.withSub = true;
            list.Add(org);

            org = new IndexTargetModel();
            org.moduleId = 5;
            org.targetId = 2;
            org.withSub = false;
            list.Add(org);

            moduleInfo.target = list;

            indexBll.UpdateIndexModuleInfo(moduleInfo, 1);

            var result = indexBll.GetModuleInfo(5);
            Assert.AreEqual(result.module.moduleId, 5);
            Assert.AreEqual(result.module.title, module.title);
            Assert.AreEqual(result.module.displayTitle, module.displayTitle);
            Assert.AreEqual(result.module.linkTarget, module.linkTarget);
            Assert.AreEqual(result.module.maxRow, module.maxRow);
            Assert.AreEqual(result.module.width, module.width);
            Assert.AreEqual(result.module.height, module.height);
            Assert.AreEqual(result.module.type, module.type);
            Assert.AreEqual(result.module.position, module.position);
            Assert.AreEqual(result.module.defaultEfficiency, module.defaultEfficiency);
            Assert.AreEqual(result.module.topDisplay, module.topDisplay);
            Assert.AreEqual(result.module.topDisplayLine, module.topDisplayLine);
            Assert.AreEqual(result.module.defaultLine, module.defaultLine);

            var newsList = indexBll.GetIndexStatistics(5);
            Assert.AreEqual(newsList.Count, 2);
        }

        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void UpdateIndexModuleInfo_Test006()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            var moduleInfo = new IndexModuleInfoModel();

            var module = new IndexModuleModel();
            module.moduleId = 6;
            moduleInfo.module = module;

            indexBll.UpdateIndexModuleInfo(moduleInfo, 1);
        }
        #endregion

        #region 删除模块（DeleteModule）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void DeleteModule_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "DeleteModule_Test001");

            indexBll.DeleteModule(1);
        }

        /// <summary>
        /// 删除新闻类模块
        /// </summary>
        [TestMethod]
        public void DeleteModule_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteModule(1);

            var result = indexBll.GetModuleInfo(1);

            Assert.AreEqual(result.module, null);

            var list = indexBll.GetIndexNews(1);
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// 删除通知类模块
        /// </summary>
        [TestMethod]
        public void DeleteModule_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteModule(2);

            var result = indexBll.GetModuleInfo(2);

            Assert.AreEqual(result.module, null);

            var list = indexBll.GetIndexNotice(2);
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// 删除文档类模块
        /// </summary>
        [TestMethod]
        public void DeleteModule_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteModule(3);

            var result = indexBll.GetModuleInfo(3);

            Assert.AreEqual(result.module, null);

            var list = indexBll.GetIndexDocument(3);
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// 删除图片轮播类模块
        /// </summary>
        [TestMethod]
        public void DeleteModule_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteModule(4);

            var result = indexBll.GetModuleInfo(4);

            Assert.AreEqual(result.module, null);
            Assert.AreEqual(result.image, null);
        }

        /// <summary>
        /// 删除价值激励类模块
        /// </summary>
        [TestMethod]
        public void DeleteModule_Test006()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteModule(5);

            var result = indexBll.GetModuleInfo(5);

            Assert.AreEqual(result.module, null);

            var list = indexBll.GetIndexStatistics(5);
            Assert.AreEqual(list.Count, 0);
        }
        #endregion

        #region 更新模块大小（UpdateModuleSize）
        /// <summary>
        /// DB中不存在相关数据
        /// </summary>
        [TestMethod]
        public void UpdateModuleSize_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "UpdateModuleSize_Test001");

            //indexBll.UpdateModuleSize(1, 80, 1);
        }

        /// <summary>
        /// DB中不存在相关数据
        /// </summary>
        [TestMethod]
        public void UpdateModuleSize_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            //indexBll.UpdateModuleSize(1, 80, 1);

            var result = indexBll.GetModuleInfo(1);
            Assert.AreEqual(result.module.width, 80);
        }
        
        #endregion

        #region 删除首页图片（DeleteIndexImage）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void DeleteIndexImage_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteIndexImage(4, 6);

            var list = indexBll.GetIndexImage(4);
            Assert.AreEqual(list.Count, 5);
        }

        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void DeleteIndexImage_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("IndexManagementBLLTestData.xlsx", "GetModuleInfo");

            indexBll.DeleteIndexImage(4, 2);

            var list = indexBll.GetIndexImage(4);
            Assert.AreEqual(list.Count, 4);
        }
        #endregion
    }
}
