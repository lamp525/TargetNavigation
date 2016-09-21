using MB.DAL;
using MB.New.BLL.Organization;
using MB.New.BLL.User;
using MB.New.Common;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MB.Facade.Login
{
    public class LoginFacade : ILoginFacade
    {
        /// <summary>
        /// 用户登录处理
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="validateCode"></param>
        /// <returns>登录处理结果</returns>
        public EnumDefine.UserLoginResult DoLogin(string userName, string password, string validateCode)
        {
            IUserBLL User = new UserBLL();
            IOrganizationBLL Organization = new OrganizationBLL();

            //用户名为空
            if (string.IsNullOrEmpty(userName)) return EnumDefine.UserLoginResult.BlankUserName;

            //密码为空
            if (string.IsNullOrEmpty(password)) return EnumDefine.UserLoginResult.BlankPwd;

            //从Cookie中取得校验用验证码
            var verifyVCode = CookieHelper.GetValue(ConstVar.CookieName_VerifyVCode);

            //校验用验证码存在的场合
            if (!string.IsNullOrEmpty(verifyVCode))
            {
                //验证码为空
                if (string.IsNullOrEmpty(validateCode)) return EnumDefine.UserLoginResult.BlankVCode;

                //验证码校验不通过
                if (!validateCode.Equals("false") && !validateCode.Equals(verifyVCode)) return EnumDefine.UserLoginResult.VCodeError;
            }

            using (var db = new TargetNavigationDBEntities())
            {
                //取得登录用户名对应的用户信息
                var userInfoList = User.GetUserInfoByName(db, userName);

                //登录用户不存在
                if (userInfoList == null || userInfoList.Count == 0) return EnumDefine.UserLoginResult.UserNotExist;

                //登录用户存在重名
                if (userInfoList.Count > 1) return EnumDefine.UserLoginResult.DuplicationUserName;

                //登录用户信息
                var userInfo = userInfoList.First();

                //密码错误次数等于或超过系统设定值
                if (userInfo.errorPassword >= ConstVar.WrongPwdNum)
                {
                    // 生成校验用验证码
                    var code = GenerateValidateCode();

                    //保存校验用验证码到Cookie
                    CookieHelper.SetObj(ConstVar.CookieName_VerifyVCode, 1, code, ConstVar.WebHostURL);
                }

                //登录密码有效的场合
                if (userInfo.password.Equals(EncryptHelper.PwdEncrypt(password, userInfo.randomKey)))
                {
                    //重置登录用户密码错误次数为0
                    User.UpdUserWrongPwdNum(db, userInfo.userId, isReset: true);

                    //设置用户的默认岗位ID
                    User.SetUserDefaultStation(db, userInfo.userId);

                    db.SaveChanges();

                    //设置用户权限信息
                    SetUserAuth(userInfo.userId, userInfo.isAdmin);

                    //保存用户信息到Cookie
                    NameValueCollection KeyValue = new NameValueCollection();
                    //用户ID
                    KeyValue.Set(ConstVar.CookieKey_UserID, userInfo.userId.ToString());
                    //岗位ID
                    KeyValue.Set(ConstVar.CookieKey_StationID, userInfo.defaultStationId.ToString());
                    //部门ID

                    var orgInfo = Organization.GetOrgInfoByStationId(db, userInfo.defaultStationId.Value);
                    if (orgInfo != null)
                    {
                        KeyValue.Set(ConstVar.CookieKey_OrgID, orgInfo.organizationId.Value.ToString());
                    }
                    CookieHelper.SetObj(ConstVar.CookieName_UserInfo, 1, KeyValue, ConstVar.WebHostURL);

                    //删除Cookie中的校验验证码
                    CookieHelper.Del(ConstVar.CookieName_VerifyVCode, ConstVar.WebHostURL);

                    return EnumDefine.UserLoginResult.Succeed;
                }
                //登录密码无效的场合
                else
                {
                    //累加登录用户密码错误次数
                    User.UpdUserWrongPwdNum(db, userInfo.userId, isReset: false);
                    db.SaveChanges();

                    //密码错误次数等于或超过系统设定值
                    if (userInfo.errorPassword >= ConstVar.WrongPwdNum)
                        return EnumDefine.UserLoginResult.WrongPwdOverTime;
                    else
                        return EnumDefine.UserLoginResult.PwdError;
                }
            }
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public byte[] CreateValidateGraphic(string code)
        {
            ValidateCode validateCode = new ValidateCode();
            return validateCode.CreateValidateGraphic(code);
        }

        #region 私有方法

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        private string GenerateValidateCode()
        {
            ValidateCode validateCode = new ValidateCode();
            return validateCode.CreateValidateCode(4);
        }

        /// <summary>
        /// 设置用户权限信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="admin"></param>
        private void SetUserAuth(int userId, bool? admin)
        {
            string roles = string.Empty;

            FormsAuthentication.SetAuthCookie(userId.ToString(), false);

            if (admin.HasValue && admin.Value)
            {
                roles = "Admin";
            }
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userId.ToString(), DateTime.Now, DateTime.Now.AddDays(1), false, roles);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        #endregion 私有方法
    }
}