using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MB.New.Common
{
    public static class TagUtility
    {
        #region 常量区域

        /// <summary>用户常用标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_USER = "tag:user:";

        /// <summary>检索用标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_SEARCH = "tag:search:";

        /// <summary>一般计划标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_PLAN = "tag:plan:";

        /// <summary>循环计划标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_LOOP_PLAN = "tag:loopplan";

        /// <summary>用户文档标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_USER_DOCUMENT = "tag:udoc:";

        /// <summary>公司文档标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_COMPANY_DOCUMENT = "tag:cdoc:";

        /// <summary>新闻/通知标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_NEWS = "tag:news:";

        /// <summary>目标标签Key前缀</summary>
        private const string KEY_PREFIX_TAG_OBJECTIVE = "tag:objective:";

        /// <summary>常用标签的单位分值 </summary>
        private const double SCORE_INCREMENT_OR_DECREMENT = 10;

        /// <summary>用户常用标签数量</summary>
        private const long NUMBER_USER_MOST_USED_TAG = 20;

        /// <summary>用户检索标签数量</summary>
        private const long NUMBER_USER_RECENT_SEARCH_TAG = 20;

        #endregion 常量区域

        #region 取得登陆用户常用的标签

        /// <summary>
        /// 取得用户常用的标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string[] GetTopTag(int userId, int num)
        {
            IDatabase _Cache = RedisHelper.redis.GetDatabase();

            long length = _Cache.SortedSetLength(KEY_PREFIX_TAG_USER + userId);
            if (length == 0) return null;
            long start = 0;
            long stop = num > length ? length - 1 : num - 1;

            return _Cache.SortedSetRangeByRank(KEY_PREFIX_TAG_USER + userId, start, stop, Order.Descending).Select(p => p.ToString()).ToArray();
        }

        #endregion 取得登陆用户常用的标签

        #region 用户搜索标签处理

        /// <summary>
        /// 取得用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string[] GetRecentSearchTag(int userId, int num)
        {
            IDatabase _Cache = RedisHelper.redis.GetDatabase();

            long length = _Cache.ListLength(KEY_PREFIX_TAG_SEARCH + userId);
            if (length == 0) return null;
            long start = 0;
            long stop = num > length ? length - 1 : num - 1;
            return _Cache.ListRange(KEY_PREFIX_TAG_SEARCH + userId, start, stop).Select(p => p.ToString()).ToArray();
        }

        /// <summary>
        /// 保存用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagNames"></param>
        public static async Task SaveRecentSearchTagAsync(int userId, string[] tagNames)
        {
            IDatabase _Cache = RedisHelper.redis.GetDatabase();

            if (tagNames.Length == 0) return;

            foreach (var name in tagNames)
            {
                //移除列表中重名的标签
                _Cache.ListRemoveAsync(KEY_PREFIX_TAG_SEARCH + userId, name, 0);
                //保存最新的标签
                _Cache.ListLeftPushAsync(KEY_PREFIX_TAG_SEARCH + userId, name);
            }

            //保留最近的检索标签
            long length = _Cache.ListLengthAsync(KEY_PREFIX_TAG_SEARCH + userId).Result;
            if (length > NUMBER_USER_RECENT_SEARCH_TAG)
                _Cache.ListTrimAsync(KEY_PREFIX_TAG_SEARCH + userId, 0, NUMBER_USER_RECENT_SEARCH_TAG - 1);
        }

        #endregion 用户搜索标签处理

        #region 添加标签缓存

        /// <summary>
        /// 添加标签缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="tagNames"></param>
        /// <param name="type"></param>
        public static async Task AddTagCacheAsync(int userId, int targetId, string[] tagNames, EnumDefine.TagType type)
        {
            IDatabase _Cache = RedisHelper.redis.GetDatabase();

            if (tagNames.Length == 0) return;

            var preKey = GetKeyPrefix(type);

            foreach (var name in tagNames)
            {
                //保存缓存标签
                _Cache.ListLeftPushAsync(preKey + name, targetId);

                //保存用户常用标签
                _Cache.SortedSetIncrementAsync(KEY_PREFIX_TAG_USER + userId, name, SCORE_INCREMENT_OR_DECREMENT);
            }

            long length = _Cache.SortedSetLengthAsync(KEY_PREFIX_TAG_USER + userId).Result;
            if (length > NUMBER_USER_MOST_USED_TAG)
                //保留最常用的20个标签
                _Cache.SortedSetRemoveRangeByRankAsync(KEY_PREFIX_TAG_USER + userId, -length, -NUMBER_USER_MOST_USED_TAG - 1);
        }

        #endregion 添加标签缓存

        #region 移除标签缓存

        /// <summary>
        /// 移除标签缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="tagNames"></param>
        /// <param name="type"></param>
        public static async Task RemoveTagCacheAsync(int userId, int targetId, string[] tagNames, EnumDefine.TagType type)
        {
            IDatabase _Cache = RedisHelper.redis.GetDatabase();

            if (tagNames.Length == 0) return;

            var preKey = GetKeyPrefix(type);

            foreach (var name in tagNames)
            {
                _Cache.SortedSetDecrementAsync(KEY_PREFIX_TAG_USER + userId, name, SCORE_INCREMENT_OR_DECREMENT);

                var score = _Cache.SortedSetScoreAsync(KEY_PREFIX_TAG_USER + userId, name).Result ?? 0;

                if (score == 0) _Cache.SortedSetRemoveAsync(KEY_PREFIX_TAG_USER + userId, name);

                var targetKey = preKey + name;
                _Cache.ListRemoveAsync(targetKey, targetId);

                if (_Cache.ListLengthAsync(targetKey).Result == 0) _Cache.KeyDeleteAsync(targetKey);
            }
        }

        #endregion 移除标签缓存

        #region 取得标签检索结果

        /// <summary>
        /// 取得标签检索结果
        /// </summary>
        /// <param name="tagNames"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<int> GetTagSearchResult(string[] tagNames, EnumDefine.TagType type)
        {
            IDatabase _Cache = RedisHelper.redis.GetDatabase();

            var targetIds = new List<int>();

            var preKey = GetKeyPrefix(type);

            foreach (var name in tagNames)
            {
                var resultIds = _Cache.ListRange(preKey + name).Select(p => (int)p).ToList();

                targetIds.AddRange(resultIds);
            }

            return targetIds;
        }

        #endregion 取得标签检索结果

        #region 私有方法

        #region 取得标签Key前缀

        /// <summary>
        /// 取得标签Key前缀
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetKeyPrefix(EnumDefine.TagType type)
        {
            var preKey = string.Empty;
            switch (type)
            {
                case EnumDefine.TagType.Plan:
                    preKey = KEY_PREFIX_TAG_PLAN;
                    break;

                case EnumDefine.TagType.LoopPlan:
                    preKey = KEY_PREFIX_TAG_LOOP_PLAN;
                    break;

                case EnumDefine.TagType.UserDocument:
                    preKey = KEY_PREFIX_TAG_USER_DOCUMENT;
                    break;

                case EnumDefine.TagType.CompanyDocument:
                    preKey = KEY_PREFIX_TAG_COMPANY_DOCUMENT;
                    break;

                case EnumDefine.TagType.Objective:
                    preKey = KEY_PREFIX_TAG_OBJECTIVE;
                    break;

                case EnumDefine.TagType.News:
                    preKey = KEY_PREFIX_TAG_NEWS;
                    break;
            }

            return preKey;
        }

        #endregion 取得标签Key前缀

        #endregion 私有方法
    }
}