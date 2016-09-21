using System;
using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IRosterBLL
    {
        /// 用户详细信息取得
        RosterModel GetRosterInfo(int userId);

        /// 用户列表取得(用户)
        List<RosterInfo> GetRosterList(string userName = null);

        /// 用户列表取得(组织)
        List<RosterInfo> GetRosterOrgList(int currentPage, int orgId, string userName = null);

        /// 更新密码
        string UpdatePassWord(int userId);

        /// 根据银行卡号查找银行名称
        data GetBankName(string bankNum);

        /// 根据身份证获取省份、城市、地区
        List<data> GetTownByidentityCard(string identityCard);

        /// 保存用户信息
        int SaveRosterInfo(RosterModel userModel, int LoginUser);

        /// 根据用户id更新状态
        void UpdateWorkStatusById(int userId, int workStatus, DateTime validDate);

        /// 删除用户
        void DeleteUser(int userId);

        /// 导出用户信息
        List<RosterModel> ExportFile(int orgId, string userName = null);

        /// <summary>
        /// 根据工号验证工号重复
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        bool UserNumBerIsHave(string num);
    }
}