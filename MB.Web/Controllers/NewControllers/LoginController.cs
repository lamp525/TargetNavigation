using MB.Facade.Login;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MB.Web.Controllers.NewControllers
{
    public class LoginController : BaseController
    {
        private ILoginFacade facade { set; get; }

        public ActionResult Index()
        {
            ViewBag.time = ConfigHelper.InputErrorValidate;
            FormsAuthentication.SignOut();
            return View();
        }

        /// <summary>
        /// 客户端跳转登陆处理
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public ActionResult ClientLogin(string userName, string password)
        {
            userName = HttpUtility.UrlDecode(userName);
            password = HttpUtility.UrlDecode(password);
            ViewBag.UserName = userName;
            ViewBag.Password = password;
            return View();
        }

        /// <summary>
        /// 用户登录处理
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="vCode"></param>
        /// <returns></returns>
        public string UserLogin(string userName, string password, string vCode)
        {
            var loginResult = facade.DoLogin(userName, password, vCode);

            JsonResultModel result = null;

            //登录成功
            if (loginResult == EnumDefine.UserLoginResult.Succeed)
                result = new JsonResultModel(JsonResultType.success, null);

            //登录失败
            else
                result = new JsonResultModel(JsonResultType.error, loginResult);

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 生成验证码图片文件
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateCode()
        {
            //从Cookie中取得校验用验证码
            var verifyVCode = CookieHelper.GetValue(ConstVar.CookieName_VerifyVCode);

            //校验用验证码不存在的场合
            if (string.IsNullOrEmpty(verifyVCode)) return null;

            var bytes = facade.CreateValidateGraphic(verifyVCode);

            //返回验证码图片文件
            return File(bytes, @"image/jpeg");
        }
    }
}