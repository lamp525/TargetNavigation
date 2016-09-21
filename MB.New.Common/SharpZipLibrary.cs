using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace MB.New.Common
{
    public class CompressInfo
    {
        public string folder { get; set; }
        public string path { get; set; }
        public string display { get; set; }
    }

    public class SharpZipLibrary
    {
        /// <summary>
        /// 不带密码的压缩包
        /// </summary>
        /// <param name="compressList"></param>
        /// <returns></returns>
        public MemoryStream Compress(List<CompressInfo> compressList)
        {
            return Compress(compressList, null);
        }

        /// <summary>
        /// 带密码的压缩包
        /// </summary>
        /// <param name="compressList"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public MemoryStream Compress(List<CompressInfo> compressList, string password)
        {
            MemoryStream memoryFile = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(memoryFile);

            zipStream.SetLevel(6);
            zipStream.Password = password;

            foreach (CompressInfo target in compressList)
            {
                CompressFile(target, zipStream);
            }

            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;
            //close
            zipStream.Finish();
            zipStream.Close();

            memoryFile.Position = 0;
            return memoryFile;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="target">待压缩的文件信息</param>
        /// <param name="zipStream">压缩包流</param>
        /// <returns></returns>
        private void CompressFile(CompressInfo target, ZipOutputStream zipStream)
        {
            if (!File.Exists(target.path))
            {
                return;
            }

            //打开压缩文件
            using (FileStream fs = File.OpenRead(target.path))
            {
                ZipEntry entry = new ZipEntry(target.folder + target.display);
                entry.DateTime = DateTime.Now;
                entry.Size = fs.Length;
                zipStream.PutNextEntry(entry);
                StreamUtils.Copy(fs, zipStream, new byte[4096]);
            }
        }
    }
}