namespace MB.Client
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.inputLogin = new System.Windows.Forms.Button();
            this.myTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.remindSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.remindhalfHour = new System.Windows.Forms.ToolStripMenuItem();
            this.remindoneHour = new System.Windows.Forms.ToolStripMenuItem();
            this.remindthreehour = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuOutLine = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.checkBoxRemPassword = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoLogin = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.showMsg = new System.Windows.Forms.Label();
            this.outTimer = new System.Windows.Forms.Timer(this.components);
            this.loginTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // inputLogin
            // 
            this.inputLogin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("inputLogin.BackgroundImage")));
            this.inputLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inputLogin.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.inputLogin.FlatAppearance.BorderSize = 0;
            this.inputLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.inputLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.inputLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.inputLogin.Location = new System.Drawing.Point(82, 276);
            this.inputLogin.Name = "inputLogin";
            this.inputLogin.Size = new System.Drawing.Size(229, 35);
            this.inputLogin.TabIndex = 20;
            this.inputLogin.Text = "登 录";
            this.inputLogin.Click += new System.EventHandler(this.inputLogin_Click);
            // 
            // myTimer
            // 
            this.myTimer.Interval = 10000;
            this.myTimer.Tick += new System.EventHandler(this.myTimer_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "目标导航任务栏提醒";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remindSpace,
            this.toolStripSeparator1,
            this.remindhalfHour,
            this.remindoneHour,
            this.remindthreehour,
            this.toolStripSeparator2,
            this.menuOutLine,
            this.menuClose});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 148);
            // 
            // remindSpace
            // 
            this.remindSpace.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.remindSpace.Image = ((System.Drawing.Image)(resources.GetObject("remindSpace.Image")));
            this.remindSpace.Name = "remindSpace";
            this.remindSpace.Size = new System.Drawing.Size(124, 22);
            this.remindSpace.Text = "提醒间隔";
            this.remindSpace.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // remindhalfHour
            // 
            this.remindhalfHour.Checked = true;
            this.remindhalfHour.CheckState = System.Windows.Forms.CheckState.Checked;
            this.remindhalfHour.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.remindhalfHour.Name = "remindhalfHour";
            this.remindhalfHour.Size = new System.Drawing.Size(124, 22);
            this.remindhalfHour.Text = "半小时";
            this.remindhalfHour.Visible = false;
            this.remindhalfHour.Click += new System.EventHandler(this.remindhalfHour_Click);
            // 
            // remindoneHour
            // 
            this.remindoneHour.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.remindoneHour.Name = "remindoneHour";
            this.remindoneHour.Size = new System.Drawing.Size(124, 22);
            this.remindoneHour.Text = "一小时";
            this.remindoneHour.Visible = false;
            this.remindoneHour.Click += new System.EventHandler(this.remindoneHour_Click);
            // 
            // remindthreehour
            // 
            this.remindthreehour.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.remindthreehour.Name = "remindthreehour";
            this.remindthreehour.Size = new System.Drawing.Size(124, 22);
            this.remindthreehour.Text = "三小时";
            this.remindthreehour.Visible = false;
            this.remindthreehour.Click += new System.EventHandler(this.remindthreehour_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(121, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // menuOutLine
            // 
            this.menuOutLine.CheckOnClick = true;
            this.menuOutLine.Image = ((System.Drawing.Image)(resources.GetObject("menuOutLine.Image")));
            this.menuOutLine.Name = "menuOutLine";
            this.menuOutLine.Size = new System.Drawing.Size(124, 22);
            this.menuOutLine.Text = "登陆";
            this.menuOutLine.Click += new System.EventHandler(this.menuOutLine_Click);
            // 
            // menuClose
            // 
            this.menuClose.CheckOnClick = true;
            this.menuClose.Image = ((System.Drawing.Image)(resources.GetObject("menuClose.Image")));
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(124, 22);
            this.menuClose.Text = "关闭";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // textPassword
            // 
            this.textPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textPassword.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textPassword.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textPassword.Location = new System.Drawing.Point(94, 194);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(239, 32);
            this.textPassword.TabIndex = 23;
            // 
            // checkBoxRemPassword
            // 
            this.checkBoxRemPassword.AutoSize = true;
            this.checkBoxRemPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxRemPassword.Location = new System.Drawing.Point(94, 235);
            this.checkBoxRemPassword.Name = "checkBoxRemPassword";
            this.checkBoxRemPassword.Size = new System.Drawing.Size(72, 16);
            this.checkBoxRemPassword.TabIndex = 24;
            this.checkBoxRemPassword.Text = "记住密码";
            this.checkBoxRemPassword.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoLogin
            // 
            this.checkBoxAutoLogin.AutoSize = true;
            this.checkBoxAutoLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBoxAutoLogin.Location = new System.Drawing.Point(261, 235);
            this.checkBoxAutoLogin.Name = "checkBoxAutoLogin";
            this.checkBoxAutoLogin.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutoLogin.TabIndex = 25;
            this.checkBoxAutoLogin.Text = "自动登陆";
            this.checkBoxAutoLogin.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(150, 43);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 93);
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(371, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(21, 26);
            this.pictureBox2.TabIndex = 27;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.White;
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(66, 194);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(29, 32);
            this.pictureBox4.TabIndex = 29;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(66, 153);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(29, 29);
            this.pictureBox3.TabIndex = 28;
            this.pictureBox3.TabStop = false;
            // 
            // textUserName
            // 
            this.textUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textUserName.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textUserName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textUserName.Location = new System.Drawing.Point(94, 153);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(239, 29);
            this.textUserName.TabIndex = 21;
            // 
            // showMsg
            // 
            this.showMsg.AutoSize = true;
            this.showMsg.ForeColor = System.Drawing.Color.Red;
            this.showMsg.Location = new System.Drawing.Point(13, 337);
            this.showMsg.Name = "showMsg";
            this.showMsg.Size = new System.Drawing.Size(185, 12);
            this.showMsg.TabIndex = 30;
            this.showMsg.Text = "* 用户名或密码错误，请重新输入";
            this.showMsg.Visible = false;
            // 
            // outTimer
            // 
            this.outTimer.Interval = 30000;
            this.outTimer.Tick += new System.EventHandler(this.outTimer_Tick);
            // 
            // loginTimer
            // 
            this.loginTimer.Interval = 120000;
            this.loginTimer.Tick += new System.EventHandler(this.loginTimer_Tick);
            // 
            // LoginForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(395, 356);
            this.Controls.Add(this.showMsg);
            this.Controls.Add(this.textUserName);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBoxAutoLogin);
            this.Controls.Add(this.checkBoxRemPassword);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.inputLogin);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "目标导航";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button inputLogin;
        private System.Windows.Forms.Timer myTimer;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem remindSpace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem remindhalfHour;
        private System.Windows.Forms.ToolStripMenuItem remindoneHour;
        private System.Windows.Forms.ToolStripMenuItem remindthreehour;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuOutLine;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.CheckBox checkBoxRemPassword;
        private System.Windows.Forms.CheckBox checkBoxAutoLogin;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.TextBox textUserName;
        private System.Windows.Forms.Label showMsg;
        private System.Windows.Forms.Timer outTimer;
        private System.Windows.Forms.Timer loginTimer;  //true:正常连接，发送心跳包  false:断线的情况
    }
}