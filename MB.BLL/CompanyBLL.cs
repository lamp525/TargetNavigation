using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class CompanyBLL : ICompanyBLL
    {
        #region 获取新闻通知首页信息

        /// <summary>
        /// 获取新闻通知首页信息
        /// </summary>
        /// <param name="notice">标识：true、通知  false、新闻</param>
        /// <returns>新闻或信息列表</returns>
        public List<NewsInfo> GetNewsInfo()
        {
            var newsList = new List<NewsInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                newsList = (from news in db.tblNews
                            orderby news.createTime descending
                            select new NewsInfo
                            {
                                newId = news.newId,
                                title = news.title,
                                notice = news.notice,
                                contents = news.contents,
                                titleImage = news.titleImage,
                                createTime = news.createTime
                            }).ToList();
            }
            return newsList;
        }

        #endregion 获取新闻通知首页信息

        #region 公司首页获取图片新闻（JS轮播）               --谢小鹏

        /// <summary>
        /// 获取首页图片新闻[JS轮播的图片新闻，前5条]
        /// </summary>
        /// <returns></returns>
        public List<NewsInfo> GetNewsImageNewsTop5()
        {
            var imageNewsList = new List<NewsInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                imageNewsList = ((from news in db.tblNews
                                  orderby news.createTime descending
                                  where news.titleImage != null && (!news.titleImage.Equals(""))
                                  select new NewsInfo
                                  {
                                      newId = news.newId,
                                      title = news.title,
                                      contents = news.contents,
                                      titleImage = news.titleImage,
                                      createTime = news.createTime
                                  }).Take(5)).ToList();
            }
            return imageNewsList;
        }

        #endregion 公司首页获取图片新闻（JS轮播）               --谢小鹏

        public List<IndexImageInfo> GetCompabyImageTop5()
        {
            var imageNewsList = new List<IndexImageInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                imageNewsList = ((from img in db.tblImage
                                  select new IndexImageInfo
                                  {
                                      imageId = img.imageId,
                                      imageName = img.saveName + "." + img.extension
                                  }).Take(5)).ToList();
            }
            return imageNewsList;
        }

        #region 公司首页获取“文档”数据前9条                --谢小鹏

        /// <summary>
        /// 获取首页文档数据，前9条]
        /// </summary>
        /// <returns></returns>
        public List<DocumentInfo> GetDocumentTop9()
        {
            var imageNewsList = new List<DocumentInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                imageNewsList = ((from docu in db.tblCompanyDocument
                                  where docu.isFolder == false
                                  orderby docu.createTime descending
                                  select new DocumentInfo
                                  {
                                      documentId = docu.documentId,
                                      //directoryId = docu.directoryId,
                                      displayName = docu.displayName,
                                      saveName = docu.saveName,
                                      extension = docu.extension,
                                      archiveTime = docu.archiveTime,
                                      createUser = docu.createUser,
                                      createTime = docu.createTime,
                                      updateUser = docu.updateUser,
                                      updateTime = docu.updateTime,
                                      deleteFlag = docu.deleteFlag
                                  }).Take(9)).ToList();
            }
            for (int i = 0; i < imageNewsList.Count; i++)
            {
                imageNewsList[i].dtShort = imageNewsList[i].createTime.ToString("yyyy-MM-dd");
            }
            return imageNewsList;
        }

        #endregion 公司首页获取“文档”数据前9条                --谢小鹏

        public DocumentInfo GetDocumentById(int docId)
        {
            DocumentInfo docInfo = new DocumentInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                docInfo = (from doc in db.tblCompanyDocument
                           where doc.documentId == docId
                           select new DocumentInfo
                           {
                               saveName = doc.saveName,
                               extension = doc.extension,
                               displayName = doc.displayName
                           }).FirstOrDefault();
            }
            return docInfo;
        }
    }
}