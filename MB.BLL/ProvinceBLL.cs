using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class ProvinceBLL : IProvinceBLL
    {
        public List<ProvinceModel> GetProvinceList()
        {
            var list = new List<ProvinceModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = (from p in db.tblProvince
                        select new ProvinceModel
                        {
                            provinceId = p.provinceId,
                            provinceName = p.provinceName
                        }
                          ).ToList();
            }
            return list;
        }

        public bool AddProvince(List<ProvinceModel> addlist)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in addlist)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var provinceId = db.prcGetPrimaryKey("tblProvince", obj).FirstOrDefault().Value;
                    var model = new tblProvince
                    {
                        provinceId = provinceId,
                        provinceName = item.provinceName
                    };
                    db.tblProvince.Add(model);
                }
                flag = true;
                db.SaveChanges();
            }
            return flag;
        }

        public bool DeleteProvince(List<int> addlist)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in addlist)
                {
                    var model = db.tblProvince.Where(p => p.provinceId == item).FirstOrDefault();
                    if (model != null)
                    {
                        db.tblProvince.Remove(model);
                    }
                }
                db.SaveChanges();
                flag = true;
            }
            return flag;
        }
    }
}