using MB.DAL;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.New.BLL.Station
{
    public interface IStationBLL
    {
        /// <summary>
        /// 根据用户ID取得职位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<StationInfoModel> GetStationInfoByUserId(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 根据岗位ID取得上级岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="level">上级层数</param>
        /// <returns></returns>
        List<StationInfoModel> GetParentStationByStationId(TargetNavigationDBEntities db, int stationId, int level = 99);

        /// <summary>
        /// 根据岗位ID取得下级岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="level">下级层数</param>
        /// <returns></returns>
        List<StationInfoModel> GetSubStationByStationId(TargetNavigationDBEntities db, int stationId, int level = 99);

        /// <summary>
        /// 根据部门ID取得岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        List<StationInfoModel> GetStationInfoByOrgId(TargetNavigationDBEntities db, int orgId);
    }
}