using MB.Facade.User;
using MB.New.Common;
using MB.Web.Common;
using MB.Web.Models;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace MB.Web.Controllers.NewControllers
{
    [UserAuthorize]
    public class UserController : BaseController
    {
        //
        // GET: /User/

        private IUserFacade facade { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取头部个人用户信息
        /// </summary>
        /// <returns></returns>
        public string GetHeadUserInfo()
        {
            var result = facade.GetHeadUserInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户部门信息（部门从属连接显示）
        /// </summary>
        /// <returns></returns>
        public string GetUserOrgInfo()
        {
            var result = facade.GetUserOrgInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得常用联系人信息
        /// </summary>
        /// <param name="type">常用联系人类型</param>
        /// 1：上级
        /// 2：下属
        /// 99：全部
        /// <returns></returns>
        public string GetTopContacts(EnumDefine.TopContactsType type)
        {
            var result = facade.GetTopContacts(LoginUserInfo().userId, type);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 人员模糊检索
        /// </summary>
        /// <returns></returns>
        public string UserNameFuzzySearch(string userName)
        {
            var result = facade.UserNameFuzzySearch(userName);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户原头像
        /// </summary>
        /// <returns></returns>
        public string GetUserOriginalImage()
        {
            var result = facade.GetUserOriginalImage(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 保存用户头像
        /// </summary>
        /// <returns></returns>
        public string SaveUserHead()
        {
            var userId = LoginUserInfo().userId;
            var info = JsonConvert.DeserializeObject<PageUserHeadImageModel>(Request.Form["data"]);
            var result = facade.SaveUserHead(userId, info);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "头像保存成功！");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public string ChangePassword()
        {
            var info = JsonConvert.DeserializeObject<PageUserPasswordModel>(Request.Form["data"]);
            info.userId = LoginUserInfo().userId;
            var result = facade.ChangePassword(info);
            var jsonResult = new JsonResultModel(JsonResultType.success, result, "密码修改成功！");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 岗位切换
        /// </summary>
        /// <returns></returns>
        public string ChangeStation()
        {
            var info = JsonConvert.DeserializeObject<PageUserInfoSimpleModel>(Request.Form["data"]);

            if (info != null && info.stationId.HasValue && info.orgId.HasValue)
            {
                //保存用户信息至Session
                Session["stationId"] = info.stationId;
                Session["stationName"] = info.stationName;
                Session["orgId"] = info.orgId;
                Session["orgName"] = info.orgName;

                //保存用户信息至Cookie
                NameValueCollection keyValue = new NameValueCollection();
                keyValue.Set(ConstVar.CookieKey_StationID, info.stationId.Value.ToString());
                keyValue.Set(ConstVar.CookieKey_OrgID, info.orgId.Value.ToString());
                CookieHelper.SetObj(ConstVar.CookieName_UserInfo, 1, keyValue, ConstVar.WebHostURL);

                //更新用户默认岗位
                facade.UpdUserDefaultStation(LoginUserInfo().userId, info.stationId.Value);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, null);
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 取得用户岗位（带部门）信息
        /// </summary>
        /// <returns></returns>
        public string GetUserStationInfo()
        {
            var result = facade.GetUserOrgStationInfo(LoginUserInfo().userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, result);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}