using MB.DAL;
using MB.New.BLL.Organization;
using MB.New.BLL.Station;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.User
{
    public class UserBLL : IUserBLL
    {
        #region 用户信息添加、更新

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        public EnumDefine.ChangePasswordResult ChangePassword(TargetNavigationDBEntities db, int userId, string oldPwd, string newPwd)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var info = db.tblUser.Where(u => u.userId == userId).FirstOrDefault();
            if (info == null)
            {
                return EnumDefine.ChangePasswordResult.Failed;
            }
            string oldEncryptPwd = EncryptHelper.PwdEncrypt(oldPwd, info.randomKey);
            if (!info.password.Equals(oldEncryptPwd))
            {
                return EnumDefine.ChangePasswordResult.PwdError;
            }
            string key = StringUtility.GetNumcharRandom(8);
            string newEncryptPwd = EncryptHelper.PwdEncrypt(newPwd, key);
            info.password = newEncryptPwd;
            info.randomKey = key;

            return EnumDefine.ChangePasswordResult.Succeed;
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        public int InsUserInfo(TargetNavigationDBEntities db, UserBaseInfoModel userInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var userId = DBUtility.GetPrimaryKeyByTableName(db, "tblUser");
            var info = ModelMapping.JsonMapping<UserBaseInfoModel, tblUser>(userInfo);
            info.userId = userId;

            return userId;
        }

        /// <summary>
        /// 更新用户头像信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        public void UpdUserHeadImageInfo(TargetNavigationDBEntities db, UserBaseInfoModel userInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var info = db.tblUser.Where(u => u.userId == userInfo.userId).FirstOrDefault();
            if (info == null) return;

            info.originalImage = userInfo.originalImage;
            info.bigImage = userInfo.headImage;
            info.imagePosition = userInfo.imagePosition;
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        public void DelUserInfo(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var info = db.tblUser.Where(x => x.userId == userId).FirstOrDefault();
            if (info == null) return;
            info.deleteFlag = true;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        public void UpdUserInfo(TargetNavigationDBEntities db, UserBaseInfoModel userInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var info = db.tblUser.Where(x => x.userId == userInfo.userId).FirstOrDefault();
            if (info == null) return;
            info = ModelMapping.JsonMapping<UserBaseInfoModel, tblUser>(userInfo);
        }

        /// <summary>
        /// 更新用户默认岗位
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userInfo"></param>
        public void UpdUserDefaultStation(TargetNavigationDBEntities db, UserBaseInfoModel userInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var info = db.tblUser.Where(x => x.userId == userInfo.userId).FirstOrDefault();

            if (info == null) return;

            info.defaultStationId = userInfo.defaultStationId;
            info.updateTime = DateTime.Now;
            info.updateUser = userInfo.userId;
        }

        #endregion 用户信息添加、更新

        #region 根据用户名的处理

        /// <summary>
        /// 根据用户名取得登录用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetUserInfoByName(TargetNavigationDBEntities db, string userName)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.tblUser.Where(x => x.userName == userName && x.deleteFlag != true && x.workStatus == (int)EnumDefine.WorkStatus.OnWork)
                .Select(x => new UserBaseInfoModel
                {
                    userId = x.userId,
                    userName = userName,
                    password = x.password,
                    randomKey = x.randomKey,
                    errorPassword = x.errorPassword,
                    isAdmin = x.admin,
                    isExecution = x.execution
                }).ToList();

            return result;
        }

        #endregion 根据用户名的处理

        #region 根据用户ID的处理

        /// <summary>
        /// 更新用户错误密码次数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="isReset"></param>
        public void UpdUserWrongPwdNum(TargetNavigationDBEntities db, int userId, bool isReset)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var info = db.tblUser.Where(x => x.userId == userId).FirstOrDefault();

            if (info == null) return;

            if (isReset) info.errorPassword = 0;
            else info.errorPassword += 1;
        }

        /// <summary>
        /// 设置用户的默认岗位
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        public void SetUserDefaultStation(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            IStationBLL Station = new StationBLL();

            var stationInfo = Station.GetStationInfoByUserId(db, userId);

            if (stationInfo != null && stationInfo.Count > 0)
            {
                var userInfo = db.tblUser.Where(x => x.userId == userId).FirstOrDefault();
                if (userInfo != null) userInfo.defaultStationId = stationInfo.First().stationId;
            }
        }

        /// <summary>
        /// 根据用户ID取得所有下属用户信息（递归）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetAllSubordinateInfoByUserId(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = new List<UserBaseInfoModel>();

            IStationBLL stationBLL = new StationBLL();
            var stationInfo = stationBLL.GetStationInfoByUserId(db, userId);

            if (stationInfo == null || stationInfo.Count == 0) return result;

            var subordinateStationInfo = new List<StationInfoModel>();
            foreach (var item in stationInfo)
            {
                subordinateStationInfo.AddRange(stationBLL.GetSubStationByStationId(db, item.stationId.Value).Where(x => x.level > 0).ToList());
            }

            if (subordinateStationInfo.Count == 0) return result;
            var subordinateStationIds = subordinateStationInfo.Select(x => x.stationId).ToArray();
            //所有下属用户信息
            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      where subordinateStationIds.Contains(us.stationId) && u.deleteFlag != true && u.workStatus == (int)EnumDefine.WorkStatus.OnWork
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 根据用户ID取得所有上级用户信息（递归）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetAllSuperiorInfoByUserId(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = new List<UserBaseInfoModel>();

            IStationBLL stationBLL = new StationBLL();
            var stationInfo = stationBLL.GetStationInfoByUserId(db, userId);

            if (stationInfo == null || stationInfo.Count == 0) return result;

            var superiorStationInfo = new List<StationInfoModel>();
            foreach (var item in stationInfo)
            {
                superiorStationInfo.AddRange(stationBLL.GetParentStationByStationId(db, item.stationId.Value).Where(x => x.level > 0).ToList());
            }

            if (superiorStationInfo.Count == 0) return result;
            var superiorStationIds = superiorStationInfo.Select(x => x.stationId).ToArray();
            //所有下属用户信息
            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      where superiorStationIds.Contains(us.stationId)
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 取得用户部门岗位信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserInfoSimpleModel> GetUserOrgStationInfoByUser(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<UserInfoSimpleModel>();

            result = (from us in db.tblUserStation
                      join s in db.tblStation
                      on us.stationId equals s.stationId
                      join o in db.tblOrganization
                      on s.organizationId equals o.organizationId
                      where us.userId == userId && s.deleteFlag != true && o.deleteFlag != true
                      select new UserInfoSimpleModel
                      {
                          orgId = o.organizationId,
                          orgName = o.organizationName,
                          stationId = s.stationId,
                          stationName = s.stationName
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 取得用户部门岗位信息（含上级部门信息）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserInfoSimpleModel> GetAllUserOrgStationInfoByUser(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<UserInfoSimpleModel>();
            IStationBLL stationBLL = new StationBLL();
            //用户岗位信息
            var stationInfo = stationBLL.GetStationInfoByUserId(db, userId);
            if (stationInfo == null || stationInfo.Count == 0) return result;

            IOrganizationBLL orgBLL = new OrganizationBLL();
            foreach (var item in stationInfo)
            {
                //部门信息（含上级）
                var orgList = orgBLL.GetParentOrgByStationId(db, item.stationId.Value).OrderByDescending(x => x.level).ToList();

                if (orgList == null || orgList.Count == 0) continue;

                var model = new UserInfoSimpleModel
                {
                    userId = userId,
                    stationId = item.stationId,
                    stationName = item.stationName,
                    orgId = orgList.Last().organizationId,
                    orgInfo = StringUtility.ListToString(orgList.Select(x => x.organizationName).ToList(), "-")
                };

                result.Add(model);
            }

            return result;
        }

        /// <summary>
        /// 根据用户ID取得直属下属用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetSubordinateInfoByUserId(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            //直属下级岗位ID
            var subordinateStationId = (from s in db.tblStation
                                        join us in db.tblUserStation
                                        on s.parentStation equals us.stationId
                                        where us.userId == userId && s.deleteFlag != true
                                        select s.stationId
                                        ).ToArray();

            //直属下属用户信息
            var result = (from u in db.tblUser
                          join us in db.tblUserStation
                          on u.userId equals us.userId
                          where subordinateStationId.Contains(us.stationId)
                          select new UserBaseInfoModel
                          {
                              userId = u.userId,
                              userName = u.userName,
                              headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 根据用户ID取得直属上级用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetSuperiorInfoByUserId(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            //直属上级岗位ID
            var superiorStationId = (from s in db.tblStation
                                     join us in db.tblUserStation
                                     on s.stationId equals us.stationId
                                     where us.userId == userId && s.deleteFlag != true
                                     select s.parentStation
                                      ).ToArray();

            //直属上级用户信息
            var result = (from u in db.tblUser
                          join us in db.tblUserStation
                          on u.userId equals us.userId
                          where superiorStationId.Contains(us.stationId) && u.deleteFlag != true && u.workStatus == (int)EnumDefine.WorkStatus.OnWork
                          select new UserBaseInfoModel
                          {
                              userId = u.userId,
                              userName = u.userName,
                              headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 取得用户的默认信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfoSimpleModel GetUserDefaultInfo(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var userDefaultInfo = new UserInfoSimpleModel();
            var userInfo = GetUserInfoById(db, userId);

            if (userInfo != null)
            {
                //用户默认岗位不为空的场合
                if (userInfo.defaultStationId.HasValue)
                    userDefaultInfo = (from u in db.tblUser
                                       join s in db.tblStation
                                       on u.defaultStationId equals s.stationId
                                       join o in db.tblOrganization
                                       on s.organizationId equals o.organizationId
                                       where u.userId == userId
                                       select new UserInfoSimpleModel
                                       {
                                           userId = u.userId,
                                           userName = u.userName,
                                           headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                                           stationId = s.stationId,
                                           stationName = s.stationName,
                                           orgId = o.organizationId,
                                           orgName = o.organizationName
                                       }).FirstOrDefault();

                //用户默认岗位为空的场合
                else
                    userDefaultInfo = (from u in db.tblUser
                                       join us in db.tblUserStation
                                       on u.userId equals us.userId
                                       join s in db.tblStation
                                       on us.stationId equals s.stationId
                                       join o in db.tblOrganization
                                       on s.organizationId equals o.organizationId
                                       where u.userId == userId
                                       select new UserInfoSimpleModel
                                       {
                                           userId = u.userId,
                                           userName = u.userName,
                                           headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                                           stationId = s.stationId,
                                           stationName = s.stationName,
                                           orgId = o.organizationId,
                                           orgName = o.organizationName
                                       }).FirstOrDefault();
            }

            return userDefaultInfo;
        }

        /// <summary>
        /// 取得用户详情
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserBaseInfoModel GetUserInfoById(TargetNavigationDBEntities db, int userId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var condition = "userId == @0";
            var values = new object[] { userId };
            var userInfo = db.tblUser.Where(condition, values).Select(u => new UserBaseInfoModel
            {
                userId = u.userId,
                userNumber = u.userNumber,
                userName = u.userName,
                randomKey = u.randomKey,
                defaultStationId = u.defaultStationId,
                imagePosition = u.imagePosition,
                originalImage = u.originalImage,
                headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                password = u.password,
                sex = u.sex,
                workStatus = (EnumDefine.WorkStatus)u.workStatus.Value,
                quitTime = u.quitTime,
                errorPassword = u.errorPassword,
                deleteFlag = u.deleteFlag
            }).FirstOrDefault();
            return userInfo;
        }

        #endregion 根据用户ID的处理

        #region 根据岗位ID的处理

        /// <summary>
        /// 根据岗位ID取得所有下级人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetAllSubordinateInfoByStationId(TargetNavigationDBEntities db, int stationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<UserBaseInfoModel>();

            //取得所有下级岗位信息
            IStationBLL stationBLL = new StationBLL();
            var subordinateStationInfo = stationBLL.GetSubStationByStationId(db, stationId).Where(x => x.level > 0).ToList();
            if (subordinateStationInfo == null || subordinateStationInfo.Count == 0) return result;

            //所有下级人员信息
            var subordinateStationIds = subordinateStationInfo.Select(x => x.stationId).ToArray();
            //所有下属用户信息
            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      where subordinateStationIds.Contains(us.stationId)
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得直属下级人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetSubordinateInfoByStationId(TargetNavigationDBEntities db, int stationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            //直属下属用户信息
            var result = (from u in db.tblUser
                          join us in db.tblUserStation
                          on u.userId equals us.userId
                          join s in db.tblStation
                          on us.stationId equals s.stationId
                          where s.parentStation == stationId && s.deleteFlag != true && u.deleteFlag != true
                          select new UserBaseInfoModel
                          {
                              userId = u.userId,
                              userName = u.userName,
                              headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得所有上级用户信息（递归）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetAllSuperiorInfoByStationId(TargetNavigationDBEntities db, int stationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = new List<UserBaseInfoModel>();

            //所有上级用户岗位信息
            IStationBLL stationBLL = new StationBLL();
            var superiorStationInfo = stationBLL.GetParentStationByStationId(db, stationId).Where(x => x.level > 0).ToList();

            if (superiorStationInfo == null || superiorStationInfo.Count == 0) return result;

            var superiorStationId = superiorStationInfo.Select(x => x.stationId).ToArray();

            //所有上级用户信息
            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      where superiorStationId.Contains(us.stationId) && u.deleteFlag != true
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得直属上级用户信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetSuperiorInfoByStationId(TargetNavigationDBEntities db, int stationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<UserBaseInfoModel>();

            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      join s in db.tblStation
                      on us.stationId equals s.parentStation
                      where s.stationId == stationId && s.deleteFlag != true && u.deleteFlag != true
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                      }).ToList();

            return result;
        }

        /// <summary>
        /// 根据岗位ID取得人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetUserInfoByStationId(TargetNavigationDBEntities db, int stationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = new List<UserBaseInfoModel>();

            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      where us.stationId == stationId && u.deleteFlag != true
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                      }).ToList();

            return result;
        }

        #endregion 根据岗位ID的处理

        #region 根据部门ID的处理

        /// <summary>
        /// 根据部门ID取得人员信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetUserInfoByOrgId(TargetNavigationDBEntities db, int organizationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = (from u in db.tblUser
                          join us in db.tblUserStation
                          on u.userId equals us.userId
                          join s in db.tblStation
                          on us.stationId equals s.stationId
                          where s.organizationId == organizationId && s.deleteFlag != true && u.deleteFlag != true
                          select new UserBaseInfoModel
                          {
                              userId = u.userId,
                              userName = u.userName,
                              headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 根据部门ID取得人员信息（递归取得所有下属）
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> GetUserInfoByOrgIdWithSub(TargetNavigationDBEntities db, int organizationId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            var result = new List<UserBaseInfoModel>();

            //取得该部门的岗位信息
            IStationBLL stationBLL = new StationBLL();
            var stationInfo = stationBLL.GetStationInfoByOrgId(db, organizationId);

            if (stationInfo == null || stationInfo.Count == 0) return result;
            var stationIds = stationInfo.Select(x => x.stationId).ToArray();

            result = (from u in db.tblUser
                      join us in db.tblUserStation
                      on u.userId equals us.userId
                      where stationIds.Contains(us.stationId) && u.deleteFlag != true
                      select new UserBaseInfoModel
                      {
                          userId = u.userId,
                          userName = u.userName,
                          headImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage
                      }).ToList();

            return result;
        }

        #endregion 根据部门ID的处理

        #region 人员检索处理

        /// <summary>
        /// 人员模糊查询
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<UserBaseInfoModel> UserNameFuzzySearch(TargetNavigationDBEntities db, string userName)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = db.tblUser.Where(x => x.userName.Contains(userName) && x.deleteFlag != true && x.workStatus == (int)EnumDefine.WorkStatus.OnWork)
                .Select(x => new UserBaseInfoModel
                {
                    userId = x.userId,
                    userName = x.userName,
                    headImage = string.IsNullOrEmpty(x.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + x.bigImage
                }).ToList();

            return result;
        }

        #endregion 人员检索处理
    }
}