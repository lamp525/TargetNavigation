using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class ImageBLL : IImageBLL
    {
        public List<ImageManageModel> GetImageList()
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var list = (from i in db.tblImage
                            select new ImageManageModel
                                {
                                    extension = i.extension,

                                    imageId = i.imageId,
                                    height = Convert.ToInt32(i.height),
                                    saveName = i.saveName,
                                    width = Convert.ToInt32(i.width)
                                }).ToList();
                return list;
            }
        }

        public bool AddImage(List<ImageManageModel> addlist)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in addlist)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var imageId = db.prcGetPrimaryKey("tblImage", obj).FirstOrDefault().Value;
                    var model = new tblImage
                    {
                        extension = item.extension,
                        height = item.height,
                        imageId = imageId,
                        saveName = item.saveName,
                        width = item.width
                    };
                    db.tblImage.Add(model);
                    db.SaveChanges();
                }
            }
            return flag;
        }
    }
}