using System;
using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface IUserDocumentBLL
    {
        /// 获取公司文档列表（含权限）
        List<DocumentModel> GetCompanyDocumentList(string condition, Sort sort, int userId);

        /// 获取个人文档列表
        List<DocumentModel> GetUserDocumentList(string condition, int type, Sort sort, int userId);

        /// 获取我的共享文档列表
        List<DocumentModel> GetMySharedDocumentList(string condition, DateTime start, DateTime end, Sort sort, int userId);

        /// 获取他人共享文档列表
        List<DocumentModel> GetOtherSharedDocumentList(string condition, DateTime start, DateTime end, Sort sort, int userId);

        /// 获取文档统计信息（饼图）
        List<DocumentStatisticsModel> GetDocumentStatisticsInfo(int userId);

        /// 新建用户文档（数据库操作，不包含上传动作）
        bool InsertUserDocument(UserDocument ud);

        /// 删除服务器上刚上传的文档
        void DeleteFile(string saveName, int type);
    }
}