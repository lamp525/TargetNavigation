using MB.DAL;
using MB.New.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MB.New.BLL.Tag
{
    public class TagManagementBLL : ITagManagementBLL
    {
        #region 一般计划标签处理

        /// <summary>
        /// 移除一般计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planIds"></param>
        public async Task RemovePlanTagAsync(TargetNavigationDBEntities db, int userId, int[] planIds)
        {
            await RemoveTagAsync(db, userId, planIds, EnumDefine.TagType.Plan);
        }

        /// <summary>
        /// 移除一般计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public async Task RemovePlanTagAsync(TargetNavigationDBEntities db, int userId, int planId)
        {
            await RemoveTagAsync(db, userId, planId, EnumDefine.TagType.Plan);
        }

        /// <summary>
        /// 保存一般计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public async Task SavePlanTagAsync(TargetNavigationDBEntities db, int userId, int planId)
        {
            await SaveTagAsync(db, userId, planId, EnumDefine.TagType.Plan);
        }

        #endregion 一般计划标签处理

        #region 循环计划标签处理

        /// <summary>
        /// 移除循环计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopIds"></param>
        public async Task RemoveLoopPlanTagAsync(TargetNavigationDBEntities db, int userId, int[] loopIds)
        {
            await RemoveTagAsync(db, userId, loopIds, EnumDefine.TagType.LoopPlan);
        }

        /// <summary>
        /// 移除循环计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public async Task RemoveLoopPlanTagAsync(TargetNavigationDBEntities db, int userId, int loopId)
        {
            await RemoveTagAsync(db, userId, loopId, EnumDefine.TagType.LoopPlan);
        }

        /// <summary>
        /// 保存循环计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public async Task SaveLoopPlanTagAsync(TargetNavigationDBEntities db, int userId, int loopId)
        {
            await SaveTagAsync(db, userId, loopId, EnumDefine.TagType.LoopPlan);
        }

        #endregion 循环计划标签处理

        #region 目标标签处理

        /// <summary>
        /// 移除目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveIds"></param>
        public async Task RemoveObjectiveTag(int userId, int[] objectiveIds)
        {
            //   await RemoveTagAsync(userId, objectiveIds, EnumDefine.TagType.Objective);
        }

        /// <summary>
        /// 移除目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveId"></param>
        public async Task RemoveObjectiveTag(int userId, int objectiveId)
        {
            // await RemoveTagAsync(userId, objectiveId, EnumDefine.TagType.Objective);
        }

        /// <summary>
        /// 保存目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveId"></param>
        public async Task SaveObjectiveTag(int userId, int objectiveId)
        {
            //await SaveTagAsync(userId, objectiveId, EnumDefine.TagType.Objective);
        }

        #endregion 目标标签处理

        #region 新闻标签处理

        /// <summary>
        /// 移除新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsIds"></param>
        public async Task RemoveNewsTag(int userId, int[] newsIds)
        {
            // await RemoveTagAsync(userId, newsIds, EnumDefine.TagType.News);
        }

        /// <summary>
        /// 移除新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsId"></param>
        public async Task RemoveNewsTag(int userId, int newsId)
        {
            //  await RemoveTagAsync(userId, newsId, EnumDefine.TagType.News);
        }

        /// <summary>
        /// 保存新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsId"></param>
        public async Task SaveNewsTag(int userId, int newsId)
        {
            //await SaveTagAsync(userId, newsId, EnumDefine.TagType.News);
        }

        #endregion 新闻标签处理

        #region 文档标签处理

        /// <summary>
        /// 移除用户文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentIds"></param>
        public async Task RemoveUserDocumentTag(int userId, int[] documentIds)
        {
            // await RemoveTagAsync(userId, documentIds, EnumDefine.TagType.UserDocument);
        }

        /// <summary>
        /// 保存用户文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        public async Task SaveUserDocumentTag(int userId, int documentId)
        {
            //  await SaveTagAsync(userId, documentId, EnumDefine.TagType.UserDocument);
        }

        /// <summary>
        /// 移除公司文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentIds"></param>
        public async Task RemoveCompanyDocumentTag(int userId, int[] documentIds)
        {
            // await RemoveTagAsync(userId, documentIds, EnumDefine.TagType.CompanyDocument);
        }

        /// <summary>
        /// 保存公司文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        public async Task SaveCompanyDocumentTag(int userId, int documentId)
        {
            //await SaveTagAsync(userId, documentId, EnumDefine.TagType.CompanyDocument);
        }

        #endregion 文档标签处理

        #region 取得用户常用标签

        /// <summary>
        /// 取得登陆用户常用标签
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string[] GetMostUsedTag(int userId)
        {
            return TagUtility.GetTopTag(userId, ConstVar.MostUsedTagNum);
        }

        #endregion 取得用户常用标签

        #region 私有方法

        #region 缓存标签处理

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="targetIds"></param>
        /// <param name="type"></param>
        private async Task RemoveTagAsync(TargetNavigationDBEntities db, int userId, int[] targetIds, EnumDefine.TagType type)
        {
            if (targetIds.Length == 0) return;

            foreach (var id in targetIds)
            {
                var tag = GetTagFromDB(db, id, type);

                if (tag == null && tag.Length == 0) continue;

                await TagUtility.RemoveTagCacheAsync(userId, id, tag, type);
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        private async Task RemoveTagAsync(TargetNavigationDBEntities db, int userId, int targetId, EnumDefine.TagType type)
        {
            if (targetId == 0) return;

            var tag = GetTagFromDB(db, targetId, type);

            if (tag == null && tag.Length == 0) return;

            await TagUtility.RemoveTagCacheAsync(userId, targetId, tag, type);
        }

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        private async Task SaveTagAsync(TargetNavigationDBEntities db, int userId, int targetId, EnumDefine.TagType type)
        {
            var tag = GetTagFromDB(db, targetId, EnumDefine.TagType.Plan);

            if (tag == null || tag.Length == 0) return;

            await TagUtility.AddTagCacheAsync(userId, targetId, tag, type);
        }

        #endregion 缓存标签处理

        #region 从DB中取得信息标签

        /// <summary>
        /// 从DB中取得信息标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string[] GetTagFromDB(TargetNavigationDBEntities db, int targetId, EnumDefine.TagType type)
        {
            string[] result = null;

            string keyword = null;

            switch (type)
            {
                case EnumDefine.TagType.Plan:
                    keyword = db.tblPlan.Where(p => p.planId == targetId).Select(p => p.keyword).FirstOrDefault();
                    break;

                case EnumDefine.TagType.LoopPlan:
                    keyword = db.tblLoopPlan.Where(p => p.loopId == targetId).Select(p => p.keyword).FirstOrDefault();
                    break;

                case EnumDefine.TagType.News:
                    keyword = db.tblNews.Where(p => p.newId == targetId).Select(p => p.keyword).FirstOrDefault();
                    break;

                case EnumDefine.TagType.CompanyDocument:
                    keyword = db.tblCompanyDocument.Where(p => p.documentId == targetId).Select(p => p.keyword).FirstOrDefault();
                    break;

                case EnumDefine.TagType.UserDocument:
                    keyword = db.tblUserDocument.Where(p => p.documentId == targetId).Select(p => p.keyword).FirstOrDefault();
                    break;

                case EnumDefine.TagType.Objective:
                    keyword = db.tblObjective.Where(p => p.objectiveId == targetId).Select(p => p.keyword).FirstOrDefault();
                    break;
            }

            if (!string.IsNullOrEmpty(keyword)) result = keyword.Split(',');

            return result;
        }

        #endregion 从DB中取得信息标签

        #endregion 私有方法
    }
}