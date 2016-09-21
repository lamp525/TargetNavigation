using MB.Web.Models;
using System.Web.Mvc;

using MB.Web.Common;

namespace MB.Web.Controllers
{
    public class ChatController : BaseController
    {
        //
        // GET: /Chat/

        public ActionResult Chat()
        {
            return View();
        }
    }
}