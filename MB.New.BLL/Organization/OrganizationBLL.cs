using MB.DAL;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Organization
{
    public class OrganizationBLL : IOrganizationBLL
    {
        /// <summary>
        /// 根据部门ID取得上级部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <param name="level">上级层数</param>
        /// <returns></returns>
        public List<OrganizationInfoModel> GetParentOrgByOrgId(TargetNavigationDBEntities db, int orgId, int level = 99)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.fGetParentOrgByOrgId(orgId, level).Select(x => new OrganizationInfoModel
            {
                organizationId = x.organizationId,
                organizationName = x.organizationName,
                parentOrganization = x.parentOrganization,
                level = x.lv
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得上级部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="level">上级层数</param>
        /// <returns></returns>
        public List<OrganizationInfoModel> GetParentOrgByStationId(TargetNavigationDBEntities db, int stationId, int level = 99)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.fGetParentOrgByStationId(stationId, level).Select(x => new OrganizationInfoModel
            {
                organizationId = x.organizationId,
                organizationName = x.organizationName,
                parentOrganization = x.parentOrganization,
                level = x.lv
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据部门ID取得下级部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <param name="level">下级层数</param>
        /// <returns></returns>
        public List<OrganizationInfoModel> GetSubOrgByOrgId(TargetNavigationDBEntities db, int orgId, int level = 99)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.fGetSubOrgByOrgId(orgId, level).Select(x => new OrganizationInfoModel
            {
                organizationId = x.organizationId,
                organizationName = x.organizationName,
                parentOrganization = x.parentOrganization,
                level = x.lv
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public OrganizationInfoModel GetOrgInfoByStationId(TargetNavigationDBEntities db, int stationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = (from o in db.tblOrganization
                          join s in db.tblStation
                          on o.organizationId equals s.organizationId
                          where s.stationId == stationId
                          select new OrganizationInfoModel
                          {
                              organizationId = o.organizationId,
                              organizationName = o.organizationName,
                              parentOrganization = o.parentOrganization
                          }).FirstOrDefault();

            return result;
        }
    }
}