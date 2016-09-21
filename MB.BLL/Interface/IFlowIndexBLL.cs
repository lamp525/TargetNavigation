using System;
using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IFlowIndexBLL
    {
        /// 获取模板分类列表
        List<TemplateCategoryModel> GetTemplateCategoryList();

        /// 获取模板列表
        List<TemplateSimpleModel> GetTemplateList(int categoryId, int userId);

        /// 获取模板html
        TemplateModel GetTemplateHtml(int templateId);

        /// 模板详情取得
        TemplateInfoModel GetTemplateInfoById(int templateId, int? nodeId, int? formId, int flag, int? operateStatus);

        /// 新建流程
        ReturnConfirm AddFlow(AddFormContentModel flowModel, int flag);

        /// 保存控件的值
        bool SaveControlValue(int formId, AddFormContentModel formInfo, int flag);

        /// 获取待提交或已提交的流程列表
        List<UserFormModel> GetUnSubmitFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type, int status, int admin);

        /// 获取待审批状态列表
        List<UserFormModel> GetUnCheckFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type);

        /// 获取待查阅状态列表
        List<UserFormModel> GetUnReadFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type);

        /// 获取已处理状态列表
        List<UserFormModel> GetCheckedFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type);

        /// 获取已办结状态列表
        List<UserFormModel> GetCompletedFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type, int admin);

        /// 管理员：获取流程中的列表
        List<UserFormModel> GetFlowingUserFormList(string conditionString, DateTime start, DateTime end, int? type);

        /// 获取所有状态的流程首页列表
        List<UserFormModel> GetAllFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type, int admin);

        /// 根据用户Id查询上级，自己以及下属的用户信息
        List<UserInfo> GetUserIdListByUserId(int userId);

        /// 输入任意数字进行模糊查询
        List<UserInfo> SelectUserList(string word, bool hasImage);

        /// 获取流程详情
        FormDetailModel GetFlowDetailListById(int formId, int nodeId);

        /// 退回操作
        bool TurnBack(int nodeId, int templateId, int formId, string suggest, int userId, int isEntruct);

        /// 撤回提交
        bool CancelSubmit(int templateId, int formId, int nodeId, int userId);

        /// 撤回操作
        bool BackFirstNode(int nodeId, int templateId, int formId, int userId);

        /// 表单抄送
        bool DuplicateForm(int[] duplicateId, int formId, int nodeId, int templateId, int userId);

        /// 被抄送人填写意见
        void AddContents(int formId, int nodeId, string contents, int userId);

        /// 同意操作
        bool AgreeFormFlow(int templateId, int formId, int nodeId, string suggest, int userId, int isEtruct);

        /// 提交操作
        ReturnConfirm SubmitFlow(int templateId, int formId, int nodeId, int userId);

        /// 删除操作
        bool deleteUserForm(int formId, int userId);

        /// 流程中的提交操作
        bool SubmitInFlow(int templateId, int formId, int nodeId, string suggest, int userId, int isEntruct);

        /// 获取饼图统计数量
        List<FlowProcessModel> GetFlowProcessList(int year, int month, int userId, int admin);

        /// 获取表单创建人部门信息
        List<OrganizationModel> GetorganizationList(int formId);

        /// 绑定登录用户的信息
        FlowIndexUserInfo GetUserOrganizationList(int userId);

        /// 绑定创建用户的岗位信息
        List<StationModel> GetUserStationList(int orgId, int userId);

        /// 获取表单详情
        TemplateInfoModel GetFormDetailInfo(int templateId, int formId, int nodeId);

        /// 自定义排序
        List<UserFormModel> GetFlowListOrderByCustom(List<Sort> orderList, List<UserFormModel> flowList);

        /// 获取登录用户未完成流程数量
        int GetUserUnCompleteFlowCount(int userId);

        /// 获取登录用户未完成流程列表
        List<FormSimpleModel> GetUserUnCompleteList(int userId);

        //获取计划日程化待审批的流程列表
        List<UserFormModel> GetCalendarUnCheckFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type);

        //获取计划日程化待提交流程列表
        List<UserFormModel> GetCalerdarUnSubmitFlowList(int userId, string conditionString, DateTime start, DateTime end);
    }
}