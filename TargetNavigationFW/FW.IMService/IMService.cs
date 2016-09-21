using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceProcess;
using MB.Model;
using Newtonsoft.Json;
using SuperWebSocket;

namespace FW.IMService
{
    public partial class IMService : ServiceBase
    {
        public IMService()
        {
            InitializeComponent();
        }

        private WebSocketServer server;

        private log4net.ILog mLog = log4net.LogManager.GetLogger("imService");

        #region 服务启动关闭处理

        protected override void OnStart(string[] args)
        {
            try
            {
                mLog.Info("WebSocket服务器开始启动");

                var ip = ConfigurationManager.AppSettings["APWebSocketIP"];
                var port = ConfigurationManager.AppSettings["APWebSocketPort"];

                // WebSocket服务器端启动
                server = new WebSocketServer();
                if (!server.Setup(ip, int.Parse(port)))
                {
                    //Debug.Write("WebSocket服务器端启动失败");
                    mLog.Error("WebSocket服务器端启动失败");
                    // 处理启动失败消息
                    return;
                }

                // 新的会话连接时
                server.NewSessionConnected += IMServer_NewSessionConnected;

                // 接收到新的消息时
                server.NewMessageReceived += IMServer_NewMessageReceived;

                // 会话关闭
                server.SessionClosed += IMServer_SessionClosed;

                if (!server.Start())
                {
                    mLog.Error(string.Format("开启WebSocket服务侦听失败:{0}:{1}", server.Config.Ip, server.Config.Port));
                    // 处理监听失败消息
                    return;
                }

                mLog.Info("WebSocket服务器启动成功");
            }
            catch (Exception ex)
            {
                mLog.Error(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            mLog.Info("WebSocket服务器开始关闭");
            // 服务关闭时处理
            if (server.State == SuperSocket.SocketBase.ServerState.Running)
            {
                server.Stop();
            }
            mLog.Info("WebSocket服务器关闭成功");
        }

        #endregion 服务启动关闭处理

        /// <summary>
        /// 新的会话链接时处理
        /// </summary>
        /// <param name="session">客户端</param>
        private void IMServer_NewSessionConnected(WebSocketSession session)
        {
            try
            {
                //获取用户ID
                int? userId = this.GetUserId(session);

                if (userId == null)
                {
                    mLog.Error("Session失效");
                    return;
                }

                mLog.Info(string.Format("用户[UserId:{0}]上线", userId));

                // 添加用户SessionID
                IMServiceBLL serverBll = new IMServiceBLL();
                serverBll.SaveUserSessionID(userId.Value, session.SessionID);

                var userModel = new UserModel();
                userModel.userId = userId.Value;

                var model = new ImMessageModel();
                // 消息类型 1：上线
                model.type = 1;
                model.sendUser = userModel;
                model.message = "上线";

                // 发送上线消息给所有用户
                this.SendMsgToAll(JsonConvert.SerializeObject(model));

                // 获取离线消息
                var msgList = serverBll.GetOfflineMessage(userId.Value);

                if (msgList != null && msgList.Count() > 0)
                {
                    // 发送消息
                    msgList.ForEach(p => session.Send(JsonConvert.SerializeObject(p)));
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 接收到新消息时处理
        /// </summary>
        /// <param name="session">客户端</param>
        /// <param name="value">接收到的信息</param>
        private void IMServer_NewMessageReceived(WebSocketSession session, string value)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ImMessageModel>(value);

                IMServiceBLL serverBll = new IMServiceBLL();

                foreach (var item in model.receiveUser)
                {
                    string messageId = Guid.NewGuid().ToString();
                    model.messageId = messageId;

                    // 保存消息到数据库
                    AddHandler handler = new AddHandler(serverBll.SaveMessage);
                    IAsyncResult result = handler.BeginInvoke(model, messageId, item.userId, new AsyncCallback(SetAsyncResult), "AsycState:OK");

                    if (item.userId == model.sendUser.userId)
                    {
                        continue;
                    }

                    // 取得SeesionID
                    var list = serverBll.GetUserSeesionId(item.userId);
                    foreach (var sessionId in list)
                    {
                        // 发送消息
                        this.SendMsgToRemotePoint(sessionId, JsonConvert.SerializeObject(model));
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex.ToString());
            }

            //this.SendMsgToAll(value);
        }

        /// <summary>
        /// 会话关闭时处理
        /// </summary>
        /// <param name="session">客户端</param>
        /// <param name="value">关闭原因</param>
        private void IMServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            try
            {
                // 获取用户ID
                int? userId = this.GetUserId(session);

                if (userId == null)
                {
                    mLog.Error("Session失效");
                    return;
                }

                mLog.Info(string.Format("用户[UserId:{0}]离线", userId));

                // 删除用户SessionID
                IMServiceBLL serverBll = new IMServiceBLL();
                serverBll.DeleteUserSessionID(session.SessionID);

                var userModel = new UserModel();
                userModel.userId = userId.Value;

                var model = new ImMessageModel();
                // 消息类型 2：下线
                model.type = 2;
                model.sendUser = userModel;
                model.message = "离线";

                // 发送消息给所有用户
                this.SendMsgToAll(JsonConvert.SerializeObject(model));
            }
            catch (Exception ex)
            {
                mLog.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private int? GetUserId(WebSocketSession session)
        {
            string userId = session.Path.TrimStart('/');

            if (!string.IsNullOrEmpty(userId))
            {
                return int.Parse(session.Path.TrimStart('/'));
            }

            return null;
        }

        /// <summary>
        /// 发送消息到指定客户端
        /// </summary>
        /// <param name="sessionId">客户端</param>
        /// <param name="msg">发送信息</param>
        private void SendMsgToRemotePoint(string sessionId, string msg)
        {
            var allSession = server.GetAppSessionByID(sessionId);
            if (allSession != null)
                allSession.Send(msg);
        }

        /// <summary>
        /// 发送消息到所有客户端
        /// </summary>
        /// <param name="msg">发送信息</param>
        private void SendMsgToAll(string msg)
        {
            foreach (var sendSession in server.GetAllSessions())
            {
                sendSession.Send(msg);
            }
        }

        private void SetAsyncResult(IAsyncResult result)
        {
            AddHandler handler = (AddHandler)((AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(result);
        }
    }
}