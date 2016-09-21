using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class NewsBLL : INewsBLL
    {
        public NewsInfo getNewsbyId(int id)
        {
            NewsInfo news = new NewsInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                news = getNewsInfoById(id, db);
            }
            return news;
        }

        /// <summary>
        /// 根据状态获取News信息
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<NewsInfo> GetNewsByNotice(bool flag, int DirId)
        {
            var newsList = new List<NewsInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (flag == false)
                {
                    if (DirId == 0)
                    {
                        newsList = (from news in db.tblNews
                                    where news.notice == flag
                                    orderby news.createTime descending
                                    select new NewsInfo
                                    {
                                        newId = news.newId,
                                        title = news.title,
                                        notice = news.notice,
                                        contents = news.contents,
                                        titleImage = news.titleImage,
                                        createTime = news.createTime,
                                        summary = news.summary,
                                        viewNum = news.viewNum
                                    }).ToList();
                    }
                    else
                    {
                        newsList = (from news in db.tblNews
                                    join directory in db.tblNewsDirectory on news.directoryId equals directory.directoryId
                                    where news.notice == flag && (news.directoryId == DirId || directory.parentDirectory == DirId)
                                    orderby news.createTime descending
                                    select new NewsInfo
                                    {
                                        newId = news.newId,
                                        title = news.title,
                                        notice = news.notice,
                                        contents = news.contents,
                                        titleImage = news.titleImage,
                                        createTime = news.createTime,
                                        summary = news.summary,
                                        viewNum = news.viewNum
                                    }).ToList();
                    }
                }
                else
                {
                    if (DirId == 0)
                    {
                        newsList = (from news in db.tblNews
                                    where news.notice == flag
                                    orderby news.createTime descending
                                    select new NewsInfo
                                    {
                                        newId = news.newId,
                                        title = news.title,
                                        notice = news.notice,
                                        contents = news.contents,
                                        titleImage = news.titleImage,
                                        createTime = news.createTime,
                                        summary = news.summary,
                                        viewNum = news.viewNum
                                    }).ToList();
                    }
                    else
                    {
                        newsList = (from news in db.tblNews
                                    join directory in db.tblNoticeDirectory on news.directoryId equals directory.directoryId
                                    where news.notice == flag && (news.directoryId == DirId || directory.parentDirectory == DirId)
                                    orderby news.createTime descending
                                    select new NewsInfo
                                    {
                                        newId = news.newId,
                                        title = news.title,
                                        notice = news.notice,
                                        contents = news.contents,
                                        titleImage = news.titleImage,
                                        createTime = news.createTime,
                                        summary = news.summary,
                                        viewNum = news.viewNum
                                    }).ToList();
                    }
                }
            }

            return newsList;
        }

        /// <summary>
        /// 根据状态获取重要News
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<NewsInfo> GetTopNewsByNotice(bool flag)
        {
            var newsList = new List<NewsInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                newsList = (from news in db.tblNews
                            where news.notice == flag && news.isTop == true
                            orderby news.createTime descending
                            select new NewsInfo
                            {
                                newId = news.newId,
                                title = news.title,
                                notice = news.notice,
                                contents = news.contents,
                                titleImage = news.titleImage,
                                createTime = news.createTime,
                                summary = news.summary,
                                viewNum = news.viewNum
                            }).ToList();
            }
            return newsList;
        }

        public NewsDirectoryInfo GetTitle(int NewsId, bool flag)
        {
            NewsDirectoryInfo name = new NewsDirectoryInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                var news = db.tblNews.Where(p => p.newId == NewsId).FirstOrDefault();
                if (news != null)
                {
                    var Did = news.directoryId;
                    name = this.GetDnameByDid(Convert.ToInt32(Did), flag, db);
                }
            }
            return name;
        }

        public NewsDirectoryInfo GetDnameByDid(int Did, bool flag, TargetNavigationDBEntities db)
        {
            NewsDirectoryInfo Pname = new NewsDirectoryInfo();
            if (flag == true)
            {
                Pname = (from directory in db.tblNoticeDirectory
                         where directory.directoryId == Did
                         select new NewsDirectoryInfo
                         {
                             directoryId = directory.directoryId,
                             directoryName = directory.directoryName,
                             parentDirectory = directory.parentDirectory
                         }).FirstOrDefault();
            }
            else
            {
                Pname = (from directory in db.tblNewsDirectory
                         where directory.directoryId == Did
                         select new NewsDirectoryInfo
                         {
                             directoryId = directory.directoryId,
                             directoryName = directory.directoryName,
                             parentDirectory = directory.parentDirectory
                         }).FirstOrDefault();
            }

            return Pname;
        }

        public List<NewsDirectoryInfo> GetDirectoryList(int dirId, bool Flag)
        {
            List<NewsDirectoryInfo> DirectoryList = new List<NewsDirectoryInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (Flag == false)
                {
                    if (dirId == 0)
                    {
                        DirectoryList = (from directory in db.tblNewsDirectory
                                         where directory.parentDirectory == null
                                         select new NewsDirectoryInfo
                                         {
                                             directoryId = directory.directoryId,
                                             directoryName = directory.directoryName,
                                             parentDirectory = directory.parentDirectory
                                         }).ToList();
                    }
                    else
                    {
                        DirectoryList = (from directory in db.tblNewsDirectory
                                         where directory.parentDirectory == dirId
                                         select new NewsDirectoryInfo
                                         {
                                             directoryId = directory.directoryId,
                                             directoryName = directory.directoryName,
                                             parentDirectory = directory.parentDirectory
                                         }).ToList();
                    }
                }
                else
                {
                    if (dirId == 0)
                    {
                        DirectoryList = (from directory in db.tblNoticeDirectory
                                         where directory.parentDirectory == null
                                         select new NewsDirectoryInfo
                                         {
                                             directoryId = directory.directoryId,
                                             directoryName = directory.directoryName,
                                             parentDirectory = directory.parentDirectory
                                         }).ToList();
                    }
                    else
                    {
                        DirectoryList = (from directory in db.tblNoticeDirectory
                                         where directory.parentDirectory == null
                                         select new NewsDirectoryInfo
                                         {
                                             directoryId = directory.directoryId,
                                             directoryName = directory.directoryName,
                                             parentDirectory = directory.parentDirectory
                                         }).ToList();
                    }
                }
                return DirectoryList;
            }
        }

        public List<NoticeDirectoryInfo> GetNoteDirectoryList(int dirId)
        {
            List<NoticeDirectoryInfo> DirectoryList = new List<NoticeDirectoryInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (dirId == 0)
                {
                    DirectoryList = (from directory in db.tblNoticeDirectory
                                     where directory.parentDirectory == null
                                     select new NoticeDirectoryInfo
                                     {
                                         directoryId = directory.directoryId,
                                         directoryName = directory.directoryName,
                                         parentDirectory = directory.parentDirectory
                                     }).ToList();
                }
                else
                {
                    DirectoryList = (from directory in db.tblNoticeDirectory
                                     where directory.parentDirectory == null
                                     select new NoticeDirectoryInfo
                                     {
                                         directoryId = directory.directoryId,
                                         directoryName = directory.directoryName,
                                         parentDirectory = directory.parentDirectory
                                     }).ToList();
                }
            }
            return DirectoryList;
        }

        public NewsInfo getNewsInfoById(int NewsId, TargetNavigationDBEntities db)
        {
            UserInfo userinfo = new UserInfo();
            UserBLL userBll = new UserBLL();
            NewsInfo newsinfo = new NewsInfo();
            newsinfo = (from news in db.tblNews
                        where news.newId == NewsId
                        select new NewsInfo
                        {
                            newId = news.newId,
                            contents = news.contents,
                            createTime = news.createTime,
                            viewNum = news.viewNum,
                            title = news.title,
                            createUser = news.createUser,
                            notice = news.notice,
                            directoryId = news.directoryId
                        }).FirstOrDefault();
            userinfo = userBll.GetUserById(newsinfo.createUser);
            if (userinfo != null)
            {
                newsinfo.UserName = userinfo.userName;
            }
            return newsinfo;
        }

        public tblNews GetBytblInfoId(int NewsId, TargetNavigationDBEntities db)
        {
            return db.tblNews.Where(a => a.newId == NewsId).FirstOrDefault<tblNews>();
        }

        public NewsInfo GetNextNews(DateTime creatTime, bool Flag)
        {
            NewsInfo newsinfo = new NewsInfo();
            UserInfo userinfo = new UserInfo();
            UserBLL userBll = new UserBLL();
            using (var db = new TargetNavigationDBEntities())
            {
                newsinfo = (from news in db.tblNews
                            where news.createTime < creatTime &&
                            news.notice == Flag
                            orderby news.createTime descending
                            select new NewsInfo
                            {
                                newId = news.newId,
                                contents = news.contents,
                                createTime = news.createTime,
                                viewNum = news.viewNum,
                                title = news.title,
                                createUser = news.createUser
                            }).Take(1).FirstOrDefault();
                if (newsinfo != null)
                {
                    userinfo = userBll.GetUserById(newsinfo.createUser);
                    if (userinfo != null)
                    {
                        newsinfo.UserName = userinfo.userName;
                    }
                }
            }
            return newsinfo;
        }

        public void UpdateNewsNum(int id)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                //var operId = db.prcGetPrimaryKey("tblNews", obj).FirstOrDefault().Value;
                tblNews firstData = GetBytblInfoId(id, db);
                firstData.viewNum = firstData.viewNum + 1;
                db.SaveChanges();
            }
        }

        public NewsInfo GetLastNews(DateTime creatTime, bool Flag)
        {
            NewsInfo newsinfo = new NewsInfo();
            UserInfo userinfo = new UserInfo();
            UserBLL userBll = new UserBLL();
            using (var db = new TargetNavigationDBEntities())
            {
                newsinfo = (from news in db.tblNews
                            where news.createTime > creatTime
                            && news.notice == Flag
                            orderby news.createTime
                            select new NewsInfo
                            {
                                newId = news.newId,
                                contents = news.contents,
                                createTime = news.createTime,
                                viewNum = news.viewNum,
                                title = news.title,
                                createUser = news.createUser
                            }).Take(1).FirstOrDefault();
            }
            if (newsinfo != null)
            {
                userinfo = userBll.GetUserById(newsinfo.createUser);
                if (userinfo != null)
                {
                    newsinfo.UserName = userinfo.userName;
                }
            }

            return newsinfo;
        }
    }
}