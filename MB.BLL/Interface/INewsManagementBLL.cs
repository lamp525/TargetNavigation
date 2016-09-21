using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface INewsManagementBLL
    {
        /// 获取新闻列表
        List<NewsinfoModel> GetNewsList(int currentPage, int directoryId);

        /// 获取通知列表
        List<NewsinfoModel> GetNoticeList(int currentPage, int directoryId);

        /// 批量删除
        void DeleteNews(int[] newId);

        /// 根据新闻ID发布新闻/通知
        void PublishNews(int[] newId, int userId);

        /// 根据新闻ID取消发布新闻/通知
        void UnPublishNews(int[] newId, int userId);

        /// 根据新闻ID将新闻/消息置顶
        void SetTopNews(int[] newId, int userId);

        /// 根据新闻ID将新闻/消息取消置顶
        void SetUnTopNews(int[] newId, int userId);

        /// 新闻详情
        NewsinfoModel GetNewsInfo(int newId);

        /// 添加/更新新闻
        int SaveNews(NewsinfoModel news, int userId);

        /// 获取新闻分类列表
        List<NewsDirModel> GetNewsTypeList(int? parentDir);

        /// 获取通知分类列表
        List<NewsDirModel> GetNoticeTypeList(int? parentdir);

        /// 更新新闻分类排序
        void OrderNewsType(List<NewsDirModel> newsDir, int userId);

        /// 更新通知分类排序
        void OrderNoticeType(List<NewsDirModel> noticeDir, int userId);

        /// 批量删除新闻分类
        void DeleteNewsType(int[] directoryId);

        /// 批量删除通知分类
        void DeleteNoticeType(int[] directoryId);

        /// 新建/更新新闻分类
        void SaveNewsType(NewsDirModel newsDir, int userId);

        /// 新建/更新通知分类
        void SaveNoticeType(NewsDirModel notice, int userId);

        /// 新闻分类详情
        NewsDirModel GetNewsModel(int directoryId);

        /// 通知分类详情
        NewsDirModel GetNoticeModel(int directoryId);

        /// 获取登录用户的名字
        string author(int userId);

        /// 获取新闻图片列表
        List<ImageInfoModel> GetImageList();

        /// 添加新闻图片
        void AddNewsImage(ImageInfoModel imgModel);

        /// 删除新闻图片
        void DeleteImage(int imgId);
    }
}