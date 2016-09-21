using MB.Web.Models;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;
using MB.Web.NotifyServiceReference;
using System.Threading.Tasks;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class IMMessageController : BaseController
    {
        private IIMMessageBLL IMMessageBLL { get; set; }

     

        //IMMessageBLL msgBll = new IMMessageBLL();

        //
        // GET: /IMMessage/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取常用联系人列表
        /// </summary>
        /// <returns></returns>
        public string GetImContactList()
        {
            // 获取常用联系人列表
            var list = IMMessageBLL.GetImContactList(int.Parse(Session["userId"].ToString()));

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 添加常用联系人
        /// </summary>
        /// <param name="contactsId">联系人ID</param>
        /// <returns></returns>
        public string AddImContact()
        {
            var contactsId = JsonConvert.DeserializeObject<int[]>(Request.Form["contactsId"]);
            // 添加 常用联系人
            IMMessageBLL.AddImContact(int.Parse(Session["userId"].ToString()), contactsId);

            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除常用联系人
        /// </summary>
        /// <param name="contactsId"></param>
        /// <returns></returns>
        public string DeleteImContact(int contactsId)
        {
            // 删除 常用联系人
            IMMessageBLL.DeleteImContact(int.Parse(Session["userId"].ToString()), contactsId);

            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取群组列表
        /// </summary>
        /// <returns></returns>
        public string GetImGroupList()
        {
            // 获取常用联系人列表
            var list = IMMessageBLL.GetImGroupList(int.Parse(Session["userId"].ToString()));

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns></returns>
        public string GetImGroupUser(int groupId)
        {
            // 获取常用联系人列表
            var list = IMMessageBLL.GetImGroupUser(groupId);

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 添加群组
        /// </summary>
        /// <returns></returns>
        public string AddImGroup()
        {
            var jsonData = Request.Form["data"];
            var groupModel = JsonConvert.DeserializeObject<ImGroupModel>(jsonData);

            // 添加群组
            IMMessageBLL.AddImGroup(groupModel, int.Parse(Session["userId"].ToString()));

            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        ///  添加群组成员
        /// </summary>
        /// <returns></returns>
        public string AddImGroupUser()
        {
            var jsonData = Request.Form["data"];
            var groupModel = JsonConvert.DeserializeObject<ImGroupModel>(jsonData);

            // 添加群组成员
            IMMessageBLL.AddImGroupUser(groupModel.groupId.Value, groupModel.groupUserId);

            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 退出群组
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns></returns>
        public string QuitGroup(int groupId, int? userId = null)
        {
            // 退出群组
            if (userId == null)
            {
                IMMessageBLL.QuitGroup(groupId, int.Parse(Session["userId"].ToString()));
            }
            else
            {
                IMMessageBLL.QuitGroup(groupId, userId.Value);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 设置群组管理员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">群组成员ID</param>
        /// <returns></returns>
        public string SetGroupManager(int groupId, int userId, int? power)
        {
            IMMessageBLL.SetGroupManager(groupId, userId, power);
            var jsonResult = new JsonResultModel(JsonResultType.success, AjaxCallBack.OK, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取最近会话列表
        /// </summary>
        /// <returns></returns>
        public string GetConversationList()
        {
            // 获取常用联系人列表
            var list = IMMessageBLL.GetConversationList(int.Parse(Session["userId"].ToString()));

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取聊天记录
        /// </summary>
        /// <returns></returns>
        public string GetHistoryMessage()
        {
            var jsonData = Request.Form["data"];
            var filter = JsonConvert.DeserializeObject<MessageFilterModel>(jsonData);

            // 添加群组
            var list = IMMessageBLL.GetHistoryMessage(filter, int.Parse(Session["userId"].ToString()));

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取组织架构及人员信息
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public string GetOrgAndUserList(int? parent)
        {
            var list = IMMessageBLL.GetOrgAndUserList(parent);

            var jsonResult = new JsonResultModel(JsonResultType.success, list, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="displayName">表示名</param>
        /// <param name="saveName">存储名</param>
        /// <param name="flag">
        /// 1、进行下载
        /// 0、不下载检测文件是否存在
        /// </param>
        /// <returns></returns>
        public ActionResult DownloadFile(string displayName, string saveName, int flag)
        {
            SharedBLL sharedBll = new SharedBLL();
            var userId = Convert.ToInt32(Session["userId"]);
            FileUpload file = new FileUpload();
            var downLoadPath = Path.Combine(file.ConfigPath(6), saveName);

            if (!System.IO.File.Exists(downLoadPath))
            {
                return JavaScript("noFile();");
            }
            if (flag == 0)
            {
                return Content("success");
            }

            return File(downLoadPath, "application/octet-stream", HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadFile(int type)
        {
            //新闻图片
            var uploadType = type == 1 ? ConstVar.UploadType.HeadImage : ConstVar.UploadType.ImFile;

            var fileModel = new UploadFileModel();
            var imgModel = new ImageInfoModel();
            //try
            //{
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                FileUpload upload = new FileUpload();
                fileModel = upload.UploadFile(hpf, (int)uploadType);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, fileModel, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 收消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ReceiveMessage(string id)
        {
            var result = IMMessageBLL.UpdateMessageStatus(id);

            var jsonResult = new JsonResultModel(JsonResultType.success, result ? AjaxCallBack.OK : AjaxCallBack.FAIL, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取各项目的数量
        /// </summary>
        /// <returns></returns>
        public string GetTypeCount()
        {
            var model = IMMessageBLL.GetCountByType(int.Parse(Session["userId"].ToString()));

            var jsonResult = new JsonResultModel(JsonResultType.success, model, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 查询登录用户未读信息
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            var userId = int.Parse(Session["userId"].ToString());
            var num = IMMessageBLL.GetMessage(userId);
            if (num != 0)
            {
                //发送即时提醒

                this.MBService(userId.ToString(), Protocol.ClientActualTimeProtocol.IUR);
                //using (var mbService = new MBServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + userId.ToString() + "+" + Protocol.ClientProtocol.IUR);
                //}

                //using (var mbService = new TaskRemindServiceClient())
                //{
                //    mbService.Send(Protocol.OperateProtocol.SIM + "+" + userId.ToString() + "+" + Protocol.ClientProtocol.IUR);
                //}

            }
            var jsonResult = new JsonResultModel(JsonResultType.success, num, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        #region 私有方法
        /// <summary>
        /// 发送及时提醒
        /// </summary>
        /// <param name="returnUserId">确认人或者责任人</param>
        /// <param name="userName">用户名字</param>
        /// <param name="protocol">客户端请求协议</param>
        /// <returns></returns>
        private async Task MBService(string returnUserId, string protocol, string userName = null)
        {
            //发送即时提醒
            using (var mbService = new TaskRemindServiceClient())
            {
                //如果
                if (string.IsNullOrEmpty(userName))
                {
                    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnUserId + "+" + protocol);
                }
                else
                {
                    mbService.Send(Protocol.OperateProtocol.SIM + "+" + returnUserId + "+" + userName + "+" + protocol);
                }

            }
        }
        #endregion
    }
}