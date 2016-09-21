namespace MB.BLL
{
    public interface ITagManagementBLL
    {
        #region 系统标签处理【已删除】

        ///// <summary>
        ///// 系统标签查询
        ///// </summary>
        ///// <param name="tabName"></param>
        ///// <returns></returns>
        //List<TabModel> GetSystemTabList(string tabName = null);

        ///// <summary>
        ///// 系统标签新建或更新
        ///// </summary>
        ///// <param name="tabModel"></param>
        //void SaveSystemTab(TabModel tabModel);

        ///// <summary>
        ///// 系统标签删除
        ///// </summary>
        ///// <param name="Id"></param>
        //void DeleteSystemTab(int[] Id);

        #endregion 系统标签处理【已删除】

        /// <summary>
        /// 移除一般计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planIds"></param>
        void RemovePlanTag(int userId, int[] planIds);

        /// <summary>
        /// 保存一般计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        void SavePlanTag(int userId, int planId);

        /// <summary>
        /// 移除循环计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopIds"></param>
        void RemoveLoopPlanTag(int userId, int[] loopIds);

        /// <summary>
        /// 保存循环计划标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        void SaveLoopPlanTag(int userId, int loopId);

        /// <summary>
        /// 移除目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveIds"></param>
        void RemoveObjectiveTag(int userId, int[] objectiveIds);

        /// <summary>
        /// 移除目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveId"></param>
        void RemoveObjectiveTag(int userId, int objectiveId);

        /// <summary>
        /// 保存目标标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectiveId"></param>
        void SaveObjectiveTag(int userId, int objectiveId);

        #region 日程标签处理【已删除】

        ///// <summary>
        ///// 移除日程标签
        ///// </summary>
        ///// <param name="calendarIds"></param>
        ///// <param name="deleteTemp"></param>
        //void RemoveCalendarTag(int[] calendarIds, bool deleteTemp);

        ///// <summary>
        ///// 保存日程标签
        ///// </summary>
        ///// <param name="calendarId"></param>
        ///// <param name="tag"></param>
        ///// <param name="mode"></param>
        //void SaveCalendarTag(int calendarId, string tag, ConstVar.TagSaveMode mode);

        #endregion 日程标签处理【已删除】

        /// <summary>
        /// 移除新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsIds"></param>
        void RemoveNewsTag(int userId, int[] newsIds);

        /// <summary>
        /// 移除新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsId"></param>
        void RemoveNewsTag(int userId, int newsId);

        /// <summary>
        /// 保存新闻标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newsId"></param>
        void SaveNewsTag(int userId, int newsId);

        /// <summary>
        /// 移除个人文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentIds"></param>
        void RemoveUserDocumentTag(int userId, int[] documentIds);

        /// <summary>
        /// 保存个人文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        void SaveUserDocumentTag(int userId, int documentId);

        /// <summary>
        /// 移除公司文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentIds"></param>
        void RemoveCompanyDocumentTag(int userId, int[] documentIds);

        /// <summary>
        /// 保存公司文档标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        void SaveCompanyDocumentTag(int userId, int documentId);

        /// <summary>
        /// 取得用户常用标签
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string[] GetMostUsedTag(int userId);
    }
}