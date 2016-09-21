using MB.DAL;
using MB.New.Common;
using System.Linq;

namespace MB.New.BLL
{
    public class SharedBLL
    {
        /// <summary>
        /// 头像上传
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="originalImage">上传后原图名称</param>
        /// <param name="bigImagefilename">上传后大图名称</param>
        /// <param name="midImagefilename">上传后中图名称</param>
        /// <param name="smallImagefilename">上传后小图名称</param>
        public string SaveImg(int userId, string originalImage, string bigImagefilename, string userImagep)
        {
            string NewImage = "";
            using (var db = new TargetNavigationDBEntities())
            {
                var tUser = db.tblUser.Where(u => u.userId == userId).FirstOrDefault();
                tUser.originalImage = originalImage;
                tUser.bigImage = bigImagefilename;
                tUser.imagePosition = userImagep;
                db.SaveChanges();
                NewImage = string.IsNullOrEmpty(db.tblUser.Where(p => p.userId == userId).FirstOrDefault().bigImage) ? "/Images/common/bigUserImg.png" : "/" + ConstVar.HeadImageUpLoadPath + "/" + db.tblUser.Where(p => p.userId == userId).FirstOrDefault().bigImage;
            }
            return NewImage;
        }

        //public UserBaseInfoModel GetImageCut(int userId)
        //{
        //    var model = new UserBaseInfoModel();
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        model = (from u in db.tblUser
        //                 where u.userId == userId
        //                 select new UserBaseInfoModel
        //                 {
        //                     imagePosition = u.imagePosition,
        //                     originalImageUrl = string.IsNullOrEmpty(db.tblUser.Where(p => p.userId == userId).FirstOrDefault().originalImage) ? "/Images/common/bigUserImg.png" : "/" + ConstVar.HeadImageUpLoadPath + "/" + db.tblUser.Where(p => p.userId == userId).FirstOrDefault().originalImage,
        //                     saveName = u.originalImage
        //                 }).FirstOrDefault();
        //    }
        //    if (!string.IsNullOrEmpty(model.imagePosition))
        //    {
        //        model.imagePositions = model.imagePosition.Split(',');
        //    }
        //    return model;
        //}
    }
}