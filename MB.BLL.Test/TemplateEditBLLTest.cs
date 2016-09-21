using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;

namespace MB.BLL.Test
{
    [TestClass]
    public class TemplateEditBLLTest
    {
        MB.BLL.TemplateEditBLL tempBll = new MB.BLL.TemplateEditBLL();

        #region 模板详情取得（GetTemplateInfoById）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetTemplateInfoById_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetTemplateInfoById(2);

            Assert.AreEqual(temp, null);
        }

        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetTemplateInfoById_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetTemplateInfoById(1);

            Assert.AreEqual(temp.template.templateId, 1);
            Assert.AreEqual(temp.template.templateName, "请假流程");
            Assert.AreEqual(temp.template.description, "请假");
            Assert.AreEqual(temp.template.defaultTitle, 0);
            Assert.AreEqual(temp.template.categoryId, 1);
            Assert.AreEqual(temp.template.status, 1);
            Assert.AreEqual(temp.template.contents, "abcdefg");
            Assert.AreEqual(temp.template.system, true);
        }
        #endregion

        #region 控件详情取得（GetControlInfoById）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetControlInfoById_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetControlInfoById(1, "aaa");

            Assert.AreEqual(temp, null);
        }

        /// <summary>
        /// DB中有相关数据（没有控件项目，没有明细公式）
        /// </summary>
        [TestMethod]
        public void GetControlInfoById_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetControlInfoById(1, "reason");

            Assert.AreEqual(temp.control.controlId, "reason");
            Assert.AreEqual(temp.control.parentControl, null);
            Assert.AreEqual(temp.control.controlType, 1);
            Assert.AreEqual(temp.control.require, 1);
            Assert.AreEqual(temp.control.title, "请假理由");
            Assert.AreEqual(temp.control.size, 2);
            Assert.AreEqual(temp.control.mutliSelect, null);
            Assert.AreEqual(temp.control.vertical, null);
            Assert.AreEqual(temp.control.lineType, null);
            Assert.AreEqual(temp.control.defaultRowNum, null);
            Assert.AreEqual(temp.control.columnStatistics, null);
            Assert.AreEqual(temp.control.color, null);
            Assert.AreEqual(temp.control.description, null);

            Assert.AreEqual(temp.item, null);
            Assert.AreEqual(temp.formula, null);
        }

        /// <summary>
        /// DB中有相关数据（有控件项目，没有明细公式）
        /// </summary>
        [TestMethod]
        public void GetControlInfoById_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetControlInfoById(1, "type");

            Assert.AreEqual(temp.control.controlId, "type");
            Assert.AreEqual(temp.control.parentControl, null);
            Assert.AreEqual(temp.control.controlType, 4);
            Assert.AreEqual(temp.control.require, 1);
            Assert.AreEqual(temp.control.title, "请假类型");
            Assert.AreEqual(temp.control.size, 2);
            Assert.AreEqual(temp.control.mutliSelect, 0);
            Assert.AreEqual(temp.control.vertical, 0);
            Assert.AreEqual(temp.control.lineType, null);
            Assert.AreEqual(temp.control.defaultRowNum, null);
            Assert.AreEqual(temp.control.columnStatistics, null);
            Assert.AreEqual(temp.control.color, null);
            Assert.AreEqual(temp.control.description, "请选择请假类型");

            Assert.AreEqual(temp.item.Count, 3);
            Assert.AreEqual(temp.item[0].itemId, 1);
            Assert.AreEqual(temp.item[0].controlId, "type");
            Assert.AreEqual(temp.item[0].templateId, 1);
            Assert.AreEqual(temp.item[0].itemText, "事假");
            Assert.AreEqual(temp.item[0].orderNum, 1);

            Assert.AreEqual(temp.formula, null);
        }

        /// <summary>
        /// DB中有相关数据（没有有控件项目，有明细公式）
        /// </summary>
        [TestMethod]
        public void GetControlInfoById_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetControlInfoById(1, "detail");

            Assert.AreEqual(temp.control.controlId, "detail");
            Assert.AreEqual(temp.control.parentControl, null);
            Assert.AreEqual(temp.control.controlType, 16);
            Assert.AreEqual(temp.control.require, null);
            Assert.AreEqual(temp.control.title, null);
            Assert.AreEqual(temp.control.size, null);
            Assert.AreEqual(temp.control.mutliSelect, null);
            Assert.AreEqual(temp.control.vertical, null);
            Assert.AreEqual(temp.control.lineType, null);
            Assert.AreEqual(temp.control.defaultRowNum, 3);
            Assert.AreEqual(temp.control.columnStatistics, null);
            Assert.AreEqual(temp.control.color, null);
            Assert.AreEqual(temp.control.description, "申请事项明细");

            Assert.AreEqual(temp.item, null);

            Assert.AreEqual(temp.formula.Count, 5);
            Assert.AreEqual(temp.formula[0].controlId, "total");
            Assert.AreEqual(temp.formula[0].displayText, "总价");
            Assert.AreEqual(temp.formula[1].operate, "=");
            Assert.AreEqual(temp.formula[2].controlId, "unit");
            Assert.AreEqual(temp.formula[2].displayText, "单价");
            Assert.AreEqual(temp.formula[3].operate, "*");
            Assert.AreEqual(temp.formula[4].controlId, "quantity");
            Assert.AreEqual(temp.formula[4].displayText, "数量");
        }

        /// <summary>
        /// 复选框没有控件项目
        /// </summary>
        [TestMethod]
        public void GetControlInfoById_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetControlInfoById(1, "checkbox");

            Assert.AreEqual(temp.item, null);
        }

        /// <summary>
        /// 明细列表没有公式
        /// </summary>
        [TestMethod]
        public void GetControlInfoById_Test006()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "GetInfoTest");

            var temp = tempBll.GetControlInfoById(1, "list");

            Assert.AreEqual(temp.formula, null);
        }

        #endregion

        #region 模板创建（AddTemplateInfo）
        /// <summary>
        /// 模板信息为NULL
        /// </summary>
        [TestMethod]
        public void AddTemplateInfo_Test001()
        {
            tempBll.AddTemplateInfo(null, 1);
        }

        /// <summary>
        /// 模板信息为NULL
        /// </summary>
        [TestMethod]
        public void AddTemplateInfo_Test002()
        {

            var model = new TemplateInfoModel();
            model.template = new TemplateModel
            {
                templateName = "首页模板",
                description = null,
                defaultTitle =0,
                categoryId = 1,
                status =0,
                contents = null,
                system = null
        };

            tempBll.AddTemplateInfo(model, 1);
        }

        /// <summary>
        /// 控件信息为NULL
        /// </summary>
        [TestMethod]
        public void AddTemplateInfo_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "AddTemplateInfo_Test003");

            var model = new TemplateInfoModel();
            var template = new TemplateModel();

            template.templateName = "请假流程";
            template.description = "请假";
            template.defaultTitle = 0;
            template.categoryId = 1;
            template.status = 1;
            template.contents = "测试用模板";
            template.system = true;

            model.template = template;

            tempBll.AddTemplateInfo(model, 1);

            var temp = tempBll.GetTemplateInfoById(1);

            var controlList = new List<tblTemplateControl>();

            using (var db = new TargetNavigationDBEntities())
            {
                controlList = db.tblTemplateControl.Where(p => p.templateId == 1).ToList();
            }

            Assert.AreEqual(temp.template.templateId, 1);
            Assert.AreEqual(temp.template.templateName, template.templateName);
            Assert.AreEqual(temp.template.description, template.description);
            Assert.AreEqual(temp.template.defaultTitle, template.defaultTitle);
            Assert.AreEqual(temp.template.categoryId, template.categoryId);
            Assert.AreEqual(temp.template.status, template.status);
            Assert.AreEqual(temp.template.contents, template.contents);
            Assert.AreEqual(temp.template.system, template.system);

            Assert.AreEqual(controlList.Count, 0);
        }

        /// <summary>
        /// 控件信息为0件
        /// </summary>
        [TestMethod]
        public void AddTemplateInfo_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "AddTemplateInfo_Test004");

            var model = new TemplateInfoModel();
            var template = new TemplateModel();

            template.templateName = "请假流程";
            template.description = "请假";
            template.defaultTitle = 0;
            template.categoryId = 1;
            template.status = 1;
            template.contents = "测试用模板";
            template.system = true;

            model.template = template;
            model.controlInfo = new List<TemplateControlInfoModel>();

            tempBll.AddTemplateInfo(model, 1);
        }

        /// <summary>
        /// 模板信息、控件信息、明细公式、控件项目都有
        /// </summary>
        [TestMethod]
        public void AddTemplateInfo_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "AddTemplateInfo_Test005");

            var model = new TemplateInfoModel();

            #region 模板信息
            var template = new TemplateModel();

            template.templateName = "请假流程";
            template.description = "请假";
            template.defaultTitle = 0;
            template.categoryId = 1;
            template.status = 1;
            template.contents = "测试用模板";
            template.system = true;
            #endregion

            var controInfolList = new List<TemplateControlInfoModel>();

            #region 第一个控件（文本输入框）
            var controlInfo = new TemplateControlInfoModel();

            var control = new TemplateControlModel();
            control.controlId = "reason";
            control.controlType = 1;
            control.require = 1;
            control.title = "请假理由";
            control.size = 2;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);
            #endregion

            #region 第二个控件（单选框，带控件项目）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "type";
            control.controlType = 4;
            control.require = 1;
            control.title = "请假类型";
            control.size = 2;
            control.mutliSelect = 0;
            control.vertical = 0;
            control.description = "请选择请假类型";
            controlInfo.control = control;

            var itemList = new List<ControlItemModel>();

            var item = new ControlItemModel();
            item.controlId = "type";
            item.itemText = "事假";
            item.orderNum = 1;
            itemList.Add(item);

            item = new ControlItemModel();
            item.controlId = "type";
            item.itemText = "病假";
            item.orderNum = 2;
            itemList.Add(item);

            item = new ControlItemModel();
            item.controlId = "type";
            item.itemText = "婚假";
            item.orderNum = 3;
            itemList.Add(item);

            controlInfo.item = itemList;

            controInfolList.Add(controlInfo);
            #endregion

            #region 第三个控件（明细子表，带明细公式）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "detail";
            control.controlType = 16;
            control.defaultRowNum = 3;
            control.description = "申请事项明细";
            controlInfo.control = control;

            var formulaList = new List<DetailFormulaModel>();

            var formula = new DetailFormulaModel();
            formula.orderNum = 1;
            formula.detailControl = "detail";
            formula.controlId = "total";
            formula.displayText = "总价";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 2;
            formula.detailControl = "detail";
            formula.operate = "=";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 3;
            formula.detailControl = "detail";
            formula.controlId = "unit";
            formula.displayText = "单价";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 4;
            formula.detailControl = "detail";
            formula.operate = "*";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 5;
            formula.detailControl = "detail";
            formula.controlId = "quantity";
            formula.displayText = "数量";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 1;
            formula.detailControl = "detail";
            formula.controlId = "unit";
            formula.displayText = "单价";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 2;
            formula.detailControl = "detail";
            formula.operate = "+";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 3;
            formula.detailControl = "detail";
            formula.controlId = "quantity";
            formula.displayText = "数量";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 4;
            formula.detailControl = "detail";
            formula.operate = "=";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 5;
            formula.detailControl = "detail";
            formula.controlId = "total";
            formula.displayText = "总价";
            formulaList.Add(formula);

            controlInfo.formula = formulaList;

            controInfolList.Add(controlInfo);
            #endregion

            #region 第四个控件（明细子表子控件）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "unit";
            control.parentControl = "detail";
            control.controlType = 3;
            control.require = 1;
            control.title = "单价";
            control.size = 1;
            control.columnStatistics = 1;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);
            #endregion

            #region 第五个控件（明细子表子控件）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "quantity";
            control.parentControl = "detail";
            control.controlType = 2;
            control.require = 1;
            control.title = "数量";
            control.size = 1;
            control.columnStatistics = 1;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);
            #endregion

            #region 第六个控件（明细子表子控件）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "total";
            control.parentControl = "detail";
            control.controlType = 3;
            control.require = 1;
            control.title = "总价";
            control.size = 1;
            control.columnStatistics = 1;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);
            #endregion

            model.template = template;
            model.controlInfo = controInfolList;

            tempBll.AddTemplateInfo(model, 1);

            #region 确认模板信息

            var temp = tempBll.GetTemplateInfoById(1);

            Assert.AreEqual(temp.template.templateId, 1);
            Assert.AreEqual(temp.template.templateName, template.templateName);
            Assert.AreEqual(temp.template.description, template.description);
            Assert.AreEqual(temp.template.defaultTitle, template.defaultTitle);
            Assert.AreEqual(temp.template.categoryId, template.categoryId);
            Assert.AreEqual(temp.template.status, template.status);
            Assert.AreEqual(temp.template.contents, template.contents);
            Assert.AreEqual(temp.template.system, template.system);
            #endregion

            #region 确认控件信息

            var controlList = new List<tblTemplateControl>();

            using (var db = new TargetNavigationDBEntities())
            {
                controlList = db.tblTemplateControl.Where(p => p.templateId == 1).OrderBy(p => p.createTime).ToList();
            }

            Assert.AreEqual(controlList.Count, 6);
            Assert.AreEqual(controlList[0].controlId, "reason");
            Assert.AreEqual(controlList[0].parentControl, null);
            Assert.AreEqual(controlList[0].controlType, 1);
            Assert.AreEqual(controlList[0].require, 1);
            Assert.AreEqual(controlList[0].title, "请假理由");
            Assert.AreEqual(controlList[0].size, 2);
            Assert.AreEqual(controlList[0].mutliSelect, null);
            Assert.AreEqual(controlList[0].vertical, null);
            Assert.AreEqual(controlList[0].lineType, null);
            Assert.AreEqual(controlList[0].defaultRowNum, null);
            Assert.AreEqual(controlList[0].columnStatistics, null);
            Assert.AreEqual(controlList[0].color, null);
            Assert.AreEqual(controlList[0].description, null);
            #endregion

            #region 确认控件项目
            var controlItemList = new List<tblControlItem>();
            using (var db = new TargetNavigationDBEntities())
            {
                controlItemList = db.tblControlItem.Where(p => p.controlId == "type").OrderBy(p => p.orderNum).ToList();
            }

            Assert.AreEqual(controlItemList.Count, 3);
            Assert.AreEqual(controlItemList[0].itemId, 1);
            Assert.AreEqual(controlItemList[0].controlId, "type");
            Assert.AreEqual(controlItemList[0].templateId, 1);
            Assert.AreEqual(controlItemList[0].itemText, "事假");
            Assert.AreEqual(controlItemList[0].orderNum, 1);
            #endregion

            #region 确认明细公式
            var detailFormulaList = new List<tblDetailFormula>();
            using (var db = new TargetNavigationDBEntities())
            {
                detailFormulaList = db.tblDetailFormula.Where(p => p.detailControl == "detail").OrderBy(p => p.orderNum).OrderBy(p => p.formulaId).ToList();
            }

            Assert.AreEqual(detailFormulaList.Count, 10);
            Assert.AreEqual(detailFormulaList[0].controlId, "total");
            Assert.AreEqual(detailFormulaList[0].displayText, "总价");
            Assert.AreEqual(detailFormulaList[1].operate, "=");
            Assert.AreEqual(detailFormulaList[2].controlId, "unit");
            Assert.AreEqual(detailFormulaList[2].displayText, "单价");
            Assert.AreEqual(detailFormulaList[3].operate, "*");
            Assert.AreEqual(detailFormulaList[4].controlId, "quantity");
            Assert.AreEqual(detailFormulaList[4].displayText, "数量");
            Assert.AreEqual(detailFormulaList[5].formulaId, 2);
            #endregion
        }
        #endregion

        #region 模板编辑（UpdateTemplateInfo）

        /// <summary>
        /// 模板信息为NULL
        /// </summary>
        [TestMethod]
        public void UpdateTemplateInfo_Test001()
        {
            tempBll.UpdateTemplateInfo(null, 1);
        }

        /// <summary>
        /// 模板信息为NULL
        /// </summary>
        [TestMethod]
        public void UpdateTemplateInfo_Test002()
        {

            var model = new TemplateInfoModel();
            model.template = null;

            tempBll.UpdateTemplateInfo(model, 1);
        }

        /// <summary>
        /// 只更新模板信息
        /// </summary>
        [TestMethod]
        public void UpdateTemplateInfo_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "UpdateTemplateInfo_Test");

            var model = new TemplateInfoModel();

            var template = new TemplateModel();

            template.templateId = 2;
            template.templateName = "请假及明细";
            template.description = "明细";
            template.defaultTitle = 1;
            template.categoryId = 2;
            template.status = 1;
            template.contents = "模板";
            template.system = true;

            model.template = template;

            tempBll.UpdateTemplateInfo(model, 1);

            #region 确认模板信息

            var temp = tempBll.GetTemplateInfoById(2);

            Assert.AreEqual(temp.template.templateName, template.templateName);
            Assert.AreEqual(temp.template.description, template.description);
            Assert.AreEqual(temp.template.defaultTitle, template.defaultTitle);
            Assert.AreEqual(temp.template.categoryId, template.categoryId);
            Assert.AreEqual(temp.template.status, template.status);
            Assert.AreEqual(temp.template.contents, template.contents);
            Assert.AreEqual(temp.template.system, template.system);
            #endregion
        }

        /// <summary>
        /// 只更新模板信息
        /// </summary>
        [TestMethod]
        public void UpdateTemplateInfo_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "UpdateTemplateInfo_Test");

            var model = new TemplateInfoModel();

            #region 模板信息
            var template = new TemplateModel();

            template.templateId = 2;
            template.templateName = "请假及明细";
            template.description = "明细";
            template.defaultTitle = 0;
            template.categoryId = 2;
            template.status = 1;
            template.contents = "模板";
            template.system = true;

            model.template = template;
            #endregion

            var controInfolList = new List<TemplateControlInfoModel>();

            #region 更新明细子表（带明细公式）

            var controlInfo = new TemplateControlInfoModel();

            var control = new TemplateControlModel();
            control.controlId = "detail";
            control.controlType = 16;
            control.title = "明细";
            control.defaultRowNum = 1;
            controlInfo.control = control;

            var formulaList = new List<DetailFormulaModel>();

            var formula = new DetailFormulaModel();
            formula.orderNum = 1;
            formula.detailControl = "detail";
            formula.controlId = "unit";
            formula.displayText = "单价";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 2;
            formula.detailControl = "detail";
            formula.operate = "+";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 3;
            formula.detailControl = "detail";
            formula.controlId = "quantity";
            formula.displayText = "数量";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 4;
            formula.detailControl = "detail";
            formula.operate = "=";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 5;
            formula.detailControl = "detail";
            formula.controlId = "total";
            formula.displayText = "总价";
            formulaList.Add(formula);

            controlInfo.formula = formulaList;

            controInfolList.Add(controlInfo);

            #endregion

            #region 更新其他控件（带控件项目）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "type";
            control.controlType = 4;
            control.require = 1;
            control.title = "类型";
            control.size = 2;
            control.mutliSelect = 0;
            control.vertical = 1;
            control.description = "请选择请假类型";
            controlInfo.control = control;

            var itemList = new List<ControlItemModel>();

            var item = new ControlItemModel();
            item.itemId = 3;
            item.controlId = "type";
            item.itemText = "婚假（晚婚）";
            item.orderNum = 3;
            itemList.Add(item);

            controlInfo.item = itemList;

            item = new ControlItemModel();
            item.controlId = "type";
            item.itemText = "丧假";
            item.orderNum = 4;
            itemList.Add(item);

            controlInfo.item = itemList;

            controInfolList.Add(controlInfo);
            #endregion

            #region 添加明细子表（带明细公式）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "welfare";
            control.controlType = 16;
            control.defaultRowNum = 1;
            control.description = "福利明细";
            controlInfo.control = control;

            formulaList = new List<DetailFormulaModel>();

            formula = new DetailFormulaModel();
            formula.orderNum = 1;
            formula.detailControl = "welfare";
            formula.controlId = "count";
            formula.displayText = "合计";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 2;
            formula.detailControl = "welfare";
            formula.operate = "=";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 3;
            formula.detailControl = "welfare";
            formula.controlId = "price";
            formula.displayText = "价格";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 4;
            formula.detailControl = "welfare";
            formula.operate = "*";
            formulaList.Add(formula);

            formula = new DetailFormulaModel();
            formula.orderNum = 5;
            formula.detailControl = "welfare";
            formula.controlId = "num";
            formula.displayText = "个数";
            formulaList.Add(formula);

            controlInfo.formula = formulaList;

            controInfolList.Add(controlInfo);
            #endregion

            #region 添加明细子表子控件（带控件项目）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "gift";
            control.controlType = 6;
            control.parentControl = "welfare";
            control.require = 1;
            control.title = "礼品种类";
            control.size = 2;
            control.mutliSelect = 0;
            control.vertical = 0;
            control.description = "请选择礼品种类";
            controlInfo.control = control;

            itemList = new List<ControlItemModel>();

            item = new ControlItemModel();
            item.controlId = "gift";
            item.itemText = "粽子";
            item.orderNum = 1;
            itemList.Add(item);

            item = new ControlItemModel();
            item.controlId = "gift";
            item.itemText = "咸鸭蛋";
            item.orderNum = 2;
            itemList.Add(item);

            item = new ControlItemModel();
            item.controlId = "gift";
            item.itemText = "牛奶";
            item.orderNum = 3;
            itemList.Add(item);

            item = new ControlItemModel();
            item.controlId = "gift";
            item.itemText = "苹果";
            item.orderNum = 4;
            itemList.Add(item);

            controlInfo.item = itemList;

            controInfolList.Add(controlInfo);
            #endregion

            #region 添加明细子表子控件（其他）
            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "price";
            control.parentControl = "welfare";
            control.controlType = 3;
            control.require = 1;
            control.title = "价格";
            control.size = 1;
            control.columnStatistics = 1;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);

            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "num";
            control.parentControl = "welfare";
            control.controlType = 2;
            control.require = 1;
            control.title = "个数";
            control.size = 1;
            control.columnStatistics = 1;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);

            controlInfo = new TemplateControlInfoModel();

            control = new TemplateControlModel();
            control.controlId = "count";
            control.parentControl = "welfare";
            control.controlType = 3;
            control.require = 1;
            control.title = "合计";
            control.size = 1;
            control.columnStatistics = 1;
            controlInfo.control = control;

            controInfolList.Add(controlInfo);
            #endregion

            model.controlInfo = controInfolList;

            // 删除控件
            model.deleteControl = new string[] { "list" };
            // 删除控件项目
            model.deleteControlItem = new int[] { 2 };

            tempBll.UpdateTemplateInfo(model, 1);

            #region 确认更新后的模板信息

            var temp = tempBll.GetTemplateInfoById(2);

            Assert.AreEqual(temp.template.templateName, template.templateName);
            Assert.AreEqual(temp.template.description, template.description);
            Assert.AreEqual(temp.template.defaultTitle, template.defaultTitle);
            Assert.AreEqual(temp.template.categoryId, template.categoryId);
            Assert.AreEqual(temp.template.status, template.status);
            Assert.AreEqual(temp.template.contents, template.contents);
            Assert.AreEqual(temp.template.system, template.system);
            #endregion

            #region 确认更新后的控件信息
            var templateControlList = new List<tblTemplateControl>();

            using (var db = new TargetNavigationDBEntities())
            {
                templateControlList = db.tblTemplateControl.Where(p => p.templateId == 2).ToList();
            }

            Assert.AreEqual(templateControlList.Count, 11);
            #endregion

            #region 确认更新后的明细子表

            var detailControl = templateControlList.Where(p => p.controlId == "detail").FirstOrDefault();

            var detailFormulaList = new List<tblDetailFormula>();
            using (var db = new TargetNavigationDBEntities())
            {
                detailFormulaList = db.tblDetailFormula.Where(p => p.detailControl == "detail").OrderBy(p => p.orderNum).OrderBy(p => p.formulaId).ToList();
            }

            Assert.AreEqual(detailControl.title, "明细");
            Assert.AreEqual(detailControl.defaultRowNum, 1);
            Assert.AreEqual(detailControl.description, null);

            Assert.AreEqual(detailFormulaList.Count, 5);
            Assert.AreEqual(detailFormulaList[0].controlId, "unit");
            Assert.AreEqual(detailFormulaList[0].displayText, "单价");
            Assert.AreEqual(detailFormulaList[1].operate, "+");
            Assert.AreEqual(detailFormulaList[2].controlId, "quantity");
            Assert.AreEqual(detailFormulaList[2].displayText, "数量");
            Assert.AreEqual(detailFormulaList[3].operate, "=");
            Assert.AreEqual(detailFormulaList[4].controlId, "total");
            Assert.AreEqual(detailFormulaList[4].displayText, "总价");
            Assert.AreEqual(detailFormulaList[4].formulaId, 1);
            #endregion

            #region 确认更新后的其他控件

            var itemControl = templateControlList.Where(p => p.controlId == "type").FirstOrDefault();

            var controlItemList = new List<tblControlItem>();
            using (var db = new TargetNavigationDBEntities())
            {
                controlItemList = db.tblControlItem.Where(p => p.controlId == "type").OrderBy(p => p.orderNum).ToList();
            }

            Assert.AreEqual(itemControl.title, "类型");
            Assert.AreEqual(itemControl.vertical, 1);

            Assert.AreEqual(controlItemList.Count, 3);
            Assert.AreEqual(controlItemList[1].itemId, 3);
            Assert.AreEqual(controlItemList[1].controlId, "type");
            Assert.AreEqual(controlItemList[1].templateId, 2);
            Assert.AreEqual(controlItemList[1].itemText, "婚假（晚婚）");
            Assert.AreEqual(controlItemList[1].orderNum, 3);
            Assert.AreEqual(controlItemList[2].itemId, 7);
            Assert.AreEqual(controlItemList[2].controlId, "type");
            Assert.AreEqual(controlItemList[2].templateId, 2);
            Assert.AreEqual(controlItemList[2].itemText, "丧假");
            Assert.AreEqual(controlItemList[2].orderNum, 4);
            #endregion

            #region 确认添加的明细子表
            detailControl = templateControlList.Where(p => p.controlId == "welfare").FirstOrDefault();

            detailFormulaList = new List<tblDetailFormula>();
            using (var db = new TargetNavigationDBEntities())
            {
                detailFormulaList = db.tblDetailFormula.Where(p => p.detailControl == "welfare").OrderBy(p => p.orderNum).OrderBy(p => p.formulaId).ToList();
            }

            Assert.AreEqual(detailControl.controlId, "welfare");
            Assert.AreEqual(detailControl.title, null);
            Assert.AreEqual(detailControl.defaultRowNum, 1);
            Assert.AreEqual(detailControl.description, "福利明细");

            Assert.AreEqual(detailFormulaList.Count, 5);
            Assert.AreEqual(detailFormulaList[0].controlId, "count");
            Assert.AreEqual(detailFormulaList[0].displayText, "合计");
            Assert.AreEqual(detailFormulaList[1].operate, "=");
            Assert.AreEqual(detailFormulaList[2].controlId, "price");
            Assert.AreEqual(detailFormulaList[2].displayText, "价格");
            Assert.AreEqual(detailFormulaList[3].operate, "*");
            Assert.AreEqual(detailFormulaList[4].controlId, "num");
            Assert.AreEqual(detailFormulaList[4].displayText, "个数");
            Assert.AreEqual(detailFormulaList[4].formulaId, 2);
            #endregion

            #region 确认添加的名字子表子控件
            var childControl = templateControlList.Where(p => p.parentControl == "welfare").ToList();

            Assert.AreEqual(childControl.Count, 4);

            itemControl = templateControlList.Where(p => p.controlId == "gift").FirstOrDefault();

            controlItemList = new List<tblControlItem>();
            using (var db = new TargetNavigationDBEntities())
            {
                controlItemList = db.tblControlItem.Where(p => p.controlId == "gift").OrderBy(p => p.orderNum).ToList();
            }

            Assert.AreEqual(itemControl.title, "礼品种类");
            Assert.AreEqual(itemControl.vertical, 0);
            Assert.AreEqual(itemControl.description, "请选择礼品种类");

            Assert.AreEqual(controlItemList.Count, 4);
            Assert.AreEqual(controlItemList[0].itemId, 8);
            Assert.AreEqual(controlItemList[0].controlId, "gift");
            Assert.AreEqual(controlItemList[0].templateId, 2);
            Assert.AreEqual(controlItemList[0].itemText, "粽子");
            Assert.AreEqual(controlItemList[0].orderNum, 1);
            #endregion
        }
        #endregion

        #region 节点设置验证（CheckNode）
        /// <summary>
        /// 模板ID为NULL
        /// </summary>
        [TestMethod]
        public void CheckNode_Test001()
        {
            var result = tempBll.CheckNode(null);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 没有节点操作人
        /// </summary>
        [TestMethod]
        public void CheckNode_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckNode_Test002");

            var result = tempBll.CheckNode(1);            

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 状态为"无"的时候，节点操作人大于1个
        /// </summary>
        [TestMethod]
        public void CheckNode_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckNode_Test003");

            var result = tempBll.CheckNode(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 没有创建节点
        /// </summary>
        [TestMethod]
        public void CheckNode_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckNode_Test004");

            var result = tempBll.CheckNode(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 没有归档节点
        /// </summary>
        [TestMethod]
        public void CheckNode_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckNode_Test005");

            var result = tempBll.CheckNode(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 节点设置正确
        /// </summary>
        [TestMethod]
        public void CheckNode_Test006()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckNode_Test006");

            var result = tempBll.CheckNode(1);

            Assert.AreEqual(result, true);
        }
        #endregion

        #region 流程设置验证（CheckFlow）
        /// <summary>
        /// 模板ID为空
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test001()
        {
           var result = tempBll.CheckFlow(null);

           Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 入口为"归档"
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test002");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 出口为"创建"
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test003");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 入口节点类型为"提交"时，状态为"不通过"
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test004");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 出口节点类型为"归档"时，状态为"不通过"
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test005()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test005");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 入口节点为"审批"，只有通过，没有不通过
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test006()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test006");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 入口节点为"审批"，只有不通过，没有通过
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test007()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test007");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 提交节点没有同时存在于入口和出口节点中
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test008()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test008");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 提交节点没有同时存在于入口和出口节点中
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test009()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test009");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 入口节点中没有"创建"节点
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test010()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test010");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 出口节点中没有"归档"节点
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test011()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test011");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, false);
        }

        /// <summary>
        /// 流程设置正确
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test012()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("TemplateEditBLLTestData.xlsx", "CheckFlow_Test012");

            var result = tempBll.CheckFlow(1);

            Assert.AreEqual(result, true);
        }
        #endregion
    }
}
