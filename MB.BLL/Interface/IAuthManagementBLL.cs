using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IAuthManagementBLL
    {
        /// <summary>
        /// 获取权限设置列表
        /// </summary>
        /// <param name="authName"></param>
        /// <returns></returns>
        List<AuthShift> GetAuthList(string authName = null);

        /// <summary>
        /// 权限新建和修改
        /// </summary>
        /// <param name="authModel"></param>
        void SaveAuth(AuthShift authModel);

        /// <summary>
        /// 删除权限设置
        /// </summary>
        /// <param name="id"></param>
        void DeleteAuth(int id);

        /// <summary>
        /// 权限转移
        /// </summary>
        /// <param name="authShift"></param>
        void AuthShift(AuthShift authShift);
    }
}