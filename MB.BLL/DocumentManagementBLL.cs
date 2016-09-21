using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class DocumentManagementBLL : IDocumentManagementBLL
    {
        #region 变量区域

        private SharedBLL shareBll = new SharedBLL();

        #endregion 变量区域

        #region 获取公司文档列表

        /// <summary>
        /// 获取公司文档列表
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="sort">排序</param>
        /// <returns>公司文档列表</returns>
        public List<DocumentModel> GetCompanyDocumentList(string condition, Sort sort)
        {
            var docList = new List<NewDocumentModel>();
            var docListNew = new List<DocumentModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //执行存储过程筛选公司文档列表
                docList = db.prcGetCompanyListNoAuthy(condition, shareBll.GetOrderStringBySort(sort)).ToList();
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
                    saveName = p.saveName,
                    extension = p.extension,
                    createUser = p.createUser,
                    createUserName = p.createUserName,
                    createTime = p.createTime
                }));
            }
            return docListNew;
        }

        #endregion 获取公司文档列表

        #region 获取权限列表

        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="documentId">文档Id</param>
        /// <returns>权限模型</returns>
        public AuthorityInfoModel GetAuthorityList(int documentId)
        {
            var authorityModel = new AuthorityInfoModel();
            var stringList = new List<string>();
            using (var db = new TargetNavigationDBEntities())
            {
                //1、获取文件夹信息
                var document = db.tblCompanyDocument.Where(p => p.documentId == documentId && !p.deleteFlag).FirstOrDefault();
                if (document != null)
                {
                    authorityModel.documentId = documentId;
                    authorityModel.displayName = document.displayName;
                    authorityModel.description = document.description;
                    //2、获取公司文档权限信息
                    var Authority = db.tblFolderAuth.Where(p => p.documentId == documentId).ToList<tblFolderAuth>();
                    if (Authority.Count() > 0)
                    {
                        authorityModel.AuthorityList = new List<AuthorityModel>();
                        for (int i = 0; i < Authority.Count(); i++)
                        {
                            var authority = new AuthorityModel();
                            authority.authId = Authority[i].authId;
                            authority.type = Authority[i].type;
                            authority.power = Authority[i].power;
                            authority.powerName = authority.power == 1 ? "禁止访问" : (authority.power == 2 ? "仅下载" : (authority.power == 3 ? "下载和上传" : "完全控制"));
                            //3、获取权限结果信息
                            var AuthorityResult = db.tblAuthResult.Where(p => p.authId == authority.authId).ToList();
                            if (AuthorityResult.Count() > 0)
                            {
                                authority.resultId = new int[AuthorityResult.Count()];
                                authority.targetId = new int?[AuthorityResult.Count()];
                                authority.targetName = new string[AuthorityResult.Count()];
                                for (int j = 0; j < AuthorityResult.Count(); j++)
                                {
                                    authority.resultId[j] = AuthorityResult[j].resultId;
                                    authority.targetId[j] = AuthorityResult[j].targetId;
                                    //组织架构
                                    if (authority.type == 1)
                                    {
                                        stringList.Clear();
                                        authority.targetName[j] = shareBll.GetOrgStringByOrgId(db, AuthorityResult[j].targetId, stringList);
                                    }
                                    //岗位
                                    else if (authority.type == 2)
                                    {
                                        stringList.Clear();
                                        authority.targetName[j] = shareBll.GetStationByStationId(db, AuthorityResult[j].targetId);
                                    }
                                    //人
                                    else if (authority.type == 3)
                                    {
                                        var userId = AuthorityResult[j].targetId;
                                        authority.targetName[j] = db.tblUser.Where(p => p.userId == userId).Count() <= 0 ? "无" : db.tblUser.Where(p => p.userId == userId).FirstOrDefault().userName;
                                    }
                                }
                            }
                            authorityModel.AuthorityList.Add(authority);
                        }
                    }
                }
            }
            return authorityModel;
        }

        #endregion 获取权限列表

        #region 设置权限

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="deleteAuthorityIds">要删除的权限Id集合</param>
        /// <param name="authorityInfo">新增的权限信息</param>
        /// <param name="userId">用户Id</param>
        public void SetAuthority(int[] deleteAuthorityIds, AuthorityInfoModel authorityInfo, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //1、更新文件夹名称及描述
                var firstData = db.tblCompanyDocument.Where(p => p.documentId == authorityInfo.documentId).FirstOrDefault();
                if (firstData != null)
                {
                    firstData.displayName = authorityInfo.displayName;
                    firstData.description = authorityInfo.description;
                }
                //2、删除已经去除的权限
                foreach (var item in deleteAuthorityIds)
                {
                    var resultModel = db.tblAuthResult.Where(p => p.resultId == item).FirstOrDefault();
                    if (resultModel != null)
                    {
                        db.tblAuthResult.Remove(resultModel);
                    }

                    //var authorityResults = db.tblAuthResult.Where(p => p.authId == item);
                    //foreach (var authority in authorityResults)
                    //{
                    //    db.tblAuthResult.Remove(authority);
                    //}
                }
                //3、新增刚添加的权限设置
                foreach (var item in authorityInfo.AuthorityList)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var authId = db.prcGetPrimaryKey("tblFolderAuth", obj).FirstOrDefault().Value;
                    var authModel = new tblFolderAuth
                    {
                        authId = authId,
                        documentId = authorityInfo.documentId,
                        type = item.type,
                        power = item.power
                    };
                    db.tblFolderAuth.Add(authModel);
                    for (int i = 0; i < item.targetId.Length; i++)
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter objResult = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                        var authResultId = db.prcGetPrimaryKey("tblAuthResult", objResult).FirstOrDefault().Value;
                        var authResultModel = new tblAuthResult
                        {
                            resultId = authResultId,
                            authId = authId,
                            targetId = item.targetId[i]
                        };
                        db.tblAuthResult.Add(authResultModel);
                    }
                }

                //4、记录日志
                shareBll.AddCompanyDocumentLog(db, authorityInfo.documentId, 6, string.Empty, userId);
                db.SaveChanges();
            }
        }

        #endregion 设置权限

        #region 拼接部门信息

        /// <summary>
        /// 拼接部门信息
        /// </summary>
        /// <param name="orgIds">部门Id集合</param>
        /// <returns>部门名称集合</returns>
        public string[] GetOrgInfoById(int[] orgIds)
        {
            var orgNames = new string[orgIds.Length];
            using (var db = new TargetNavigationDBEntities())
            {
                for (int i = 0; i < orgIds.Length; i++)
                {
                    orgNames[i] = shareBll.GetOrgStringByOrgId(db, orgIds[i], new List<string>());
                }
            }
            return orgNames;
        }

        #endregion 拼接部门信息

        #region 拼接岗位信息

        /// <summary>
        /// 拼接岗位信息
        /// </summary>
        /// <param name="stationIds">岗位Id集合</param>
        /// <returns>岗位名称的集合</returns>
        public string[] GetStationInfoById(int[] stationIds)
        {
            var stationNames = new string[stationIds.Length];
            using (var db = new TargetNavigationDBEntities())
            {
                for (int i = 0; i < stationIds.Length; i++)
                {
                    stationNames[i] = shareBll.GetStationByStationId(db, stationIds[i]);
                }
            }
            return stationNames;
        }

        #endregion 拼接岗位信息
    }
}