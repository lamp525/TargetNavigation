using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;


namespace MB.BLL.Test
{
    [TestClass]
    public class FlowChartBLLTest
    {
        MB.BLL.FlowChartBLL flowChartBLL = new FlowChartBLL();


        #region 模版流程图设置 ( SetFlowChart )
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void SetTemplateFlowChart_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowChartBLLTest.xlsx", "");

            var templateId = 999;

            var chartInfo = new TemplateFlowChartModel();

            chartInfo = flowChartBLL.SetTemplateFlowChart(templateId);

           

        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void SetTemplateFlowChart_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowChartBLLTest.xlsx", "");

            var templateId = 1;
            var chartInfo = new TemplateFlowChartModel();

            chartInfo = flowChartBLL.SetTemplateFlowChart(templateId);

          
        }

        #endregion

        #region 表单流程图设置

        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void SetFormFlowChart_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowChartBLLTest.xlsx", "");

            //var formId = 999;
            //var chartInfo = new FlowChartModel();

            //chartInfo = flowChartBLL.SetFormFlowChart(formId);

            //Assert.AreEqual(chartInfo.formId, 0);
            //Assert.IsNull(chartInfo.formTitle);
            //Assert.IsNull(chartInfo.createUserInfo);
            //Assert.IsNull(chartInfo.operateInfoList);
            //Assert.IsNull(chartInfo.nodeOperateList);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void SetFormFlowChart_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FlowChartBLLTest.xlsx", "");

            //var formId = 1;
            //var chartInfo = new FlowChartModel();
        
            //chartInfo = flowChartBLL.SetFormFlowChart(formId);
        }

        #endregion
    }
}
