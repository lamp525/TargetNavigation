using MB.Model;

namespace MB.BLL
{
    public interface ITemplateEditBLL
    {
        /// 模板详情取得
        TemplateInfoModel GetTemplateInfoById(int templateId);

        /// 控件详情取得
        TemplateControlInfoModel GetControlInfoById(int templateId, string controlId);

        /// 创建模板
        void AddTemplateInfo(TemplateInfoModel templateInfo, int userId);

        /// 更新模板
        void UpdateTemplateInfo(TemplateInfoModel templateInfo, int userId);

        /// 验证节点设置是否正确
        bool CheckNode(int? templateId);

        /// 验证流程设置是否正确
        bool CheckFlow(int? templateId);
    }
}