using MB.New.Common;
using MB.Web.Models;
using System.Web.Mvc;

namespace MB.Web.Common
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string message = string.Format("Action Start Paramters: {0}", filterContext.ActionParameters.ToString());
            LogHelper.Output(Request.Url.LocalPath, EnumDefine.ErrorLevel.DEBUG, message);
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ContentResult contentResult = filterContext.Result as ContentResult;
            string result = string.Empty;

            if (contentResult != null)
            {
                result = contentResult.Content;
            }

            string message = string.Format("Controller:{0} Action:{1} Result:{2} Action End", filterContext.RouteData.Values["controller"], filterContext.RouteData.Values["action"], result);
            LogHelper.Output(Request.Url.LocalPath, EnumDefine.ErrorLevel.DEBUG, message);
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Result执行前
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string message = string.Format("Controller:{0} Action:{1} Result Start", filterContext.RouteData.Values["controller"], filterContext.RouteData.Values["action"]);
            LogHelper.Output(Request.Url.LocalPath, EnumDefine.ErrorLevel.DEBUG, message);
            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// Result执行后
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string message = string.Format("Controller:{0} Action:{1} Result End", filterContext.RouteData.Values["controller"], filterContext.RouteData.Values["action"]);
            LogHelper.Output(Request.Url.LocalPath, EnumDefine.ErrorLevel.DEBUG, message);
            base.OnResultExecuted(filterContext);
        }

        /// <summary>
        /// 异常发生时
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            LogHelper.Output(Request.Url.LocalPath, EnumDefine.ErrorLevel.ERROR, filterContext.Exception);

            filterContext.ExceptionHandled = true;
            filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Error", action = "Shared" }));
        }

        #region 取得登录用户信息

        /// <summary>
        /// 取得登录用户信息
        /// </summary>
        /// <returns></returns>
        protected LoginInfoModel LoginUserInfo()
        {
            var loginInfo = new LoginInfoModel
            {
                //用户ID
                userId = int.Parse(Session["userId"].ToString()),

                //用户名称
                userName = Session["userName"].ToString(),

                //岗位ID
                //stationId = int.Parse(Session["stationId"].ToString()),

                //岗位名称
                //stationName = Session["stationName"].ToString();

                //部门ID
                //orgId = int.Parse(Session["orgId"].ToString())

                //部门名称
                //orgName = Session["orgName"].ToString(),
            };

            return loginInfo;
        }

        #endregion 取得登录用户信息

        #region 从Cookie中取得用户信息

        /// <summary>
        /// 用户ID
        /// </summary>
        protected int? UserId
        {
            get
            {
                var value = CookieHelper.GetValue(ConstVar.CookieName_UserInfo, ConstVar.CookieKey_UserID);
                if (string.IsNullOrEmpty(value)) return null;

                return int.Parse(value);
            }
        }

        /// <summary>
        ///岗位ID
        /// </summary>
        protected int? StationId
        {
            get
            {
                var value = CookieHelper.GetValue(ConstVar.CookieName_UserInfo, ConstVar.CookieKey_StationID);
                if (string.IsNullOrEmpty(value)) return null;

                return int.Parse(value);
            }
        }

        /// <summary>
        /// 部门ID
        /// </summary>
        protected int? OrgId
        {
            get
            {
                var value = CookieHelper.GetValue(ConstVar.CookieName_UserInfo, ConstVar.CookieKey_OrgID);
                if (string.IsNullOrEmpty(value)) return null;

                return int.Parse(value);
            }
        }

        #endregion 从Cookie中取得用户信息

        #region 服务器根目录物理地址

        /// <summary>
        /// 服务器根目录物理地址
        /// </summary>
        protected string WebHostPhysicalPath
        {
            get
            {
                return System.Web.HttpContext.Current.Server.MapPath("~");
            }
        }

        #endregion 服务器根目录物理地址
    }
}