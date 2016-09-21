using MB.Model;

namespace MB.BLL
{
    public interface ITagSearchBLL
    {
        /// <summary>
        /// 取得搜索结果信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        object GetSearchResult(int userId, SearchInfoModel searchInfo);

        /// <summary>
        /// 保存用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tags"></param>
        void SaveRecentSearchTag(int userId, string[] tags);

        /// <summary>
        /// 取得用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        string[] GetRecentSearchTag(int userId);
    }
}