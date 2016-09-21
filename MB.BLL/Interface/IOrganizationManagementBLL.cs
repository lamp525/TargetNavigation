using System.Collections.Generic;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface IOrganizationManagementBLL
    {
        /// 获取组织架构列表(含模糊查询)
        List<OrgModel> GetOrganizationList(string name, int? parentId);

        /// 获取组织架构详情
        OrgModel GetOrganizationInfo(int organizationId);

        /// 新建/更新组织架构
        void SaveOrganization(OrgModel orgInfo, int userId);

        /// 根据上级组织Id获取下级的组织列表
        List<OrgModel> GetOrgListById(int? orgId, int? organizationId);

        /// 删除组织架构
        string DeleteOrganization(int[] organizationId);

        /// 组织架构排序
        void OrderOrganization(List<OrgModel> orderList);

        /// 获取所有下级岗位
        List<StationModel> GetDownStation(int Stationid, ref List<StationModel> StationList, TargetNavigationDBEntities db);

        List<OrgModel> GetDownOrg(int orgid, ref List<OrgModel> orgList, TargetNavigationDBEntities db);

        /// 获取岗位列表
        List<StationModel> GetStationList(string name, int? organizationId, int? stationId);

        //获取当前岗位下人员
        List<UserInfo> GetUserList(int stationid);

        List<UserInfo> GetAllUser();

        /// 获取岗位详情
        StationModel GetStationInfo(int id);

        /// 删除岗位
        string DeleteStation(int[] id);

        /// 新建/更新岗位
        int SaveStation(StationModel stationInfo, int userId);

        /// 添加岗位人员
        void AddUser(int stationId, int[] addUser);

        /// 添加岗位手册
        void AddStationManual(int[] deleteId, List<LoopPlanInfo> loopPlanList, int userId, int stationid);

        /// 获取岗位手册列表
        List<LoopPlanInfo> GetStationManual(int stationId);
    }
}