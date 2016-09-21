using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class MeetingRoomController : BaseController
    {
        //
        // GET: /MeetingRoom/
        private IMeetingRoomBLL MeetingRoomBLL { get; set; }

 

        //private MeetingRoomBLL meetingBll = new MeetingRoomBLL();

        public ActionResult MeetingRoom()
        {
            return View();
        }

        public ActionResult MeetManage()
        {
            return View("MeetManage");
        }

        public ActionResult MyAppoint()
        {
            return View("MyAppoint");
        }

        //获取我的预约列表
        public string GetMyAppointment()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJson(JsonResultType.error, null, "获取列表失败");
            var conditionModel = JsonConvert.DeserializeObject<MyAppointmentCondition>(dataJson);
            //筛选的开始时间
            var conditionStart = conditionModel.start == null ? DateTime.MinValue : conditionModel.start.Value;
            //筛选的结束时间
            var conditionEnd = conditionModel.end == null ? DateTime.MaxValue : conditionModel.end.Value.AddDays(1).Date;
            var myAppointmentList = MeetingRoomBLL.GetMyAppointment(conditionModel.status, Convert.ToInt32(Session["userId"]), conditionStart, conditionEnd);
            return ReturnJson(JsonResultType.success, myAppointmentList, "获取列表失败");
        }

        //获取会议室列表
        public string GetMeetingRoomList()
        {
            var meetingRoomList = MeetingRoomBLL.GetMeetingRoomList();
            return ReturnJson(JsonResultType.success, meetingRoomList, "获取列表失败");
        }

        //获取会议室详情
        public string GetMeetingRoomInfo(int roomId)
        {
            var roomInfo = MeetingRoomBLL.GetMeetingRoomInfo(roomId);
            return ReturnJson(JsonResultType.success, roomInfo, "获取会议室详情失败");
        }

        //获取会议计划列表
        public string GetPlanList(int meetingId)
        {
            var planList = MeetingRoomBLL.GetPlanList(meetingId);
            return ReturnJson(JsonResultType.success, planList, "获取会议计划列表失败");
        }

        //获取会议室预约信息
        public string GetMeetingRoomAppointment(DateTime time)
        {
            var roomAppointmentList = MeetingRoomBLL.GetMeetingRoomAppointment(time);
            return ReturnJson(JsonResultType.success, roomAppointmentList, "获取会议室预约信息失败");
        }

        //获取会议详情
        public string GetMeetingInfo(int meetingId)
        {
            var meetingModel = MeetingRoomBLL.GetMeetingInfo(meetingId);
            return ReturnJson(JsonResultType.success, meetingModel, "获取会议室预约信息失败");
        }

        //获取预约详情
        public string GetAppointmentDetail(DateTime time, int roomId)
        {
            var appointmentDetail = MeetingRoomBLL.GetAppointmentDetail(time, roomId);
            return ReturnJson(JsonResultType.success, appointmentDetail, "获取预约详情失败");
        }

        //获取会议室预约列表
        public string GetAppointmentTimeDetail(DateTime time, int roomId)
        {
            var appointmentDetail = MeetingRoomBLL.GetAppointmentTimeDetail(time, roomId);
            return ReturnJson(JsonResultType.success, appointmentDetail, "获取预约详情失败");
        }

        //删除会议室信息
        public string DeleteMeetingRoom(int roomId)
        {
            var flag = MeetingRoomBLL.DeleteMeetingRoom(roomId);
            return ReturnJson(JsonResultType.success, flag, "删除失败");
        }

        //取消会议室预约
        public string CancelAppointment(int meetingId)
        {
            var flag = MeetingRoomBLL.CancelAppointment(meetingId);
            return ReturnJson(JsonResultType.success, flag, "取消预约失败");
        }

        //保存会议室信息
        public string SaveMeetingRoom()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) ReturnJson(JsonResultType.error, null, "保存失败");
            var meetingModel = JsonConvert.DeserializeObject<MeetingRoomModel>(dataJson);
            var flag = MeetingRoomBLL.SaveMeetingRoom(meetingModel);
            return ReturnJson(JsonResultType.success, flag, "保存失败");
        }

        //保存会议室预约记录
        public string SaveAppointment()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) ReturnJson(JsonResultType.error, null, "保存失败");
            var meetingModel = JsonConvert.DeserializeObject<AddMeetingModel>(dataJson);
            var flag = MeetingRoomBLL.SaveAppointment(Convert.ToInt32(Session["userId"]), meetingModel);
            return ReturnJson(JsonResultType.success, flag, "保存失败");
        }

        //获取会议室设备列表
        public string GetEquipmentList()
        {
            var equipmentList = MeetingRoomBLL.GetEquipmentList();
            return ReturnJson(JsonResultType.success, equipmentList, "获取设备列表失败");
        }

        //获取会议室名称列表
        public string GetSimpleRoomList()
        {
            var roomList = MeetingRoomBLL.GetSimpleRoomList();
            return ReturnJson(JsonResultType.success, roomList, "获取会议室列表失败");
        }

        //获取会议室附件列表
        public string GetMeetingAttachmentList(int meetingId)
        {
            var attachmentList = MeetingRoomBLL.GetMeetingAttachmentList(meetingId);
            return ReturnJson(JsonResultType.success, attachmentList, "获取会议室列表失败");
        }

        //单文件下载:flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult Download(string displayName, string saveName, int flag)
        {
            var downLoadPath = Path.Combine(new FileUpload().ConfigPath(Convert.ToInt32(ConstVar.UploadType.MeetingFile)), saveName);
            if (!System.IO.File.Exists(downLoadPath))
            {
                return Content("fail");
            }
            if (flag == 0)
            {
                return Content("success");
            }
            return File(downLoadPath, "application/octet-stream", HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8));
        }

        //全部下载 flag  1、进行下载  0、不下载检测文件是否存在
        public ActionResult MultiDownload(int meetingId, int flag)
        {
            List<CompressInfo> lstFile = new List<CompressInfo>();
            var meetingAttachmentList = MeetingRoomBLL.GetMeetingAttachmentList(meetingId);
            var fileExists = false;
            var fileName = string.Empty;

            foreach (var item in meetingAttachmentList)
            {
                var downLoadPath = Path.Combine(FilePath.MeetingUploadPath, item.saveName);
                if (!fileExists && System.IO.File.Exists(downLoadPath))
                {
                    fileName = item.attachmentName;
                    fileExists = true;
                }
                lstFile.Add(new CompressInfo { path = downLoadPath, display = item.attachmentName });
            }
            if (!fileExists) return Content("fail");
            if (flag == 0) return Content("success");
            SharpZipLibrary sharpZip = new SharpZipLibrary();
            return File(sharpZip.Compress(lstFile), "application/octet-stream", HttpUtility.UrlEncode(string.Format("{0}等.zip", fileName), System.Text.Encoding.UTF8));
        }

        //上传目标文档
        [HttpPost]
        public string UploadDocument()
        {
            var uploadType = ConstVar.UploadType.MeetingFile;
            var fileModel = new UploadFileModel();
            var userId = Convert.ToInt32(Session["userId"]);
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                FileUpload upload = new FileUpload();
                fileModel = upload.UploadFile(hpf, (int)uploadType);
                //上传成功后插入表数据
                //meetingBll.InsertObjectiveDoc(meetingId, fileModel, userId);
            }
            var jsonResult = new JsonResultModel(JsonResultType.success, fileModel, "上传文档失败");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //返回JSON串
        public string ReturnJson(JsonResultType type, object content, string errorMsg)
        {
            var jsonResult = new JsonResultModel(type, content, errorMsg);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}