using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;
using MB.BLL;


namespace MB.BLL.Test
{
    [TestClass]
    public class NewsManagementBLLTest
    {
        NewsManagementBLL newsManagementBll = new NewsManagementBLL();

        /// <summary>
        /// 新闻详情
        /// </summary>
        [TestMethod]
        public void GetNewsInfo_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsModel = newsManagementBll.GetNewsInfo(5);
            //验证
            Assert.AreEqual(newsModel.newId,5);
            Assert.AreEqual(newsModel.directoryId,5);
            Assert.AreEqual(newsModel.title, "美越以惊人速度接近 外媒:联手抗衡中国");
            Assert.AreEqual(newsModel.contents, "境外媒体称,在过去的几年里,越南和美国走到一起的步伐之快,就连两国和解的策划者也感到吃惊");
            Assert.AreEqual(newsModel.summary,null);
            Assert.AreEqual(newsModel.titleImage, "../../newsImages/416219056.jpg");
            Assert.AreEqual(newsModel.notice,false);
            Assert.AreEqual(newsModel.viewNum,1);
            Assert.AreEqual(newsModel.isTop,true);
            Assert.AreEqual(newsModel.keyword, null);
            Assert.AreEqual(newsModel.publish,false);
            Assert.AreEqual(newsModel.createUser,1);
            Assert.AreEqual(newsModel.updateUser,1);
            Assert.AreEqual(newsModel.UserName,"wqx");
        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNewsInfo_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsModel = newsManagementBll.GetNewsInfo(30);
            //验证
            Assert.AreEqual(newsModel,null);
        }

        /// <summary>
        /// 获取新闻列表
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNewsList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsInfo = newsManagementBll.GetNewsList(1, 8);

            //验证
            Assert.AreEqual(newsInfo.Count,0 );
          
        }
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        [TestMethod]
        public void GetNewsList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsInfo = newsManagementBll.GetNewsList(2, 0);

            //验证
            Assert.AreEqual(newsInfo.Count, 2);
            Assert.AreEqual(newsInfo[0].newId, 8);
            Assert.AreEqual(newsInfo[0].title, "日媒:中国新型反潜机将服役 装备独特武器");
            Assert.AreEqual(newsInfo[0].titleImage, "../../newsImages/416219056.jpg");
            Assert.AreEqual(newsInfo[0].notice, false);
            Assert.AreEqual(newsInfo[0].summary, null);
            Assert.AreEqual(newsInfo[0].viewNum, 1);
            Assert.AreEqual(newsInfo[0].createUser, 1);
            Assert.AreEqual(newsInfo[0].FCreatTime, null);
            Assert.AreEqual(newsInfo[0].directoryId, 5);
            Assert.AreEqual(newsInfo[0].directoryName, "军事");
            Assert.AreEqual(newsInfo[0].publish, false);
            Assert.AreEqual(newsInfo[0].isTop, true);
            Assert.AreEqual(newsInfo[0].keyword, null);
            Assert.AreEqual(newsInfo[0].pageCount, 6);
          
        }

        /// <summary>
        /// 获取新闻列表
        /// </summary>
        [TestMethod]
        public void GetNewsList_Test003()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsInfo = newsManagementBll.GetNewsList(1, 5);

            //验证
            Assert.AreEqual(newsInfo.Count, 2);
        }

        /// <summary>
        /// 获取通知列表
        /// </summary>
        [TestMethod]
        public void GetNoticeList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var noticeList = newsManagementBll.GetNoticeList(1, 0);

            //验证
            Assert.AreEqual(noticeList.Count, 2);
          
            Assert.AreEqual(noticeList[0].newId, 14);
            Assert.AreEqual(noticeList[0].title, "三星预计Q2运营利润61亿美元 或不及预期");
            Assert.AreEqual(noticeList[0].titleImage, "../../newsImages/416219056.jpg");
            Assert.AreEqual(noticeList[0].notice, true);
            Assert.AreEqual(noticeList[0].summary, null);
            Assert.AreEqual(noticeList[0].viewNum, 1);
            Assert.AreEqual(noticeList[0].createUser, 1);
            Assert.AreEqual(noticeList[0].FCreatTime, null);
            Assert.AreEqual(noticeList[0].directoryId, 5);
            Assert.AreEqual(noticeList[0].directoryName, "军事");
            Assert.AreEqual(noticeList[0].publish, false);
            Assert.AreEqual(noticeList[0].isTop, true);
            Assert.AreEqual(noticeList[0].keyword, null);
     
        }

        /// <summary>
        /// 获取通知列表
        /// </summary>
        [TestMethod]
        public void GetNoticeList_Test003()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var noticeList = newsManagementBll.GetNoticeList(1,5);

            //验证
            Assert.AreEqual(noticeList.Count, 2);
        }

        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNoticeList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var noticeList = newsManagementBll.GetNoticeList(1, 8);

            //验证
            Assert.AreEqual(noticeList.Count, 0);
        }

        /// <summary>
        /// 删除新闻列表
        /// </summary>
        [TestMethod]
        public void DeleteNews_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");

            newsManagementBll.DeleteNews(new int[] { 4,15 });
            var newsModel = newsManagementBll.GetNewsInfo(15);
            //验证
            Assert.AreEqual(newsModel, null);
        }
     
        /// <summary>
        /// 发布新闻/通知
        /// </summary>
        [TestMethod]
        public void PublishNews_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.PublishNews(new int[] { 5 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(5);
            //验证
            Assert.AreEqual(newsModel.publish, true);
            Assert.AreEqual(newsModel.updateUser, 1);
        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void PublishNews_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.PublishNews(new int[] { 1,30 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(30);
            //验证
            Assert.AreEqual(newsModel,null);
        }

        /// <summary>
        /// 取消发布新闻/通知
        /// </summary>
        [TestMethod]
        public void UnPublishNews_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.UnPublishNews(new int[] { 5 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(5);
            //验证
            Assert.AreEqual(newsModel.publish, false);
            Assert.AreEqual(newsModel.updateUser, 1);
        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void UnPublishNews_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.UnPublishNews(new int[] { 1,30 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(30);
            //验证
            Assert.AreEqual(newsModel,null);
        }

        /// <summary>
        /// 新闻/通知置顶
        /// </summary>
        [TestMethod]
        public void SetTopNews_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.SetTopNews(new int[] { 3,10}, 1);
            var newsModel = newsManagementBll.GetNewsInfo(3);
            //验证
            Assert.AreEqual(newsModel.isTop, true);
            Assert.AreEqual(newsModel.updateUser, 1);
        }
        /// <summary>
        /// 没有数据
        /// </summary>
        [TestMethod]
        public void SetTopNews_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.SetTopNews(new int[] { 1, 30 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(30);
            //验证
            Assert.AreEqual(newsModel,null);
        }

        /// <summary>
        /// 取消新闻/通知置顶
        /// </summary>
        [TestMethod]
        public void SetUnTopNews_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.SetUnTopNews(new int[] { 3,10 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(3);
            //验证
     
            Assert.AreEqual(newsModel.isTop, false);
            Assert.AreEqual(newsModel.updateUser, 1);

        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void SetUnTopNews_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            newsManagementBll.SetUnTopNews(new int[] { 1, 30 }, 1);
            var newsModel = newsManagementBll.GetNewsInfo(30);
            //验证
            Assert.AreEqual(newsModel,null);
        }
       
        /// <summary>
        /// 获取新闻分类列表
        /// </summary>
        [TestMethod]
        public void GetNewsTypeList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsTypeList_Test008");
            var newsDir = newsManagementBll.GetNewsTypeList(5);
            //验证
            Assert.AreEqual(newsDir.Count,1);
            Assert.AreEqual(newsDir[0].parentDirectory, 5);
            Assert.AreEqual(newsDir[0].directoryId, 6);
            Assert.AreEqual(newsDir[0].directoryName, "网络");

         
        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNewsTypeList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsTypeList_Test008");
            var newsDir = newsManagementBll.GetNewsTypeList(1);
            //验证
            Assert.AreEqual(newsDir.Count,0);
        }

        /// <summary>
        /// 获取通知分类列表
        /// </summary>
        [TestMethod]
        public void GetNoticeTypeList_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNoticeTypeList_Test009");
            var noticeDir = newsManagementBll.GetNoticeTypeList(2);
            //验证
            Assert.AreEqual(noticeDir.Count, 1);
            Assert.AreEqual(noticeDir[0].parentDirectory,2);
            Assert.AreEqual(noticeDir[0].directoryId, 3);
            Assert.AreEqual(noticeDir[0].directoryName, "娱乐");

          
        }
        /// <summary>
        /// DB没有数据
        /// </summary>
        [TestMethod]
        public void GetNoticeTypeList_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNoticeTypeList_Test009");
            var noticeDir = newsManagementBll.GetNoticeTypeList(1);
            //验证
            Assert.AreEqual(noticeDir.Count, 0);
        }

        /// <summary>
        /// 更新新闻排序
        /// </summary>
        [TestMethod]
        public void OrderNewsType_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsTypeList_Test008");
            var newsDirList = new List<NewsDirModel>();
            var news = new NewsDirModel();
            news.parentDirectory = null;
            news.directoryId = 1;
            news.orderNum = 2;
            news.updateTime = DateTime.Now;
            news.updateUser = 1;
            newsDirList.Add(news);

            news = new NewsDirModel();
            news.parentDirectory = null;
            news.directoryId = 2;
            news.orderNum =1;
            news.updateTime = DateTime.Now;
            news.updateUser = 1;
            newsDirList.Add(news);

           

            newsManagementBll.OrderNewsType(newsDirList, 1);
            var newsDir = newsManagementBll.GetNewsTypeList(null);
            //验证
            Assert.AreEqual(newsDir.Count, 3);
            Assert.AreEqual(newsDir[0].parentDirectory, newsDirList[1].parentDirectory);
            Assert.AreEqual(newsDir[0].directoryId, newsDirList[1].directoryId);
            Assert.AreEqual(newsDir[0].orderNum, newsDirList[1].orderNum);
            Assert.AreEqual(newsDir[0].updateUser, newsDirList[1].updateUser);

            Assert.AreEqual(newsDir[1].parentDirectory, newsDirList[0].parentDirectory);
            Assert.AreEqual(newsDir[1].directoryId, newsDirList[0].directoryId);
            Assert.AreEqual(newsDir[1].orderNum, newsDirList[0].orderNum);
            Assert.AreEqual(newsDir[1].updateUser, newsDirList[0].updateUser);
       
        }
      

        /// <summary>
        /// 更新通知排序
        /// </summary>
        [TestMethod]
        public void OrderNoticeType_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNoticeTypeList_Test009");
            var noticeDirList = new List<NewsDirModel>();
            var notice = new NewsDirModel();
            notice.parentDirectory = null;
            notice.directoryId = 1;
            notice.orderNum = 2;
            notice.updateTime = DateTime.Now;
            notice.updateUser = 1;
            noticeDirList.Add(notice);

            notice = new NewsDirModel();
            notice.parentDirectory = null;
            notice.directoryId = 2;
            notice.orderNum = 1;
            notice.updateTime = DateTime.Now;
            notice.updateUser = 1;
            noticeDirList.Add(notice);

            newsManagementBll.OrderNoticeType(noticeDirList, 1);
            var noticeDir = newsManagementBll.GetNoticeTypeList(null);
            //验证
            Assert.AreEqual(noticeDir.Count,2);
            Assert.AreEqual(noticeDir[0].parentDirectory, noticeDirList[1].parentDirectory);
            Assert.AreEqual(noticeDir[0].directoryId, noticeDirList[1].directoryId);
            Assert.AreEqual(noticeDir[0].orderNum, noticeDirList[1].orderNum);
            Assert.AreEqual(noticeDir[0].updateUser, noticeDirList[1].updateUser);

            Assert.AreEqual(noticeDir[1].parentDirectory, noticeDirList[0].parentDirectory);
            Assert.AreEqual(noticeDir[1].directoryId, noticeDirList[0].directoryId);
            Assert.AreEqual(noticeDir[1].orderNum, noticeDirList[0].orderNum);
            Assert.AreEqual(noticeDir[1].updateUser, noticeDirList[0].updateUser);
        }

        /// <summary>
        /// 删除新闻分类
        /// </summary>
        [TestMethod]
        public void DeleteNewsType_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsTypeList_Test008");
            newsManagementBll.DeleteNewsType(new int[] { 1,2 });
            var newsMode = newsManagementBll.GetNewsModel(1);
            //验证
            Assert.AreEqual(newsMode,null);
        }

        /// <summary>
        /// 删除通知分类
        /// </summary>
        [TestMethod]
        public void DeleteNoticeType_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNoticeTypeList_Test009");
            newsManagementBll.DeleteNoticeType(new int[] {1,2});
            var noticeModel = newsManagementBll.GetNoticeModel(1);
            //验证
            Assert.AreEqual(noticeModel,null);
        }

        /// <summary>
        /// 更新新闻分类
        /// </summary>
        [TestMethod]
        public void SaveNewsType_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsDir = new NewsDirModel();
            newsDir.directoryId = 5;
            newsDir.directoryName = "金融业";
            newsDir.parentDirectory = 1;
            newsDir.orderNum = 5;
            newsDir.createTime = DateTime.Now;
            newsDir.createUser = 1;
            newsDir.updateTime = DateTime.Now;
            newsDir.updateUser = 1;

            newsManagementBll.SaveNewsType(newsDir, 1);
          
        }
        /// <summary>
        /// 新建
        /// </summary>
        [TestMethod]
        public void SaveNewsType_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var newsDir = new NewsDirModel();
            newsDir.directoryId = 6;
            newsDir.directoryName = "金融业";
            newsDir.parentDirectory = 1;
            newsDir.orderNum = 5;
            newsDir.createTime = DateTime.Now;
            newsDir.createUser = 1;
            newsDir.updateTime = DateTime.Now;
            newsDir.updateUser = 1;

            newsManagementBll.SaveNewsType(newsDir, 1);

        }
      
        /// <summary>
        ///更新通知分类 
        /// </summary>
        [TestMethod]
        public void SaveNoticeType_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var noticeDir = new NewsDirModel();
            noticeDir.directoryId = 5;
            noticeDir.directoryName = "互联网";
            noticeDir.parentDirectory = 1;
            noticeDir.orderNum = 5;
            noticeDir.createTime = DateTime.Now;
            noticeDir.createUser = 1;
            noticeDir.updateTime = DateTime.Now;
            noticeDir.updateUser = 1;

            newsManagementBll.SaveNoticeType(noticeDir,1);
         

        }
        /// <summary>
        /// 新建
        /// </summary>
        [TestMethod]
        public void SaveNoticeType_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
            var noticeDir = new NewsDirModel();
            noticeDir.directoryId = 6;
            noticeDir.directoryName = "互联网";
            noticeDir.parentDirectory = 1;
            noticeDir.orderNum = 5;
            noticeDir.createTime = DateTime.Now;
            noticeDir.createUser = 1;
            noticeDir.updateTime = DateTime.Now;
            noticeDir.updateUser = 1;

            newsManagementBll.SaveNoticeType(noticeDir, 1);


        }

        /// <summary>
        /// 更新新闻
        /// </summary>
        [TestMethod]
        public void SaveNews_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo_Test001");

            var news = new NewsinfoModel();
            news.newId = 1;
            news.directoryId = 1;
            news.title = "解读地球计划首次发布";
            news.contents = "在日前举行的聚合数据 思创中国2015中科曙光技术创新大会上,解读地球计划首次发布。";
            news.summary = null;
            news.titleImage = "../../newsImages/416219056.jpg";
            news.notice = false;
            news.viewNum = 1;
            news.isTop = false;
            news.keyword = null;
            news.publish = false;
            news.createUser = 1;
            news.createTime = DateTime.Now;
            news.updateTime = DateTime.Now;
            news.updateUser = 1;

            newsManagementBll.SaveNews(news,1);
           
        }
        /// <summary>
        /// 新建
        /// </summary>
        [TestMethod]
        public void SaveNews_Test002()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo_Test001");

            var news = new NewsinfoModel();
            news.newId = 5;
            news.directoryId = 1;
            news.title = "解读地球计划首次发布";
            news.contents = "在日前举行的聚合数据 思创中国2015中科曙光技术创新大会上,解读地球计划首次发布。";
            news.summary = null;
            news.titleImage = "../../newsImages/416219056.jpg";
            news.notice = false;
            news.viewNum = 1;
            news.isTop = false;
            news.keyword = null;
            news.publish = false;
            news.createUser = 1;
            news.createTime = DateTime.Now;
            news.updateTime = DateTime.Now;
            news.updateUser = 1;

            newsManagementBll.SaveNews(news, 1);

        }

        /// <summary>
        /// 获取图片列表
        /// </summary>
        [TestMethod]
        public void GetImageList_Test001()
        {
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetImage_Test001");
            var imgList = newsManagementBll.GetImageList();
            //验证
            Assert.AreNotEqual(imgList.Count, 0);

        }

        /// <summary>
        /// 添加新闻图片
        /// </summary>
        [TestMethod]
        public void AddNewsImage_Test001()
        {
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetImage_Test001");
            var model = new ImageInfoModel
            {
                saveName = "201506250712161440",
                extension = "jpg",
                width =200,
                height = 200
            };
            newsManagementBll.AddNewsImage(model);

        }

        /// <summary>
        /// 删除图片
        /// </summary>
        [TestMethod]
        public void DeleteImage_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetImage_Test001");
            newsManagementBll.DeleteImage(3);

            var imgList = newsManagementBll.GetImageList();
           
            //验证
            Assert.AreEqual(imgList.Count,2);
        }

        //获取登录用户昵称
        [TestMethod]
        public void author_Test001()
        {
            //导入测试数据
            DataUtility.InsertDataBase("NewsManagementBLLTestData.xlsx", "GetNewsInfo");
           var name=newsManagementBll.author(1);
            //验证
           Assert.AreEqual(name,"wqx");
        }

    }
}
