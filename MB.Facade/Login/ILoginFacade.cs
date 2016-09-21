using MB.New.Common;

namespace MB.Facade.Login
{
    public interface ILoginFacade
    {
        /// <summary>
        /// 用户登录处理
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="validateCode"></param>
        /// <returns>登录处理结果</returns>
        EnumDefine.UserLoginResult DoLogin(string userName, string password, string validateCode);

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        byte[] CreateValidateGraphic(string validateCode);
    }
}