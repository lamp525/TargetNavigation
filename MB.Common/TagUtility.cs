using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace MB.Common
{
    public static class TagUtility
    {
        #region 变量区域

        /// <summary>Redis缓存实例</summary>
        private static readonly IDatabase Cache = TagRedis.redis.GetDatabase();

        #endregion 变量区域

        #region 常量区域

        private const string KEY_PREFIX_TAG_USER = "tag:user:";
        private const string KEY_PREFIX_TAG_SEARCH = "tag:search:";

        //private const string KEY_TAG_INFO = "tag:info";
        private const string KEY_PREFIX_TAG_PLAN = "tag:plan:";

        private const string KEY_PREFIX_TAG_LOOP_PLAN = "tag:loopplan";
        private const string KEY_PREFIX_TAG_USER_DOCUMENT = "tag:udoc:";
        private const string KEY_PREFIX_TAG_COMPANY_DOCUMENT = "tag:cdoc:";

        //private const string KEY_PREFIX_TAG_CALENDAR = "tag:calendar:";
        private const string KEY_PREFIX_TAG_NEWS = "tag:news:";

        private const string KEY_PREFIX_TAG_OBJECTIVE = "tag:objective:";

        /*
        private const string KEY_PREFIX_TAG_PLAN_TEMP = "temptag:plan:";
        private const string KEY_PREFIX_TAG_LOOP_PLAN_TEMP = "temptag:loopplan";
        private const string KEY_PREFIX_TAG_USER_DOCUMENT_TEMP = "temptag:udoc:";
        private const string KEY_PREFIX_TAG_COMPANY_DOCUMENT_TEMP = "temptag:cdoc:";
        private const string KEY_PREFIX_TAG_CALENDAR_TEMP = "temptag:calendar:";
        private const string KEY_PREFIX_TAG_NEWS_TEMP = "temptag:news:";
        private const string KEY_PREFIX_TAG_OBJECTIVE_TEMP = "temptag:objective:";
        */

        //private const double SCORE_DEFAULT_TAG = 9;
        private const double SCORE_INCREMENT_OR_DECREMENT = 10;

        /// <summary>用户常用标签数量</summary>
        private const long NUMBER_USER_MOST_USED_TAG = 20;

        /// <summary>用户检索标签数量</summary>
        private const long NUMBER_USER_RECENT_SEARCH_TAG = 10;

        #endregion 常量区域

        #region 临时标签处理【已删除】

        ///// <summary>
        ///// 保存临时标签
        ///// </summary>
        ///// <param name="targetId"></param>
        ///// <param name="tag"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static bool SaveTempTag(int targetId, string tag, ConstVar.TagType type)
        //{
        //    return Cache.StringSet(GetTempKeyPrefix(type) + targetId, tag);
        //}

        ///// <summary>
        ///// 取得临时标签
        ///// </summary>
        ///// <param name="targetId"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static string GetTempTag(int targetId, ConstVar.TagType type)
        //{
        //    return Cache.StringGet(GetTempKeyPrefix(type) + targetId);
        //}

        ///// <summary>
        ///// 删除临时标签
        ///// </summary>
        ///// <param name="targetId"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static bool RemoveTempTag(int targetId, ConstVar.TagType type)
        //{
        //    return Cache.KeyDelete(GetTempKeyPrefix(type) + targetId);
        //}

        //public static long RemoveTempTag(int[] tagetIds, ConstVar.TagType type)
        //{
        //    if (tagetIds.Length == 0) return 0L;

        //    var tempPreKey = GetTempKeyPrefix(type);

        //    RedisKey[] tempKeys = new RedisKey[tagetIds.Length];

        //    for (int i = 0; i < tagetIds.Length - 1; i++)
        //    {
        //        tempKeys[i] = (RedisKey)(tempPreKey + tagetIds[i]);
        //    }
        //    return Cache.KeyDelete(tempKeys);
        //}

        #endregion 临时标签处理【已删除】

        #region 系统标签【已删除】

        //#region 保存系统标签到缓存

        ///// <summary>
        ///// 保存系统标签到缓存
        ///// </summary>
        ///// <param name="tagName"></param>
        ///// <returns></returns>
        //public static double SaveSystemTag(string tagName)
        //{
        //    return Cache.SortedSetIncrement(KEY_TAG_INFO, tagName, SCORE_DEFAULT_TAG);
        //}

        //#endregion 保存系统标签到缓存

        //#region 删除缓存中的标签信息

        ///// <summary>
        ///// 删除缓存中的用户标签信息
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="tagName"></param>
        ///// <returns></returns>
        //public static bool RemoveTagInfo(int userId, string tagName)
        //{
        //    return Cache.SortedSetRemove(KEY_PREFIX_TAG_USER + userId, tagName);
        //}

        //#endregion 删除缓存中的标签信息

        #endregion 系统标签【已删除】

        #region 取得登陆用户常用的标签

        /// <summary>
        /// 取得用户常用的标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string[] GetTopTag(int userId, int num)
        {
            long length = Cache.SortedSetLength(KEY_PREFIX_TAG_USER + userId);
            if (length == 0) return null;
            long start = 0;
            long stop = num > length ? length - 1 : num - 1;

            return Cache.SortedSetRangeByRank(KEY_PREFIX_TAG_USER + userId, start, stop, Order.Descending).Select(p => p.ToString()).ToArray();
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
            long length = Cache.ListLength(KEY_PREFIX_TAG_SEARCH + userId);
            if (length == 0) return null;
            long start = 0;
            long stop = num > length ? length - 1 : num - 1;
            return Cache.ListRange(KEY_PREFIX_TAG_SEARCH + userId, start, stop).Select(p => p.ToString()).ToArray();
        }

        /// <summary>
        /// 保存用户最近使用的检索标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tagNames"></param>
        public static void SaveRecentSearchTag(int userId, string[] tagNames)
        {
            if (tagNames.Length == 0) return;

            foreach (var name in tagNames)
            {
                //移除列表中重名的标签
                Cache.ListRemove(KEY_PREFIX_TAG_SEARCH + userId, name, 0);
                //保存最新的标签
                Cache.ListLeftPush(KEY_PREFIX_TAG_SEARCH + userId, name);
            }

            //保留最近的10个检索标签
            if (Cache.ListLength(KEY_PREFIX_TAG_SEARCH + userId) > NUMBER_USER_RECENT_SEARCH_TAG)
                Cache.ListTrim(KEY_PREFIX_TAG_SEARCH + userId, 0, NUMBER_USER_RECENT_SEARCH_TAG - 1);
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
        public static void AddTagCache(int userId, int targetId, string[] tagNames, ConstVar.TagType type)
        {
            if (tagNames.Length == 0) return;

            var preKey = GetKeyPrefix(type);

            foreach (var name in tagNames)
            {
                //保存用户常用标签
                Cache.SortedSetIncrement(KEY_PREFIX_TAG_USER + userId, name, SCORE_INCREMENT_OR_DECREMENT);

                long length = Cache.SortedSetLength(KEY_PREFIX_TAG_USER + userId);
                if (length > NUMBER_USER_MOST_USED_TAG)
                    //保留最常用的20个标签
                    Cache.SortedSetRemoveRangeByRank(KEY_PREFIX_TAG_USER + userId, -length, -NUMBER_USER_MOST_USED_TAG);

                Cache.ListLeftPush(preKey + name, targetId);
            }
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
        public static void RemoveTagCache(int userId, int targetId, string[] tagNames, ConstVar.TagType type)
        {
            if (tagNames.Length == 0) return;

            var preKey = GetKeyPrefix(type);

            foreach (var name in tagNames)
            {
                Cache.SortedSetDecrement(KEY_PREFIX_TAG_USER + userId, name, SCORE_INCREMENT_OR_DECREMENT);

                var score = Cache.SortedSetScore(KEY_PREFIX_TAG_USER + userId, name) ?? 0L;

                if (score == 0L) Cache.SortedSetRemove(KEY_PREFIX_TAG_USER + userId, name);

                var targetKey = preKey + name;
                Cache.ListRemove(targetKey, targetId);

                if (Cache.ListLength(targetKey) == 0L) Cache.KeyDelete(targetKey);
            }
        }

        #endregion 移除标签缓存

        #region 标签检索

        /// <summary>
        /// 取得标签检索结果
        /// </summary>
        /// <param name="tagNames"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<int> GetTagSearchResult(string[] tagNames, ConstVar.TagType type)
        {
            var targetIds = new List<int>();

            var preKey = GetKeyPrefix(type);

            foreach (var name in tagNames)
            {
                var resultIds = Cache.ListRange(preKey + name).Select(p => (int)p).ToList();

                targetIds.AddRange(resultIds);
            }

            return targetIds;
        }

        #endregion 标签检索

        #region 私有方法

        #region 取得标签Key前缀

        /// <summary>
        /// 取得标签Key前缀
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetKeyPrefix(ConstVar.TagType type)
        {
            var preKey = string.Empty;
            switch (type)
            {
                case ConstVar.TagType.Plan:
                    preKey = KEY_PREFIX_TAG_PLAN;
                    break;

                case ConstVar.TagType.LoopPlan:
                    preKey = KEY_PREFIX_TAG_LOOP_PLAN;
                    break;

                //case ConstVar.TagType.Calendar:
                //    preKey = KEY_PREFIX_TAG_CALENDAR;
                //    break;

                case ConstVar.TagType.UserDocument:
                    preKey = KEY_PREFIX_TAG_USER_DOCUMENT;
                    break;

                case ConstVar.TagType.CompanyDocument:
                    preKey = KEY_PREFIX_TAG_COMPANY_DOCUMENT;
                    break;

                case ConstVar.TagType.Objective:
                    preKey = KEY_PREFIX_TAG_OBJECTIVE;
                    break;

                case ConstVar.TagType.News:
                    preKey = KEY_PREFIX_TAG_NEWS;
                    break;
            }

            return preKey;
        }

        #endregion 取得标签Key前缀

        #region 取得标签临时Key前缀【已删除】

        ///// <summary>
        ///// 取得标签临时Key前缀
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //private static string GetTempKeyPrefix(ConstVar.TagType type)
        //{
        //    var preKey = string.Empty;
        //    switch (type)
        //    {
        //        case ConstVar.TagType.Plan:
        //            preKey = KEY_PREFIX_TAG_PLAN_TEMP;
        //            break;

        //        case ConstVar.TagType.LoopPlan:
        //            preKey = KEY_PREFIX_TAG_LOOP_PLAN_TEMP;
        //            break;

        //        case ConstVar.TagType.Calendar:
        //            preKey = KEY_PREFIX_TAG_CALENDAR_TEMP;
        //            break;

        //        case ConstVar.TagType.UserDocument:
        //            preKey = KEY_PREFIX_TAG_USER_DOCUMENT_TEMP;
        //            break;

        //        case ConstVar.TagType.CompanyDocument:
        //            preKey = KEY_PREFIX_TAG_COMPANY_DOCUMENT_TEMP;
        //            break;

        //        case ConstVar.TagType.Objective:
        //            preKey = KEY_PREFIX_TAG_OBJECTIVE_TEMP;
        //            break;

        //        case ConstVar.TagType.News:
        //            preKey = KEY_PREFIX_TAG_NEWS_TEMP;
        //            break;
        //    }

        //    return preKey;
        //}

        #endregion 取得标签临时Key前缀【已删除】

        #endregion 私有方法
    }
}