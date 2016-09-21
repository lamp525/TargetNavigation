using MB.DAL;
using System.Threading.Tasks;

namespace MB.New.BLL.Tag
{
    public interface ITagManagementBLL
    {
        #region 取得用户常用标签

        /// <summary>
        /// 取得用户常用标签
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string[] GetMostUsedTag(int userId);

        #endregion 取得用户常用标签

        #region 一般计划标签处理

        /// <summary>
        /// 保存一般计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        Task SavePlanTagAsync(TargetNavigationDBEntities db, int userId, int planId);

        /// <summary>
        /// 移除一般计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        Task RemovePlanTagAsync(TargetNavigationDBEntities db, int userId, int planId);

        #endregion 一般计划标签处理

        #region 循环计划标签处理

        /// <summary>
        /// 移除循环计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        Task RemoveLoopPlanTagAsync(TargetNavigationDBEntities db, int userId, int loopId);

        /// <summary>
        /// 保存循环计划标签
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="loopId"></param>
        Task SaveLoopPlanTagAsync(TargetNavigationDBEntities db, int userId, int loopId);

        #endregion 循环计划标签处理
    }
}