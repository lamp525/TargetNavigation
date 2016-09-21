using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using Newtonsoft.Json;
using MB.Web.Models;

namespace MB.Web.Controllers
{
    public class AuthManagementController : Controller
    {
        private IAuthManagementBLL authManagementBLL { get; set; }

        //
        // GET: /AuthManagement/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AuthManagement()
        {
            return View();
        }

        /// <summary>
        /// 获取权限设置列表
        /// </summary>
        /// <returns></returns>
        public string GetAuthList()
        {
            var modelList = authManagementBLL.GetAuthList();
            var jsonResult = new JsonResultModel(JsonResultType.success, modelList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 新增或修改权限
        /// </summary>
        /// <returns></returns>
        public string SaveAuth()
        {
            var data = Request.Form["data"];
            var authModel = JsonConvert.DeserializeObject<AuthShift>(data);
            authManagementBLL.SaveAuth(authModel);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除权限设置
        /// </summary>
        /// <returns></returns>
        public string DeleteAuth(int id)
        {
            authManagementBLL.DeleteAuth(id);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 权限转移
        /// </summary>
        /// <returns></returns>
        public string AuthShift()
        {
            var data = Request.Form["data"];
            var authModel = JsonConvert.DeserializeObject<AuthShift>(data);
            authManagementBLL.AuthShift(authModel);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}