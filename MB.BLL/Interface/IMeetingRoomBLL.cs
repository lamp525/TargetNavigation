using System;
using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IMeetingRoomBLL
    {
        /// 获取会议室列表
        List<MeetingRoomModel> GetMeetingRoomList();

        /// 获取会议室详情
        MeetingRoomModel GetMeetingRoomInfo(int roomId);

        /// 取得我的预约列表
        List<MeetingModel> GetMyAppointment(int status, int userId, DateTime start, DateTime end);

        /// 获取计划列表
        List<PlanSimpleModel> GetPlanList(int meetingId);

        /// 会议室预约信息取得
        List<MeetingRoomModel> GetMeetingRoomAppointment(DateTime time);

        /// 获取会议详情
        MeetingInfo GetMeetingInfo(int meetingId);

        /// 获取预约详情
        List<MeetingModel> GetAppointmentDetail(DateTime time, int roomId);

        /// 获取会议室预约时间列表
        List<MeetingModel> GetAppointmentTimeDetail(DateTime time, int roomId);

        /// 删除会议室
        bool DeleteMeetingRoom(int roomId);

        /// 取消会议室预约
        bool CancelAppointment(int meetingId);

        /// 保存会议室信息
        int SaveMeetingRoom(MeetingRoomModel meetingModel);

        /// 保存会议室预约记录
        bool SaveAppointment(int userId, AddMeetingModel meetingModel);

        /// 获取会议室设备列表
        List<EquipmentModel> GetEquipmentList();

        /// 获取会议室名称列表
        List<RoomModel> GetSimpleRoomList();

        /// 根据会议Id查询附件信息
        List<PlanAttachment> GetMeetingAttachmentList(int meetingId);
    }
}