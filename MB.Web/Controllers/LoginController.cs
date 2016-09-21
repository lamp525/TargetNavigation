using MB.BLL;
using MB.Common;
using MB.Model;
using MB.Web.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MB.Web.Controllers
{
    public class LoginController : Controller
    {
        private IUserBLL UserBLL { get; set; }

        //
        // GET: /Login/
        //UserBLL userBLL = new UserBLL();
        public ActionResult Login()
        {
            ViewBag.time = MB.Web.Common.ConfigHelper.InputErrorValidate;
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("tnCookie", ""));
            return View();
        }

        //客户端跳转的情况
        public ActionResult ClientLogin(string userName, string password)
        {
            userName = HttpUtility.UrlDecode(userName);
            password = HttpUtility.UrlDecode(password);
            ViewBag.UserName = userName;
            ViewBag.Password = password;
            return View();
        }

        public string UserLogin(string userId, string userPass, string VCode)
        {
            UserInfo userList = new UserInfo();
            UserBLL userBLL = new UserBLL();
            string validateCode = null;
            userList = userBLL.UserLogin(userId);

            Session["userName"] = userId;

            if (userList == null)
            {
                return AjaxCallBack.FAIL;
            }

            //if (Session["ValidateCode"] != null)
            //{
            //    validateCode = Session["ValidateCode"].ToString();
            //}
            if (userList.errorPass >= MB.Web.Common.ConfigHelper.InputErrorValidate)
            {
                //VCode==false:登录次数超过3次，但是刷新页面后验证码隐藏的情况
                if (VCode == "false")
                {
                    if (userList.password == EncryptHelper.PwdEncrypt(userPass, userList.randomKey))
                    {
                        Session["userId"] = userList.userId;
                        Session["admin"] = userList.admin;
                        Session["stationId"] = userList.stationId;
                        Response.Cookies["userId"].Value = userList.userId.ToString();
                        Response.Cookies["userId"].Expires = DateTime.Now.AddDays(30);
                        userBLL.UpUserLoginWorryNum(true, userId);
                        SetUserAuth(userList.userId, userList.admin);
                        return AjaxCallBack.OK;
                    }
                    else
                    {
                        return userList.errorPass.ToString();
                    }
                }
                else
                {
                    validateCode = Session["ValidateCode"].ToString();
                    if (validateCode.Equals(VCode))
                    {
                        if (userList.password == EncryptHelper.PwdEncrypt(userPass, userList.randomKey))
                        {
                            Session["userId"] = userList.userId;
                            Session["admin"] = userList.admin;
                            Session["stationId"] = userList.stationId;
                            Response.Cookies["userId"].Value = userList.userId.ToString();
                            Response.Cookies["userId"].Expires = DateTime.Now.AddDays(30);
                            userBLL.UpUserLoginWorryNum(true, userId);
                            SetUserAuth(userList.userId, userList.admin);
                            return AjaxCallBack.OK;
                        }
                        else
                        {
                            userBLL.UpUserLoginWorryNum(false, userId);
                            return userList.errorPass.ToString();
                        }
                    }
                    else
                    {
                        userBLL.UpUserLoginWorryNum(false, userId);
                        return AjaxCallBack.Vf;
                    }
                }
            }
            else
            {
                // 验证码输入不正确
                //if (!validateCode.ToLower().Equals(VCode.ToLower()))
                //{
                //    return AjaxCallBack.FAIL;
                //}
                if (userList.password == EncryptHelper.PwdEncrypt(userPass, userList.randomKey))
                {
                    Session["userId"] = userList.userId;
                    Session["admin"] = userList.admin;
                    Session["stationId"] = userList.stationId;
                    Response.Cookies["userId"].Value = userList.userId.ToString();
                    Response.Cookies["userId"].Expires = DateTime.Now.AddDays(30);
                    userBLL.UpUserLoginWorryNum(true, userId);

                    SetUserAuth(userList.userId, userList.admin);
                    return AjaxCallBack.OK;
                }
                else
                {
                    userBLL.UpUserLoginWorryNum(false, userId);
                    return userList.errorPass.ToString();
                }
            }
        }

        public ActionResult BrowserView()
        {
            return View("~/Views/Browser/Browser.cshtml");
        }

        private void SetUserAuth(int userId, bool admin)
        {
            string roles = string.Empty;

            FormsAuthentication.SetAuthCookie(userId.ToString(), false);

            if (admin)
            {
                roles = "Admin";
            }
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userId.ToString(), DateTime.Now, DateTime.Now.AddDays(1), false, roles);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            this.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }
    }
}