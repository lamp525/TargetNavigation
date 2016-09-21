using MB.Web.Models;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class UserHeadController : BaseController
    {
        //
        // GET: /UserHead/
        private IUserBLL UserBLL { get; set; }

     

        private string configpath = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

        public ActionResult Index()
        {
            return View("~/Views/User/UserHeadImg.cshtml");
        }

        [HttpPost]
        public ActionResult uploadHead(HttpPostedFileBase head)//命名和上传控件name 一样
        {
            if ((head == null))
            {
                return Json(new { msg = 0 });
            }
            else
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                var fileExt = System.IO.Path.GetExtension(head.FileName).Substring(1).ToLower();
                if (!supportedTypes.Contains(fileExt))
                {
                    return Json(new { msg = -1 });
                }

                if (head.ContentLength > 1024 * 1000 * 10)
                {
                    return Json(new { msg = -2 });
                }
                var filename = Session["userId"].ToString() + "Test." + fileExt;//假文件
                string mapPath = Server.MapPath(configpath).Replace("\\UserHead", null);
                var filepath = Path.Combine(mapPath, filename);
                if (this.CreateFolderIfNeeded(mapPath))
                {
                    head.SaveAs(filepath);
                }
                return Json(new { msg = filename });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult saveHead()
        {
            int userId = Convert.ToInt32(Session["userId"]);
            UploadImageModel model = new UploadImageModel();
            if (!string.IsNullOrEmpty(Request.Form["headFileName"].ToString()))
            {
                model.headFileName = Request.Form["headFileName"].ToString();
            }
            else
            {
                UserInfo userInfo = UserBLL.GetUserHeadImg(userId);
                model.headFileName = userInfo.OriginalImage;
            }
            if (string.IsNullOrEmpty(model.headFileName))
            {
                return Json(new { msg = 2 });
            }
            model.x = Convert.ToInt32(Request.Form["x"]);
            model.y = Convert.ToInt32(Request.Form["y"]);
            model.rx = Convert.ToInt32(Request.Form["rx"]);
            model.ry = Convert.ToInt32(Request.Form["ry"]);
            model.width = Convert.ToInt32(Request.Form["width"]);
            model.height = Convert.ToInt32(Request.Form["height"]);
            model.ratioW = Convert.ToInt32(Request.Form["ratioW"]);
            model.ratioH = Convert.ToInt32(Request.Form["ratioH"]);

            string imgimagePosition = model.rx + "," + model.ry + "," + model.ratioW + "," + model.ry;
            if ((model == null))
            {
                return Json(new { msg = 0 });
            }
            else
            {
                var filepath = Path.Combine(Server.MapPath(configpath), model.headFileName).Replace("\\UserHead", null);
                string fileExt = Path.GetExtension(filepath);
                //Random r = new Random();
                var originalImage = Session["userId"].ToString() + fileExt;
                var bigImagefilename = Session["userId"].ToString() + "bigImage" + DateTime.Now.ToString("yyyyMMddhhmmss") + fileExt;
                var midImagefilename = Session["userId"].ToString() + "midImage" + DateTime.Now.ToString("yyyyMMddhhmmss") + fileExt;
                var smallImagefilename = Session["userId"].ToString() + "smallImage" + DateTime.Now.ToString("yyyyMMddhhmmss") + fileExt;
                string mapPath = Server.MapPath(configpath).Replace("\\UserHead", null);
                string newPath = Path.Combine(mapPath, originalImage);
                var pathbigImage = Path.Combine(mapPath, bigImagefilename);
                var pathmidImage = Path.Combine(mapPath, midImagefilename);
                var pathsmallImage = Path.Combine(mapPath, smallImagefilename);
                if (System.IO.File.Exists(filepath))
                {
                    if (System.IO.File.Exists(newPath))
                    {
                        if (!filepath.Equals(newPath))
                        {
                            System.IO.File.Delete(newPath);
                        }
                    }
                    System.IO.File.Move(filepath, newPath);/*真文件*/
                }
                cutAvatar(newPath, model.x, model.y, model.width, model.height, 75L, pathbigImage, 140);
                cutAvatar(newPath, model.x, model.y, model.width, model.height, 75L, pathmidImage, 64);
                cutAvatar(newPath, model.x, model.y, model.width, model.height, 75L, pathsmallImage, 32);

                //数据库操作

                UserBLL.SaveImg(userId, originalImage, bigImagefilename, midImagefilename, smallImagefilename, imgimagePosition);
                return Json(new { msg = 1 });
            }
        }

        /// <summary>
        /// 创建缩略图
        /// </summary>
        public void cutAvatar(string imgSrc, int x, int y, int width, int height, long Quality, string SavePath, int t)
        {
            Image original = Image.FromFile(imgSrc);

            Bitmap img = new Bitmap(t, t, PixelFormat.Format24bppRgb);

            img.MakeTransparent(img.GetPixel(0, 0));
            img.SetResolution(72, 72);
            using (Graphics gr = Graphics.FromImage(img))
            {
                if (original.RawFormat.Equals(ImageFormat.Jpeg) || original.RawFormat.Equals(ImageFormat.Png) || original.RawFormat.Equals(ImageFormat.Bmp))
                {
                    gr.Clear(Color.Transparent);
                }
                if (original.RawFormat.Equals(ImageFormat.Gif))
                {
                    gr.Clear(Color.White);
                }

                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.CompositingQuality = CompositingQuality.HighQuality;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var attribute = new System.Drawing.Imaging.ImageAttributes())
                {
                    attribute.SetWrapMode(WrapMode.TileFlipXY);
                    gr.DrawImage(original, new Rectangle(0, 0, t, t), x, y, width, height, GraphicsUnit.Pixel, attribute);
                }
            }
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            if (original.RawFormat.Equals(ImageFormat.Jpeg))
            {
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
            }
            else
                if (original.RawFormat.Equals(ImageFormat.Png))
                {
                    myImageCodecInfo = GetEncoderInfo("image/png");
                }
                else
                    if (original.RawFormat.Equals(ImageFormat.Gif))
                    {
                        myImageCodecInfo = GetEncoderInfo("image/gif");
                    }
                    else
                        if (original.RawFormat.Equals(ImageFormat.Bmp))
                        {
                            myImageCodecInfo = GetEncoderInfo("image/bmp");
                        }

            Encoder myEncoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, Quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            img.Save(SavePath, myImageCodecInfo, myEncoderParameters);
            original.Dispose();
        }

        //根据长宽自适应 按原图比例缩放
        private static Size GetThumbnailSize(System.Drawing.Image original, int desiredWidth, int desiredHeight)
        {
            var widthScale = (double)desiredWidth / original.Width;
            var heightScale = (double)desiredHeight / original.Height;
            var scale = widthScale < heightScale ? widthScale : heightScale;
            return new Size
            {
                Width = (int)(scale * original.Width),
                Height = (int)(scale * original.Height)
            };
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    //TODO：处理异常
                    result = false;
                }
            }
            return result;
        }

        [HttpPost]
        public string getUrl()
        {
            int userId = Convert.ToInt32(Session["userId"]);
            UserInfo userInfo = UserBLL.GetUserHeadImg(userId);

            var jsonResult = new JsonResultModel(JsonResultType.success, userInfo, "正常", true);
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path">文件的完整路径</param>
        /// <returns>返回结果</returns>
        public Boolean IsImage(string path)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}