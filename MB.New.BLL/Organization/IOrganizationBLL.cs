using MB.DAL;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.New.BLL.Organization
{
    public interface IOrganizationBLL
    {
        /// <summary>
        /// 根据部门ID取得上级部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <param name="level">上级层数</param>
        /// <returns></returns>
        List<OrganizationInfoModel> GetParentOrgByOrgId(TargetNavigationDBEntities db, int orgId, int level = 99);

        /// <summary>
        /// 根据岗位ID取得上级部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="level">上级层数</param>
        /// <returns></returns>
        List<OrganizationInfoModel> GetParentOrgByStationId(TargetNavigationDBEntities db, int stationId, int level = 99);

        /// <summary>
        /// 根据部门ID取得下级部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <param name="level">下级层数</param>
        /// <returns></returns>
        List<OrganizationInfoModel> GetSubOrgByOrgId(TargetNavigationDBEntities db, int orgId, int level = 99);

        /// <summary>
        /// 根据岗位ID取得部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        OrganizationInfoModel GetOrgInfoByStationId(TargetNavigationDBEntities db, int stationId);
    }
}