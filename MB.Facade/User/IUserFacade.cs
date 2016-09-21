using MB.New.Common;
using System.Collections.Generic;

namespace MB.Facade.User
{
    public interface IUserFacade
    {
        /// <summary>
        /// 获取头部个人用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PageUserInfoSimpleModel GetHeadUserInfo(int userId);

        /// <summary>
        /// 取得用户部门信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<PageUserOrgInfoModel> GetUserOrgInfo(int userId);

        /// <summary>
        /// 取得常用联系人信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<PageUserInfoSimpleModel> GetTopContacts(int userId, EnumDefine.TopContactsType type);

        /// <summary>
        /// 人员模糊检索
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        List<PageUserInfoSimpleModel> UserNameFuzzySearch(string userName);

        /// <summary>
        /// 取得用户原头像信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>用户原头像地址</returns>
        PageUserHeadImageModel GetUserOriginalImage(int userId);

        /// <summary>
        /// 保存用户头像信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imageInfo"></param>
        /// <returns>头像地址</returns>
        string SaveUserHead(int userId, PageUserHeadImageModel imageInfo);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="passwordInfo"></param>
        EnumDefine.ChangePasswordResult ChangePassword(PageUserPasswordModel passwordInfo);

        /// <summary>
        ///  取得用户岗位（带部门）信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<PageUserInfoSimpleModel> GetUserOrgStationInfo(int userId);

        /// <summary>
        /// 更新用户默认岗位信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="stationId"></param>
        void UpdUserDefaultStation(int userId, int stationId);
    }
}