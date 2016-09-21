using System.Collections.Generic;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface ISharedBLL
    {
        /// 新建文件夹（公司文档）
        void BuildNewFolder(int? beforeFolder, string folderName, string description, int userId);

        /// 新建文件夹（个人文档）
        void BuildNewUserFolder(int? beforeFolder, string folderName, string description, int userId);

        /// 新建公司文件
        int AddCompanyFile(AddNewFileModel fileModel, int userId);

        /// 新建个人文件
        int AddUserFile(AddNewFileModel fileModel, int userId);

        /// 公司文档下载添加日志
        void AddDownloadLog(int documentId, int type, string comment, int createUser);

        /// 添加公司文档操作日志
        void AddCompanyDocumentLog(TargetNavigationDBEntities db, int documentId, int type, string comment, int createUser);

        /// 获取公司文件夹目录(不含权限)
        List<FileDirectoryModel> GetFolderDirectory(int? folder, bool topItem = true);

        /// 获取公司文件夹目录(含权限);
        List<FileDirectoryModel> GetFolderDirectoryHasAuth(int? folder, int userId);

        /// 模糊查询文件夹
        List<FileDirectoryModel> GetLikeFolderList(int type, string folderName, int userId);

        /// 根据文档Id获取文档集合(公司文档)
        List<DocumentModel> GetDocumentListByIds(int[] documentIds);

        /// 根据文档Id获取文档集合(公司文档)
        List<DocumentModel> GetDocumentListByIds(int[] documentIds, int userId);

        /// 根据文档Id获取文档集合(用户文档)
        List<DocumentModel> GetUserDocumentListByIds(int[] documentIds);

        /// 获取个人文件夹目录
        List<FileDirectoryModel> GetUserFolderDirectory(int? folder, int userId);

        /// 查询最近5个上传文档的人员（公司文档）
        List<UserInfo> GetLastFiveCreateUser();

        /// 查询用户文档创建人
        List<UserInfo> GetUserDocCreateUsers(int userId);

        /// 模糊查询用户列表
        List<UserInfo> SelectUserList(string text);

        /// 模糊查询用户信息列表（流程测试画面用）
        List<UserInfo> SelectUserInfoList(string text);

        /// 公司文档复制（含批量）
        void CopyDocument(int[] documentIds, int?[] companyFolder, int?[] userFolder, int userId, bool withAuth);

        /// 个人文档复制（含批量）
        void CopyUserDocument(int[] documentIds, int?[] companyFolder, int?[] userFolder, int userId);

        /// 复制该目录下的所有文档
        bool UpdateChildDocuments(TargetNavigationDBEntities db, int oldDocumentId, int newDocumentId, int userId, int type, bool flag);

        /// 个人文档共享给他人
        bool ShareToOthers(int[] documentId, int[] userIds, int flags);

        /// 取消共享
        bool NoShareToOthers(int[] documentId);

        /// 获取文档共享人
        List<UserInfo> GetUserList(int documentId);

        /// 根据当前文档Id获取所有的子文档Id(公司文档)
        List<tblCompanyDocument> GetAllDocumentList(TargetNavigationDBEntities db, tblCompanyDocument documentModel, List<tblCompanyDocument> companyDocList);

        /// 根据当前文档Id获取所有的子文档Id(用户文档)
        List<tblUserDocument> GetAllUserDocumentList(TargetNavigationDBEntities db, tblUserDocument documentModel, List<tblUserDocument> userDocList);

        /// 公司文档移动（含批量）
        void MoveDocumentWithoutAuth(int[] documentIds, int? folder, int userId);

        /// 公司文档移动（含批量）
        void MoveDocument(int[] documentIds, int? folder, int userId);

        /// 个人文档移动（含批量）
        void MoveUserDocument(int[] documentIds, int? folder, int userId);

        /// 公司文档删除（含批量）
        void DeleteCompanyDocument(int[] documentIds, int userId, int flag);

        /// 公司文档删除（含批量）
        bool DeleteFlagDocument(int[] documentIds, int userId, int flag);

        ///获取公司文档日志
        List<DocumentLogModel> GetCompanyDocumenLogList(int documentId);

        /// 递归获取组织架构信息（拼接）
        string GetOrgStringByOrgId(TargetNavigationDBEntities db, int? OrganizationId, List<string> orgStringList);

        /// 获取岗位信息（拼接）
        string GetStationByStationId(TargetNavigationDBEntities db, int? stationId);

        /// 获取人员信息（拼接）
        string GetUsernByUserId(TargetNavigationDBEntities db, int userId, int orgId);

        /// 递归获取组织架构信息（拼接）
        string GetOrgStringByOrgIdNew(TargetNavigationDBEntities db, int? OrganizationId, List<string> orgStringList);

        /// 根据上级组织Id获取下级的组织列表
        List<OrgSimpleModel> GetOrgListById(int? orgId);

        /// 模糊查询获取组织架构列表
        List<OrgSimpleModel> GetOrgListByName(string text);

        /// 根据组织Id获取岗位列表(含下级)
        List<StationModel> GetStationListByOrgId(int orgId);

        /// 根据组织Id获取岗位列表(不含下级)
        List<StationModel> GetStationListByThisOrgId(int orgId);

        /// 根据岗位ID取得该岗位下的在职用户数
        int GetUserNumByStationId(int stationId);

        /// 模糊查询岗位列表
        List<StationModel> GetStationListByName(string text);

        /// 根据组织Id获取人员列表(含下级)
        List<UserInfo> GetPersonListByOrgId(int orgId);

        /// 获取该组织Id上的人员列表(不含下级)
        List<UserInfo> GetPersonListByThisOrgId(int orgId);

        /// 根据组织Id获取人员列表共通方法
        List<UserInfo> GetUserListByOrgId(TargetNavigationDBEntities db, int orgId);

        /// 权限设置模糊查询人员
        List<UserInfo> GetUserListByName(string text);

        /// 递归获取该组织架构下面的所有组织Id
        List<int> GetAllorganizationIds(int organizationId, TargetNavigationDBEntities db);

        /// 排序通用方法
        List<DocumentModel> DocListOrderBySort(List<DocumentModel> docList, Sort sort);

        /// 拼接排序字符串
        string GetOrderStringBySort(Sort sort);

        /// 获取目录取第一级或非第一级的目录查询条件
        string GetCondition(int folder);

        /// 获取新闻分类列表
        List<NewsDirectoryModel> GetNewsDirectory(int? parent);

        /// 获取通知分类列表
        List<NewsDirectoryModel> GetNoticeDirectory(int? parent);

        /// 获取文件夹下的所有文档
        List<DocumentModel> GetAllDocumentInFolder(int folderId, int userId, List<DocumentModel> docList, ref string folderPath);

        /// 获取文件夹下的所有文档
        List<DocumentModel> GetAllDocumentInFolder(int folderId, List<DocumentModel> docList, ref string folderPath);

        List<DocumentModel> GetUserDocumentInFolder(int folderId, List<DocumentModel> docList, ref string folderPath);

        /// 判断是否是系统管理员
        bool GetAdmin(int userId);

        /// 返回所有的该文件夹所有的上级文件夹列表
        List<FileDirectoryModel> getAllParentIds(int documentId, List<FileDirectoryModel> docList);

        List<FileDirectoryModel> getAllParentIds(int documentId, List<FileDirectoryModel> docList, int type);

        /// 删除图片库图片
        void DeleteImage(int imageId);

    }
}