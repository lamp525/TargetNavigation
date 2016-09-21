using MB.DAL;
using MB.New.Model;

namespace MB.New.BLL.Incentive
{
    public interface IIncentiveBLL
    {
        /// <summary>
        /// 取得用户周激励信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="weekOfYear">一年中的的几周</param>
        /// <returns></returns>
        UserIndexIncentiveInfoModel GetUserIncentiveByWeek(TargetNavigationDBEntities db, int userId, int weekOfYear);

        /// <summary>
        /// 取得用户月激励信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="yearMonth">格式为【yyyyMM】</param></param>
        /// <returns></returns>
        UserIndexIncentiveInfoModel GetUserIncentiveByMonth(TargetNavigationDBEntities db, int userId, string yearMonth);
    }
}