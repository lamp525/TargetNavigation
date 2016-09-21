using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.DAL;
using MB.New.Model.AuthManagement;
using MB.New.BLL.AuthManagement;

namespace MB.Facade.AuthManagement
{
    public class AuthManagentFacade
    {
        AuthManagementBLL AuthManagementBLL = new AuthManagementBLL();

        #region 权限设置查询
        /// <summary>
        /// 权限设置查询
        /// </summary>
        /// <param name="authName">权限名</param>
        /// <returns></returns>
        public List<AuthShift> GetAuthList(string authName = null)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                return AuthManagementBLL.GetAuthList(db, authName);
            }
        }
        #endregion

        #region 权限新建和修改
        /// <summary>
        ///  权限新建和修改
        /// </summary>
        /// <param name="authModel"></param>
        public void SaveAuth(AuthShift authModel)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                AuthManagementBLL.SaveAuth(authModel,db);
            }
        }
        #endregion

        #region 删除权限

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        public void DeleteAuth(int id)
        {
            using(var db=new TargetNavigationDBEntities ())
            {
                AuthManagementBLL.DeleteAuth(id,db);
            }
        }

        #endregion

        #region 权限转移
        /// <summary>
        ///  权限转移
        /// </summary>
        /// <param name="authShift"></param>
        public void AuthShift(AuthShift authShift)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                AuthManagementBLL.AuthShift(authShift,db);
            }
        }
        #endregion
    }
}
