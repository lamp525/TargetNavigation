using System;
using System.Collections.Generic;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface IUserBLL
    {
        UserInfo UserLogin(string userName);

        void UpUserLoginWorryNum(bool Falg, string UserName);

        UserInfo GetUserById(int Userid);

        void UpdateCal(Calendar calendar);

        /// 根据用户ID和时间获取日程信息
        List<Calendar> getCanlendarByUserTime(int userid, string strtime);

        List<Calendar> getCanlendarByUserendTime(int userid, DateTime strtime, DateTime endtime);

        List<Calendar> getCalendarByCId(int Cid);

        tblCalendar getTblCalendarInfoByCId(int Cid, TargetNavigationDBEntities db);

        tblUser getUserByNamee(string Name, TargetNavigationDBEntities db);

        Calendar getCalendarInfoByCId(int Cid);

        void AddCalendarId(Calendar calendar);

        /// 根据用户ID更改密码
        int UpUserPwd(int userId, string oldpwd, string pwd);

        /// 根据用户ID更新用户设定
        bool UpPersonalSetting(int userId, int pagesize/*站内信刷新时间*/, int refreshTime/*每页显示数量*/);

        /// 根据用户ID获取个人设定信息
        PersonalSetting GetPersonalSetting(int userId);

        /// 头像上传
        void SaveImg(int userId, string originalImage, string bigImagefilename, string midImagefilename, string smallImagefilename, string imgimagePosition);

        /// 返回用户头像路径 和 坐标
        UserInfo GetUserHeadImg(int userId);

        /// 删除用户SeesionID
        void DeleteUserSessionID(int userId);
    }
}