using MB.DAL;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Station
{
    public class StationBLL : IStationBLL
    {
        /// <summary>
        /// 根据用户ID取得岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<StationInfoModel> GetStationInfoByUserId(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<StationInfoModel>();

            result = (from s in db.tblStation
                      join us in db.tblUserStation
                      on s.stationId equals us.stationId
                      where us.userId == userId && s.deleteFlag != true
                      select new StationInfoModel
                      {
                          stationId = s.stationId,
                          parentStation = s.parentStation,
                          stationName = s.stationName,
                          orgId = s.organizationId
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得上级岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="level">上级层数</param>
        /// <returns></returns>
        public List<StationInfoModel> GetParentStationByStationId(TargetNavigationDBEntities db, int stationId, int level = 99)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.fGetParentStationByStationId(stationId, level).Select(x => new StationInfoModel
            {
                stationId = x.stationId,
                stationName = x.stationName,
                parentStation = x.parentStation,
                orgId = x.organizationId,
                level = x.lv
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得下级岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="level">下级层数</param>
        /// <returns></returns>
        public List<StationInfoModel> GetSubStationByStationId(TargetNavigationDBEntities db, int stationId, int level = 99)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = db.fGetSubStationByStationId(stationId, level).Select(x => new StationInfoModel
            {
                stationId = x.stationId,
                stationName = x.stationName,
                parentStation = x.parentStation,
                orgId = x.organizationId,
                level = x.lv
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据部门ID取得岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public List<StationInfoModel> GetStationInfoByOrgId(TargetNavigationDBEntities db, int orgId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = db.tblStation.Where(x => x.organizationId == orgId && x.deleteFlag != true)
                .Select(x => new StationInfoModel
                {
                    stationId = x.stationId,
                    stationName = x.stationName,
                    parentStation = x.parentStation,
                    orgId = x.organizationId
                }).ToList();

            return result;
        }
    }
}