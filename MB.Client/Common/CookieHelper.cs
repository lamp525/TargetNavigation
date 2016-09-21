using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MB.Client.Common
{
    public static class CookieHelper
    {
        /// <summary>
        /// 写入函数
        /// </summary>
        /// <param name="lpszUrlName"></param>
        /// <param name="lbszCookieName"></param>
        /// <param name="lpszCookieData"></param>
        /// <returns></returns>
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        /// <summary>
        /// 读取函数
        /// </summary>
        /// <param name="lpszUrlName"></param>
        /// <param name="lbszCookieName"></param>
        /// <param name="lpszCookieData"></param>
        /// <param name="lpdwSize"></param>
        /// <returns></returns>
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string lpszUrlName, string lbszCookieName, StringBuilder lpszCookieData, ref int lpdwSize);

        /// <summary>
        /// 检测错误函数
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieName"></param>
        public static void DeleteCookie(string url,string cookieName)
        {
            InternetSetCookie(url, cookieName, "Sun,22-Feb-1970 00:00:00 GMT");
        }

    }
}
