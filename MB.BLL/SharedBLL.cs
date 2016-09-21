using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class SharedBLL : ISharedBLL
    {
        #region 新建文件夹（公司文档）

        /// <summary>
        /// 新建文件夹（公司文档）
        /// </summary>
        /// <param name="beforeFolder">上级文件夹Id</param>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="description">描述</param>
        /// <param name="userId">用户Id</param>
        public void BuildNewFolder(int? beforeFolder, string folderName, string description, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;
                var companyModel = new tblCompanyDocument
                {
                    documentId = documentId,
                    folder = beforeFolder,
                    displayName = folderName,
                    description = description,
                    archive = false,
                    isFolder = true,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblCompanyDocument.Add(companyModel);
                //复制父文件夹的权限
                if (beforeFolder != null)
                {
                    CopyFileAuth(db, documentId, beforeFolder.Value);
                }
                //添加新建日志
                AddCompanyDocumentLog(db, documentId, 4, description, userId);
                db.SaveChanges();
            }
        }

        #endregion 新建文件夹（公司文档）

        #region 复制文件夹的权限

        /// <summary>
        /// 复制文件夹的权限
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="documentId">文档Id</param>
        /// <param name="parent">要复制权限的文档Id</param>
        private void CopyFileAuth(TargetNavigationDBEntities db, int documentId, int? parent)
        {
            //公司文档权限表
            var folderAuthList = db.tblFolderAuth.Where(p => p.documentId == parent);
            if (folderAuthList.Count() > 0)
            {
                foreach (var item in folderAuthList)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter authobj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var authId = db.prcGetPrimaryKey("tblCompanyDocument", authobj).FirstOrDefault().Value;
                    var folderModel = new tblFolderAuth
                    {
                        authId = authId,
                        documentId = documentId,
                        type = item.type,
                        power = item.power
                    };
                    db.tblFolderAuth.Add(folderModel);
                    //文档权限结果表
                    var resultList = db.tblAuthResult.Where(p => p.authId == item.authId);
                    foreach (var result in resultList)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter resultobj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        var resultId = db.prcGetPrimaryKey("tblCompanyDocument", resultobj).FirstOrDefault().Value;
                        var resultModel = new tblAuthResult
                        {
                            resultId = resultId,
                            authId = authId,
                            targetId = result.targetId
                        };
                        db.tblAuthResult.Add(resultModel);
                    }
                }
            }
        }

        #endregion 复制文件夹的权限

        #region 新建文件夹（个人文档）

        /// <summary>
        /// 新建文件夹（个人文档）
        /// </summary>
        /// <param name="beforeFolder">上级文件夹Id</param>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="description">描述</param>
        /// <param name="userId">用户Id</param>
        public void BuildNewUserFolder(int? beforeFolder, string folderName, string description, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var documentId = db.prcGetPrimaryKey("tblUserDocument", obj).FirstOrDefault().Value;
                var userDocModel = new tblUserDocument
                {
                    documentId = documentId,
                    folder = beforeFolder,
                    displayName = folderName,
                    description = description,
                    //archive = false,
                    isFolder = true,
                    withShared = false,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblUserDocument.Add(userDocModel);
                db.SaveChanges();
            }
        }

        #endregion 新建文件夹（个人文档）

        #region 新建公司文件

        /// <summary>
        /// 新建公司文件
        /// </summary>
        /// <param name="folder">上级文件夹Id</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="tag">标签集合</param>
        /// <param name="userId">用户Id</param>
        /// <return></return>
        public int AddCompanyFile(AddNewFileModel fileModel, int userId)
        {
            var documentId = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                if (fileModel != null)
                {
                    foreach (var item in fileModel.File)
                    {
                        //公司文档表添加数据
                        System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;
                        var companyFile = new tblCompanyDocument
                        {
                            documentId = documentId,
                            folder = fileModel.folder,
                            displayName = item.displayname,
                            //archive = false,
                            isFolder = false,
                            createUser = userId,
                            createTime = DateTime.Now,
                            updateUser = userId,
                            updateTime = DateTime.Now,
                            deleteFlag = false,
                            saveName = item.saveName,
                            extension = item.extension,
                            //标签
                            keyword = fileModel.keyword != null && fileModel.keyword.Length > 0 ? string.Join(",", fileModel.keyword) : null
                        };
                        db.tblCompanyDocument.Add(companyFile);
                        if (fileModel.SheraUser.Length > 0)
                        {
                            int[] id = { documentId };
                            ShareToOthers(id, fileModel.SheraUser, 0);
                        }

                        var AAId = db.prcGetPrimaryKey("tblFolderAuth", obj).FirstOrDefault().Value;
                        var AA = new tblFolderAuth
                        {
                            authId = AAId,
                            documentId = documentId,
                            power = 4,
                            type = 1
                        };
                        db.tblFolderAuth.Add(AA);

                        //添加日志
                        AddCompanyDocumentLog(db, documentId, 4, null, userId);

                        db.SaveChanges();
                    }
                }
            }

            return documentId;
        }

        #endregion 新建公司文件

        #region 新建个人文件

        /// <summary>
        /// 新建个人文件
        /// </summary>
        /// <param name="folder">上级文件夹Id</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="tag">标签集合</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public int AddUserFile(AddNewFileModel fileModel, int userId)
        {
            var documentId = 0;

            using (var db = new TargetNavigationDBEntities())
            {
                if (fileModel.File.Count > 0)
                {
                    foreach (var item in fileModel.File)
                    {
                        //个人文档表添加数据
                        System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        documentId = db.prcGetPrimaryKey("tblUserDocument", obj).FirstOrDefault().Value;
                        var userFile = new tblUserDocument
                        {
                            documentId = documentId,
                            folder = fileModel.folder,
                            displayName = item.displayname,
                            //archive = false,
                            withShared = false,
                            isFolder = false,
                            createUser = userId,
                            createTime = DateTime.Now,
                            updateUser = userId,
                            updateTime = DateTime.Now,
                            deleteFlag = false,
                            saveName = item.saveName,
                            extension = item.extension,
                            //标签
                            keyword = fileModel.keyword != null && fileModel.keyword.Length > 0 ? string.Join(",", fileModel.keyword) : null
                        };
                        db.tblUserDocument.Add(userFile);
                        db.SaveChanges();
                        if (fileModel.SheraUser.Length > 0)
                        {
                            int[] id = { documentId };
                            ShareToOthers(id, fileModel.SheraUser, 0);
                        }
                    }
                }
            }

            return documentId;
        }

        #endregion 新建个人文件

        #region 公司文档下载添加日志
        /// <summary>
        /// 公司文档下载添加日志
        /// </summary>
        /// <param name="documentId">文档Id</param>
        /// <param name="type">文档类型</param>
        /// <param name="comment">操作说明</param>
        /// <param name="createUser">创建用户</param>
        public void AddDownloadLog(int documentId, int type, string comment, int createUser)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                AddCompanyDocumentLog(db, documentId, type, comment, createUser);
                db.SaveChanges();
            }
        }
        #endregion

        #region 添加公司文档操作日志

        /// <summary>
        /// 添加公司文档操作日志
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="documentId">文档Id</param>
        /// <param name="type">操作类型</param>
        /// <param name="comment">操作说明</param>
        /// <param name="createUser">操作人</param>
        public void AddCompanyDocumentLog(TargetNavigationDBEntities db, int documentId, int type, string comment, int createUser)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operateId = db.prcGetPrimaryKey("tblCompanyDocumentLog", obj).FirstOrDefault().Value;
            var operateModel = new tblCompanyDocumentLog
            {
                logId = operateId,
                documentId = documentId,
                type = type,
                comment = comment,
                createUser = createUser,
                createTime = DateTime.Now,
                updateUser = createUser,
                updateTime = DateTime.Now
            };
            db.tblCompanyDocumentLog.Add(operateModel);
        }

        #endregion 添加公司文档操作日志

        #region 获取公司文件夹目录(不含权限)

        /// <summary>
        /// 获取公司文件夹目录(不含权限)
        /// </summary>
        /// <param name="folder">上级文件夹目录</param>
        /// <returns>公司文件夹</returns>
        /// <param name="condition">条件：是否请求第一级目录</param>
        public List<FileDirectoryModel> GetFolderDirectory(int? folder, bool topItem = true)
        {
            var fileDirectoryList = new List<FileDirectoryModel>();
            if (folder == null && topItem == true)
            {
                fileDirectoryList.Add(new FileDirectoryModel
                {
                    id = 0,
                    name = "目录设置",
                    isParent = false
                });
            }

            using (var db = new TargetNavigationDBEntities())
            {
                var newFileDirectoryList = (from c in db.tblCompanyDocument
                                            where c.folder == folder && c.isFolder.Value && !c.deleteFlag
                                            select new FileDirectoryModel
                                            {
                                                id = c.documentId,
                                                name = c.displayName
                                            }).ToList();
                //验证每个目录是否含有子级
                if (newFileDirectoryList.Count > 0)
                {
                    foreach (var item in newFileDirectoryList)
                    {
                        if (db.tblCompanyDocument.Where(p => p.folder == item.id && p.isFolder.Value && !p.deleteFlag).Count() > 0)
                        {
                            item.isParent = true;
                        }
                    }
                    fileDirectoryList.AddRange(newFileDirectoryList);
                }
            }
            return fileDirectoryList;
        }

        #endregion 获取公司文件夹目录(不含权限)

        #region 获取公司文件夹目录(含权限)

        /// <summary>
        /// 获取公司文件夹目录(含权限)
        /// </summary>
        /// <param name="folder">上级文件夹目录</param>
        /// <param name="userId">用户Id</param>
        /// <returns>公司文件夹</returns>
        public List<FileDirectoryModel> GetFolderDirectoryHasAuth(int? folder, int userId)
        {
            var fileDirectoryList = new List<FileDirectoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //按人员设置权限的文档列表
                var personAuthDocuments = (from c in db.tblCompanyDocument
                                           join f in db.tblFolderAuth on c.documentId equals f.documentId
                                           join a in db.tblAuthResult on f.authId equals a.authId
                                           join u in db.tblUser on a.targetId equals u.userId
                                           where c.folder == folder && c.isFolder.Value && !c.deleteFlag && f.type == 3 && a.targetId == userId && (f.power == 3 || f.power == 4 || f.power == 2)
                                           select new FileDirectoryModel
                                           {
                                               id = c.documentId,
                                               name = c.displayName,
                                               power = f.power
                                           }).ToList();

                if (personAuthDocuments.Count() > 0)
                {
                    foreach (var item in personAuthDocuments)
                    {
                        int IsparentCont = (from c in db.tblCompanyDocument
                                            where c.folder == item.id && c.isFolder.Value && c.deleteFlag != true
                                            select c).Count();
                        if (IsparentCont > 0)
                        {
                            item.isParent = true;
                        }
                    }
                    foreach (var item in personAuthDocuments)
                    {
                        if (fileDirectoryList.Where(p => p.id == item.id).Count() <= 1)
                        {
                            fileDirectoryList.Add(item);
                        }
                    }
                }
                //按岗位设置权限的文档列表
                var stationAuthDocuments = (from c in db.tblCompanyDocument
                                            join f in db.tblFolderAuth on c.documentId equals f.documentId
                                            join a in db.tblAuthResult on f.authId equals a.authId
                                            join u in db.tblUserStation on a.targetId equals u.stationId
                                            where c.folder == folder && c.isFolder.Value && !c.deleteFlag && f.type == 2 && u.userId == userId && (f.power == 3 || f.power == 4 || f.power == 2)
                                            select new FileDirectoryModel
                                            {
                                                id = c.documentId,
                                                name = c.displayName,
                                                power = f.power
                                            }).ToList();
                if (stationAuthDocuments.Count() > 0)
                {
                    foreach (var item in stationAuthDocuments)
                    {
                        int IsparentCont = (from c in db.tblCompanyDocument
                                            join f in db.tblFolderAuth on c.documentId equals f.documentId
                                            join a in db.tblAuthResult on f.authId equals a.authId
                                            join u in db.tblUserStation on a.targetId equals u.stationId
                                            where c.folder == item.id && c.isFolder.Value && c.deleteFlag != true && f.type == 2 && u.userId == userId && (f.power == 3 || f.power == 4 || f.power == 2)
                                            select c).Count();
                        if (IsparentCont > 0)
                        {
                            item.isParent = true;
                        }
                    }
                    foreach (var item in stationAuthDocuments)
                    {
                        if (fileDirectoryList.Where(p => p.id == item.id).Count() <= 0)
                        {
                            fileDirectoryList.Add(item);
                        }
                    }
                }
                //按组织架构设置的文档列表
                var orgAuthDocuments = (from c in db.tblCompanyDocument
                                        join f in db.tblFolderAuth on c.documentId equals f.documentId
                                        join a in db.tblAuthResult on f.authId equals a.authId
                                        join s in db.tblStation on a.targetId equals s.organizationId
                                        join u in db.tblUserStation on s.stationId equals u.stationId
                                        where c.folder == folder && c.isFolder.Value && !c.deleteFlag && f.type == 1 && u.userId == userId && (f.power == 3 || f.power == 4 || f.power == 2)
                                        select new FileDirectoryModel
                                        {
                                            id = c.documentId,
                                            name = c.displayName,
                                            power = f.power
                                        }).ToList();
                if (orgAuthDocuments.Count() > 0)
                {
                    foreach (var item in orgAuthDocuments)
                    {
                        int IsparentCont = (from c in db.tblCompanyDocument
                                            join f in db.tblFolderAuth on c.documentId equals f.documentId
                                            join a in db.tblAuthResult on f.authId equals a.authId
                                            join s in db.tblStation on a.targetId equals s.organizationId
                                            join u in db.tblUserStation on s.stationId equals u.stationId
                                            where c.folder == item.id && c.isFolder.Value && c.deleteFlag != true && f.type == 1 && u.userId == userId && (f.power == 3 || f.power == 4 || f.power == 2)
                                            select c).Count();
                        if (IsparentCont > 0)
                        {
                            item.isParent = true;
                        }
                    }
                    foreach (var item in orgAuthDocuments)
                    {
                        if (fileDirectoryList.Where(p => p.id == item.id).Count() <= 0)
                        {
                            fileDirectoryList.Add(item);
                        }
                    }
                }
            }
            return fileDirectoryList;
        }

        #endregion 获取公司文件夹目录(含权限)

        #region 模糊查询文件夹

        /// <summary>
        /// 模糊查询文件夹
        /// </summary>
        /// <param name="type">查询类型:1、公司文档 2、个人文档</param>
        /// <param name="folderName">文件夹名称</param>
        /// <returns>文件夹列表</returns>
        public List<FileDirectoryModel> GetLikeFolderList(int type, string folderName, int userId)
        {
            var fileDirectoryList = new List<FileDirectoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (type == 1)  //模糊查询公司文件夹
                {
                    fileDirectoryList = (from c in db.tblCompanyDocument
                                         where c.isFolder.Value && c.deleteFlag == false
                                         select new FileDirectoryModel
                                         {
                                             id = c.documentId,
                                             name = c.displayName,
                                             deletefalse = c.deleteFlag
                                         }).ToList();
                }
                else if (type == 2) //模糊查询个人文件夹
                {
                    fileDirectoryList = (from c in db.tblUserDocument
                                         where c.displayName.IndexOf(folderName) != -1 && c.isFolder.Value && c.deleteFlag == false && c.createUser == userId
                                         select new FileDirectoryModel
                                         {
                                             id = c.documentId,
                                             name = c.displayName
                                         }).ToList();
                }
                else
                {
                    //按人员设置权限的文档列表
                    var personAuthDocuments = (from c in db.tblCompanyDocument
                                               join f in db.tblFolderAuth on c.documentId equals f.documentId
                                               join a in db.tblAuthResult on f.authId equals a.authId
                                               join u in db.tblUser on a.targetId equals u.userId
                                               where c.displayName.IndexOf(folderName) != -1 && c.isFolder.Value && !c.deleteFlag && f.type == 3 && a.targetId == userId && (f.power == 3 || f.power == 4)
                                               select new FileDirectoryModel
                                               {
                                                   id = c.documentId,
                                                   name = c.displayName
                                               }).ToList();

                    foreach (var item in personAuthDocuments)
                    {
                        fileDirectoryList.Add(item);
                    }

                    //按岗位设置权限的文档列表
                    var stationAuthDocuments = (from c in db.tblCompanyDocument
                                                join f in db.tblFolderAuth on c.documentId equals f.documentId
                                                join a in db.tblAuthResult on f.authId equals a.authId
                                                join u in db.tblUserStation on a.targetId equals u.stationId
                                                where c.displayName.IndexOf(folderName) != -1 && c.isFolder.Value && !c.deleteFlag && f.type == 2 && u.userId == userId && (f.power == 3 || f.power == 4)
                                                select new FileDirectoryModel
                                                {
                                                    id = c.documentId,
                                                    name = c.displayName
                                                }).ToList();

                    foreach (var item in stationAuthDocuments)
                    {
                        if (fileDirectoryList.Where(p => p.id == item.id).Count() <= 0)
                        {
                            fileDirectoryList.Add(item);
                        }
                    }

                    //按组织架构设置的文档列表
                    var orgAuthDocuments = (from c in db.tblCompanyDocument
                                            join f in db.tblFolderAuth on c.documentId equals f.documentId
                                            join a in db.tblAuthResult on f.authId equals a.authId
                                            join s in db.tblStation on a.targetId equals s.organizationId
                                            join u in db.tblUserStation on s.stationId equals u.stationId
                                            where c.displayName.IndexOf(folderName) != -1 && c.isFolder.Value && !c.deleteFlag && f.type == 1 && u.userId == userId && (f.power == 3 || f.power == 4)
                                            select new FileDirectoryModel
                                            {
                                                id = c.documentId,
                                                name = c.displayName
                                            }).ToList();

                    foreach (var item in orgAuthDocuments)
                    {
                        if (fileDirectoryList.Where(p => p.id == item.id).Count() <= 0)
                        {
                            fileDirectoryList.Add(item);
                        }
                    }
                }
            }
            return fileDirectoryList;
        }

        #endregion 模糊查询文件夹

        #region 根据文档Id获取文档集合(公司文档，批量下载用到)

        /// <summary>
        /// 根据文档Id获取文档集合(公司文档)
        /// </summary>
        /// <param name="documentIds">文档Id集合</param>
        /// <returns>文档集合</returns>
        public List<DocumentModel> GetDocumentListByIds(int[] documentIds)
        {
            var docList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in documentIds)
                {
                    var docModel = (from c in db.tblCompanyDocument
                                    where c.documentId == item && c.deleteFlag == false
                                    select new DocumentModel
                                    {
                                        documentId = c.documentId,
                                        displayName = c.displayName,
                                        saveName = c.saveName,
                                        isFolder = c.isFolder
                                    }).FirstOrDefault();
                    if (docModel != null)
                    {
                        if (docModel.isFolder.Value)
                        {
                            var path = docModel.displayName;
                            docList.AddRange(GetAllDocumentInFolder(docModel.documentId, new List<DocumentModel>(), ref path));
                        }
                        docList.Add(docModel);
                    }
                }
            }
            return docList;
        }

        /// <summary>
        /// 根据文档Id获取文档集合(公司文档)
        /// </summary>
        /// <param name="documentIds">文档Id集合</param>
        /// <returns>文档集合</returns>
        public List<DocumentModel> GetDocumentListByIds(int[] documentIds, int userId)
        {
            var docList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in documentIds)
                {
                    var docModel = (from c in db.tblCompanyDocument
                                    where c.documentId == item && c.deleteFlag == false
                                    select new DocumentModel
                                    {
                                        documentId = c.documentId,
                                        displayName = c.displayName,
                                        saveName = c.saveName,
                                        isFolder = c.isFolder
                                    }).FirstOrDefault();
                    if (docModel.isFolder.Value)
                    {
                        var path = docModel.displayName;
                        docList.AddRange(GetAllDocumentInFolder(docModel.documentId, userId, new List<DocumentModel>(), ref path));
                    }
                    if (docModel != null) docList.Add(docModel);
                }
            }
            return docList;
        }

        #endregion 根据文档Id获取文档集合(公司文档，批量下载用到)

        #region 根据文档Id获取文档集合(用户文档，批量下载用到)

        /// <summary>
        /// 根据文档Id获取文档集合(用户文档)
        /// </summary>
        /// <param name="documentIds">文档Id集合</param>
        /// <returns>文档集合</returns>
        public List<DocumentModel> GetUserDocumentListByIds(int[] documentIds)
        {
            var docList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in documentIds)
                {
                    var docModel = (from c in db.tblUserDocument
                                    where c.documentId == item && c.deleteFlag == false
                                    select new DocumentModel
                                    {
                                        documentId = c.documentId,
                                        displayName = c.displayName,
                                        saveName = c.saveName,
                                        isFolder = c.isFolder
                                    }).FirstOrDefault();
                    if (docModel != null)
                    {
                        if (docModel.isFolder.Value)
                        {
                            var path = docModel.displayName;
                            docList.AddRange(GetUserDocumentInFolder(docModel.documentId, new List<DocumentModel>(), ref path));
                        }
                        docList.Add(docModel);
                    }
                }
            }
            return docList;
        }

        #endregion 根据文档Id获取文档集合(用户文档，批量下载用到)

        #region 获取个人文件夹目录

        /// <summary>
        /// 获取个人文件夹目录
        /// </summary>
        /// <param name="folder">上级文件夹目录</param>
        /// <param name="userId">用户Id</param>
        /// <returns>个人文件夹目录</returns>
        public List<FileDirectoryModel> GetUserFolderDirectory(int? folder, int userId)
        {
            var fileDirectoryList = new List<FileDirectoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                fileDirectoryList = (from c in db.tblUserDocument
                                     where c.folder == folder && c.isFolder.Value && !c.deleteFlag && c.createUser == userId
                                     select new FileDirectoryModel
                                     {
                                         id = c.documentId,
                                         name = c.displayName
                                     }).ToList();
                if (fileDirectoryList.Count > 0)
                {
                    foreach (var item in fileDirectoryList)
                    {
                        if (db.tblUserDocument.Where(p => p.folder == item.id && p.isFolder.Value && !p.deleteFlag).Count() > 0)
                        {
                            item.isParent = true;
                        }
                    }
                }
            }
            return fileDirectoryList;
        }

        #endregion 获取个人文件夹目录

        #region 查询最近5个上传文档的人员（公司文档）

        /// <summary>
        /// 查询最近5个上传文档的人员（公司文档）
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetLastFiveCreateUser()
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from c in db.tblCompanyDocument
                            join u in db.tblUser on c.createUser equals u.userId
                            orderby c.createTime descending
                            where !c.deleteFlag
                            select new UserInfo
                            {
                                id = c.createUser,
                                name = u.userName,
                                img = u.smallImage
                            }).Distinct().Take(5).ToList();
            }
            return userList;
        }

        #endregion 查询最近5个上传文档的人员（公司文档）

        #region 查询用户文档创建人(自己、共享给他人，他人共享)--暂时没用到

        /// <summary>
        /// 查询用户文档创建人
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>用户集合</returns>
        public List<UserInfo> GetUserDocCreateUsers(int userId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //1、用户自己
                var user = (from u in db.tblUser
                            where u.userId == userId && !u.deleteFlag
                            select new UserInfo
                            {
                                userId = u.userId,
                                userName = u.userName,
                                smallImage = u.smallImage
                            }).FirstOrDefault();
                if (user != null) userList.Add(user);
                //2、共享给他人
                var shareOthers = (from c in db.tblUserDocument
                                   join ds in db.tblDocumentShared on c.documentId equals ds.documentId
                                   join u in db.tblUser on ds.userId equals u.userId
                                   where c.createUser == userId && c.withShared.Value && !c.deleteFlag && u.deleteFlag
                                   select new UserInfo
                                   {
                                       userId = u.userId,
                                       userName = u.userName,
                                       smallImage = u.smallImage
                                   }).ToList();
                if (shareOthers.Count() > 0) userList.AddRange(shareOthers);
                //3、他人共享
                var otherShares = (from c in db.tblUserDocument
                                   join ds in db.tblDocumentShared on c.documentId equals ds.documentId
                                   join u in db.tblUser on ds.userId equals u.userId
                                   where ds.userId == userId && c.withShared.Value && !c.deleteFlag && u.deleteFlag
                                   select new UserInfo
                                     {
                                         userId = u.userId,
                                         userName = u.userName,
                                         smallImage = u.smallImage
                                     }).ToList();
                if (otherShares.Count() > 0) userList.AddRange(otherShares);
            }
            return userList;
        }

        #endregion 查询用户文档创建人(自己、共享给他人，他人共享)--暂时没用到

        #region 模糊查询用户列表

        /// <summary>
        /// 模糊查询用户列表
        /// </summary>
        /// <param name="text">模糊查询的字段</param>
        /// <returns>用户列表</returns>
        public List<UserInfo> SelectUserList(string text)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from us in db.tblUser
                            where us.userName.IndexOf(text) != -1 && !us.deleteFlag && us.workStatus == (int)ConstVar.workStatus.OnWork
                            select new UserInfo
                            {
                                id = us.userId,
                                name = us.userName,
                                img = us.smallImage
                            }).ToList();
            }
            return userList;
        }

        /// <summary>
        /// 模糊查询用户信息列表（流程测试画面用）
        /// </summary>
        /// <param name="text">模糊查询的字段</param>
        /// <returns>用户信息列表</returns>
        public List<UserInfo> SelectUserInfoList(string text)
        {
            string path = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from u in db.tblUser
                            join us in db.tblUserStation
                                on u.userId equals us.userId
                            join s in db.tblStation
                                on us.stationId equals s.stationId
                            join o in db.tblOrganization
                             on s.organizationId equals o.organizationId
                            where u.userName.IndexOf(text) >= 0 && u.workStatus == (int)ConstVar.workStatus.OnWork && !u.deleteFlag
                            select new UserInfo
                            {
                                id = u.userId,
                                name = u.userName,
                                affiliationName = s.stationName + "-" + o.organizationName,
                                img = string.IsNullOrEmpty(u.originalImage) ? "/Images/common/portrait.png" : "/" + path + "/" + u.smallImage,
                                userId = u.userId,
                                userName = u.userName,
                                stationId = s.stationId,
                                stationName = s.stationName,
                                organizationId = o.organizationId,
                                organizationName = o.organizationName
                            }).ToList();
            }
            return userList;
        }

        #endregion 模糊查询用户列表

        #region 公司文档复制（含批量）

        /// <summary>
        /// 公司文档复制（含批量）
        /// </summary>
        /// <param name="documentIds">要复制的文件</param>
        /// <param name="companyFolder">复制到的公司文件夹</param>
        /// <param name="userFolder">复制到的个人文件夹</param>
        /// <param name="userId">登陆用户</param>
        /// <returns></returns>
        public void CopyDocument(int[] documentIds, int?[] companyFolder, int?[] userFolder, int userId, bool withAuth)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length == 0)
                {
                    return;
                }

                foreach (var id in documentIds)
                {
                    // 获取公司文档信息
                    var docModel = this.GetCompanyDocument(id, db);

                    if (docModel == null)
                    {
                        continue;
                    }

                    // 目标文件夹为公司文件夹
                    if (companyFolder != null)
                    {
                        foreach (var target in companyFolder)
                        {
                            // 复制到公司文件夹
                            var newDocumentId = this.CopyToCompanyDocument(docModel, target, null, userId, db, 2);

                            //添加日志
                            AddCompanyDocumentLog(db, docModel.documentId, 2, string.Empty, userId);

                            // 文件夹的场合，复制子文件到目标文件夹
                            if (docModel.isFolder == true)
                            {
                                this.CopyCompanyChildToCompanyDocument(id, newDocumentId, userId, withAuth, db);
                            }
                        }
                    }

                    // 目标文件夹为公司文件夹
                    if (userFolder != null)
                    {
                        foreach (var target in userFolder)
                        {
                            // 复制到公司文件夹
                            var newDocumentId = this.CopyToUserDocument(docModel, target.Value, userId, db, 2);

                            //添加日志
                            AddCompanyDocumentLog(db, docModel.documentId, 2, string.Empty, userId);

                            // 文件夹的场合，复制子文件到目标文件夹
                            if (docModel.isFolder == true)
                            {
                                this.CopyCompanyChildToUserDocument(id, newDocumentId, userId, db);
                            }
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        #region 注释

        /// <summary>
        /// 公司文档复制（含批量）
        /// </summary>
        /// <param name="documentIds">要复制的文件</param>
        /// <param name="folders">复制到的目标文件夹</param>
        /// <param name="userId">用户Id</param>
        /// <returns>flag:true、复制成功  false:复制失败</returns>
        //public bool CopyDocument(int[] documentIds, int?[] folders, int userId)
        //{
        //    var flag = false;
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        if (documentIds.Length > 0)
        //        {
        //            foreach (var item in documentIds)
        //            {
        //                var docModel = (from c in db.tblCompanyDocument
        //                                where c.documentId == item && c.deleteFlag==false
        //                                select new DocumentModel
        //                                {
        //                                    documentId = c.documentId,
        //                                    folder = c.folder,
        //                                    displayName = c.displayName,
        //                                    description = c.description,
        //                                    saveName = c.saveName,
        //                                    extension = c.extension,
        //                                    archive = c.archive,
        //                                    archiveTime = c.archiveTime,
        //                                    isFolder = c.isFolder,
        //                                    deleteFlag = c.deleteFlag

        //                                }).FirstOrDefault();
        //                if (docModel != null)
        //                {
        //                    //服务器上文档地址
        //                    var path = Path.Combine(FilePath.DocumentUpLoadPath, docModel.saveName == null ? string.Empty : docModel.saveName);

        //                    if (docModel.isFolder.Value || File.Exists(path))
        //                    {
        //                        flag = true;
        //                        //先存下被复制文件的id，留作记录日志
        //                        var oldDocumentId = docModel.documentId;
        //                        foreach (var folderModel in folders)
        //                        {
        //                            //生成存储名
        //                            var rd = new Random();
        //                            int numName = rd.Next(1000, 9999);
        //                            var saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
        //                            //新建复制的文件
        //                            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
        //                            var documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;
        //                            docModel.documentId = documentId;
        //                            var newDocModel = new tblCompanyDocument
        //                            {
        //                                documentId = docModel.documentId,
        //                                folder = folderModel,
        //                                displayName = docModel.displayName,
        //                                description = docModel.description,
        //                                saveName = docModel.isFolder == false ? saveName : null,
        //                                extension = docModel.extension,
        //                                archive = docModel.archive,
        //                                archiveTime = docModel.archiveTime,
        //                                isFolder = docModel.isFolder,
        //                                createUser = userId,
        //                                createTime = DateTime.Now,
        //                                updateUser = userId,
        //                                updateTime = DateTime.Now,
        //                                deleteFlag = docModel.deleteFlag.Value
        //                            };
        //                            db.tblCompanyDocument.Add(newDocModel);
        //                            //更新所有子文档的上级目录
        //                            UpdateChildDocuments(db, oldDocumentId, docModel.documentId, userId, 1, flag);

        //                            if (!newDocModel.isFolder.Value&&File.Exists(path))
        //                            {
        //                                var newPath = Path.Combine(FilePath.DocumentUpLoadPath, newDocModel.saveName);
        //                                // 服务器上复制文档
        //                                File.Copy(path, newPath, true);
        //                            }
        //                        }
        //                        //添加日志
        //                        AddCompanyDocumentLog(db, oldDocumentId, 2, string.Empty, userId);
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return flag;
        //}

        #endregion 注释

        #endregion 公司文档复制（含批量）

        #region 个人文档复制（含批量）

        /// <summary>
        /// 个人文档复制（含批量）
        /// </summary>
        /// <param name="documentIds">要复制的文件</param>
        /// <param name="companyFolder">复制到的公司文件夹</param>
        /// <param name="userFolder">复制到的个人文件夹</param>
        /// <param name="userId">登陆用户</param>
        /// <returns></returns>
        public void CopyUserDocument(int[] documentIds, int?[] companyFolder, int?[] userFolder, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length == 0)
                {
                    return;
                }

                foreach (var id in documentIds)
                {
                    // 获取公司文档信息
                    var docModel = this.GetUserDocument(id, db);

                    if (docModel == null)
                    {
                        continue;
                    }

                    // 目标文件夹为公司文件夹
                    if (companyFolder != null)
                    {
                        foreach (var target in companyFolder)
                        {
                            // 复制到公司文件夹
                            var newDocumentId = this.CopyToCompanyDocument(docModel, target.Value, null, userId, db, 1);

                            // 文件夹的场合，复制子文件到目标文件夹
                            if (docModel.isFolder == true)
                            {
                                this.CopyUserChildToCompanyDocument(id, newDocumentId, target, userId, db);
                            }
                        }
                    }

                    // 目标文件夹为个人文件夹
                    if (userFolder != null)
                    {
                        foreach (var target in userFolder)
                        {
                            // 复制到个人文件夹
                            var newDocumentId = this.CopyToUserDocument(docModel, target.Value, userId, db, 1);

                            // 文件夹的场合，复制子文件到目标文件夹
                            if (docModel.isFolder == true)
                            {
                                this.CopyUserChildToUserDocument(id, newDocumentId, userId, db);
                            }
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        #region 注释

        /// <summary>
        /// 个人文档复制（含批量）
        /// </summary>
        /// <param name="documentType">源文档类型:1、公司文档 2、个人文档</param>
        /// <param name="documentIds">要复制的文件</param>
        /// <param name="folders">复制到的目标文件夹</param>
        /// <param name="userId">用户Id</param>
        /// <returns>flag:true、复制成功  false:复制失败</returns>
        //public bool CopyUserDocument(int documentType, int[] documentIds, int?[] folders, int userId)
        //{
        //    var flag = false;
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        if (documentIds.Length > 0)
        //        {
        //            foreach (var item in documentIds)
        //            {
        //                var docModel = new DocumentModel();
        //                if (documentType == 1)
        //                {
        //                    docModel = (from c in db.tblCompanyDocument
        //                                where c.documentId == item && c.deleteFlag == false
        //                                select new DocumentModel
        //                                {
        //                                    documentId = c.documentId,
        //                                    folder = c.folder,
        //                                    displayName = c.displayName,
        //                                    description = c.description,
        //                                    saveName = c.saveName,
        //                                    extension = c.extension,
        //                                    archive = c.archive,
        //                                    archiveTime = c.archiveTime,
        //                                    isFolder = c.isFolder,
        //                                    deleteFlag = c.deleteFlag
        //                                }).FirstOrDefault();
        //                }
        //                else
        //                {
        //                    docModel = (from c in db.tblUserDocument
        //                                where c.documentId == item && c.deleteFlag == false
        //                                select new DocumentModel
        //                                {
        //                                    documentId = c.documentId,
        //                                    folder = c.folder,
        //                                    displayName = c.displayName,
        //                                    description = c.description,
        //                                    saveName = c.saveName,
        //                                    extension = c.extension,
        //                                    //archive = c.archive,
        //                                    //archiveTime = c.archiveTime,
        //                                    withShared = c.withShared,
        //                                    isFolder = c.isFolder,
        //                                    deleteFlag = c.deleteFlag

        //                                }).FirstOrDefault();
        //                }

        //                if (docModel != null)
        //                {
        //                    //服务器上文档地址
        //                    var path = Path.Combine(FilePath.DocumentUpLoadPath, docModel.saveName == null ? string.Empty : docModel.saveName);
        //                    if (docModel.isFolder.Value || File.Exists(path))
        //                    {
        //                        flag = true;
        //                        //先存下被复制文件的id，留作记录日志
        //                        var oldDocumentId = docModel.documentId;
        //                        foreach (var folderModel in folders)
        //                        {
        //                            //生成存储名
        //                            var rd = new Random();
        //                            int numName = rd.Next(1000, 9999);
        //                            var saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
        //                            //新建复制的文件
        //                            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
        //                            var documentId = db.prcGetPrimaryKey("tblUserDocument", obj).FirstOrDefault().Value;
        //                            docModel.documentId = documentId;
        //                            var newDocModel = new tblUserDocument
        //                            {
        //                                documentId = docModel.documentId,
        //                                folder = folderModel,
        //                                displayName = docModel.displayName,
        //                                description = docModel.description,
        //                                saveName = docModel.isFolder == false ? saveName : null,
        //                                extension = docModel.extension,
        //                                //archive = docModel.archive,
        //                                //archiveTime = docModel.archiveTime,
        //                                withShared = docModel.withShared,
        //                                isFolder = docModel.isFolder,
        //                                createUser = userId,
        //                                createTime = DateTime.Now,
        //                                updateUser = userId,
        //                                updateTime = DateTime.Now,
        //                                deleteFlag = docModel.deleteFlag.Value
        //                            };
        //                            db.tblUserDocument.Add(newDocModel);
        //                            //更新所有子文档的上级目录
        //                            UpdateChildDocuments(db, oldDocumentId, docModel.documentId, userId, 2, flag);

        //                            if (!newDocModel.isFolder.Value && File.Exists(path))
        //                            {
        //                                var newPath = Path.Combine(FilePath.DocumentUpLoadPath, newDocModel.saveName);
        //                                //服务器上复制文档
        //                                File.Copy(path, newPath, true);
        //                            }
        //                        }
        //                        db.SaveChanges();
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return flag;
        //}

        #endregion 注释

        #endregion 个人文档复制（含批量）

        #region 复制该目录下的所有文档

        /// <summary>
        /// 复制该目录下的所有文档
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="oldDocumentId">复制前的文档Id</param>
        /// <param name="newDocumentId">复制后的文档Id</param>
        /// <param name="type">1、公司文档 2、个人文档</param>
        /// <param name="userId">用户名</param>
        /// <param name="flag">操作结果的标识</param>
        /// <returns>flag:true、复制成功  false:复制失败</returns>
        public bool UpdateChildDocuments(TargetNavigationDBEntities db, int oldDocumentId, int newDocumentId, int userId, int type, bool flag)
        {
            if (type == 1)
            {
                var list = (from c in db.tblCompanyDocument
                            where c.folder == oldDocumentId && c.deleteFlag == false
                            select new DocumentModel
                            {
                                documentId = c.documentId,
                                folder = c.folder,
                                displayName = c.displayName,
                                description = c.description,
                                saveName = c.saveName,
                                extension = c.extension,
                                archive = c.archive,
                                archiveTime = c.archiveTime,
                                isFolder = c.isFolder,
                                deleteFlag = c.deleteFlag
                            }).ToList();
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        //服务器上文档地址
                        var path = Path.Combine(FilePath.DocumentUpLoadPath, item.saveName == null ? string.Empty : item.saveName);
                        if (flag)
                        {
                            //生成存储名
                            int numName = StringUtils.GetRandom();
                            var saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
                            //给上级文件夹赋值
                            item.folder = newDocumentId;
                            //存下被复制文档的Id，供记录日志使用
                            var lastDocumentId = item.documentId;
                            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                            var documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;
                            item.documentId = documentId;
                            var document = new tblCompanyDocument
                            {
                                documentId = item.documentId,
                                folder = item.folder,
                                displayName = item.displayName,
                                description = item.description,
                                saveName = item.isFolder == false ? saveName : null,
                                extension = item.extension,
                                archive = item.archive,
                                archiveTime = item.archiveTime,
                                isFolder = item.isFolder,
                                createUser = userId,
                                createTime = DateTime.Now,
                                updateUser = userId,
                                updateTime = DateTime.Now,
                                deleteFlag = item.deleteFlag.Value
                            };
                            db.tblCompanyDocument.Add(document);
                            if (!document.isFolder.Value && File.Exists(path))
                            {
                                var newPath = Path.Combine(FilePath.DocumentUpLoadPath, document.saveName);
                                //服务器上复制文档
                                File.Copy(path, newPath, true);
                            }

                            //添加日志
                            AddCompanyDocumentLog(db, lastDocumentId, 2, string.Empty, userId);
                            flag = UpdateChildDocuments(db, lastDocumentId, item.documentId, userId, 1, flag);
                        }
                        else
                        {
                            // return continue;
                        }
                    }
                }
            }
            else
            {
                var list = (from c in db.tblUserDocument
                            where c.folder == oldDocumentId && c.deleteFlag == false
                            select new DocumentModel
                            {
                                documentId = c.documentId,
                                folder = c.folder,
                                displayName = c.displayName,
                                description = c.description,
                                saveName = c.saveName,
                                extension = c.extension,
                                //archive = c.archive,
                                //archiveTime = c.archiveTime,
                                isFolder = c.isFolder,
                                deleteFlag = c.deleteFlag
                            }).ToList();
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        //服务器上文档地址
                        var path = Path.Combine(FilePath.DocumentUpLoadPath, item.saveName == null ? string.Empty : item.saveName);
                        if (item.isFolder.Value || (File.Exists(path) && flag))
                        {
                            //生成存储名
                            int numName = StringUtils.GetRandom();
                            var saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();

                            item.folder = newDocumentId;
                            var lastDocumentId = item.documentId;
                            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                            var documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;
                            item.documentId = documentId;
                            var document = new tblUserDocument
                            {
                                documentId = item.documentId,
                                folder = item.folder,
                                displayName = item.displayName,
                                description = item.description,
                                saveName = item.isFolder == false ? saveName : null,
                                extension = item.extension,
                                //archive = item.archive,
                                //archiveTime = item.archiveTime,
                                isFolder = item.isFolder,
                                createUser = userId,
                                createTime = DateTime.Now,
                                updateUser = userId,
                                updateTime = DateTime.Now,
                                deleteFlag = item.deleteFlag.Value
                            };
                            db.tblUserDocument.Add(document);

                            if (!document.isFolder.Value && File.Exists(path))
                            {
                                var newPath = Path.Combine(FilePath.DocumentUpLoadPath, document.saveName);
                                //服务器上复制文档
                                File.Copy(path, newPath, true);
                            }
                            flag = UpdateChildDocuments(db, lastDocumentId, item.documentId, userId, 2, flag);
                        }
                        else continue;
                    }
                }
            }
            return flag;
        }

        #endregion 复制该目录下的所有文档

        #region 个人文档共享给他人

        /// <summary>
        /// 个人文档共享给他人
        /// </summary>
        /// <param name="documentId">文档Id</param>
        /// <param name="userIds">用户Id集合</param>
        public bool ShareToOthers(int[] documentId, int[] userIds, int flags)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (int i in documentId)
                {
                    if (flags == 0)
                    {
                        var firstData = db.tblDocumentShared.Where(p => p.documentId == i).ToList();
                        if (firstData.Count > 0)
                        {
                            if (userIds.Length == 0)
                            {
                                this.NoShareToOthers(new int[] { i });
                            }
                            else
                            {
                                foreach (var item in firstData)
                                {
                                    db.tblDocumentShared.Remove(item);
                                }
                            }
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var firstData = db.tblDocumentShared.Where(p => p.documentId == i).ToList();
                        if (firstData != null)
                        {
                            foreach (var item in firstData)
                            {
                                foreach (int id in userIds)
                                {
                                    if (id == item.userId)
                                    {
                                        db.tblDocumentShared.Remove(item);
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                    }

                    var userDocModel = db.tblUserDocument.Where(p => p.documentId == i).FirstOrDefault();
                    if (userDocModel != null && userIds.Length > 0)
                    {
                        userDocModel.withShared = true;
                        userDocModel.updateTime = DateTime.Now;
                        foreach (var item in userIds)
                        {
                            var shareModel = new tblDocumentShared
                            {
                                documentId = i,
                                userId = item
                            };
                            db.tblDocumentShared.Add(shareModel);
                            flag = true;
                        }
                        db.SaveChanges();
                    }
                }
            }
            return flag;
        }

        #endregion 个人文档共享给他人

        #region 取消共享

        /// <summary>
        /// 取消共享
        /// </summary>
        /// <param name="documentId">文档Id</param>
        public bool NoShareToOthers(int[] documentId)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in documentId)
                {
                    var userDocModel = db.tblUserDocument.Where(p => p.documentId == item).FirstOrDefault();
                    if (userDocModel != null)
                    {
                        //更新用户文档共享字段
                        userDocModel.withShared = false;
                        userDocModel.updateTime = DateTime.Now;

                        //删除个人文档共享表中该文档的共享人
                        var shareList = db.tblDocumentShared.Where(p => p.documentId == item);
                        db.tblDocumentShared.RemoveRange(shareList);
                        db.SaveChanges();
                    }
                }
                flag = true;
            }
            return flag;
        }

        #endregion 取消共享

        #region 获取文档共享人

        /// <summary>
        /// 获取文档共享人
        /// </summary>
        /// <param name="documentId">文档Id</param>
        /// <returns>共享人列表</returns>
        public List<UserInfo> GetUserList(int documentId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from c in db.tblDocumentShared
                            join u in db.tblUser
                            on c.userId equals u.userId
                            where c.documentId == documentId && !u.deleteFlag
                            select new UserInfo
                            {
                                userId = u.userId,
                                userName = u.userName,
                                smallImage = u.smallImage
                            }).ToList();
            }
            return userList;
        }

        #endregion 获取文档共享人

        #region 根据当前文档Id获取所有的子文档Id(公司文档)--暂时没用到

        /// <summary>
        /// 根据当前文档Id获取所有的子文档Id(公司文档)
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="documentModel">文档实体</param>
        /// <param name="companyDocList">公司文档集合</param>
        /// <returns>文档Id集合</returns>
        public List<tblCompanyDocument> GetAllDocumentList(TargetNavigationDBEntities db, tblCompanyDocument documentModel, List<tblCompanyDocument> companyDocList)
        {
            companyDocList.Add(documentModel);
            var list = (from c in db.tblCompanyDocument where !c.deleteFlag && c.folder == documentModel.documentId select c).ToList<tblCompanyDocument>();
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var tempList = GetAllDocumentList(db, item, companyDocList);
                    if (tempList.Count() <= 0) continue;
                }
            }
            return companyDocList;
        }

        #endregion 根据当前文档Id获取所有的子文档Id(公司文档)--暂时没用到

        #region 根据当前文档Id获取所有的子文档Id(用户文档)--暂时没用到

        /// <summary>
        /// 根据当前文档Id获取所有的子文档Id(用户文档)
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="documentModel">文档实体</param>
        /// <param name="companyDocList">用户文档集合</param>
        /// <returns>该目录下的所有子文档集合</returns>
        public List<tblUserDocument> GetAllUserDocumentList(TargetNavigationDBEntities db, tblUserDocument documentModel, List<tblUserDocument> userDocList)
        {
            userDocList.Add(documentModel);
            var list = (from c in db.tblUserDocument where !c.deleteFlag && c.folder == documentModel.documentId select c).ToList<tblUserDocument>();
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var tempList = GetAllUserDocumentList(db, item, userDocList);
                    if (tempList.Count() <= 0) continue;
                }
            }
            return userDocList;
        }

        #endregion 根据当前文档Id获取所有的子文档Id(用户文档)--暂时没用到

        #region 公司文档移动（含批量）

        /// <summary>
        /// 公司文档移动（含批量）
        /// </summary>
        /// <param name="documentIds">要移动的文件</param>
        /// <param name="folder">移动到的目标文件夹</param>
        /// <param name="userId">用户Id</param>
        public void MoveDocumentWithoutAuth(int[] documentIds, int? folder, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length > 0)
                {
                    foreach (var item in documentIds)
                    {
                        var docModel = db.tblCompanyDocument.Where(p => p.documentId == item).FirstOrDefault();

                        if (docModel != null)
                        {
                            docModel.folder = folder;
                            docModel.updateUser = userId;
                            docModel.updateTime = DateTime.Now;
                            AddCompanyDocumentLog(db, docModel.documentId, 3, string.Empty, userId);
                        }
                    }

                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 公司文档移动（含批量）
        /// </summary>
        /// <param name="documentIds">要移动的文件</param>
        /// <param name="folder">移动到的目标文件夹</param>
        /// <param name="userId">用户Id</param>
        public void MoveDocument(int[] documentIds, int? folder, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length > 0)
                {
                    foreach (var item in documentIds)
                    {
                        var doc = db.tblCompanyDocument.Where(p => p.documentId == item).FirstOrDefault();

                        if (doc == null)
                        {
                            continue;
                        }

                        if (!doc.isFolder.Value)
                        {
                            doc.folder = folder;
                            doc.updateUser = userId;
                            doc.updateTime = DateTime.Now;
                            AddCompanyDocumentLog(db, doc.documentId, 3, string.Empty, userId);
                        }
                        else
                        {
                            var dircList = new List<FileDirectoryModel>();

                            dircList.AddRange(GetDirectoryById(item, userId, db));

                            // 获取公司目录及权限
                            this.GetDirectoryList(dircList, item, userId, db);

                            foreach (var dirc in dircList)
                            {
                                if (dirc.power == 4)
                                {
                                    dirc.type = 0;
                                }
                                else
                                {
                                    dirc.type = 1;

                                    SetParentType(dircList, dirc.parent);
                                }
                            }

                            var documentId = folder;

                            foreach (var dirc in dircList)
                            {
                                var docModel = db.tblCompanyDocument.Where(p => p.documentId == dirc.id).FirstOrDefault();

                                if (docModel != null)
                                {
                                    // 父文件夹为可以移动时，子文件不用操作
                                    if (dircList.Exists(p => p.id == dirc.parent && p.type == 0))
                                    {
                                        continue;
                                    }

                                    // 当前文件夹为可以移动
                                    if (dirc.type == 0)
                                    {
                                        docModel.folder = documentId;
                                        docModel.updateUser = userId;
                                        docModel.updateTime = DateTime.Now;
                                        AddCompanyDocumentLog(db, docModel.documentId, 3, string.Empty, userId);
                                    }
                                    else
                                    {
                                        var model = GetCompanyDocument(dirc.id.Value, db);

                                        if (model == null)
                                        {
                                            continue;
                                        }

                                        if (dirc.power == 4 || IsCopy(model.documentId, dircList))
                                        {
                                            documentId = CopyToCompanyDocument(model, documentId, null, userId, db, 2);
                                        }

                                        if (dirc.power == 4)
                                        {
                                            var childList = db.tblCompanyDocument.Where(p => p.isFolder == false && p.folder == model.documentId).ToList();

                                            foreach (var child in childList)
                                            {
                                                child.folder = documentId;
                                                child.updateUser = userId;
                                                child.updateTime = DateTime.Now;
                                                AddCompanyDocumentLog(db, child.documentId, 3, string.Empty, userId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                }
            }
        }

        #endregion 公司文档移动（含批量）

        #region 个人文档移动（含批量）

        /// <summary>
        /// 个人文档移动（含批量）
        /// </summary>
        /// <param name="documentIds">要移动的文件</param>
        /// <param name="folder">移动到的目标文件夹</param>
        /// <param name="userId">用户Id</param>
        public void MoveUserDocument(int[] documentIds, int? folder, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length > 0)
                {
                    foreach (var item in documentIds)
                    {
                        var docModel = db.tblUserDocument.Where(p => p.documentId == item).FirstOrDefault();
                        if (docModel != null)
                        {
                            docModel.folder = folder;
                            docModel.updateUser = userId;
                            docModel.updateTime = DateTime.Now;
                            //添加日志
                            AddCompanyDocumentLog(db, docModel.documentId, 3, string.Empty, userId);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        #endregion 个人文档移动（含批量）

        #region 文档删除（含批量）

        /// <summary>
        /// 公司文档删除（含批量）
        /// </summary>
        /// <param name="documentIds">要删除的文件</param>
        /// <param name="userId">用户Id</param>
        /// <param name="flag">1、公司文档 2、用户文档</param>
        public void DeleteCompanyDocument(int[] documentIds, int userId, int flag)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length > 0)
                {
                    foreach (var item in documentIds)
                    {
                        //调用删除的存储过程
                        db.prcDocumentDelete(item, userId, DateTime.Now, flag);
                        AddCompanyDocumentLog(db, item, 3, string.Empty, userId);
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 公司文档删除（含批量）
        /// </summary>
        /// <param name="documentIds">要删除的文件</param>
        /// <param name="userId">用户Id</param>
        /// <param name="flag">1、公司文档 2、用户文档</param>
        public bool DeleteFlagDocument(int[] documentIds, int userId, int flag)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (documentIds.Length > 0)
                {
                    foreach (var item in documentIds)
                    {
                        if (flag == 1)
                        {
                            var model = db.tblCompanyDocument.Where(p => p.documentId == item && p.deleteFlag == false).FirstOrDefault();

                            if (model != null && model.isFolder == true)
                            {
                                var dircList = new List<FileDirectoryModel>();
                                // 获取公司目录及权限
                                this.GetDirectoryList(dircList, item, userId, db);

                                if (dircList.Exists(p => p.power != 4))
                                {
                                    return false;
                                }
                            }
                        }

                        //调用删除的存储过程
                        db.prcDocumentDelete(item, userId, DateTime.Now, flag);
                        AddCompanyDocumentLog(db, item, 3, string.Empty, userId);
                    }
                }
                db.SaveChanges();
            }

            return true;
        }

        #endregion 文档删除（含批量）

        #region 获取公司文档日志

        /// <summary>
        ///获取公司文档日志
        /// </summary>
        /// <param name="documentId">文档Id</param>
        /// <returns>文档日志列表</returns>
        public List<DocumentLogModel> GetCompanyDocumenLogList(int documentId)
        {
            var logList = new List<DocumentLogModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                logList = (from c in db.tblCompanyDocumentLog
                           join u in db.tblUser on c.createUser equals u.userId
                           where c.documentId == documentId
                           select new DocumentLogModel
                           {
                               logId = c.logId,
                               documentId = c.documentId,
                               type = c.type,
                               comment = c.comment,
                               createUser = c.createUser,
                               createUserName = u.userName,
                               createTime = c.createTime
                           }).ToList();
                logList.ForEach(p =>
                {
                    p.createDate = p.createTime.ToString("yyyy-MM-dd");
                    p.createHMS = p.createTime.ToString().Substring(p.createTime.ToString().IndexOf(" "));
                });
            }
            return logList;
        }

        #endregion 获取公司文档日志

        #region 递归获取组织架构信息（拼接）:"XX > XX"

        /// <summary>
        /// 递归获取组织架构信息（拼接）
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="OrganizationId">组织Id</param>
        /// <param name="orgStringList">临时存储组织列表</param>
        /// <returns>拼接后的组织信息</returns>
        public string GetOrgStringByOrgId(TargetNavigationDBEntities db, int? OrganizationId, List<string> orgStringList)
        {
            var orgString = string.Empty;
            var first = (from org in db.tblOrganization
                         where org.organizationId == OrganizationId
                         select new OrganizationInfo
                         {
                             organizationId = org.organizationId,
                             organizationName = org.organizationName,
                             parentOrganization = org.parentOrganization
                         }).FirstOrDefault<OrganizationInfo>();
            if (first != null)
            {
                orgStringList.Add(first.organizationName);
                if (first.parentOrganization == null)
                {
                    if (orgStringList.Count() > 0)
                    {
                        orgStringList.Reverse();
                        foreach (var item in orgStringList)
                        {
                            orgString = orgString + item + ">";
                        }
                        orgString = orgString.Substring(0, orgString.LastIndexOf('>'));
                    }
                    return orgString;
                }
                else
                {
                    return GetOrgStringByOrgId(db, first.parentOrganization, orgStringList);
                }
            }
            return orgString;
        }

        #endregion 递归获取组织架构信息（拼接）:"XX > XX"

        #region 获取岗位信息（拼接）

        /// <summary>
        /// 获取岗位信息（拼接）
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="stationId">职位Id</param>
        /// <returns>拼接后的岗位信息</returns>
        public string GetStationByStationId(TargetNavigationDBEntities db, int? stationId)
        {
            var station = db.tblStation.Where(p => p.stationId == stationId && !p.deleteFlag).FirstOrDefault();
            if (station == null) return string.Empty;
            var org = db.tblOrganization.Where(p => p.organizationId == station.organizationId && !p.deleteFlag).FirstOrDefault();
            if (org == null) return string.Empty;
            var stationString = org.organizationName + ">" + station.stationName;
            return stationString;
        }

        #endregion 获取岗位信息（拼接）

        #region 获取人员信息（拼接）

        /// <summary>
        /// 获取人员信息（拼接）
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="userId">用户Id</param>
        /// <param name="orgId">部门Id</param>
        /// <returns>拼接后的人员信息</returns>
        public string GetUsernByUserId(TargetNavigationDBEntities db, int userId, int orgId)
        {
            var orgModel = db.tblOrganization.Where(p => p.organizationId == orgId && !p.deleteFlag).FirstOrDefault();
            if (orgModel == null) return string.Empty;
            var user = db.tblUser.Where(p => p.userId == userId && !p.deleteFlag).FirstOrDefault();
            if (user == null) return string.Empty;
            var userString = orgModel.organizationName + " - " + user.userName;
            return userString;
        }

        #endregion 获取人员信息（拼接）

        #region 递归获取组织架构信息（拼接）:"XX - XX"

        /// <summary>
        /// 递归获取组织架构信息（拼接）
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="OrganizationId">组织Id</param>
        /// <param name="orgStringList">临时存储组织列表</param>
        /// <returns>拼接后的组织信息</returns>
        public string GetOrgStringByOrgIdNew(TargetNavigationDBEntities db, int? OrganizationId, List<string> orgStringList)
        {
            var orgString = string.Empty;
            var first = (from org in db.tblOrganization
                         where org.organizationId == OrganizationId
                         select new OrganizationInfo
                         {
                             organizationId = org.organizationId,
                             organizationName = org.organizationName,
                             parentOrganization = org.parentOrganization
                         }).FirstOrDefault<OrganizationInfo>();
            if (first != null)
            {
                orgStringList.Add(first.organizationName);
                if (first.parentOrganization == null)
                {
                    if (orgStringList.Count() > 0)
                    {
                        orgStringList.Reverse();
                        foreach (var item in orgStringList)
                        {
                            orgString = orgString + item + " - ";
                        }
                        orgString = orgString.Substring(0, orgString.LastIndexOf('-'));
                    }
                    return orgString;
                }
                else
                {
                    return GetOrgStringByOrgIdNew(db, first.parentOrganization, orgStringList);
                }
            }
            return orgString;
        }

        #endregion 递归获取组织架构信息（拼接）:"XX - XX"

        #region 根据上级组织Id获取下级的组织列表

        /// <summary>
        /// 根据上级组织Id获取下级的组织列表
        /// </summary>
        /// <param name="orgId">上级组织Id</param>
        /// <returns>下级组织列表</returns>
        public List<OrgSimpleModel> GetOrgListById(int? orgId)
        {
            var orgList = new List<OrgSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                orgList = (from o in db.tblOrganization
                           where o.parentOrganization == orgId && !o.deleteFlag
                           orderby o.orderNumber
                           select new OrgSimpleModel
                           {
                               id = o.organizationId,
                               name = o.organizationName,
                               isParent = o.withSub.Value
                           }).ToList();
            }
            return orgList;
        }

        #endregion 根据上级组织Id获取下级的组织列表

        #region 模糊查询获取组织架构列表

        /// <summary>
        /// 模糊查询获取组织架构列表
        /// </summary>
        /// <param name="text">模糊查询的字段</param>
        /// <returns>组织架构列表</returns>
        public List<OrgSimpleModel> GetOrgListByName(string text)
        {
            var orgList = new List<OrgSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                orgList = (from o in db.tblOrganization
                           where o.organizationName.IndexOf(text) >= 0 && !o.deleteFlag
                           select new OrgSimpleModel
                           {
                               id = o.organizationId,
                               name = o.organizationName,
                               isParent = o.withSub.Value
                           }).ToList();
            }
            return orgList;
        }

        #endregion 模糊查询获取组织架构列表

        #region 根据组织Id获取岗位列表(含下级)

        /// <summary>
        /// 根据组织Id获取岗位列表(含下级)
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns>岗位列表</returns>
        public List<StationModel> GetStationListByOrgId(int orgId)
        {
            var stationList = new List<StationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //获取该组织架构下面所有的组织Id
                var orgIds = GetAllorganizationIds(orgId, db);
                if (orgIds.Count() > 0)
                {
                    foreach (var item in orgIds)
                    {
                        var stationTemplist = (from s in db.tblStation
                                               join o in db.tblOrganization
                                                   on s.organizationId equals o.organizationId
                                               where s.organizationId == item
                                               select new StationModel
                                               {
                                                   organizationId = o.organizationId,
                                                   organizationName = o.organizationName,
                                                   stationId = s.stationId,
                                                   stationName = s.stationName,
                                                   approval = s.approval
                                               }).ToList();
                        if (stationTemplist.Count > 0) stationList.AddRange(stationTemplist);
                    }
                }
            }
            return stationList;
        }

        #endregion 根据组织Id获取岗位列表(含下级)

        #region 根据组织Id获取岗位列表(不含下级)

        /// <summary>
        /// 根据组织Id获取岗位列表(不含下级)
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns>岗位列表</returns>
        public List<StationModel> GetStationListByThisOrgId(int orgId)
        {
            var stationList = new List<StationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                stationList = (from s in db.tblStation
                               join o in db.tblOrganization
                                   on s.organizationId equals o.organizationId
                               where s.organizationId == orgId
                               select new StationModel
                               {
                                   organizationId = o.organizationId,
                                   organizationName = o.organizationName,
                                   stationId = s.stationId,
                                   stationName = s.stationName,
                                   approval = s.approval
                               }).ToList();
            }
            return stationList;
        }

        #endregion 根据组织Id获取岗位列表(不含下级)

        #region 根据岗位ID取得该岗位下的在职用户数

        /// <summary>
        /// 根据岗位ID取得该岗位下的在职用户数
        /// </summary>
        /// <param name="stationId">岗位ID</param>
        /// <returns>该岗位下的在职用户数</returns>
        public int GetUserNumByStationId(int stationId)
        {
            var userNum = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                userNum = (from u in db.tblUser
                           join us in db.tblUserStation
                           on u.userId equals us.userId
                           where us.stationId == stationId && !u.deleteFlag && u.workStatus == (int)ConstVar.workStatus.OnWork
                           select u.userId
                        ).Count();
            }
            return userNum;
        }

        #endregion 根据岗位ID取得该岗位下的在职用户数

        #region 模糊查询岗位列表

        /// <summary>
        /// 模糊查询岗位列表
        /// </summary>
        /// <param name="text">模糊查询的字段</param>
        /// <returns>岗位列表</returns>
        public List<StationModel> GetStationListByName(string text)
        {
            var stationList = new List<StationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                stationList = (from s in db.tblStation
                               join o in db.tblOrganization
                                   on s.organizationId equals o.organizationId
                               where s.stationName.IndexOf(text) >= 0
                               select new StationModel
                               {
                                   affiliationName = o.organizationName,
                                   id = s.stationId,
                                   name = s.stationName
                               }).ToList();
            }
            return stationList;
        }

        #endregion 模糊查询岗位列表

        #region 根据组织Id获取人员列表(含下级)

        /// <summary>
        /// 根据组织Id获取人员列表(含下级)
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns>人员列表</returns>
        public List<UserInfo> GetPersonListByOrgId(int orgId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //1、获取该组织架构下面所有的组织Id
                var orgIds = GetAllorganizationIds(orgId, db);
                if (orgIds.Count() > 0)
                {
                    foreach (var item in orgIds)
                    {
                        userList.AddRange(GetUserListByOrgId(db, item));
                    }
                }
            }
            return userList;
        }

        #endregion 根据组织Id获取人员列表(含下级)

        #region 获取该组织Id上的人员列表(不含下级)

        /// <summary>
        /// 获取该组织Id上的人员列表(不含下级)
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <returns>人员列表</returns>
        public List<UserInfo> GetPersonListByThisOrgId(int orgId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = GetUserListByOrgId(db, orgId);
            }
            return userList;
        }

        #endregion 获取该组织Id上的人员列表(不含下级)

        #region 根据组织Id获取人员列表共通方法

        /// <summary>
        /// 根据组织Id获取人员列表共通方法
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="orgId">组织Id</param>
        /// <returns>人员列表</returns>
        public List<UserInfo> GetUserListByOrgId(TargetNavigationDBEntities db, int orgId)
        {
            var userList = new List<UserInfo>();
            var orgName = db.tblOrganization.Where(p => p.organizationId == orgId).FirstOrDefault() == null ? "" : db.tblOrganization.Where(p => p.organizationId == orgId).FirstOrDefault().organizationName;
            //获取该组织架构下面的职位
            var stations = db.tblStation.Where(p => p.organizationId == orgId).ToList<tblStation>();
            if (stations.Count > 0)
            {
                foreach (var station in stations)
                {
                    //获取该职位上的人员信息
                    var userStations = db.tblUserStation.Where(p => p.stationId == station.stationId);
                    foreach (var userStation in userStations)
                    {
                        var userModel = db.tblUser.Where(p => p.userId == userStation.userId && !p.deleteFlag && p.workStatus == (int)ConstVar.workStatus.OnWork).FirstOrDefault();
                        if (userModel != null)
                        {
                            var user = new UserInfo
                            {
                                userId = userModel.userId,
                                userName = userModel.userName,
                                smallImage = userModel.smallImage,
                                organizationId = orgId,
                                organizationName = orgName
                            };

                            if (!userList.Exists(p => p.userId == user.userId))
                            {
                                userList.Add(user);
                            }
                        }
                    }
                }
            }
            return userList;
        }

        #endregion 根据组织Id获取人员列表共通方法

        #region 权限设置模糊查询人员

        /// <summary>
        /// 权限设置模糊查询人员
        /// </summary>
        /// <param name="text">模糊查询的字段</param>
        /// <returns>人员列表</returns>
        public List<UserInfo> GetUserListByName(string text)
        {
            string path = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();

            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                userList = (from u in db.tblUser
                            join us in db.tblUserStation
                                on u.userId equals us.userId
                            join s in db.tblStation
                                on us.stationId equals s.stationId
                            join o in db.tblOrganization
                             on s.organizationId equals o.organizationId
                            where u.userName.IndexOf(text) >= 0 && u.workStatus != 2 && u.workStatus != 3 && !u.deleteFlag
                            select new UserInfo
                            {
                                id = u.userId,
                                name = u.userName,
                                affiliationName = o.organizationName,
                                img = string.IsNullOrEmpty(u.originalImage) ? "/Images/common/portrait.png" : "/" + path + "/" + u.smallImage
                            }).ToList();
            }
            return userList;
        }

        #endregion 权限设置模糊查询人员

        #region 递归获取该组织架构下面的所有组织Id

        /// <summary>
        /// 递归获取该组织架构下面的所有组织Id
        /// </summary>
        /// <param name="organizationId">组织Id</param>
        /// <param name="db">数据库上下文</param>
        /// <returns>组织Id集合</returns>
        public List<int> GetAllorganizationIds(int organizationId, TargetNavigationDBEntities db)
        {
            var organizationIds = new List<int>();
            organizationIds.Add(organizationId);
            var organizationList = (from c in db.tblOrganization where c.parentOrganization == organizationId select c.organizationId).ToList();
            if (organizationList.Count() > 0)
            {
                foreach (var item in organizationList)
                {
                    organizationIds.Add(item);
                    var organizationIdsTemp = GetAllorganizationIds(item, db);
                    if (organizationIdsTemp.Count <= 0) continue;
                }
                return organizationIds;
            }
            else
            {
                return organizationIds;
            }
        }

        #endregion 递归获取该组织架构下面的所有组织Id

        #region 排序通用方法 --暂时没用到

        /// <summary>
        /// 排序通用方法
        /// </summary>
        /// <param name="docList">排序的文档列表</param>
        /// <param name="sort">排序内容</param>
        /// <returns>排序后的文档列表</returns>
        public List<DocumentModel> DocListOrderBySort(List<DocumentModel> docList, Sort sort)
        {
            if (docList.Count() > 0)
            {
                switch (sort.type)
                {
                    //默认
                    case 1:
                        docList = (sort.direct == 1 ? docList.OrderBy(p => p.displayName).OrderBy(p => p.isFolder) : docList.OrderByDescending(p => p.displayName).OrderByDescending(p => p.isFolder)).ToList();
                        break;
                    //名称
                    case 2:
                        docList = (sort.direct == 1 ? docList.OrderBy(p => p.displayName) : docList.OrderByDescending(p => p.displayName)).ToList();
                        break;
                    //创建时间
                    case 3:
                        docList = (sort.direct == 1 ? docList.OrderBy(p => p.createTime) : docList.OrderByDescending(p => p.createTime)).ToList();
                        break;
                    //类型
                    case 4:
                        docList = (sort.direct == 1 ? docList.OrderBy(p => p.isFolder) : docList.OrderByDescending(p => p.isFolder)).ToList();
                        break;
                }
            }
            return docList;
        }

        #endregion 排序通用方法 --暂时没用到

        #region 拼接排序字符串

        /// <summary>
        /// 拼接排序字符串
        /// </summary>
        /// <param name="docList">排序的文档列表</param>
        /// <param name="sort">排序内容</param>
        /// <returns>排序的字符串</returns>
        public string GetOrderStringBySort(Sort sort)
        {
            var docOrderString = string.Empty;
            if (sort != null)
            {
                switch (sort.type)
                {
                    //默认
                    case 1:
                        docOrderString = sort.direct == 0 ? " order by c.isFolder,c.displayName " : " order by c.isFolder desc,c.displayName desc ";
                        break;
                    //名称
                    case 2:
                        docOrderString = sort.direct == 0 ? " order by c.displayName " : " order by c.displayName desc ";
                        break;
                    //创建时间
                    case 3:
                        docOrderString = sort.direct == 0 ? " order by c.createTime " : " order by c.createTime desc";
                        break;
                    //类型
                    case 4:
                        docOrderString = sort.direct == 0 ? " order by isFolder" : " order by isFolder desc ";
                        break;
                }
            }
            return docOrderString;
        }

        #endregion 拼接排序字符串

        #region 获取目录取第一级或非第一级的目录查询条件 --暂时没用到

        /// <summary>
        /// 获取目录取第一级或非第一级的目录查询条件
        /// </summary>
        /// <param name="folder">上级文件夹Id</param>
        /// <returns>目录查询条件</returns>
        public string GetCondition(int folder)
        {
            var condition = string.Empty;
            //第一级目录
            if (folder == 0)
            {
                condition = "folder is null";
            }
            else
            {
                condition = "folder==" + folder;
            }
            return condition;
        }

        #endregion 获取目录取第一级或非第一级的目录查询条件 --暂时没用到

        #region 获取新闻分类列表

        /// <summary>
        /// 获取新闻分类列表
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<NewsDirectoryModel> GetNewsDirectory(int? parent)
        {
            var modelList = new List<NewsDirectoryModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                modelList = (from dirc in db.tblNewsDirectory
                             where dirc.parentDirectory == parent
                             orderby dirc.orderNum ascending
                             select new NewsDirectoryModel
                             {
                                 id = dirc.directoryId,
                                 name = dirc.directoryName
                             }).ToList();

                foreach (var item in modelList)
                {
                    var model = db.tblNewsDirectory.Where(p => p.parentDirectory == item.id).ToList().Count();

                    if (model > 0)
                    {
                        item.isParent = true;
                    }
                }
            }

            return modelList;
        }

        #endregion 获取新闻分类列表

        #region 获取通知分类列表

        /// <summary>
        /// 获取通知分类列表
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<NewsDirectoryModel> GetNoticeDirectory(int? parent)
        {
            var modelList = new List<NewsDirectoryModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                modelList = (from dirc in db.tblNoticeDirectory
                             where dirc.parentDirectory == parent
                             orderby dirc.orderNum ascending
                             select new NewsDirectoryModel
                             {
                                 id = dirc.directoryId,
                                 name = dirc.directoryName
                             }).ToList();

                foreach (var item in modelList)
                {
                    var model = db.tblNoticeDirectory.Where(p => p.parentDirectory == item.id).ToList().Count();

                    if (model > 0)
                    {
                        item.isParent = true;
                    }
                }
            }

            return modelList;
        }

        #endregion 获取通知分类列表

        #region 获取文件夹下的所有文档

        /// <summary>
        /// 获取文件夹下的所有文档
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <returns>该文件夹下的所有文档</returns>
        public List<DocumentModel> GetAllDocumentInFolder(int folderId, int userId, List<DocumentModel> docList, ref string folderPath)
        {
            string folder = folderPath;
            var childList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                childList = (from c in db.tblCompanyDocument
                             where c.folder == folderId && !c.deleteFlag
                             select new DocumentModel
                             {
                                 documentId = c.documentId,
                                 displayName = c.displayName,
                                 saveName = c.saveName,
                                 isFolder = c.isFolder,
                                 path = folder + "\\"
                             }).ToList();
                if (childList.Count > 0)
                {
                    foreach (var item in childList)
                    {
                        folderPath = folder;

                        if (item.isFolder.Value)
                        {
                            var model = GetDirectoryById(item.documentId, userId, db).FirstOrDefault();

                            if (model == null || model.power == 1)
                            {
                                continue;
                            }

                            folderPath = folderPath + "\\" + item.displayName;
                            var childTemp = GetAllDocumentInFolder(item.documentId, userId, docList, ref folderPath);
                            if (childTemp.Count <= 0) continue;
                        }
                        else
                        {
                            //服务器上文档地址
                            var path = Path.Combine(FilePath.DocumentUpLoadPath, item.saveName);
                            if (File.Exists(path))
                            {
                                docList.Add(item);
                                continue;
                            }
                        }
                    }
                    return docList;
                }
                else
                {
                    return docList;
                }
            }
        }

        /// <summary>
        /// 获取文件夹下的所有文档
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <returns>该文件夹下的所有文档</returns>
        public List<DocumentModel> GetAllDocumentInFolder(int folderId, List<DocumentModel> docList, ref string folderPath)
        {
            string folder = folderPath;
            var childList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                childList = (from c in db.tblCompanyDocument
                             where c.folder == folderId && !c.deleteFlag
                             select new DocumentModel
                             {
                                 documentId = c.documentId,
                                 displayName = c.displayName,
                                 saveName = c.saveName,
                                 isFolder = c.isFolder,
                                 path = folder + "\\"
                             }).ToList();
                if (childList.Count > 0)
                {
                    foreach (var item in childList)
                    {
                        folderPath = folder;
                        if (item.isFolder.Value)
                        {
                            folderPath = folderPath + "\\" + item.displayName;
                            var childTemp = GetAllDocumentInFolder(item.documentId, docList, ref folderPath);
                            if (childTemp.Count <= 0) continue;
                        }
                        else
                        {
                            //服务器上文档地址
                            var path = Path.Combine(FilePath.DocumentUpLoadPath, item.saveName);
                            if (File.Exists(path))
                            {
                                docList.Add(item);
                                continue;
                            }
                        }
                    }
                    return docList;
                }
                else
                {
                    return docList;
                }
            }
        }

        public List<DocumentModel> GetUserDocumentInFolder(int folderId, List<DocumentModel> docList, ref string folderPath)
        {
            string folder = folderPath;
            var childList = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                childList = (from c in db.tblUserDocument
                             where c.folder == folderId && c.deleteFlag == false
                             select new DocumentModel
                             {
                                 documentId = c.documentId,
                                 displayName = c.displayName,
                                 saveName = c.saveName,
                                 isFolder = c.isFolder,
                                 path = folder + "\\"
                             }).ToList();
                if (childList.Count > 0)
                {
                    foreach (var item in childList)
                    {
                        folderPath = folder;
                        if (item.isFolder.Value)
                        {
                            folderPath = folderPath + "\\" + item.displayName;
                            var childTemp = GetUserDocumentInFolder(item.documentId, docList, ref folderPath);
                            if (childTemp.Count <= 0) continue;
                        }
                        else
                        {
                            //服务器上文档地址
                            var path = Path.Combine(FilePath.MineUpLoadPath, item.saveName);
                            if (File.Exists(path))
                            {
                                docList.Add(item);
                                continue;
                            }
                        }
                    }
                    return docList;
                }
                else
                {
                    return docList;
                }
            }
        }

        #endregion 获取文件夹下的所有文档

        #region 判断是否是系统管理员

        /// <summary>
        /// 判断是否是系统管理员
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public bool GetAdmin(int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var user = (from u in db.tblUser
                            where u.userId == userId
                            select u.admin).FirstOrDefault();
                return user == null ? false : user.Value;
            }
        }

        #endregion 判断是否是系统管理员

        #region 返回所有的该文件夹所有的上级文件夹列表

        /// <summary>
        /// 返回所有的该文件夹所有的上级文件夹列表
        /// </summary>
        /// <param name="documentId">文档Id</param>
        /// <returns>所有上级文件夹列表</returns>
        public List<FileDirectoryModel> getAllParentIds(int documentId, List<FileDirectoryModel> docList)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var docModel = db.tblCompanyDocument.Where(p => p.documentId == documentId).FirstOrDefault();
                if (docModel != null)
                {
                    docList.Add(new FileDirectoryModel
                    {
                        id = docModel.documentId,
                        name = docModel.displayName
                    });
                    if (docModel.folder != null)
                    {
                        return getAllParentIds(docModel.folder.Value, docList);
                    }
                    else
                    {
                        return docList;
                    }
                }
                else
                {
                    return docList;
                }
            }
        }

        public List<FileDirectoryModel> getAllParentIds(int documentId, List<FileDirectoryModel> docList, int type)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (type == 1)
                {
                    var docModel = db.tblCompanyDocument.Where(p => p.documentId == documentId).FirstOrDefault();

                    if (docModel != null)
                    {
                        docList.Add(new FileDirectoryModel
                        {
                            id = docModel.documentId,
                            name = docModel.displayName
                        });
                        if (docModel.folder != null)
                        {
                            return getAllParentIds(docModel.folder.Value, docList, type);
                        }
                        else
                        {
                            return docList;
                        }
                    }
                    else
                    {
                        return docList;
                    }
                }
                else
                {
                    var docModel = db.tblUserDocument.Where(p => p.documentId == documentId).FirstOrDefault();
                    if (docModel != null)
                    {
                        docList.Add(new FileDirectoryModel
                        {
                            id = docModel.documentId,
                            name = docModel.displayName
                        });
                        if (docModel.folder != null)
                        {
                            return getAllParentIds(docModel.folder.Value, docList, type);
                        }
                        else
                        {
                            return docList;
                        }
                    }
                    else
                    {
                        return docList;
                    }
                }
            }
        }

        #endregion 返回所有的该文件夹所有的上级文件夹列表,

        #region 删除图片库图片

        /// <summary>
        /// 删除图片库图片
        /// </summary>
        /// <param name="imageId"></param>
        public void DeleteImage(int imageId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblImage.Where(p => p.imageId == imageId).FirstOrDefault();

                if (model != null)
                {
                    db.tblImage.Remove(model);
                }

                db.SaveChanges();
            }
        }

        #endregion 删除图片库图片

        #region 私有方法

        /// <summary>
        /// 取得公司文档信息
        /// </summary>
        /// <param name="documentId">文档ID</param>
        /// <param name="db">DB</param>
        /// <returns>公司文档信息</returns>
        private DocumentModel GetCompanyDocument(int documentId, TargetNavigationDBEntities db)
        {
            var docModel = (from c in db.tblCompanyDocument
                            where c.documentId == documentId && c.deleteFlag == false
                            select new DocumentModel
                            {
                                documentId = c.documentId,
                                folder = c.folder,
                                displayName = c.displayName,
                                description = c.description,
                                saveName = c.saveName,
                                extension = c.extension,
                                archive = c.archive,
                                archiveTime = c.archiveTime,
                                isFolder = c.isFolder,
                                deleteFlag = c.deleteFlag
                            }).FirstOrDefault();

            return docModel;
        }

        /// <summary>
        /// 取得用户文档信息
        /// </summary>
        /// <param name="documentId">文档ID</param>
        /// <param name="db">DB</param>
        /// <returns>公司文档信息</returns>
        private DocumentModel GetUserDocument(int documentId, TargetNavigationDBEntities db)
        {
            var docModel = (from c in db.tblUserDocument
                            where c.documentId == documentId && c.deleteFlag == false
                            select new DocumentModel
                            {
                                documentId = c.documentId,
                                folder = c.folder,
                                displayName = c.displayName,
                                description = c.description,
                                saveName = c.saveName,
                                extension = c.extension,
                                withShared = c.withShared,
                                isFolder = c.isFolder,
                                deleteFlag = c.deleteFlag
                            }).FirstOrDefault();

            return docModel;
        }

        /// <summary>
        /// 取得公司文档子文件
        /// </summary>
        /// <param name="parent">上级文件夹ID</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        private List<DocumentModel> GetCompanyChildDocument(int parent, TargetNavigationDBEntities db)
        {
            var list = (from c in db.tblCompanyDocument
                        where c.folder == parent && c.deleteFlag == false
                        select new DocumentModel
                        {
                            documentId = c.documentId,
                            folder = c.folder,
                            displayName = c.displayName,
                            description = c.description,
                            saveName = c.saveName,
                            extension = c.extension,
                            archive = c.archive,
                            archiveTime = c.archiveTime,
                            isFolder = c.isFolder,
                            deleteFlag = c.deleteFlag
                        }).ToList();

            return list;
        }

        /// <summary>
        /// 取得个人文档子文件
        /// </summary>
        /// <param name="parent">上级文件夹ID</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        private List<DocumentModel> GetUserChildDocument(int parent, TargetNavigationDBEntities db)
        {
            var list = (from c in db.tblUserDocument
                        where c.folder == parent && c.deleteFlag == false
                        select new DocumentModel
                        {
                            documentId = c.documentId,
                            folder = c.folder,
                            displayName = c.displayName,
                            description = c.description,
                            saveName = c.saveName,
                            extension = c.extension,
                            isFolder = c.isFolder,
                            deleteFlag = c.deleteFlag
                        }).ToList();

            return list;
        }

        /// <summary>
        /// 复制文件到公司文档
        /// </summary>
        /// <param name="model">文件信息</param>
        /// <param name="targetId">目标文件夹</param>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="db">数据库</param>
        /// <param name="flag">1、复制个人文档 2、复制公司文档</param>
        /// <returns></returns>
        private int CopyToCompanyDocument(DocumentModel model, int? targetId, int? newTarget, int userId, TargetNavigationDBEntities db, int flag)
        {
            var documentId = 0;
            var filePath = flag == 2 ? FilePath.DocumentUpLoadPath : FilePath.MineUpLoadPath;

            if (model.isFolder == true)
            {
                //新建复制的文件
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;

                var newDocModel = new tblCompanyDocument
                {
                    documentId = documentId,
                    folder = targetId,
                    displayName = model.displayName,
                    description = model.description,
                    isFolder = model.isFolder,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblCompanyDocument.Add(newDocModel);
                if (flag == 2) //公司文档复制到公司文档，权限复制
                {
                    CopyFileAuth(db, documentId, model.documentId);
                }
                else if (flag == 1)//个人文档复制到公司文档，权限复制
                {
                    if (newTarget != null)
                    {
                        CopyFileAuth(db, documentId, newTarget);
                    }
                    else
                    {
                        CopyFileAuth(db, documentId, targetId);
                    }
                }
            }
            else
            {
                //服务器上文档地址
                var path = Path.Combine(filePath, model.saveName);

                if (!File.Exists(path))
                {
                    return -1;
                }

                //生成存储名
                int numName = StringUtils.GetRandom();
                var saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
                //新建复制的文件
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                documentId = db.prcGetPrimaryKey("tblCompanyDocument", obj).FirstOrDefault().Value;

                var newDocModel = new tblCompanyDocument
                {
                    documentId = documentId,
                    folder = targetId,
                    displayName = model.displayName,
                    description = model.description,
                    saveName = saveName,
                    extension = model.extension,
                    isFolder = model.isFolder,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblCompanyDocument.Add(newDocModel);

                var newPath = Path.Combine(FilePath.DocumentUpLoadPath, newDocModel.saveName);
                // 服务器上复制文档
                File.Copy(path, newPath, true);
            }

            return documentId;
        }

        /// <summary>
        /// 复制文件到个人文档
        /// </summary>
        /// <param name="model">文件信息</param>
        /// <param name="targetId">目标文件夹</param>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="db">数据库</param>
        /// <returns></returns>
        private int CopyToUserDocument(DocumentModel model, int targetId, int userId, TargetNavigationDBEntities db, int flag)
        {
            var filePath = flag == 2 ? FilePath.DocumentUpLoadPath : FilePath.MineUpLoadPath;

            var documentId = 0;

            if (model.isFolder == true)
            {
                //新建复制的文件
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                documentId = db.prcGetPrimaryKey("tblUserDocument", obj).FirstOrDefault().Value;

                var newDocModel = new tblUserDocument
                {
                    documentId = documentId,
                    folder = targetId,
                    displayName = model.displayName,
                    description = model.description,
                    isFolder = model.isFolder,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblUserDocument.Add(newDocModel);
            }
            else
            {
                //服务器上文档地址
                var path = Path.Combine(filePath, model.saveName);

                if (!File.Exists(path))
                {
                    return -1;
                }

                //生成存储名
                int numName = StringUtils.GetRandom();
                var saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
                //新建复制的文件
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                documentId = db.prcGetPrimaryKey("tblUserDocument", obj).FirstOrDefault().Value;

                var newDocModel = new tblUserDocument
                {
                    documentId = documentId,
                    folder = targetId,
                    displayName = model.displayName,
                    description = model.description,
                    saveName = saveName,
                    extension = model.extension,
                    isFolder = model.isFolder,
                    createUser = userId,
                    createTime = DateTime.Now,
                    updateUser = userId,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblUserDocument.Add(newDocModel);

                var newPath = Path.Combine(FilePath.MineUpLoadPath, newDocModel.saveName);
                // 服务器上复制文档
                File.Copy(path, newPath, true);
            }

            return documentId;
        }

        /// <summary>
        /// 复制子文件到目标文件夹
        /// </summary>
        /// <param name="parent">上级文件夹</param>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="db">DB</param>
        private void CopyCompanyChildToCompanyDocument(int parent, int targetId, int userId, bool withAuth, TargetNavigationDBEntities db)
        {
            // 取得子文件列表
            var list = this.GetCompanyChildDocument(parent, db);

            if (list.Count == 0)
            {
                return;
            }

            foreach (var item in list)
            {
                if (item.isFolder == true)
                {
                    if (withAuth)
                    {
                        var model = GetDirectoryById(item.documentId, userId, db).FirstOrDefault();

                        if (model == null || model.power == 1)
                        {
                            continue;
                        }
                    }

                    // 复制文件到目标文件夹
                    int newDocumentId = this.CopyToCompanyDocument(item, targetId, null, userId, db, 2);

                    // 添加日志
                    AddCompanyDocumentLog(db, item.documentId, 2, string.Empty, userId);

                    this.CopyCompanyChildToCompanyDocument(item.documentId, newDocumentId, userId, withAuth, db);
                }
                else
                {
                    // 复制文件到目标文件夹
                    int newDocumentId = this.CopyToCompanyDocument(item, targetId, null, userId, db, 2);
                }
            }
        }

        /// <summary>
        /// 复制子文件到目标文件夹
        /// </summary>
        /// <param name="parent">上级文件夹</param>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="db">DB</param>
        private void CopyCompanyChildToUserDocument(int parent, int targetId, int userId, TargetNavigationDBEntities db)
        {
            // 取得子文件列表
            var list = this.GetCompanyChildDocument(parent, db);

            if (list.Count == 0)
            {
                return;
            }

            foreach (var item in list)
            {
                if (item.isFolder == true)
                {
                    var model = GetDirectoryById(item.documentId, userId, db).FirstOrDefault();

                    if (model == null || model.power == 1)
                    {
                        continue;
                    }

                    // 复制文件到目标文件夹
                    int newDocumentId = this.CopyToUserDocument(item, targetId, userId, db, 2);

                    // 添加日志
                    AddCompanyDocumentLog(db, item.documentId, 2, string.Empty, userId);

                    this.CopyCompanyChildToUserDocument(item.documentId, newDocumentId, userId, db);
                }
                else
                {
                    // 复制文件到目标文件夹
                    int newDocumentId = this.CopyToUserDocument(item, targetId, userId, db, 2);
                }
            }
        }

        /// <summary>
        /// 复制子文件到目标文件夹
        /// </summary>
        /// <param name="parent">上级文件夹</param>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="db">DB</param>
        private void CopyUserChildToCompanyDocument(int parent, int targetId, int? newTarget, int userId, TargetNavigationDBEntities db)
        {
            // 取得子文件列表
            var list = this.GetUserChildDocument(parent, db);

            if (list.Count == 0)
            {
                return;
            }

            foreach (var item in list)
            {
                // 复制文件到目标文件夹
                int newDocumentId = this.CopyToCompanyDocument(item, targetId, newTarget, userId, db, 1);

                // 文件夹的场合，复制子文件夹
                if (item.isFolder == true)
                {
                    this.CopyUserChildToCompanyDocument(item.documentId, newDocumentId, newTarget, userId, db);
                }
            }
        }

        /// <summary>
        /// 复制子文件到目标文件夹
        /// </summary>
        /// <param name="parent">上级文件夹</param>
        /// <param name="userId">登陆用户ID</param>
        /// <param name="db">DB</param>
        private void CopyUserChildToUserDocument(int parent, int targetId, int userId, TargetNavigationDBEntities db)
        {
            // 取得子文件列表
            var list = this.GetUserChildDocument(parent, db);

            if (list.Count == 0)
            {
                return;
            }

            foreach (var item in list)
            {
                // 复制文件到目标文件夹
                int newDocumentId = this.CopyToUserDocument(item, targetId, userId, db, 1);

                // 文件夹的场合，复制子文件夹
                if (item.isFolder == true)
                {
                    this.CopyUserChildToUserDocument(item.documentId, newDocumentId, userId, db);
                }
            }
        }

        private void GetDirectoryList(List<FileDirectoryModel> dircList, int parent, int userId, TargetNavigationDBEntities db)
        {
            var list = GetDirectory(parent, userId, db);

            foreach (var item in list)
            {
                dircList.Add(item);

                GetDirectoryList(dircList, item.id.Value, userId, db);
            }
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="parent">上级文件夹</param>
        /// <param name="db">DB</param>
        /// <returns></returns>
        private List<FileDirectoryModel> GetDirectory(int parent, int userId, TargetNavigationDBEntities db)
        {
            var fileDirectoryList = new List<FileDirectoryModel>();
            //按人员设置权限的文档列表
            var personAuthDocuments = (from c in db.tblCompanyDocument
                                       join f in db.tblFolderAuth on c.documentId equals f.documentId
                                       join a in db.tblAuthResult on f.authId equals a.authId
                                       join u in db.tblUser on a.targetId equals u.userId
                                       where c.folder == parent && c.isFolder.Value && !c.deleteFlag && f.type == 3 && a.targetId == userId
                                       select new FileDirectoryModel
                                       {
                                           id = c.documentId,
                                           name = c.displayName,
                                           power = f.power,
                                           type = 1,
                                           parent = c.folder.Value
                                       }).ToList();

            fileDirectoryList.AddRange(personAuthDocuments);

            //按岗位设置权限的文档列表
            var stationAuthDocuments = (from c in db.tblCompanyDocument
                                        join f in db.tblFolderAuth on c.documentId equals f.documentId
                                        join a in db.tblAuthResult on f.authId equals a.authId
                                        join u in db.tblUserStation on a.targetId equals u.stationId
                                        where c.folder == parent && c.isFolder.Value && !c.deleteFlag && f.type == 2 && u.userId == userId
                                        select new FileDirectoryModel
                                        {
                                            id = c.documentId,
                                            name = c.displayName,
                                            power = f.power,
                                            type = 2,
                                            parent = c.folder.Value
                                        }).ToList();

            foreach (var item in stationAuthDocuments)
            {
                var model = fileDirectoryList.Where(p => p.id == item.id).FirstOrDefault();

                if (model == null)
                {
                    fileDirectoryList.Add(item);
                }
                else if (model.type == 2 && model.power < item.power)
                {
                    model.power = item.power;
                }
            }

            //按组织架构设置的文档列表
            var orgAuthDocuments = (from c in db.tblCompanyDocument
                                    join f in db.tblFolderAuth on c.documentId equals f.documentId
                                    join a in db.tblAuthResult on f.authId equals a.authId
                                    join s in db.tblStation on a.targetId equals s.organizationId
                                    join u in db.tblUserStation on s.stationId equals u.stationId
                                    where c.folder == parent && c.isFolder.Value && !c.deleteFlag && f.type == 1 && u.userId == userId
                                    select new FileDirectoryModel
                                    {
                                        id = c.documentId,
                                        name = c.displayName,
                                        power = f.power,
                                        type = 3,
                                        parent = c.folder.Value
                                    }).ToList();

            foreach (var item in orgAuthDocuments)
            {
                var model = fileDirectoryList.Where(p => p.id == item.id).FirstOrDefault();

                if (model == null)
                {
                    fileDirectoryList.Add(item);
                }
                else if (model.type == 3 && model.power < item.power)
                {
                    model.power = item.power;
                }
            }
            return fileDirectoryList;
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="documentId">文件夹</param>
        /// <param name="db">DB</param>
        /// <returns></returns>
        private List<FileDirectoryModel> GetDirectoryById(int documentId, int userId, TargetNavigationDBEntities db)
        {
            var fileDirectoryList = new List<FileDirectoryModel>();
            //按人员设置权限的文档列表
            var personAuthDocuments = (from c in db.tblCompanyDocument
                                       join f in db.tblFolderAuth on c.documentId equals f.documentId
                                       join a in db.tblAuthResult on f.authId equals a.authId
                                       join u in db.tblUser on a.targetId equals u.userId
                                       where c.documentId == documentId && c.isFolder.Value && !c.deleteFlag && f.type == 3 && a.targetId == userId
                                       select new FileDirectoryModel
                                       {
                                           id = c.documentId,
                                           name = c.displayName,
                                           power = f.power,
                                           type = 1,
                                           parent = 0
                                       }).ToList();

            fileDirectoryList.AddRange(personAuthDocuments);

            //按岗位设置权限的文档列表
            var stationAuthDocuments = (from c in db.tblCompanyDocument
                                        join f in db.tblFolderAuth on c.documentId equals f.documentId
                                        join a in db.tblAuthResult on f.authId equals a.authId
                                        join u in db.tblUserStation on a.targetId equals u.stationId
                                        where c.documentId == documentId && c.isFolder.Value && !c.deleteFlag && f.type == 2 && u.userId == userId
                                        select new FileDirectoryModel
                                        {
                                            id = c.documentId,
                                            name = c.displayName,
                                            power = f.power,
                                            type = 2,
                                            parent = 0
                                        }).ToList();

            foreach (var item in stationAuthDocuments)
            {
                var model = fileDirectoryList.Where(p => p.id == item.id).FirstOrDefault();

                if (model == null)
                {
                    fileDirectoryList.Add(item);
                }
                else if (model.type == 2 && model.power < item.power)
                {
                    model.power = item.power;
                }
            }

            //按组织架构设置的文档列表
            var orgAuthDocuments = (from c in db.tblCompanyDocument
                                    join f in db.tblFolderAuth on c.documentId equals f.documentId
                                    join a in db.tblAuthResult on f.authId equals a.authId
                                    join s in db.tblStation on a.targetId equals s.organizationId
                                    join u in db.tblUserStation on s.stationId equals u.stationId
                                    where c.documentId == documentId && c.isFolder.Value && !c.deleteFlag && f.type == 1 && u.userId == userId
                                    select new FileDirectoryModel
                                    {
                                        id = c.documentId,
                                        name = c.displayName,
                                        power = f.power,
                                        type = 3,
                                        parent = 0
                                    }).ToList();

            foreach (var item in orgAuthDocuments)
            {
                var model = fileDirectoryList.Where(p => p.id == item.id).FirstOrDefault();

                if (model == null)
                {
                    fileDirectoryList.Add(item);
                }
                else if (model.type == 3 && model.power < item.power)
                {
                    model.power = item.power;
                }
            }
            return fileDirectoryList;
        }

        private void SetParentType(List<FileDirectoryModel> dircList, int parent)
        {
            var list = dircList.Where(p => p.id == parent).ToList();

            foreach (var item in list)
            {
                item.type = 1;

                SetParentType(dircList, item.parent);
            }
        }

        private bool IsCopy(int parent, List<FileDirectoryModel> list)
        {
            var childList = list.Where(p => p.parent == parent).ToList();

            if (childList.Count() == 0)
            {
                return false;
            }

            foreach (var item in childList)
            {
                if (item.power == 4)
                {
                    return true;
                }

                return IsCopy(item.id.Value, list);
            }

            return false;
        }

        #endregion 私有方法
    }
}