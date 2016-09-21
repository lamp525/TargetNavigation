using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface ICityManagementBLL
    {
        //新建或修改
        void SaveCity(CityManagement cityModel);

        void DeleteCity(int cityId);

        //查询
        List<CityManagement> GetCityList(string cityName);
    }
}