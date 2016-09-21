using MB.DAL;
using MB.New.BLL.Organization;
using MB.New.BLL.Station;
using MB.New.BLL.User;
using MB.New.Common;
using MB.New.Model;
using System.Collections.Generic;
using System.Linq;

namespace MB.Facade.User
{
    public class UserFacade : IUserFacade
    {
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="passwordInfo"></param>
        /// <returns></returns>
        public EnumDefine.ChangePasswordResult ChangePassword(PageUserPasswordModel passwordInfo)
        {
            IUserBLL User = new UserBLL();

            var result = new EnumDefine.ChangePasswordResult();
            if (passwordInfo == null)
            {
                result = EnumDefine.ChangePasswordResult.Failed;
            }
            using (var db = new TargetNavigationDBEntities())
            {
                result = User.ChangePassword(db, passwordInfo.userId.Value, passwordInfo.oldPassword, passwordInfo.newPassword);
                db.SaveChanges();
            }

            return result;
        }

        /// <summary>
        /// 获取头部个人用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageUserInfoSimpleModel GetHeadUserInfo(int userId)
        {
            IUserBLL User = new UserBLL();

            var result = new PageUserInfoSimpleModel();
            using (var db = new TargetNavigationDBEntities())
            {
                var info = User.GetUserDefaultInfo(db, userId);

                if (info != null)
                    result = ModelMapping.JsonMapping<UserInfoSimpleModel, PageUserInfoSimpleModel>(info);
            }

            return result;
        }

        /// <summary>
        /// 取得常用联系人信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PageUserInfoSimpleModel> GetTopContacts(int userId, EnumDefine.TopContactsType type)
        {
            IUserBLL User = new UserBLL();

            var result = new List<PageUserInfoSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var infoList = new List<UserBaseInfoModel>();

                //取得登录用户信息
                var myInfo = User.GetUserInfoById(db, userId);
                if (myInfo != null) infoList.Add(myInfo);

                switch (type)
                {
                    //上级和下属的场合
                    case EnumDefine.TopContactsType.Both:
                        //直属上级信息
                        infoList.AddRange(User.GetSuperiorInfoByUserId(db, userId));
                        //所有下属信息
                        infoList.AddRange(User.GetAllSubordinateInfoByUserId(db, userId));
                        break;

                    //下属的场合
                    case EnumDefine.TopContactsType.Subordinate:
                        //所有下属信息
                        infoList.AddRange(User.GetAllSubordinateInfoByUserId(db, userId));
                        break;

                    //上级的场合
                    case EnumDefine.TopContactsType.Superior:
                        //直属上级信息
                        infoList.AddRange(User.GetSuperiorInfoByUserId(db, userId));

                        break;
                }

                //去除重复信息
                var temp = new List<UserBaseInfoModel>();
                infoList.ForEach(x =>
                       {
                           if (!temp.Exists(p => p.userId == x.userId)) temp.Add(x);
                       });

                //按人名排序
                infoList = temp.OrderBy(x => x.userName).ToList();

                result = ModelMapping.JsonMapping<List<UserBaseInfoModel>, List<PageUserInfoSimpleModel>>(infoList);
            }

            return result;
        }

        /// <summary>
        /// 取得用户部门信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PageUserOrgInfoModel> GetUserOrgInfo(int userId)
        {
            IUserBLL User = new UserBLL();
            IStationBLL Station = new StationBLL();
            IOrganizationBLL Organization = new OrganizationBLL();

            var result = new List<PageUserOrgInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var stationList = Station.GetStationInfoByUserId(db, userId);

                if (stationList == null || stationList.Count == 0) return result;

                //拼接层级数
                var level = ConstVar.PlanOrgSpliceNum;
                foreach (var stationItem in stationList)
                {
                    var orgList = Organization.GetParentOrgByStationId(db, stationItem.stationId.Value, level).OrderByDescending(x => x.level).ToList();

                    if (orgList == null || orgList.Count == 0) continue;

                    var model = new PageUserOrgInfoModel
                    {
                        orgId = orgList.Last().organizationId.Value,
                        orgInfo = StringUtility.ListToString(orgList.Select(x => x.organizationName).ToList(), "-")
                    };

                    //处理重复数据
                    var isExist = result.Exists(x => x.orgId == model.orgId);
                    if (!isExist) result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// 取得用户原头像信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>用户原头像地址</returns>
        public PageUserHeadImageModel GetUserOriginalImage(int userId)
        {
            IUserBLL User = new UserBLL();

            var result = new PageUserHeadImageModel();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = User.GetUserInfoById(db, userId);
                if (info == null) return result;

                result.originalImage = info.originalImage;
                result.imagePosition = info.imagePosition;
                if (!string.IsNullOrEmpty(info.imagePosition))
                {
                    var positionInfo = info.imagePosition.Split(',');
                    if (positionInfo.Length == 4)
                    {
                        result.startPointX = int.Parse(positionInfo[0]);
                        result.startPointY = int.Parse(positionInfo[1]);
                        result.cutHeight = int.Parse(positionInfo[2]);
                        result.cutWidth = int.Parse(positionInfo[3]);
                    }
                }
                result.imageUrl = "http://" + ConstVar.WebHostURL + "/" + ConstVar.HeadImageUpLoadPath + "/" + info.originalImage;
            }
            return result;
        }

        /// <summary>
        /// 取得用户岗位信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PageUserInfoSimpleModel> GetUserOrgStationInfo(int userId)
        {
            IUserBLL User = new UserBLL();

            var result = new List<PageUserInfoSimpleModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var info = User.GetUserOrgStationInfoByUser(db, userId);

                result = ModelMapping.JsonMapping<List<UserInfoSimpleModel>, List<PageUserInfoSimpleModel>>(info);
            }
            return result;
        }

        /// <summary>
        /// 用户头像修改
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imageInfo"></param>
        /// <returns>头像地址</returns>
        public string SaveUserHead(int userId, PageUserHeadImageModel imageInfo)
        {
            IUserBLL User = new UserBLL();

            // 截取头像并取得保存后名字
            var saveFileName = ImageHelper.GetCutHeadImage(userId, imageInfo.extension, imageInfo.isUploaded, imageInfo.startPointX, imageInfo.startPointY, imageInfo.cutWidth, imageInfo.cutHeight);

            // 组合截图位置
            imageInfo.imagePosition = StringUtility.ListToString(new List<string>() { imageInfo.startPointX.ToString(), imageInfo.startPointY.ToString(), imageInfo.cutHeight.ToString(), imageInfo.cutWidth.ToString() }, ",");

            UserBaseInfoModel userInfo = new UserBaseInfoModel();
            userInfo.imagePosition = imageInfo.imagePosition;
            userInfo.originalImage = ImageHelper.GetUserImageName(userId, imageInfo.extension);
            userInfo.headImage = saveFileName;
            userInfo.userId = userId;

            using (var db = new TargetNavigationDBEntities())
            {
                User.UpdUserHeadImageInfo(db, userInfo);
                db.SaveChanges();
            }

            // 头像原图已经上传
            if (imageInfo.isUploaded)
            {
                //删除旧头像原图
                FileUtility.Delete(EnumDefine.FileFolderType.HeadImage, ImageHelper.GetUserImageName(userId, imageInfo.extension));

                //头像原图文件地址
                var originalImageFilePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.HeadImage, ImageHelper.GetUserImageName(userId, imageInfo.extension));

                //头像原图临时文件地址
                var originalImageTempFilePath = FileUtility.GetFilePath(EnumDefine.FileFolderType.HeadImage, ImageHelper.GetUserImageName(userId, imageInfo.extension, true));

                //将头像临时文件保存为头像新原图
                System.IO.File.Move(originalImageTempFilePath, originalImageFilePath);
            }

            // 删除头像临时文件
            FileUtility.Delete(EnumDefine.FileFolderType.HeadImage, ImageHelper.GetUserImageName(userId, imageInfo.extension, true));

            // 返回截取后头像地址
            return "http://" + ConstVar.WebHostURL + "/" + ConstVar.HeadImageUpLoadPath + "/" + saveFileName;
        }

        /// <summary>
        /// 人员模糊检索
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<PageUserInfoSimpleModel> UserNameFuzzySearch(string userName)
        {
            IUserBLL User = new UserBLL();

            var result = new List<PageUserInfoSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var info = User.UserNameFuzzySearch(db, userName);
                result = ModelMapping.JsonMapping<List<UserBaseInfoModel>, List<PageUserInfoSimpleModel>>(info);
            }

            return result;
        }

        /// <summary>
        /// 更新用户默认岗位信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="stationId"></param>
        public void UpdUserDefaultStation(int userId, int stationId)
        {
            IUserBLL User = new UserBLL();

            using (var db = new TargetNavigationDBEntities())
            {
                var userInfo = new UserBaseInfoModel
                {
                    userId = userId,
                    defaultStationId = stationId
                };
                User.UpdUserDefaultStation(db, userInfo);
                db.SaveChanges();
            }
        }
    }
}