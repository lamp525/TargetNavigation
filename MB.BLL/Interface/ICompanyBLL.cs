using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface ICompanyBLL
    {
        /// 获取新闻通知首页信息
        List<NewsInfo> GetNewsInfo();

        /// 获取首页图片新闻[JS轮播的图片新闻，前5条]
        List<NewsInfo> GetNewsImageNewsTop5();

        List<IndexImageInfo> GetCompabyImageTop5();

        /// 获取首页文档数据，前9条]
        List<DocumentInfo> GetDocumentTop9();

        DocumentInfo GetDocumentById(int docId);
    }
}