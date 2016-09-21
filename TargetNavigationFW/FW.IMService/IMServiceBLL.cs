using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace FW.IMService
{
    public delegate int AddHandler(ImMessageModel model, string messageId, int userId);

    public class IMServiceBLL
    {
        /// <summary>
        /// 添加登陆用户SeesionID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="sessionId">sessionId</param>
        /// <returns></returns>
        public void SaveUserSessionID(int userId, string sessionId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblImLogin.Where(p => p.userId == userId).ToList();

                db.tblImLogin.RemoveRange(list);

                var model = new tblImLogin();

                model.sessionId = sessionId;
                model.userId = userId;

                db.tblImLogin.Add(model);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除用户SeesionID
        /// </summary>
        /// <param name="sessionId">sessionId</param>
        public void DeleteUserSessionID(string sessionId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImLogin.Where(p => p.sessionId == sessionId).FirstOrDefault();

                if (model != null)
                {
                    db.tblImLogin.Remove(model);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 保存消息到数据库
        /// </summary>
        public int SaveMessage(ImMessageModel model, string messgeId, int userId)
        {
            log4net.ILog mLog = log4net.LogManager.GetLogger("imService");

            try
            {
                if (model == null || model.receiveUser == null)
                {
                    return -1;
                }

                using (var db = new TargetNavigationDBEntities())
                {
                    var msgModel = new tblImMessages
                    {
                        messageId = messgeId,
                        type = model.type,
                        groupId = model.groupId,
                        message = model.message,
                        fileName = model.fileName,
                        fromUser = model.sendUser.userId,
                        toUser = userId,
                        readFlag = 0,
                        sendTime = DateTime.Now
                    };

                    if (model.sendUser.userId == userId)
                    {
                        msgModel.readFlag = 1;
                        msgModel.receiveTime = DateTime.Now;
                    }

                    db.tblImMessages.Add(msgModel);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex.ToString());
            }

            return 0;
        }

        /// <summary>
        /// 获取离线消息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<ImMessageModel> GetOfflineMessage(int userId)
        {
            var msgList = new List<ImMessageModel>();
            //string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            using (var db = new TargetNavigationDBEntities())
            {
                msgList = (from msg in db.tblImMessages
                           join user in db.tblUser
                           on msg.fromUser equals user.userId
                           where msg.toUser == userId && msg.readFlag == 0
                           select new ImMessageModel
                           {
                               messageId = msg.messageId,
                               type = msg.type.Value,
                               groupId = msg.groupId,
                               sendUser = new UserModel
                               {
                                   userId = msg.fromUser,
                                   userName = user.userName,
                                   headImage = user.bigImage
                               },
                               message = msg.message,
                               fileName = msg.fileName,
                               sendTime = msg.sendTime.Value
                           }).ToList();
            }

            return msgList;
        }

        public string[] GetUserSeesionId(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var list = (from login in db.tblImLogin
                            where login.userId == userId
                            select login.sessionId).ToArray();

                return list;
            }
        }
    }
}