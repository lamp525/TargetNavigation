using MB.Web.Models;
using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MB.Web.Common;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class DownloadController : BaseController
    {
        //
        // GET: /Download/

        public ActionResult DownLoad()
        {
            string path = "";
            //Define the FileStream Class
            FileStream myFile = null;
            //Define the File Read Class
            BinaryReader reader = null;

            string displayName = Request.QueryString["displayname"];

            try
            {
                myFile = new FileStream(path, FileMode.Open);
                //Set the File Binary Reader
                reader = new BinaryReader(myFile);

                //Get the DownLoad File Size
                long fileLength = myFile.Length;
                //Set Read Start position
                long startBytes = 0;
                //Set The Read Size Every Time = 10K
                int everyReadPack = 10240;
                //Set Sleep Time
                double temp = 1000 * everyReadPack / 102400;
                int sleepTime = (int)Math.Floor(temp) + 1;

                #region Set Response's Header For Down

                //Set the Response Accept-Ranges
                Response.AddHeader("Accept-Ranges", "bytes");
                Response.Buffer = false;
                if (Request.Headers["Range"] != null)
                {
                    Response.StatusCode = 206;
                    string[] str_Range = Request.Headers["Range"].Split(new char[] { '=', '-' });
                    startBytes = Convert.ToInt64(str_Range[1]);
                }
                //Set Response Content-Length
                Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                if (startBytes != 0)
                {
                    Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                }
                //Set Response Header For File Down
                Response.AddHeader("Connection", "Keep-Alive");
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(displayName, System.Text.Encoding.UTF8).Replace("+", "%20"));

                #endregion Set Response's Header For Down

                #region Read the File Byte To Response By Loop

                reader.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                if (fileLength > 0)
                {
                    //Get The File's Block Divde By The Pack
                    double dbl_FileBlockCount = (fileLength - startBytes) / everyReadPack;
                    int int_MaxBlockCount = (int)Math.Floor(dbl_FileBlockCount) + 1;
                    //Loop Begain to Read
                    for (int i = 0; i < int_MaxBlockCount; i++)
                    {
                        if (Response.IsClientConnected)
                        {
                            Response.BinaryWrite(reader.ReadBytes(everyReadPack));

                            Response.Flush();
                            Thread.Sleep(sleepTime);
                        }
                        //No Connected Break The Loop
                        else
                        {
                            i = int_MaxBlockCount;
                        }
                    }
                }

                #endregion Read the File Byte To Response By Loop
            }
            catch (Exception)
            {
                // Response.Write(CMessage.CmnGetMessage("99001"));
                //  CLog.Output("download_Page_Load", EErrorLevel.ERROR, ex);
            }
            finally
            {
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (myFile != null)
                {
                    myFile.Close();
                    myFile.Dispose();
                }
            }
            return View();
        }
    }
}