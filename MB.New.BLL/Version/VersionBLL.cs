using MB.DAL;
using MB.New.Model.Version;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Version
{
    public class VersionBLL : IVersionBLL
    {
        /// <summary>
        /// 取得版本信息
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<VersionModel> GetVersionList(TargetNavigationDBEntities db)
        {
            var versionInfo = db.tblVersion.OrderByDescending(v => v.updateTime).Select(v => new VersionModel
            {
                content = v.content,
                number = v.number,
                updateTime = v.updateTime,
                versionId = v.versionId
            }).ToList();

            return versionInfo;
        }
    }
}