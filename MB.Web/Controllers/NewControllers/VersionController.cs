using MB.Facade.Version;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    public class VersionController : BaseController
    {
        //
        // GET: /Version/
        private IVersionFacade facade { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public string GetVersionList()
        {
            var list = facade.GetVersionList();
            var jsonResult = new JsonResultModel(JsonResultType.success, list);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}