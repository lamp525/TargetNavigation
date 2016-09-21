using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class UserBLL : IUserBLL
    {
        public UserInfo UserLogin(string userName)
        {
            TargetNavigationDBEntities db = new TargetNavigationDBEntities();
            var user = new UserInfo();
            user = (from User in db.tblUser
                    where User.userName == userName
                    select new UserInfo
                    {
                        userId = User.userId,
                        workStatus = User.workStatus,
                        deleteFlag = User.deleteFlag,
                        password = User.password,
                        randomKey = User.randomKey,
                        errorPass = User.errorPassword,
                        admin = User.admin.HasValue ? User.admin.Value : false,
                        execution = User.execution.HasValue ? User.execution.Value : false,
                    }).FirstOrDefault();

            return user;
        }

        public void UpUserLoginWorryNum(bool flag, string userName)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var operId = db.prcGetPrimaryKey("tblCanlendar", obj).FirstOrDefault().Value;
                tblUser firstData = getUserByNamee(userName, db);
                if (flag == false)
                {
                    firstData.errorPassword = firstData.errorPassword + 1;
                }
                else
                {
                    firstData.errorPassword = 0;
                }
                db.SaveChanges();
            }
        }

        public UserInfo GetUserById(int Userid)
        {
            TargetNavigationDBEntities db = new TargetNavigationDBEntities();
            var user = new UserInfo();
            user = (from User in db.tblUser
                    join Us in db.tblUserStation on User.userId equals Us.userId
                    join s in db.tblStation on Us.stationId equals s.stationId
                    where User.userId == Userid
                    select new UserInfo
                    {
                        userId = User.userId,
                        workStatus = User.workStatus,
                        deleteFlag = User.deleteFlag,
                        password = User.password,
                        randomKey = User.randomKey,
                        bigImage = User.bigImage,
                        midImage = User.midImage,
                        smallImage = User.smallImage,
                        userName = User.userName,
                        stationName = s.stationName
                    }).FirstOrDefault();

            return user;
        }

        public void UpdateCal(Calendar calendar)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var operId = db.prcGetPrimaryKey("tblCanlendar", obj).FirstOrDefault().Value;
                tblCalendar firstData = getTblCalendarInfoByCId(calendar.calendarId.Value, db);
                firstData.updateUser = calendar.UserId;
                firstData.updateTime = DateTime.Now;
                firstData.startTime = calendar.startTime;
                firstData.endTime = calendar.endTime;
                firstData.comment = calendar.comment;
                firstData.place = calendar.place;
                db.SaveChanges();

                List<tblCalendarUser> results = (from r in db.tblCalendarUser where r.calendarId == calendar.calendarId select r).ToList();
                foreach (var item in results)
                {
                    db.tblCalendarUser.Remove(item);
                }
                db.SaveChanges();
                if (calendar.partner != null)
                {
                    foreach (var item in calendar.partner)
                    {
                        var newClaendarUser = new tblCalendarUser
                        {
                            calendarId = int.Parse(calendar.calendarId.ToString()),
                            userId = item
                        };
                        db.tblCalendarUser.Add(newClaendarUser);
                    }
                }
                //firstData.progress = newProcess;
                //firstData.updateUser = userId;
                //firstData.updateTime = DateTime.Now;
                //var opera = new PlanOperateInfo
                //{
                //    operateId = operId,
                //    planId = planId,
                //    result = 16,
                //    message = "",
                //    reviewUser = userId,
                //    reviewTime = DateTime.Now
                //};
                //AddPlanOperate(opera, db);
            }
        }

        #region 根据用户ID和时间获取日程信息

        /// <summary>
        /// 根据用户ID和时间获取日程信息
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="strtime">时间</param>
        /// <returns></returns>
        public List<Calendar> getCanlendarByUserTime(int userid, string strtime)
        {
            List<Calendar> list = new List<Calendar>();
            DateTime time;
            DateTime startime = DateTime.Parse(strtime).AddHours(23);
            if (DateTime.TryParse(strtime, out time))
            {
                using (var db = new TargetNavigationDBEntities())
                {
                    list = (from cal in db.tblCalendar
                            // join caluser in db.tblCalendarUser on cal.calendarId equals caluser.calendarId
                            where cal.createUser == userid && cal.startTime >= time && cal.endTime <= startime
                            select new Calendar
                            {
                                calendarId = cal.calendarId,
                                startTime = cal.startTime,
                                endTime = cal.endTime,
                                place = cal.place,
                                comment = cal.comment
                                ////tag = getTag(cal.calendarId)
                                //tag = new List<string> { "sss","222"}
                            }).ToList();
                    foreach (var li in list)
                    {
                        li.tag = string.Join(",", getTag(li.calendarId.Value));
                        li.partner = GetUserListIDByCId(li.calendarId.Value);
                        if (li.partner != null)
                        {
                            li.partnerName = getUser(li.partner);
                        }
                        li.FendTime = li.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                        li.FstartTime = li.startTime.Value.ToString("yyyy-MM-dd HH:mm");
                    }
                    return list;
                }
            }
            else
            {
                return null;
            }
        }

        public List<Calendar> getCanlendarByUserendTime(int userid, DateTime strtime, DateTime endtime)
        {
            List<Calendar> list = new List<Calendar>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from cal in db.tblCalendar 
                        // join caluser in db.tblCalendarUser on cal.calendarId equals caluser.calendarId
                        where cal.createUser == userid && cal.startTime >= strtime && cal.endTime <= endtime
                        select new Calendar
                        {
                            calendarId = cal.calendarId,
                            startTime = cal.startTime,
                            endTime = cal.endTime,
                            place = cal.place,
                            comment = cal.comment
                            ////tag = getTag(cal.calendarId)
                            //tag = new List<string> { "sss","222"}
                        }).ToList();
                foreach (var li in list)
                {
                    li.tag = string.Join(",", getTag(li.calendarId.Value));
                    li.partner = GetUserListIDByCId(li.calendarId.Value);
                    if (li.partner != null)
                    {
                        li.partnerName = getUser(li.partner);
                    }
                    li.FendTime = li.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                    li.FstartTime = li.startTime.Value.ToString("yyyy-MM-dd HH:mm");
                }
                return list;
            }
        }

        #endregion 根据用户ID和时间获取日程信息

        /// <summary>
        /// 根据日程ID获取日程标签
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        private List<string> getTag(int calendarId)
        {
            //List<Tab> listTab = new List<Tab>();
            List<string> strList = new List<string>();
            //using (var db = new TargetNavigationDBEntities())
            //{
            //    listTab = (from tab in db.tblTab
            //               join calTab in db.tblCalendarTab on tab.tabId equals calTab.tabId
            //               where calTab.calendarId == calendarId
            //               select new Tab
            //               {
            //                   tabId = tab.tabId,
            //                   tabName = tab.tabName,
            //                   num = tab.num
            //               }).ToList();
            //    foreach (var itme in listTab)
            //    {
            //        strList.Add(itme.tabName);
            //    }

            //}

            return strList;
        }

        public List<Calendar> getCalendarByCId(int Cid)
        {
            List<Calendar> CalendarList = new List<Calendar>();
            using (var db = new TargetNavigationDBEntities())
            {
                CalendarList = (from cList in db.tblCalendar
                                where cList.calendarId == Cid
                                select new Calendar
                                {
                                    calendarId = cList.calendarId,
                                    startTime = cList.startTime,
                                    endTime = cList.endTime,
                                    place = cList.place,
                                    comment = cList.comment
                                }).ToList();
                foreach (var li in CalendarList)
                {
                    li.tag = string.Join(",", getTag(li.calendarId.Value));
                    li.partner = GetUserListIDByCId(li.calendarId.Value);
                    if (li.partner != null)
                    {
                        li.partnerName = getUser(li.partner);
                    }
                    li.FendTime = li.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                    li.FstartTime = li.startTime.Value.ToString("yyyy-MM-dd HH:mm");
                }
                return CalendarList;
            }
        }

        public int DeleteCalendarById(int Cid)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblCalendar.Where(p => p.calendarId == Cid).FirstOrDefault();
                if (model != null)
                {
                    db.tblCalendar.Remove(model);
                }
            }
            return 1;
        }


        public tblCalendar getTblCalendarInfoByCId(int Cid, TargetNavigationDBEntities db)
        {
            return db.tblCalendar.Where(a => a.calendarId == Cid).FirstOrDefault<tblCalendar>();
        }

        public tblUser getUserByNamee(string Name, TargetNavigationDBEntities db)
        {
            return db.tblUser.Where(a => a.userName == Name).FirstOrDefault<tblUser>();
        }

        public Calendar getCalendarInfoByCId(int Cid)
        {
            Calendar CalendarList = new Calendar();
            using (var db = new TargetNavigationDBEntities())
            {
                CalendarList = (from cList in db.tblCalendar
                                where cList.calendarId == Cid
                                select new Calendar
                                {
                                    calendarId = cList.calendarId,
                                    startTime = cList.startTime,
                                    endTime = cList.endTime,
                                    place = cList.place,
                                    comment = cList.comment
                                }).FirstOrDefault();

                CalendarList.tag = string.Join(",", getTag(CalendarList.calendarId.Value));
                CalendarList.partner = GetUserListIDByCId(CalendarList.calendarId.Value);
                if (CalendarList.partner != null)
                {
                    CalendarList.partnerName = getUser(CalendarList.partner);
                }
                CalendarList.FendTime = CalendarList.endTime.Value.ToString("yyyy-MM-dd HH:mm");
                CalendarList.FstartTime = CalendarList.startTime.Value.ToString("yyyy-MM-dd HH:mm");

                return CalendarList;
            }
        }

        private List<string> getUser(List<int> Userid)
        {
            List<UserInfo> listUser = new List<UserInfo>();
            List<string> strList = new List<string>();
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var id in Userid)
                {
                    listUser = (from user in db.tblUser
                                where user.userId == id
                                select new UserInfo
                                {
                                    userId = user.userId,
                                    userName = user.userName
                                }).ToList();
                    foreach (var itme in listUser)
                    {
                        strList.Add(itme.userName);
                    }
                }
                return strList;
            }
        }

        private List<int> GetUserListIDByCId(int calendarId)
        {
            List<int> PUserId = new List<int>();
            List<UserInfo> userlist = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userlist = (from user in db.tblCalendarUser
                            join calendar in db.tblCalendar on user.calendarId equals calendar.calendarId
                            where calendar.calendarId == calendarId
                            select new UserInfo
                            {
                                userId = user.userId
                            }).ToList();
            }
            foreach (var item in userlist)
            {
                PUserId.Add(item.userId);
            }
            return PUserId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="calendar"></param>
        public void AddCalendarId(Calendar calendar)
        {
            CommonWorkTime commonWORK = new CommonWorkTime();
            var db = new TargetNavigationDBEntities();
            var newCalendar = new tblCalendar
            {
                calendarId = commonWORK.GetPlanidByTblName("tblCalendar"),
                comment = calendar.comment,
                endTime = calendar.endTime,
                startTime = calendar.startTime,
                place = calendar.place,
                createTime = DateTime.Now,
                createUser = calendar.UserId,
                updateTime = DateTime.Now,
                updateUser = calendar.UserId
            };
            db.tblCalendar.Add(newCalendar);
            db.SaveChanges();
            // db.SaveChanges();
            if (calendar.partner != null)
            {
                foreach (var item in calendar.partner)
                {
                    var newClaendarUser = new tblCalendarUser
                    {
                        calendarId = newCalendar.calendarId,
                        userId = item
                    };
                    db.tblCalendarUser.Add(newClaendarUser);
                }
            }

            db.SaveChanges();
        }

        #region 根据用户ID更改密码  暂时在方法里写了 验证，后期更改

        /// <summary>
        /// 根据用户ID更改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="pwd">密码</param>
        public int UpUserPwd(int userId, string oldpwd, string pwd)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                tblUser user = db.tblUser.Where(u => u.userId == userId).FirstOrDefault<tblUser>();
                if (user != null)
                {
                    string eqkey = user.randomKey;
                    string eqoldpwd = Common.EncryptHelper.PwdEncrypt(oldpwd, eqkey);
                    if (user.password.Equals(eqoldpwd))
                    {
                        string key = Common.StringUtils.GetNumcharRandom(8);
                        string strPwd = Common.EncryptHelper.PwdEncrypt(pwd, key);
                        user.password = strPwd;
                        user.randomKey = key;
                        db.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 2;
                }
            }
        }

        #endregion 根据用户ID更改密码  暂时在方法里写了 验证，后期更改

        #region 根据用户ID更新用户设定

        /// <summary>
        /// 根据用户ID更新用户设定
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pagesize">站内信刷新时间</param>
        /// <param name="refreshTime">每页显示数量</param>
        /// <returns></returns>
        public bool UpPersonalSetting(int userId, int pagesize/*站内信刷新时间*/, int refreshTime/*每页显示数量*/)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                tblPersonalSetting personalSetting = db.tblPersonalSetting.Where(p => p.userId == userId).FirstOrDefault<tblPersonalSetting>();
                if (personalSetting != null)
                {
                    personalSetting.pageSize = pagesize;
                    personalSetting.refreshTime = refreshTime;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion 根据用户ID更新用户设定

        #region 根据用户ID获取个人设定信息

        /// <summary>
        /// 根据用户ID获取个人设定信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public PersonalSetting GetPersonalSetting(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                tblUser user = db.tblUser.Where(u => u.userId == userId).FirstOrDefault<tblUser>();
                tblPersonalSetting tbl = db.tblPersonalSetting.Where(p => p.userId == userId).FirstOrDefault<tblPersonalSetting>();
                if (tbl == null)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var operId = db.prcGetPrimaryKey("tblPersonalSetting", obj).FirstOrDefault().Value;
                    var tblP = new tblPersonalSetting
                    {
                        settingId = operId,
                        userId = user.userId,
                        refreshTime = null,
                        pageSize = null,
                        //createUser = user.userId,
                        //createTime = DateTime.Now,
                        //updateTime = DateTime.Now,
                        //updateUser = user.userId
                    };
                    db.tblPersonalSetting.Add(tblP);
                    string path = "/Images/common/portrait.png";
                    var personalSetting = new PersonalSetting
                    {
                        settingId = tblP.settingId,
                        userId = tblP.userId,
                        refreshTime = tblP.refreshTime,
                        pageSize = tblP.pageSize,
                        //createUser = tblP.createUser,
                        //createTime = tblP.createTime,
                        //updateUser = tblP.updateUser,
                        //updateTime = tblP.updateTime,
                        bigImage = path,
                        midImage = path,
                        smallImage = path
                    };
                    db.SaveChanges();
                    return personalSetting;
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.originalImage))
                    {
                        FileUpload file = new FileUpload();
                        string path = file.ConfigPath(Convert.ToInt32(UploadFilePath.HeadImage));
                        var personalSetting = new PersonalSetting
                        {
                            settingId = tbl.settingId,
                            userId = tbl.userId,
                            refreshTime = tbl.refreshTime,
                            pageSize = tbl.pageSize,
                            //createUser = tbl.createUser,
                            //createTime = tbl.createTime,
                            //updateUser = tbl.updateUser,
                            //updateTime = tbl.updateTime,
                            bigImage = "/" + path + "/" + user.bigImage,
                            midImage = "/" + path + "/" + user.midImage,
                            smallImage = "/" + path + "/" + user.smallImage
                        };
                        return personalSetting;
                    }
                    else
                    {
                        string path = "/Images/common/portrait.png";
                        var personalSetting = new PersonalSetting
                        {
                            settingId = tbl.settingId,
                            userId = tbl.userId,
                            refreshTime = tbl.refreshTime,
                            pageSize = tbl.pageSize,
                            //createUser = tbl.createUser,
                            //createTime = tbl.createTime,
                            //updateUser = tbl.updateUser,
                            //updateTime = tbl.updateTime,
                            bigImage = path,
                            midImage = path,
                            smallImage = path
                        };
                        return personalSetting;
                    }
                }
            }
        }

        #endregion 根据用户ID获取个人设定信息

        #region 头像上传

        /// <summary>
        /// 头像上传
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="originalImage">上传后原图名称</param>
        /// <param name="bigImagefilename">上传后大图名称</param>
        /// <param name="midImagefilename">上传后中图名称</param>
        /// <param name="smallImagefilename">上传后小图名称</param>
        public void SaveImg(int userId, string originalImage, string bigImagefilename, string midImagefilename, string smallImagefilename, string imgimagePosition)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                tblUser tUser = db.tblUser.Where(u => u.userId == userId).FirstOrDefault<tblUser>();
                tUser.originalImage = originalImage;
                tUser.bigImage = bigImagefilename;
                tUser.midImage = midImagefilename;
                tUser.smallImage = smallImagefilename;
                tUser.imagePosition = imgimagePosition;
                db.SaveChanges();
            }
        }

        #endregion 头像上传

        #region 返回用户头像路径

        /// <summary>
        /// 返回用户头像路径 和 坐标
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public UserInfo GetUserHeadImg(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                tblUser user = db.tblUser.Where(u => u.userId == userId).FirstOrDefault<tblUser>();
                var userInfo = new UserInfo
                {
                    userId = user.userId,
                    bigImage = user.bigImage,
                    smallImage = user.smallImage,
                    OriginalImage = user.originalImage,
                    ImagePosition = user.imagePosition
                };
                return userInfo;
            }
        }

        #endregion 返回用户头像路径

        #region 删除IM登录信息

        /// <summary>
        /// 删除用户SeesionID
        /// </summary>
        /// <param name="sessionId">sessionId</param>
        public void DeleteUserSessionID(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImLogin.Where(p => p.userId == userId).ToList();

                if (model != null)
                {
                    db.tblImLogin.RemoveRange(model);
                    db.SaveChanges();
                }
            }
        }

        #endregion 删除IM登录信息
    }
}