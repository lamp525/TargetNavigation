using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IProvinceBLL
    {
        List<ProvinceModel> GetProvinceList();

        bool AddProvince(List<ProvinceModel> addlist);

        bool DeleteProvince(List<int> addlist);
    }
}