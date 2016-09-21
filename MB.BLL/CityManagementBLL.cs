using System.Collections.Generic;
using System.Linq;

using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class CityManagementBLL : ICityManagementBLL
    {
        //新增或修改
        public void SaveCity(CityManagement cityModel)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var cityId = db.prcGetPrimaryKey("tblCity", obj).FirstOrDefault().Value;

                var model = db.tblCity.Where(c => c.cityId == cityModel.cityId).FirstOrDefault();
                //新增
                if (model == null)
                {
                    var model2 = new tblCity
                    {
                        cityId = cityId,
                        cityName = cityModel.cityName,
                    
                        provinceId = cityModel.provinceId
                    };
                    db.tblCity.Add(model2);
                }
                else
                {//修改
                    model.cityName = cityModel.cityName;
                  
                    model.provinceId = cityModel.provinceId;
                }
                db.SaveChanges();
            }
        }

        //删除
        public void DeleteCity(int cityId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblCity.Where(c => c.cityId == cityId).FirstOrDefault();
                if (model != null)
                {
                    db.tblCity.Remove(model);
                }
                db.SaveChanges();
            }
        }

        //查询
        public List<CityManagement> GetCityList(string cityName)
        {
            var cityList = new List<CityManagement>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (cityName == null)
                {
                    cityList = (from city in db.tblCity
                                join province in db.tblProvince on city.provinceId equals province.provinceId
                                select new CityManagement
                                {
                                    cityId = city.cityId,
                                    cityName = city.cityName, 
                                    provinceId = city.provinceId
                                }).ToList();
                }
                else
                {
                    cityList = (from city in db.tblCity
                                join province in db.tblProvince on city.provinceId equals province.provinceId
                                where city.cityName.IndexOf(cityName) != -1
                                select new CityManagement
                                {
                                    cityId = city.cityId,
                                    cityName = city.cityName, 
                                    provinceId = city.provinceId
                                }).ToList();
                }
            }
            return cityList;
        }
    }
}