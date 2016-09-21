using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class OrganizationManagementBLL : IOrganizationManagementBLL
    {
        #region 获取组织架构列表(含模糊查询)

        /// <summary>
        /// 获取组织架构列表(含模糊查询)
        /// </summary>
        /// <param name="name">模糊查询字段</param>
        /// <param name="parentId">父Id</param>
        /// <returns>组织架构列表</returns>
        public List<OrgModel> GetOrganizationList(string name, int? parentId)
        {
            var orgList = new List<OrgModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    orgList = (from o in db.tblOrganization
                               join p in db.tblOrganization on o.parentOrganization equals p.organizationId into group1
                               from p in group1.DefaultIfEmpty()
                               where o.parentOrganization == parentId && !o.deleteFlag
                               orderby o.organizationId
                               select new OrgModel
                               {
                                   organizationId = o.organizationId,
                                   parentOrganization = o.parentOrganization,
                                   parentName = p.organizationName,
                                   schemaName = o.schemaName.Value,
                                   orderNumber = o.orderNumber,
                                   organizationName = o.organizationName,
                                   description = o.description
                               }).ToList();
                }
                else
                {
                    orgList = (from o in db.tblOrganization
                               join p in db.tblOrganization on o.parentOrganization equals p.organizationId into group1
                               from p in group1.DefaultIfEmpty()
                               where !o.deleteFlag && o.organizationName.IndexOf(name) >= 0
                               orderby o.organizationId
                               select new OrgModel
                               {
                                   organizationId = o.organizationId,
                                   parentOrganization = o.parentOrganization,
                                   parentName = p.organizationName,
                                   schemaName = o.schemaName.Value,
                                   orderNumber = o.orderNumber,
                                   organizationName = o.organizationName,
                                   description = o.description
                               }).ToList();
                    //if (orgList.Count > 0 && !string.IsNullOrWhiteSpace(name))
                    //{
                    //    orgList = orgList.Where(p => p.organizationName.IndexOf(name) >= 0).ToList();
                    //}
                }
            }
            return orgList;
        }

        #endregion 获取组织架构列表(含模糊查询)

        #region 获取组织架构详情

        /// <summary>
        /// 获取组织架构详情
        /// </summary>
        /// <param name="organizationId">组织Id</param>
        /// <returns>组织架构详情</returns>
        public OrgModel GetOrganizationInfo(int organizationId)
        {
            var orgInfo = new OrgModel();
            using (var db = new TargetNavigationDBEntities())
            {
                orgInfo = (from o in db.tblOrganization
                           where o.organizationId == organizationId

                           select new OrgModel
                           {
                               organizationId = o.organizationId,
                               organizationName = o.organizationName,
                               schemaName = o.schemaName.Value,
                               parentOrganization = o.parentOrganization.Value,
                               description = o.description,
                               withSub = o.withSub
                           }).FirstOrDefault();
            }
            return orgInfo;
        }

        #endregion 获取组织架构详情

        #region 新建/更新组织架构

        /// <summary>
        /// 新建/更新组织架构
        /// </summary>
        /// <param name="orgInfo">组织架构实体</param>
        /// <param name="userId">用户Id</param>
        public void SaveOrganization(OrgModel orgInfo, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (orgInfo.organizationId == null) //新建
                {
                    if (orgInfo.parentOrganization != null)
                    {
                        var date = db.tblOrganization.Where(p => p.organizationId == orgInfo.parentOrganization).FirstOrDefault();
                        if (date != null)
                        {
                            date.withSub = true;
                            db.SaveChanges();
                        }
                    }
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    orgInfo.organizationId = db.prcGetPrimaryKey("tblOrganization", obj).FirstOrDefault().Value;
                    var orderNum = db.tblOrganization.Count();
                    var orgModel = new tblOrganization
                    {
                        organizationId = Convert.ToInt32(orgInfo.organizationId),
                        parentOrganization = orgInfo.parentOrganization,
                        schemaName = orgInfo.schemaName,
                        organizationName = orgInfo.organizationName,
                        withSub = false,
                        orderNumber = orderNum + 1,
                        description = orgInfo.description,
                        createUser = userId,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        updateUser = userId
                    };
                    db.tblOrganization.Add(orgModel);
                    db.SaveChanges();
                }
                else   //更新
                {
                    var firstOrgModel = db.tblOrganization.Where(p => p.organizationId == orgInfo.organizationId && !p.deleteFlag).FirstOrDefault();
                    var thisPare = firstOrgModel.parentOrganization;
                    if (orgInfo.parentOrganization != null)
                    {
                        var date = db.tblOrganization.Where(p => p.organizationId == orgInfo.parentOrganization).FirstOrDefault();
                        if (date != null)
                        {
                            date.withSub = true;
                            db.SaveChanges();
                        }
                    }

                    if (firstOrgModel != null)
                    {
                        firstOrgModel.parentOrganization = orgInfo.parentOrganization;
                        firstOrgModel.schemaName = orgInfo.schemaName;
                        firstOrgModel.organizationName = orgInfo.organizationName;
                        firstOrgModel.description = orgInfo.description;
                        firstOrgModel.updateTime = DateTime.Now;
                        firstOrgModel.updateUser = userId;
                    }
                    db.SaveChanges();

                    if (thisPare != null)
                    {
                        var date = db.tblOrganization.Where(p => p.organizationId == thisPare).FirstOrDefault();
                        var isparent = db.tblOrganization.Where(p => p.parentOrganization == date.organizationId).ToList();
                        if (isparent.Count == 0)
                        {
                            date.withSub = false;
                        }
                        else
                        {
                            date.withSub = true;
                        }
                        db.SaveChanges();
                    }
                }
            }
        }

        #endregion 新建/更新组织架构

        #region 根据上级组织Id获取下级的组织列表

        /// <summary>
        /// 根据上级组织Id获取下级的组织列表
        /// </summary>
        /// <param name="orgId">上级组织Id</param>
        /// <returns>下级组织列表</returns>
        public List<OrgModel> GetOrgListById(int? orgId, int? organizationId)
        {
            var orgList = new List<OrgModel>();
            var downlist = new List<OrgModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                orgList = (from o in db.tblOrganization
                           where o.parentOrganization == orgId && !o.deleteFlag
                           orderby o.orderNumber
                           select new OrgModel
                           {
                               id = o.organizationId,
                               name = o.organizationName,
                               isParent = o.withSub.Value,
                               parentOrganization = o.parentOrganization
                           }).ToList();
                if (organizationId != null)
                {
                    if (orgList.Count > 0)
                    {
                        downlist = GetDownOrg(Convert.ToInt32(organizationId), ref downlist, db);
                    }
                }
            }
            var mySelf = new List<OrgModel>();
            if (downlist.Count > 0)
            {
                foreach (var item in downlist)
                {
                    orgList.Remove(item);
                }
                foreach (var index in orgList)
                {
                    if (index.id == organizationId)
                    {
                        mySelf.Add(index);
                    }
                }
                if (mySelf.Count > 0)
                {
                    foreach (var dd in mySelf)
                    {
                        orgList.Remove(dd);
                    }
                }
            }
            else
            {
                foreach (var index in orgList)
                {
                    if (index.id == organizationId)
                    {
                        mySelf.Add(index);
                    }
                }
                if (mySelf.Count > 0)
                {
                    foreach (var dd in mySelf)
                    {
                        orgList.Remove(dd);
                    }
                }
            }
            return orgList;
        }

        #endregion 根据上级组织Id获取下级的组织列表

        #region 删除组织架构

        /// <summary>
        /// 删除组织架构
        /// </summary>
        /// <param name="organizationId">删除的Id集合</param>
        public string DeleteOrganization(int[] organizationId)
        {
            var msg = string.Empty;
            using (var db = new TargetNavigationDBEntities())
            {
                if (organizationId.Length > 0)
                {
                    foreach (var id in organizationId)
                    {
                        var orgdata = db.tblOrganization.Where(p => p.organizationId == id).FirstOrDefault();
                        //存在子组织的情况，不能删除
                        if (orgdata.withSub.Value)
                        {
                            msg = "1";
                            break;
                        }
                        //组织架构上如果存在岗位，也不能删除
                        var station = db.tblStation.Where(p => p.organizationId == orgdata.organizationId && !p.deleteFlag);
                        if (station.Count() > 0)
                        {
                            msg = "2";
                            break;
                        }
                        else
                        {
                            var parentid = orgdata.parentOrganization;
                            db.tblOrganization.Remove(orgdata);
                            db.SaveChanges();
                            var isTo = db.tblOrganization.Where(p => p.parentOrganization == parentid && p.deleteFlag != true).Count();
                            if (isTo == 0)
                            {
                                var pareOrg = db.tblOrganization.Where(p => p.organizationId == parentid).FirstOrDefault();
                                pareOrg.withSub = false;
                                db.SaveChanges();
                            }
                        }
                    }
                    //foreach (var item in orgList)
                    //{
                    //}
                }
            }
            return msg;
        }

        #endregion 删除组织架构

        #region 组织架构排序

        /// <summary>
        /// 组织架构排序
        /// </summary>
        /// <param name="orderList">组织架构排序集合</param>
        public void OrderOrganization(List<OrgModel> orderList)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (orderList.Count > 0)
                {
                    foreach (var item in orderList)
                    {
                        var orgModel = db.tblOrganization.Where(p => p.organizationId == item.organizationId).FirstOrDefault();
                        if (orgModel != null)
                        {
                            orgModel.orderNumber = item.orderNumber;
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        #endregion 组织架构排序

        /// <summary>
        /// 获取所有下级岗位
        /// </summary>
        /// <param name="Stationid"></param>
        /// <param name="StationList"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<StationModel> GetDownStation(int Stationid, ref List<StationModel> StationList, TargetNavigationDBEntities db)
        {
            var downList = db.tblStation.Where(p => p.parentStation == Stationid).ToList();
            if (downList != null)
            {
                foreach (var Model in downList)
                {
                    var Station = new StationModel
                    {
                        id = Model.stationId,
                        name = Model.stationName,
                        parentStation = Model.parentStation
                    };

                    StationList.Add(Station);
                    if (Station.parentStation != null)
                    {
                        GetDownStation(Convert.ToInt32(Station.stationId), ref StationList, db);
                    }
                }
            }
            return StationList;
        }

        public List<OrgModel> GetDownOrg(int orgid, ref List<OrgModel> orgList, TargetNavigationDBEntities db)
        {
            var downList = db.tblOrganization.Where(p => p.parentOrganization == orgid).ToList();
            if (downList != null)
            {
                foreach (var Model in downList)
                {
                    var org = new OrgModel
                    {
                        id = Model.organizationId,
                        name = Model.organizationName,
                        parentOrganization = Model.parentOrganization,
                        isParent = Model.withSub.Value
                    };

                    orgList.Add(org);
                    if (org.parentOrganization != null)
                    {
                        GetDownOrg(Convert.ToInt32(org.organizationId), ref orgList, db);
                    }
                }
            }
            return orgList;
        }

        #region 岗位列表

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <param name="name">岗位模糊查询字段</param>
        /// <param name="organizationId">组织Id</param>
        /// <returns>岗位列表</returns>
        public List<StationModel> GetStationList(string name, int? organizationId, int? stationId)
        {
            var stationList = new List<StationModel>();
            var DownList = new List<StationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    stationList = (from s in db.tblStation
                                   join p in db.tblStation on s.parentStation equals p.stationId into group1
                                   from p in group1.DefaultIfEmpty()
                                   join o in db.tblOrganization on s.organizationId equals o.organizationId into group2
                                   from o in group2.DefaultIfEmpty()
                                   where !s.deleteFlag
                                   select new StationModel
                                   {
                                       organizationId = s.organizationId.Value,
                                       organizationName = o.organizationName,
                                       stationId = s.stationId,
                                       stationName = s.stationName,
                                       parentStationName = p.stationName,
                                       approval = s.approval
                                   }).ToList();
                }
                else
                {
                    if (organizationId != null)
                    {
                        stationList = (from s in db.tblStation
                                       join p in db.tblStation on s.parentStation equals p.stationId into group1
                                       from p in group1.DefaultIfEmpty()
                                       join o in db.tblOrganization on s.organizationId equals o.organizationId into group2
                                       from o in group2.DefaultIfEmpty()
                                       where s.organizationId == organizationId && !s.deleteFlag
                                       select new StationModel
                                       {
                                           organizationId = s.organizationId.Value,
                                           organizationName = o.organizationName,
                                           stationId = s.stationId,
                                           stationName = s.stationName,
                                           parentStationName = p.stationName,
                                           approval = s.approval
                                           
                                       }).ToList();
                        if (stationId != null)
                        {
                            if (stationList.Count > 0)
                            {
                                DownList = GetDownStation(Convert.ToInt32(stationId), ref  DownList, db);
                            }
                            var Rem = new List<StationModel>();
                            if (DownList.Count > 0)
                            {
                                foreach (var id in DownList)
                                {
                                    foreach (var item in stationList)
                                    {
                                        if (item.stationId == id.id || item.stationId == stationId)
                                        {
                                            Rem.Add(item);
                                        }
                                    }
                                }

                                foreach (var re in Rem)
                                {
                                    stationList.Remove(re);
                                }
                            }
                            else
                            {
                                foreach (var dd in stationList)
                                {
                                    if (dd.stationId == stationId)
                                    {
                                        Rem.Add(dd);
                                    }
                                }
                                foreach (var re in Rem)
                                {
                                    stationList.Remove(re);
                                }
                            }
                        }
                    }
                    else
                    {
                        stationList = (from s in db.tblStation
                                       join p in db.tblStation on s.parentStation equals p.stationId into group1
                                       from p in group1.DefaultIfEmpty()
                                       join o in db.tblOrganization on s.organizationId equals o.organizationId into group2
                                       from o in group2.DefaultIfEmpty()
                                       where !s.deleteFlag
                                       select new StationModel
                                       {
                                           organizationId = s.organizationId.Value,
                                           organizationName = o.organizationName,
                                           stationId = s.stationId,
                                           stationName = s.stationName,
                                           parentStationName = p.stationName,
                                           approval = s.approval 
                                       }).ToList();
                    }
                }
                if (stationList.Count > 0)
                {
                    //name不为空，及存在模糊查询
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        stationList = stationList.Where(p => p.stationName.IndexOf(name) >= 0).ToList();
                    }
                    //加载每个岗位上的人员
                    stationList.ForEach(p =>
                    {
                        p.userName = (from us in db.tblUserStation
                                      join u in db.tblUser on us.userId equals u.userId
                                      where !u.deleteFlag && us.stationId == p.stationId
                                      select u.userName).ToArray();
                    });
                }
            }
            return stationList.OrderBy(p => p.organizationId).ThenBy(p => p.stationName).ToList();
        }

        #endregion 岗位列表

        #region 获取当前岗位下人员

        /// <summary>

        /// </summary>
        /// <param name="stationid">岗位Id</param>
        /// <returns>共享人列表</returns>
        public List<UserInfo> GetUserList(int stationid)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from c in db.tblUserStation
                            join u in db.tblUser
                            on c.userId equals u.userId
                            where c.stationId == stationid && !u.deleteFlag
                            select new UserInfo
                            {
                                userId = u.userId,
                                userName = u.userName
                            }).ToList();
            }
            return userList;
        }

        public List<UserInfo> GetAllUser()
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from u in db.tblUser
                            join us in db.tblUserStation on u.userId equals us.userId into staion
                            from s in staion.DefaultIfEmpty()
                            where !u.deleteFlag && u.workStatus == 1 && s.stationId == 0
                            select new UserInfo
                            {
                                userId = u.userId,
                                userName = u.userName,
                                smallImage = u.smallImage
                            }).ToList();
            }
            return userList;
        }

        #endregion 获取当前岗位下人员

        #region 获取岗位详情

        /// <summary>
        /// 获取岗位详情
        /// </summary>
        /// <param name="id">岗位Id</param>
        /// <returns>该岗位的详情</returns>
        public StationModel GetStationInfo(int id)
        {
            var stationInfo = new StationModel();
            using (var db = new TargetNavigationDBEntities())
            {
                stationInfo = (from s in db.tblStation
                               where s.stationId == id
                               select new StationModel
                               {
                                   stationId = s.stationId,
                                   parentStation = s.parentStation.Value,
                                   stationName = s.stationName,
                                   organizationId = s.organizationId.Value,
                                   comment = s.comment,
                                   approval = s.approval.Value 
                               }).FirstOrDefault();
            }
            return stationInfo;
        }

        #endregion 获取岗位详情

        #region 删除岗位

        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="id">岗位Id</param>
        public string DeleteStation(int[] id)
        {
            var msg = string.Empty;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in id)
                {
                    var stationModel = db.tblStation.Where(p => p.stationId == item).FirstOrDefault();
                    if (stationModel != null)
                    {
                        //存在下级岗位或者该岗位上有人员，无法删除
                        if (db.tblStation.Where(p => p.parentStation == stationModel.stationId && !p.deleteFlag).Count() > 0 || db.tblUserStation.Where(p => p.stationId == stationModel.stationId).Count() > 0)
                        {
                            msg = "1";
                            return msg;
                        }
                        db.tblStation.Remove(stationModel);
                        db.SaveChanges();
                    }
                }
            }
            return msg;
        }

        #endregion 删除岗位

        #region 新建/更新岗位

        /// <summary>
        /// 新建/更新岗位
        /// </summary>
        /// <param name="stationModel">岗位实体</param>
        public int SaveStation(StationModel stationInfo, int userId)
        {
            int flag = 2;
            using (var db = new TargetNavigationDBEntities())
            {
                if (stationInfo.stationId == null) //新建
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    stationInfo.stationId = db.prcGetPrimaryKey("tblStation", obj).FirstOrDefault().Value;
                    var station = new tblStation
                    {
                        stationId = Convert.ToInt32(stationInfo.stationId),
                        parentStation = stationInfo.parentStation,
                        stationName = stationInfo.stationName,
                        organizationId = stationInfo.organizationId,
                        comment = stationInfo.comment,
                        approval = stationInfo.approval, 
                        createUser = userId,
                        createTime = DateTime.Now,
                        updateUser = userId,
                        updateTime = DateTime.Now
                    };
                    db.tblStation.Add(station);
                    flag = 0;
                }
                else  //更新
                {
                    var firstStation = db.tblStation.Where(p => p.stationId == stationInfo.stationId).FirstOrDefault();
                    if (firstStation != null)
                    {
                        firstStation.parentStation = stationInfo.parentStation;
                        firstStation.stationName = stationInfo.stationName;
                        firstStation.organizationId = stationInfo.organizationId;
                        firstStation.comment = stationInfo.comment;
                        firstStation.approval = stationInfo.approval; 
                        firstStation.updateUser = userId;
                        firstStation.updateTime = DateTime.Now;
                    }
                    flag = 1;
                }
                db.SaveChanges();
            }
            return flag;
        }

        #endregion 新建/更新岗位

        #region 添加岗位人员

        /// <summary>
        /// 添加岗位人员
        /// </summary>
        /// <param name="stationId">岗位Id</param>
        /// <param name="deleteUser">删除的人员</param>
        /// <param name="addUser">新增的人员</param>
        public void AddUser(int stationId, int[] addUser)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //1、删除该岗位上需要删除的人员
                var hasUser = db.tblUserStation.Where(p => p.stationId == stationId).ToList();
                if (hasUser.Count > 0)
                {
                    foreach (var deluser in hasUser)
                    {
                        db.tblUserStation.Remove(deluser);
                    }
                }

                //2、添加该岗位上新增人员
                if (addUser.Length > 0)
                {
                    foreach (var item in addUser)
                    {
                        var user = new tblUserStation
                        {
                            stationId = stationId,
                            userId = item
                        };
                        db.tblUserStation.Add(user);
                    }
                }
                db.SaveChanges();
            }
        }

        #endregion 添加岗位人员

        #region 添加岗位手册

        /// <summary>
        /// 添加岗位手册
        /// </summary>
        /// <param name="deleteId">删除的计划Id集合</param>
        /// <param name="loopPlanList">新增的计划集合</param>
        /// <param name="userId">用户</param>
        public void AddStationManual(int[] deleteId, List<LoopPlanInfo> loopPlanList, int userId, int stationid)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //1、删除当前岗位上需删除的计划
                if (deleteId.Length > 0)
                {
                    var delPlanList = db.tblLoopPlan.Where(p => deleteId.Contains(p.loopId));
                    db.tblLoopPlan.RemoveRange(delPlanList);
                }
                //2、添加当前岗位上新增的循环计划
                if (loopPlanList.Count > 0)
                {
                    loopPlanList.ForEach(p =>
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        p.loopId = db.prcGetPrimaryKey("tblLoopPlan", obj).FirstOrDefault().Value;
                        var loopModel = new tblLoopPlan
                        {
                            loopId = Convert.ToInt32(p.loopId),
                            stationId = stationid,
                            organizationId = p.organizationId,
                            executionModeId = p.executionModeId,
                            eventOutput = p.eventOutput,
                            confirmUser = p.confirmUser,
                            status = 0,
                            loopYear = p.loopYear,
                            loopMonth = p.loopMonth,
                            loopWeek = p.loopWeek,
                            loopTime = p.loopTime,
                            loopType = p.loopType,
                            loopStatus = true,
                            createUser = userId,
                            createTime = DateTime.Now,
                            updateUser = userId,
                            updateTime = DateTime.Now,
                            unitTime = p.unitTime,
                            difficulty = 1,
                            importance = 1,
                            urgency = 1
                        };
                        db.tblLoopPlan.Add(loopModel);
                        ////计划完成表插入数据
                        //var loopSubmintModel = new tblLoopplanSubmit
                        //{
                        //    submitId = db.prcGetPrimaryKey("tblLoopPlanSubmit", obj).FirstOrDefault().Value,
                        //    loopId = Convert.ToInt32(p.loopId),
                        //    completeQuantity = Convert.ToDecimal(1.0),
                        //    completeQuality = Convert.ToDecimal(1.0),
                        //    completeTime = Convert.ToDecimal(1.0),
                        //    createUser = userId,
                        //    createTime = DateTime.Now,
                        //    updateUser = userId,
                        //    updateTime = DateTime.Now
                        //};
                        //db.tblLoopplanSubmit.Add(loopSubmintModel);
                    });
                }
                db.SaveChanges();
            }
        }

        #endregion 添加岗位手册

        #region 获取岗位手册列表

        /// <summary>
        /// 获取岗位手册列表
        /// </summary>
        /// <param name="stationId">岗位Id</param>
        /// <returns>岗位手册列表</returns>
        public List<LoopPlanInfo> GetStationManual(int stationId)
        {
            var loopList = new List<LoopPlanInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                loopList = (from l in db.tblLoopPlan
                            join n in db.tblExecutionMode on l.executionModeId equals n.executionId
                            where l.stationId == stationId
                            select new LoopPlanInfo
                            {
                                loopId = l.loopId,
                                stationId = l.stationId,
                                organizationId = l.organizationId,
                                executionModeId = l.executionModeId,
                                eventOutput = l.eventOutput,
                                confirmUser = l.confirmUser,
                                loopYear = l.loopYear,
                                loopMonth = l.loopMonth,
                                loopWeek = l.loopWeek,
                                loopTime = l.loopTime,
                                loopType = l.loopType,
                                executionName = n.executionMode
                            }).ToList();
            }
            return loopList;
        }

        #endregion 获取岗位手册列表
    }
}