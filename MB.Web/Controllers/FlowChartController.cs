using MB.Web.Models;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class FlowChartController : BaseController
    {
        private IFlowChartBLL FlowChartBLL { get; set; }

      

        //
        // GET: /FlowChart/

        #region 变量区域

        /// <summary>流程图对象</summary>
        //private FlowChartBLL flowChartBLL = new FlowChartBLL();

        #endregion 变量区域

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 显示模版流程图
        /// </summary>
        /// <param name="id">模版ID</param>
        /// <returns></returns>
        public string DisplayTemplateFlowChart(int id)
        {
            //取得模版流程图信息
            var flowChartInfo = FlowChartBLL.SetTemplateFlowChart(id);

            var jsonResult = new JsonResultModel(JsonResultType.success, flowChartInfo, "正常");

            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 显示表单流程图
        /// </summary>
        /// <param name="formId">表单ID</param>
        /// <returns></returns>
        public string DisplayFormFlowChart(int formId)
        {
            //取得表单流程图信息
            var flowChartInfo = FlowChartBLL.SetFormFlowChart(formId);

            var jsonResult = new JsonResultModel(JsonResultType.success, flowChartInfo, "正常");

            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}