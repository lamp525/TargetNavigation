using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class RosterBLL : IRosterBLL
    {
        /// <summary>
        /// 用户详细信息取得
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public RosterModel GetRosterInfo(int userId)
        {
            var userModel = new RosterModel();
            using (var db = new TargetNavigationDBEntities())
            {
                userModel = (from userInfo in db.tblUser
                             //join province in db.tblProvince on userInfo.province equals province.provinceId
                             //join city in db.tblCity on province.provinceId equals city.provinceId
                             //join district in db.tblDistrict on city.cityId equals district.cityId
                             where userInfo.userId == userId && userInfo.deleteFlag == false
                             select new RosterModel
                             {
                                 userId = userId,
                                 userName = userInfo.userName,
                                 userNumber = userInfo.userNumber,
                                 sex = userInfo.sex,
                                 nation = userInfo.nation,
                                 political = userInfo.political,
                                 marriage = userInfo.marriage,
                                 mobile1 = userInfo.mobile1,
                                 mobile2 = userInfo.mobile2,
                                 address = userInfo.address,
                                 workPlace = userInfo.workPlace,
                                 entryTime = userInfo.entryTime,
                                 probationaryPeriod = userInfo.probationaryPeriod,
                                 positiveDate = userInfo.positiveDate,
                                 term = userInfo.term,
                                 expiredDate = userInfo.expiredDate,
                                 comment = userInfo.comment,
                                 birthday = userInfo.birthday,
                                 nature = userInfo.nature,
                                 provinceId = userInfo.province,
                                 cityId = userInfo.city,
                                 districtId = userInfo.district,
                                 school = userInfo.school,
                                 professional = userInfo.professional,
                                 education = userInfo.education,
                                 firstWork = userInfo.firstWork,
                                 qualification = userInfo.qualification,
                                 cornet = userInfo.cornet,
                                 emergencyNumber = userInfo.emergencyNumber,
                                 bankCard = userInfo.bankCard,
                                 workStatus = userInfo.workStatus,
                                 userType = userInfo.userType,
                                 quitTime = userInfo.quitTime,
                                 salary = userInfo.salary,
                                 identityCard = userInfo.identityCard,
                                 //籍贯
                                 nativePlace = userInfo.nativePlace,
                                 //qq
                                 qq = userInfo.qq,
                                 //email
                                 email = userInfo.email
                             }).FirstOrDefault();

                if (userModel != null)
                {
                    var model = (from province in db.tblProvince
                                 join city in db.tblCity on province.provinceId equals city.provinceId
                                 join district in db.tblDistrict on city.cityId equals district.cityId
                                 where province.provinceId == userModel.provinceId && city.cityId == userModel.cityId && district.districtId == userModel.districtId
                                 select new
                                 {
                                     province.provinceName,
                                     city.cityName,
                                     district.districtName
                                 }).FirstOrDefault();
                    if (model != null)
                    {
                        userModel.province = model.provinceName;
                        userModel.city = model.cityName;
                        userModel.district = model.districtName;
                    }

                    if (!string.IsNullOrEmpty(userModel.bankCard))
                    {
                        var cardId = userModel.bankCard.Substring(0, 6);
                        var bankId = db.tblBankCard.Where(c => c.cardId == cardId).Select(c => c.bankId).FirstOrDefault();
                        //银行卡名
                        userModel.bankName = db.tblBank.Where(c => c.bankId == bankId).Select(c => c.bankName).FirstOrDefault();
                    }

                    //用户所属组织
                    userModel.orgList = (from userStation in db.tblUserStation
                                         join station in db.tblStation on userStation.stationId equals station.stationId
                                         join organization in db.tblOrganization on station.organizationId equals organization.organizationId
                                         where userStation.userId == userId
                                         orderby organization.organizationId
                                         select new Organization
                                         {
                                             organizationId = organization.organizationId,
                                             OrganizationName = organization.organizationName,
                                             stationId = station.stationId,
                                             stationName = station.stationName
                                         }).ToList();
                }
            }
            return userModel;
        }

        /// <summary>
        /// 用户列表取得(用户)
        /// </summary>
        /// <returns></returns>
        public List<RosterInfo> GetRosterList(string userName = null)
        {
            string path = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            var rosterInfo = new List<RosterInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userName == null)
                {
                    rosterInfo = (from userinfo in db.tblUser
                                  where userinfo.deleteFlag == false
                                  select new RosterInfo
                                  {
                                      userId = userinfo.userId,
                                      userName = userinfo.userName,
                                      headImage = string.IsNullOrEmpty(userinfo.smallImage) ? "/Images/common/portrait.png" : "/" + path + "/" + userinfo.smallImage,
                                      mobile1 = userinfo.mobile1,
                                      mobile2 = userinfo.mobile2
                                  }).ToList();
                }
                else
                {
                    rosterInfo = (from userinfo in db.tblUser
                                  where userinfo.deleteFlag == false && userinfo.userName.IndexOf(userName) != -1
                                  select new RosterInfo
                                  {
                                      userId = userinfo.userId,
                                      userName = userinfo.userName,
                                      headImage = string.IsNullOrEmpty(userinfo.smallImage) ? "/Images/common/portrait.png" : "/" + path + "/" + userinfo.smallImage,
                                      mobile1 = userinfo.mobile1,
                                      mobile2 = userinfo.mobile2
                                  }).ToList();
                }

                foreach (var item in rosterInfo)
                {
                    item.station = (from us in db.tblUserStation
                                    join s in db.tblStation on us.stationId equals s.stationId
                                    where us.userId == item.userId
                                    select new Organization
                                    {
                                        stationId = s.stationId,
                                        stationName = s.stationName
                                    }).ToList();
                }
            }
            return rosterInfo;
        }

        /// <summary>
        /// 用户列表取得(组织)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public List<RosterInfo> GetRosterOrgList(int currentPage, int orgId, string userName = null)
        {
            var rosterList = new List<RosterInfo>();
            var num = int.Parse(ConfigurationManager.AppSettings["userPage"].ToString());
            var pageCount = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                if (userName == "")
                {
                    if (orgId == 0)
                    {
                        rosterList = (from user in db.tblUser
                                      where !user.deleteFlag
                                      select new RosterInfo
                                      {
                                          userId = user.userId,
                                          userNumber = user.userNumber,
                                          userName = user.userName,
                                          mobile1 = user.mobile1,
                                          mobile2 = user.mobile2,
                                          workStatus = user.workStatus,
                                          userType = user.userType,
                                          positiveDate = user.positiveDate,
                                          probationDate = user.probationDate,
                                          quitTime = user.quitTime,
                                          internDate = user.internDate
                                      }).Distinct().ToList();
                    }
                    else
                    {
                        rosterList = (from user in db.tblUser
                                      join userStation in db.tblUserStation on user.userId equals userStation.userId
                                      join station in db.tblStation on userStation.stationId equals station.stationId
                                      where user.deleteFlag == false && station.organizationId == orgId
                                      select new RosterInfo
                                      {
                                          userId = user.userId,
                                          userNumber = user.userNumber,
                                          userName = user.userName,
                                          mobile1 = user.mobile1,
                                          mobile2 = user.mobile2,
                                          workStatus = user.workStatus,
                                          userType = user.userType,
                                          positiveDate = user.positiveDate,
                                          probationDate = user.probationDate,
                                          quitTime = user.quitTime,
                                          internDate = user.internDate
                                      }).Distinct().ToList();
                    }
                    pageCount = rosterList.Count();
                    rosterList = rosterList.Skip(num * (currentPage - 1)).Take(num).ToList();
                }
                else
                {
                    if (orgId == 0)
                    {
                        rosterList = (from user in db.tblUser
                                      where !user.deleteFlag && user.userName.IndexOf(userName) != -1
                                      select new RosterInfo
                                      {
                                          userId = user.userId,
                                          userNumber = user.userNumber,
                                          userName = user.userName,
                                          mobile1 = user.mobile1,
                                          mobile2 = user.mobile2,
                                          workStatus = user.workStatus,
                                          userType = user.userType,
                                          positiveDate = user.positiveDate,
                                          probationDate = user.probationDate,
                                          quitTime = user.quitTime,
                                          internDate = user.internDate
                                      }).Distinct().ToList();
                    }

                    pageCount = rosterList.Count();
                    rosterList = rosterList.Skip(num * (currentPage - 1)).Take(num).ToList();
                }

                foreach (var item in rosterList)
                {
                    item.pageCount = pageCount;
                    //有效时间
                    //switch (item.workStatus)
                    //{
                    //    //转正时间
                    //    case 0: item.validDate = item.positiveDate; break;
                    //    case 1: item.validDate = item.quitTime; break;
                    //    case 2: item.validDate = item.quitTime; break;
                    //}
                    //switch (item.userType)
                    //{
                    //     //实习时间
                    //    case 1: item.validDate = item.internDate; break;
                    //    //试用时间
                    //    case 2: item.validDate = item.probationDate; break;
                    //}

                    //用户所属组织
                    item.station = (from userStation in db.tblUserStation
                                    join station in db.tblStation on userStation.stationId equals station.stationId
                                    join organization in db.tblOrganization on station.organizationId equals organization.organizationId
                                    where userStation.userId == item.userId
                                    orderby organization.organizationId
                                    select new Organization
                                    {
                                        organizationId = organization.organizationId,
                                        OrganizationName = organization.organizationName,
                                        stationId = station.stationId,
                                        stationName = station.stationName
                                    }).ToList();
                }
            }

            return rosterList;
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="passWord">带字母、数字、特殊字符的8位数密码</param>
        /// <param name="userId"></param>
        public string UpdatePassWord(int userId)
        {
            var key = "";
            using (var db = new TargetNavigationDBEntities())
            {
                var userInfo = db.tblUser.Where(c => c.userId == userId).FirstOrDefault();

                if (userInfo != null)
                {
                    key = Common.StringUtils.GetAllRandom(8);
                    userInfo.password = Common.EncryptHelper.PwdEncrypt(key, key);
                    userInfo.randomKey = key;
                }
                db.SaveChanges();
            }
            return key;
        }

        /// <summary>
        /// 根据银行卡号查找银行名称
        /// </summary>
        /// <param name="bankNum"></param>
        /// <returns></returns>
        public data GetBankName(string bankNum)
        {
            var data = new data();
            using (var db = new TargetNavigationDBEntities())
            {
                if (bankNum.Length >= 6)
                {
                    var cardId = bankNum.Substring(0, 6);
                    var bankId = db.tblBankCard.Where(c => c.cardId == cardId).Select(c => c.bankId).FirstOrDefault();
                    //银行卡名
                    data.strName = db.tblBank.Where(c => c.bankId == bankId).Select(c => c.bankName).FirstOrDefault();
                }
            }
            return data;
        }

        /// <summary>
        /// 根据身份证获取省份、城市、地区
        /// </summary>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        public List<data> GetTownByidentityCard(string identityCard)
        {
            var listData = new List<data>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (identityCard != null)
                {
                    if (identityCard.Length >= 2)
                    {
                        var two = int.Parse(identityCard.Substring(0, 2));
                        listData.Add(new data { strId = two, strName = db.tblProvince.Where(c => c.provinceId == two).Select(c => c.provinceName).FirstOrDefault() });
                    }
                    if (identityCard.Length >= 4)
                    {
                        var four = int.Parse(identityCard.Substring(2, 2));
                        listData.Add(new data { strId = four, strName = db.tblCity.Where(c => c.cityId == four).Select(c => c.cityName).FirstOrDefault() });
                    }
                    if (identityCard.Length >= 6)
                    {
                        var six = int.Parse(identityCard.Substring(4, 2));
                        listData.Add(new data { strId = six, strName = db.tblDistrict.Where(c => c.districtId == six).Select(c => c.districtName).FirstOrDefault() });
                    }
                }
            }
            return listData;
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="rosterModel">用户信息模型</param>
        /// <param name="deleteStation">删除岗位Id</param>
        /// <param name="newStation">添加岗位Id</param>
        public int SaveRosterInfo(RosterModel userModel, int LoginUser)
        {
            var id = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var userId = db.prcGetPrimaryKey("tblUser", obj).FirstOrDefault().Value;

                var user = db.tblUser.Where(c => c.userId == userModel.userId).FirstOrDefault();
                if (user == null)
                {
                    var key = Common.StringUtils.GetAllRandom(8);
                    //添加用户信息
                    var userInfo = new tblUser
                    {
                        userId = userId,
                        userNumber = userModel.userNumber,
                        userName = userModel.userName,
                        randomKey = key,
                        password = Common.EncryptHelper.PwdEncrypt("1", key),
                        sex = userModel.sex,
                        nation = userModel.nation,
                        political = userModel.political,
                        marriage = userModel.marriage,
                        mobile1 = userModel.mobile1,
                        mobile2 = userModel.mobile2,
                        address = userModel.address,
                        workPlace = userModel.workPlace,
                        entryTime = userModel.entryTime,
                        probationaryPeriod = userModel.probationaryPeriod,
                        positiveDate = userModel.positiveDate,
                        term = userModel.term,
                        expiredDate = userModel.expiredDate,
                        comment = userModel.comment,
                        birthday = userModel.birthday,
                        nature = userModel.nature,
                        province = userModel.provinceId,
                        city = userModel.cityId,
                        district = userModel.districtId,
                        school = userModel.school,
                        professional = userModel.professional,
                        education = userModel.education,
                        firstWork = userModel.firstWork,
                        qualification = userModel.qualification,
                        cornet = userModel.cornet,
                        emergencyNumber = userModel.emergencyNumber,
                        bankCard = userModel.bankCard,
                        workStatus = userModel.workStatus,
                        userType = userModel.userType,
                        quitTime = userModel.quitTime,
                        salary = userModel.salary,
                        nativePlace = userModel.nativePlace,
                        qq = userModel.qq,
                        email = userModel.email,
                        admin = false,
                        execution = false,
                        errorPassword = 0,
                        createUser = LoginUser,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        updateUser = LoginUser,
                        deleteFlag = false,
                        identityCard = userModel.identityCard
                    };
                    db.tblUser.Add(userInfo);
                    //添加用户岗位
                    if (userModel.newStation != null)
                    {
                        var uStation = db.tblUserStation.Where(c => c.userId == userInfo.userId).ToList();
                        db.tblUserStation.RemoveRange(uStation);
                        foreach (var item in userModel.newStation.Distinct())
                        {
                            var userStation = new tblUserStation
                            {
                                userId = userInfo.userId,
                                stationId = item
                            };
                            db.tblUserStation.Add(userStation);
                        }
                    }
                    id = userId;
                }
                else
                {
                    //更新用户信息
                    user.userNumber = userModel.userNumber;
                    user.userName = userModel.userName;
                    user.sex = userModel.sex;
                    user.nation = userModel.nation;
                    user.political = userModel.political;
                    user.marriage = userModel.marriage;
                    user.mobile1 = userModel.mobile1;
                    user.mobile2 = userModel.mobile2;
                    user.address = userModel.address;
                    user.workPlace = userModel.workPlace;
                    user.entryTime = userModel.entryTime;
                    user.probationaryPeriod = userModel.probationaryPeriod;
                    user.positiveDate = userModel.positiveDate;
                    user.term = userModel.term;
                    user.expiredDate = userModel.expiredDate;
                    user.comment = userModel.comment;
                    user.birthday = userModel.birthday;
                    user.nature = userModel.nature;
                    user.province = userModel.provinceId;
                    user.city = userModel.cityId;
                    user.district = userModel.districtId;
                    user.school = userModel.school;
                    user.professional = userModel.professional;
                    user.education = userModel.education;
                    user.firstWork = userModel.firstWork;
                    user.qualification = userModel.qualification;
                    user.cornet = userModel.cornet;
                    user.emergencyNumber = userModel.emergencyNumber;
                    user.bankCard = userModel.bankCard;
                    user.workStatus = userModel.workStatus;
                    user.userType = userModel.userType;
                    user.quitTime = userModel.quitTime;
                    user.salary = userModel.salary;
                    user.updateTime = DateTime.Now;
                    user.updateUser = LoginUser;
                    user.qq = userModel.qq;
                    user.email = userModel.email;
                    user.nativePlace = userModel.nativePlace;
                    user.identityCard = userModel.identityCard;

                    //添加用户岗位
                    if (userModel.newStation != null)
                    {
                        var uStation = db.tblUserStation.Where(c => c.userId == userModel.userId).ToList();
                        db.tblUserStation.RemoveRange(uStation);

                        foreach (var item in userModel.newStation.Distinct())
                        {
                            var userStation = new tblUserStation
                            {
                                userId = userModel.userId,
                                stationId = item
                            };
                            db.tblUserStation.Add(userStation);
                        }
                    }
                    id = userModel.userId;
                }
                //db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }
            return id;
        }

        /// <summary>
        /// 根据用户id更新状态
        /// 在职状态  1：转正 2：离职 3：退休 4：实习 5：试用
        /// </summary>
        public void UpdateWorkStatusById(int userId, int workStatus, DateTime validDate)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var userInfo = db.tblUser.Where(c => c.userId == userId).FirstOrDefault();
                switch (workStatus)
                {
                    case 1: userInfo.workStatus = 1;
                        userInfo.positiveDate = validDate; break;
                    case 2: userInfo.workStatus = 2;
                        userInfo.quitTime = validDate; break;
                    case 3: userInfo.workStatus = 3;
                        userInfo.quitTime = validDate; break;
                    //实习时间
                    case 4: userInfo.workStatus = 4;
                        userInfo.internDate = validDate; break;
                    //试用时间
                    case 5: userInfo.workStatus = 5;
                        userInfo.probationDate = validDate; break;
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public void DeleteUser(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = (from user in db.tblUser
                             where user.userId == userId
                             select user).FirstOrDefault();
                model.deleteFlag = true;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 导出用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public List<RosterModel> ExportFile(int orgId, string userName = null)
        {
            var rosterList = new List<RosterModel>();
            var userIdList = new List<int>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (userName == null || userName == "")
                {
                    if (orgId == 0)
                    {
                        userIdList = (from user in db.tblUser
                                      where !user.deleteFlag
                                      select user.userId).Distinct().ToList();
                    }
                    else
                    {
                        userIdList = (from user in db.tblUser
                                      join userStation in db.tblUserStation on user.userId equals userStation.userId
                                      join station in db.tblStation on userStation.stationId equals station.stationId
                                      where station.organizationId == orgId && !user.deleteFlag
                                      select user.userId).Distinct().ToList();
                    }
                    foreach (var item in userIdList)
                    {
                        rosterList.Add(this.GetRosterInfo(item));
                    }
                }
                else
                {
                    if (orgId == 0)
                    {
                        userIdList = (from user in db.tblUser
                                      where !user.deleteFlag && user.userName.IndexOf(userName) != -1
                                      select user.userId).Distinct().ToList();
                    }
                    foreach (var item in userIdList)
                    {
                        rosterList.Add(this.GetRosterInfo(item));
                    }
                }
            }
            return rosterList;
        }

        /// <summary>
        /// 根据工号验证工号重复
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool UserNumBerIsHave(string num)
        {
            var value = false;
            using(var db=new TargetNavigationDBEntities()){
                var model = db.tblUser.Where(c => c.userNumber == num).FirstOrDefault();
                if (model == null)
                {
                    value = false;
                }
                else
                {
                    value = true;
                }
            }
            return value;
        }
    }
}