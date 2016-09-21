using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.New.BLL.User
{
    public interface IUserBLL
    {
        #region 用户信息添加、更新、删除

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        int InsUserInfo(TargetNavigationDBEntities db, UserBaseInfoModel userInfo);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        void UpdUserInfo(TargetNavigationDBEntities db, UserBaseInfoModel userInfo);

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        void DelUserInfo(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 更新用户头像信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        void UpdUserHeadImageInfo(TargetNavigationDBEntities db, UserBaseInfoModel userInfo);

        /// <summary>
        /// 更新用户默认岗位
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        void UpdUserDefaultStation(TargetNavigationDBEntities db, UserBaseInfoModel userInfo);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        EnumDefine.ChangePasswordResult ChangePassword(TargetNavigationDBEntities db, int userId, string oldPwd, string newPwd);

        #endregion 用户信息添加、更新、删除

        #region 人员检索处理

        /// <summary>
        /// 人员模糊查询
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> UserNameFuzzySearch(TargetNavigationDBEntities db, string userName);

        #endregion 人员检索处理

        #region 根据用户名的处理

        /// <summary>
        /// 取得用户详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetUserInfoByName(TargetNavigationDBEntities db, string userName);

        #endregion 根据用户名的处理

        #region 根据用户ID的处理

        /// <summary>
        /// 更新用户错误密码次数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="isReset"></param>
        void UpdUserWrongPwdNum(TargetNavigationDBEntities db, int userId, bool isReset);

        /// <summary>
        /// 设置用户的默认岗位
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        void SetUserDefaultStation(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 取得用户详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserBaseInfoModel GetUserInfoById(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 取得用户的默认信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserInfoSimpleModel GetUserDefaultInfo(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 取得用户岗位部门信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserInfoSimpleModel> GetUserOrgStationInfoByUser(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 取得用户部门岗位信息（含上级部门信息）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserInfoSimpleModel> GetAllUserOrgStationInfoByUser(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 根据用户ID取得所有下属用户信息（递归）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetAllSubordinateInfoByUserId(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 根据用户ID取得直属下属用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetSubordinateInfoByUserId(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 根据用户ID取得所有上级用户信息（递归）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetAllSuperiorInfoByUserId(TargetNavigationDBEntities db, int userId);

        /// <summary>
        /// 根据用户ID取得直属上级用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetSuperiorInfoByUserId(TargetNavigationDBEntities db, int userId);

        #endregion 根据用户ID的处理

        #region 根据岗位ID的处理

        /// <summary>
        /// 根据岗位ID取得人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetUserInfoByStationId(TargetNavigationDBEntities db, int stationId);

        /// <summary>
        /// 根据岗位ID取得直属下级人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetSubordinateInfoByStationId(TargetNavigationDBEntities db, int stationId);

        /// <summary>
        /// 根据岗位ID取得所有下级人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetAllSubordinateInfoByStationId(TargetNavigationDBEntities db, int stationId);

        /// <summary>
        /// 根据岗位ID取得所有上级用户信息（递归）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetAllSuperiorInfoByStationId(TargetNavigationDBEntities db, int stationId);

        /// <summary>
        /// 根据岗位ID取得直属上级用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetSuperiorInfoByStationId(TargetNavigationDBEntities db, int stationId);

        #endregion 根据岗位ID的处理

        #region 根据部门ID的处理

        /// <summary>
        /// 根据部门ID取得人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetUserInfoByOrgId(TargetNavigationDBEntities db, int organizationId);

        /// <summary>
        /// 根据部门ID取得人员信息（递归取得所有下属）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserBaseInfoModel> GetUserInfoByOrgIdWithSub(TargetNavigationDBEntities db, int organizationId);

        #endregion 根据部门ID的处理
    }
}