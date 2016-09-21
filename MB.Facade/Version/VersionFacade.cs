using MB.DAL;
using MB.New.BLL.Version;
using MB.New.Common;
using MB.New.Model.Version;
using System.Collections.Generic;
using System.Linq;

namespace MB.Facade.Version
{
    internal class VersionFacade : IVersionFacade
    {
        /// <summary>
        /// 取得版本更新信息
        /// </summary>
        /// <returns></returns>
        public List<PageVersionModel> GetVersionList()
        {
            IVersionBLL versionBLL = new VersionBLL();
            List<VersionModel> versionModel = null;
            var pageModel = new List<PageVersionModel>();

            // 取得版本信息数据
            using (var db = new TargetNavigationDBEntities())
            {
                versionModel = versionBLL.GetVersionList(db);
            }

            // 内容分开、日期格式转换
            pageModel = versionModel.Select(v => new PageVersionModel
            {
                contents = v.content.Split(';'),
                number = v.number,
                updateTime = v.updateTime.Value.ToString("yyyy-MM-dd")
            }).ToList();

            return pageModel;
        }
    }
}