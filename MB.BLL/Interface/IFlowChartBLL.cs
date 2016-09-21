using MB.Model;

namespace MB.BLL
{
    public interface IFlowChartBLL
    {
        /// 模版流程图设置
        TemplateFlowChartModel SetTemplateFlowChart(int templateId);

        /// 表单流程图设置
        FormFlowChartModel SetFormFlowChart(int formId);
    }
}