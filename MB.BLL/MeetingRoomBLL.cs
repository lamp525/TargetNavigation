using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class MeetingRoomBLL : IMeetingRoomBLL
    {
        #region 获取会议室列表

        /// <summary>
        /// 获取会议室列表
        /// </summary>
        /// <returns>会议室列表</returns>
        public List<MeetingRoomModel> GetMeetingRoomList()
        {
            var meetingList = new List<MeetingRoomModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var roomList = (from m in db.tblMeetingRoom
                                select new RoomModel
                                {
                                    roomId = m.roomId,
                                    roomName = m.roomName,
                                    position = m.position,
                                    seating = m.seating,
                                    comment = m.comment
                                }).ToList();
                if (roomList.Count > 0)
                {
                    roomList.ForEach(p =>
                    {
                        var meetingModel = new MeetingRoomModel() { room = p };
                        meetingModel.equipment = (from mr in db.tblMeetingRoomEquipment
                                                  join e in db.tblEquipment
                                                  on mr.equipmentId equals e.equipmentId
                                                  where mr.roomId == p.roomId
                                                  select new EquipmentModel
                                                  {
                                                      equipmentId = e.equipmentId,
                                                      equipmentName = e.equipmentName
                                                  }).ToList();
                        meetingList.Add(meetingModel);
                    });
                }
            }
            return meetingList;
        }

        #endregion 获取会议室列表

        #region 获取会议室详情

        /// <summary>
        /// 获取会议室详情
        /// </summary>
        /// <param name="roomId">会议室Id</param>
        /// <returns>会议室详情</returns>
        public MeetingRoomModel GetMeetingRoomInfo(int roomId)
        {
            var meetingModel = new MeetingRoomModel();
            using (var db = new TargetNavigationDBEntities())
            {
                meetingModel.room = (from m in db.tblMeetingRoom
                                     where m.roomId == roomId
                                     select new RoomModel
                                     {
                                         roomId = m.roomId,
                                         roomName = m.roomName,
                                         position = m.position,
                                         seating = m.seating,
                                         comment = m.comment
                                     }).FirstOrDefault();
                if (meetingModel.room != null)
                {
                    meetingModel.equipment = (from mr in db.tblMeetingRoomEquipment
                                              join e in db.tblEquipment
                                              on mr.equipmentId equals e.equipmentId
                                              where mr.roomId == roomId
                                              select new EquipmentModel
                                              {
                                                  equipmentId = e.equipmentId,
                                                  equipmentName = e.equipmentName
                                              }).ToList();
                }
            }
            return meetingModel;
        }

        #endregion 获取会议室详情

        #region 取得我的预约列表

        /// <summary>
        /// 取得我的预约列表
        /// </summary>
        /// <param name="status">0：全部 1：未进行 2：已完成</param>
        /// <param name="userId">登陆者Id</param>
        /// <returns>我的预约列表</returns>
        public List<MeetingModel> GetMyAppointment(int status, int userId, DateTime start, DateTime end)
        {
            var appointmentList = new List<MeetingModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (status == (int)ConstVar.MeetingStatus.all)    //全部
                {
                    appointmentList = (from ma in db.tblMeetingRoomAppointment
                                       join m in db.tblMeetingRoom on ma.roomId equals m.roomId into group1
                                       from m in group1.DefaultIfEmpty()
                                       join r in db.tblMeetingMember on ma.meetingId equals r.meetingId into group2
                                       from r in group2.DefaultIfEmpty()
                                       where ma.createUser == userId || r.userId == userId
                                       select new MeetingModel
                                       {
                                           meetingId = ma.meetingId,
                                           roomId = m.roomId,
                                           roomName = m.roomName,
                                           startTime = ma.startTime,
                                           endTime = ma.endTime,
                                           content = ma.content,
                                           isComplete = ma.endTime < DateTime.Now ? true : false
                                       }).Where("startTime >=@0 And startTime <@1", start, end).Distinct().ToList();
                }
                else if (status == (int)ConstVar.MeetingStatus.unDone)   //未进行
                {
                    appointmentList = (from ma in db.tblMeetingRoomAppointment
                                       join m in db.tblMeetingRoom on ma.roomId equals m.roomId into group1
                                       from m in group1.DefaultIfEmpty()
                                       join r in db.tblMeetingMember on ma.meetingId equals r.meetingId into group2
                                       from r in group2.DefaultIfEmpty()
                                       where (ma.createUser == userId || r.userId == userId) && ma.startTime > DateTime.Now
                                       select new MeetingModel
                                       {
                                           meetingId = ma.meetingId,
                                           roomId = m.roomId,
                                           roomName = m.roomName,
                                           startTime = ma.startTime,
                                           endTime = ma.endTime,
                                           content = ma.content,
                                           isComplete = false
                                       }).Where("startTime >=@0 And startTime <@1", start, end).Distinct().ToList();
                }
                else if (status == (int)ConstVar.MeetingStatus.hasComplete)  //已完成
                {
                    appointmentList = (from ma in db.tblMeetingRoomAppointment
                                       join m in db.tblMeetingRoom on ma.roomId equals m.roomId into group1
                                       from m in group1.DefaultIfEmpty()
                                       join r in db.tblMeetingMember on ma.meetingId equals r.meetingId into group2
                                       from r in group2.DefaultIfEmpty()
                                       where (ma.createUser == userId || r.userId == userId) && ma.endTime <= DateTime.Now
                                       select new MeetingModel
                                       {
                                           meetingId = ma.meetingId,
                                           roomId = m.roomId,
                                           roomName = m.roomName,
                                           startTime = ma.startTime,
                                           endTime = ma.endTime,
                                           content = ma.content,
                                           isComplete = true
                                       }).Where("startTime >=@0 And startTime <@1", start, end).Distinct().ToList();
                }

                //统计参与会议的人数
                if (appointmentList.Count > 0)
                {
                    appointmentList.ForEach(p =>
                    {
                        p.member = db.tblMeetingMember.Where(a => a.meetingId == p.meetingId).Count();
                    });
                }
            }
            return appointmentList;
        }

        #endregion 取得我的预约列表

        #region 获取计划列表

        /// <summary>
        /// 获取计划列表
        /// </summary>
        /// <param name="meetingId">会议室Id</param>
        /// <returns>计划列表</returns>
        public List<PlanSimpleModel> GetPlanList(int meetingId)
        {
            var planList = new List<PlanSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                planList = (from p in db.tblPlan
                            join e in db.tblExecutionMode on p.executionModeId equals e.executionId into group1
                            from e in group1.DefaultIfEmpty()
                            join u in db.tblUser on p.responsibleUser equals u.userId into group2
                            from u in group2.DefaultIfEmpty()
                            join conUser in db.tblUser on p.confirmUser equals conUser.userId into group3
                            from conUser in group3.DefaultIfEmpty()
                            where !p.deleteFlag && p.meetingId == meetingId
                            select new PlanSimpleModel
                            {
                                planId = p.planId,
                                executionMode = e.executionMode,
                                eventOutput = p.eventOutput,
                                responsibleUser = u.userName,
                                smallImage = (u.bigImage == null || u.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + u.bigImage),
                                confirmUser = conUser.userName,
                                endTime = p.endTime,
                                createTime = p.createTime
                            }).ToList();
            }
            return planList;
        }

        #endregion 获取计划列表

        #region 会议室预约信息取得

        /// <summary>
        /// 会议室预约信息取得
        /// </summary>
        /// <param name="time">日期（年月日）</param>
        /// <returns></returns>
        public List<MeetingRoomModel> GetMeetingRoomAppointment(DateTime time)
        {
            var meetingList = new List<MeetingRoomModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var roomList = (from m in db.tblMeetingRoom
                                select new RoomModel
                                {
                                    roomId = m.roomId,
                                    roomName = m.roomName,
                                    position = m.position,
                                    seating = m.seating,
                                    comment = m.comment
                                }).ToList();
                if (roomList.Count > 0)
                {
                    roomList.ForEach(p =>
                    {
                        var meetingModel = new MeetingRoomModel() { room = p };
                        meetingModel.equipment = (from mr in db.tblMeetingRoomEquipment
                                                  join e in db.tblEquipment
                                                  on mr.equipmentId equals e.equipmentId
                                                  where mr.roomId == p.roomId
                                                  select new EquipmentModel
                                                  {
                                                      equipmentId = e.equipmentId,
                                                      equipmentName = e.equipmentName
                                                  }).ToList();
                        meetingModel.meeting = (from ma in db.tblMeetingRoomAppointment
                                                join u in db.tblUser on ma.createUser equals u.userId
                                                where ma.roomId == p.roomId && !u.deleteFlag && ((ma.startTime.Year == time.Year && ma.startTime.Month == time.Month
                                                && ma.startTime.Day == time.Day) || (ma.endTime.Year == time.Year && ma.endTime.Month == time.Month
                                                && ma.endTime.Day == time.Day))
                                                select new MeetingModel
                                                {
                                                    meetingId = ma.meetingId,
                                                    startTime = ma.startTime,
                                                    endTime = ma.endTime,
                                                    content = ma.content,
                                                    createUser = u.userName,
                                                    isHasAttachment = db.tblMeetingAttachment.Where(a => a.meetingId == ma.meetingId).Count() > 0 ? true : false
                                                }).ToList();
                        meetingList.Add(meetingModel);
                    });
                }
            }
            return meetingList;
        }

        #endregion 会议室预约信息取得

        #region 获取会议详情

        /// <summary>
        /// 获取会议详情
        /// </summary>
        /// <param name="meetingId">会议Id</param>
        /// <returns>会议详情</returns>
        public MeetingInfo GetMeetingInfo(int meetingId)
        {
            var meetingInfo = new MeetingInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                meetingInfo = (from mr in db.tblMeetingRoomAppointment
                               where mr.meetingId == meetingId
                               select new MeetingInfo
                               {
                                   meetingId = meetingId,
                                   startTime = mr.startTime,
                                   endTime = mr.endTime,
                                   content = mr.content
                               }).FirstOrDefault();
                if (meetingInfo != null)
                {
                    //获取主讲人信息
                    meetingInfo.speakerName = (from m in db.tblMeetingMember
                                               join u in db.tblUser on m.userId equals u.userId
                                               where !u.deleteFlag && m.meetingId == meetingId && m.type == (int)ConstVar.MeetingMember.speaker
                                               select u.userName).ToArray();
                    //获取参与人信息
                    meetingInfo.participantUser = (from m in db.tblMeetingMember
                                                   join u in db.tblUser on m.userId equals u.userId
                                                   where !u.deleteFlag && m.meetingId == meetingId && m.type == (int)ConstVar.MeetingMember.joining
                                                   select u.userName).ToArray();
                }
            }
            return meetingInfo;
        }

        #endregion 获取会议详情

        #region 获取预约详情

        /// <summary>
        /// 获取预约详情
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="roomId">会议室Id</param>
        /// <returns></returns>
        public List<MeetingModel> GetAppointmentDetail(DateTime time, int roomId)
        {
            var meetingList = new List<MeetingModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                meetingList = (from ma in db.tblMeetingRoomAppointment
                               join u in db.tblUser on ma.createUser equals u.userId
                               where ma.roomId == roomId && !u.deleteFlag && ((ma.startTime.Year == time.Year && ma.startTime.Month == time.Month
                               && ma.startTime.Day == time.Day) || (ma.endTime.Year == time.Year && ma.endTime.Month == time.Month
                               && ma.endTime.Day == time.Day))
                               select new MeetingModel
                               {
                                   meetingId = ma.meetingId,
                                   startTime = ma.startTime,
                                   endTime = ma.endTime,
                                   content = ma.content,
                                   createUser = u.userName,
                                   isHasAttachment = db.tblMeetingAttachment.Where(a => a.meetingId == ma.meetingId).Count() > 0 ? true : false
                               }).ToList();
            }
            return meetingList;
        }

        #endregion 获取预约详情

        #region 获取会议室预约时间列表

        /// <summary>
        /// 获取预约详情
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="roomId">会议室Id</param>
        /// <returns></returns>
        public List<MeetingModel> GetAppointmentTimeDetail(DateTime time, int roomId)
        {
            var meetingList = new List<MeetingModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                meetingList = (from ma in db.tblMeetingRoomAppointment
                               join u in db.tblUser on ma.createUser equals u.userId
                               where ma.roomId == roomId && !u.deleteFlag && ((ma.startTime.Year == time.Year && ma.startTime.Month == time.Month
                               && ma.startTime.Day == time.Day) || (ma.endTime.Year == time.Year && ma.endTime.Month == time.Month
                               && ma.endTime.Day == time.Day))
                               select new MeetingModel
                               {
                                   startTime = ma.startTime,
                                   endTime = ma.endTime
                               }).ToList();
            }
            return meetingList;
        }

        #endregion 获取会议室预约时间列表

        #region 删除会议室

        /// <summary>
        /// 删除会议室
        /// </summary>
        /// <param name="roomId">会议室Id</param>
        /// <returns>true：删除成功 false：删除失败</returns>
        public bool DeleteMeetingRoom(int roomId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //删除会议室信息
                var roomModel = this.GetMeetingRoomModel(db, roomId);
                if (roomModel == null) return false;
                db.tblMeetingRoom.Remove(roomModel);
                //删除会议室设备信息
                var equipmentList = db.tblMeetingRoomEquipment.Where(p => p.roomId == roomId);
                if (equipmentList.Count() > 0)
                {
                    db.tblMeetingRoomEquipment.RemoveRange(equipmentList);
                }
                //删除当前会议室的预约信息
                var appointmentList = db.tblMeetingRoomAppointment.Where(p => p.roomId == roomId);
                if (appointmentList.Count() > 0)
                {
                    db.tblMeetingRoomAppointment.RemoveRange(appointmentList);
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 删除会议室

        #region 取消会议室预约

        /// <summary>
        /// 取消会议室预约
        /// </summary>
        /// <param name="meetingId">会议Id</param>
        /// <returns>true:取消成功  false：取消失败</returns>
        public bool CancelAppointment(int meetingId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //删除会议室预约表
                var meetingModel = this.GetMeetingModel(db, meetingId);
                if (meetingModel == null) return false;
                db.tblMeetingRoomAppointment.Remove(meetingModel);
                //删除会议参与人员信息
                var meetingUserList = db.tblMeetingMember.Where(p => p.meetingId == meetingId);
                if (meetingUserList.Count() > 0)
                {
                    db.tblMeetingMember.RemoveRange(meetingUserList);
                }
                //删除会议附件信息
                var attList = db.tblMeetingAttachment.Where(p => p.meetingId == meetingId);
                if (attList.Count() > 0)
                {
                    db.tblMeetingAttachment.RemoveRange(attList);
                }
                //删除会议计划信息
                var planList = db.tblPlan.Where(p => p.meetingId == meetingId);
                if (planList.Count() > 0)
                {
                    db.tblPlan.RemoveRange(planList);
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 取消会议室预约

        #region 保存会议室信息

        /// <summary>
        /// 保存会议室信息
        /// </summary>
        /// <param name="meetingModel">会议室模型</param>
        /// <returns>true：保存成功 false：保存失败</returns>
        public int SaveMeetingRoom(MeetingRoomModel meetingModel)
        {
            var flag = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                if (meetingModel.room == null) return flag;
                var hasMeetingRoomName = db.tblMeetingRoom.Where(p => p.roomName == meetingModel.room.roomName).FirstOrDefault();

                if (meetingModel.room.roomId == null)   //新建
                {
                    if (hasMeetingRoomName != null)
                    {
                        flag = 2;
                    }
                    else
                    {
                        //会议室信息表中插入数据
                        var roomObj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        var roomId = db.prcGetPrimaryKey("tblMeetingRoom", roomObj).FirstOrDefault().Value;
                        flag = roomId;
                        var roomModel = new tblMeetingRoom
                        {
                            roomId = roomId,
                            roomName = meetingModel.room.roomName,
                            position = meetingModel.room.position,
                            seating = meetingModel.room.seating,
                            comment = meetingModel.room.comment
                        };
                        db.tblMeetingRoom.Add(roomModel);
                        //会议室设备表插入数据
                        this.AddMeetingEquipment(db, roomId, meetingModel.equipmentId);
                    }
                }
                else   //保存修改
                {
                    if (hasMeetingRoomName != null)
                    {
                        flag = 2;
                    }
                    else
                    {
                        //更新会议室信息表
                        var roomModel = this.GetMeetingRoomModel(db, meetingModel.room.roomId.Value);
                        if (roomModel == null) return flag;
                        roomModel.roomName = meetingModel.room.roomName;
                        roomModel.position = meetingModel.room.position;
                        roomModel.seating = meetingModel.room.seating;
                        roomModel.comment = meetingModel.room.comment;
                        //更新会议室设备表
                        var meetingEquipmentList = db.tblMeetingRoomEquipment.Where(p => p.roomId == meetingModel.room.roomId.Value);
                        if (meetingEquipmentList.Count() > 0)
                        {
                            db.tblMeetingRoomEquipment.RemoveRange(meetingEquipmentList);
                        }
                        //会议室设备表插入数据
                        this.AddMeetingEquipment(db, meetingModel.room.roomId.Value, meetingModel.equipmentId);
                    }
                }
                    db.SaveChanges();
                } 
            return flag;
        }

        #endregion 保存会议室信息

        #region 保存会议室预约记录

        /// <summary>
        /// 保存会议室预约记录
        /// </summary>
        /// <param name="userId">登录者用户</param>
        /// <returns></returns>
        public bool SaveAppointment(int userId, AddMeetingModel meetingModel)
        {
            if (meetingModel == null) return false;
            using (var db = new TargetNavigationDBEntities())
            {
                if (meetingModel.meetingId == null)   //新建
                {
                    //会议室信息表中插入数据
                    System.Data.Entity.Core.Objects.ObjectParameter roomObj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    meetingModel.meetingId = db.prcGetPrimaryKey("tblMeetingRoomAppointment", roomObj).FirstOrDefault().Value;

                    var meetingDbModel = new tblMeetingRoomAppointment
                    {
                        meetingId = meetingModel.meetingId.Value,
                        roomId = meetingModel.roomId,
                        startTime = meetingModel.startTime,
                        endTime = meetingModel.endTime,
                        content = meetingModel.content,
                        createUser = userId,
                        createTime = DateTime.Now
                    };
                    db.tblMeetingRoomAppointment.Add(meetingDbModel);
                    //添加参加会议人员信息
                    this.AddMeetingMember(db, meetingModel.meetingId.Value, meetingModel.speechUser, meetingModel.joinUser);
                    //添加会议附件信息
                    if (meetingModel.file.Count > 0)
                    {
                        meetingModel.file.ForEach(p =>
                        {
                            this.AddMeetingDoc(meetingModel.meetingId.Value, p, userId);
                        });
                    }
                }
                else              //更新保存
                {
                    //更新会议预约表
                    var meetingDbModel = this.GetMeetingModel(db, meetingModel.meetingId.Value);
                    if (meetingDbModel == null) return false;
                    meetingDbModel.roomId = meetingModel.roomId;
                    meetingDbModel.startTime = meetingModel.startTime;
                    meetingDbModel.endTime = meetingModel.endTime;
                    meetingDbModel.content = meetingModel.content;
                    meetingDbModel.createUser = userId;
                    meetingDbModel.createTime = DateTime.Now;
                    //保存参会人员信息,先删除旧数据
                    var oldmemberList = db.tblMeetingMember.Where(p => p.meetingId == meetingModel.meetingId.Value);
                    if (oldmemberList.Count() > 0)
                    {
                        db.tblMeetingMember.RemoveRange(oldmemberList);
                    }
                    //添加参加会议人员信息
                    this.AddMeetingMember(db, meetingModel.meetingId.Value, meetingModel.speechUser, meetingModel.joinUser);
                    var docList = db.tblMeetingAttachment.Where(p => !p.deleteFlag && p.meetingId == meetingModel.meetingId);
                    if (docList.Count() > 0)
                    {
                        foreach (var item in docList)
                        {
                            item.deleteFlag = true;
                        }
                        //添加会议附件信息
                        if (meetingModel.file.Count > 0)
                        {
                            meetingModel.file.ForEach(p =>
                            {
                                this.AddMeetingDoc(meetingModel.meetingId.Value, p, userId);
                            });
                        }
                    }
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 保存会议室预约记录

        #region 获取会议室设备列表

        /// <summary>
        /// 获取会议室设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public List<EquipmentModel> GetEquipmentList()
        {
            var equipmentList = new List<EquipmentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                equipmentList = (from e in db.tblEquipment
                                 select new EquipmentModel
                                 {
                                     equipmentId = e.equipmentId,
                                     equipmentName = e.equipmentName
                                 }).ToList();
            }
            return equipmentList;
        }

        #endregion 获取会议室设备列表

        #region 获取会议室名称列表

        /// <summary>
        /// 获取会议室名称列表
        /// </summary>
        /// <returns>会议室名称列表</returns>
        public List<RoomModel> GetSimpleRoomList()
        {
            var roomList = new List<RoomModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                roomList = (from r in db.tblMeetingRoom
                            select new RoomModel
                            {
                                roomId = r.roomId,
                                roomName = r.roomName
                            }).ToList();
            }
            return roomList;
        }

        #endregion 获取会议室名称列表

        #region 根据会议Id查询附件信息

        /// <summary>
        /// 根据会议Id查询附件信息
        /// </summary>
        /// <param name="meetingId">会议Id</param>
        /// <returns>附件集合</returns>
        public List<PlanAttachment> GetMeetingAttachmentList(int meetingId)
        {
            var meetingAttachmentList = new List<PlanAttachment>();
            using (var db = new TargetNavigationDBEntities())
            {
                meetingAttachmentList = (from c in db.tblMeetingAttachment
                                         where c.meetingId == meetingId
                                         select new PlanAttachment
                                         {
                                             attachmentId = c.attachmentId,
                                             attachmentName = c.displayName,
                                             saveName = c.saveName,
                                             extension = c.extension
                                         }).ToList();
            }
            return meetingAttachmentList;
        }

        #endregion 根据会议Id查询附件信息

        #region 私有方法

        #region 获取会议室数据库模型

        /// <summary>
        /// 获取会议室数据库模型
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="roomId">会议室Id</param>
        /// <returns>会议室数据库模型</returns>
        public tblMeetingRoom GetMeetingRoomModel(TargetNavigationDBEntities db, int roomId)
        {
            var roomModel = db.tblMeetingRoom.Where(p => p.roomId == roomId).FirstOrDefault();
            return roomModel;
        }

        #endregion 获取会议室数据库模型

        #region 获取预约会议数据库模型

        /// <summary>
        /// 获取预约会议数据库模型
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="meetingId">会议Id</param>
        /// <returns>预约会议数据库模型</returns>
        public tblMeetingRoomAppointment GetMeetingModel(TargetNavigationDBEntities db, int meetingId)
        {
            var meetingModel = db.tblMeetingRoomAppointment.Where(p => p.meetingId == meetingId).FirstOrDefault();
            return meetingModel;
        }

        #endregion 获取预约会议数据库模型

        #region 获取参加会议的用户信息

        /// <summary>
        /// 获取参加会议的用户信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="meetingId">会议室Id</param>
        /// <returns>用户列表</returns>
        public List<UserInfoSimpleModel> GetUserList(TargetNavigationDBEntities db, int meetingId)
        {
            var userList = (from m in db.tblMeetingMember
                            join u in db.tblUser on m.userId equals u.userId
                            where !u.deleteFlag && m.meetingId == meetingId
                            select new UserInfoSimpleModel
                            {
                                userId = m.userId,
                                userName = u.userName
                            }).ToList();
            return userList;
        }

        #endregion 获取参加会议的用户信息

        #region 附件保存到数据库

        /// <summary>
        /// 附件保存到数据库
        /// </summary>
        /// <param name="meetingId">会议Id</param>
        /// <param name="fileModel">文件模型</param>
        /// <param name="userId">用户Id</param>
        private void AddMeetingDoc(int meetingId, UploadFileModel fileModel, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var attachmentId = db.prcGetPrimaryKey("tblMeetingAttachment", obj).FirstOrDefault().Value;

                var documentModel = new tblMeetingAttachment
                {
                    attachmentId = attachmentId,
                    meetingId = meetingId,
                    displayName = fileModel.displayName,
                    saveName = fileModel.saveName,
                    extension = fileModel.extension,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblMeetingAttachment.Add(documentModel);
                db.SaveChanges();
            }
        }

        #endregion 附件保存到数据库

        #region 新增会议人员信息

        /// <summary>
        /// 新增会议人员信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="meetingId">会议Id</param>
        /// <param name="speechUser">主讲人</param>
        /// <param name="joinUser">参与人</param>
        private void AddMeetingMember(TargetNavigationDBEntities db, int meetingId, int[] speechUser, int[] joinUser)
        {
            if (speechUser.Length > 0)
            {
                foreach (var item in speechUser)
                {
                    var meetingMember = new tblMeetingMember
                    {
                        meetingId = meetingId,
                        userId = item,
                        type = (int)ConstVar.MeetingMember.speaker
                    };
                    db.tblMeetingMember.Add(meetingMember);
                }
            }
            if (joinUser.Length > 0)
            {
                foreach (var item in joinUser)
                {
                    var meetingMember = new tblMeetingMember
                    {
                        meetingId = meetingId,
                        userId = item,
                        type = (int)ConstVar.MeetingMember.joining
                    };
                    db.tblMeetingMember.Add(meetingMember);
                }
            }
        }

        #endregion 新增会议人员信息

        #region 添加会议室设备

        /// <summary>
        /// 添加会议室设备
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="roomId">会议室Id</param>
        /// <param name="equipmentId">设备Id集合</param>
        public void AddMeetingEquipment(TargetNavigationDBEntities db, int roomId, int?[] equipmentId)
        {
            if (equipmentId.Length > 0)
            {
                foreach (var item in equipmentId)
                {
                    var meetingEquipment = new tblMeetingRoomEquipment
                    {
                        roomId = roomId,
                        equipmentId = item.Value
                    };
                    db.tblMeetingRoomEquipment.Add(meetingEquipment);
                }
            }
        }

        #endregion 添加会议室设备

        #endregion 私有方法
    }
}