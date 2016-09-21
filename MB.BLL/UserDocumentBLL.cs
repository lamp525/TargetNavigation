using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class UserDocumentBLL : IUserDocumentBLL
    {
        #region 变量区域

        private SharedBLL shareBll = new SharedBLL();

        #endregion 变量区域

        #region 获取公司文档列表（含权限）

        /// <summary>
        /// 获取公司文档列表（含权限）
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <param name="userId">用户Id</param>
        /// <returns>公司文档列表</returns>
        public List<DocumentModel> GetCompanyDocumentList(string condition, Sort sort, int userId)
        {
            var docList = new List<NewDocumentModel>();
            var docListNew = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                #region 未使用存储过程前的代码

                //    //按人员设置权限的文档列表
                //    var personAuthDocuments = (from c in db.tblCompanyDocument
                //                               join f in db.tblFolderAuth on c.documentId equals f.documentId
                //                               join a in db.tblAuthResult on f.authId equals a.authId
                //                               join u in db.tblUser on a.targetId equals u.userId
                //                               where f.type == 3 && a.targetId == userId &&f.power!=1
                //                               select new DocumentModel
                //                               {
                //                                   documentId = c.documentId,
                //                                   power = f.power
                //                               }).ToList();
                //    //按岗位设置权限的文档列表
                //    var stationAuthDocuments = (from c in db.tblCompanyDocument
                //                                join f in db.tblFolderAuth on c.documentId equals f.documentId
                //                                join a in db.tblAuthResult on f.authId equals a.authId
                //                                join u in db.tblUserStation on a.targetId equals u.stationId
                //                                where f.type == 2 && u.userId == userId && f.power != 1
                //                                select new DocumentModel
                //                                {
                //                                    documentId = c.documentId,
                //                                    power = f.power
                //                                }).ToList();
                //    //按组织架构设置的文档列表
                //    var orgAuthDocuments = (from c in db.tblCompanyDocument
                //                            join f in db.tblFolderAuth on c.documentId equals f.documentId
                //                            join a in db.tblAuthResult on f.authId equals a.authId
                //                            join s in db.tblStation on a.targetId equals s.organizationId
                //                            join u in db.tblUserStation on s.stationId equals u.stationId
                //                            where f.type == 1 && u.userId == userId && f.power != 1
                //                            select new DocumentModel
                //                            {
                //                                documentId = c.documentId,
                //                                power = f.power
                //                            }).ToList();
                //    //筛选出来的文档列表
                //    var docListOld = (from c in db.tblCompanyDocument
                //                      join u in db.tblUser on c.createUser equals u.userId into group1
                //                      from u in group1.DefaultIfEmpty()
                //                      join f in db.tblFolderAuth on c.documentId equals f.documentId into group2
                //                      from f in group2.DefaultIfEmpty()
                //                      where !c.deleteFlag && f.power!=1
                //                      select new DocumentModel
                //                      {
                //                          documentId = c.documentId,
                //                          displayName = c.displayName,
                //                          description = c.description,
                //                          isFolder = c.isFolder,
                //                          createUser = c.createUser,
                //                          createUserName = u.userName,
                //                          createTime = c.createTime
                //                      }).Where(condition).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                //    if (docListOld.Count() > 0)
                //    {
                //        //给每一个文档赋上相应的权限
                //        foreach (var item in docListOld)
                //        {
                //            var personDocModel = personAuthDocuments.Where(p => p.documentId == item.documentId).FirstOrDefault();
                //            var stationDocModel = stationAuthDocuments.Where(p => p.documentId == item.documentId).FirstOrDefault();
                //            var orgDocModel = stationAuthDocuments.Where(p => p.documentId == item.documentId).FirstOrDefault();
                //            if (personDocModel != null)
                //            {
                //                docListOld.Remove(item);
                //                item.power = personDocModel.power;
                //                docListNew.Add(item);
                //            }
                //            else if (stationDocModel != null)
                //            {
                //                docListOld.Remove(item);
                //                item.power = stationDocModel.power;
                //                docListNew.Add(item);
                //            }
                //            else if (orgDocModel != null)
                //            {
                //                docListOld.Remove(item);
                //                item.power = orgDocModel.power;
                //                docListNew.Add(item);
                //            }
                //        }
                //        //追加加没有设置权限的数据
                //        docListNew.AddRange(docListOld);
                //    }
                //}

                ////排序
                //docListNew = shareBll.DocListOrderBySort(docListNew, sort);

                #endregion 未使用存储过程前的代码

                //调用存储过程获取公司文档列表
                docList = db.prcGetCompanyListWithAuthy(userId.ToString(), condition, shareBll.GetOrderStringBySort(sort)).ToList();
            }
            if (docList.Count() > 0)
            {
                //转换返回列表类型
                docList.ForEach(p => docListNew.Add(new DocumentModel
                {
                    documentId = p.documentId,
                    displayName = p.displayName,
                    description = p.description,
                    isFolder = p.isFolder,
                    createUser = p.createUser,
                    createUserName = p.createUserName,
                    createTime = p.createTime,
                    power = p.power,
                    saveName = p.saveName,
                    extension = p.extension,
                    folder = p.folder
                }));
            }
            //foreach (var item in docListNew)
            //{
            //    if (item.folder != null)
            //    {
            //        item.UpPower = GetUpDOCPower(item);
            //    }
            //}
            return docListNew;
        }

        //public int GetUpDOCPower(DocumentModel doc)
        //{
        //    var docc = new tblFolderAuth();
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        docc = db.tblFolderAuth.Where(p => p.documentId == doc.folder).FirstOrDefault();
        //        if (docc != null)
        //        {
        //            doc.UpPower = docc.power;
        //        }

        //    }
        //    return doc.UpPower;
        //}

        #endregion 获取公司文档列表（含权限）

        #region 获取个人文档列表(包括我的共享和他人共享)

        /// <summary>
        /// 获取个人文档列表
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="type">2、个人文档 3、我的共享  4、他人共享</param>
        /// <param name="sort">排序</param>
        /// <param name="userId">用户Id</param>
        /// <returns>个人文档列表</returns>
        public List<DocumentModel> GetUserDocumentList(string condition, int type, Sort sort, int userId)
        {
            var docListNew = new List<DocumentModel>();
            var docList = new List<NewDocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                #region 使用存储过程前注释的代码

                //docList = (from c in db.tblUserDocument
                //           join u in db.tblUser on c.createUser equals u.userId into group1
                //           from u in group1.DefaultIfEmpty()
                //           where !c.deleteFlag.Value && c.createUser == userId
                //           select new DocumentModel
                //           {
                //               documentId = c.documentId,
                //               displayName = c.displayName,
                //               description = c.description,
                //               isFolder = c.isFolder,
                //               withShared = c.withShared,
                //               createUser = c.createUser.Value,
                //               createUserName = u.userName,
                //               createTime = c.createTime.Value
                //           }).Where(condition).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                //}

                ////排序
                //docList = shareBll.DocListOrderBySort(docList, sort);

                #endregion 使用存储过程前注释的代码

                //调用存储过程获取用户文档列表
                docList = db.prcGetUserDocumentList(type.ToString(), userId.ToString(), condition, shareBll.GetOrderStringBySort(sort)).ToList();
            }
            if (docList.Count() > 0)
            {
                //转换返回列表类型
                docList.ForEach(p => docListNew.Add(new DocumentModel
                {
                    documentId = p.documentId,
                    displayName = p.displayName,
                    description = p.description,
                    isFolder = p.isFolder,
                    withShared = p.withShared,
                    createUser = p.createUser,
                    createUserName = p.createUserName,
                    createTime = p.createTime,
                    saveName = p.saveName,
                    extension = p.extension
                }));
            }
            return docListNew;
        }

        #endregion 获取个人文档列表(包括我的共享和他人共享)

        #region 获取我的共享文档列表(该方法目前没用)

        /// <summary>
        /// 获取我的共享文档列表
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="sort">排序</param>
        /// <param name="userId">用户Id</param>
        /// <returns>我的共享文档列表</returns>
        public List<DocumentModel> GetMySharedDocumentList(string condition, DateTime start, DateTime end, Sort sort, int userId)
        {
            var docList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                docList = (from c in db.tblUserDocument
                           join u in db.tblUser on c.createUser equals u.userId into group1
                           from u in group1.DefaultIfEmpty()
                           where !c.deleteFlag && c.withShared.Value && c.createUser == userId
                           select new DocumentModel
                           {
                               documentId = c.documentId,
                               displayName = c.displayName,
                               description = c.description,
                               isFolder = c.isFolder,
                               withShared = c.withShared,
                               createUser = c.createUser,
                               createUserName = u.userName,
                               createTime = c.createTime,
                               saveName = c.saveName
                           }).Where(condition).Where("createTime >=@0 And createTime <@1", start, end).ToList();
            }

            //排序
            docList = shareBll.DocListOrderBySort(docList, sort);
            return docList;
        }

        #endregion 获取我的共享文档列表(该方法目前没用)

        #region 获取他人共享文档列表(该方法目前没用)

        /// <summary>
        /// 获取他人共享文档列表
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="sort">排序</param>
        /// <param name="userId">用户Id</param>
        /// <returns>他人共享文档列表</returns>
        public List<DocumentModel> GetOtherSharedDocumentList(string condition, DateTime start, DateTime end, Sort sort, int userId)
        {
            var docList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                docList = (from c in db.tblUserDocument
                           join u in db.tblUser on c.createUser equals u.userId into group1
                           from u in group1.DefaultIfEmpty()
                           join s in db.tblDocumentShared on c.documentId equals s.documentId
                           where !c.deleteFlag && c.withShared.Value && s.userId == userId
                           select new DocumentModel
                           {
                               documentId = c.documentId,
                               displayName = c.displayName,
                               description = c.description,
                               isFolder = c.isFolder,
                               withShared = c.withShared,
                               createUser = c.createUser,
                               createUserName = u.userName,
                               createTime = c.createTime
                           }).Where(condition).Where("createTime >=@0 And createTime <@1", start, end).ToList();
            }

            //排序
            docList = shareBll.DocListOrderBySort(docList, sort);
            return docList;
        }

        #endregion 获取他人共享文档列表(该方法目前没用)

        #region 获取文档统计信息（饼图）

        /// <summary>
        /// 获取文档统计信息（饼图）
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>文档统计列表</returns>
        public List<DocumentStatisticsModel> GetDocumentStatisticsInfo(int userId)
        {
            var statisticList = new List<DocumentStatisticsModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //1、统计公司文档数
                //var companyDocModel = new DocumentStatisticsModel();
                //companyDocModel.id = 1;
                //companyDocModel.count = db.tblCompanyDocument.Count(p => !p.deleteFlag);
                //statisticList.Add(companyDocModel);
                //2、统计我的文档数
                var userDocModel = new DocumentStatisticsModel();
                userDocModel.id = 2;
                userDocModel.count = db.tblUserDocument.Count(p => p.createUser == userId && p.isFolder == false && p.deleteFlag == false);
                statisticList.Add(userDocModel);
                //3、统计我的共享数
                var myShareModel = new DocumentStatisticsModel();
                myShareModel.id = 3;
                myShareModel.count = db.tblUserDocument.Count(p => p.createUser == userId && !p.deleteFlag && p.withShared.Value);
                statisticList.Add(myShareModel);
                //4、统计他人共享数
                var otherShareModel = new DocumentStatisticsModel();
                otherShareModel.id = 4;
                otherShareModel.count = (from u in db.tblUserDocument
                                         join ds in db.tblDocumentShared
                                             on u.documentId equals ds.documentId
                                         where !u.deleteFlag && u.withShared.Value && ds.userId == userId && u.isFolder == false
                                         select u).Count();
                statisticList.Add(otherShareModel);
            }
            return statisticList;
        }

        #endregion 获取文档统计信息（饼图）

        #region 新建用户文档(数据库操作，不包含上传动作)

        /// <summary>
        /// 新建用户文档（数据库操作，不包含上传动作）
        /// </summary>
        /// <param name="ud">UserDocument</param>
        /// <returns>true/false</returns>
        public bool InsertUserDocument(UserDocument ud)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var userDocument = new tblUserDocument
                {
                    documentId = ud.documentId,/*文档ID*/
                    folder = ud.folder,/*上级文件夹*/
                    displayName = ud.displayName,/*表示名*/
                    description = ud.description,/*描述*/
                    saveName = ud.saveName,/*存储名*/
                    extension = ud.extension,/*后缀名*/
                    //archive = ud.archive,/*归档标志*/
                    //archiveTime = ud.archiveTime,/*归档时间*/
                    withShared = ud.withShared,/*共享标志*/
                    isFolder = ud.isFolder,/*类型*/
                    createUser = ud.createUser,/*创建用户*/
                    createTime = ud.createTime,/*创建时间*/
                    updateUser = ud.updateUser,/*修改用户*/
                    updateTime = ud.updateTime,/*修改时间*/
                    deleteFlag = ud.deleteFlag/*删除标志*/
                };
                db.tblUserDocument.Add(userDocument);
                if (db.SaveChanges() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion 新建用户文档(数据库操作，不包含上传动作)

        #region 删除服务器上刚上传的文档

        /// <summary>
        /// 删除服务器上刚上传的文档
        /// </summary>
        /// <param name="saveName">存储名称</param>
        public void DeleteFile(string saveName, int type)
        {
            //服务器上的存储地址
            var DocumentPath = type == 1 ? FilePath.DocumentUpLoadPath : FilePath.MineUpLoadPath;
            var realPath = Path.Combine(DocumentPath, saveName);
            if (File.Exists(realPath))
            {
                File.Delete(realPath);
            }
            //using (var db = new TargetNavigationDBEntities())
            //{
            //    FileUpload file = new FileUpload();
            //    file.DeletePlanAttachmentById(id, UploadFilePath.Plan, db);
            //    db.SaveChanges();
            //}
        }

        #endregion 删除服务器上刚上传的文档
    }
}