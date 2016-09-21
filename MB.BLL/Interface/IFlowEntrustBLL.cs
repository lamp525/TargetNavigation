using System;
using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IFlowEntrustBLL
    {
        /// 获取委托列表
        List<FlowEntrustModel> GetFlowEntrustList(string condition, int userId, DateTime start, DateTime end, DateTime stutsS, DateTime stutsE);

        /// 获取详情
        FlowEntrustModel GetFlowById(int id);

        /// 新增
        bool AddNewFlowE(FlowEntrustModel Entrust);

        /// 收回
        bool UpdateFlowE(int id, int user);

        /// 获取分类
        List<TemplateCategoryModel> GetCategoryModel();

        /// 获取表单
        List<TemplateModel> GetTemById(int id);
    }
}