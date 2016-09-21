using System.Collections.Generic;
using MB.Model;

namespace MB.BLL
{
    public interface ITemplateCategoryBLL
    {
        /// <summary>
        /// 获取所有分类列表
        /// </summary>
        List<TemplateCategoryModel> GetCategoryList();

        /// <summary>
        /// 获取分类详情
        /// </summary>
        TemplateCategoryModel GetCareoryById(int caregoryId);

        /// <summary>
        /// 新增分类
        /// </summary>
        bool AddCareoryById(TemplateCategoryModel Category);

        /// <summary>
        /// 分类修改
        /// </summary>
        bool UpdateCaregoryById(TemplateCategoryModel temCaregory);

        /// <summary>
        /// 分类删除
        /// </summary>
        int DeleteCaregory(int id);

        /// <summary>
        /// 拖动排序
        /// </summary>
        void UpdateOldNum(List<TemplateCategoryOlderModel> List);
    }
}