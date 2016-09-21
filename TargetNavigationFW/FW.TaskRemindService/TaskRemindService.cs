using FW.TaskRemindService.Common;
using FW.TaskRemindService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FW.TaskRemindService
{
    public class TaskRemindService:ITaskRemindService
    {
        #region 变量区域
        //通知一个或多个正在等待的线程已发生事件。
        ManualResetEvent manager = new ManualResetEvent(false);
        private Socket socketServer;
        //private string clientAddress = string.Empty;
        //private static Socket clientServer = null;
        private static Dictionary<int, Socket> clientList = new Dictionary<int, Socket>();
        private TaskRemindBLL taskRemindBll = new TaskRemindBLL();
        private static List<ThreadModel> threadList = new List<ThreadModel>();
        private delegate void AddHandler(Socket client);
        private Byte[] oldBytes = new byte[1024 * 1024];
        #endregion

        #region 启动服务 void  CreateSocketService()
        /// <summary>
        /// 启动服务
        /// </summary>
        public void CreateSocketService()
        {
            try
            {
                threadList.Clear();
                clientList.Clear();
                var ip = ConfigurationManager.AppSettings["ServiceAddress"];
                var port = ConfigurationManager.AppSettings["ServicePost"];
                //获取IP
                var address = IPAddress.Parse(ip);
                //创建一个包含ip port 的网络节点对象
                var ipPoint = new IPEndPoint(address, int.Parse(port));
                //创建一个套接字socket,参数(IP4寻址协议,流式连接,使用TCP协议传输数据)
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //将负责监听的套接字绑定到唯一的IP和端口上                 
                socketServer.Bind(ipPoint);
                //设置监听队列的长度，同时100个队列
                socketServer.Listen(100);
                //线程开始监听客户端的请求
                var threadService = new Thread(StartSocketService);
                //设置线程为后台线程
                threadService.IsBackground = true;
                //启动线程
                threadService.Start();
                //开启检测心跳包线程
                // StartHeartBeat();
            }
            catch (SocketException ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), " void CreateSocketService():" + ex.ToString());
            }
        }
        #endregion

        #region 启动服务监听  StartSocketService()
        /// <summary>
        /// 启动服务监听
        /// </summary>
        private void StartSocketService()
        {
            while (true)
            {
                try
                {
                    //将事件状态设置为非终止状态，导致线程阻止
                    manager.Reset();
                    //开始监听客户端的连接请求
                    var args = new SocketAsyncEventArgs();
                    args.Completed += args_Completed;
                    socketServer.AcceptAsync(args);
                    //WaitHandle receives a signal.'>阻止当前线程，直到当前 WaitHandle 收到信号。
                    manager.WaitOne();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(typeof(TaskRemindService), " void StartSocketService():" + ex.ToString());
                    ServerExOperate();
                }
            }
        }
        #endregion

        #region 监听完成客户端的请求 void args_Completed(object sender, SocketAsyncEventArgs e)
        /// <summary>
        /// 监听完成客户端的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void args_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                //监听完成客户端的请求,一但监听到返回新的套接字
                var clientSocket = e.AcceptSocket;

                if (clientSocket == null) return;

                //启动线程获取客户端发来的消息
                //这部分为接收消息
                //var t = new Thread(GetClientMsg);
                //设置线程为后台线程
                // t.IsBackground = true;
                //启动线程
                //t.Start(clientSocket);
                //添加socket线程关联
                //threadList.Add(t.ManagedThreadId,clientSocket);
                AddHandler handler = new AddHandler(GetClientMsg);
                handler.BeginInvoke(clientSocket, null, null);
                //将事件状态设置为终止状态，允许一个或多个等待线程继续
                manager.Set();
            }
            catch (SocketException ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void args_Completed(object sender, SocketAsyncEventArgs e):" + ex.ToString());
                ServerExOperate();
            }
        }
        #endregion

        #region 获取客户端信息后暂停当前监听客户端的线程 void GetClientMsg(object socket)
        /// <summary>
        /// 获取客户端信息后暂停当前监听客户端的线程
        /// </summary>
        /// <param name="socket"></param>
        private void GetClientMsg(object socket)
        {
            var socketClient = socket as Socket;
            if (socketClient == null) return;
            //客户端线程加入列表
            var reviceManager = new ManualResetEvent(false);
            var thisClientThread = Thread.CurrentThread.ManagedThreadId;
            var threadModel = this.GetCurrendThread(socketClient, thisClientThread);
            if (threadModel != null)
            {
                threadList.Remove(threadModel);
            }
            threadList.Add(new ThreadModel
            {
                threadId = thisClientThread,
                socketClient = socketClient,
                reviceManager = reviceManager
            });
            while (true)
            {
                try
                {
                    var thisThreadId = Thread.CurrentThread.ManagedThreadId;
                    var thisThreadModel = this.GetCurrendThread(socketClient, thisThreadId);
                    if (thisThreadModel != null && thisThreadModel.socketClient == null)  //表示该线程所监听的客户端已经下线，跳出循环以释放该线程
                    {
                        threadList.Remove(thisThreadModel);
                        break;
                    }
                    thisThreadModel.reviceManager.Reset();
                    var receiveArgs = new SocketAsyncEventArgs();
                    var bytes = oldBytes;
                    //设置缓冲区
                    receiveArgs.SetBuffer(bytes, 0, bytes.Length);
                    receiveArgs.Completed += ReceiveArgs_Completed;
                    //开始异步接收
                    socketClient.ReceiveAsync(receiveArgs);
                    thisThreadModel.reviceManager.WaitOne();
                }
                catch (SocketException ex)
                {
                    LogHelper.WriteLog(typeof(TaskRemindService), "void GetClientMsg(object socket):" + ex.ToString());
                    ServerExOperate();
                }
            }

        }
        #endregion

        #region 完成客户端信息接收  ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        /// <summary>
        /// 完成客户端信息接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            var socketClient = sender as Socket;
            var thisThreadModel = threadList.Where(p => p.socketClient == socketClient).FirstOrDefault();
            if (thisThreadModel == null) return;
            try
            {
                var bytes = e.Buffer;
                var message = System.Text.Encoding.UTF8.GetString(bytes, 0, e.BytesTransferred).Trim();
                //获取正确信息
                var rightMsg = message.Split('|')[0];
                //用户下线协议
                if (string.IsNullOrWhiteSpace(rightMsg) || rightMsg == Protocol.OperateProtocol.UDL)
                {
                    ClientOutLine(socketClient);
                }
                //客户端连接正常
                else
                {
                    var operateArry = rightMsg.Split('+');
                    if (operateArry.Length >= 2)
                    {
                        //用户上线("UOL+用户名+密码")
                        if (operateArry.Length > 2 && operateArry[0] == Protocol.OperateProtocol.UOL && operateArry[1] != null && operateArry[2] != null)
                        {
                            //登录验证
                            var userId = taskRemindBll.UserLogin(operateArry[1], operateArry[2]);
                            //登录成功
                            if (userId > 0)
                            {
                                var thisClient = clientList.Where(p => p.Key == userId).FirstOrDefault();
                                //异常登陆
                                if (thisClient.Value != null)
                                {
                                    var msg = Protocol.OperateProtocol.URL;
                                    this.Send(socketClient, msg);
                                    ClientOutLine(thisClient.Value);
                                    clientList.Add(userId, socketClient);
                                    //发送客户端当天计划信息
                                    this.GetFirstLoginMsg(socketClient, userId);
                                }
                                else  //正常登录
                                {
                                    if (thisClient.Key > 0)
                                    {
                                        clientList.Remove(thisClient.Key);
                                    }
                                    clientList.Add(userId, socketClient);
                                    //发送客户端当天计划信息
                                    this.GetFirstLoginMsg(socketClient, userId);
                                }
                            }
                            else   //登录失败
                            {
                                var msg = Protocol.OperateProtocol.UDL;
                                this.Send(socketClient, msg);
                                //删除客户端
                                socketDispose(socketClient);
                            }
                        }
                        //17:00的请求协议
                        else if (operateArry[0] == Protocol.OperateProtocol.OWR)
                        {
                            var userId = this.GetRightUserId(operateArry[1]);
                            if (userId != 0)
                            {
                                var sendMsg = taskRemindBll.GetClientBeforeOLMsg(userId);
                                if (!string.IsNullOrWhiteSpace(sendMsg)) this.Send(socketClient,sendMsg);
                            }
                        }
                        //定时随机请求协议
                        else if (operateArry[0] == Protocol.OperateProtocol.CAM)
                        {
                            var userId = int.Parse(operateArry[1]);
                            var type = int.Parse(operateArry[2]);
                            if (userId != 0)
                            {
                                var sendMsg = taskRemindBll.GetClientRandomInfo(userId, type);
                                if (!string.IsNullOrWhiteSpace(sendMsg))
                                {
                                    this.Send(socketClient, sendMsg);
                                }
                            }
                        }
                        //断线重连协议
                        //else if (operateArry[0] == Protocol.OperateProtocol.OLA)
                        //{
                        //    var userId = this.GetRightUserId(operateArry[1]);
                        //    var clientModel = clientList.Where(p => p.Key == userId).FirstOrDefault();
                        //    if (clientModel.Value != null)
                        //    {
                        //        clientList.Remove(clientModel.Key);
                        //    }
                        //    clientList.Add(userId, socketClient);
                        //    //发送用户昨日统计信息
                        //    this.GetFirstLoginMsg(socketClient,userId);
                        //}
                        //版本检查
                        else if (operateArry[0] == Protocol.OperateProtocol.CVU)
                        {
                            var clientVersionNum = operateArry[2].Trim();
                            var serviceVersionNum = taskRemindBll.GetNewestVersionNum();
                            if (!string.IsNullOrWhiteSpace(clientVersionNum) && !string.IsNullOrWhiteSpace(serviceVersionNum))
                            {
                                //客户端不是最新版本，提示更新
                                if (int.Parse(clientVersionNum.Replace(".", "")) < int.Parse(serviceVersionNum.Replace(".", "")))
                                {
                                    var userId = this.GetRightUserId(operateArry[1]);
                                    if (userId != 0)
                                    {
                                        var sendMsg = Protocol.OperateProtocol.CVU;
                                        this.Send(socketClient, sendMsg);
                                    }
                                }
                            }
                        }
                    }
                }
                thisThreadModel.reviceManager.Set();

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void receiveArgs_Completed(object sender, SocketAsyncEventArgs e):" + ex.ToString());
                ClientOutLine(socketClient);
                thisThreadModel.reviceManager.Set();
            }
        }
        #endregion

        #region 即时提醒发送数据到客户端 void Send(string msg)
        /// <summary>
        /// 即时提醒发送数据到客户端
        /// <param name="msg">向客户端发送的信息</param>
        /// </summary>
        public void Send(string msg)
        {
            Thread t = new Thread(SendReal);
            t.IsBackground = true;
            t.Start(msg);
        }
        #endregion

        #region 发送数据到客户端 void SendReal(string msg)
        /// <summary>
        /// 发送数据到客户端
        /// <param name="msg">向客户端发送的信息</param>
        /// </summary>
        public void SendReal(object msg)
        {
            var sendMsg = msg.ToString();
            if (string.IsNullOrWhiteSpace(sendMsg)) return;
            var clientArry = sendMsg.Split('+');
            if (clientArry.Length < 2) return;
            //如果是计划待确认提醒,延时3分钟
            if (clientArry.Length>3&&clientArry[3]==Protocol.ClientActualTimeProtocol.PUA)  
            {
                System.Threading.Thread.Sleep(180000);
            }
            var userId = 0;
            if (int.TryParse(clientArry[1], out userId))
            {
                var clientSocket = clientList.Where(p => p.Key == userId).FirstOrDefault().Value;
                if (clientSocket == null) return;

                try
                {
                    //客户端向服务器发送消息
                    byte[] b = System.Text.Encoding.UTF8.GetBytes(sendMsg);
                    var sendArgs = new SocketAsyncEventArgs();
                    sendArgs.SetBuffer(b, 0, b.Length);
                    clientSocket.SendAsync(sendArgs);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(typeof(TaskRemindService), "void SendReal(string msg):" + ex.ToString());
                    ClientOutLine(clientSocket);
                }
            }
        }
        #endregion

        #region 发送数据到客户端 void Send(string msg)
        /// <summary>
        /// 发送数据到客户端
        /// <param name="socketClient">客户端socket</param>
        /// <param name="msg">向客户端发送的信息</param>
        /// </summary>
        public void Send(Socket socketClient, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg) || socketClient == null) return;
            try
            {
                //客户端向服务器发送消息
                byte[] b = System.Text.Encoding.UTF8.GetBytes(msg);
                var sendArgs = new SocketAsyncEventArgs();
                sendArgs.SetBuffer(b, 0, b.Length);
                socketClient.SendAsync(sendArgs);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void Send(string msg):" + ex.ToString());
                ClientOutLine(socketClient);
            }
        }
        #endregion

        #region 服务器断线处理
        /// <summary>
        /// 服务器断线处理
        /// </summary>
        //private void ServiceOutLine()
        //{
        //    if (clientList.Count() > 0)
        //    {
        //        foreach (var item in clientList.Keys)
        //        {
        //            var msg = this.AssemSendMsg(new string[] { Protocol.OperateProtocol.SDL, item.ToString() });
        //            this.Send(msg);
        //        }
        //    }
        //}
        #endregion

        #region 客户端下线的处理
        /// <summary>
        /// 客户端下线的处理
        /// </summary>
        public void ClientOutLine(Socket client)
        {
            try
            {
                lock (clientList)
                {
                    if (client != null)
                    {
                        //客户端列表中删除该客户端
                        var clientModel = clientList.Where(p => p.Value == client).FirstOrDefault();
                        if (clientModel.Value != null)
                        {
                            var thisThreadModel = this.GetCurrendThread(client);
                            if (thisThreadModel != null) thisThreadModel.socketClient = null;
                            clientList.Remove(clientModel.Key);
                            client.Shutdown(SocketShutdown.Both);
                            client.Dispose();
                            client.Close();
                        }

                    }
                }
            }
            catch (SocketException ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void ClientOutLine(Socket client):" + ex.ToString());
                ServerExOperate();
            }
        }
        #endregion

        #region 释放客户端资源
        /// <summary>
        /// 释放客户端资源
        /// </summary>
        /// <param name="client"></param>
        private void socketDispose(Socket client)
        {
            try
            {
                var threadModel = this.GetCurrendThread(client);
                if (threadModel != null)
                {
                    threadModel.socketClient = null;
                }
                client.Shutdown(SocketShutdown.Both);
                client.Dispose();
                client.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void socketDispose(Socket client):" + ex.ToString());
            }
        }
        #endregion

        #region 提取正确的用户Id
        /// <summary>
        /// 提取正确的用户Id
        /// </summary>
        /// <param name="msg">包含用户Id的字符串</param>
        /// <returns>用户Id</returns>
        public int GetRightUserId(string msg)
        {
            var userId = string.Empty;
            foreach (var item in msg)
            {
                if (item >= 48 && item <= 58)
                {
                    userId += item;
                }
                else
                {
                    break;
                }
            }
            return string.IsNullOrWhiteSpace(userId) ? 0 : int.Parse(userId);
        }
        #endregion

        #region 服务器异常处理
        /// <summary>
        /// 服务器异常处理
        /// </summary>
        private void ServerExOperate()
        {
            try
            {
                if (socketServer != null)
                {
                    manager.Reset();
                    //reviceManager.Reset();
                    socketServer.Shutdown(SocketShutdown.Both);
                    socketServer.Close();
                    socketServer.Dispose();
                    socketServer = null;
                    CreateSocketService();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void ServerExOperate():" + ex.ToString());
            }
        }
        #endregion

        #region 拼接发送包
        /// <summary>
        /// 拼接发送包
        /// </summary>
        /// <param name="msg">信息集合</param>
        /// <returns></returns>
        private string AssemSendMsg(string[] msg)
        {
            try
            {
                var sendMsg = new StringBuilder();
                if (msg.Length > 0)
                {
                    foreach (var item in msg)
                    {
                        if (sendMsg.Length > 0)
                        {
                            sendMsg.Append("+");
                        }
                        sendMsg.Append(item);
                    }
                }
                return sendMsg.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "string AssemSendMsg(string[] msg):" + ex.ToString());
                return string.Empty;
            }
        }
        #endregion

        #region 发送客户端首次登录的信息
        /// <summary>
        /// 发送客户端首次登录的信息
        /// </summary>
        /// <param name="socketClient">客户端socket</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        private void GetFirstLoginMsg(Socket socketClient, int userId)
        {
            try
            {
                //发送用户昨日统计信息
                var sendMsg = taskRemindBll.GetClientFirstLoginInfo(userId);
                this.Send(socketClient, sendMsg);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(TaskRemindService), "void SendYesterdayInfo(int userId):" + ex.ToString());
            }
        }
        #endregion

        #region 获取当前线程对象
        /// <summary>
        /// 获取当前线程对象
        /// </summary>
        /// <param name="client">socket对象</param>
        /// <returns></returns>
        private ThreadModel GetCurrendThread(Socket client, int threadId = -1)
        {
            return threadId == -1 ? threadList.Where(p => p.socketClient == client).FirstOrDefault() : threadList.Where(p => p.threadId == threadId).FirstOrDefault();
        }
        #endregion
    }
}
