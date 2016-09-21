using MB.DAL;
using MB.New.Model.Version;
using System.Collections.Generic;

namespace MB.New.BLL.Version
{
    public interface IVersionBLL
    {
        /// <summary>
        /// 取得版本信息
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        List<VersionModel> GetVersionList(TargetNavigationDBEntities db);
    }
}