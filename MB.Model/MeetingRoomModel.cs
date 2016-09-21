using System.Collections.Generic;

namespace MB.Model
{
    //会议室列表模型
    public class MeetingRoomModel
    {
        /// <summary>会议室信息模型</summary>
        public RoomModel room { get; set; }

        /// <summary>会议室设备ID</summary>
        public int?[] equipmentId { get; set; }

        /// <summary>会议室设备集合</summary>
        public List<EquipmentModel> equipment { get; set; }

        public List<MeetingModel> meeting { get; set; }
    }

    public class RoomModel
    {
        /// <summary>自定义ID</summary>
        public int? roomId { get; set; }

        /// <summary>会议室名称</summary>
        public string roomName { get; set; }

        /// <summary>位置</summary>
        public string position { get; set; }

        /// <summary>座位数 </summary>
        public int? seating { get; set; }

        /// <summary>备注</summary>
        public string comment { get; set; }
    }

    //设备
    public class EquipmentModel
    {
        /// <summary>设备ID</summary>
        public int equipmentId { get; set; }

        /// <summary>设备名称</summary>
        public string equipmentName { get; set; }
    }
}