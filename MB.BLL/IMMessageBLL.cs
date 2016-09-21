using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class IMMessageBLL : IIMMessageBLL
    {
        #region 常用联系人操作

        /// <summary>
        ///获取常用联系人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ImContactModel> GetImContactList(int userId)
        {
            var list = new List<ImContactModel>();
            string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            using (var db = new TargetNavigationDBEntities())
            {
                list = (from contact in db.tblImContacts
                        join user in db.tblUser
                        on contact.contactsId equals user.userId
                        where contact.userId == userId
                        select new ImContactModel
                        {
                            id = contact.userId,
                            contactId = contact.contactsId,
                            contactName = user.userName,
                            headImage = string.IsNullOrEmpty(user.originalImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + user.bigImage
                        }).ToList();

                foreach (var item in list)
                {
                    // 判断是否在线
                    var model = db.tblImLogin.Where(p => p.userId == item.contactId).FirstOrDefault();
                    item.onLine = model == null ? false : true;

                    // 取得所在岗位
                    item.station = this.getUserStation(item.contactId, db);
                }
            }

            return list;
        }

        /// <summary>
        /// 添加常用联系人
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="contactsId">联系人ID</param>
        public void AddImContact(int userId, int[] contactsId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var id in contactsId)
                {
                    var model = db.tblImContacts.Where(p => p.userId == userId && p.contactsId == id).FirstOrDefault();

                    if (model != null)
                    {
                        continue;
                    }

                    var contact = new tblImContacts
                    {
                        userId = userId,
                        contactsId = id
                    };

                    db.tblImContacts.Add(contact);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除常用联系人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contactsId"></param>
        public void DeleteImContact(int userId, int contactsId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImContacts.Where(p => p.userId == userId && p.contactsId == contactsId).FirstOrDefault();

                if (model != null)
                {
                    db.tblImContacts.Remove(model);
                    db.SaveChanges();
                }
            }
        }

        #endregion 常用联系人操作

        #region 群组操作

        /// <summary>
        /// 获取群组列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<ImGroupModel> GetImGroupList(int userId)
        {
            var list = new List<ImGroupModel>();

            string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            using (var db = new TargetNavigationDBEntities())
            {
                list = (from grp in db.tblImGroups
                        join user in db.tblImGroupUser
                        on grp.groupId equals user.groupId
                        where user.userId == userId
                        select new ImGroupModel
                        {
                            groupId = grp.groupId,
                            groupName = grp.groupName,
                            headImage = string.IsNullOrEmpty(grp.headImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + grp.headImage,
                            type = grp.type,
                            createUser = grp.createUser
                        }).ToList();
            }

            return list;
        }

        /// <summary>
        /// 获取群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns></returns>
        public List<ImContactModel> GetImGroupUser(int groupId)
        {
            var list = new List<ImContactModel>();

            string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            using (var db = new TargetNavigationDBEntities())
            {
                list = (from grp in db.tblImGroupUser
                        join user in db.tblUser
                        on grp.userId equals user.userId
                        where grp.groupId == groupId
                        select new ImContactModel
                        {
                            id = grp.groupId,
                            contactId = grp.userId,
                            contactName = user.userName,
                            headImage = string.IsNullOrEmpty(user.originalImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + user.bigImage,
                            power = grp.power
                        }).ToList();

                foreach (var item in list)
                {
                    // 判断是否在线
                    var model = db.tblImLogin.Where(p => p.userId == item.contactId).FirstOrDefault();
                    item.onLine = model == null ? false : true;

                    // 取得所在岗位
                    item.station = this.getUserStation(item.contactId, db);
                }
            }

            return list;
        }

        /// <summary>
        /// 添加新的群组
        /// </summary>
        /// <param name="model">群组信息</param>
        /// <param name="userId">群组联系人</param>
        /// <param name="groupUserId">登陆用户ID</param>
        public void AddImGroup(ImGroupModel model, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                // 添加群组信息
                var groupId = this.AddNewGroup(userId, model, db);

                // 添加群组成员
                this.AddGroupUser(groupId, model.groupUserId, userId, db);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 添加群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="groupUserId">群组联系人/param>
        public void AddImGroupUser(int groupId, int[] groupUserId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                // 添加群组成员
                this.AddGroupUser(groupId, groupUserId, null, db);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 退出群组
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">用户ID</param>
        public void QuitGroup(int groupId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImGroupUser.Where(p => p.groupId == groupId && p.userId == userId).FirstOrDefault();

                if (model == null)
                {
                    return;
                }

                // 解散群组
                if (model.power == 1)
                {
                    var group = db.tblImGroups.Where(p => p.groupId == groupId).FirstOrDefault();

                    if (group != null)
                    {
                        db.tblImGroups.Remove(group);
                    }

                    var userList = db.tblImGroupUser.Where(p => p.groupId == groupId).ToList();

                    if (userList.Count > 0)
                    {
                        db.tblImGroupUser.RemoveRange(userList);
                    }
                }
                else
                {
                    db.tblImGroupUser.Remove(model);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 设置群组管理员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">群成员ID</param>
        public void SetGroupManager(int groupId, int userId, int? power)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImGroupUser.Where(p => p.groupId == groupId && p.userId == userId).FirstOrDefault();

                if (model != null)
                {
                    model.power = power;
                    db.SaveChanges();
                }
            }
        }

        #endregion 群组操作

        #region 最近会话

        /// <summary>
        /// 获取最近会话列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<ConversationModel> GetConversationList(int userId)
        {
            FileUpload file = new FileUpload();
            string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            var list = new List<ConversationModel>();

            var startTime = DateTime.Now.AddDays(-6).Date;
            var endTime = DateTime.Now.AddDays(1).Date;

            var userMsgList = new List<ConversationModel>();
            var grpMsgList = new List<ConversationModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var tmpList = (from msg in db.tblImMessages
                               join user1 in db.tblUser
                                   on msg.fromUser equals user1.userId into tmp1
                               from fromUser in tmp1.DefaultIfEmpty()
                               join user2 in db.tblUser
                                   on msg.toUser equals user2.userId into tmp2
                               from toUser in tmp2.DefaultIfEmpty()
                               where msg.groupId == null && (msg.fromUser == userId || msg.toUser == userId)
                               select new ConversationModel
                               {
                                   id = msg.fromUser == userId ? msg.toUser : msg.fromUser,
                                   name = msg.fromUser == userId ? toUser.userName : fromUser.userName,
                                   headImage = msg.fromUser == userId ? toUser.bigImage : fromUser.bigImage,
                                   sendTime = msg.sendTime.Value
                               }).ToList();

                userMsgList = (from msg in tmpList
                               group msg by new { msg.id, msg.name, msg.headImage } into tmp
                               select new ConversationModel
                               {
                                   type = 1,
                                   id = tmp.Key.id,
                                   name = tmp.Key.name,
                                   headImage = string.IsNullOrEmpty(tmp.Key.headImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + tmp.Key.headImage,
                                   sendTime = tmp.Max(p => p.sendTime)
                               }).ToList();

                foreach (var item in userMsgList)
                {
                    // 判断是否在线
                    var model = db.tblImLogin.Where(p => p.userId == item.id).FirstOrDefault();
                    item.onLine = model == null ? false : true;

                    // 取得所在岗位
                    item.station = this.getUserStation(item.id, db);
                }

                var myGroup = (from grp in db.tblImGroupUser
                               where grp.userId == userId
                               select grp.groupId).ToArray();

                grpMsgList = (from msg in db.tblImMessages
                              group msg by new { msg.groupId } into tmp
                              join grp in db.tblImGroups
                              on tmp.Key.groupId equals grp.groupId
                              where myGroup.Contains(tmp.Key.groupId.Value)
                              select new ConversationModel
                              {
                                  type = 2,
                                  id = tmp.Key.groupId.Value,
                                  name = grp.groupName,
                                  headImage = string.IsNullOrEmpty(grp.headImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + grp.headImage,
                                  sendTime = tmp.Max(p => p.sendTime.Value)
                              }).ToList();

                list.AddRange(userMsgList);
                list.AddRange(grpMsgList);
            }

            return list.Where(p => p.sendTime >= startTime && p.sendTime <= endTime).OrderByDescending(p => p.sendTime).ToList();
        }

        #endregion 最近会话

        #region 组织架构操作

        /// <summary>
        /// 取得组织架构及其下用户信息
        /// </summary>
        /// <param name="parent">上级组织架构ID</param>
        /// <returns></returns>
        public List<OrgAndUserModel> GetOrgAndUserList(int? orgId)
        {
            var list = new List<OrgAndUserModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                // 获取组织架构
                var orgList = this.GetOrgListByOrgId(db, orgId);
                list.AddRange(orgList);

                if (orgId != null)
                {
                    // 获取人员列表
                    var userList = this.GetUserListByOrgId(db, orgId.Value);
                    list.AddRange(userList);
                }
            }

            return list;
        }

        #endregion 组织架构操作

        #region 消息处理

        /// <summary>
        /// 获取历史聊天记录
        /// </summary>
        /// <param name="type">1:个人 2：群组</param>
        /// <param name="userId">登陆用户</param>
        /// <param name="convId">用户/群组ID</param>
        /// <returns></returns>
        public List<HistoryMessageModel> GetHistoryMessage(MessageFilterModel filter, int userId)
        {
            var currentPage = filter.page == null || filter.page == 0 ? 1 : filter.page.Value;
            var list = new List<HistoryMessageModel>();

            var startTime = new DateTime();
            var endTime = new DateTime();

            if (!string.IsNullOrEmpty(filter.time))
            {
                startTime = DateTime.Parse(filter.time + " 00:00:00");
                endTime = DateTime.Parse(filter.time + " 23:59:59");
            }

            using (var db = new TargetNavigationDBEntities())
            {
                // 个人聊天记录
                if (filter.type == 1)
                {
                    list = (from msg in db.tblImMessages
                            join user in db.tblUser
                            on msg.fromUser equals user.userId
                            where msg.groupId == null && ((msg.fromUser == userId && msg.toUser == filter.id) || (msg.fromUser == filter.id && msg.toUser == userId))
                                    && (string.IsNullOrEmpty(filter.time) ? true : msg.sendTime >= startTime && msg.sendTime <= endTime)
                                    && (string.IsNullOrEmpty(filter.message) ? true : msg.message.IndexOf(filter.message) != -1)
                            orderby msg.sendTime descending
                            select new HistoryMessageModel
                            {
                                type = msg.type.Value,
                                userId = msg.fromUser,
                                userName = user.userName,
                                headImage = user.bigImage,
                                message = msg.message,
                                fileName = msg.fileName,
                                sendTime = msg.sendTime.Value
                            }).ToList();
                }
                else
                {
                    list = (from msg in db.tblImMessages
                            join user in db.tblUser
                            on msg.fromUser equals user.userId
                            where msg.groupId == filter.id
                                    && (string.IsNullOrEmpty(filter.time) ? true : msg.sendTime >= startTime && msg.sendTime <= endTime)
                                    && (string.IsNullOrEmpty(filter.message) ? true : msg.message.IndexOf(filter.message) != -1)
                                    && msg.toUser == userId
                            orderby msg.sendTime descending
                            select new HistoryMessageModel
                            {
                                type = msg.type.Value,
                                userId = msg.fromUser,
                                userName = user.userName,
                                headImage = user.bigImage,
                                message = msg.message,
                                fileName = msg.fileName,
                                sendTime = msg.sendTime.Value
                            }).ToList();
                }
            }

            list = list.Skip(10 * (currentPage - 1)).Take(10).OrderBy(p => p.sendTime).ToList();

            return list;
        }

        /// <summary>
        /// 更新消息状态为已读
        /// </summary>
        /// <param name="id">消息ID</param>
        public bool UpdateMessageStatus(string id)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImMessages.Where(p => p.messageId == id).FirstOrDefault();

                if (model == null)
                {
                    return false;
                }

                model.readFlag = 1;
                model.receiveTime = DateTime.Now;

                db.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 查询登录用户未读信息
        /// </summary>
        /// <param name="id">消息ID</param>
        public int GetMessage(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImMessages.Where(p => p.toUser == userId&&p.readFlag==0).ToList();

                if (model.Count()==0)
                {
                    return 0;
                }

                return model.Count();

               
            }
        }

        #endregion 消息处理

        #region 其他

        /// <summary>
        /// 获取各个项目的数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TypeCountModel GetCountByType(int userId)
        {
            TypeCountModel model = new TypeCountModel();

            // 取得常用联系人数
            model.contactCount = this.GetImContactList(userId).Count();

            // 取得群组数
            model.groupCount = this.GetImGroupList(userId).Count();

            // 用户数
            model.userCount = this.GetUserCount();

            // 最近会话数
            model.conversationCount = this.GetConversationList(userId).Count();

            return model;
        }

        #endregion 其他

        #region 私有方法

        /// <summary>
        /// 添加新的群组
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="model">群组信息</param>
        /// <param name="db">DB</param>
        private int AddNewGroup(int userId, ImGroupModel model, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var groupId = db.prcGetPrimaryKey("tblImGroups", obj).FirstOrDefault().Value;

            var group = new tblImGroups
            {
                groupId = groupId,
                groupName = model.groupName,
                headImage = model.headImage,
                type = model.type,
                createUser = userId,
                createTime = DateTime.Now
            };

            db.tblImGroups.Add(group);

            return groupId;
        }

        /// <summary>
        /// 添加群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">成员ID</param>
        /// <param name="db">DB</param>
        private void AddGroupUser(int groupId, int[] groupUser, int? userId, TargetNavigationDBEntities db)
        {
            foreach (var id in groupUser)
            {
                var model = db.tblImGroupUser.Where(p => p.groupId == groupId && p.userId == id).FirstOrDefault();

                if (model != null)
                {
                    continue;
                }

                model = new tblImGroupUser
                {
                    groupId = groupId,
                    userId = id
                };

                db.tblImGroupUser.Add(model);
            }

            if (userId == null)
            {
                return;
            }

            var myself = db.tblImGroupUser.Where(p => p.groupId == groupId && p.userId == userId).FirstOrDefault();

            if (myself == null)
            {
                myself = new tblImGroupUser
                {
                    groupId = groupId,
                    userId = userId.Value,
                    power = 1
                };

                db.tblImGroupUser.Add(myself);
            }
        }

        /// <summary>
        /// 获取组织架构列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        private List<OrgAndUserModel> GetOrgListByOrgId(TargetNavigationDBEntities db, int? orgId)
        {
            var orgList = (from o in db.tblOrganization
                           where o.parentOrganization == orgId && !o.deleteFlag
                           orderby o.orderNumber
                           select new OrgAndUserModel
                           {
                               id = o.organizationId,
                               name = o.organizationName,
                               isParent = o.withSub.Value,
                               type = 1
                           }).ToList();

            foreach (var item in orgList)
            {
                if (!item.isParent)
                {
                    var userList = this.GetUserListByOrgId(db, item.id);
                    if (userList.Count() > 0)
                    {
                        item.isParent = true;
                    }
                }
            }

            return orgList;
        }

        /// <summary>
        /// 根据组织Id获取人员列表
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="orgId">组织Id</param>
        /// <returns>人员列表</returns>
        private List<OrgAndUserModel> GetUserListByOrgId(TargetNavigationDBEntities db, int orgId)
        {
            string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();
            var userList = new List<OrgAndUserModel>();
            //获取该组织架构下面的职位
            var stations = db.tblStation.Where(p => p.organizationId == orgId).ToList<tblStation>();
            if (stations.Count > 0)
            {
                foreach (var station in stations)
                {
                    //获取该职位上的人员信息
                    var userStations = db.tblUserStation.Where(p => p.stationId == station.stationId);
                    foreach (var userStation in userStations)
                    {
                        var userModel = db.tblUser.Where(p => p.userId == userStation.userId && !p.deleteFlag && p.workStatus == 1).FirstOrDefault();
                        if (userModel != null)
                        {
                            var user = new OrgAndUserModel
                            {
                                id = userModel.userId,
                                name = userModel.userName,
                                isParent = false,
                                icon = string.IsNullOrEmpty(userModel.originalImage) ? "/Images/common/portrait.png" : "/" + configpath + "/" + userModel.bigImage,
                                type = 2
                            };

                            if (!userList.Exists(p => p.id == userModel.userId))
                            {
                                userList.Add(user);
                            }
                        }
                    }
                }
            }
            return userList;
        }

        /// <summary>
        /// 获取用户数
        /// </summary>
        /// <returns></returns>
        private int GetUserCount()
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var count = db.tblUser.Where(p => p.workStatus == 1 && !p.deleteFlag).ToList().Count();

                return count;
            }
        }

        /// <summary>
        /// 取得用户所在岗位
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string getUserStation(int userId, TargetNavigationDBEntities db)
        {
            var station = string.Empty;

            var stationList = (from us in db.tblUserStation
                               join s in db.tblStation
                                   on us.stationId equals s.stationId
                               where us.userId == userId
                               select s.stationName).ToArray();

            foreach (var item in stationList)
            {
                station = string.IsNullOrEmpty(station) ? item : station + "、" + item;
            }

            return station;
        }

        #endregion 私有方法
    }
}