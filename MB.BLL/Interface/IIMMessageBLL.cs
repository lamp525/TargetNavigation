using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IIMMessageBLL
    {
        ///获取常用联系人列表
        List<ImContactModel> GetImContactList(int userId);

        /// 添加常用联系人
        void AddImContact(int userId, int[] contactsId);

        /// 删除常用联系人
        void DeleteImContact(int userId, int contactsId);

        /// 获取群组列表
        List<ImGroupModel> GetImGroupList(int userId);

        /// 获取群组成员
        List<ImContactModel> GetImGroupUser(int groupId);

        /// 添加新的群组
        void AddImGroup(ImGroupModel model, int userId);

        /// 添加群组成员
        void AddImGroupUser(int groupId, int[] groupUserId);

        /// 退出群组
        void QuitGroup(int groupId, int userId);

        /// 设置群组管理员
        void SetGroupManager(int groupId, int userId, int? power);

        /// 获取最近会话列表
        List<ConversationModel> GetConversationList(int userId);

        /// 取得组织架构及其下用户信息
        List<OrgAndUserModel> GetOrgAndUserList(int? orgId);

        /// 获取历史聊天记录
        List<HistoryMessageModel> GetHistoryMessage(MessageFilterModel filter, int userId);

        /// 更新消息状态为已读
        bool UpdateMessageStatus(string id);

        /// 获取各个项目的数量
        TypeCountModel GetCountByType(int userId);

        /// <summary>
        /// 查询登录用户未读信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetMessage(int userId);
    }
}