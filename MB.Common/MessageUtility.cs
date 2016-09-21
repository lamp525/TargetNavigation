using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace MB.Common
{
    public class MessageUtility
    {
        #region Private变量

        /// <summary>
        /// 信息列表
        /// </summary>
        private static DataSet messageStore;

        /// <summary>
        /// 文件更新时间
        /// </summary>
        private static DateTime fileUpdate;

        #endregion Private变量

        #region 属性

        /// <summary>
        /// 信息文件路径
        /// </summary>
        private static string FilePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["MessagePath"] != null)
                {
                    return System.Web.HttpContext.Current.Server.MapPath("~")
                            + "\\"
                            + ConfigurationManager.AppSettings["MessagePath"].ToString();
                }
                else
                {
                    return System.Web.HttpContext.Current.Server.MapPath("~")
                            + "\\"
                            + "Message.xml";
                }
            }
        }

        #endregion 属性

        #region Public函数

        /// <summary>
        /// 信息取得
        /// </summary>
        /// <param name="messageId">信息Key</param>
        /// <returns>信息</returns>
        public static string GetMessage(string messageId)
        {
            string messageText = string.Empty;

            // 读取XML文件
            ReadXmlFile();

            if (messageStore == null || messageStore.Tables.Count == 0)
            {
                return "信息文件取得失败，请确认信息文件路径以及文件名！";
            }

            // 根据key取得信息内容
            DataRow[] row_MessageInfo = messageStore.Tables[0].Select(string.Format("key = '{0}'", messageId));

            // 信息存在的情况
            if (row_MessageInfo.Length > 0)
            {
                messageText = row_MessageInfo[0]["text"].ToString().Trim();
            }
            else
            {
                messageText = string.Format("信息[{0}]不存在", messageId);
            }

            return messageText;
        }

        /// <summary>
        /// 信息取得
        /// </summary>
        /// <param name="messageId">信息Key</param>
        /// <param name="param">参数</param>
        /// <returns>信息</returns>
        public static string GetMessage(string messageId, params string[] param)
        {
            string str_MessageText = string.Empty;

            // 读取XML文件
            ReadXmlFile();

            if (messageStore == null || messageStore.Tables.Count == 0)
            {
                return "信息文件取得失败，请确认信息文件路径以及文件名！";
            }

            // 根据key取得信息内容
            DataRow[] messageInfo = messageStore.Tables[0].Select(string.Format("key = '{0}'", messageId));

            // 信息存在的情况
            if (messageInfo.Length > 0)
            {
                str_MessageText = string.Format(messageInfo[0]["text"].ToString().Trim(), param);
            }
            else
            {
                str_MessageText = string.Format("信息[{0}]不存在", messageId);
            }

            return str_MessageText;
        }

        #endregion Public函数

        #region Private函数

        /// <summary>
        /// 读取XML
        /// </summary>
        private static void ReadXmlFile()
        {
            if (!File.Exists(FilePath))
            {
                throw new IOException("信息文件取得失败，请确认信息文件存放地址！");
            }

            // 文件最后更新日期取得
            DateTime fileTime = File.GetLastWriteTime(FilePath);

            // 如果文件更新时间不等于记录时间，重新读取XML文件
            if (fileTime != fileUpdate)
            {
                // 更新文件时间
                fileUpdate = fileTime;

                if (messageStore != null)
                {
                    messageStore.Clear();
                }
                else
                {
                    messageStore = new DataSet();
                }

                // 重新读取XML文件
                messageStore.ReadXml(FilePath);
            }
        }

        #endregion Private函数
    }
}