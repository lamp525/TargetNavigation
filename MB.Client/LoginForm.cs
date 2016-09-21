using MB.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace MB.Client
{
    public partial class LoginForm : Form
    {
        #region 变量区域
        private const int WM_NCLBUTTONDOWN = 0XA1;   //定义鼠标左鍵按下
        private const int HTCAPTION = 2;
        ManualResetEvent reviceManager = new ManualResetEvent(false);
        NotifyForm taskbarNotifier;
        private Thread threadClient;
        private Socket socketClient = null;//客户端套接字
        //获取配置文件基本设置
        private bool closeClickAble = Convert.ToBoolean(AppsetingHelper.GetValue("closeClickAble"));
        private bool titleClickable = Convert.ToBoolean(AppsetingHelper.GetValue("titleClickable"));
        private bool contentClickable = Convert.ToBoolean(AppsetingHelper.GetValue("contentClickable"));
        private bool showLucencyArea = Convert.ToBoolean(AppsetingHelper.GetValue("showLucencyArea"));
        private bool mouseOverable = Convert.ToBoolean(AppsetingHelper.GetValue("mouseOverable"));
        private bool mouseReOver = Convert.ToBoolean(AppsetingHelper.GetValue("mouseReOver"));
        private int showingTime = Convert.ToInt32(AppsetingHelper.GetValue("showTime"));
        private int stayingTime = Convert.ToInt32(AppsetingHelper.GetValue("holdTime"));
        private int hidingTime = Convert.ToInt32(AppsetingHelper.GetValue("hideTime"));
        private int loginUserId = 0;
        private int cookieSize = 500;
        private string cookieUrl = AppsetingHelper.GetValue("cookieUrl");
        private string cookieExpire = AppsetingHelper.GetValue("cookieExpires");
        private StringBuilder cookieValue = new StringBuilder();
        private bool timerFlag = true;
        private Byte[] oldBytes = new byte[1024 * 1024];
        //随机请求的类型集合
        private List<int> randomOperates = new List<int> {
            (int)ConstVar.RequestType.YWorkTime,
            (int)ConstVar.RequestType.WWorkTime,
            (int)ConstVar.RequestType.unCheck,
            (int)ConstVar.RequestType.unConfirm
        };
        //true:第一次请求  false：第二次以上请求
        private bool requestFlag = true;
        #endregion

        //构造函数
        public LoginForm()
        {
            InitializeComponent();
            //初始化数据
            InitWindow();

            //关闭窗口，关闭socket连接
            this.FormClosed += SocketDispose;
            this.AcceptButton = inputLogin;

            //读取用户名cookie
            this.TextBoxGetValue(this.textUserName, "userName");
            //密码
            this.TextBoxGetValue(this.textPassword, "password");
            //记住密码
            this.CheckGetValue(this.checkBoxRemPassword, "rember", 1);
            //自动登录
            this.CheckGetValue(this.checkBoxAutoLogin, "auto", 2);
        }

        #region Socket操作

        #region  创建套接字连接到服务端 void CreateSocketConnection()
        /// <summary>
        /// 创建套接字连接到服务端
        /// </summary>
        private void CreateSocketConnection()
        {
            try
            {
                outTimer.Stop();
                //创建一个客户端的套接字 参数(IP4寻址协议,流连接方式,TCP数据传输协议)
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //获取IP
                IPAddress address = IPAddress.Parse(AppsetingHelper.GetValue("ipAddress"));
                //创建一个包含IP和端口号的网络节点对象
                IPEndPoint ipPoint = new IPEndPoint(address, Convert.ToInt32(AppsetingHelper.GetValue("port")));
                // 连接服务端
                socketClient.Connect(ipPoint);
                //创建一个线程,接受服务端发来的数据
                threadClient = new Thread(ReceiveService);
                //设置线程为后台线程
                threadClient.IsBackground = true;
                //启动线程连接服务端
                threadClient.Start();
                //发送用户登录信息
                //var loginFlag = Properties.Settings.Default.loginEx;
                var userName = this.textUserName.Text.Trim();
                var password = this.textPassword.Text.Trim();
                var userLoginInfo = this.AssemSendMsg(new string[]{
                Protocol.OperateProtocol.UOL,userName,password
            });
                Send(userLoginInfo);
                outTimer.Start();
            }
            catch (Exception ex)
            {
                this.inputLogin.Text = "登录";
                this.notifyIcon1.ShowBalloonTip(500, "连接服务器失败", DateTime.Now.ToLocalTime().ToString(), ToolTipIcon.Info);
                LogHelper.WriteLog(typeof(LoginForm), " void CreateSocketConnection():" + ex.ToString());
                timerFlag = false;
                outTimer.Start();
            }

        }
        #endregion

        #region 发送数据到服务端 void Send()
        /// <summary>
        /// 发送数据到服务端
        /// <param name="msg">发送服务器的信息</param>
        /// </summary>
        private void Send(string msg)
        {
            try
            {
                lock (this)
                {
                    if (socketClient == null)
                    {

                        InitMenu(false, "服务未启动，30秒后重试！", false);
                        return;
                    }
                    var b = System.Text.Encoding.UTF8.GetBytes(msg);
                    socketClient.Send(b);
                }
            }
            catch (Exception ex)
            {
                InitMenu(false, "与服务器断开连接，30秒后尝试重连！", true);
                timerFlag = false;
                //setLoginEx(1);
                LogHelper.WriteLog(typeof(LoginForm), " void Send(string msg):" + ex.ToString());
                outTimer.Start();
            }
        }
        #endregion

        #region 接收来自服务器端的信息  void ReceiveService()
        /// <summary>
        /// 接收来自服务器端的信息
        /// </summary>
        private void ReceiveService()
        {
            while (true)
            {
                try
                {
                    reviceManager.Reset();
                    var receiveArgs = new SocketAsyncEventArgs();
                    var bytes = oldBytes;
                    //设置缓冲区
                    receiveArgs.SetBuffer(bytes, 0, bytes.Length);
                    receiveArgs.Completed += ReceiveArgs_Completed;
                    //开始异步接收
                    socketClient.ReceiveAsync(receiveArgs);
                    reviceManager.WaitOne();
                    //if (socketClient.Available <= 0) continue;
                }
                catch (Exception ex)
                {
                    InitMenu(false, "与服务器断开连接", false);
                    //setLoginEx(1);
                    LogHelper.WriteLog(typeof(LoginForm), " void ReceiveService():" + ex.ToString());
                }
            }
        }

        #endregion

        #region 完成信息接收
        /// <summary>
        /// 完成信息接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                //socketClient = sender as Socket;
                if (socketClient != null)
                {
                    var bytes = e.Buffer;
                    var msg = Encoding.UTF8.GetString(bytes, 0, e.BytesTransferred).Trim();
                    var showMsg = string.Empty;
                    var operateArry = msg.Split('+');
                    //登录成功,获取当天计划信息
                    if ((operateArry[0] == Protocol.ClientLoginProtocol.PHC || operateArry[0] == Protocol.ClientLoginProtocol.PUB) && operateArry.Length > 1)
                    {
                        loginUserId = Convert.ToInt32(operateArry[1]);
                        if (loginUserId > 0)
                        {
                            Invoke(new MethodInvoker(delegate ()
                            {
                                this.loginTimer.Stop();
                                this.Visible = false;
                                this.inputLogin.Text = "登录";
                                this.showMsg.Visible = false;
                            }));

                            //登录成功设置cookie
                            SetLoginCookie();
                            //初始化菜单
                            InitMenu(true, string.Empty, true);
                            showMsg = operateArry.Length > 2 ? AnalysisHelper.AnalysisFirstLoginMsg(operateArry[2], operateArry[0]) : showMsg = AnalysisHelper.AnalysisFirstLoginMsg(string.Empty, operateArry[0]);
                            if (!string.IsNullOrWhiteSpace(showMsg))
                            {
                                taskbarNotifier.Tag = Protocol.OperateProtocol.GCM;
                                taskbarNotifier.Show("目标导航欢迎您", showMsg, showingTime, stayingTime, hidingTime);
                            }
                            timerFlag = true;

                            //检查是否有版本更新
                            var versionsNum = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                            var versionsMsg = this.AssemSendMsg(new string[] {
                                     Protocol.OperateProtocol.CVU,loginUserId.ToString(),versionsNum
                                    });
                            this.Send(versionsMsg);
                            //设置是否开机启动
                            StartApp();

                        }
                    }
                    //登录失败协议
                    else if (operateArry[0] == Protocol.OperateProtocol.UDL)
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            this.showMsg.Text = "* 用户名或密码错误，请重新输入";
                            this.showMsg.Visible = true;
                            this.inputLogin.Text = "登录";
                        }));
                        clientOutLine();
                    }
                    //下班前请求协议
                    else if (operateArry[0] == Protocol.OperateProtocol.OWR && operateArry.Length > 1)
                    {
                        showMsg = AnalysisHelper.AnalysisBeforeOLMsg(operateArry);
                        if (!string.IsNullOrWhiteSpace(showMsg))
                        {
                            taskbarNotifier.Tag = Protocol.OperateProtocol.CAM;
                            taskbarNotifier.Show("目标导航友情提醒", showMsg, showingTime, stayingTime, hidingTime);
                        }
                    }
                    //版本更新
                    else if (operateArry[0] == Protocol.OperateProtocol.CVU)
                    {
                        var content = "检测到新版本，请即时更新！";
                        taskbarNotifier.Tag = Protocol.OperateProtocol.CVU;
                        taskbarNotifier.Show("版本更新提醒", content, showingTime, stayingTime, hidingTime);
                    }
                    //客户端随机请求
                    else if (operateArry[0] == Protocol.OperateProtocol.CAM && operateArry.Length > 1)
                    {
                        showMsg = AnalysisHelper.AnalysisRandomMsg(operateArry[1]);
                        if (!string.IsNullOrWhiteSpace(showMsg))
                        {
                            taskbarNotifier.Tag = Protocol.OperateProtocol.CAM;
                            taskbarNotifier.Show("目标导航友情提醒", showMsg, showingTime, stayingTime, hidingTime);
                        }
                    }
                    //即时提醒
                    else if (operateArry[0] == Protocol.OperateProtocol.SIM)
                    {
                        showMsg = AnalysisHelper.AnalysisIM(operateArry);
                        if (!string.IsNullOrWhiteSpace(showMsg))
                        {
                            taskbarNotifier.Tag = Protocol.OperateProtocol.SIM;
                            taskbarNotifier.Show("目标导航友情提醒", showMsg, showingTime, stayingTime, hidingTime);
                        }
                    }

                    //账号在其他地方登陆
                    else if (operateArry[0] == Protocol.OperateProtocol.URL)
                    {
                        InitMenu(false, "您的账号在其他地方登录，请重新登录", true);
                    }
                    //断线协议
                    else if (operateArry[0] == Protocol.OperateProtocol.SDL)
                    {
                        InitMenu(false, "与服务器断开连接", true);
                        clientOutLine();
                    }
                    reviceManager.Set();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                CookieHelper.DeleteCookie(cookieUrl, "auto");
                LogHelper.WriteLog(typeof(LoginForm), " void ReceiveService():" + ex.ToString());
            }
        }
        #endregion

        #region 窗口关闭释放socket
        /// <summary>
        /// 窗口关闭释放socket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketDispose(object sender, EventArgs e)
        {
            this.Send(Protocol.OperateProtocol.UDL + "|");
            clientOutLine();
        }
        #endregion

        #region 客户端下线
        /// <summary>
        /// 客户端下线
        /// </summary>
        private void clientOutLine()
        {
            try
            {
                if (this.socketClient != null)
                {
                    this.inputLogin.Text = "登录";
                    myTimer.Stop();
                    outTimer.Stop();
                    if (timerFlag)
                    {
                        this.socketClient.Shutdown(SocketShutdown.Both);
                    }
                    // this.socketClient = null;
                    //this.socketClient.Close();
                    //this.socketClient = null;
                    //setLoginEx(0);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginForm), " void clientOutLine():" + ex.ToString());
                //TODO:客户端下线处理，图标转灰，给出断开连接提示
                outTimer.Start();
            }
        }
        #endregion


        #endregion

        #region ---------------事件-------------

        #region 关闭按钮点击事件
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        void CloseClick(object obj, EventArgs ea)
        {

        }
        #endregion

        #region 标题点击事件
        /// <summary>
        /// 标题点击事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        void TitleClick(object obj, EventArgs ea)
        {

        }
        #endregion

        #region 内容点击跳转
        /// <summary>
        /// 内容点击跳转
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        void ContentClick(object obj, EventArgs ea)
        {
            var tag = taskbarNotifier.Tag;
            //版本更新链接地址
            if (tag != null && tag.ToString() == Protocol.OperateProtocol.CVU)
            {
                System.Diagnostics.Process.Start(AppsetingHelper.GetValue("clientUpdateUrl"));
            }
            else
            {
                System.Diagnostics.Process.Start("http://10.10.10.2:78//Login/ClientLogin?userName=" + HttpUtility.UrlEncode(this.textUserName.Text.Trim()) + "&password=" + HttpUtility.UrlEncode(this.textPassword.Text.Trim()));
            }

        }
        #endregion

        #region 计时器到设定时间触发的事件
        /// <summary>
        /// 计时器到设定时间触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myTimer_Tick(object sender, EventArgs e)
        {
            this.outTimer.Stop();
            var ran = new Random();
            if (randomOperates.Count <= 0)
            {
                randomOperates = new List<int> {
                                            (int)ConstVar.RequestType.YWorkTime,
                                            (int)ConstVar.RequestType.WWorkTime,
                                            (int)ConstVar.RequestType.unCheck,
                                            (int)ConstVar.RequestType.unConfirm
                                           };

            }
            var type = randomOperates[ran.Next(0, randomOperates.Count)];
            randomOperates.Remove(type);
            var sendMsg = this.AssemSendMsg(new string[]{
                Protocol.OperateProtocol.CAM,loginUserId.ToString(),type.ToString()
            });
            this.Send(sendMsg);
            this.outTimer.Start();
        }
        #endregion

        #region 登录按钮点击事件
        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputLogin_Click(object sender, EventArgs e)
        {
            if (this.inputLogin.Text != "正在登录...")
            {
                if (string.IsNullOrWhiteSpace(this.textUserName.Text))
                {
                    this.showMsg.Text = "* 用户名不能为空";
                    this.showMsg.Visible = true;
                    return;
                }
                else if (string.IsNullOrWhiteSpace(this.textPassword.Text))
                {
                    this.showMsg.Text = "* 密码不能为空";
                    this.showMsg.Visible = true;
                    return;
                }
                this.showMsg.Visible = false;
                this.inputLogin.Text = "正在登录...";
                loginTimer.Start();
                //连接服务器
                CreateSocketConnection();
                //this.Visible = false;
            }

        }
        #endregion

        #region 关闭窗口事件
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 登录或者退出登录的事件(关闭连接)
        /// <summary>
        /// 登录或者退出登录的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuOutLine_Click(object sender, EventArgs e)
        {
            if (this.menuOutLine.Text == "登录")
            {
                this.Visible = true;
            }
            else
            {
                var msg = this.AssemSendMsg(new string[] { Protocol.OperateProtocol.UDL });
                this.Send(msg);
                this.InitMenu(false, string.Empty, true);
            }
        }
        #endregion

        #region 设置点击事件

        //半小时
        private void remindhalfHour_Click(object sender, EventArgs e)
        {
            this.SetRemind(new bool[] { true, false, false });
            this.myTimer.Stop();
            Properties.Settings.Default.remindTime = 1800000;
            Properties.Settings.Default.Save();
            this.myTimer.Interval = 1800000;
            this.myTimer.Start();
        }

        //一小时
        private void remindoneHour_Click(object sender, EventArgs e)
        {
            this.SetRemind(new bool[] { false, true, false });
            this.myTimer.Stop();
            Properties.Settings.Default.remindTime = 3600000;
            Properties.Settings.Default.Save();
            this.myTimer.Interval = 3600000;
            this.myTimer.Start();
        }

        //三小时
        private void remindthreehour_Click(object sender, EventArgs e)
        {
            this.SetRemind(new bool[] { false, false, true });
            this.myTimer.Stop();
            Properties.Settings.Default.remindTime = 10800000;
            Properties.Settings.Default.Save();
            this.myTimer.Interval = 10800000;
            this.myTimer.Start();
        }

        //设置勾选状态
        private void SetRemind(bool[] checks)
        {
            if (checks.Length >= 3)
            {
                this.remindhalfHour.Checked = checks[0];
                this.remindoneHour.Checked = checks[1];
                this.remindthreehour.Checked = checks[2];
            }
        }
        #endregion

        #region 关闭按钮点击事件
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        #endregion

        #endregion

        #region ---------------方法-------------

        #region 拼接发送包
        /// <summary>
        /// 拼接发送包
        /// </summary>
        /// <param name="msg">信息集合</param>
        /// <returns></returns>
        private string AssemSendMsg(string[] msg)
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
            return sendMsg.ToString() + "|";
        }
        #endregion

        #region 判断输入是否是数字
        /// <summary>
        /// 判断输入是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 登录成功后初始化窗口
        /// <summary>
        /// 登录成功后初始化窗口
        /// </summary>
        private void InitWindow()
        {
            taskbarNotifier = new NotifyForm();
            taskbarNotifier.SetBackgroundBitmap(Image.FromFile(Application.StartupPath + "/img/skin3.bmp"), Color.FromArgb(255, 0, 255));
            taskbarNotifier.SetCloseBitmap(Image.FromFile(Application.StartupPath + "/img/close.bmp"), Color.FromArgb(255, 0, 255), new Point(280, 57));
            taskbarNotifier.TitleRectangle = new Rectangle(150, 57, 125, 28);
            taskbarNotifier.ContentRectangle = new Rectangle(75, 92, 230, 55);
            taskbarNotifier.TitleClick += new EventHandler(TitleClick);
            taskbarNotifier.ContentClick += new EventHandler(ContentClick);
            taskbarNotifier.CloseClick += new EventHandler(CloseClick);
            taskbarNotifier.CloseClickable = closeClickAble;
            taskbarNotifier.TitleClickable = titleClickable;
            taskbarNotifier.ContentClickable = contentClickable;
            taskbarNotifier.EnableSelectionRectangle = showLucencyArea;
            taskbarNotifier.KeepVisibleOnMousOver = mouseOverable;
            taskbarNotifier.ReShowOnMouseOver = mouseReOver;
        }
        #endregion

        #region 初始化菜单
        /// <summary>
        /// 初始化菜单
        /// </summary>
        /// <param name="loginFlag">true：登录成功  false：未登录或登录失败</param>
        private void InitMenu(bool loginFlag, string msg, bool operateFlag)
        {
            try
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    if (loginFlag) //登录成功
                    {

                        this.remindSpace.Visible = true;
                        this.toolStripSeparator1.Visible = true;
                        this.remindhalfHour.Visible = true;
                        this.remindoneHour.Visible = true;
                        this.remindthreehour.Visible = true;
                        this.toolStripSeparator2.Visible = true;
                        this.menuOutLine.Text = "退出登录";
                        this.notifyIcon1.Icon = new System.Drawing.Icon(Application.StartupPath + "/img/bearHit.ico");
                        this.notifyIcon1.Text = "能诚集团：" + this.textUserName.Text;
                        var remindTime = Properties.Settings.Default.remindTime;
                        if (remindTime == 0)
                        {
                            this.myTimer.Enabled = false;
                            this.SetRemind(new bool[] { false, false, false });
                        }
                        else
                        {
                            if (remindTime == 1800000)
                            {
                                this.SetRemind(new bool[] { true, false, false });
                            }
                            else if (remindTime == 3600000)
                            {
                                this.SetRemind(new bool[] { false, true, false });
                            }
                            else if (remindTime == 10800000)
                            {
                                this.SetRemind(new bool[] { false, false, true });
                            }
                            this.myTimer.Interval = remindTime;
                            this.myTimer.Start();
                        }
                    }
                    else
                    {
                        this.remindSpace.Visible = false;
                        this.toolStripSeparator1.Visible = false;
                        this.remindhalfHour.Visible = false;
                        this.remindoneHour.Visible = false;
                        this.remindthreehour.Visible = false;
                        this.toolStripSeparator2.Visible = false;
                        this.menuOutLine.Text = "登录";
                        this.notifyIcon1.Icon = new System.Drawing.Icon(Application.StartupPath + "/img/bear.ico");
                        this.notifyIcon1.Text = "目标导航任务栏提醒";
                        taskbarNotifier.Hide();
                        if (msg != string.Empty)
                        {
                            this.notifyIcon1.ShowBalloonTip(500, msg, DateTime.Now.ToLocalTime().ToString(), ToolTipIcon.Warning);
                        }
                        if (operateFlag)
                        {
                            clientOutLine();
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginForm), "void InitMenu(bool loginFlag, string msg):" + ex.ToString());
            }
        }
        #endregion

        #region 文本框设置cookie
        /// <summary>
        /// 文本框设置cookie
        /// </summary>
        /// <param name="textbox"></param>
        private void TextBoxSetCookie(TextBox textbox, string cookieName)
        {
            cookieValue.Clear();
            cookieSize = 500;
            CookieHelper.InternetGetCookie(cookieUrl, cookieName, cookieValue, ref cookieSize);
            if (cookieValue.Length > 0)
            {
                var value = cookieValue.ToString().Split('=')[1];
                if (textbox.Text.Trim() != value)
                {
                    CookieHelper.InternetSetCookie(cookieUrl, cookieName, textbox.Text.Trim() + ";expires=" + cookieExpire);
                }
            }
            else
            {
                CookieHelper.InternetSetCookie(cookieUrl, cookieName, textbox.Text.Trim() + ";expires=" + cookieExpire);
            }
        }
        #endregion

        #region 登录成功后重置是否异常的标志cookie
        /// <summary>
        /// 登录成功后重置是否异常的标志cookie
        /// </summary>
        /// <param name="cookieName">cookie名称</param>
        /// <param name="cookieNewValue">cookie最新设置的值</param>
        private void LoginSetCookie(string cookieName, string cookieNewValue)
        {
            cookieValue.Clear();
            cookieSize = 500;
            CookieHelper.InternetGetCookie(cookieUrl, cookieName, cookieValue, ref cookieSize);
            if (cookieValue.Length > 0)
            {
                var value = cookieValue.ToString().Split('=')[1];
                if (cookieNewValue != value)
                {
                    CookieHelper.InternetSetCookie(cookieUrl, cookieName, cookieNewValue + ";expires=" + cookieExpire);
                }
            }
            else
            {
                CookieHelper.InternetSetCookie(cookieUrl, cookieName, cookieNewValue + ";expires=" + cookieExpire);
            }
        }
        #endregion

        #region 设置登录异常值
        /// <summary>
        /// 设置登录异常值
        /// </summary>
        /// <param name="value"></param>
        private void setLoginEx(int value)
        {
            Properties.Settings.Default.loginEx = value;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region 复选框设置cookie
        /// <summary>
        /// 文本框设置cookie
        /// </summary>
        /// <param name="checkBox"></param>
        /// <param name="cookieName"></param>
        private void CheckBoxSetCookie(CheckBox checkBox, string cookieName)
        {
            cookieValue.Clear();
            cookieSize = 500;
            CookieHelper.InternetGetCookie(cookieUrl, cookieName, cookieValue, ref cookieSize);
            if (cookieValue.Length <= 0)
            {
                CookieHelper.InternetSetCookie(cookieUrl, cookieName, "1;expires=" + cookieExpire);
            }
        }
        #endregion

        #region 文本框赋值
        /// <summary>
        /// 文本框赋值
        /// </summary>
        /// <param name="textbox"></param>
        /// <param name="cookieName"></param>
        public void TextBoxGetValue(TextBox textbox, string cookieName)
        {
            cookieValue.Clear();
            cookieSize = 500;
            CookieHelper.InternetGetCookie(cookieUrl, cookieName, cookieValue, ref cookieSize);
            if (cookieValue.Length > 0)
            {
                textbox.Text = cookieValue.ToString().Split('=')[1];
            }
        }
        #endregion

        #region 复选框赋值
        /// <summary>
        /// 复选框赋值
        /// </summary>
        /// <param name="checkBox"></param>
        /// <param name="cookieName"></param>
        /// <param name="flag">1、记住密码 2、自动登录</param>
        public void CheckGetValue(CheckBox checkBox, string cookieName, int flag)
        {
            cookieValue.Clear();
            cookieSize = 500;
            CookieHelper.InternetGetCookie(cookieUrl, cookieName, cookieValue, ref cookieSize);
            if (cookieValue.Length > 0)
            {
                checkBox.Checked = cookieValue.ToString().Split('=')[1] == "1" ? true : false;
                if (flag == 2 && checkBox.Checked && !string.IsNullOrWhiteSpace(this.textUserName.Text.Trim()) && !string.IsNullOrWhiteSpace(this.textPassword.Text.Trim()))
                {
                    inputLogin_Click(this.inputLogin, new EventArgs());
                }
            }
        }
        #endregion

        #region 设置登录成功后相应的cookie
        /// <summary>
        /// 设置登录成功后相应的cookie
        /// </summary>
        public void SetLoginCookie()
        {
            this.TextBoxSetCookie(this.textUserName, "userName");
            if (this.checkBoxRemPassword.Checked)   //记住密码
            {
                //写入cookie
                this.TextBoxSetCookie(this.textPassword, "password");
                this.CheckBoxSetCookie(this.checkBoxRemPassword, "rember");
            }
            else
            {
                //删除cookie
                CookieHelper.DeleteCookie(cookieUrl, "password");
                CookieHelper.DeleteCookie(cookieUrl, "rember");
            }
            cookieValue.Clear();
            cookieSize = 500;
            CookieHelper.InternetGetCookie(cookieUrl, "auto", cookieValue, ref cookieSize);
            if (this.checkBoxAutoLogin.Checked)
            {
                if (cookieValue.Length <= 0)
                {
                    CookieHelper.InternetSetCookie(cookieUrl, "auto", "1;expires=" + cookieExpire);
                }
            }
            else
            {
                if (cookieValue.Length > 0)
                {
                    CookieHelper.DeleteCookie(cookieUrl, "auto");
                }
            }
        }
        #endregion

        #region 设置开机自动启动
        /// <summary>
        /// 设置开机自动启动
        /// </summary>
        public void StartApp()
        {
            string strName = Application.ExecutablePath;
            string strnewName = strName.Substring(strName.LastIndexOf("\\") + 1);
            if (this.checkBoxAutoLogin.Checked)
            {
                if (!File.Exists(strName))//指定文件是否存在  
                    return;
                Microsoft.Win32.RegistryKey Rkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (Rkey == null)
                    Rkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                Rkey.SetValue(strnewName, strName);//修改注册表，使程序开机时自动执行  

            }
            else
            {
                //修改注册表，使程序开机时不自动执行
                Microsoft.Win32.RegistryKey Rkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (Rkey == null) return;
                Rkey.DeleteValue(strnewName, false);
            }
        }
        #endregion

        #endregion

        #region 窗口移动
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();

            SendMessage((int)this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int ReleaseCapture();
        #endregion

        #region 发送心跳包或者断线重连
        /// <summary>
        /// 发送心跳包或者断线重连
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void outTimer_Tick(object sender, EventArgs e)
        {
            if (timerFlag)
            {
                var sendMsg = string.Empty;
                var now = DateTime.Now;
                if (now >= DateTime.Today.AddHours(17) && requestFlag)  //17点时发送请求
                {
                    sendMsg = this.AssemSendMsg(new string[] { Protocol.OperateProtocol.OWR, loginUserId.ToString() });
                    requestFlag = false;
                }
                this.Send(sendMsg);
            }
            else
            {
                CreateSocketConnection();
            }
        }
        #endregion

        #region 防止登录超时
        /// <summary>
        /// 防止登录超时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginTimer_Tick(object sender, EventArgs e)
        {
            loginTimer.Stop();
            this.showMsg.Visible = true;
            this.showMsg.Text = "* 登录超时，请稍候再试";
            this.inputLogin.Text = "登录";
        }
        #endregion
    }
}
