using MB.Common;
using System.Web.Mvc;

namespace MB.Web.Controllers
{
    public class ValidateCodeController : Controller
    {
        //
        // GET: /ValidateCode/
        public ActionResult ValidateCode()
        {
            //生成验证码
            ValidateCode validateCode = new ValidateCode();
            string code = validateCode.CreateValidateCode(4);
            Session["ValidateCode"] = code;
            byte[] bytes = validateCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}