using System;
using System.Collections.Generic;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface INewsBLL
    {
        NewsInfo getNewsbyId(int id);

        /// 根据状态获取News信息
        List<NewsInfo> GetNewsByNotice(bool flag, int DirId);

        /// 根据状态获取重要News
        List<NewsInfo> GetTopNewsByNotice(bool flag);

        NewsDirectoryInfo GetTitle(int NewsId, bool flag);

        NewsDirectoryInfo GetDnameByDid(int Did, bool flag, TargetNavigationDBEntities db);

        List<NewsDirectoryInfo> GetDirectoryList(int dirId, bool Flag);

        List<NoticeDirectoryInfo> GetNoteDirectoryList(int dirId);

        NewsInfo getNewsInfoById(int NewsId, TargetNavigationDBEntities db);

        tblNews GetBytblInfoId(int NewsId, TargetNavigationDBEntities db);

        NewsInfo GetNextNews(DateTime creatTime, bool Flag);

        void UpdateNewsNum(int id);

        NewsInfo GetLastNews(DateTime creatTime, bool Flag);
    }
}