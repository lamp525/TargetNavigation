using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MB.Client
{
    public partial class NotifyForm : Form
    {
        private delegate void richTextBoxCallBack();
        #region 受保护变量
        protected Bitmap BackgroundBitmap = null;
        protected Bitmap CloseBitmap = null;
        protected Point CloseBitmapLocation;
        protected Size CloseBitmapSize;
        protected Rectangle RealTitleRectangle;
        protected Rectangle RealContentRectangle;
        protected Rectangle WorkAreaRectangle;
        protected Timer timer = new Timer();
        protected TaskbarStates taskbarState = TaskbarStates.hidden;
        protected string titleText;
        protected string contentText;
        protected Color normalTitleColor = Color.FromArgb(255, 0, 0);
        protected Color hoverTitleColor = Color.FromArgb(255, 0, 0);
        protected Color normalContentColor = Color.FromArgb(0, 0, 0);
        protected Color hoverContentColor = Color.FromArgb(0, 0, 0x66);
        protected Font normalTitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font hoverTitleFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        protected Font normalContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font hoverContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected int nShowEvents;
        protected int nHideEvents;
        protected int nVisibleEvents;
        protected int nIncrementShow;
        protected int nIncrementHide;
        protected bool bIsMouseOverPopup = false;
        protected bool bIsMouseOverClose = false;
        protected bool bIsMouseOverContent = false;
        protected bool bIsMouseOverTitle = false;
        protected bool bIsMouseDown = false;
        protected bool bKeepVisibleOnMouseOver = true;			// Added Rev 002
        protected bool bReShowOnMouseOver = false;				// Added Rev 002
        #endregion

        #region 公有变量
        public Rectangle TitleRectangle;
        public Rectangle ContentRectangle;
        public bool TitleClickable = false;
        public bool ContentClickable = true;
        public bool CloseClickable = true;
        public bool EnableSelectionRectangle = true;
        public event EventHandler CloseClick = null;
        public event EventHandler TitleClick = null;
        public event EventHandler ContentClick = null;
        private int hideTime = 0;
        private int showTime = 0;
        #endregion

        [DllImport("user32")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        //下面是可用的常量，根据不同的动画效果声明自己需要的  
        private const int AW_HOR_POSITIVE = 0x0001;//自左向右显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志  
        private const int AW_HOR_NEGATIVE = 0x0002;//自右向左显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志  
        private const int AW_VER_POSITIVE = 0x0004;//自顶向下显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志  
        private const int AW_VER_NEGATIVE = 0x0008;//自下向上显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志该标志  
        private const int AW_CENTER = 0x0010;//若使用了AW_HIDE标志，则使窗口向内重叠；否则向外扩展  
        private const int AW_HIDE = 0x10000;//隐藏窗口  
        private const int AW_ACTIVE = 0x20000;//激活窗口，在使用了AW_HIDE标志后不要使用这个标志  
        private const int AW_SLIDE = 0x40000;//使用滑动类型动画效果，默认为滚动动画类型，当使用AW_CENTER标志时，这个标志就被忽略  
        private const int AW_BLEND = 0x80000;//使用淡入淡出效果 

        #region 枚举
        /// <summary>
        /// 弹窗动画的不同状态
        /// </summary>
        public enum TaskbarStates
        {
            hidden = 0,
            appearing = 1,
            visible = 2,
            disappearing = 3
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyForm()
        {
            // Window Style
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            base.Show();
            base.Hide();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = false;
            TopMost = true;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;

            //timer.Enabled = true;
            timer.Tick += new EventHandler(OnTimer);
        }
        #endregion

        #region 属性
        /// <summary>
        ///获取当前的窗口状态(隐藏, 正在显示, 显示, 正在隐藏)
        /// </summary>
        public TaskbarStates TaskbarState
        {
            get
            {
                return taskbarState;
            }
        }

        /// <summary>
        /// 获取/设置弹出窗口标题
        /// </summary>
        public string TitleText
        {
            get
            {
                return titleText;
            }
            set
            {
                titleText = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取/设置弹出窗口内容
        /// </summary>
        public string ContentText
        {
            get
            {
                return contentText;
            }
            set
            {
                contentText = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取/设置正常的标题颜色
        /// </summary>
        public Color NormalTitleColor
        {
            get
            {
                return normalTitleColor;
            }
            set
            {
                normalTitleColor = value;
                Refresh();
            }
        }

        /// <summary>
        ///获取/设置悬浮上去时的标题颜色
        /// </summary>
        public Color HoverTitleColor
        {
            get
            {
                return hoverTitleColor;
            }
            set
            {
                hoverTitleColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取/设置正常内容的颜色
        /// </summary>
        public Color NormalContentColor
        {
            get
            {
                return normalContentColor;
            }
            set
            {
                normalContentColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取/设置悬浮时内容的颜色
        /// </summary>
        public Color HoverContentColor
        {
            get
            {
                return hoverContentColor;
            }
            set
            {
                hoverContentColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取/设置正常的标题字体
        /// </summary>
        public Font NormalTitleFont
        {
            get
            {
                return normalTitleFont;
            }
            set
            {
                normalTitleFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取/设置悬浮时标题的字体
        /// </summary>
        public Font HoverTitleFont
        {
            get
            {
                return hoverTitleFont;
            }
            set
            {
                hoverTitleFont = value;
                Refresh();
            }
        }

        /// <summary>
        ///获取/设置正常时内容的字体
        /// </summary>
        public Font NormalContentFont
        {
            get
            {
                return normalContentFont;
            }
            set
            {
                normalContentFont = value;
                Refresh();
            }
        }

        /// <summary>
        ///获取/设置悬浮时内容的字体
        /// </summary>
        public Font HoverContentFont
        {
            get
            {
                return hoverContentFont;
            }
            set
            {
                hoverContentFont = value;
                Refresh();
            }
        }

        /// <summary>
        /// 当鼠标悬浮在窗口上时，保持窗口显示
        /// Added Rev 002
        /// </summary>
        public bool KeepVisibleOnMousOver
        {
            get
            {
                return bKeepVisibleOnMouseOver;
            }
            set
            {
                bKeepVisibleOnMouseOver = value;
            }
        }

        /// <summary>
        /// 当窗口正在消失时，鼠标悬浮上去，设置窗口状态
        /// Added Rev 002
        /// </summary>
        public bool ReShowOnMouseOver
        {
            get
            {
                return bReShowOnMouseOver;
            }
            set
            {
                bReShowOnMouseOver = value;
            }
        }

        #endregion

        #region 公有方法
        /// <summary>
        /// 显示提示框
        /// </summary>
        /// <param name="strTitle">弹窗标题</param>
        /// <param name="strContent">弹窗内容</param>
        /// <param name="nTimeToShow">显示动画的时间（以毫秒为单位）</param>
        /// <param name="nTimeToStay">显示持续的时间（以毫秒为单位）</param>
        /// <param name="nTimeToHide">隐藏动画持续时间(以毫秒为单位)</param>
        /// <returns>Nothing</returns>
        public void Show(string strTitle, string strContent, int nTimeToShow, int nTimeToStay, int nTimeToHide)
        {
            richTextBoxCallBack callback = delegate()//使用委托  
          {
              WorkAreaRectangle = Screen.GetWorkingArea(WorkAreaRectangle);
              titleText = strTitle;
              contentText = strContent;
              nVisibleEvents = nTimeToStay;
              hideTime = nTimeToHide;
              showTime = nTimeToShow;
              CalculateMouseRectangles();

              // 我们计算的像素增量和计时器值显示动画
              int nEvents;
              if (nTimeToShow > 10)
              {
                  nEvents = Math.Min((nTimeToShow / 10), BackgroundBitmap.Height);
                  nShowEvents = nTimeToShow / nEvents;
                  nIncrementShow = BackgroundBitmap.Height / nEvents;
              }
              else
              {
                  nShowEvents = 10;
                  nIncrementShow = BackgroundBitmap.Height;
              }

              // 我们计算的像素增量和计时器值隐藏动画
              if (nTimeToHide > 10)
              {
                  nEvents = Math.Min((nTimeToHide / 10), BackgroundBitmap.Height);
                  nHideEvents = nTimeToHide / nEvents;
                  nIncrementHide = BackgroundBitmap.Height / nEvents;
              }
              else
              {
                  nHideEvents = 10;
                  nIncrementHide = BackgroundBitmap.Height;
              }

              int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
              int y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
              this.Location = new Point(x, y);//设置窗体在屏幕右下角显示  
              switch (taskbarState)
              {
                  case TaskbarStates.hidden:
                      taskbarState = TaskbarStates.appearing;
                      //SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17, WorkAreaRectangle.Bottom - 1, BackgroundBitmap.Width, 0);
                      timer.Interval = nShowEvents;
                      timer.Start();

                      AnimateWindow(this.Handle, nTimeToShow, AW_SLIDE | AW_ACTIVE | AW_VER_NEGATIVE);
                      break;

                  case TaskbarStates.appearing:
                      Refresh();
                      break;

                  case TaskbarStates.visible:
                      timer.Stop();
                      timer.Interval = nVisibleEvents;
                      timer.Start();
                      Refresh();
                      break;

                  case TaskbarStates.disappearing:
                      timer.Stop();
                      taskbarState = TaskbarStates.visible;
                      AnimateWindow(this.Handle, nTimeToShow, AW_SLIDE | AW_ACTIVE | AW_VER_NEGATIVE);
                      timer.Interval = nVisibleEvents;
                      timer.Start();
                      Refresh();
                      break;
              }
          };
            Invoke(callback);
        }

        /// <summary>
        /// 隐藏弹窗
        /// </summary>
        /// <returns>Nothing</returns>
        public new void Hide()
        {
            if (taskbarState != TaskbarStates.hidden)
            {
                timer.Stop();
                taskbarState = TaskbarStates.hidden;
                base.Hide();
            }
        }

        /// <summary>
        /// 设置背景图片以及设置透明度颜色
        /// </summary>
        /// <param name="strFilename">图片路径</param>
        /// <param name="transparencyColor">颜色</param>
        /// <returns>Nothing</returns>
        public void SetBackgroundBitmap(string strFilename, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(strFilename);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        /// <summary>
        ///设置背景的颜色和透明度
        /// </summary>
        /// <param name="image">Image/Bitmap object which represents the Background Bitmap</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// <returns>Nothing</returns>
        public void SetBackgroundBitmap(Image image, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(image);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        /// <summary>
        /// Sets the 3-State Close Button bitmap, its transparency color and its coordinates
        /// </summary>
        /// <param name="strFilename">Path of the 3-state Close button Bitmap on the disk (width must a multiple of 3)</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// <param name="position">Location of the close button on the popup</param>
        /// <returns>Nothing</returns>
        public void SetCloseBitmap(string strFilename, Color transparencyColor, Point position)
        {
            CloseBitmap = new Bitmap(strFilename);
            CloseBitmap.MakeTransparent(transparencyColor);
            CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            CloseBitmapLocation = position;
        }

        /// <summary>
        /// Sets the 3-State Close Button bitmap, its transparency color and its coordinates
        /// </summary>
        /// <param name="image">Image/Bitmap object which represents the 3-state Close button Bitmap (width must be a multiple of 3)</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// /// <param name="position">Location of the close button on the popup</param>
        /// <returns>Nothing</returns>
        public void SetCloseBitmap(Image image, Color transparencyColor, Point position)
        {
            CloseBitmap = new Bitmap(image);
            CloseBitmap.MakeTransparent(transparencyColor);
            CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            CloseBitmapLocation = position;
        }
        #endregion

        #region 受保护方法
        protected void DrawCloseButton(Graphics grfx)
        {
            if (CloseBitmap != null)
            {
                Rectangle rectDest = new Rectangle(CloseBitmapLocation, CloseBitmapSize);
                Rectangle rectSrc;

                if (bIsMouseOverClose)
                {
                    if (bIsMouseDown)
                        rectSrc = new Rectangle(new Point(CloseBitmapSize.Width * 2, 0), CloseBitmapSize);
                    else
                        rectSrc = new Rectangle(new Point(CloseBitmapSize.Width, 0), CloseBitmapSize);
                }
                else
                    rectSrc = new Rectangle(new Point(0, 0), CloseBitmapSize);


                grfx.DrawImage(CloseBitmap, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }

        protected void DrawText(Graphics grfx)
        {
            if (titleText != null && titleText.Length != 0)
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;	
                if (bIsMouseOverTitle)
                    grfx.DrawString(titleText, hoverTitleFont, new SolidBrush(hoverTitleColor), TitleRectangle, sf);
                else
                    grfx.DrawString(titleText, normalTitleFont, new SolidBrush(normalTitleColor), TitleRectangle, sf);
            }

            if (contentText != null && contentText.Length != 0)
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                sf.Trimming = StringTrimming.Word;		
                //if (EnableSelectionRectangle)
                //    ControlPaint.DrawBorder3D(grfx, RealContentRectangle, Border3DStyle.RaisedInner, Border3DSide.Top | Border3DSide.Bottom | Border3DSide.Left | Border3DSide.Right);
                    //ControlPaint.DrawBorder3D(grfx, RealContentRectangle, Border3DStyle.RaisedInner, Border3DSide.Bottom);

                if (bIsMouseOverContent)
                {
                    grfx.DrawString(contentText, hoverContentFont, new SolidBrush(hoverContentColor), ContentRectangle, sf);
                }
                else
                    grfx.DrawString(contentText, normalContentFont, new SolidBrush(normalContentColor), ContentRectangle, sf);
            }
        }

        protected void CalculateMouseRectangles()
        {
            Graphics grfx = CreateGraphics();
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            SizeF sizefTitle = grfx.MeasureString(titleText, hoverTitleFont, TitleRectangle.Width, sf);
            SizeF sizefContent = grfx.MeasureString(contentText, hoverContentFont, ContentRectangle.Width, sf);
            grfx.Dispose();

            if (sizefTitle.Height > TitleRectangle.Height)
            {
                RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, TitleRectangle.Width, TitleRectangle.Height);
            }
            else
            {
                RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, (int)sizefTitle.Width, (int)sizefTitle.Height);
            }
            RealTitleRectangle.Inflate(0, 2);

            if (sizefContent.Height > ContentRectangle.Height)
            {
                RealContentRectangle = new Rectangle(75, ContentRectangle.Top, (int)sizefContent.Width, ContentRectangle.Height);
            }
            else
            {
                RealContentRectangle = new Rectangle(75, (ContentRectangle.Height - (int)sizefContent.Height) / 2 + ContentRectangle.Top, (int)sizefContent.Width, (int)sizefContent.Height);
            }
            RealContentRectangle.Inflate(0, 2);
        }

        protected Region BitmapToRegion(Bitmap bitmap, Color transparencyColor)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

            int height = bitmap.Height;
            int width = bitmap.Width;

            GraphicsPath path = new GraphicsPath();

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (bitmap.GetPixel(i, j) == transparencyColor)
                        continue;

                    int x0 = i;

                    while ((i < width) && (bitmap.GetPixel(i, j) != transparencyColor))
                        i++;

                    path.AddRectangle(new Rectangle(x0, j, i - x0, 1));
                }

            Region region = new Region(path);
            path.Dispose();
            return region;
        }
        #endregion

        #region 事件重写
        protected void OnTimer(Object obj, EventArgs ea)
        {
            switch (taskbarState)
            {
                case TaskbarStates.appearing:
                    if (Height >= BackgroundBitmap.Height)
                    {
                        timer.Stop();
                        Height = BackgroundBitmap.Height;
                        timer.Interval = nVisibleEvents;
                        taskbarState = TaskbarStates.visible;
                        timer.Start();
                    }
                    break;

                case TaskbarStates.visible:
                    timer.Stop();
                    timer.Interval = nHideEvents;
                    if ((bKeepVisibleOnMouseOver && !bIsMouseOverPopup) || (!bKeepVisibleOnMouseOver))
                    {
                        taskbarState = TaskbarStates.disappearing;
                    }
                    timer.Start();
                    break;

                case TaskbarStates.disappearing:
                    if (bReShowOnMouseOver && bIsMouseOverPopup)
                    {
                        taskbarState = TaskbarStates.appearing;
                    }
                    else
                    {
                        timer.Stop();
                        AnimateWindow(this.Handle, hideTime, AW_SLIDE | AW_HIDE | AW_VER_POSITIVE);
                        taskbarState = TaskbarStates.hidden;
                        base.Hide();
                    }
                    break;
            }

        }

        protected override void OnMouseEnter(EventArgs ea)
        {
            base.OnMouseEnter(ea);
            bIsMouseOverPopup = true;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs ea)
        {
            base.OnMouseLeave(ea);
            bIsMouseOverPopup = false;
            bIsMouseOverClose = false;
            bIsMouseOverTitle = false;
            bIsMouseOverContent = false;
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            base.OnMouseMove(mea);

            bool bContentModified = false;

            if ((mea.X > CloseBitmapLocation.X) && (mea.X < CloseBitmapLocation.X + CloseBitmapSize.Width) && (mea.Y > CloseBitmapLocation.Y) && (mea.Y < CloseBitmapLocation.Y + CloseBitmapSize.Height) && CloseClickable)
            {
                if (!bIsMouseOverClose)
                {
                    bIsMouseOverClose = true;
                    bIsMouseOverTitle = false;
                    bIsMouseOverContent = false;
                    Cursor = Cursors.Hand;
                    bContentModified = true;
                }
            }
            else if (RealContentRectangle.Contains(new Point(mea.X, mea.Y)) && ContentClickable)
            {
                if (!bIsMouseOverContent)
                {
                    bIsMouseOverClose = false;
                    bIsMouseOverTitle = false;
                    bIsMouseOverContent = true;
                    Cursor = Cursors.Hand;
                    bContentModified = true;
                }
            }
            else if (RealTitleRectangle.Contains(new Point(mea.X, mea.Y)) && TitleClickable)
            {
                if (!bIsMouseOverTitle)
                {
                    bIsMouseOverClose = false;
                    bIsMouseOverTitle = true;
                    bIsMouseOverContent = false;
                    Cursor = Cursors.Hand;
                    bContentModified = true;
                }
            }
            else
            {
                if (bIsMouseOverClose || bIsMouseOverTitle || bIsMouseOverContent)
                    bContentModified = true;

                bIsMouseOverClose = false;
                bIsMouseOverTitle = false;
                bIsMouseOverContent = false;
                Cursor = Cursors.Default;
            }

            if (bContentModified)
                Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs mea)
        {
            base.OnMouseDown(mea);
            bIsMouseDown = true;

            if (bIsMouseOverClose)
                Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);
            bIsMouseDown = false;

            if (bIsMouseOverClose)
            {
                Hide();

                if (CloseClick != null)
                    CloseClick(this, new EventArgs());
            }
            else if (bIsMouseOverTitle)
            {
                if (TitleClick != null)
                    TitleClick(this, new EventArgs());
            }
            else if (bIsMouseOverContent)
            {
                if (ContentClick != null)
                    ContentClick(this, new EventArgs());
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pea)
        {
            Graphics grfx = pea.Graphics;
            grfx.PageUnit = GraphicsUnit.Pixel;

            Graphics offScreenGraphics;
            Bitmap offscreenBitmap;

            offscreenBitmap = new Bitmap(BackgroundBitmap.Width, BackgroundBitmap.Height);
            offScreenGraphics = Graphics.FromImage(offscreenBitmap);

            if (BackgroundBitmap != null)
            {
                offScreenGraphics.DrawImage(BackgroundBitmap, 0, 0, BackgroundBitmap.Width, BackgroundBitmap.Height);
            }

            DrawCloseButton(offScreenGraphics);
            DrawText(offScreenGraphics);

            grfx.DrawImage(offscreenBitmap, 0, 0);
        }
        #endregion

    }
}
