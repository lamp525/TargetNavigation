using System.Configuration;
using System.Linq;
using MB.Common;
using MB.DAL;

namespace MB.BLL
{
    public class TagManagementBLL : ITagManagementBLL
    {
        private string mostUsedTagNum = ConfigurationManager.AppSettings["MostUsedTagNum"].ToString();

        private const int DEFAULT_TAG_NUM = 7;

        #region 系统标签处理【已删除】

        ///// <summary>
        ///// 系统标签删除
        ///// </summary>
        ///// <param name="Id"></param>
        //public void DeleteSystemTab(int[] Id)
        //{
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        var list = db.tblTab.Where(c => Id.Contains(c.tabId)).ToList();
        //        if (list.Count != 0)
        //        {
        //            db.tblTab.RemoveRange(list);
        //        }
        //        db.SaveChanges();
        //    }
        //}

        ///// <summary>
        ///// 系统标签查询
        ///// </summary>
        ///// <param name="tabName"></param>
        ///// <returns></returns>
        //public List<TabModel> GetSystemTabList(string tabName = null)
        //{
        //    var list = new List<TabModel>();
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        if (tabName != null)
        //        {
        //            list = (from tab in db.tblTab
        //                    where tab.tabName == tabName
        //                    select new TabModel
        //                    {
        //                        tabId = tab.tabId,
        //                        tabName = tab.tabName,
        //                        num = tab.num,
        //                        systemTab = tab.systemTab
        //                    }).ToList();
        //        }
        //        else
        //        {
        //            list = (from tab in db.tblTab
        //                    select new TabModel
        //                    {
        //                        tabId = tab.tabId,
        //                        tabName = tab.tabName,
        //                        num = tab.num,
        //                        systemTab = tab.systemTab
        //                    }).ToList();
        //        }
        //    }

        //    return list;
        //}

        ///// <summary>
        ///// 系统标签新建或更新
        ///// </summary>
        ///// <param name="tabModel"></param>
        //public void SaveSystemTab(TabModel tabModel)
        //{
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
        //        var TabId = db.prcGetPrimaryKey("tblTab", obj).FirstOrDefault().Value;

        //        var Tab = db.tblTab.Where(c => c.tabId == tabModel.tabId).FirstOrDefault();
        //        if (Tab == null)
        //        {//新增
        //            var model = new tblTab
        //            {
        //                tabId = TabId,
        //                tabName = tabModel.tabName,
        //                num = tabModel.num,
        //                systemTab = tabModel.systemTab
        //            };
        //            db.tblTab.Add(model);
        //        }
        //        else
        //        { //更新
        //            Tab.tabId = tabModel.tabId;
        //            Tab.tabName = tabModel.tabName;
        //            Tab.num = tabModel.num;
        //            Tab.systemTab = tabModel.systemTab;
        //        }
        //        db.SaveChanges();
        //    }
        //}

        #endregion 系统标签处理【已删除】

        #region 一般计划标签处理

        /// <summary>
        /// 移除一般计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planIds"></param>
        public void RemovePlanTag(int userId, int[] planIds)
        {
            this.RemoveTag(userId, planIds, ConstVar.TagType.Plan);
        }

        /// <summary>
        /// 保存一般计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        public void SavePlanTag(int userId, int planId)
        {
            this.SaveTag(userId, planId, ConstVar.TagType.Plan);
        }

        #endregion 一般计划标签处理

        #region 循环计划标签处理

        /// <summary>
        /// 移除循环计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopIds"></param>
        public void RemoveLoopPlanTag(int userId, int[] loopIds)
        {
            this.RemoveTag(userId, loopIds, ConstVar.TagType.LoopPlan);
        }

        /// <summary>
        /// 保存循环计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        public void SaveLoopPlanTag(int userId, int loopId)
        {
            this.SaveTag(userId, loopId, ConstVar.TagType.LoopPlan);
        }

        #endregion 循环计划标签处理

        #region 目标标签处理

        /// <summary>
        /// 移除目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveIds"></param>
        public void RemoveObjectiveTag(int userId, int[] objectiveIds)
        {
            this.RemoveTag(userId, objectiveIds, ConstVar.TagType.Objective);
        }

        /// <summary>
        /// 移除目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveId"></param>
        public void RemoveObjectiveTag(int userId, int objectiveId)
        {
            this.RemoveTag(userId, objectiveId, ConstVar.TagType.Objective);
        }

        /// <summary>
        /// 保存目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveId"></param>
        public void SaveObjectiveTag(int userId, int objectiveId)
        {
            this.SaveTag(userId, objectiveId, ConstVar.TagType.Objective);
        }

        #endregion 目标标签处理

        #region 日程标签处理【已删除】

        ///// <summary>
        ///// 移除日程标签
        ///// </summary>
        ///// <param name="calendarIds"></param>
        ///// <param name="deleteTemp"></param>
        //public void RemoveCalendarTag(int[] calendarIds,bool deleteTemp)
        //{
        //    this.RemoveTag(calendarIds, ConstVar.TagType.Calendar,deleteTemp);
        //}

        ///// <summary>
        ///// 保存日程标签
        ///// </summary>
        ///// <param name="calendarId"></param>
        ///// <param name="tag"></param>
        ///// <param name="mode"></param>
        //public void SaveCalendarTag(int calendarId, string tag, ConstVar.TagSaveMode mode)
        //{
        //    this.SaveTag(calendarId, tag, ConstVar.TagType.Calendar, mode);
        //}

        #endregion 日程标签处理【已删除】

        #region 新闻标签处理

        /// <summary>
        /// 移除新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsIds"></param>
        public void RemoveNewsTag(int userId, int[] newsIds)
        {
            this.RemoveTag(userId, newsIds, ConstVar.TagType.News);
        }

        /// <summary>
        /// 移除新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsId"></param>
        public void RemoveNewsTag(int userId, int newsId)
        {
            this.RemoveTag(userId, newsId, ConstVar.TagType.News);
        }

        /// <summary>
        /// 保存新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsId"></param>
        public void SaveNewsTag(int userId, int newsId)
        {
            this.SaveTag(userId, newsId, ConstVar.TagType.News);
        }

        #endregion 新闻标签处理

        #region 文档标签处理

        /// <summary>
        /// 移除用户文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentIds"></param>
        public void RemoveUserDocumentTag(int userId, int[] documentIds)
        {
            this.RemoveTag(userId, documentIds, ConstVar.TagType.UserDocument);
        }

        /// <summary>
        /// 保存用户文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        public void SaveUserDocumentTag(int userId, int documentId)
        {
            this.SaveTag(userId, documentId, ConstVar.TagType.UserDocument);
        }

        /// <summary>
        /// 移除公司文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentIds"></param>
        public void RemoveCompanyDocumentTag(int userId, int[] documentIds)
        {
            this.RemoveTag(userId, documentIds, ConstVar.TagType.CompanyDocument);
        }

        /// <summary>
        /// 保存公司文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        public void SaveCompanyDocumentTag(int userId, int documentId)
        {
            this.SaveTag(userId, documentId, ConstVar.TagType.CompanyDocument);
        }

        #endregion 文档标签处理

        #region 取得登陆用户常用标签

        /// <summary>
        /// 取得登陆用户常用标签
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string[] GetMostUsedTag(int userId)
        {
            var num = string.IsNullOrEmpty(mostUsedTagNum) ? DEFAULT_TAG_NUM : int.Parse(mostUsedTagNum);
            return TagUtility.GetTopTag(userId, num);
        }

        #endregion 取得登陆用户常用标签

        #region 私有方法

        #region 缓存标签处理

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetIds"></param>
        /// <param name="type"></param>
        private void RemoveTag(int userId, int[] targetIds, ConstVar.TagType type)
        {
            if (targetIds.Length == 0) return;

            foreach (var id in targetIds)
            {
                var tag = this.GetTagFromDB(id, type);

                if (string.IsNullOrWhiteSpace(tag)) return;

                TagUtility.RemoveTagCache(userId, id, tag.Split(','), type);
            }
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        private void RemoveTag(int userId, int targetId, ConstVar.TagType type)
        {
            if (targetId == 0) return;

            var tag = this.GetTagFromDB(targetId, type);

            if (string.IsNullOrWhiteSpace(tag)) return;

            TagUtility.RemoveTagCache(userId, targetId, tag.Split(','), type);
        }

        /// <summary>
        /// 保存标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        private void SaveTag(int userId, int targetId, ConstVar.TagType type)
        {
            var tagNames = this.GetTagFromDB(targetId, type);

            if (string.IsNullOrWhiteSpace(tagNames)) return;

            TagUtility.AddTagCache(userId, targetId, tagNames.Split(','), type);
        }

        #endregion 缓存标签处理

        #region 从DB中取得信息标签

        /// <summary>
        /// 从DB中取得信息标签
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTagFromDB(int targetId, ConstVar.TagType type)
        {
            string result = null;

            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                switch (type)
                {
                    case ConstVar.TagType.Plan:
                        result = db.tblPlan.Where(p => p.planId == targetId).Select(p => p.keyword).FirstOrDefault();
                        break;

                    case ConstVar.TagType.LoopPlan:
                        result = db.tblLoopPlan.Where(p => p.loopId == targetId).Select(p => p.keyword).FirstOrDefault();
                        break;

                    case ConstVar.TagType.News:
                        result = db.tblNews.Where(p => p.newId == targetId).Select(p => p.keyword).FirstOrDefault();
                        break;

                    case ConstVar.TagType.CompanyDocument:
                        result = db.tblCompanyDocument.Where(p => p.documentId == targetId).Select(p => p.keyword).FirstOrDefault();
                        break;

                    case ConstVar.TagType.UserDocument:
                        result = db.tblUserDocument.Where(p => p.documentId == targetId).Select(p => p.keyword).FirstOrDefault();
                        break;

                    case ConstVar.TagType.Objective:
                        result = db.tblObjective.Where(p => p.objectiveId == targetId).Select(p => p.keyword).FirstOrDefault();
                        break;
                }
            }

            return result;
        }

        #endregion 从DB中取得信息标签

        #endregion 私有方法
    }
}