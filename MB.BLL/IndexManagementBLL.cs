using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class IndexManagementBLL : IIndexManagementBLL
    {
        #region 获取首页模块列表

        /// <summary>
        /// 获取首页模块列表
        /// </summary>
        /// <returns>模块列表</returns>
        public List<IndexModuleInfoModel> GetModuleList()
        {
            var moduleInfoModelList = new List<IndexModuleInfoModel>();
            var moduleList = new List<tblIndexModule>();

            using (var db = new TargetNavigationDBEntities())
            {
                moduleList = db.tblIndexModule.OrderBy(p => p.position).ToList();
            }

            if (moduleList.Count() == 0)
            {
                return null;
            }

            foreach (var module in moduleList)
            {
                var moduleModel = new IndexModuleModel();

                moduleModel.moduleId = module.moduleId;
                moduleModel.title = module.title;
                moduleModel.displayTitle = module.displayTitle;
                moduleModel.linkTarget = module.linkTarget;
                moduleModel.maxRow = module.maxRow;
                moduleModel.width = module.width;
                moduleModel.height = module.height;
                moduleModel.type = module.type.Value;
                moduleModel.position = module.position;
                moduleModel.defaultEfficiency = module.defaultEfficiency;
                moduleModel.topDisplay = module.topDisplay;
                moduleModel.topDisplayLine = module.topDisplayLine;
                moduleModel.defaultLine = module.defaultLine;
                moduleModel.efficiencyValue = module.efficiencyValue;
                moduleModel.executiveForce = module.executiveForce;
                moduleModel.objective = module.objective;

                var targetList = new List<IndexTargetModel>();
                var imageList = new List<ImageModel>();
                var moduleId = moduleModel.moduleId.Value;

                switch (moduleModel.type)
                {
                    // 新闻模块的场合，取得新闻来源
                    case (int)ConstVar.ModuleType.New:
                        targetList = this.GetIndexNews(moduleId);
                        imageList = null;
                        break;
                    // 通知模块的场合，取得通知来源
                    case (int)ConstVar.ModuleType.Notice:
                        targetList = this.GetIndexNotice(moduleId);
                        imageList = null;
                        break;

                    // 文档模块的场合，取得文档来源
                    case (int)ConstVar.ModuleType.Document:
                        targetList = this.GetIndexDocument(moduleId);
                        imageList = null;
                        break;

                    // 图片轮播模块的场合，取得首页图片
                    case (int)ConstVar.ModuleType.Image:
                        targetList = null;
                        imageList = this.GetIndexImage(moduleId);
                        break;

                    // 功效价值模块的场合，取得统计对象
                    case (int)ConstVar.ModuleType.Performance:
                        targetList = this.GetIndexStatistics(moduleId);
                        imageList = null;
                        break;
                }

                // 添加到模块信息列表中
                moduleInfoModelList.Add(new IndexModuleInfoModel
                {
                    module = moduleModel,
                    target = targetList,
                    image = imageList
                });
            }

            return moduleInfoModelList;
        }

        #endregion 获取首页模块列表

        #region 模块详情取得

        /// <summary>
        /// 模块详情取得
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>模块详情</returns>
        public IndexModuleInfoModel GetModuleInfo(int moduleId)
        {
            var moduleInfo = new IndexModuleInfoModel();

            using (var db = new TargetNavigationDBEntities())
            {
                moduleInfo.module = (from module in db.tblIndexModule
                                     where module.moduleId == moduleId
                                     select new IndexModuleModel
                                     {
                                         moduleId = module.moduleId,
                                         title = module.title,
                                         displayTitle = module.displayTitle,
                                         linkTarget = module.linkTarget,
                                         maxRow = module.maxRow,
                                         width = module.width,
                                         height = module.height,
                                         type = module.type.Value,
                                         position = module.position,
                                         defaultEfficiency = module.defaultEfficiency,
                                         topDisplay = module.topDisplay,
                                         topDisplayLine = module.topDisplayLine,
                                         defaultLine = module.defaultLine,
                                         efficiencyValue = module.efficiencyValue,
                                         executiveForce = module.executiveForce,
                                         objective = module.objective
                                     }).FirstOrDefault();

                // 图片轮播模块的场合，取得首页图像
                if (moduleInfo.module != null && moduleInfo.module.type == (int)ConstVar.ModuleType.Image)
                {
                    moduleInfo.image = this.GetIndexImage(moduleId);
                }
            }

            return moduleInfo;
        }

        #endregion 模块详情取得

        #region 新闻来源取得

        /// <summary>
        /// 新闻来源取得
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>新闻来源</returns>
        public List<IndexTargetModel> GetIndexNews(int moduleId)
        {
            var newsList = new List<IndexTargetModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                newsList = (from index in db.tblIndexNews
                            join news in db.tblNewsDirectory
                                on index.directoryId equals news.directoryId
                            where index.moduleId == moduleId
                            select new IndexTargetModel
                            {
                                moduleId = index.moduleId,
                                targetId = index.directoryId,
                                targetName = news.directoryName,
                                withSub = index.withSub
                            }).ToList();
            }

            return newsList;
        }

        #endregion 新闻来源取得

        #region 通知来源取得

        /// <summary>
        /// 通知来源取得
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>通知来源</returns>
        public List<IndexTargetModel> GetIndexNotice(int moduleId)
        {
            var noticeList = new List<IndexTargetModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                noticeList = (from index in db.tblIndexNotice
                              join notice in db.tblNoticeDirectory
                                  on index.directoryId equals notice.directoryId
                              where index.moduleId == moduleId
                              select new IndexTargetModel
                              {
                                  moduleId = index.moduleId,
                                  targetId = index.directoryId,
                                  targetName = notice.directoryName,
                                  withSub = index.withSub
                              }).ToList();
            }

            return noticeList;
        }

        #endregion 通知来源取得

        #region 文档来源取得

        /// <summary>
        /// 文档来源取得
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>文档来源</returns>
        public List<IndexTargetModel> GetIndexDocument(int moduleId)
        {
            var docList = new List<IndexTargetModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                docList = (from index in db.tblIndexDocument
                           join doc in db.tblCompanyDocument
                               on index.documentId equals doc.documentId
                           where index.moduleId == moduleId && !doc.deleteFlag
                           select new IndexTargetModel
                           {
                               moduleId = index.moduleId,
                               targetId = index.documentId,
                               targetName = doc.displayName,
                               withSub = index.withSub
                           }).ToList();
            }

            return docList;
        }

        #endregion 文档来源取得

        #region 首页图像取得

        /// <summary>
        /// 首页图像取得
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>首页图像</returns>
        public List<ImageModel> GetIndexImage(int moduleId)
        {
            var imgList = new List<ImageModel>();

            var imagePath = ConfigurationManager.AppSettings["NewsUpLoadPath"].ToString();

            using (var db = new TargetNavigationDBEntities())
            {
                imgList = (from index in db.tblIndexImage
                           join img in db.tblImage
                               on index.imageId equals img.imageId
                           where index.moduleId == moduleId
                           select new ImageModel
                           {
                               moduleId = index.moduleId,
                               imageId = index.imageId,
                               imageUrl = "\\" + imagePath + "\\" + img.saveName + "." + img.extension,
                               width = img.width.Value,
                               height = img.height.Value
                           }).ToList();
            }

            return imgList;
        }

        #endregion 首页图像取得

        #region 统计对象取得

        /// <summary>
        /// 统计对象取得
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns>统计对象</returns>
        public List<IndexTargetModel> GetIndexStatistics(int moduleId)
        {
            var orgList = new List<IndexTargetModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                orgList = (from index in db.tblIndexStatistics
                           join org in db.tblOrganization
                               on index.organizationId equals org.organizationId
                           where index.moduleId == moduleId && !org.deleteFlag
                           select new IndexTargetModel
                           {
                               moduleId = index.moduleId,
                               targetId = index.organizationId,
                               targetName = org.organizationName,
                               withSub = index.withSub
                           }).ToList();
            }

            return orgList;
        }

        #endregion 统计对象取得

        #region 添加模块

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="moduleInfo">模块信息</param>
        /// <param name="userId">用户ID</param>
        public void AddIndexModuleInfo(IndexModuleInfoModel moduleInfo, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                // 添加模块信息
                var moduleId = this.AddIndexModule(moduleInfo.module, userId, db);

                switch (moduleInfo.module.type)
                {
                    // 新闻模块的场合，添加新闻来源
                    case (int)ConstVar.ModuleType.New:
                        this.AddIndexNews(moduleInfo.target, moduleId, db);
                        break;
                    // 通知模块的场合，添加通知来源
                    case (int)ConstVar.ModuleType.Notice:
                        this.AddIndexNotice(moduleInfo.target, moduleId, db);
                        break;

                    // 文档模块的场合，添加文档来源
                    case (int)ConstVar.ModuleType.Document:
                        this.AddIndexDocument(moduleInfo.target, moduleId, db);
                        break;

                    // 图片轮播模块的场合，添加首页图片
                    case (int)ConstVar.ModuleType.Image:
                        this.AddIndexImage(moduleInfo.target, moduleId, db);
                        break;

                    // 功效价值模块的场合，添加统计对象
                    case (int)ConstVar.ModuleType.Performance:
                        this.AddIndexStatistics(moduleInfo.target, moduleId, db);
                        break;
                }

                db.SaveChanges();
            }
        }

        #endregion 添加模块

        #region 更新模块

        /// <summary>
        /// 更新模块
        /// </summary>
        /// <param name="moduleInfo">模块信息</param>
        /// <param name="userId">用户ID</param>
        public void UpdateIndexModuleInfo(IndexModuleInfoModel moduleInfo, int userId)
        {
            var moduleId = moduleInfo.module.moduleId.Value;

            using (var db = new TargetNavigationDBEntities())
            {
                // 更新模块信息
                this.UpdateIndexModule(moduleInfo.module, userId, db);

                switch (moduleInfo.module.type)
                {
                    // 新闻模块的场合
                    case (int)ConstVar.ModuleType.New:
                        // 删除新闻来源
                        this.DeleteIndexNews(moduleId, db);
                        // 添加新闻来源
                        this.AddIndexNews(moduleInfo.target, moduleId, db);
                        break;
                    // 通知模块的场合
                    case (int)ConstVar.ModuleType.Notice:
                        // 删除通知来源
                        this.DeleteIndexNotice(moduleId, db);
                        // 添加通知来源
                        this.AddIndexNotice(moduleInfo.target, moduleId, db);
                        break;

                    // 文档模块的场合
                    case (int)ConstVar.ModuleType.Document:
                        // 删除文档来源
                        this.DeleteIndexDocument(moduleId, db);
                        // 添加文档来源
                        this.AddIndexDocument(moduleInfo.target, moduleId, db);
                        break;

                    // 图片轮播模块的场合
                    case (int)ConstVar.ModuleType.Image:
                        // 删除首页图片
                        this.DeleteIndexImage(moduleId, db);
                        // 添加首页图片
                        this.AddIndexImage(moduleInfo.target, moduleId, db);
                        break;

                    // 功效价值模块的场合
                    case (int)ConstVar.ModuleType.Performance:
                        // 删除统计对象
                        this.DeleteIndexStatistics(moduleId, db);
                        // 添加统计对象
                        this.AddIndexStatistics(moduleInfo.target, moduleId, db);
                        break;
                }

                db.SaveChanges();
            }
        }

        #endregion 更新模块

        #region 删除模块

        /// <summary>
        /// 删除首页模块
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        public void DeleteModule(int moduleId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var module = db.tblIndexModule.Where(p => p.moduleId == moduleId).FirstOrDefault();

                if (module == null)
                {
                    return;
                }

                switch (module.type)
                {
                    // 新闻模块的场合
                    case (int)ConstVar.ModuleType.New:
                        // 删除新闻来源
                        this.DeleteIndexNews(moduleId, db);
                        break;
                    // 通知模块的场合
                    case (int)ConstVar.ModuleType.Notice:
                        // 删除通知来源
                        this.DeleteIndexNotice(moduleId, db);
                        break;

                    // 文档模块的场合
                    case (int)ConstVar.ModuleType.Document:
                        // 删除文档来源
                        this.DeleteIndexDocument(moduleId, db);
                        break;

                    // 图片轮播模块的场合
                    case (int)ConstVar.ModuleType.Image:
                        // 删除首页图片
                        this.DeleteIndexImage(moduleId, db);
                        break;

                    // 功效价值模块的场合
                    case (int)ConstVar.ModuleType.Performance:
                        // 删除统计对象
                        this.DeleteIndexStatistics(moduleId, db);
                        break;
                }

                // 删除模块信息
                this.DeleteIndexModule(moduleId, db);

                db.SaveChanges();
            }
        }

        #endregion 删除模块

        #region 更新模块大小

        /// <summary>
        /// 更新木块大小
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="width">宽度</param>
        /// <param name="userId">用户ID</param>
        public void UpdateModuleSize(IndexModuleModel moduleInfo, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var module = db.tblIndexModule.Where(p => p.moduleId == moduleInfo.moduleId).FirstOrDefault();

                if (module == null)
                {
                    return;
                }

                module.position = moduleInfo.position;
                module.width = moduleInfo.width;
                module.updateUser = userId;
                module.updateTime = DateTime.Now;

                db.SaveChanges();
            }
        }

        #endregion 更新模块大小

        #region 删除首页图片

        /// <summary>
        /// 删除首页图片
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="imageId">图片ID</param>
        public void DeleteIndexImage(int moduleId, int imageId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var module = db.tblIndexImage.Where(p => p.moduleId == moduleId && p.imageId == imageId).FirstOrDefault();

                if (module != null)
                {
                    db.tblIndexImage.Remove(module);
                }

                db.SaveChanges();
            }
        }

        #endregion 删除首页图片

        #region 私有方法

        #region 添加模块

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="module">模块信息</param>
        /// <param name="userId">用户ID</param>
        /// <param name="db">DB</param>
        /// <returns>模块ID</returns>
        private int AddIndexModule(IndexModuleModel module, int userId, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            int moduleId = db.prcGetPrimaryKey("tblIndexModule", obj).FirstOrDefault().Value;

            var moduleModel = new tblIndexModule
            {
                moduleId = moduleId,
                title = module.title,
                displayTitle = module.displayTitle,
                linkTarget = module.linkTarget,
                maxRow = module.maxRow,
                width = module.width,
                height = module.height,
                type = module.type,
                position = module.position,
                defaultEfficiency = module.defaultEfficiency,
                topDisplay = module.topDisplay,
                topDisplayLine = module.topDisplayLine,
                defaultLine = module.defaultLine,
                efficiencyValue = module.efficiencyValue,
                executiveForce = module.executiveForce,
                objective = module.objective,
                createUser = userId,
                createTime = DateTime.Now,
                updateUser = userId,
                updateTime = DateTime.Now
            };

            db.tblIndexModule.Add(moduleModel);
            db.SaveChanges();

            return moduleId;
        }

        #endregion 添加模块

        #region 更新模块

        /// <summary>
        /// 更新模块
        /// </summary>
        /// <param name="module">模块信息</param>
        /// <param name="userId">用户ID</param>
        /// <param name="db">DB</param>
        private void UpdateIndexModule(IndexModuleModel module, int userId, TargetNavigationDBEntities db)
        {
            var moduleModel = db.tblIndexModule.Where(p => p.moduleId == module.moduleId).FirstOrDefault();

            if (moduleModel == null)
            {
                return;
            }

            moduleModel.title = module.title;
            moduleModel.displayTitle = module.displayTitle;
            moduleModel.linkTarget = module.linkTarget;
            moduleModel.maxRow = module.maxRow;
            moduleModel.width = module.width;
            moduleModel.height = module.height;
            moduleModel.type = module.type;
            moduleModel.position = module.position;
            moduleModel.defaultEfficiency = module.defaultEfficiency;
            moduleModel.topDisplay = module.topDisplay;
            moduleModel.topDisplayLine = module.topDisplayLine;
            moduleModel.defaultLine = module.defaultLine;
            moduleModel.efficiencyValue = module.efficiencyValue;
            moduleModel.executiveForce = module.executiveForce;
            moduleModel.objective = module.objective;
            moduleModel.updateUser = userId;
            moduleModel.updateTime = DateTime.Now;
        }

        #endregion 更新模块

        #region 删除模块

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void DeleteIndexModule(int moduleId, TargetNavigationDBEntities db)
        {
            var moduleModel = db.tblIndexModule.Where(p => p.moduleId == moduleId).FirstOrDefault();

            if (moduleModel != null)
            {
                db.tblIndexModule.Remove(moduleModel);
            }
        }

        #endregion 删除模块

        #region 添加新闻来源

        /// <summary>
        /// 添加新闻来源
        /// </summary>
        /// <param name="targetList">新闻来源</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void AddIndexNews(List<IndexTargetModel> targetList, int moduleId, TargetNavigationDBEntities db)
        {
            var newsList = new List<tblIndexNews>();

            targetList.ForEach(p => newsList.Add(new tblIndexNews
            {
                moduleId = moduleId,
                directoryId = p.targetId,
                withSub = p.withSub
            }));

            db.tblIndexNews.AddRange(newsList);
            db.SaveChanges();
        }

        #endregion 添加新闻来源

        #region 删除新闻来源

        /// <summary>
        /// 删除新闻来源
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void DeleteIndexNews(int moduleId, TargetNavigationDBEntities db)
        {
            var newsList = db.tblIndexNews.Where(p => p.moduleId == moduleId).ToList();

            if (newsList.Count() > 0)
            {
                db.tblIndexNews.RemoveRange(newsList);
            }
        }

        #endregion 删除新闻来源

        #region 添加通知来源

        /// <summary>
        /// 添加通知来源
        /// </summary>
        /// <param name="targetList">通知来源</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void AddIndexNotice(List<IndexTargetModel> targetList, int moduleId, TargetNavigationDBEntities db)
        {
            var noticeList = new List<tblIndexNotice>();

            targetList.ForEach(p => noticeList.Add(new tblIndexNotice
            {
                moduleId = moduleId,
                directoryId = p.targetId,
                withSub = p.withSub
            }));

            db.tblIndexNotice.AddRange(noticeList);
            db.SaveChanges();
        }

        #endregion 添加通知来源

        #region 删除通知来源

        /// <summary>
        /// 删除通知来源
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void DeleteIndexNotice(int moduleId, TargetNavigationDBEntities db)
        {
            var noticeList = db.tblIndexNotice.Where(p => p.moduleId == moduleId).ToList();

            if (noticeList.Count() > 0)
            {
                db.tblIndexNotice.RemoveRange(noticeList);
            }
        }

        #endregion 删除通知来源

        #region 添加文档来源

        /// <summary>
        /// 添加文档来源
        /// </summary>
        /// <param name="targetList">文档来源</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void AddIndexDocument(List<IndexTargetModel> targetList, int moduleId, TargetNavigationDBEntities db)
        {
            var docList = new List<tblIndexDocument>();

            targetList.ForEach(p => docList.Add(new tblIndexDocument
            {
                moduleId = moduleId,
                documentId = p.targetId,
                withSub = p.withSub
            }));

            db.tblIndexDocument.AddRange(docList);
        }

        #endregion 添加文档来源

        #region 删除文档来源

        /// <summary>
        /// 删除文档来源
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void DeleteIndexDocument(int moduleId, TargetNavigationDBEntities db)
        {
            var docList = db.tblIndexDocument.Where(p => p.moduleId == moduleId).ToList();

            if (docList.Count() > 0)
            {
                db.tblIndexDocument.RemoveRange(docList);
            }
        }

        #endregion 删除文档来源

        #region 添加首页图片

        /// <summary>
        /// 添加首页图片
        /// </summary>
        /// <param name="targetList">图片</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void AddIndexImage(List<IndexTargetModel> targetList, int moduleId, TargetNavigationDBEntities db)
        {
            var imgList = new List<tblIndexImage>();

            targetList.ForEach(p => imgList.Add(new tblIndexImage
            {
                moduleId = moduleId,
                imageId = p.targetId
            }));

            db.tblIndexImage.AddRange(imgList);
        }

        #endregion 添加首页图片

        #region 删除首页图片

        /// <summary>
        /// 删除首页图片
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void DeleteIndexImage(int moduleId, TargetNavigationDBEntities db)
        {
            var imgList = db.tblIndexImage.Where(p => p.moduleId == moduleId).ToList();

            if (imgList.Count() > 0)
            {
                db.tblIndexImage.RemoveRange(imgList);
            }
        }

        #endregion 删除首页图片

        #region 添加统计对象

        /// <summary>
        /// 添加统计对象
        /// </summary>
        /// <param name="targetList">统计对象</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void AddIndexStatistics(List<IndexTargetModel> targetList, int moduleId, TargetNavigationDBEntities db)
        {
            var orgList = new List<tblIndexStatistics>();

            targetList.ForEach(p => orgList.Add(new tblIndexStatistics
            {
                moduleId = moduleId,
                organizationId = p.targetId,
                withSub = p.withSub
            }));

            db.tblIndexStatistics.AddRange(orgList);
        }

        #endregion 添加统计对象

        #region 删除统计对象

        /// <summary>
        /// 删除统计对象
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="db">DB</param>
        private void DeleteIndexStatistics(int moduleId, TargetNavigationDBEntities db)
        {
            var orgList = db.tblIndexStatistics.Where(p => p.moduleId == moduleId).ToList();

            if (orgList.Count() > 0)
            {
                db.tblIndexStatistics.RemoveRange(orgList);
            }
        }

        #endregion 删除统计对象

        #endregion 私有方法
    }
}