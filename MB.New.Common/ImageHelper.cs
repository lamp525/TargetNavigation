using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MB.New.Common
{
    public class ImageHelper
    {
        /// <summary>
        /// 取得用户头像名
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetUserImageName(int userId, string extension, bool temp = false)
        {
            // 后缀名不带点加上点
            extension = extension.IndexOf(".") == 0 ? extension : "." + extension;
            return string.Format("{0}{1}{2}", userId, temp ? "Temp" : "", extension);
        }

        /// <summary>
        /// 截取用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="extension">后缀名</param>
        /// <param name="isTemp">是否临时文件</param>
        /// <param name="pointX">截取点X坐标</param>
        /// <param name="pointY">截取点Y坐标</param>
        /// <param name="cutWidth">截取宽度</param>
        /// <param name="cutHeight">截取高度</param>
        /// <returns></returns>
        public static string GetCutHeadImage(int userId, string extension, bool isTemp, int pointX, int pointY, int cutWidth, int cutHeight)
        {
            // 取得原图地址
            var path = FileUtility.GetFilePath(EnumDefine.FileFolderType.HeadImage, GetUserImageName(userId, extension, isTemp));
            // 取得保存后的文件名
            var saveFileName = string.Format("{0}{1}{2}", userId, "head",extension);

            GetPartImage(path, FileUtility.GetFilePath(EnumDefine.FileFolderType.HeadImage, saveFileName), 140, 140, pointX, pointY, cutWidth, cutHeight);

            return saveFileName;
        }

        /// <summary>
        /// 获取图片指定部分
        /// </summary>
        /// <param name="path">原图路径</param>
        /// <param name="savedFile">保存文件路径</param>
        /// <param name="partWidth">目标图片的宽度</param>
        /// <param name="partHeight">目标图片的高度</param>
        /// <param name="origStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="origStartPointY">原始图片开始截取处的坐标Y值</param>
        /// <param name="cutWidth">截取图片的宽度</param>
        /// <param name="cutHeight">截取图片的高度</param>
        /// <param name="format">保存格式，通常可以是jpeg</param>
        private static void GetPartImage(string path, string savedFile, int partWidth, int partHeight, int origStartPointX, int origStartPointY, int cutWidth, int cutHeight)
        {
            using (Image originalImg = Image.FromFile(path))
            {
                Bitmap partImg = new Bitmap(partWidth, partHeight);
                Graphics graphics = Graphics.FromImage(partImg);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // 设置目标图保存格式
                ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo i in icis)
                {
                    if (i.MimeType == "image/png")
                    {
                        ici = i;
                        break;
                    }
                }

                // 设置清晰度
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(Encoder.Quality, 100);

                // 目标位置
                Rectangle destRect = new Rectangle(new Point(0, 0), new Size(partWidth, partHeight));
                // 原图位置（默认从原图中截取的图片大小等于目标图片的大小）
                Rectangle origRect = new Rectangle(new Point(origStartPointX, origStartPointY), new Size(cutWidth, cutHeight));

                graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
                partImg.Save(savedFile, ici, ep);

                // 资源释放
                graphics.Dispose();
                partImg.Dispose();
            }
        }
    }
}