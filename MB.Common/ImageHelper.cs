using System.Drawing;
using System.Drawing.Imaging;

namespace MB.Common
{
    public class ImageHelper
    {
        /// <summary>
        /// 获取图片指定部分
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <param name="savedFile">保存文件</param>
        /// <param name="partWidth">目标图片的宽度</param>
        /// <param name="partHeight">目标图片的高度</param>
        /// <param name="origStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="origStartPointY">原始图片开始截取处的坐标Y值</param>
        /// <param name="cutWidth">截取图片的宽度</param>
        /// <param name="cutHeight">截取图片的高度</param>
        /// <param name="format">保存格式，通常可以是jpeg</param>
        public void GetPartImage(string path/*图片路径*/, string savedFile/*保存文件*/, int partWidth/*目标图片的宽度*/, int partHeight/*目标图片的高度*/, int origStartPointX/*原始图片开始截取处的坐标X值*/, int origStartPointY/*原始图片开始截取处的坐标Y值*/, int cutWidth/*截取图片的宽度*/, int cutHeight/*截取图片的高度*/, ImageFormat format/*保存格式，通常可以是jpeg*/)
        {
            Image originalImg = Image.FromFile(path);

            Bitmap partImg = new Bitmap(partWidth, partHeight);
            Graphics graphics = Graphics.FromImage(partImg);

            //目标位置
            Rectangle destRect = new Rectangle(new Point(0, 0), new Size(partWidth, partHeight));
            //原图位置（默认从原图中截取的图片大小等于目标图片的大小）
            Rectangle origRect = new Rectangle(new Point(origStartPointX, origStartPointY), new Size(cutWidth, cutHeight));

            graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
            partImg.Save(savedFile, format);
        }
    }
}