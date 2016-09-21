using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class NewsManagementBLL : INewsManagementBLL
    {
        //发布管理

        #region 获取(通知/新闻)列表

        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="flag">true:通知/false:新闻</param>
        /// <param name="directoryId">分类ID</param>
        /// <returns></returns>
        public List<NewsinfoModel> GetNewsList(int currentPage, int directoryId)
        {
            var newList = new List<NewsinfoModel>();
            var num = int.Parse(ConfigurationManager.AppSettings["pageNum"].ToString());
            using (var db = new TargetNavigationDBEntities())
            {
                if (directoryId == 0)
                {
                    newList = (from news in db.tblNews
                               join newsdirectory in db.tblNewsDirectory on news.directoryId equals newsdirectory.directoryId
                               where news.notice == false
                               orderby news.isTop descending, news.updateTime descending
                               select new NewsinfoModel
                               {
                                   newId = news.newId,
                                   directoryId = news.directoryId,
                                   title = news.title,
                                   notice = news.notice,
                                   createTime = news.createTime,
                                   publish = news.publish,
                                   isTop = news.isTop,
                                   updateTime = news.updateTime,
                                   directoryName = newsdirectory.directoryName,
                                   createUser = news.createUser,
                                   updateUser = news.updateUser,
                                   viewNum = news.viewNum
                               }).ToList();
                    foreach (var item in newList)
                    {
                        item.pageCount = newList.Count();
                    }
                }
                else
                {
                    newList = (from news in db.tblNews
                               join newsdirectory in db.tblNewsDirectory on news.directoryId equals newsdirectory.directoryId
                               where news.notice == false && news.directoryId == directoryId
                               orderby news.isTop descending, news.updateTime descending
                               select new NewsinfoModel
                               {
                                   newId = news.newId,
                                   directoryId = news.directoryId,
                                   title = news.title,
                                   notice = news.notice,
                                   createTime = news.createTime,
                                   publish = news.publish,
                                   isTop = news.isTop,
                                   updateTime = news.updateTime,
                                   directoryName = newsdirectory.directoryName,
                                   createUser = news.createUser,
                                   updateUser = news.updateUser,
                                   viewNum = news.viewNum
                               }).ToList();
                    foreach (var item in newList)
                    {
                        item.pageCount = newList.Count();
                    }
                }
            }
            newList = newList.Skip(num * (currentPage - 1)).Take(num).ToList();
            return newList;
        }

        /// <summary>
        /// 获取通知列表
        /// </summary>
        /// <param name="flag">true:通知/false:新闻</param>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        public List<NewsinfoModel> GetNoticeList(int currentPage, int directoryId)
        {
            var noticeInfo = new List<NewsinfoModel>();
            var num = int.Parse(ConfigurationManager.AppSettings["pageNum"].ToString());
            using (var db = new TargetNavigationDBEntities())
            {
                if (directoryId == 0)
                {
                    noticeInfo = (from news in db.tblNews
                                  join noticedirectory in db.tblNoticeDirectory on news.directoryId equals noticedirectory.directoryId
                                  where news.notice == true
                                  orderby news.isTop descending, news.updateTime descending
                                  select new NewsinfoModel
                                  {
                                      newId = news.newId,
                                      directoryId = news.directoryId,
                                      title = news.title,
                                      notice = news.notice,
                                      createTime = news.createTime,
                                      publish = news.publish,
                                      isTop = news.isTop,
                                      updateTime = news.updateTime,
                                      directoryName = noticedirectory.directoryName,
                                      createUser = news.createUser,
                                      updateUser = news.updateUser,
                                      viewNum = news.viewNum
                                  }).ToList();
                    foreach (var item in noticeInfo)
                    {
                        item.pageCount = noticeInfo.Count();
                    }
                    noticeInfo = noticeInfo.Skip(num * (currentPage - 1)).Take(num).ToList();
                }
                else
                {
                    noticeInfo = (from news in db.tblNews
                                  join noticedirectory in db.tblNoticeDirectory on news.directoryId equals noticedirectory.directoryId
                                  where news.notice == true && news.directoryId == directoryId
                                  orderby news.isTop descending, news.updateTime descending
                                  select new NewsinfoModel
                                  {
                                      newId = news.newId,
                                      directoryId = news.directoryId,
                                      title = news.title,
                                      notice = news.notice,
                                      createTime = news.createTime,
                                      publish = news.publish,
                                      isTop = news.isTop,
                                      updateTime = news.updateTime,
                                      directoryName = noticedirectory.directoryName,
                                      createUser = news.createUser,
                                      updateUser = news.updateUser,
                                      viewNum = news.viewNum
                                  }).ToList();
                    foreach (var item in noticeInfo)
                    {
                        item.pageCount = noticeInfo.Count();
                    }
                    noticeInfo = noticeInfo.Skip(num * (currentPage - 1)).Take(num).ToList();
                }
            }

            return noticeInfo;
        }

        #endregion 获取(通知/新闻)列表

        #region 批量删除新闻/通知

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="db"></param>
        public void DeleteNews(int[] newId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in newId)
                {
                    var newsinfo = db.tblNews.Where(c => c.newId == item).FirstOrDefault();
                    if (newsinfo != null)
                    {
                        db.tblNews.Remove(newsinfo);
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>

        #endregion 批量删除新闻/通知

        #region 根据新闻ID发布新闻/通知

        /// <summary>
        /// 根据新闻ID发布新闻/通知
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="db"></param>
        public void PublishNews(int[] newId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in newId)
                {
                    var newsinfo = db.tblNews.Where(c => c.newId == item).FirstOrDefault();
                    if (newsinfo != null)
                    {
                        //状态为已发布
                        newsinfo.publish = true;
                        newsinfo.updateTime = DateTime.Now;
                        newsinfo.updateUser = userId;
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 根据新闻ID发布新闻/通知

        #region 根据新闻ID取消发布新闻/通知

        /// <summary>
        /// 根据新闻ID取消发布新闻/通知
        /// </summary>
        /// <param name="newId">新闻ID</param>
        /// <param name="db"></param>
        public void UnPublishNews(int[] newId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in newId)
                {
                    var newsinfo = db.tblNews.Where(c => c.newId == item).FirstOrDefault();
                    if (newsinfo != null)
                    {
                        //状态为待发布
                        newsinfo.publish = false;
                        newsinfo.updateUser = userId;
                        newsinfo.updateTime = DateTime.Now;
                        newsinfo.isTop = false;
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 根据新闻ID取消发布新闻/通知

        #region 根据新闻ID将新闻/消息置顶

        /// <summary>
        /// 根据新闻ID将新闻/消息置顶
        /// </summary>
        public void SetTopNews(int[] newId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in newId)
                {
                    var newsinfo = db.tblNews.Where(c => c.newId == item).FirstOrDefault();
                    if (newsinfo != null)
                    {
                        newsinfo.isTop = true;
                        newsinfo.updateTime = DateTime.Now;
                        newsinfo.updateUser = userId;
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 根据新闻ID将新闻/消息置顶

        #region 根据新闻ID将新闻/消息取消置顶

        /// <summary>
        /// 根据新闻ID将新闻/消息取消置顶
        /// </summary>
        /// <param name="newId">新闻ID</param>
        /// <param name="db"></param>
        public void SetUnTopNews(int[] newId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in newId)
                {
                    var newsinfo = db.tblNews.Where(c => c.newId == item).FirstOrDefault();
                    if (newsinfo != null)
                    {
                        newsinfo.isTop = false;
                        newsinfo.updateUser = userId;
                        newsinfo.updateTime = DateTime.Now;
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 根据新闻ID将新闻/消息取消置顶

        //添加新闻

        #region 新闻详情

        /// <summary>
        /// 新闻详情
        /// </summary>
        /// <param name="newId"></param>
        /// <returns></returns>
        public NewsinfoModel GetNewsInfo(int newId)
        {
            var newsinfo = new NewsinfoModel();
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblNews.Where(c => c.newId == newId).FirstOrDefault();
                if (model == null)
                {
                    return null;
                }
                else
                {
                    newsinfo.newId = model.newId;
                    newsinfo.directoryId = model.directoryId;
                    newsinfo.title = model.title;
                    newsinfo.contents = model.contents;
                    newsinfo.summary = model.summary;
                    newsinfo.titleImage = model.titleImage;
                    newsinfo.notice = model.notice;
                    newsinfo.isTop = model.isTop;
                    newsinfo.keyword = model.keyword.Length > 0 ? model.keyword.Split(',') : null;
                    newsinfo.publish = model.publish;
                    newsinfo.createTime = model.createTime;
                    newsinfo.updateTime = model.updateTime;
                    newsinfo.viewNum = model.viewNum;
                    newsinfo.createUser = model.createUser;
                    newsinfo.updateUser = model.updateUser;
                    var usersModel = db.tblUser.Where(c => c.userId == model.createUser).FirstOrDefault();
                    if (usersModel != null)
                    {
                        newsinfo.UserName = usersModel.userName;
                    }
                    if (model.notice == true)
                    {
                        newsinfo.directoryName = db.tblNoticeDirectory.Where(c => c.directoryId == model.directoryId).FirstOrDefault().directoryName;
                    }
                    else
                    {
                        newsinfo.directoryName = db.tblNewsDirectory.Where(c => c.directoryId == model.directoryId).FirstOrDefault().directoryName;
                    }
                }
                return newsinfo;
            }
        }

        #endregion 新闻详情

        #region 添加/更新新闻

        /// <summary>
        /// 添加/更新新闻
        /// </summary>
        /// <param name="news"></param>
        /// <param name="userId"></param>
        public int SaveNews(NewsinfoModel news, int userId)
        {
            var newId = news.newId;

            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblNews.Where(c => c.newId == newId).FirstOrDefault();
                if (model == null)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    newId = db.prcGetPrimaryKey("tblNews", obj).FirstOrDefault().Value;
                    var newsmodel = new tblNews
                                {
                                    newId = newId,
                                    directoryId = news.directoryId,
                                    title = news.title,
                                    contents = news.contents,
                                    summary = news.summary,
                                    titleImage = news.titleImage,
                                    notice = news.notice,
                                    viewNum = news.viewNum,
                                    isTop = news.isTop,
                                    publish = news.publish,
                                    keyword = news.keyword != null && news.keyword.Length > 0 ? string.Join(",", news.keyword) : null,
                                    createTime = DateTime.Now,
                                    createUser = userId,
                                    updateTime = DateTime.Now,
                                    updateUser = userId
                                };
                    db.tblNews.Add(newsmodel);
                }
                else
                {
                    model.directoryId = news.directoryId;
                    model.title = news.title;
                    model.contents = news.contents;
                    model.summary = news.summary;
                    model.titleImage = news.titleImage;
                    model.notice = news.notice;
                    model.viewNum = news.viewNum;
                    model.isTop = news.isTop;
                    model.keyword = news.keyword != null && news.keyword.Length > 0 ? string.Join(",", news.keyword) : null;
                    model.publish = news.publish;
                    model.updateTime = DateTime.Now;
                    model.updateUser = userId;
                }
                db.SaveChanges();
            }

            return newId;
        }

        #endregion 添加/更新新闻

        //类别管理

        #region 获取新闻分类列表

        /// <summary>
        /// 获取新闻分类列表
        /// </summary>
        /// <param name="parentdir"></param>
        /// <returns></returns>
        public List<NewsDirModel> GetNewsTypeList(int? parentDir)
        {
            var modellist = new List<NewsDirModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                modellist = (from newsdirectory in db.tblNewsDirectory
                             where newsdirectory.parentDirectory == parentDir
                             orderby newsdirectory.orderNum
                             select new NewsDirModel
                             {
                                 directoryId = newsdirectory.directoryId,
                                 parentDirectory = newsdirectory.parentDirectory,
                                 directoryName = newsdirectory.directoryName,
                                 orderNum = newsdirectory.orderNum.Value,
                                 updateUser = newsdirectory.updateUser,
                                 createUser = newsdirectory.createUser,
                                 updateTime = DateTime.Now,
                                 createTime = DateTime.Now,
                                 isParent = false,
                                 childOuter = false,
                                 isNew = false
                             }).ToList();
                foreach (var item in modellist)
                {
                    item.id = item.directoryId;
                    item.name = item.directoryName;
                    var date = db.tblNewsDirectory.Where(p => p.parentDirectory == item.directoryId).Count();
                    if (date > 0)
                    {
                        item.isParent = true;
                    }
                    var newlist = GetNewsList(1, item.directoryId.Value).Count();
                    if (newlist > 0)
                    {
                        item.isNew = true;
                    }
                }
            }
            return modellist;
        }

        #endregion 获取新闻分类列表

        #region 获取通知分类列表

        /// <summary>
        /// 获取通知分类列表
        /// </summary>
        /// <param name="parentdir"></param>
        /// <returns></returns>
        public List<NewsDirModel> GetNoticeTypeList(int? parentdir)
        {
            var modellist = new List<NewsDirModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                modellist = (from noticedirectory in db.tblNoticeDirectory
                             where noticedirectory.parentDirectory == parentdir
                             orderby noticedirectory.orderNum
                             select new NewsDirModel
                             {
                                 directoryId = noticedirectory.directoryId,
                                 parentDirectory = noticedirectory.parentDirectory,
                                 directoryName = noticedirectory.directoryName,
                                 orderNum = noticedirectory.orderNum.Value,
                                 updateUser = noticedirectory.updateUser,
                                 createUser = noticedirectory.createUser,
                                 updateTime = DateTime.Now,
                                 createTime = DateTime.Now,
                                 isParent = false,
                                 childOuter = false,
                                 isNew = false
                             }).ToList();
                foreach (var item in modellist)
                {
                    item.id = item.directoryId;
                    item.name = item.directoryName;
                    var date = db.tblNoticeDirectory.Where(p => p.parentDirectory == item.directoryId).Count();
                    if (date > 0)
                    {
                        item.isParent = true;
                    }
                    var noticelist = GetNoticeList(1, item.directoryId.Value).Count();
                    if (noticelist > 0)
                    {
                        item.isNew = true;
                    }
                }
            }
            return modellist;
        }

        #endregion 获取通知分类列表

        #region 更新新闻分类排序

        /// <summary>
        /// 更新新闻分类排序
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="orderNum"></param>
        /// <param name="userId"></param>
        public void OrderNewsType(List<NewsDirModel> newsDir, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var model in newsDir)
                {
                    var updateModel = (from newsdirectory in db.tblNewsDirectory
                                       where newsdirectory.directoryId == model.directoryId
                                       select newsdirectory).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.orderNum = model.orderNum;
                        updateModel.updateTime = DateTime.Now;
                        updateModel.updateUser = userId;
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 更新新闻分类排序

        #region 更新通知分类排序

        /// <summary>
        /// 更新通知分类排序
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="orderNum"></param>
        /// <param name="userId"></param>
        public void OrderNoticeType(List<NewsDirModel> noticeDir, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var model in noticeDir)
                {
                    var updateModel = (from noticedirectory in db.tblNoticeDirectory
                                       where noticedirectory.directoryId == model.directoryId
                                       select noticedirectory).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.orderNum = model.orderNum;
                        updateModel.updateTime = DateTime.Now;
                        updateModel.updateUser = userId;
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 更新通知分类排序

        #region 批量删除新闻分类

        /// <summary>
        /// 批量删除新闻分类
        /// </summary>
        /// <param name="directoryId"></param>
        public void DeleteNewsType(int[] directoryId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in directoryId)
                {
                    var newsdirectory = db.tblNewsDirectory.Where(c => c.directoryId == item).FirstOrDefault();
                    if (newsdirectory != null)
                    {
                        db.tblNewsDirectory.Remove(newsdirectory);
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>

        #endregion 批量删除新闻分类

        #region 批量删除通知分类

        /// <summary>
        /// 批量删除通知分类
        /// </summary>
        /// <param name="directoryId"></param>
        public void DeleteNoticeType(int[] directoryId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in directoryId)
                {
                    var noticedirectory = db.tblNoticeDirectory.Where(c => c.directoryId == item).FirstOrDefault();
                    if (noticedirectory != null)
                    {
                        db.tblNoticeDirectory.Remove(noticedirectory);
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>

        #endregion 批量删除通知分类

        #region 新建/更新新闻分类

        /// <summary>
        /// 新建/更新新闻分类
        /// </summary>
        /// <param name="newsdir"></param>
        /// <param name="userId"></param>
        public void SaveNewsType(NewsDirModel newsDir, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var directoryId = db.prcGetPrimaryKey("tblNewsDirectory", obj).FirstOrDefault().Value;

                var newsDirectory = db.tblNewsDirectory.Where(c => c.directoryId == newsDir.directoryId).FirstOrDefault();
                if (newsDirectory == null)
                {
                    var model = new tblNewsDirectory
                    {
                        directoryId = directoryId,
                        directoryName = newsDir.directoryName,
                        parentDirectory = newsDir.parentDirectory,
                        orderNum = newsDir.orderNum,
                        createTime = DateTime.Now,
                        createUser = userId,
                        updateTime = DateTime.Now,
                        updateUser = userId
                    };
                    db.tblNewsDirectory.Add(model);
                }
                else
                {
                    newsDirectory.directoryName = newsDir.directoryName;
                    newsDirectory.parentDirectory = newsDir.parentDirectory;
                    newsDirectory.orderNum = newsDir.orderNum;
                    newsDirectory.updateTime = DateTime.Now;
                    newsDirectory.updateUser = userId;
                }
                db.SaveChanges();
            }
        }

        #endregion 新建/更新新闻分类

        #region 新建/更新通知分类

        /// <summary>
        /// 新建/更新通知分类
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="userId"></param>
        public void SaveNoticeType(NewsDirModel notice, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var directoryId = db.prcGetPrimaryKey("tblNoticeDirectory", obj).FirstOrDefault().Value;
                var noticeDirectory = db.tblNoticeDirectory.Where(c => c.directoryId == notice.directoryId).FirstOrDefault();
                if (noticeDirectory == null)
                {
                    var model = new tblNoticeDirectory
                    {
                        directoryId = directoryId,
                        directoryName = notice.directoryName,
                        parentDirectory = notice.parentDirectory,
                        createTime = DateTime.Now,
                        createUser = userId,
                        orderNum = notice.orderNum,
                        updateTime = DateTime.Now,
                        updateUser = userId
                    };
                    db.tblNoticeDirectory.Add(model);
                }
                else
                {
                    noticeDirectory.directoryName = notice.directoryName;
                    noticeDirectory.parentDirectory = notice.parentDirectory;
                    noticeDirectory.orderNum = notice.orderNum;
                    noticeDirectory.updateTime = DateTime.Now;
                    noticeDirectory.updateUser = userId;
                }
                db.SaveChanges();
            }
        }

        #endregion 新建/更新通知分类

        /// <summary>
        /// 新闻分类详情
        /// </summary>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        public NewsDirModel GetNewsModel(int directoryId)
        {
            var newsModel = new NewsDirModel();
            using (var db = new TargetNavigationDBEntities())
            {
                newsModel = (from newsDir in db.tblNewsDirectory
                             where newsDir.directoryId == directoryId
                             select new NewsDirModel
                             {
                                 directoryId = newsDir.directoryId,
                                 directoryName = newsDir.directoryName,
                                 parentDirectory = newsDir.parentDirectory,
                                 orderNum = newsDir.orderNum.Value,
                                 createUser = newsDir.createUser,
                                 createTime = newsDir.createTime,
                                 updateTime = newsDir.updateTime,
                                 updateUser = newsDir.updateUser
                             }).FirstOrDefault();
            }
            return newsModel;
        }

        /// <summary>
        /// 通知分类详情
        /// </summary>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        public NewsDirModel GetNoticeModel(int directoryId)
        {
            var noticeModel = new NewsDirModel();
            using (var db = new TargetNavigationDBEntities())
            {
                noticeModel = (from noticeDir in db.tblNoticeDirectory
                               where noticeDir.directoryId == directoryId
                               select new NewsDirModel
                               {
                                   directoryId = noticeDir.directoryId,
                                   directoryName = noticeDir.directoryName,
                                   parentDirectory = noticeDir.parentDirectory,
                                   orderNum = noticeDir.orderNum.Value,
                                   createUser = noticeDir.createUser,
                                   createTime = noticeDir.createTime,
                                   updateTime = noticeDir.updateTime,
                                   updateUser = noticeDir.updateUser
                               }).FirstOrDefault();
            }
            return noticeModel;
        }

        /// <summary>
        /// 获取登录用户的名字
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string author(int userId)
        {
            var author = "";
            using (var db = new TargetNavigationDBEntities())
            {
                author = db.tblUser.Where(c => c.userId == userId).Select(c => c.userName).FirstOrDefault();
            }
            return author;
        }

        /// <summary>
        /// 获取新闻图片列表
        /// </summary>
        /// <returns></returns>
        public List<ImageInfoModel> GetImageList()
        {
            var imageList = new List<ImageInfoModel>();
            var imgUrl = ConfigurationManager.AppSettings["NewsUpLoadPath"].ToString();
            using (var db = new TargetNavigationDBEntities())
            {
                imageList = (from newImage in db.tblImage
                             select new ImageInfoModel
                             {
                                 imageId = newImage.imageId,
                                 saveName = newImage.saveName,
                                 extension = newImage.extension,
                                 width = newImage.width.Value==null?0:newImage.width.Value,
                                 height = newImage.height.Value==null?0:newImage.height.Value,
                                 imageUrl = "\\" + imgUrl + "\\" + newImage.saveName + "." + newImage.extension
                             }).ToList();
            }
            return imageList;
        }

        /// <summary>
        /// 添加新闻图片
        /// </summary>
        /// <param name="imgModel"></param>
        public void AddNewsImage(ImageInfoModel imgModel)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var imgId = db.prcGetPrimaryKey("tblImage", obj).FirstOrDefault().Value;
                var model = new tblImage
                {
                    imageId = imgId,
                    saveName = imgModel.saveName,
                    extension = imgModel.extension,
                    width = imgModel.width,
                    height = imgModel.height
                };
                db.tblImage.Add(model);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除新闻图片
        /// </summary>
        /// <param name="imgId"></param>
        public void DeleteImage(int imgId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImage.Where(c => c.imageId == imgId).FirstOrDefault();
                if (model != null)
                {
                    db.tblImage.Remove(model);
                }
                db.SaveChanges();
            }
        }
    }
}