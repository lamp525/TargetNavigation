using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface ITemplateBLL
    {
        /// 获取系统表单
        List<TemplateManageModel> GetTemplateList(int? categoryid, bool system);

        /// 获取表单分类
        List<TemplateCategoryModel> GetTemplateCategoryList(bool system);

        bool CopyTemplate(int tempId, int[] categoryid, bool system);

        /// 表单分类模糊查询
        List<TemplateCategoryModel> GetSelectCategoryList(string text);

        /// 新建表单
        int AddTemplate(TemplateManageModel model);

        //表单删除
        int DeleteTem(int[] temId);

        //表单移动
        bool MoveTem(int[] tempIds, int tocaregoryId);

        bool StopOrStartTem(int tempId, int flag);
    }
}