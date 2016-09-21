using System.Collections.Generic;

namespace MB.Facade.Version
{
    public interface IVersionFacade
    {
        /// <summary>
        /// 取得版本更新信息
        /// </summary>
        /// <returns></returns>
        List<PageVersionModel> GetVersionList();
    }
}