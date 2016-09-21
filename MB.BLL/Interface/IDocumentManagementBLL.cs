using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IDocumentManagementBLL
    {
        /// 获取公司文档列表
        List<DocumentModel> GetCompanyDocumentList(string condition, Sort sort);

        /// 获取权限数据
        AuthorityInfoModel GetAuthorityList(int documentId);

        /// 设置权限
        void SetAuthority(int[] deleteAuthorityIds, AuthorityInfoModel authorityInfo, int userId);

        /// 拼接部门信息
        string[] GetOrgInfoById(int[] orgIds);

        /// 拼接岗位信息
        string[] GetStationInfoById(int[] stationIds);
    }
}