using MB.Web.Models;
using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MB.Web.Common
{
    public class UserAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private new string[] Roles { get; set; }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            var loginUser = filterContext.HttpContext.Session["userId"];

            if (loginUser == null)
            {
                if (System.Web.Mvc.AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
                {
                    var jsonResult = new JsonResultModel(JsonResultType.success, null, "登录失效，请重新登录", false);
                    filterContext.Result = new System.Web.Mvc.JsonResult { Data = new { success = true, login = false, data = "", message = "" }, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Login", action = "Login" }));
                }
                return;
            }

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            string roles = GetActionRoles(actionName, controllerName);

            if (!string.IsNullOrWhiteSpace(roles))
            {
                this.Roles = roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            if (Roles == null)
            {
                return true;
            }

            if (Roles.Length == 0)
            {
                return true;
            }

            if (Roles.Any(httpContext.User.IsInRole))
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (System.Web.Mvc.AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, string.Empty, string.Empty, true, false);
                filterContext.Result = new System.Web.Mvc.JsonResult { Data = jsonResult, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
                return;
            }

            filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Login", action = "Login" }));
        }

        private string GetActionRoles(string action, string controller)
        {
            string rolesXml = HttpContext.Current.Server.MapPath("/") + "ActionRoles.xml";

            if (!System.IO.File.Exists(rolesXml))
            {
                throw new System.IO.FileNotFoundException("权限设置文件不存在", rolesXml);
            }

            XElement rootElement = XElement.Load(rolesXml);
            XElement controllerElement = findElementByAttribute(rootElement, "Controller", controller);

            if (controllerElement != null)
            {
                XElement actionElement = findElementByAttribute(controllerElement, "Action", action);
                if (actionElement != null)
                {
                    return actionElement.Value;
                }
            }
            return string.Empty;
        }

        private XElement findElementByAttribute(XElement xElement, string tagName, string attribute)
        {
            return xElement.Elements(tagName).FirstOrDefault(x => x.Attribute("name").Value.Equals(attribute, StringComparison.OrdinalIgnoreCase));
        }
    }
}